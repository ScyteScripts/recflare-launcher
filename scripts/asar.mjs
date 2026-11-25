import fs from 'node:fs';
import path from 'node:path';
import crypto from 'node:crypto';

// Electron ASAR's two Chromium Pickles followed by a sorted file-data stream.
// No executable rewriting, unpacked members, symlinks, or install-time scripts.
export function createAsar(source, output, {exclude = () => false} = {}) {
  const chunks = []; let offset = 0;
  const digest = b => crypto.createHash('sha256').update(b).digest('hex');
  function walk(dir) {
    const files = {};
    for (const name of fs.readdirSync(dir).sort()) {
      const file = path.join(dir, name), stat = fs.lstatSync(file);
      if (stat.isSymbolicLink()) throw new Error('ASAR inputs must not contain links');
      if (exclude(path.relative(source,file).replaceAll('\\','/'))) continue;
      if (stat.isDirectory()) files[name] = {files: walk(file)};
      else {
        const data = fs.readFileSync(file), blockSize = 4 * 1024 * 1024;
        const blocks = [];
        for (let i = 0; i < data.length; i += blockSize) blocks.push(digest(data.subarray(i, i + blockSize)));
        files[name] = {size: data.length, offset: String(offset), integrity: {algorithm: 'SHA256', hash: digest(data), blockSize, blocks}};
        chunks.push(data); offset += data.length;
      }
    }
    return files;
  }
  const raw = Buffer.from(JSON.stringify({files: walk(source)}));
  const payload = Buffer.alloc(4 + raw.length + ((-(4 + raw.length)) & 3));
  payload.writeUInt32LE(raw.length); raw.copy(payload, 4);
  const prefix = Buffer.alloc(16);
  prefix.writeUInt32LE(4, 0); prefix.writeUInt32LE(payload.length + 4, 4);
  prefix.writeUInt32LE(payload.length, 8); prefix.writeUInt32LE(raw.length, 12);
  fs.writeFileSync(output, Buffer.concat([prefix.subarray(0, 12), payload, ...chunks]));
}
