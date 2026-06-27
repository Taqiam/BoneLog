---
id: "00011"
slug: "publishing"
categoryPath: "Guide"
title: "Publishing"
date: "2026-06-27"
tags: ["BoneLog", "documentation", "deploy"]
shortDescription: "GitHub Pages and static hosting."
---

## GitHub Pages (this repo)

| Workflow | When |
|----------|------|
| **Update index on main** | Push changes under `posts/` or `scripts/` |
| **Update data on gh-pages** | Syncs `data/` to live site (auto or manual) |
| **Deploy to GitHub Pages** | Full WASM + assets deploy (manual) |
| **Release** | Tag `v*` → release + deploy |

Typical loop: edit posts → push `main` → index updates → data syncs to `gh-pages`.

Run **Deploy to GitHub Pages** when you change WASM, CSS, `config.json`, or `wwwroot/images/`.

## Other hosts

BoneLog is static WASM — any host with **SPA fallback** (`/*` → `index.html`) works: Netlify, Cloudflare Pages, Azure Static Web Apps, nginx, IIS.

Set `BaseDir` to your subpath or `/`.

## Separate data host

Set `BaseDataPath` to a full URL. Regenerate `index.json` whenever posts change. Details: [Custom hosting](/posts/en/00012/full-custom-hosting).

## Next

- [Quick start](/posts/en/00003/quick-start)
- [Workflows](/posts/en/00013/workflows)
- [Custom hosting](/posts/en/00012/full-custom-hosting)
