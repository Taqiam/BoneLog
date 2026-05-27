---
title: "GitHub Actions workflows"
date: "2026-05-27"
tags: ["BoneLog", "documentation", "ci"]
language: "EN"
shortDescription: "CI, index generation, manual deploy, and tag releases."
---

| Workflow | Trigger | Purpose |
|----------|---------|---------|
| **CI** | Push / PR to `main` | Build & test (.NET 10). |
| **Generate Index** | Reusable | Runs `scripts/GenerateIndex.cs`. |
| **Update index on main** | Push to `posts/**` or `scripts/**` | Commits `index.json` on `main`. |
| **Update data on gh-pages** | Manual, or after **Update index on main** | Regenerates index and syncs full `data/` (posts, AboutMe, index) to `gh-pages`. |
| **Deploy to GitHub Pages** | Manual | Full publish → `gh-pages` (sets `<base href>` from `config.json`). |
| **Release** | Tag `v*` | GitHub Release + deploy → `gh-pages` (sets `<base href>` from `config.json`). |

## Generate index locally

```bash
dotnet run scripts/GenerateIndex.cs -- \
  src/BoneLog.Blazor/wwwroot/data/posts \
  src/BoneLog.Blazor/wwwroot/data/index.json
```

`index.json` is what powers the home page post list. The app loads it from `Paths.BaseDataPath` + `Paths.IndexPath`.

If **`BaseDataPath` points at another host** (split site + data), regenerating on `main` is not enough by itself — you must also publish `index.json` (and posts) to that data URL. See [Paths — separating the website and data](Paths).

## Typical flow

1. Push post changes → **Update index on main** (updates `main`).
2. **Update data on gh-pages** runs automatically (or run it manually) → syncs full `data/` to the live site (no WASM rebuild).
3. **Deploy to GitHub Pages** when you need WASM, CSS, new markdown files, images, or `config.json` changes on the live site.

## See also

- [Publishing & static hosting](Publishing)
- [Quick start](Quick-Start)
- [Documentation index](Index)
