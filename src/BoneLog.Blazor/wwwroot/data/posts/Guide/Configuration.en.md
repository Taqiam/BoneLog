---
id: "00009"
slug: "configuration"
categoryPath: "Guide"
title: "Configuration"
date: "2026-06-27"
tags: ["BoneLog", "documentation"]
shortDescription: "config.json reference."
---

File: `wwwroot/config.json`

## Main keys

| Key | Description |
|-----|-------------|
| `Title` | Site title in header |
| `BaseDir` | App base path — `"/"` or `"/BoneLog/"`. Must match `<base href>`. |
| `PostsPerPage` | Home page pagination (default 10) |
| `Paths.BaseDataPath` | `data/` or full URL to data host |
| `Paths.PostsPath` | Usually `"posts/"` |
| `Paths.IndexPath` | Usually `"index.json"` |
| `Paths.AboutMePath` | Usually `"AboutMe.md"` |
| `NavItems` | Header links — `{ "Title", "Url" }` |
| `SocialLinks` | Footer icons — `{ "Url", "IconClass" }` |
| `Features.CategorySidebar` | Category filter on home |
| `Features.TagSidebar` | Tag filter on home |

## NavItems

```json
"NavItems": [
  { "Title": "Guide", "Url": "posts/en/00001/index" },
  { "Title": "About", "Url": "about" },
  { "Title": "GitHub", "Url": "https://github.com/Taqiam/BoneLog" }
]
```

- App routes: no leading slash (`about`, `posts/en/00001/index`)
- External: full `https://` URL

## Example (GitHub Pages subpath)

```json
{
  "Title": "My Blog",
  "BaseDir": "/BoneLog/",
  "PostsPerPage": 10,
  "Paths": {
    "BaseDataPath": "data/",
    "PostsPath": "posts/",
    "IndexPath": "index.json",
    "AboutMePath": "AboutMe.md"
  }
}
```

## Next

- [Paths & URLs](/posts/en/00007/paths)
- [Custom pages](/posts/en/00010/custom-pages)
