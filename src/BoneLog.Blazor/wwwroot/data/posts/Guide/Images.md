---
title: "Images & asset paths"
date: "2026-05-27"
tags: ["BoneLog", "documentation"]
language: "EN"
shortDescription: "Put images anywhere under wwwroot, relative paths resolve from each post file."
---

You can store images **anywhere** under `wwwroot` (or link to external URLs). Common places:

| Location | Example use |
|----------|-------------|
| `wwwroot/images/` | Shared site assets (logo, default covers) |
| `wwwroot/data/posts/MyPost/assets/` | Images for one post |
| Next to the `.md` file | `photo.jpg` in the same folder |

BoneLog resolves **relative** paths from the post file’s directory at runtime (same idea as cover/thumbnail in front matter).

## Rules

| Path type | Behavior |
|-----------|----------|
| Relative (`../`, `images/...`, `assets/...`) | Resolved from the post folder → fetch URL under `data/` or site `images/` |
| Absolute URL (`https://...`) | Unchanged |
| Root path (`/images/...`) | Kept as-is (host root) |

Inline `![alt](path)` images and `cover` / `thumbnail` in YAML use the same server-side resolver.

## See also

- [Paths & addresses](Paths) — overall path rules, `config.json`, and URLs
- [Writing posts](Writing-Posts) (links between posts)
- [Documentation index](Index)
