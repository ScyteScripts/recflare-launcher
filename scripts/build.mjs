import fs from 'node:fs';
import path from 'node:path';
import crypto from 'node:crypto';
import { fileURLToPath } from 'node:url';
import * as esbuild from 'esbuild';
import {execFileSync} from 'node:child_process';

export const root = path.resolve(path.dirname(fileURLToPath(import.meta.url)), '..');
const readJson = p => JSON.parse(fs.readFileSync(p, 'utf8'));
export const sha = bytes => crypto.createHash('sha256').update(bytes).digest('hex');
export const appOut = path.join(root, 'build/source-app');
export function verifyInputs() {
  const manifest = readJson(path.join(root, 'config/assets.json'));
  for (const asset of manifest.files) {
    const file = path.resolve(root, asset.path);
    if (!file.startsWith(path.join(root, 'assets') + path.sep) || sha(fs.readFileSync(file)) !== asset.sha256)
      throw new Error('Asset hash mismatch: ' + asset.path);
  }
  return manifest;
}

export async function build() {
  verifyInputs();
  const out = appOut;
  if(!out.startsWith(path.join(root,'build')+path.sep))throw new Error('Invalid generated output path');
  fs.rmSync(out,{recursive:true,force:true});
  fs.mkdirSync(out, { recursive: true });
  const overlaid = new Set();
  const overlayPaths = new Map();
  const resolvedPackages = new Map();
  const overlay = {
    name: 'recovered-sources',
    setup(build) {
      build.onResolve({ filter: /^recroom-studio-launcher-api-foundation$/ }, () => ({ path: path.join(root, 'src/api/$index.js') }));
      const aliases = {'@recovered/policy':'src/policy.cjs','@recovered/network':'src/network.cjs','@recovered/updater':'src/updater/bridge.cjs','@recovered/marshal':'src/updater/marshal.cjs','ps-list':'src/platform/processes.cjs','electron-app-universal-protocol-client':'src/platform/protocol.cjs'};
      aliases['@recovered/routing-repair'] = 'src/platform/routing-repair.cjs';
      build.onResolve({filter:/^(?:@recovered\/|ps-list$|electron-app-universal-protocol-client$)/},args=>aliases[args.path]?{path:path.join(root,aliases[args.path])}:undefined);
      build.onLoad({ filter: /node_modules.*\.[cm]?js$/ }, args => {
        let dir = path.dirname(args.path), pkg;
        while (dir.startsWith(root) && dir !== root) {
          const candidate = path.join(dir, 'package.json');
          if (fs.existsSync(candidate)) { pkg = readJson(candidate); break; }
          dir = path.dirname(dir);
        }
        if (!pkg) return;
        resolvedPackages.set(pkg.name + '@' + pkg.version, true);
        const rel = path.relative(dir, args.path);
        const recovered = path.join(root, 'src/vendor', pkg.name.replaceAll('/', '+') + '@' + pkg.version, rel);
        let contents = fs.readFileSync(fs.existsSync(recovered) ? recovered : args.path, 'utf8');
        if (fs.existsSync(recovered)) {
          overlaid.add(path.relative(root, recovered).replaceAll('\\', '/'));
          overlayPaths.set(path.resolve(args.path), recovered);
        }
        return { contents, loader: 'js', resolveDir: path.dirname(args.path) };
      });
    }
  };
  const result = await esbuild.build({
    absWorkingDir: root, entryPoints: ['src/bootstrap.cjs'], outfile: 'build/source-app/main.js',
    bundle: true, platform: 'node', format: 'cjs', target: 'node16', mainFields: ['module', 'main'],
    sourcemap: 'external', sourcesContent: true, metafile: true,
    minify: false, legalComments: 'external', plugins: [overlay],
    external: ['electron'],
    logLevel: 'warning'
  });
  const mapFile = path.join(out, 'main.js.map');
  const sourceMap = readJson(mapFile);
  sourceMap.sources = sourceMap.sources.map(source => {
    const recovered = overlayPaths.get(path.resolve(out, source));
    return recovered ? path.relative(out, recovered).replaceAll('\\', '/') : source;
  });
  fs.writeFileSync(mapFile, JSON.stringify(sourceMap));
  fs.writeFileSync(path.join(out, 'recflare-version.json'), JSON.stringify({version: readJson(path.join(root, 'package.json')).version}) + '\n');
  fs.writeFileSync(path.join(out, 'package.json'), JSON.stringify({name: 'recroom-studio-launcher', author: {name: 'RecFlare'}, version: '3.0.0', main: 'main.js'}) + '\n');
  let renderer;
  const page = path.join(root, 'src/renderer');
  const theme = path.join(out, 'theme');
  fs.mkdirSync(theme, {recursive: true});
  for (const file of ['index.html', 'styles.css', 'app.js', 'icons.js', 'studio.js'])
    if (fs.existsSync(path.join(page, file))) fs.copyFileSync(path.join(page, file), path.join(theme, file));
  for (const dir of ['img', 'fonts'])
    fs.cpSync(path.join(page, dir), path.join(theme, dir), {recursive: true});
  const forbidden = Object.keys(result.metafile.inputs).filter(p=>/node_modules\/(?:@xingrz\/ffi-napi|ref-napi|ref-struct-di|ps-list|electron-app-universal-protocol-client)\//.test(p));
  if(forbidden.length)throw new Error('Native dependency leaked into build: '+forbidden.join(', '));
  const report = {rendererMetafile:renderer?.metafile,nativeAddonCount:0, updater:'source-built-dotnet-rpc',overlaidModules: [...overlaid].sort(), resolvedPackages: [...resolvedPackages.keys()].sort(),
    lockfileSha256: sha(fs.readFileSync(path.join(root, 'package-lock.json'))),
    mainSha256: sha(fs.readFileSync(path.join(out, 'main.js'))), metafile: result.metafile};
  report.sourceModules = [...new Set([...overlaid, ...Object.keys(result.metafile.inputs).filter(p=>p.startsWith('src/'))])].sort();
  report.localFrameworkPackages = Object.entries(readJson(path.join(root,'package.json')).dependencies).filter(([name])=>/^(troy-|patchkit-)/.test(name)).map(([name,source])=>({name,source}));
  if(report.localFrameworkPackages.some(p=>!p.source.startsWith('file:src/vendor/')))throw new Error('A framework package still depends on its original registry distribution');
  fs.writeFileSync(path.join(root, 'build/build-report.json'), JSON.stringify(report, null, 2) + '\n');
  console.log(`Built launcher from ${report.sourceModules.length} source modules.`);
  return report;
}
if (process.argv[1] && path.resolve(process.argv[1]) === fileURLToPath(import.meta.url)) await build();
