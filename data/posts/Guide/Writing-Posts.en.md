---
id: "00004"
slug: "writing-posts"
categoryPath: "Guide"
title: "Writing posts"
date: "2026-06-27"
tags: ["BoneLog", "documentation", "markdown"]
shortDescription: "Front matter, language files, links, and Markdown."
---

## File naming

| File | Language |
|------|----------|
| `MyPost.en.md` | English (default) |
| `MyPost.fa.md` | Persian |
| `MyPost.md` | English |

Folder layout is for your organization only — URLs use **id + slug**, not paths.

## Front matter

```yaml
---
id: "00100"
slug: "my-post"
categoryPath: "Guide"
title: "My Post"
date: "2026-06-27"
tags: ["docs"]
shortDescription: "One-line summary for the home page."
thumbnail: "/images/Logo.png"
cover: "/images/cover.jpg"
---
```

| Field | Required | Notes |
|-------|----------|-------|
| `id` | Yes | Digits only. Same id for all language files of one post. |
| `slug` | Yes | URL segment (lowercase, hyphens). |
| `categoryPath` | Yes | Shown in UI; use `/` for nested (`Guide/Advanced`). |
| `title` | Yes on English | Other languages may override title only. |
| `date` | Recommended | ISO date for sorting. |

## URLs

`/posts/{language}/{id}/{slug}` — example: `/posts/en/00100/my-post`

## Links between posts

Use explicit post paths (works with `BaseDir` on GitHub Pages):

```markdown
[Configuration](/posts/en/00009/configuration)
[Quick start](/posts/en/00003/quick-start)
```

Keep full URLs as-is: `https://...`, `mailto:...`, `#heading`.

## Markdown

Standard Markdown plus **Mermaid** diagrams, **code blocks** (Prism), and auto **RTL** for Arabic/Persian paragraphs.

## Next

- [Tags & language](/posts/en/00005/tags-categories-language)
- [Paths & URLs](/posts/en/00007/paths)
- [Images](/posts/en/00008/images)
