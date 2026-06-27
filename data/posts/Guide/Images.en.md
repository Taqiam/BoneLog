---
id: "00008"
slug: "images"
categoryPath: "Guide"
title: "Images"
date: "2026-06-27"
tags: ["BoneLog", "documentation"]
shortDescription: "Image paths in posts and front matter."
---

Put shared images in `wwwroot/images/`.

## Path rules

| Path | Example | Result |
|------|---------|--------|
| **Site root** | `/images/Logo.png` | `images/Logo.png` (relative to `BaseDir` / `<base href>`) |
| **Full URL** | `https://cdn.example/x.jpg` | Unchanged |

Use a leading **`/`** for site assets under `wwwroot`. Do not use `../` or paths relative to the post file.

## Front matter

```yaml
thumbnail: "/images/thumb.jpg"
cover: "/images/cover.jpg"
```

## Inline markdown

```markdown
![Logo](/images/Logo.png)
```

## Next

- [Paths & URLs](/posts/en/00007/paths)
- [Writing posts](/posts/en/00004/writing-posts)
