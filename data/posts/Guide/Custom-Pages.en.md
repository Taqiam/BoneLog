---
id: "00010"
slug: "custom-pages"
categoryPath: "Guide"
title: "Custom pages & navigation"
date: "2026-06-27"
tags: ["BoneLog", "documentation"]
shortDescription: "Extra Markdown pages outside the post index."
---

## Custom pages

Any `.md` file under `wwwroot/data/` **outside** the indexed posts flow:

| File | URL |
|------|-----|
| `data/legal/Privacy.md` | `/legal/Privacy` |
| `data/projects/Note.md` | `/projects/Note` |

Served by the catch-all route — no `index.json` entry needed.

## Navigation

Add links in `config.json` → `NavItems`:

```json
{ "Title": "Privacy", "Url": "legal/Privacy" }
{ "Title": "Guide", "Url": "posts/en/00001/index" }
```

App routes have **no leading slash** so `BaseDir` applies on GitHub Pages.

## About page

Fixed route `/about` — content from `Paths.AboutMePath` (default `AboutMe.md`).

## Next

- [Configuration](/posts/en/00009/configuration)
- [Paths & URLs](/posts/en/00007/paths)
