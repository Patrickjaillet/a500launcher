# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.8.2]

### Added
- Workbench-style arrow mouse cursor over the desktop (`assets/cursors/workbench.cur`).
- Generic diskette thumbnail next to each entry in the Recent drive menus.
- Hidden advanced setting: NTSC timing (60 Hz) instead of PAL. Not shown in the
  façade — the checkbox appears only when Ctrl+Shift is held as Settings opens, or
  when it is already enabled. Maps to `ntsc` / `chipset_refreshrate`.

## [0.8.1]

### Added
- `KickstartCatalog`: CRC-32 identification of common Kickstart ROMs (1.2, 1.3,
  2.04, 3.1). The boot check now names the ROM and warns before starting when it
  is not a 1.2/1.3 A500 image (`System.IO.Hashing`).
- `WinUaeInfo`: reads the configured WinUAE executable's file version and warns
  before starting if it is older than 5.x.

## [0.8.0]

### Added
- Git-derived versioning via MinVer (gitignored `Directory.Build.props`): version
  comes from the nearest `v*` tag; commits after a tag get a numeric `-rc.N`
  pre-release suffix, and every build carries `+<sha>`. The csproj `<Version>` is
  the offline fallback.
- `tools/check-conventions.ps1`: fails on comments in code, French identifiers or
  prose outside `ROADMAP.md`, AI-tool markers, disallowed release-name words, or
  missing `.gitignore` entries. Wired into `build-release.ps1`.
- Inno Setup installer (`installer/A500Launcher.iss`) with an Amiga 500 themed
  wizard (Workbench palette, diskette artwork, custom messages). Components:
  application + WinUAE (fixed), translations and drive sound (optional). Requires
  Windows 11; keeps `config.json`, `sets`, `bios`, `roms` on uninstall.
- `tools/build-release.ps1`: tests, i18n-check, publish, portable zip, installer,
  `SHA256SUMS`. Output in `dist/` (gitignored).
- Installer and uninstaller icons.

## [0.7.0]

### Added
- Translations: French, German, Spanish, Italian, Dutch, Polish, Swedish
  (`assets/i18n/*.json`, data only — no strings in code).
- `tools/i18n-check.ps1`: reports missing keys, unknown keys and placeholder
  mismatches against `en.json`.
- `docs/i18n.md`: translator guide.
- Tests now assert every translation file has exactly the reference key set and
  matching `{0}`/`{1}` placeholder slots.

## [0.6.0]

### Added
- Settings: Port 1 device (None / Mouse / Joystick), screen filter
  (Crisp / CRT light / CRT heavy), master volume (0-100%), and a language selector
  populated from the available `assets/i18n/*.json` files.
- These map to `joyport1`, `gfx_filter_scanlines`, `sound_volume` (inverted for
  WinUAE) and `LanguageCode` in the generated configuration.

## [0.5.1]

### Added
- Optional drive sound when a disk is inserted (synthetic Amiga stepper click),
  toggled in Settings. Own asset, licensed with the application.

## [0.5.0]

### Added
- Disk library: named configurations ({DF0, DF1, trapdoor RAM, full screen} plus
  title, publisher, year, notes), stored one JSON per set in `%AppData%\A500Launcher\sets`.
- Library window reached from a new "Library" menu entry: list of sets, launch
  (button or double-click), edit, delete, "Save current" from the desktop state.
- Folder import that scans recursively for `.adf` files and de-duplicates by SHA-256
  content hash, feeding the recent-floppies list.

## [0.4.0]

### Added
- Drag and drop: an `.adf` onto DF0:/DF1: or the window inserts it; a `.rom`/`.bin`
  onto the Kickstart icon (or the window) sets the ROM.
- Right-click menu on each drive: Insert, Eject, Open folder, Recent.
- Recent floppies list (last 10, persisted), offered in the drive menus.
- Eject button shown on a drive while a disk is inserted.
- Optional animated Amiga 500 boot screen (~1.5 s, blinking power LED) shown before
  WinUAE starts. Toggle in Settings.

## [0.3.0]

### Added
- Embedded Topaz (Amiga 500) bitmap font, applied across the whole UI with a
  Consolas fallback (GPL Font Exception, notice in `assets/fonts/LICENSE.txt`).
- Window icon on the Settings and About windows.
- Unified launch error handling (`LauncherException` with i18n message keys).
- Kickstart ROM and `.adf` size/extension validation, with a confirm-or-cancel
  prompt for non-standard files (never blocks a recognised file).
- Rolling local log at `%AppData%\A500Launcher\logs` (7-day retention).
- Single-instance guard: a second launch focuses the running window and exits.
- Global unhandled-exception handler that logs and shows a dialog.

### Changed
- Desktop floppy icons redrawn as a proper diskette shape (label area + shutter).
- Each launch deletes stale `.uae` session files before writing a new one.

## [0.1.0]

### Added
- Portable folder layout (`winuae/`, `bios/`, `roms/`) resolved next to the executable,
  with startup auto-detection that fills empty settings without overwriting user paths.
- Bundled WinUAE 6.0.3 (x64).
- Full-screen launcher window (borderless, maximized), `Esc` to exit.
- JSON internationalisation layer (`assets/i18n/en.json`, `II18nProvider`,
  `Localize` markup extension). No user-facing string remains in code.
- About window with product name, version, copyright, e-mail and website links,
  and a Licence button.
- Windows 11 startup guard (refuses to run on build < 22000).
- Application manifest (per-monitor v2 DPI awareness, `asInvoker`).
- Application and installer icons (`assets/icons`).
- Freeware `LICENSE`.
- Test project (`tests/`) covering the locked A500 profile, portable layout and i18n.
- `.editorconfig` with warnings-as-errors and file-scoped namespaces.

### Changed
- Project, assembly, namespace and `%AppData%` folder renamed to `A500Launcher`.
- Legacy `%AppData%\AmigaLauncher\config.json` is migrated automatically on first run.
- Settings written atomically (temp file + move).
- All identifiers and code are English-only; no comments remain in source files.

[Unreleased]: https://github.com/patrickjaillet/a500launcher/compare/v0.8.2...HEAD
[0.8.2]: https://github.com/patrickjaillet/a500launcher/compare/v0.8.1...v0.8.2
[0.8.1]: https://github.com/patrickjaillet/a500launcher/compare/v0.8.0...v0.8.1
[0.8.0]: https://github.com/patrickjaillet/a500launcher/compare/v0.7.0...v0.8.0
[0.7.0]: https://github.com/patrickjaillet/a500launcher/compare/v0.6.0...v0.7.0
[0.6.0]: https://github.com/patrickjaillet/a500launcher/compare/v0.5.1...v0.6.0
[0.5.1]: https://github.com/patrickjaillet/a500launcher/compare/v0.5.0...v0.5.1
[0.5.0]: https://github.com/patrickjaillet/a500launcher/compare/v0.4.0...v0.5.0
[0.4.0]: https://github.com/patrickjaillet/a500launcher/compare/v0.3.0...v0.4.0
[0.3.0]: https://github.com/patrickjaillet/a500launcher/compare/v0.1.0...v0.3.0
[0.1.0]: https://github.com/patrickjaillet/a500launcher/releases/tag/v0.1.0
