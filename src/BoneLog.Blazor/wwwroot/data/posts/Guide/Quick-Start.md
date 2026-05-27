---
title: "Quick start on GitHub Pages"
date: "2026-05-27"
tags: ["BoneLog", "documentation", "github-pages"]
language: "EN"
shortDescription: "Fork, configure, enable Pages, and deploy BoneLog in a few steps."
---

## 1. Fork and enable Actions

Fork [BoneLog on GitHub](https://github.com/Taqiam/BoneLog), then enable **Actions** for your repository.

## 2. Add content

| What | Where |
|------|--------|
| Blog posts | `src/BoneLog.Blazor/wwwroot/data/posts/` |
| About page | `src/BoneLog.Blazor/wwwroot/data/AboutMe.md` |
| Site settings | `src/BoneLog.Blazor/wwwroot/config.json` |
| Shared images | `src/BoneLog.Blazor/wwwroot/images/` |

Example: `data/posts/Guide/My-Post.md` → URL `/post/Guide/My-Post`

## 3. Configure for your URL

Edit `config.json`:

```json
{
  "BaseDir": "/YourRepoName/",
  "Paths": {
    "BaseDataPath": "data/",
    "PostsPath": "posts/",
    "IndexPath": "index.json",
    "AboutMePath": "AboutMe.md"
  }
}
```

| Setting | GitHub project site | Custom domain |
|---------|---------------------|---------------|
| `BaseDir` | `"/YourRepoName/"` (e.g. `"/BoneLog/"`) | `"/"` |
| `BaseDataPath` | `"data/"` | `"data/"` |

`BaseDir` must start and end with `/`. It must match the `<base href>` in `index.html` and `404.html`. The **Deploy to GitHub Pages** and **Release** workflows set both from `config.json` when publishing; for manual deploys, edit those tags yourself (or run `bash scripts/set-base-href.sh path/to/wwwroot` after publish).

## 4. Enable GitHub Pages

1. **Settings → Pages**
2. Source: branch **`gh-pages`**, folder **`/` (root)**

## 5. First deploy

- **Actions → Deploy to GitHub Pages → Run workflow**, or
- Push a tag: `git tag v1.0.0 && git push origin v1.0.0`

Open `https://<user>.github.io/<repo>/`.

## 6. Day-to-day updates

1. Edit `.md` files under `data/posts/`.
2. Push to `main` → **Update index on main** refreshes `index.json`.
3. Run **Deploy to GitHub Pages** again (or tag a release) to update the live site.

## See also

- [Publishing & releases](Publishing)
- [Configuration](Configuration)
- [Documentation index](Index)
