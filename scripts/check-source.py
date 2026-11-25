"""Audit files eligible for commit; never inspect ignored personal captures."""
import json
from pathlib import Path
import re
import subprocess

ROOT = Path(__file__).resolve().parent.parent
files = subprocess.check_output(['git', 'ls-files', '--cached', '--others', '--exclude-standard', '-z'], cwd=ROOT).decode().split('\0')
files = sorted(set(filter(None, files)))
issues = []
private_path = re.compile(r'[A-Z]:[\\/](?:Users|Development)[\\/]', re.I)
credential = re.compile(r'-----BEGIN (?:RSA |EC |OPENSSH )?PRIVATE KEY-----|(?:ghp_|github_pat_)[A-Za-z0-9_]{20,}|AKIA[0-9A-Z]{16}')
for relative in files:
    file = ROOT / relative
    if re.match(r'^(?:\.local|build|dist|validation|inputs|tools|node_modules)/', relative):
        issues.append(relative + ': generated or private material')
    if file.suffix.lower() in {'.exe', '.dll', '.node', '.pdb', '.map', '.pfx', '.key'}:
        issues.append(relative + ': binary/debug/credential artifact')
    if file.suffix.lower() in {'.png', '.ico', '.woff', '.woff2'}:
        continue
    for number, line in enumerate(file.read_text(encoding='utf-8-sig', errors='replace').splitlines(), 1):
        if private_path.search(line) or credential.search(line):
            issues.append(f'{relative}:{number}: possible private path or credential')
        if ('source' + '-owned') in line.lower():
            issues.append(f'{relative}:{number}: obsolete release naming')
report = {'status':'failed' if issues else 'passed', 'files':len(files), 'issues':issues}
output = ROOT / 'validation/source-audit.json'
output.parent.mkdir(exist_ok=True)
output.write_text(json.dumps(report, indent=2)+'\n')
print(json.dumps(report, indent=2))
raise SystemExit(bool(issues))
