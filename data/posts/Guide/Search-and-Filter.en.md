---
id: "00006"
slug: "search-and-filter"
categoryPath: "Guide"
title: "Search & filters"
date: "2026-06-27"
tags: ["BoneLog", "documentation"]
shortDescription: "Home page search syntax and language filter."
---

## Search box

Free text matches **title** and **shortDescription**.

### Filters

| Syntax | Example | Effect |
|--------|---------|--------|
| `Tag:` | `Tag:blazor hello` | Posts with tag containing `blazor`, plus free text |
| `Cat:` | `Cat:Guide` | Category path contains `Guide` |
| `Lang:` | `Lang:fa` | Posts with a Persian file |

Combine filters: `Lang:en Tag:docs quick`

`Lang:` in search overrides the sidebar `?lang=` filter.

## Sidebar filters

- **Categories** — click to filter by category
- **Tags** — click to add `Tag:` to search
- **Languages** — `?lang=fa` or **All** (`?lang=`)

## Next

- [Tags & language](/posts/en/00005/tags-categories-language)
- [Configuration](/posts/en/00009/configuration)
