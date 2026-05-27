---
title: "Configuration reference"
date: "2026-05-27"
tags: ["BoneLog", "documentation"]
language: "EN"
shortDescription: "Every important key in wwwroot/config.json explained."
---

File: `src/BoneLog.Blazor/wwwroot/config.json`

| Key | Description |
|-----|-------------|
| `Title` | Site title in the header. |
| `BaseDir` | App base path (`"/"`, `"/BoneLog/"`). Must match `<base href>` in `index.html` and `404.html`. |
| `PostsPerPage` | Home page pagination size. |
| `Paths.BaseDataPath` | Root for fetching markdown/JSON (`data/` on same host, or full URL if data is hosted separately). |
| `Paths.PostsPath` | Posts folder under data (usually `posts/`). |
| `Paths.IndexPath` | Post list file (`index.json`). |
| `Paths.AboutMePath` | About page markdown (`AboutMe.md`). |
| `NavItems` | Header links: `Title`, `Url`. |
| `SocialLinks` | Footer: `Url`, `IconClass` (Font Awesome). |
| `Features.CategorySidebar` | Show category sidebar on home. |
| `Features.LanguageSidebar` | Show language sidebar. |
| `Features.TagSidebar` | Show tag sidebar. |
| `Features.EnableMultilanguage` | Enable language metadata and `Lang:` search. |

## Example (GitHub Pages)

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

`BaseDir` must match `<base href="/" />` in `index.html` and `404.html` (source uses `/` for local dev; deploy workflows replace it from this value).

## Split website and data

Optional: host WASM on one domain and set `BaseDataPath` to a full URL elsewhere. You must keep `index.json` updated on that data host (GenerateIndex + deploy). Details: [Paths — separating the website and data](Paths#separating-the-website-and-data).

## See also

- [Paths & addresses](Paths) — full guide to URLs, `BaseDir`, images, and relative paths
- [Quick start](Quick-Start)
- [Custom pages & navigation](Custom-Pages)
- [Documentation index](Index)
