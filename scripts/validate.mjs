import fs from 'node:fs';
import path from 'node:path';
import { spawn } from 'node:child_process';
import { root, sha } from './build.mjs';
import { serveFixture } from './fixture-server.mjs';

const dir = path.join(root, 'validation');
fs.mkdirSync(dir, {recursive: true});
const runRoot = fs.mkdtempSync(path.join(dir, 'run-'));
const report = path.join(dir, 'electron-smoke.json');
const profile = path.join(dir, 'blank.ini');
const config = JSON.parse(fs.readFileSync(path.join(root, 'dist/RecFlare Launcher/resources/config.json')));
const fixtureSecret = 'launcher-native-fixture';
const fixture = process.argv.includes('--native') ? await serveFixture(path.join(dir, 'distribution'), fixtureSecret) : null;
fs.writeFileSync(profile, `[Network]\n${fixture ? `PatchKitApiUrl=${fixture.origin}\nPatchKitCacheUrl=${fixture.origin}\nPatchKitAppSecretSha256=${sha(fixtureSecret)}\n` : ''}[Launcher]\nUseInstalledTemplate=0\n`);
// A fixture config with no agreement requirement exercises the post-onboarding UI
// without accepting any legal agreement or changing the shipping configuration.
const fixtureConfig = path.join(runRoot, 'config.json');
fs.writeFileSync(fixtureConfig, JSON.stringify({...config, agreements: []}));
const exe = path.join(root, 'dist/RecFlare Launcher/RecFlare Launcher.exe');
const env = {...process.env, RECROOM_STUDIO_PATCH_CONFIG: profile,
  LOCALAPPDATA: path.join(runRoot, 'local-app-data'), XDG_DATA_HOME: path.join(runRoot, 'local-data'),
  STUDIO_LAUNCHER_VALIDATION_ROOT: runRoot, STUDIO_LAUNCHER_SMOKE_REPORT: report,
  ...(fixture ? {STUDIO_LAUNCHER_FIXTURE_ORIGIN:fixture.origin, STUDIO_LAUNCHER_FIXTURE_SECRET:fixtureSecret} : {})};
delete env.ELECTRON_RUN_AS_NODE;
for (const location of [env.LOCALAPPDATA, env.XDG_DATA_HOME]) fs.mkdirSync(location, {recursive:true});
if (fs.existsSync(report)) fs.unlinkSync(report);
const child = spawn(exe, ['--overrideConfigFilePath', fixtureConfig], {cwd: path.dirname(exe), env, windowsHide: true, stdio: ['ignore', 'pipe', 'pipe']});
const log = fs.createWriteStream(path.join(dir, 'electron-smoke.log'));
child.stdout.pipe(log); child.stderr.pipe(log);
const timeout = setTimeout(() => child.kill(), 55000);
const code = await new Promise((resolve, reject) => {child.once('error', reject); child.once('exit', resolve);});
clearTimeout(timeout); log.end();
if (fixture) {fs.writeFileSync(path.join(dir,'native-requests.json'),JSON.stringify(fixture.state.requests,null,2)); await fixture.close();}
const result = fs.existsSync(report) ? JSON.parse(fs.readFileSync(report, 'utf8')) : null;
console.log(JSON.stringify({exitCode: code, ...result}, null, 2));
if (code !== 0 || result?.status !== 'loaded') process.exitCode = 1;
