# Configuration

`config/product.json` defines product origins and the build label.
`config/launcher.json` supplies launcher defaults and catalog identifiers.
`config/template-manifest.json` and `config/template-releases.json` define the
supported template versions and SHA-256 guards. `config/assets.json` verifies
checked-in artwork before a build. `config/legacy-files.json` lists older launcher
files that setup may replace or retire; it contains paths, not captured binaries.

For service overrides, copy `config/services.ini.example` to `services.ini` beside
`Launch.cmd`. `RECROOM_STUDIO_PATCH_CONFIG` selects an explicit profile. If neither
is supplied, the launcher reads an existing global Studio profile when available.
Operator values in the example are deliberately blank. Keep real profiles and
credentials out of source control.

Rec Room web/service hostnames route to matching `recflare.net` hosts. The scoped
Studio distribution default is `https://studio.recflare.net`. The PatchKit app
catalog and Unity downloads remain vendor services. The original launcher
self-updater is disabled; Studio template updates remain enabled. Original
RudderStack and Statsig telemetry is disabled. Legacy updater telemetry requires
explicit `STUDIO_UPDATER_LEGACY_TELEMETRY=1`; HTTPS uses platform certificate checks.

Portable mode keeps state under the application's `data` directory. Installed
mode uses the existing launcher paths and Windows integration. Do not rename
compatibility IDs or metadata files without adding a migration.
