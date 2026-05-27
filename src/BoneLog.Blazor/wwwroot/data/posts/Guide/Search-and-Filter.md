---
title: "Search & filters"
date: "2026-05-27"
tags: ["BoneLog", "documentation"]
language: "EN"
shortDescription: "Home page search syntax — free text, Tag:, Cat:, and Lang: filters."
---

The home page search box supports **free text** and **typed filters**:

| Syntax | Example | Matches |
|--------|---------|---------|
| *(free text)* | `blazor wasm` | Title, description, tags, etc. |
| `Tag:` | `Tag:BoneLog` | Posts with that tag |
| `Cat:` | `Cat:Guide` | Posts in that category |
| `Lang:` | `Lang:FA` | Posts with that language |

Combine filters:

```text
blazor Tag:docs Cat:Guide
```

Clicking category, tag, or language in the UI updates the URL (`?q=...`), so filtered views are shareable.

**Pagination:** set `PostsPerPage` in `config.json` (default `10`).

## See also

- [Tags, categories & language](Tags-Categories-Language)
- [Configuration](Configuration)
- [Documentation index](Index)
