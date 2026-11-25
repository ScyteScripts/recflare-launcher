# RecFlare Launcher

One Windows launcher for RecFlare: install and play the game, and build with RecFlare Studio.

## What it does

**RecFlare**
- Installs the game the same way the RecFlare installer does: from Steam (DepotDownloader, signing in with a Steam username or a QR code in its own window) or from the community mirror (checked against its known checksum), then installs the RecFlare 2026 patch and points it at RecFlare.
- Play and Play in VR, with patch updates checked at start and before playing.
- Live player numbers, a photo slideshow from recent in-game photos, hot rooms, a featured rooms carousel, and your favorited and cheered rooms.

**Studio** (only for accounts with Studio access)
- Installs, updates and repairs the RecFlare Studio files, installs Unity Hub and the Unity editor Studio needs, and creates and opens Studio projects.

**Account**
- Sign in with your RecFlare account in the browser (device sign-in). The launcher keeps you signed in using Windows' encrypted storage.

**Moving over**
- On first start it looks for the old RecFlare installer, the old Studio launcher, its Studio files and your Studio projects, and asks before using them, so nothing is downloaded again. Afterwards it offers to remove the old launchers. Settings > Migrations can scan again and lists what was moved.

**Updates**
- The launcher updates itself from this repository's GitHub Releases.

## Build

On Windows x64 you need Node.js 22.12 or newer, Python 3.12 or newer and the .NET 10 SDK. The installer also needs NSIS 3 and 7-Zip.

```powershell
npm ci
npm run fetch:runtime
npm run package
npm start
```

`npm run package` writes the portable launcher to `dist/RecFlare Launcher`. `npm run package:installer` also builds `dist/RecFlareLauncher-Setup.exe` and its `.sha256`.

Tests:

```powershell
npm test
```

## Releasing an update

1. Bump `version` in `package.json`.
2. Run `npm run package:installer`.
3. Publish a release tagged with the version, with both files attached:

```powershell
gh release create v0.2.1 "dist/RecFlareLauncher-Setup.exe" "dist/RecFlareLauncher-Setup.exe.sha256" --repo ScyteScripts/recflare-launcher --title "RecFlare Launcher 0.2.1" --notes "What changed"
```

Installed launchers find the release, check the setup against its SHA-256, install it and start again.

## Layout

| Path | What it is |
| --- | --- |
| `src/renderer` | The launcher's page: plain HTML, CSS and JavaScript |
| `src/recflare` | Sign in, rooms, the game install and launcher updates |
| `src/launcher`, `src/api`, `src/platform` | Studio: Unity Hub and editor, projects, routing checks |
| `src/vendor`, `src/managed`, `updater` | The Studio update engine and its .NET updater |
| `installer` | The Windows installer |
| `config` | Product settings and Studio template checks |

## Credits

See [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md). Icons: [Lucide](https://lucide.dev) (ISC). Fonts: Bricolage Grotesque and Figtree (SIL Open Font License).

## License

[MIT](LICENSE) for the launcher's own code. Third-party components keep their own licenses.
