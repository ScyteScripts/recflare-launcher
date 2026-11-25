'use strict';
const { execFile } = require('node:child_process');
const path = require('node:path');

const PS = path.join(process.env.SystemRoot || 'C:\\Windows', 'System32', 'WindowsPowerShell', 'v1.0', 'powershell.exe');

function powershell(command, timeout = 120000) {
  return new Promise((resolve) => {
    execFile(PS, ['-NoProfile', '-NonInteractive', '-Command', command], { windowsHide: true, timeout }, (error, stdout) =>
      resolve({ ok: !error, out: String(stdout || '').trim() }),
    );
  });
}

function encoded(script) {
  return Buffer.from(script, 'utf16le').toString('base64');
}

async function isExcluded(dir) {
  const target = path.resolve(String(dir)).toLowerCase().replace(/\\+$/, '');
  const script =
    "try { (Get-MpPreference).ExclusionPath } catch { '' }";
  const { ok, out } = await powershell(script, 20000);
  if (!ok) return null;
  return out
    .split(/\r?\n/)
    .map((p) => p.trim().toLowerCase().replace(/\\+$/, ''))
    .filter(Boolean)
    .some((p) => target === p || target.startsWith(p + '\\'));
}

async function addExclusion(dir) {
  const folder = path.resolve(String(dir));
  const inner = `Add-MpPreference -ExclusionPath '${folder.replace(/'/g, "''")}' -ErrorAction Stop`;
  const script =
    `try { $p = Start-Process powershell.exe -Verb RunAs -Wait -PassThru -WindowStyle Hidden ` +
    `-ArgumentList '-NoProfile','-NonInteractive','-EncodedCommand','${encoded(inner)}'; exit $p.ExitCode } catch { exit 1223 }`;
  const { ok } = await powershell(script);
  return ok;
}

module.exports = { isExcluded, addExclusion };
