---
title: "Writing posts"
date: "2026-05-27"
tags: ["BoneLog", "documentation", "markdown"]
language: "EN"
shortDescription: "Front matter, Markdown features, Mermaid diagrams, and post URLs."
---

Create a `.md` file under `data/posts/`.

## Front matter

```yaml
---
title: "My Post Title"
date: "2026-05-27"
tags: ["dotnet", "blog"]
language: "EN"
cover: "../../images/cover.jpg"
thumbnail: "../../images/thumb.jpg"
shortDescription: "Shown on the home page card."
---
```

## Markdown features

- Headings, lists, tables, blockquotes
- Fenced code blocks (Prism syntax highlighting)
- **Mermaid** diagrams:

````markdown
```mermaid
flowchart LR
  A --> B
```
````

## Post URLs

| File | URL |
|------|-----|
| `posts/Hello.md` | `/post/Hello` |
| `posts/Guide/Intro.md` | `/post/Guide/Intro` |

Posts appear on the home page after `index.json` is regenerated.

## Links between posts

Use **relative** links from the current post file. After the page loads, JavaScript rewrites them to `post/...` routes (same idea as Mermaid rendering, works with `BaseDir` on GitHub Pages).

From `data/posts/Guide/Index.md`:

```markdown
[Quick start](Quick-Start)
[Writing posts](Writing-Posts)
```

From another folder:

```markdown
[Documentation hub](../Guide/Index)
```

Also works with `.md` suffix: `[Quick start](Quick-Start.md)`.

`https://...`, `mailto:...`, and `#heading` anchors are left unchanged.

## See also

- [Images & asset paths](Images)
- [Tags, categories & language](Tags-Categories-Language)
- [Documentation index](Index)
