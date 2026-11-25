(() => {
  const D = window.DEMO;
  const bridge = window.sendPatchKitLauncher2ApiFuncRequest;
  async function call(fullname, args) {
    const reply = await bridge(fullname, args);
    if (reply?.execErrorInfo) {
      const error = new Error(reply.execErrorInfo.message || reply.execErrorInfo.typeFullname);
      error.type = reply.execErrorInfo.typeFullname;
      throw error;
    }
    return reply?.execResult;
  }
  const events = new Map();
  window.onRecFlareLauncherEvent?.((message) => events.get(message?.channel)?.(message.payload));
  const api = bridge
    ? {
        call,
        windowControl: (action) => call('RecFlareLauncher.windowControl', action),
        openExternal: (url) => call('RecFlareLauncher.openExternal', url),
        openFolder: (dir) => call('RecFlareLauncher.openFolder', dir),
        myRooms: () => call('RecFlareLauncher.roomsMine'),
        auth: {
          restore: () => call('RecFlareLauncher.authRestore'),
          start: () => call('RecFlareLauncher.authStart'),
          cancel: () => call('RecFlareLauncher.authCancel'),
          signOut: () => call('RecFlareLauncher.authSignout'),
          onChange: (fn) => events.set('auth:changed', fn),
        },
      }
    : { windowControl() {}, openExternal() {} };
  const $ = (id) => document.getElementById(id);

  function el(tag, cls, ...children) {
    const node = document.createElement(tag);
    if (cls) node.className = cls;
    for (const c of children) node.append(c);
    return node;
  }
  function icon(name) {
    const span = el('span');
    span.dataset.icon = name;
    paint(span);
    return span;
  }
  function paint(span) {
    const svg = window.ICONS?.[span.dataset.icon];
    if (!svg) return;
    span.replaceChildren(new DOMParser().parseFromString(svg, 'image/svg+xml').documentElement);
  }
  document.querySelectorAll('[data-icon]').forEach(paint);

  function toast(text, name = 'info') {
    const t = el('div', 'toast', icon(name), text);
    $('toasts').append(t);
    setTimeout(() => t.remove(), 4000);
  }

  function work(bar, pct, ms) {
    return new Promise((resolve) => {
      const start = performance.now();
      const tick = (now) => {
        const p = Math.min(1, (now - start) / ms);
        bar.style.width = `${p * 100}%`;
        if (pct) pct.textContent = `${Math.round(p * 100)}%`;
        if (p < 1) requestAnimationFrame(tick);
        else resolve();
      };
      requestAnimationFrame(tick);
    });
  }

  document.querySelectorAll('[data-win]').forEach((b) =>
    b.addEventListener('click', () => api.windowControl(b.dataset.win)),
  );
  document.querySelectorAll('[data-open]').forEach((b) =>
    b.addEventListener('click', () => api.openExternal(b.dataset.open)),
  );
  function show(view) {
    document.querySelectorAll('.view').forEach((v) => v.classList.toggle('active', v.id === `view-${view}`));
    document.querySelectorAll('.dock-link').forEach((b) => {
      if (b.dataset.view === view) b.setAttribute('aria-current', 'page');
      else b.removeAttribute('aria-current');
    });
    document.body.dataset.view = view;
    document.querySelector('.content').scrollTop = 0;
  }
  document.querySelectorAll('.dock-link').forEach((b) =>
    b.addEventListener('click', () => show(b.dataset.view)),
  );

  const auth = api.auth ?? null;
  let account = auth ? null : { ...D.account, profileImage: null, studio: true };
  let signIn = null;

  const initials = (name) =>
    String(name)
      .split(/\s+/)
      .filter(Boolean)
      .map((w) => w[0])
      .join('')
      .slice(0, 2)
      .toUpperCase();

  function gateStudio() {
    const allowed = account?.studio === true;
    document.querySelector('.dock-link[data-view="studio"]').hidden = !allowed;
    if (!allowed && document.body.dataset.view === 'studio') show('play');
  }

  let roomsFor;
  function renderAccount() {
    gateStudio();
    if (roomsFor !== (account?.accountId ?? account?.username ?? null)) {
      roomsFor = account?.accountId ?? account?.username ?? null;
      void loadMyRooms();
    }
    $('account').hidden = !account;
    $('sign-in').hidden = !!account || !!signIn;
    $('sign-in-wait').hidden = !signIn;
    if (signIn) $('sign-in-code').textContent = signIn.userCode.replace(/^(.{4})(.{4})$/, '$1-$2');
    if (!account) return;
    $('account-name').textContent = account.displayName;
    $('account-sub').textContent = account.username ? `@${account.username}` : '';
    const avatar = $('account-avatar');
    avatar.replaceChildren(initials(account.displayName));
    if (account.profileImage) {
      const img = new Image();
      img.alt = '';
      img.onload = () => avatar.replaceChildren(img);
      img.src = `https://img.recflare.net/${encodeURIComponent(account.profileImage).replace(/%2F/g, "/")}?width=128`;
    }
  }

  $('sign-in').addEventListener('click', async () => {
    if (!auth) {
      account = { ...D.account, profileImage: null, studio: true };
      renderAccount();
      return;
    }
    $('sign-in').disabled = true;
    try {
      signIn = await auth.start();
    } catch {
      toast("Couldn't reach RecFlare. Check your connection and try again.", 'circle-alert');
    }
    $('sign-in').disabled = false;
    renderAccount();
  });
  $('sign-in-reopen').addEventListener('click', () => signIn && api.openExternal(signIn.url));
  $('sign-in-cancel').addEventListener('click', () => {
    signIn = null;
    auth?.cancel();
    renderAccount();
  });
  $('sign-out').addEventListener('click', () => {
    account = null;
    auth?.signOut();
    renderAccount();
    toast('Signed out.', 'log-out');
  });
  auth?.onChange(({ account: signedIn, error }) => {
    signIn = null;
    if (signedIn) {
      account = signedIn;
      toast(`Signed in as ${signedIn.displayName}.`, 'check');
    } else if (error) toast(error, 'circle-alert');
    renderAccount();
  });
  auth?.restore().then((restored) => {
    if (restored && !account) {
      account = restored;
      renderAccount();
    }
  });

  let busy = false;
  function renderGame() {
    const g = D.game;
    $('play').hidden = $('play-vr').hidden = !g.installed;
    $('install').hidden = g.installed;
    $('game-update').hidden = !(g.installed && g.updateAvailable);
    $('game-line').textContent = g.installed
      ? g.updateAvailable
        ? `A RecFlare update is ready${g.latestPatch ? ` (patch ${g.latestPatch})` : ''}.`
        : 'RecFlare is installed and ready.'
      : `RecFlare isn't installed yet. It needs about ${g.sizeGb} GB.`;
    const facts = g.real
      ? [
          ['Game', g.installed ? 'RecFlare 2026' : 'Not installed'],
          ['Patch', g.installed ? g.patch : 'None'],
          ['Folder', g.installed ? g.folder : 'None'],
        ]
      : [
          ['Game build', g.installed ? g.build : 'Not installed'],
          ['Patch', g.installed ? g.patch : 'None'],
          ['Size on disk', g.installed ? `${g.sizeGb} GB` : '0 GB'],
          ['Studio', `Version ${D.studio.version}`],
        ];
    $('facts').replaceChildren(...facts.flatMap(([k, v]) => [el('dt', '', k), el('dd', '', v)]));
    $('set-folder').textContent = g.folder;
    $('about-version').textContent = `Version ${D.version}`;
    $('uninstall').disabled = $('verify').disabled = !g.installed;
  }
  async function gameJob(text, ms, done) {
    if (busy) return;
    busy = true;
    $('game-task').hidden = false;
    $('game-task-text').textContent = text;
    document.querySelectorAll('#play, #play-vr, #install').forEach((b) => (b.disabled = true));
    await work($('game-bar'), $('game-task-pct'), ms);
    $('game-task').hidden = true;
    document.querySelectorAll('#play, #play-vr, #install').forEach((b) => (b.disabled = false));
    busy = false;
    done();
  }
  if (!bridge) $('play').addEventListener('click', () =>
    gameJob('Checking for a patch', 900, () =>
      toast('Demo: RecFlare would start now.', 'play'),
    ),
  );
  if (!bridge) $('play-vr').addEventListener('click', () =>
    gameJob('Checking for a patch', 900, () => toast('Demo: RecFlare would start in VR now.', 'glasses')),
  );
  if (!bridge) $('install').addEventListener('click', () =>
    gameJob('Downloading RecFlare', 4500, () => {
      D.game.installed = true;
      renderGame();
      toast('RecFlare is installed.', 'check');
    }),
  );

  function renderStats() {
    const stats = [
      ['users', D.server.online, 'Online now'],
      ['trending-up', D.server.peak, 'Peak today'],
      ['activity', D.server.average, 'Average today'],
    ];
    $('stats').replaceChildren(
      ...stats.map(([name, value, label]) =>
        el('div', 'stat', icon(name), el('div', '', el('dd', '', value.toLocaleString()), el('dt', '', label))),
      ),
    );
  }

  async function liveStats() {
    try {
      const r = await (await fetch('https://www.recflare.net/api/stats/online?range=24h')).json();
      if (typeof r.online !== 'number') return;
      D.server.online = r.online;
      if (typeof r.peak === 'number') D.server.peak = r.peak;
      if (typeof r.average === 'number') D.server.average = Math.round(r.average);
      renderStats();
    } catch {
    }
  }

  const compact = (n) =>
    new Intl.NumberFormat('en', { notation: 'compact', maximumFractionDigits: 1 }).format(n || 0);
  const imageUrl = (name, width) =>
    `https://img.recflare.net/${name.split('/').map(encodeURIComponent).join('/')}?width=${width}`;

  function roomCard(r) {
    const counts = el(
      'div',
      'room-counts',
      el('span', '', icon('footprints'), r.visits),
      el('span', '', icon('heart'), r.cheers),
    );
    const art = el('div', 'room-art', counts);
    if (r.image) {
      const img = el('img', 'room-img');
      img.alt = '';
      img.loading = 'lazy';
      img.src = r.image;
      img.onerror = () => img.remove();
      art.prepend(img);
    } else art.style.setProperty('--hue', String(r.hue ?? 24));
    const card = el('li', 'room', art, el('div', 'room-name', `^${r.name}`), el('div', 'room-by', r.by ? `@${r.by}` : ''));
    card.tabIndex = 0;
    card.addEventListener('click', () =>
      api.openExternal(`https://www.recflare.net/rooms/${encodeURIComponent(r.name)}`),
    );
    return card;
  }

  async function hotRooms() {
    $('rooms').replaceChildren(...D.rooms.map(roomCard));
    try {
      const res = await fetch('https://rooms.recflare.net/rooms/hot?skip=0&take=8');
      const body = await res.json();
      const list = (Array.isArray(body.Results) ? body.Results : []).slice(0, 8);
      if (!list.length) return;
      const ids = [...new Set(list.map((x) => x.CreatorAccountId).filter((id) => id > 0))];
      const names = new Map();
      try {
        const people = await (
          await fetch(`https://accounts.recflare.net/account/bulk?id=${ids.join(',')}`)
        ).json();
        for (const p of Array.isArray(people) ? people : []) names.set(p.accountId, p.username);
      } catch {
      }
      $('rooms').replaceChildren(
        ...list.map((x) =>
          roomCard({
            name: x.Name,
            by: names.get(x.CreatorAccountId) ?? '',
            visits: compact(x.Stats?.VisitCount),
            cheers: compact(x.Stats?.CheerCount),
            image: x.ImageName ? imageUrl(x.ImageName, 512) : null,
          }),
        ),
      );
    } catch {
    }
  }

  async function roomCards(list) {
    const ids = [...new Set(list.map((x) => x.CreatorAccountId).filter((id) => id > 0))];
    const names = new Map();
    if (ids.length)
      try {
        const people = await (
          await fetch(`https://accounts.recflare.net/account/bulk?id=${ids.join(',')}`)
        ).json();
        for (const p of Array.isArray(people) ? people : []) names.set(p.accountId, p.username);
      } catch {
      }
    return list.map((x) =>
      roomCard({
        name: x.Name,
        by: names.get(x.CreatorAccountId) ?? '',
        visits: compact(x.Stats?.VisitCount),
        cheers: compact(x.Stats?.CheerCount),
        image: x.ImageName ? imageUrl(x.ImageName, 512) : null,
      }),
    );
  }

  async function showRow(block, list, listId) {
    $(block).hidden = list.length === 0;
    $(listId).replaceChildren(...(list.length ? await roomCards(list.slice(0, 8)) : []));
  }

  let carouselTimer = null;
  let carouselAt = 0;
  function renderFeatured(list) {
    clearInterval(carouselTimer);
    carouselAt = 0;
    $('featured-block').hidden = list.length === 0;
    if (!list.length) return;
    const slides = list.map((r) => {
      const slide = el('button', 'slide');
      if (r.ImageName) {
        const img = el('img', 'slide-img');
        img.alt = '';
        img.src = imageUrl(r.ImageName, 1024);
        img.onerror = () => img.remove();
        slide.append(img);
      }
      slide.append(el('span', 'slide-shade'), el('span', 'slide-name', `^${r.RoomName}`));
      slide.addEventListener('click', () =>
        api.openExternal(`https://www.recflare.net/rooms/${encodeURIComponent(r.RoomName)}`),
      );
      return slide;
    });
    $('featured').replaceChildren(...slides);
    const dots = list.map((_, i) => {
      const dot = el('button', 'carousel-dot');
      dot.setAttribute('aria-label', `Featured room ${i + 1}`);
      dot.addEventListener('click', () => goTo(i));
      return dot;
    });
    $('featured-dots').replaceChildren(...dots);
    $('featured-dots').hidden = list.length < 2;
    $('featured-prev').parentElement.hidden = list.length < 2;
    function goTo(i) {
      carouselAt = (i + slides.length) % slides.length;
      const track = $('featured');
      track.scrollLeft = slides[carouselAt].offsetLeft - track.offsetLeft;
      dots.forEach((d, j) => d.classList.toggle('on', j === carouselAt));
    }
    $('featured-prev').onclick = () => goTo(carouselAt - 1);
    $('featured-next').onclick = () => goTo(carouselAt + 1);
    goTo(0);
    if (slides.length > 1) carouselTimer = setInterval(() => goTo(carouselAt + 1), 7000);
  }

  async function loadMyRooms() {
    if (!account) {
      renderFeatured([]);
      $('favorites-block').hidden = $('cheered-block').hidden = true;
      return;
    }
    let mine = null;
    try {
      mine = api.myRooms ? await api.myRooms() : D.myRooms;
    } catch {
      mine = null;
    }
    if (!mine) return;
    renderFeatured(mine.featured ?? []);
    await showRow('favorites-block', mine.favorites ?? [], 'favorites');
    await showRow('cheered-block', mine.cheered ?? [], 'cheered');
  }

  function renderStudio() {
    const u = D.studio.unity;
    $('unity-dot').classList.toggle('off', !u.ready);
    $('unity-title').textContent = u.ready ? `Unity ${u.version} is ready` : 'Unity is not set up';
    $('unity-sub').textContent = u.ready ? u.path : 'Studio needs the Unity editor.';
    const list = D.studio.projects;
    $('projects').hidden = list.length === 0;
    $('projects-empty').hidden = list.length > 0;
    $('projects').replaceChildren(
      ...list.map((p) => {
        const open = el('button', 'btn primary sm', icon('external-link'), 'Open in Unity');
        open.addEventListener('click', () => toast(`Demo: Unity would open ${p.name}.`, 'box'));
        const folder = el('button', 'btn outline sm', icon('folder-open'), 'Folder');
        folder.addEventListener('click', () =>
          toast(`Demo: File Explorer would open ${p.path}.`, 'folder-open'),
        );
        const remove = el('button', 'btn ghost icon sm danger', icon('trash-2'));
        remove.setAttribute('aria-label', `Remove ${p.name}`);
        remove.title = 'Remove from the list';
        remove.addEventListener('click', () => {
          D.studio.projects = D.studio.projects.filter((x) => x.id !== p.id);
          renderStudio();
          toast(`${p.name} was removed from the list. Its folder stays on disk.`, 'trash-2');
        });
        return el(
          'li',
          'project',
          el('span', 'project-icon', icon('box')),
          el(
            'div',
            '',
            el('div', 'project-name', p.name),
            el('div', 'project-sub', p.path),
            el('div', 'project-sub', `Last edited ${p.edited}`),
          ),
          el('div', 'btn-row', open, folder, remove),
        );
      }),
    );
  }
  if (!bridge) $('studio-new').addEventListener('click', () => {
    $('new-project').hidden = false;
    $('new-name').focus();
  });
  $('new-cancel').addEventListener('click', () => ($('new-project').hidden = true));
  $('new-project').addEventListener('submit', (e) => {
    e.preventDefault();
    const name = $('new-name').value.trim();
    if (!name) return;
    D.studio.projects.unshift({
      id: `p${Date.now()}`,
      name,
      edited: 'just now',
      path: `D:\\RecFlareStudio\\${name.replace(/[^A-Za-z0-9]+/g, '')}`,
    });
    $('new-name').value = '';
    $('new-project').hidden = true;
    renderStudio();
    toast(`Created ${name}.`, 'check');
  });
  if (!bridge) $('studio-add').addEventListener('click', () =>
    toast('Demo: the launcher would ask for a project folder.', 'folder-plus'),
  );

  document.querySelectorAll('[data-setting]').forEach((box) => {
    box.checked = !!D.settings[box.dataset.setting];
    box.addEventListener('change', () => (D.settings[box.dataset.setting] = box.checked));
  });
  if (!bridge) $('change-folder').addEventListener('click', () =>
    toast('Demo: the launcher would ask for a folder and move RecFlare there.', 'folder-open'),
  );
  if (!bridge) $('verify').addEventListener('click', () => {
    show('play');
    void gameJob('Verifying files', 2500, () => toast('All files are fine.', 'check'));
  });
  if (!bridge) $('uninstall').addEventListener('click', () => {
    D.game.installed = false;
    renderGame();
    show('play');
    toast('RecFlare was removed (demo).', 'trash-2');
  });

  const WEB = 'https://www.recflare.net';
  function creditLink(text, path) {
    const b = el('button', 'credit-link', text);
    b.addEventListener('click', () => api.openExternal(WEB + path));
    return b;
  }
  function roomPath(id, name) {
    const bare = String(name ?? '').replace(/^\^/, '').trim();
    if (bare && !/^\d+$/.test(bare) && !/[/?#]/.test(bare)) return `/rooms/${encodeURIComponent(bare)}`;
    return `/rooms/${id ?? ''}`;
  }
  async function slideshow() {
    let shots = [];
    try {
      const res = await fetch('https://api.recflare.net/api/images/v1/slideshow?take=40');
      const body = await res.json();
      shots = (Array.isArray(body.Images) ? body.Images : []).filter(
        (i) => typeof i.ImageName === 'string' && i.ImageName,
      );
    } catch {
      return;
    }
    if (!shots.length) return;
    const img = $('hero-photo');
    let at = Math.floor(Math.random() * shots.length);
    const showShot = () => {
      const shot = shots[at];
      const next = new Image();
      next.onload = () => {
        img.src = next.src;
        img.hidden = false;
        img.style.animation = 'none';
        void img.offsetWidth;
        img.style.animation = '';
        const credit = $('photo-credit');
        credit.replaceChildren('Photo');
        if (shot.Username)
          credit.append(' by ', creditLink(`@${shot.Username}`, `/u/${encodeURIComponent(shot.Username)}`));
        if (shot.RoomName)
          credit.append(' in ', creditLink(`^${shot.RoomName}`, roomPath(shot.RoomId, shot.RoomName)));
      };
      next.src = `https://img.recflare.net/${shot.ImageName.split('/').map(encodeURIComponent).join('/')}?width=1024`;
    };
    showShot();
    if (shots.length > 1)
      setInterval(() => {
        at = (at + 1 + Math.floor(Math.random() * (shots.length - 1))) % shots.length;
        showShot();
      }, 6000);
  }

  window.RF = { api, el, icon, toast, show };

  const SETTINGS_KEY = 'recflare.settings';
  try {
    Object.assign(D.settings, JSON.parse(localStorage.getItem(SETTINGS_KEY) ?? '{}'));
  } catch {
  }
  const saveSettings = () => localStorage.setItem(SETTINGS_KEY, JSON.stringify(D.settings));

  function modal(title, lead, ...body) {
    const backdrop = el('div', 'modal-backdrop');
    const card = el('div', 'modal');
    card.setAttribute('role', 'dialog');
    card.setAttribute('aria-modal', 'true');
    card.append(el('h2', '', title), ...(lead ? [el('p', 'muted', lead)] : []), ...body);
    backdrop.append(card);
    document.body.append(backdrop);
    return { card, close: () => backdrop.remove() };
  }
  function switchRow(title, sub, checked, ...extra) {
    const box = el('input', 'switch');
    box.type = 'checkbox';
    box.checked = checked;
    return {
      box,
      row: el('label', 'row', el('div', '', el('div', 'row-title', title), el('div', 'row-sub', sub), ...extra), box),
    };
  }
  async function pickFolder(title) {
    const { selectDirDialogResult: r } = await call('RecroomStudioLauncher.displaySelectDirDialog', {
      selectDirDialogConfig: { title, message: title },
    });
    return r?.dirPath;
  }

  async function refreshGame() {
    if (!bridge) return;
    try {
      const s = await call('RecFlareLauncher.gameStatus');
      Object.assign(D.game, { installed: s.installed, folder: s.folder, patch: s.patch ?? 'None', real: true });
    } catch {
    }
    renderGame();
    void checkPatch();
  }
  async function checkPatch() {
    if (!bridge || !D.game.installed) return;
    try {
      const info = await call('RecFlareLauncher.gamePatchInfo');
      D.game.updateAvailable = !!info?.updateAvailable;
      D.game.latestPatch = info?.latest ?? null;
    } catch {
      D.game.updateAvailable = false;
    }
    renderGame();
  }
  async function updatePatch() {
    try {
      await call('RecFlareLauncher.gameRepair');
      toast('RecFlare is up to date.', 'check');
      await checkPatch();
    } catch {
    }
  }
  events.set('game:progress', (p) => {
    $('game-task').hidden = !p.running;
    document.querySelectorAll('#play, #play-vr, #install, #game-update').forEach((b) => (b.disabled = !!p.running));
    $('verify').disabled = $('uninstall').disabled = !!p.running || !D.game.installed;
    if (p.running) {
      $('game-task-text').textContent = p.text ?? 'Working…';
      $('game-task-pct').textContent = p.pct === undefined ? '' : `${Math.round(p.pct)}%`;
      $('game-bar').style.width = `${p.pct ?? 100}%`;
      $('game-bar').parentElement.classList.toggle('indeterminate', p.pct === undefined);
    } else {
      if (p.code === 'AV_BLOCKED') showBlocked();
      else if (p.error) toast(p.error, 'circle-alert');
      void refreshGame();
    }
  });

  let blockedOpen = false;
  function showBlocked() {
    if (blockedOpen) return;
    blockedOpen = true;
    const open = el('button', 'btn primary', icon('external-link'), 'Open Windows Security');
    const retry = el('button', 'btn outline', icon('refresh-cw'), 'Try again');
    const close = el('button', 'btn ghost', 'Close');
    const m = modal(
      'Windows Security blocked the RecFlare patch',
      'Windows flags the RecFlare patch (its Injector.exe), so it was not updated. This is the RecFlare game patch, not the launcher.',
      el(
        'p',
        'muted',
        'To use it anyway, open Windows Security, find the RecFlare patch under Protection history, and choose Allow on device. Then click Try again. Only do this if you trust the RecFlare patch.',
      ),
      el('div', 'btn-row', open, retry, close),
    );
    const done = () => {
      blockedOpen = false;
      m.close();
    };
    open.addEventListener('click', () => call('RecFlareLauncher.openWindowsSecurity').catch(() => {}));
    retry.addEventListener('click', () => {
      done();
      void updatePatch();
    });
    close.addEventListener('click', done);
  }

  async function realPlay(vr) {
    try {
      if (D.settings.autoUpdate && (await call('RecFlareLauncher.gameNeedsPatch'))) {
        toast('Updating the RecFlare patch first…', 'download');
        await call('RecFlareLauncher.gameRepair').catch(() => {});
      }
      await call('RecFlareLauncher.gamePlay', { vr });
      toast(vr ? 'Starting RecFlare in VR…' : 'Starting RecFlare…', vr ? 'glasses' : 'play');
      if (D.settings.closeOnPlay) setTimeout(() => api.windowControl('minimize'), 1500);
    } catch (e) {
      toast(e.message, 'circle-alert');
    }
  }

  async function realInstall() {
    let folder = D.game.folder || (await call('RecFlareLauncher.gameDefaultFolder'));
    const where = el('div', 'row-sub path', folder);
    const change = el('button', 'btn outline sm', icon('folder-open'), 'Change');
    change.type = 'button';
    change.addEventListener('click', async () => {
      const dir = await pickFolder('Install RecFlare to');
      if (dir) where.textContent = folder = dir;
    });
    const steam = switchRow(
      'I have Rec Room in my Steam library',
      'Download it from Steam. A Steam window opens to sign in; the launcher never sees your password.',
      false,
    );
    const user = el('input', 'input');
    user.placeholder = 'Steam username (blank = sign in with a QR code)';
    user.hidden = true;
    steam.box.addEventListener('change', () => (user.hidden = !steam.box.checked));
    const go = el('button', 'btn primary', icon('download'), 'Install');
    const cancel = el('button', 'btn ghost', 'Cancel');
    const m = modal(
      'Install RecFlare',
      'Without Steam, the game comes from the RecFlare community mirror, checked against its known checksum.',
      el('div', 'modal-rows', el('div', 'row', el('div', '', el('div', 'row-title', 'Folder'), where), change), steam.row),
      user,
      el('div', 'btn-row', go, cancel),
    );
    cancel.addEventListener('click', m.close);
    go.addEventListener('click', async () => {
      m.close();
      show('play');
      try {
        await call('RecFlareLauncher.gameInstall', {
          folder,
          source: steam.box.checked ? 'steam' : 'mirror',
          username: user.value,
        });
        toast('RecFlare is installed.', 'check');
      } catch {
      }
    });
  }

  if (bridge) {
    $('play').addEventListener('click', () => realPlay(false));
    $('play-vr').addEventListener('click', () => realPlay(true));
    $('install').addEventListener('click', () => realInstall());
    $('game-update').addEventListener('click', () => updatePatch());
    setInterval(() => void checkPatch(), 30 * 60 * 1000);
    $('change-folder').addEventListener('click', async () => {
      const dir = await pickFolder('Folder with RecFlare in it');
      if (!dir) return;
      try {
        await call('RecFlareLauncher.gameAdopt', dir);
        toast('Using RecFlare from that folder.', 'check');
      } catch {
        D.game.folder = dir;
        renderGame();
        toast('No RecFlare in that folder yet. Install will put it there.', 'folder-open');
      }
      void refreshGame();
    });
    $('verify').addEventListener('click', async () => {
      show('play');
      try {
        await call('RecFlareLauncher.gameRepair');
        toast('RecFlare is patched and pointed at RecFlare.', 'check');
      } catch {
      }
    });
    $('uninstall').addEventListener('click', () => {
      const yes = el('button', 'btn primary', icon('trash-2'), 'Uninstall');
      const no = el('button', 'btn ghost', 'Keep it');
      const m = modal(
        'Uninstall RecFlare?',
        `This deletes ${D.game.folder} and everything in it. You can install again later.`,
        el('div', 'btn-row', yes, no),
      );
      no.addEventListener('click', m.close);
      yes.addEventListener('click', async () => {
        m.close();
        try {
          await call('RecFlareLauncher.gameUninstall');
          toast('RecFlare was uninstalled.', 'trash-2');
        } catch (e) {
          toast(e.message, 'circle-alert');
        }
        void refreshGame();
      });
    });
    document.querySelectorAll('[data-setting]').forEach((box) => box.addEventListener('change', saveSettings));
  }

  let launcherUpdate = null;
  const demoUpdate = {
    available: true,
    current: D.version,
    latest: '0.2.1',
    canSelfUpdate: true,
    url: 'https://github.com/ScyteScripts/recflare-launcher/releases',
    demo: true,
  };
  async function launcherUpdates(manual = false) {
    if (!bridge) {
      launcherUpdate = demoUpdate;
      $('about-version').textContent = `Version ${D.version}`;
      $('check-update').hidden = false;
      $('about-update').hidden = false;
      $('about-update').textContent = `Version ${demoUpdate.latest} is ready to install.`;
      $('launcher-update-text').textContent = `Update to ${demoUpdate.latest}`;
      $('launcher-update').hidden = false;
      toast(`Launcher ${demoUpdate.latest} is ready.`, 'download');
      return;
    }
    try {
      const v = await call('RecFlareLauncher.launcherVersion');
      D.version = v.version;
      $('about-version').textContent = `Version ${v.version}`;
      $('check-update').hidden = false;
    } catch {
      return;
    }
    try {
      launcherUpdate = await call('RecFlareLauncher.launcherCheckUpdate');
    } catch (e) {
      if (manual) toast(e.message, 'circle-alert');
      return;
    }
    const about = $('about-update');
    about.hidden = false;
    if (!launcherUpdate.available) {
      about.textContent = 'You have the latest version.';
      $('launcher-update').hidden = true;
      if (manual) toast('The launcher is up to date.', 'check');
      return;
    }
    about.textContent = launcherUpdate.canSelfUpdate
      ? `Version ${launcherUpdate.latest} is ready to install.`
      : `Version ${launcherUpdate.latest} is out. Download it from the releases page.`;
    $('launcher-update-text').textContent = `Update to ${launcherUpdate.latest}`;
    $('launcher-update').hidden = false;
    if (!manual) toast(`Launcher ${launcherUpdate.latest} is ready.`, 'download');
  }
  async function installLauncherUpdate() {
    if (!launcherUpdate?.available) return;
    if (!launcherUpdate.canSelfUpdate) return api.openExternal(launcherUpdate.url);
    const yes = el('button', 'btn primary', icon('download'), 'Update and restart');
    const no = el('button', 'btn ghost', 'Later');
    const m = modal(
      `Update to ${launcherUpdate.latest}?`,
      'The launcher downloads the update, checks it, installs it and opens again. Your settings, game, Studio and projects stay.',
      el('div', 'btn-row', yes, no),
    );
    no.addEventListener('click', m.close);
    yes.addEventListener('click', async () => {
      m.close();
      if (launcherUpdate.demo) {
        for (let pct = 0; pct <= 100; pct += 4) {
          events.get('launcher:update')({ downloading: true, pct });
          await new Promise((r) => setTimeout(r, 120));
        }
        events.get('launcher:update')({ installing: true });
        await new Promise((r) => setTimeout(r, 1500));
        toast('Demo: the launcher would close, install 0.2.1 and open again now.', 'check');
        $('launcher-update-text').textContent = `Update to ${launcherUpdate.latest}`;
        return;
      }
      try {
        await call('RecFlareLauncher.launcherInstallUpdate');
      } catch (e) {
        toast(e.message, 'circle-alert');
        $('launcher-update-text').textContent = `Update to ${launcherUpdate.latest}`;
      }
    });
  }
  events.set('launcher:update', (p) => {
    if (p.downloading)
      $('launcher-update-text').textContent =
        p.pct === undefined ? 'Downloading update…' : `Downloading update ${Math.round(p.pct)}%`;
    if (p.installing) $('launcher-update-text').textContent = 'Installing update…';
  });
  $('launcher-update').addEventListener('click', () => installLauncherUpdate());
  $('check-update').addEventListener('click', () => launcherUpdates(true));
  if (bridge) setInterval(() => void launcherUpdates(), 6 * 60 * 60 * 1000);

  const MIGRATE_KEY = 'recflare.migrate.asked';
  const LOG_KEY = 'recflare.migrate.log';
  function migrationLog() {
    try {
      const list = JSON.parse(localStorage.getItem(LOG_KEY) ?? '[]');
      return Array.isArray(list) ? list : [];
    } catch {
      return [];
    }
  }
  function logMigration(text) {
    localStorage.setItem(LOG_KEY, JSON.stringify([{ at: Date.now(), text }, ...migrationLog()].slice(0, 50)));
    renderMigrations();
  }
  function renderMigrations() {
    const list = migrationLog();
    $('migrate-empty').hidden = list.length > 0;
    $('migrate-log').replaceChildren(
      ...list.map((item) =>
        el(
          'li',
          'migrate-item',
          el('span', 'migrate-icon', icon('check')),
          el('span', '', item.text),
          el('time', '', new Date(item.at).toLocaleString()),
        ),
      ),
    );
  }

  async function offerMigration(manual = false) {
    if (!bridge || (!manual && localStorage.getItem(MIGRATE_KEY))) return;
    if (manual) toast('Looking for the old RecFlare launchers…', 'refresh-cw');
    let found;
    try {
      found = await call('RecFlareLauncher.findOldInstalls');
    } catch {
      if (manual) toast("The scan didn't finish. Try again.", 'circle-alert');
      return;
    }
    const known = window.RFStudio?.knownProjects() ?? [];
    const projects = (found?.projects ?? []).filter((dir) => !known.includes(dir.toLowerCase()));
    const ours = D.game.installed ? String(D.game.folder).toLowerCase() : null;
    const oldGame = found?.game && found.game.dir.toLowerCase() !== ours ? found.game : null;
    const oldStudio = found?.studioLauncher ?? null;
    const oldTemplate =
      oldStudio?.template && (await window.RFStudio?.needsTemplate().catch(() => false)) ? oldStudio.template : null;
    if (!projects.length && !oldGame && !oldTemplate) {
      if (oldStudio?.uninstaller || (found?.game && found.game.dir.toLowerCase() === ours && found.game.registered))
        return offerCleanup(found.game?.registered ? found.game : null, oldStudio);
      if (manual) toast('Nothing left to move over.', 'check');
      return;
    }

    const rows = [];
    let gameRow = null;
    let projectRow = null;
    let templateRow = null;
    if (oldTemplate) {
      templateRow = switchRow(
        'Use the RecFlare Studio you already have',
        'Installed by the old Studio launcher. Nothing is downloaded again.',
        true,
        el('div', 'row-sub path', oldTemplate),
      );
      rows.push(templateRow.row);
    }
    if (oldGame) {
      gameRow = switchRow(
        'Use the RecFlare you already have',
        'Installed by the RecFlare installer. Nothing is downloaded again.',
        true,
        el('div', 'row-sub path', oldGame.dir),
      );
      rows.push(gameRow.row);
    }
    if (projects.length) {
      const names = projects.map((d) => d.split(/[\\/]/).filter(Boolean).pop());
      const more = names.length > 3 ? ` and ${names.length - 3} more` : '';
      projectRow = switchRow(
        `Bring over ${projects.length} Studio project${projects.length === 1 ? '' : 's'}`,
        `${names.slice(0, 3).join(', ')}${more}. They stay where they are; the Studio page lists them.`,
        true,
      );
      rows.push(projectRow.row);
    }
    const move = el('button', 'btn primary', icon('check'), 'Move them over');
    const later = el('button', 'btn ghost', 'Not now');
    const never = el('button', 'btn ghost', "Don't ask again");
    const m = modal(
      'Found your old RecFlare launcher',
      'Move your things over so you can carry on where you left off.',
      el('div', 'modal-rows', ...rows),
      el('div', 'btn-row', move, later, never),
    );
    const close = (remember) => {
      if (remember) localStorage.setItem(MIGRATE_KEY, '1');
      m.close();
    };
    later.addEventListener('click', () => close(false));
    never.addEventListener('click', () => close(true));
    move.addEventListener('click', async () => {
      const done = [];
      if (gameRow?.box.checked) {
        try {
          await call('RecFlareLauncher.gameAdopt', oldGame.dir);
          done.push('RecFlare');
          logMigration(`RecFlare at ${oldGame.dir} was moved over from the original RecFlare installer.`);
        } catch (e) {
          toast(e.message, 'circle-alert');
        }
      }
      if (templateRow?.box.checked) {
        try {
          await window.RFStudio.adoptTemplate(oldTemplate);
          done.push('RecFlare Studio');
          logMigration(`RecFlare Studio at ${oldTemplate} was moved over from the original Studio launcher.`);
        } catch (e) {
          toast(e.message, 'circle-alert');
        }
      }
      if (projectRow?.box.checked) {
        const added = window.RFStudio?.importProjects(projects) ?? 0;
        done.push(`${added} project${added === 1 ? '' : 's'}`);
        if (added)
          logMigration(
            `${added} Studio project${added === 1 ? ' was' : 's were'} moved over from the original Studio launcher.`,
          );
      }
      close(true);
      void refreshGame();
      if (done.length) toast(`Moved over: ${done.join(' and ')}.`, 'check');
      offerCleanup(gameRow?.box.checked ? oldGame : null, oldStudio);
    });
    move.focus();
  }

  function offerCleanup(oldGame, oldStudio) {
    if (!oldGame && !oldStudio?.uninstaller) return;
    const rows = [];
    let gameRow = null;
    let studioRow = null;
    if (oldGame) {
      gameRow = switchRow(
        'Old RecFlare installer',
        'Removes its Start menu and desktop shortcuts and its Apps & features entry. The game files stay: this launcher runs them now.',
        true,
      );
      rows.push(gameRow.row);
    }
    if (oldStudio?.uninstaller) {
      studioRow = switchRow(
        'Old RecFlare Studio Launcher',
        'Runs its uninstaller. Your Studio install and projects stay where they are.',
        true,
        el('div', 'row-sub path', oldStudio.dir),
      );
      rows.push(studioRow.row);
    }
    const remove = el('button', 'btn primary', icon('trash-2'), 'Remove them');
    const keep = el('button', 'btn ghost', 'Keep them');
    const m = modal(
      'Remove the old launchers?',
      'Everything is in this launcher now, so the old ones are not needed.',
      el('div', 'modal-rows', ...rows),
      el('div', 'btn-row', remove, keep),
    );
    keep.addEventListener('click', m.close);
    remove.addEventListener('click', async () => {
      m.close();
      const done = [];
      if (gameRow?.box.checked)
        await call('RecFlareLauncher.gameRetireOldInstaller')
          .then(() => {
            done.push('the old installer');
            logMigration('The original RecFlare installer was removed (its game files are kept and used here).');
          })
          .catch((e) => toast(e.message, 'circle-alert'));
      if (studioRow?.box.checked)
        await call('RecFlareLauncher.retireOldStudioLauncher')
          .then(() => {
            done.push('the old Studio launcher');
            logMigration('The original RecFlare Studio Launcher was uninstalled.');
          })
          .catch((e) => toast(e.message, 'circle-alert'));
      if (done.length) toast(`Removed ${done.join(' and ')}.`, 'check');
    });
  }

  if (bridge) $('migrate-scan').addEventListener('click', () => offerMigration(true));
  $('migrate-panel').hidden = !bridge;
  renderMigrations();
  document.body.dataset.view = 'play';
  renderAccount();
  renderGame();
  if (bridge && window.RFStudio) window.RFStudio.start();
  else renderStudio();
  void refreshGame();
  setTimeout(() => void offerMigration(), 1500);
  if (bridge) void launcherUpdates();

  let lastFocusCheck = Date.now();
  function onLauncherFocus() {
    if (!bridge || document.visibilityState === 'hidden' || Date.now() - lastFocusCheck < 60 * 1000) return;
    lastFocusCheck = Date.now();
    if (account?.studio === true) window.RFStudio?.checkNow();
    void checkPatch();
  }
  window.addEventListener('focus', onLauncherFocus);
  document.addEventListener('visibilitychange', onLauncherFocus);
  if (!bridge) setTimeout(() => void launcherUpdates(), 2500);
  void slideshow();
  void hotRooms();
  renderStats();
  void liveStats();
  setInterval(liveStats, 60_000);
})();
