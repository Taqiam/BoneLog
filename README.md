# BoneLOG

**Live site:** [https://taqiam.github.io/BoneLog/](https://taqiam.github.io/BoneLog/)

**Documentation:** [Start here](https://taqiam.github.io/BoneLog/posts/en/00001/index)

---

## What's this?

A **file-based Blazor WASM blog** — Markdown in, static site out. Posts use numeric **ids**, **slugs**, and optional language files (`Post.en.md`, `Post.fa.md`).

## Quick start

1. [Fork](https://github.com/Taqiam/BoneLog/fork) and enable **GitHub Actions**
2. Add posts under `wwwroot/data/posts/` with `id`, `slug`, `categoryPath` in front matter
3. Set `BaseDir` and `Paths` in [`config.json`](src/BoneLog.Blazor/wwwroot/config.json)
4. **Settings → Pages** → `gh-pages` / `/`
5. Run **Deploy to GitHub Pages** or push tag `v1.0.0`

## Documentation

| Topic | Link |
|-------|------|
| **Hub** | [Documentation](https://taqiam.github.io/BoneLog/posts/en/00001/index) |
| Quick start | [00003](https://taqiam.github.io/BoneLog/posts/en/00003/quick-start) |
| Writing | [00004](https://taqiam.github.io/BoneLog/posts/en/00004/writing-posts) |
| Paths | [00007](https://taqiam.github.io/BoneLog/posts/en/00007/paths) |
| Config | [00009](https://taqiam.github.io/BoneLog/posts/en/00009/configuration) |
| Publishing | [00011](https://taqiam.github.io/BoneLog/posts/en/00011/publishing) |
| Developers | [00015](https://taqiam.github.io/BoneLog/posts/en/00015/developer-overview) |

## Customize

| Goal | Location |
|------|----------|
| Posts | `wwwroot/data/posts/` |
| About | `wwwroot/data/AboutMe.md` |
| Settings | `wwwroot/config.json` |
| Styles | `wwwroot/css/app.css` |

PRs welcome.
