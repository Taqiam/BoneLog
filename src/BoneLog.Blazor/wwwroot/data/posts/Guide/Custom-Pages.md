---
title: "Custom pages & navigation"
date: "2026-05-27"
tags: ["BoneLog", "documentation"]
language: "EN"
shortDescription: "CatchAll routes for extra Markdown pages and navbar links in config.json."
---

## Custom pages (CatchAll)

Markdown under `data/` that is **not** only in the posts index can be a standalone page:

| File | URL |
|------|-----|
| `data/legal/Privacy.md` | `/legal/Privacy` |
| `data/projects/Overview.md` | `/projects/Overview` |

The slug maps to `data/{slug}.md`. These pages are **not** listed on the home page unless you link to them.

## Navbar

Edit `config.json` → `NavItems`:

```json
"NavItems": [
  { "Title": "Guide", "Url": "post/Guide/Index" },
  { "Title": "About", "Url": "about" },
  { "Title": "Privacy", "Url": "legal/Privacy" },
  { "Title": "GitHub", "Url": "https://github.com/Taqiam/BoneLog" }
]
```

- **Relative URLs** (`about`, `post/...`) resolve against `BaseDir`.
- **Absolute URLs** open external sites.

## Built-in routes

| Route | Page |
|-------|------|
| `/` | Home (post list) |
| `/about` | About |
| `/post/{path}` | Blog post |

## See also

- [Configuration](Configuration)
- [Writing posts](Writing-Posts)
- [Documentation index](Index)
