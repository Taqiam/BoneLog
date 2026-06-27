---
id: "00005"
slug: "tags-categories-language"
categoryPath: "Guide"
title: "Tags, categories & language"
date: "2026-06-27"
tags: ["BoneLog", "documentation", "i18n"]
shortDescription: "Post identity, categories, tags, and multilingual files."
---

## Post identity

One logical post = one **`id`** + **`slug`**, shared across language files:

```
Guide/MyPost.en.md   id: "00100"  slug: my-post
Guide/MyPost.fa.md   (same id & slug; optional title override)
```

English front matter is canonical. Other languages inherit metadata; only `title` may differ.

## Categories

Set `categoryPath` in front matter. Sidebar groups posts by category when `Features.CategorySidebar` is true.

Nested: `categoryPath: "Guide/Advanced"` → displayed as **Guide / Advanced**.

## Tags

```yaml
tags: ["blazor", "markdown"]
```

Enable tag sidebar with `Features.TagSidebar` in [Configuration](/posts/en/00009/configuration).

## Language

- **Filename suffix** sets language (`.en.md`, `.fa.md`).
- **Home page:** sidebar language filter (`?lang=fa`) or search `Lang:fa`.
- **Post page:** language switcher when multiple translations exist.

## Next

- [Search & filters](/posts/en/00006/search-and-filter)
- [Writing posts](/posts/en/00004/writing-posts)
