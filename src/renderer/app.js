(() => {
  const D = window.DEMO;
  const api = window.launcher ?? { windowControl() {}, openExternal() {} };
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

  let signedIn = true;
  function renderAccount() {
    $('account').hidden = !signedIn;
    $('sign-in').hidden = signedIn;
    $('account-name').textContent = D.account.displayName;
    $('account-sub').textContent = `@${D.account.username}`;
    $('account-avatar').textContent = D.account.displayName
      .split(' ')
      .map((w) => w[0])
      .join('')
      .slice(0, 2);
  }
  $('sign-out').addEventListener('click', () => {
    signedIn = false;
    renderAccount();
    toast('Signed out.', 'log-out');
  });
  $('sign-in').addEventListener('click', () => {
    signedIn = true;
    renderAccount();
    toast('Demo: the launcher would open the RecFlare sign-in page.', 'log-in');
  });

  let busy = false;
  function renderGame() {
    const g = D.game;
    $('play').hidden = $('play-vr').hidden = !g.installed;
    $('install').hidden = g.installed;
    $('game-line').textContent = g.installed
      ? 'RecFlare is installed and up to date.'
      : `RecFlare isn't installed yet. It needs about ${g.sizeGb} GB.`;
    const facts = [
      ['Launcher', D.version],
      ['Game build', g.installed ? g.build : 'Not installed'],
      ['Patch', g.installed ? g.patch : 'None'],
      ['Size on disk', g.installed ? `${g.sizeGb} GB` : '0 GB'],
      ['Studio', `Version ${D.studio.version}`],
    ];
    $('facts').replaceChildren(...facts.flatMap(([k, v]) => [el('dt', '', k), el('dd', '', v)]));
    $('set-folder').textContent = g.folder;
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
  $('play').addEventListener('click', () =>
    gameJob('Checking for a patch', 900, () =>
      toast(
        D.settings.startInVr
          ? 'Demo: RecFlare would start in VR now.'
          : 'Demo: RecFlare would start now.',
        'play',
      ),
    ),
  );
  $('play-vr').addEventListener('click', () =>
    gameJob('Checking for a patch', 900, () => toast('Demo: RecFlare would start in VR now.', 'glasses')),
  );
  $('install').addEventListener('click', () =>
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
  $('studio-new').addEventListener('click', () => {
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
  $('studio-add').addEventListener('click', () =>
    toast('Demo: the launcher would ask for a project folder.', 'folder-plus'),
  );

  document.querySelectorAll('[data-setting]').forEach((box) => {
    box.checked = !!D.settings[box.dataset.setting];
    box.addEventListener('change', () => (D.settings[box.dataset.setting] = box.checked));
  });
  $('change-folder').addEventListener('click', () =>
    toast('Demo: the launcher would ask for a folder and move RecFlare there.', 'folder-open'),
  );
  $('verify').addEventListener('click', () => {
    show('play');
    void gameJob('Verifying files', 2500, () => toast('All files are fine.', 'check'));
  });
  $('uninstall').addEventListener('click', () => {
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

  document.body.dataset.view = 'play';
  renderAccount();
  renderGame();
  renderStudio();
  void slideshow();
  void hotRooms();
  renderStats();
  void liveStats();
  setInterval(liveStats, 60_000);
})();
