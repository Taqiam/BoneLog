---
title: "Developer overview"
date: "2026-05-27"
tags: ["BoneLog", "documentation", "developers"]
language: "EN"
shortDescription: "Short architecture map for contributors — projects, pipeline, routes."
---

```mermaid
flowchart TB
  subgraph client ["Blazor WASM"]
    Pages["Home, Post, About, CatchAll"]
    MD["MarkdownContent"]
    Pages --> MD
  end
  subgraph core ["BoneLog library"]
    Markdig["MarkdownExtensions"]
    Paths["ContentPathExtensions"]
  end
  subgraph static ["wwwroot"]
    DATA["data/, config.json"]
  end
  Pages --> core
  client -->|HttpClient| DATA
```

## Projects

| Project | Role |
|---------|------|
| `BoneLog` | Models, `BlogContentProvider`, markdown/path helpers. |
| `BoneLog.Blazor` | UI, static data, publish target. |
| `scripts/GenerateIndex.cs` | Builds `index.json` from front matter. |

## Content pipeline

1. Markdown + YAML → `MarkdownExtensions.ToPost` / `ToHtml`.
2. Mermaid → `<div class="mermaid">` → `boneLogMarkdown.render()` in the browser.
3. Relative assets → `ContentPathExtensions` → URLs under `data/` or `images/`.

## Routes

| Component | Route |
|-----------|--------|
| `PostPage.razor` | `/post/{*PostPath}` |
| `CatchAll.razor` | `/{**slug}` → `data/{slug}.md` |
| `Home.razor` | `/` |
| `About.razor` | `/about` |

## See also

- [Documentation index](Index)
- [Repository](https://github.com/Taqiam/BoneLog)
