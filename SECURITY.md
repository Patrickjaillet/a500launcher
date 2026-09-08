# Security Policy

## Supported versions

Only the latest released version of A500 Launcher receives security fixes.

## Reporting a vulnerability

Please report security issues privately by e-mail to
**sandefjord.development@proton.me**.

Include:

- a description of the issue and its impact,
- the A500 Launcher version and your Windows build,
- steps to reproduce, and a proof of concept if you have one.

You will normally get an acknowledgement within a few days. Please do not open a
public issue for security reports, and give a reasonable amount of time for a fix
before any public disclosure.

## Scope

In scope: the A500 Launcher executable and its bundled resources (localisation
files, images, sounds, the installer).

Out of scope: WinUAE itself (report to the WinUAE project), and the handling of
Kickstart ROMs or disk images, which are the user's responsibility.

## What the application does

A500 Launcher collects no data and makes no network connection by default. It
writes a locked Amiga 500 `.uae` file to `%TEMP%` and launches WinUAE with it.
Logs are local only. Update checking, if enabled, makes an anonymous request to
the GitHub releases API and sends nothing else.

## Downloads

Releases are not code-signed. Every release includes a `SHA256SUMS` file; verify
your download against it before running. Only download from the GitHub releases
page of this repository.
