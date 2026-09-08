# Contributing to A500 Launcher

A500 Launcher is freeware distributed as a proprietary executable. **The source
code is not published**, so contributions are limited to two things:

## 1. Translations

All user-facing text lives in JSON files under `assets/i18n/`. Adding or fixing a
translation needs no code and no build.

- Copy `assets/i18n/en.json` to `<code>.json` (ISO 639-1, e.g. `pt.json`).
- Translate every value; keep the keys and every `{0}` / `{1}` placeholder.
- Set `language.name` to the language's own name.
- Leave file-dialog filters and the e-mail / website values untranslated.
- Run `pwsh tools/i18n-check.ps1` — it must print `i18n-check: OK`.

See [docs/i18n.md](docs/i18n.md) for details. Send the file by e-mail to
**sandefjord.development@proton.me** or open a pull request against
`assets/i18n/`.

## 2. Bug reports and suggestions

Open an issue. For bugs, include:

- the A500 Launcher version (Help → About) and your Windows build,
- what you did, what you expected, what happened,
- the relevant lines from `%AppData%\A500Launcher\logs`.

## What is not accepted

- Code changes (the source is not distributed).
- Kickstart ROMs, disk images, or links to copyrighted material.
- Requests to support machines other than the Amiga 500 — this is a deliberate
  product decision, not a missing feature.

## Attribution

By contributing a translation you agree it may be distributed as part of A500
Launcher under its freeware licence. Contributors are credited by name on request.
