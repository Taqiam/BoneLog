---
id: "00013"
slug: "workflows"
categoryPath: "Guide"
title: "GitHub Actions"
date: "2026-06-27"
tags: ["BoneLog", "documentation", "ci"]
shortDescription: "CI, index generation, and deploy workflows."
---

| Workflow | Trigger | Purpose |
|----------|---------|---------|
| **CI** | Push/PR to `main` | Build & test |
| **Update index on main** | Push to `posts/**`, `scripts/**` | Commits `index.json` on `main` |
| **Update data on gh-pages** | After index update (or manual) | Syncs `data/` to `gh-pages` |
| **Deploy to GitHub Pages** | Manual | Full site publish |
| **Release** | Tag `v*` | GitHub Release + deploy |

## Generate index locally

```bash
dotnet run scripts/GenerateIndex.cs -- \
  src/BoneLog.Blazor/wwwroot/data/posts \
  src/BoneLog.Blazor/wwwroot/data/index.json
```

Use `--full` to rebuild all entries. Output includes `index.generation.log.json` (errors, warnings, timing).

## Typical flow

1. Edit posts → push `main`
2. **Update index on main** runs
3. **Update data on gh-pages** syncs live content
4. **Deploy to GitHub Pages** when WASM, CSS, or config changes

## Next

- [Publishing](/posts/en/00011/publishing)
- [Custom hosting](/posts/en/00012/full-custom-hosting)
