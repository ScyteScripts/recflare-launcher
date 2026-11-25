(() => {
  const $ = (id) => document.getElementById(id);
  const PK = 'PatchKitLauncher2.';
  const RS = 'RecroomStudioLauncher.';
  const TASK = { update: 'update-task', repair: 'repair-task', move: 'move-task', uninstall: 'uninstall-task' };
  const UNITY_GB = { '2021.3.29f1': 5.1, '2021.3.30f1': 5.1 };
  const UNITY_MARGIN_GB = 0.5;
  const LIST_KEY = 'recflare.studio.projects';

  let RF;
  let call;
  const st = {
    appId: undefined,
    branchId: undefined,
    password: undefined,
    installPath: undefined,
    branchState: undefined,
    task: undefined,
    taskId: undefined,
    lastSeenTaskId: undefined,
    latest: undefined,
    latestAt: 0,
    hub: undefined,
    editor: undefined,
    discoveryError: false,
    lastProject: undefined,
    busy: false,
    stepKey: '',
  };

  const isError = (e, name) => e?.type === PK + name || e?.type === RS + name;
  const lastTaskFailed = (s) => s?.lastFinishedDataTaskInfo?.errorTypeName !== undefined;
  const winPath = (p) => (p ? String(p).replace(/\//g, '\\') : p);
  const unixPath = (p) => (p ? String(p).replace(/\\/g, '/') : p);
  const sleep = (ms) => new Promise((r) => setTimeout(r, ms));
  const timeout = (promise, ms) =>
    Promise.race([promise, sleep(ms).then(() => Promise.reject(new Error('Unity discovery timed out')))]);

  function el(tag, cls, ...children) {
    return RF.el(tag, cls, ...children);
  }
  function button(label, iconName, cls, onClick) {
    const b = el('button', `btn ${cls}`, ...(iconName ? [RF.icon(iconName)] : []), label);
    b.type = 'button';
    b.addEventListener('click', async () => {
      if (b.disabled) return;
      b.disabled = true;
      try {
        await onClick();
      } finally {
        b.disabled = false;
      }
    });
    return b;
  }
  function progress(pct) {
    const bar = el('div', 'progress-bar');
    bar.style.width = `${Math.max(0, Math.min(100, pct ?? 0))}%`;
    const track = el('div', 'progress', bar);
    if (pct === undefined) track.classList.add('indeterminate');
    return track;
  }
  function fail(message) {
    RF.toast(message, 'circle-alert');
  }
  function step(key, build) {
    if (st.stepKey === key) return false;
    st.stepKey = key;
    const card = $('studio-step');
    card.hidden = !build;
    card.replaceChildren(...(build ? build() : []));
    return true;
  }
  function status(dotOk, title, sub, ...actions) {
    $('unity-dot').classList.toggle('off', !dotOk);
    $('unity-title').textContent = title;
    $('unity-sub').textContent = sub ?? '';
    $('studio-status-actions').replaceChildren(...actions);
  }
  function setReady(ready) {
    $('studio-new').disabled = $('studio-add').disabled = !ready;
    $('projects-block').hidden = !ready;
  }

  async function selfUpdate() {
    try {
      if (await call(PK + 'fetchDoesCheckUpdateTaskExist')) return;
      let update;
      try {
        update = await call(PK + 'fetchUpdateState');
      } catch (e) {
        if (isError(e, 'UpdateDoesNotExist')) return;
        throw e;
      }
      if (!update.isDownloaded) {
        let id;
        try {
          id = await call(PK + 'fetchOngoingDownloadUpdateTaskId');
        } catch (e) {
          if (!isError(e, 'OngoingDownloadUpdateTaskDoesNotExist')) throw e;
          id = (await call(PK + 'startDownloadUpdateTask')).downloadUpdateTaskId;
        }
        RF.toast('Downloading a launcher update…', 'download');
        for (;;) {
          const s = await call(PK + 'fetchDownloadUpdateTaskState', { downloadUpdateTaskId: id });
          if (s?.isFinished || s?.progress?.processedBytesCount >= s?.progress?.totalBytesCount) break;
          await sleep(1000);
        }
      }
      if (!(await call(PK + 'fetchDoesApplyUpdateTaskExist'))) await call(PK + 'startApplyUpdateTask');
      RF.toast('Applying the launcher update…', 'download');
    } catch {
    }
  }

  async function resolveIds() {
    while (st.appId === undefined) {
      try {
        const r = await call(PK + 'fetchAppsInfoQueryResult', { appsInfoQueryConfig: {} });
        st.appId = Object.keys(r?.appsInfo ?? {})[0];
      } catch {
      }
      if (st.appId === undefined) await sleep(2000);
    }
    while (st.branchId === undefined) {
      try {
        st.branchId = await call(RS + 'fetchRecRoomStudioDefaultBranchId');
      } catch {
      }
      if (st.branchId === undefined) await sleep(1000);
    }
  }
  const ids = () => ({ appId: st.appId, appBranchId: st.branchId });
  const credentials = () => ({ appBranchDownloadingCredentials: { password: st.password } });

  async function startUpdate() {
    try {
      await call(PK + 'startAppBranchUpdateTask', { ...ids(), ...credentials() });
    } catch {
      fail('Cannot install the app right now. Please try again.');
    }
  }
  async function startRepair() {
    try {
      await call(PK + 'startAppBranchRepairTask', { ...ids(), ...credentials() });
    } catch {
      fail(
        'Studio repair could not start. Close Unity, check your connection and file permissions, then retry. Details are in the launcher logs/routing-repair.json and logs/libpkapps folders.',
      );
    }
  }
  async function cancelTask() {
    if (st.taskId === undefined || !st.task) return;
    const repair = st.task.type === TASK.repair;
    await call(PK + (repair ? 'cancelAppBranchRepairTask' : 'cancelAppBranchUpdateTask'), {
      ...ids(),
      [repair ? 'appBranchRepairTaskId' : 'appBranchUpdateTaskId']: st.taskId,
    }).catch(() => fail('That task could not be cancelled.'));
  }

  function reportFinished(task) {
    const e = task?.errorTypeName;
    if (!e || /Cancelled$/.test(e)) return;
    if (/NotHaveEnoughFreeSpace/.test(e)) return fail('Out of free disk space.');
    if (e !== 'FatalError') return;
    const byType = {
      [TASK.update]: 'Cannot check for the app updates right now. Please try again later.',
      [TASK.repair]:
        'Studio repair could not complete. Check your connection and file permissions. Details are in the launcher logs/routing-repair.json and logs/libpkapps folders.',
      [TASK.move]: 'Cannot move the app right now. Please try again later.',
      [TASK.uninstall]: 'Cannot uninstall the app right now. Please try again later.',
    };
    if (byType[task.type]) fail(byType[task.type]);
  }

  async function register(dir) {
    if (!dir) return false;
    try {
      await call(PK + 'registerAppBranch', {
        ...ids(),
        appBranchRootDirPath: dir,
        isAppBranchNotEmptyRootDirAllowed: false,
      });
      return true;
    } catch (e) {
      if (isError(e, 'AppBranchIsAlreadyRegistered')) return true;
      if (isError(e, 'AppBranchRootDirIsConflictingWithOtherFileSystemEntry'))
        fail('Cannot install the app in this location. Please select different one.');
      else if (isError(e, 'AppBranchRootDirIsNotEmpty'))
        fail('Cannot install the app in this location. The folder is not empty. Please select an empty folder.');
      else fail('Cannot install the app right now. Please try again.');
      return false;
    }
  }
  async function install(dir) {
    if (await register(dir)) {
      await startUpdate();
      st.latestAt = Date.now();
    }
  }

  async function installPathStep() {
    if (st.installPath === undefined) {
      try {
        const info = await call(PK + 'fetchAppBranchInfo', ids());
        st.installPath = unixPath(info?.defaultRootDirPath);
      } catch {
      }
    }
    const input = el('input', 'input path');
    input.readOnly = true;
    step(`path:${st.installPath}`, () => {
      input.value = winPath(st.installPath) ?? '';
      return [
        el('h2', '', 'Install RecFlare Studio'),
        el('p', 'muted', 'RecFlare Studio will be installed to:'),
        input,
        el(
          'div',
          'btn-row',
          button('Install now', 'download', 'primary', () => install(winPath(st.installPath))),
          button('Change folder', 'folder-open', 'outline', async () => {
            const { selectAppBranchRootDirDialogResult: r } = await call(
              PK + 'displaySelectAppBranchRootDirDialog',
              {
                selectAppBranchRootDirDialogConfig: {
                  ...ids(),
                  message: 'Select Install Path',
                  title: 'Select Install Path',
                  overrideDefaultDirPath: winPath(st.installPath),
                },
              },
            );
            if (r?.appBranchSelectedRootDirPath === undefined) return;
            st.installPath = unixPath(r.appBranchSelectedRootDirPath);
            st.stepKey = '';
            tick();
          }),
        ),
      ];
    });
  }

  function taskStep(task) {
    const labels = {
      [TASK.update]: st.branchState?.installedVersionId ? 'Updating RecFlare Studio…' : 'Installing RecFlare Studio…',
      [TASK.repair]: 'Repairing RecFlare Studio…',
      [TASK.move]: 'Moving RecFlare Studio…',
      [TASK.uninstall]: 'Uninstalling RecFlare Studio…',
    };
    const p = task.progress;
    const pct = p && p.totalBytesCount > 0 ? (p.processedBytesCount / p.totalBytesCount) * 100 : undefined;
    const mb = (n) => (n / 1024 / 1024).toFixed(1);
    const fresh = step(`task:${st.taskId}`, () => {
      const cancellable = task.type === TASK.update || task.type === TASK.repair;
      return [
        el('h2', '', labels[task.type] ?? 'Working…'),
        el('div', 'task', el('div', 'task-head', el('span', '', ''), el('span', '', '')), progress(pct)),
        el('div', 'btn-row', ...(cancellable ? [button('Cancel', 'x', 'ghost', cancelTask)] : [])),
      ];
    });
    const card = $('studio-step');
    const bar = card.querySelector('.progress-bar');
    if (bar) {
      bar.style.width = `${pct ?? 0}%`;
      bar.parentElement.classList.toggle('indeterminate', pct === undefined);
    }
    const head = card.querySelectorAll('.task-head span');
    if (head.length === 2) {
      head[0].textContent = p && p.totalBytesCount > 0 ? `${mb(p.processedBytesCount)} / ${mb(p.totalBytesCount)} MB` : '';
      head[1].textContent = pct === undefined ? '' : `${Math.round(pct)}%`;
    }
    return fresh;
  }

  function repairStep(failedRepair) {
    step(failedRepair ? 'repair-failed' : 'repair', () => [
      el('h2', '', failedRepair ? 'Studio repair stopped' : 'Studio needs repairing'),
      el(
        'p',
        'muted',
        failedRepair
          ? 'The last repair did not finish. Resume it to restore the Studio files.'
          : 'Some Studio files are missing or changed. Repair downloads them again.',
      ),
      el('div', 'btn-row', button(failedRepair ? 'Resume' : 'Repair', 'refresh-cw', 'primary', startRepair)),
    ]);
  }
  function resumeStep(info) {
    const repair = info.type === TASK.repair;
    step(`resume:${info.type}`, () => [
      el('h2', '', repair ? 'Studio repair stopped' : 'Studio install stopped'),
      el('p', 'muted', 'The last task did not finish. Resume it to carry on where it stopped.'),
      el('div', 'btn-row', button('Resume', 'play', 'primary', repair ? startRepair : startUpdate)),
    ]);
  }

  async function checkHub() {
    st.hub = { checking: true };
    try {
      const installed = await call(RS + 'fetchIsUnityHubInstalled', {});
      if (installed) {
        const broken = await call(RS + 'fetchIsUnityHubInstallBroken', {});
        st.hub = { installed: true, broken };
      } else {
        st.hub = { installed: false, installing: true };
        tick();
        try {
          await call(RS + 'installUnityHub', {});
          const ok = await call(RS + 'fetchIsUnityHubInstalled', {});
          st.hub = ok ? { installed: true, broken: false } : { installed: false, failed: true };
        } catch {
          st.hub = { installed: false, failed: true };
        }
      }
    } catch {
      st.hub = { installed: false, failed: true };
    }
    tick();
  }
  function hubStep() {
    const h = st.hub;
    if (!h || h.checking) return step('hub-checking', () => [el('h2', '', 'Searching for Unity Hub…'), progress()]);
    if (h.installing) return step('hub-installing', () => [el('h2', '', 'Installing Unity Hub…'), progress()]);
    if (h.failed)
      return step('hub-failed', () => [
        el('h2', '', 'Failed to install Unity Hub'),
        el('p', 'muted', 'Install Unity Hub yourself, then check again.'),
        el(
          'div',
          'btn-row',
          button('Download Unity Hub', 'external-link', 'primary', () =>
            RF.api.openExternal('https://unity.com/download'),
          ),
          button('Check again', 'refresh-cw', 'outline', checkHub),
        ),
      ]);
    if (h.broken)
      return step('hub-broken', () => [
        el('h2', '', 'Unity Hub needs reinstalling'),
        el(
          'p',
          'muted',
          "Unity Hub installation registry not found. Select 'Download Unity Hub' and allow the Registry Editor to make changes, then install Unity Hub manually to proceed.",
        ),
        el(
          'div',
          'btn-row',
          button('Download Unity Hub', 'external-link', 'primary', async () => {
            await call(RS + 'removeUnityHubRegKey', {}).catch(() => {});
            await RF.api.openExternal('https://unity.com/download');
          }),
          button("I've installed Unity Hub. Check again", 'refresh-cw', 'outline', checkHub),
        ),
      ]);
    return false;
  }

  async function roomFor(dir) {
    const v = st.editor?.id?.version;
    if (!v || !(v in UNITY_GB)) return true;
    const free = await call(RS + 'getDiskSpaceForPath', { installPath: dir });
    return free > (UNITY_GB[v] + UNITY_MARGIN_GB) * 1024 ** 3;
  }
  async function checkEditor() {
    st.discoveryError = false;
    st.editor = { checking: true };
    tick();
    try {
      const id = await timeout(call(RS + 'fetchRecRoomStudioTemplateUnityEditorId', ids()), 15000);
      st.editor = { id, checking: true };
      const [installed, broken, hubPath] = await Promise.all([
        timeout(call(RS + 'fetchIsUnityEditorInstalled', { unityEditorId: id }), 15000),
        timeout(call(RS + 'fetchIsUnityEditorInstallBroken', { unityEditorId: id }), 15000),
        timeout(call(RS + 'fetchCurrentUnityHubInstallPath', {}), 15000),
      ]);
      const uninstaller = await call(RS + 'fetchUnityUninstallerLocation', { unityEditorId: id }).catch(
        () => undefined,
      );
      st.editor = { id, installed, broken, uninstaller, path: hubPath };
      st.editor.roomy = await roomFor(hubPath).catch(() => true);
    } catch {
      st.discoveryError = true;
    }
    tick();
  }
  async function installEditor() {
    const e = st.editor;
    if (!e?.id || e.installed || e.installing) return;
    e.installing = true;
    e.output = 'Starting Unity installation';
    tick();
    try {
      await call(RS + 'startInstallUnityEditorTask', { unityEditorId: e.id, installPath: e.path });
    } catch {
      e.installing = false;
      e.failed = true;
      tick();
      return;
    }
    for (;;) {
      await sleep(1000);
      const s = await call(RS + 'fetchOngoingInstallUnityEditorTaskState').catch(() => undefined);
      if (s === undefined) break;
      if (s.errorTypeName !== undefined) {
        e.output = `Unity installation failed with error ${s.errorTypeName}`;
        e.installing = false;
        e.failed = true;
        break;
      }
      if (s.isFinished) {
        e.output = 'Unity installation complete';
        e.installing = false;
        e.installed = true;
        break;
      }
      if (String(s.processOutput ?? '').trim()) e.output = s.processOutput;
      const out = $('studio-step').querySelector('.unity-output');
      if (out) out.textContent = e.output;
    }
    st.stepKey = '';
    tick();
  }
  function editorStep() {
    const e = st.editor;
    if (st.discoveryError)
      return step('editor-error', () => [
        el('h2', '', 'Unity could not be checked'),
        el('p', 'muted', 'Looking for the Unity editor took too long. Close Unity Hub if it is stuck, then retry.'),
        el('div', 'btn-row', button('Retry', 'refresh-cw', 'primary', checkEditor)),
      ]);
    if (!e || e.checking)
      return step(`editor-checking:${e?.id?.version ?? ''}`, () => [
        el('h2', '', e?.id ? `Searching for Unity ${e.id.version}…` : 'Searching for Unity…'),
        progress(),
      ]);
    const v = e.id.version;
    if (e.installed && e.broken)
      return step('editor-broken', () => [
        el('h2', '', `Unity ${v} is damaged`),
        el(
          'p',
          'muted',
          "Detected missing/corrupt files in the Unity editor installation. Please uninstall and reinstall the editor. This often happens if you move the Unity files after installation, so once you reinstall, don't move anything!",
        ),
        el(
          'div',
          'btn-row',
          ...(e.uninstaller !== undefined
            ? [
                button('Fix', 'wrench', 'primary', async () => {
                  await call(RS + 'uninstallUnityEditor', { unityEditorId: e.id }).catch(() =>
                    fail('Unity could not be uninstalled.'),
                  );
                  e.installed = false;
                  e.broken = false;
                  e.failed = false;
                  st.stepKey = '';
                  tick();
                }),
              ]
            : []),
        ),
      ]);
    if (e.installed) return false;
    if (e.installing)
      return step('editor-installing', () => [
        el('h2', '', `Installing Unity ${v}…`),
        progress(),
        el('p', 'muted unity-output', e.output ?? ''),
      ]);
    if (e.failed)
      return step('editor-failed', () => [
        el('h2', '', `Failed to install Unity ${v}`),
        el('p', 'muted', e.output ?? ''),
        el('div', 'btn-row', button('Try again', 'refresh-cw', 'primary', checkEditor)),
      ]);
    const need = UNITY_GB[v] !== undefined ? UNITY_GB[v] + UNITY_MARGIN_GB : undefined;
    return step(`editor-install:${e.path}:${e.roomy}`, () => {
      const input = el('input', 'input path');
      input.readOnly = true;
      input.value = winPath(e.path) ?? '';
      const go = button('Install now', 'download', 'primary', installEditor);
      go.disabled = !e.roomy;
      return [
        el('h2', '', `Install Unity ${v}`),
        el('p', 'muted', 'Studio needs this Unity editor. It will be installed to:'),
        input,
        ...(!e.roomy && need
          ? [el('p', 'warn', `Not enough space. The Unity Editor requires at least ${need}GB of available disk space.`)]
          : []),
        el(
          'div',
          'btn-row',
          go,
          button('Change location', 'folder-open', 'outline', async () => {
            const { selectDirDialogResult: r } = await call(RS + 'displaySelectDirDialog', {
              selectDirDialogConfig: {
                message: 'Select Unity Editor Install Location',
                title: 'Select Unity Editor Install Location',
              },
            });
            if (r?.dirPath === undefined) return;
            e.path = r.dirPath;
            e.roomy = await roomFor(r.dirPath).catch(() => true);
            st.stepKey = '';
            tick();
          }),
        ),
      ];
    });
  }

  function projects() {
    try {
      const list = JSON.parse(localStorage.getItem(LIST_KEY) ?? '[]');
      return Array.isArray(list) ? list.filter((p) => typeof p?.path === 'string') : [];
    } catch {
      return [];
    }
  }
  function saveProjects(list) {
    localStorage.setItem(LIST_KEY, JSON.stringify(list.slice(0, 30)));
  }
  function remember(dir) {
    const name = String(dir).split(/[\\/]/).filter(Boolean).pop() ?? dir;
    saveProjects([{ path: dir, name, openedAt: Date.now() }, ...projects().filter((p) => p.path !== dir)]);
    renderProjects();
  }
  function ago(ms) {
    const s = Math.round((Date.now() - ms) / 1000);
    if (s < 60) return 'just now';
    const m = Math.round(s / 60);
    if (m < 60) return `${m} minute${m === 1 ? '' : 's'} ago`;
    const h = Math.round(m / 60);
    if (h < 24) return `${h} hour${h === 1 ? '' : 's'} ago`;
    const d = Math.round(h / 24);
    return `${d} day${d === 1 ? '' : 's'} ago`;
  }
  async function openProject(dir) {
    try {
      const unityEditorId = await call(RS + 'fetchUnityProjectUnityEditorId', { unityProjectDirPath: dir });
      await call(RS + 'openUnityProject', { unityEditorId, unityProjectDirPath: dir });
      remember(dir);
      RF.toast('Opening the project in Unity…', 'box');
    } catch (e) {
      fail(
        isError(e, 'UnityProjectDoesNotExist')
          ? 'That folder is not a Unity project any more.'
          : e?.message ||
              'The launcher could not complete the project request. Check the template installation and Unity editor, then try again.',
      );
    }
  }
  function renderProjects() {
    const list = projects();
    if (st.lastProject && !list.some((p) => p.path === st.lastProject))
      list.push({ path: st.lastProject, name: String(st.lastProject).split(/[\\/]/).filter(Boolean).pop() });
    $('projects').hidden = list.length === 0;
    $('projects-empty').hidden = list.length > 0;
    $('projects').replaceChildren(
      ...list.map((p) => {
        const remove = button('', 'trash-2', 'ghost icon sm danger', () => {
          saveProjects(projects().filter((x) => x.path !== p.path));
          if (p.path === st.lastProject) st.lastProject = undefined;
          renderProjects();
          RF.toast(`${p.name} was removed from the list. Its folder stays on disk.`, 'trash-2');
        });
        remove.setAttribute('aria-label', `Remove ${p.name}`);
        remove.title = 'Remove from the list';
        return el(
          'li',
          'project',
          el('span', 'project-icon', RF.icon('box')),
          el(
            'div',
            '',
            el('div', 'project-name', p.name),
            el('div', 'project-sub', winPath(p.path)),
            el('div', 'project-sub', p.openedAt ? `Opened ${ago(p.openedAt)}` : 'Your last Studio project'),
          ),
          el(
            'div',
            'btn-row',
            button('Open in Unity', 'external-link', 'primary sm', () => openProject(p.path)),
            button('Folder', 'folder-open', 'outline sm', () =>
              RF.api.openFolder(winPath(p.path)).catch((e) => fail(e.message)),
            ),
            remove,
          ),
        );
      }),
    );
  }
  async function newProject() {
    const { selectDirDialogResult: r } = await call(RS + 'displaySelectDirDialog', {
      selectDirDialogConfig: { title: 'Select Unity Project Directory', message: 'Select Unity Project Directory' },
    });
    if (r?.dirPath === undefined) return;
    RF.toast('Creating the Unity project…', 'folder-plus');
    try {
      const unityEditorId = await call(RS + 'fetchRecRoomStudioTemplateUnityEditorId', ids());
      await call(RS + 'createUnityProjectFromRecRoomStudioTemplate', {
        ...ids(),
        unityEditorId,
        unityProjectDirPath: r.dirPath,
      });
      st.lastProject = r.dirPath;
      remember(r.dirPath);
      RF.toast('Project created. Unity is opening it.', 'check');
    } catch (e) {
      fail(
        e?.message ||
          'The launcher could not complete the project request. Check the template installation and Unity editor, then try again.',
      );
    }
  }
  async function addExisting() {
    const { selectDirDialogResult: r } = await call(RS + 'displaySelectDirDialog', {
      selectDirDialogConfig: { title: 'Select a Studio project folder', message: 'Select a Studio project folder' },
    });
    if (r?.dirPath === undefined) return;
    try {
      await call(RS + 'fetchUnityProjectUnityEditorId', { unityProjectDirPath: r.dirPath });
      remember(r.dirPath);
      RF.toast('Project added.', 'check');
    } catch {
      fail('That folder is not a Unity project.');
    }
  }

  const internal = () => /internal|test/.test(String(window.patchKitLauncher2Id ?? ''));
  async function channelPicker() {
    if (!internal()) return [];
    let branches = {};
    try {
      branches = (await call(PK + 'fetchAppBranchesInfoQueryResult', { appId: st.appId, appBranchesInfoQueryConfig: {} }))
        ?.branchesInfo ?? {};
    } catch {
      return [];
    }
    const select = el('select', 'input channel');
    for (const [id, info] of Object.entries(branches)) {
      const o = el('option', '', String(info?.name ?? id).replace('Rec Room - ', '').replace(/Rec Room/g, 'RecFlare'));
      o.value = id;
      o.selected = id === st.branchId;
      select.append(o);
    }
    select.addEventListener('change', () => {
      const id = select.value;
      if (id === 'main') return switchBranch(id, undefined);
      passwordStep(id);
    });
    return [select];
  }
  function switchBranch(id, password) {
    if (id !== st.branchId) st.installPath = undefined;
    st.branchId = id;
    st.password = password;
    st.hub = st.editor = undefined;
    st.latest = undefined;
    st.latestAt = 0;
    st.stepKey = '';
    tick();
  }
  function passwordStep(id) {
    const input = el('input', 'input');
    input.type = 'password';
    input.placeholder = 'Channel password';
    st.stepKey = `password:${id}`;
    const card = $('studio-step');
    card.hidden = false;
    card.replaceChildren(
      el('h2', '', 'Enter channel password'),
      input,
      el(
        'div',
        'btn-row',
        button('Submit', 'check', 'primary', async () => {
          const ok = await call(PK + 'fetchIsAppBranchPasswordValid', {
            appId: st.appId,
            appBranchId: id,
            appBranchPassword: input.value,
          }).catch(() => false);
          if (!ok) {
            input.value = '';
            return fail('The password you entered is incorrect.');
          }
          switchBranch(id, input.value);
        }),
        button('Cancel', '', 'ghost', () => {
          st.stepKey = '';
          tick();
        }),
      ),
    );
    input.focus();
  }

  let ticking = false;
  async function tick() {
    if (ticking || st.branchId === undefined) return;
    if (st.stepKey.startsWith('password:')) return;
    ticking = true;
    try {
      await tickOnce();
    } finally {
      ticking = false;
    }
  }
  async function tickOnce() {
    let s;
    try {
      s = await call(PK + 'retrieveAppBranchState', ids());
    } catch (e) {
      if (!isError(e, 'AppBranchIsNotAllowed') && !isError(e, 'AppBranchIsNotRegistered')) throw e;
      s = null;
    }
    st.branchState = s;
    const channel = await channelPicker();

    if (s?.ongoingTaskId !== undefined) {
      st.taskId = s.ongoingTaskId;
      st.lastSeenTaskId = s.ongoingTaskId;
      const task = await call(PK + 'fetchAppBranchTaskState', { ...ids(), appBranchTaskId: s.ongoingTaskId }).catch(
        () => undefined,
      );
      st.task = task;
      setReady(false);
      status(true, 'Studio is busy', '', ...channel);
      if (task) taskStep(task);
      return;
    }
    if (st.lastSeenTaskId !== undefined) {
      const finished = await call(PK + 'fetchAppBranchTaskState', { ...ids(), appBranchTaskId: st.lastSeenTaskId }).catch(
        () => undefined,
      );
      st.lastSeenTaskId = undefined;
      st.taskId = undefined;
      reportFinished(finished);
      st.latestAt = 0;
    }

    if (s && s.doesNeedRepairing) {
      setReady(false);
      status(false, 'Studio needs repairing', '', ...channel);
      return repairStep(s.lastFinishedDataTaskInfo?.type === TASK.repair);
    }
    if (s && lastTaskFailed(s)) {
      setReady(false);
      status(false, 'Studio did not finish installing', s.lastFinishedDataTaskInfo.errorTypeName, ...channel);
      return resumeStep(s.lastFinishedDataTaskInfo);
    }

    if (!s || s.installedVersionId === undefined) {
      setReady(false);
      status(false, 'Studio is not installed', 'Install it to start building.', ...channel);
      const retry = localStorage.getItem('elevated_retry_install_path');
      if (retry !== null) {
        localStorage.removeItem('elevated_retry_install_path');
        await install(retry);
        return;
      }
      return installPathStep();
    }

    if (Date.now() - st.latestAt > 5 * 60 * 1000) {
      st.latestAt = Date.now();
      try {
        st.latest = await call(PK + 'fetchAppBranchLatestVersionId', ids());
      } catch {
      }
      if (st.latest !== undefined && st.latest !== s.installedVersionId) {
        status(true, 'Updating Studio', '', ...channel);
        step('updating', () => [el('h2', '', 'Updating RecFlare Studio…'), progress()]);
        await startUpdate();
        return;
      }
    }

    const repair = button('Repair', 'refresh-cw', 'ghost sm', startRepair);
    repair.title = 'Download any missing or changed Studio files again';

    if (st.hub === undefined) void checkHub();
    if (hubStep() !== false) {
      setReady(false);
      status(true, 'Studio is installed', 'Setting up Unity Hub', repair, ...channel);
      return;
    }
    if (st.editor === undefined) void checkEditor();
    if (editorStep() !== false) {
      setReady(false);
      status(true, 'Studio is installed', 'Setting up the Unity editor', repair, ...channel);
      return;
    }

    step('', null);
    setReady(true);
    status(true, `Unity ${st.editor.id.version} is ready`, winPath(st.editor.path) ?? '', repair, ...channel);
  }

  async function start() {
    RF = window.RF;
    call = RF.api.call;
    setReady(false);
    status(false, 'Starting Studio…', '');
    $('studio-new').addEventListener('click', () => newProject());
    $('studio-add').addEventListener('click', () => addExisting());
    renderProjects();
    await selfUpdate();
    await resolveIds();
    st.lastProject = await call(RS + 'fetchRecRoomBranchLastStudioUnityProjectDirPath', {
      recRoomBranchId: st.branchId,
    }).catch(() => undefined);
    renderProjects();
    for (;;) {
      if (document.body.dataset.view === 'studio' || st.taskId !== undefined) await tick().catch(() => {});
      await sleep(1000);
    }
  }

  function importProjects(dirs) {
    const list = projects();
    let added = 0;
    for (const dir of dirs)
      if (!list.some((p) => p.path.toLowerCase() === String(dir).toLowerCase())) {
        list.push({ path: dir, name: String(dir).split(/[\\/]/).filter(Boolean).pop() ?? dir });
        added++;
      }
    saveProjects(list);
    if (RF) renderProjects();
    return added;
  }
  const knownProjects = () => projects().map((p) => p.path.toLowerCase());

  async function needsTemplate() {
    await resolveIds();
    try {
      const s = await call(PK + 'retrieveAppBranchState', ids());
      return s?.installedVersionId === undefined;
    } catch {
      return true;
    }
  }
  async function adoptTemplate(dir) {
    await resolveIds();
    try {
      await call(PK + 'registerAppBranch', {
        ...ids(),
        appBranchRootDirPath: dir,
        isAppBranchNotEmptyRootDirAllowed: true,
      });
    } catch (e) {
      if (!isError(e, 'AppBranchIsAlreadyRegistered')) throw e;
    }
    st.stepKey = '';
    st.latestAt = 0;
    void tick();
  }

  window.RFStudio = { start, importProjects, knownProjects, needsTemplate, adoptTemplate };
})();
