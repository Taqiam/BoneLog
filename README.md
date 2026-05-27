# BoneLOG

**Live site:** [https://taqiam.github.io/BoneLog/](https://taqiam.github.io/BoneLog/)

**Documentation:** [BoneLog docs (start here)](https://taqiam.github.io/BoneLog/post/Guide/Index)

---

## What's this?

A small **file-based blog** on **Blazor WebAssembly**, write Markdown, push to Git, deploy static files. No CMS, no database.

## Quick start

1. [Fork](https://github.com/Taqiam/BoneLog/fork) this repo and enable **GitHub Actions**.
2. Add posts under [`src/BoneLog.Blazor/wwwroot/data/posts`](src/BoneLog.Blazor/wwwroot/data/posts).
3. Edit [`config.json`](src/BoneLog.Blazor/wwwroot/config.json), set `BaseDir` (e.g. `"/BoneLog/"`) and `BaseDataPath` (`"data/"`). Deploy workflows patch `<base href>` in `index.html` and `404.html` from `BaseDir`.
4. **Settings → Pages** → branch **`gh-pages`**, root **`/`**.
5. Deploy: **Actions → Deploy to GitHub Pages**, or push tag `v1.0.0`.

Push post changes → **Update index on main** refreshes `index.json`. Deploy again to update the live site.

## Documentation posts

| Topic | Link |
|-------|------|
| **Index** | [Documentation hub](https://taqiam.github.io/BoneLog/post/Guide/Index) |
| Quick start | [GitHub Pages](https://taqiam.github.io/BoneLog/post/Guide/Quick-Start) |
| Publishing | [Deploy & hosting](https://taqiam.github.io/BoneLog/post/Guide/Publishing) |
| Custom hosting | [Release zip & any host](https://taqiam.github.io/BoneLog/post/Guide/Full-Custom-Hosting) |
| Writing | [Posts & Markdown](https://taqiam.github.io/BoneLog/post/Guide/Writing-Posts) |
| Paths | [Paths & addresses](https://taqiam.github.io/BoneLog/post/Guide/Paths) |
| Search | [Filters](https://taqiam.github.io/BoneLog/post/Guide/Search-and-Filter) |
| Config | [config.json](https://taqiam.github.io/BoneLog/post/Guide/Configuration) |
| Workflows | [GitHub Actions](https://taqiam.github.io/BoneLog/post/Guide/Workflows) |
| Developers | [Architecture](https://taqiam.github.io/BoneLog/post/Guide/Developer-Overview) |

## Customize

| Goal | Location |
|------|----------|
| Posts | `wwwroot/data/posts/` |
| About | `wwwroot/data/AboutMe.md` |
| Settings & nav | `wwwroot/config.json` |
| Images | `wwwroot/images/` |
| Styles | `wwwroot/css/app.css` |

## Contribute

PRs welcome, keep small and clear.