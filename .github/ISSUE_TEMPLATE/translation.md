---
name: Translation
about: Add or fix a language
labels: i18n
---

**Language** (name and ISO 639-1 code):

**New language or fix to an existing one?**

**How to submit**: attach your `<code>.json` (a copy of `assets/i18n/en.json` with
every value translated and every `{0}` / `{1}` placeholder kept), or open a pull
request against `assets/i18n/`. Run `pwsh tools/i18n-check.ps1` first — it must
print `i18n-check: OK`. See `docs/i18n.md`.
