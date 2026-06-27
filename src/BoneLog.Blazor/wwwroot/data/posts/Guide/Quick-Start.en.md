---
id: "00003"
slug: "quick-start"
categoryPath: "Guide"
title: "Quick start"
date: "2026-06-27"
tags: ["BoneLog", "documentation", "github-pages"]
shortDescription: "Fork, configure, and publish on GitHub Pages."
---

## 1. Fork & enable Actions

[Fork BoneLog](https://github.com/Taqiam/BoneLog/fork) and enable **GitHub Actions**.

## 2. Configure

Edit `wwwroot/config.json`:

```json
{
  "BaseDir": "/BoneLog/",
  "Paths": {
    "BaseDataPath": "data/",
    "PostsPath": "posts/",
    "IndexPath": "index.json",
    "AboutMePath": "AboutMe.md"
  },
  "NavItems": [
    { "Title": "Guide", "Url": "posts/en/00001/index" },
    { "Title": "About", "Url": "about" }
  ]
}
```

`BaseDir` must match your GitHub Pages path (`/RepoName/` or `/`). Deploy workflows set `<base href>` from this value.

## 3. Add a post

Create `wwwroot/data/posts/My-Post.en.md`:

```yaml
---
id: "00100"
slug: "my-post"
categoryPath: "Blog"
title: "Hello"
date: "2026-06-27"
tags: ["hello"]
---
Your content here.
```

URL: `/posts/en/00100/my-post`

## 4. Publish

1. **Settings → Pages** → branch `gh-pages`, folder `/`
2. Run **Deploy to GitHub Pages** (or push tag `v1.0.0`)

Post edits on `main` trigger **Update index on main**, then **Update data on gh-pages**.

## Next

- [Writing posts](/posts/en/00004/writing-posts)
- [Publishing](/posts/en/00011/publishing)
- [Documentation hub](/posts/en/00001/index)
