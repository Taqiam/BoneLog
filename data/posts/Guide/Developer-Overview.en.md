---
id: "00015"
slug: "developer-overview"
categoryPath: "Guide"
title: "Developer overview"
date: "2026-06-27"
tags: ["BoneLog", "documentation", "developers"]
shortDescription: "Architecture for contributors."
---

## Projects

| Project | Role |
|---------|------|
| `BoneLog` | Models, `BlogContentProvider`, markdown/path helpers |
| `BoneLog.Blazor` | WASM UI, static `wwwroot` |
| `scripts/GenerateIndex.cs` | Builds grouped `index.json` from front matter |

## Pipeline

1. Markdown + YAML → `MarkdownExtensions.ToPost`
2. Non-English posts merge English metadata (`PostFrontMatter.MergeForLanguage`)
3. Relative assets → root `/images/...` or full URL (`ContentPathExtensions.ResolveAssetUrl`)
4. Post links in markdown → `/posts/{lang}/{id}/{slug}` (see [Writing posts](/posts/en/00004/writing-posts))

## Index entry

One row per logical post (grouped by `id`):

```json
{
  "id": "00007",
  "slug": "paths",
  "languages": ["en"],
  "filePaths": { "en": "Guide/Paths.en" }
}
```

## Routes

| Route | Page |
|-------|------|
| `/posts/{Language}/{Id}/{Slug}` | Post |
| `/about` | About |
| `/{**slug}` | Custom page (`data/{slug}.md`) |
| `/` | Home |

## Next

- [Documentation hub](/posts/en/00001/index)
- [GitHub](https://github.com/Taqiam/BoneLog)
