---
id: "00002"
slug: "introduction"
categoryPath: "Guide"
title: "What is BoneLog?"
date: "2026-06-27"
tags: ["BoneLog", "documentation"]
shortDescription: "File-based Blazor WASM blog — how it works."
---

BoneLog turns Markdown files into a static blog:

- Posts live in `wwwroot/data/posts/`
- `index.json` lists posts for the home page
- Blazor WASM renders pages in the browser
- Deploy anywhere static files + SPA fallback work

Each post has a numeric **`id`**, URL **`slug`**, and optional **language files** (`Post.en.md`, `Post.fa.md`).

Post URLs: `/posts/{language}/{id}/{slug}` — e.g. `/posts/en/00001/index`.

## Next

- [Quick start](/posts/en/00003/quick-start)
- [Documentation hub](/posts/en/00001/index)
