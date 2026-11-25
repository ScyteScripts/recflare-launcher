# Architecture

| Area | Location |
| --- | --- |
| Unity discovery, project creation and launcher workflows | `src/launcher` |
| Launcher API contracts | `src/api` |
| React UI, state and typed component props | `src/renderer` |
| Editable launcher framework packages | `src/vendor` |
| Process enumeration, protocols and installation detection | `src/platform` |
| Distribution routing and template hash checks | `src/policy.cjs`, `src/network.cjs` |
| Managed updater implementation | `src/managed`, `updater` |
| Electron/updater JSON-lines bridge | `src/updater` |
| Product settings, runtime version and template hashes | `config` |
| Images, fonts and launcher assets | `assets` |
| Windows installer | `installer` |

`updater/Recovered.Updater.csproj` is the updater build entry point. It compiles
managed source into a self-contained process; there are no original PatchKit DLL
or native Node-addon build inputs. Open-source runtimes and libraries remain
upstream dependencies. Some reconstructed payload contracts still use explicit
`RecoveredValue` boundaries; these do not imply fully recovered original types.

The renderer builds from TSX and editable JavaScript framework modules. The
normal build and tests do not require captured bundles or prior installations.
For an optional comparison with a locally retained original renderer bundle,
set `STUDIO_RENDERER_BASELINE` to that file before running `test:renderer`.

Project creation checks the approved template hashes before copying into a new
or empty project directory. Unity Hub shutdown uses an exact process-name match
and reports a bounded failure rather than repeating its dialog. Templates and the
Studio SDK are separate from this launcher.

The displayed product is RecFlare Studio Launcher. Launcher chrome and Windows
product entries include the Launcher suffix; Studio and game workflow labels
retain their own product names. Historical executable filenames,
registry keys, protocols and metadata filenames are retained where needed for
compatibility with installed launchers and the Studio SDK.

To upgrade Electron, change `config/runtime.json`, fetch the runtime, rebuild,
and run the test suites. No native Node addons need an Electron ABI rebuild.
NuGet versions and lockfiles are maintained in `updater`; regenerate the lockfile
explicitly when changing dependencies.
