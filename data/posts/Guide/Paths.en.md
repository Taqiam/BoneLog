---
id: "00007"
slug: "paths"
categoryPath: "Guide"
title: "Paths & URLs"
date: "2026-06-27"
tags: ["BoneLog", "documentation"]
shortDescription: "BaseDir, post URLs, nav links, images, and data paths."
---

Three path layers:

| Layer | Example |
|-------|---------|
| **Browser URL** | `https://user.github.io/BoneLog/posts/en/00007/paths` |
| **App route** | `posts/en/00007/paths` (relative to `BaseDir`) |
| **Data file** | `data/posts/Guide/Paths.en.md` |

## config.json

```json
{
  "BaseDir": "/BoneLog/",
  "Paths": {
    "BaseDataPath": "data/",
    "PostsPath": "posts/",
    "IndexPath": "index.json",
    "AboutMePath": "AboutMe.md"
  }
}
```

| Key | Purpose |
|-----|---------|
| `BaseDir` | Must match `<base href>` in `index.html` / `404.html` |
| `BaseDataPath` | Where `index.json` and posts load from (`data/` or full URL) |
| `PostsPath` | Prefix for post files under data (usually `posts/`) |

## Post URLs

Pattern: `/posts/{language}/{id}/{slug}`

No leading slash in **NavItems** (`about`, `posts/en/00001/index`) so `BaseDir` applies.

## Markdown links

```markdown
[Configuration](/posts/en/00009/configuration)
[About](/about)
[External](https://example.com)
```

- **`/posts/en/...`** — post pages (leading `/` is normalized for `BaseDir`)
- **`https://...`** — unchanged
- **Custom pages** — `legal/Privacy` → `data/legal/Privacy.md`

## Images

Site assets use a **root path** from `wwwroot`:

```yaml
thumbnail: "/images/Logo.png"
```

```markdown
![Logo](/images/Logo.png)
```

Full URLs (`https://...`) are unchanged. See [Images](/posts/en/00008/images).

## Split site + data

Host WASM on one domain, data on another — set `BaseDataPath` to a full URL. Regenerate and upload `index.json` + `posts/` to the data host. See [Custom hosting](/posts/en/00012/full-custom-hosting).

## Next

- [Configuration](/posts/en/00009/configuration)
- [Images](/posts/en/00008/images)
- [Custom pages](/posts/en/00010/custom-pages)
