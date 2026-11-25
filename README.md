# RecFlare Launcher

A desktop launcher for RecFlare, built with Electron. One window to play RecFlare and manage your RecFlare Studio projects.

> **Demo build.** The launcher doesn't install or start anything yet. Accounts, projects and paths come from `src/renderer/demo.js`, and each button shows a toast that says what the real launcher would do. The player counts, the photo slideshow and "Hot right now" are pulled live from RecFlare.

## Pages

- **RecFlare**: a slideshow of recent in-game photos behind the whole window, Play and Play in VR, live player stats, and the hottest rooms. Each photo credit links to the player and the room on the website.
- **Studio**: start a new project or add an existing one, Unity status, and your project list.
- **Settings**: install folder, close on play, start in VR, auto update, diagnostics.

## Run it

Needs [Node.js](https://nodejs.org) 20 or newer.

```sh
npm install
npm start
```

`npm start` copies the fonts and icons into `src/renderer` first, then opens the window.

## Layout

| Path | What it is |
| --- | --- |
| `src/main.js` | The window, its controls, and opening links in your browser |
| `src/preload.js` | The small bridge the page uses to talk to the window |
| `src/renderer/` | The UI: plain HTML, CSS and JavaScript, no build step |
| `src/renderer/demo.js` | The demo data |
| `scripts/assets.js` | Copies the fonts and inlines the icons the UI uses |

## Security

The window runs with `contextIsolation` and `sandbox` on, and Node integration off. A Content Security Policy limits the page to RecFlare's own hosts. Links only open in your browser when they're `https` links to `www.recflare.net`, `join.recflare.net` or `github.com`.

## Credits

- Icons: [Lucide](https://lucide.dev) (ISC)
- Fonts: [Bricolage Grotesque](https://fonts.google.com/specimen/Bricolage+Grotesque) and [Figtree](https://fonts.google.com/specimen/Figtree) (SIL Open Font License)

## License

[MIT](LICENSE)
