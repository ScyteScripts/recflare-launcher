import fs from 'node:fs';
import path from 'node:path';
import http from 'node:http';

export async function serveFixture(root, secret) {
  const catalog = JSON.parse(fs.readFileSync(path.join(root, 'catalog.json')));
  const state = {latest: 171, slow: false, slowTransferBytes: 0, requests: []};
  const server = http.createServer((req, res) => {
    const url = new URL(req.url, 'http://localhost');
    const json = value => {res.writeHead(200, {'Content-Type': 'application/json'}); res.end(JSON.stringify(value));};
    state.requests.push({method: req.method, path: url.pathname.replace(secret, '<app>'), range: req.headers.range});
    if (url.pathname === '/_fixture/latest') {state.latest = Number(url.searchParams.get('version')); return json({ok:true});}
    if (url.pathname === '/_fixture/slow') {state.slow = url.searchParams.get('enabled') === '1'; state.slowTransferBytes=0; return json({ok:true});}
    if (url.pathname === '/_fixture/status') return json({slowTransferBytes:state.slowTransferBytes});
    const zip = /^\/content\/v(171|172)\.zip$/.exec(url.pathname);
    if (zip) {
      const file = path.join(root, `v${zip[1]}.zip`), size = fs.statSync(file).size;
      const range = /^bytes=(\d+)-(\d*)$/.exec(req.headers.range || '');
      const start = range ? Number(range[1]) : 0, end = range?.[2] ? Math.min(Number(range[2]), size - 1) : size - 1;
      res.writeHead(range ? 206 : 200, {'Content-Type': 'application/zip', 'Accept-Ranges': 'bytes',
        'Content-Length': end-start+1, ...(range ? {'Content-Range': `bytes ${start}-${end}/${size}`} : {})});
      if (req.method === 'HEAD') return res.end();
      const stream = fs.createReadStream(file, {start, end, highWaterMark: 32*1024});
      res.on('close', () => stream.destroy());
      if (!state.slow) return stream.pipe(res);
      stream.on('data', data => {stream.pause(); res.write(data); state.slowTransferBytes+=data.length; setTimeout(() => stream.resume(), 30);});
      stream.on('end', () => res.end()); return;
    }
    const prefix = '/1/apps/' + secret;
    if (!url.pathname.startsWith(prefix)) {res.writeHead(404); return res.end();}
    const suffix = url.pathname.slice(prefix.length).replace(/^\//, '');
    const release = catalog.releases.find(r => r.version === state.latest);
    const presentation = r => ({id:r.version,label:r.label,changelog:r.label,published:true,draft:false,pending_publish:false,publish_progress:1,main_executable:'dummy.exe',main_executable_args:'',ignored_files:[],content_guid:r.summary.hash_code,content_meta_guid:r.summary.hash_code,patch_links:{incoming:[],outgoing:[]}});
    if (!suffix) return json({secret,display_name:'Rec Room Studio',name:'Rec Room Studio',platform:'windows',current_version:state.latest,lowest_version_with_diff:0,use_keys:false,use_token:false,use_password:false,patcher_whitelabel:false,patcher_download_speed_unit:'bytes'});
    if (suffix === 'versions/latest/id') return json({id:state.latest});
    if (suffix === 'versions') return json(catalog.releases.map(presentation));
    if (suffix === 'changelog') return json({versions:catalog.releases.map(r=>({vid:r.version,label:r.label,changelog:r.label}))});
    const match = /^versions\/(latest|171|172)(?:\/(content_summary|content_urls|diff_summary|diff_urls))?$/.exec(suffix);
    if (match) {
      const r = match[1] === 'latest' ? release : catalog.releases.find(r => r.version === Number(match[1]));
      if (match[2] === 'content_summary') return json(r.summary);
      if (match[2] === 'content_urls' || match[2] === 'diff_urls') return json([{url:`http://127.0.0.1:${server.address().port}/content/v${r.version}.zip`,part_size:r.partSize}]);
      if (match[2] === 'diff_summary') {const {files,...summary}=r.summary; return json({...summary,added_files:files.map(f=>f.path),modified_files:[],unchanged_files:[],removed_files:[]});}
      return json(presentation(r));
    }
    res.writeHead(404); res.end();
  });
  await new Promise(resolve => server.listen(0, '127.0.0.1', resolve));
  return {origin: `http://127.0.0.1:${server.address().port}`, state, close: () => new Promise(resolve => {server.close(resolve);server.closeAllConnections();})};
}
