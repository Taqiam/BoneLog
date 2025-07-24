# BoneLOG
### Easy to use, Free for all
---

## What's this?

Wanna spin up a small blog on GitHub Pages real quick and post your stuff with zero hassle?  
**BoneLOG** is made just for that.

Just write your posts in Markdown, push them, and boom — they go live automatically.  
No build tools, no CMS, no configs. Just files.

## Getting Started

1. Fork this repo
2. Enable GitHub Actions in your fork
3. Add your Markdown posts to:
   `/src/BoneLog.Blazor/wwwroot/data`
4. Enable GitHub Pages:
   - Go to **Settings > Pages**
   - Choose the `gh-pages` branch as source
5. (Optional) If you renamed the repo or use a custom domain:
   - Edit the `base href` in `.github/workflows/full-build.yml`:
     ```yaml
     sed -i 's|<base href="/" />|<base href="/YourRepoName/" />|g' ...
     ```
   - Or remove these lines if you're using a custom domain.

> [!NOTE]
> posts.json is generated automatically after each push if GitHub Actions is enabled.

## Customization

All data and content are inside:  
[`/src/BoneLog.Blazor/wwwroot/data`](...)

- To add blog posts → Put `.md` files inside `/data/posts`
- To create custom pages → Create folders/files like:
  `/data/projects/project1.md` → will be accessible at `yourdomain.com/data/projects/project1`
- To change site settings (title, navbar, social links, etc.) → Edit `site-settings.json`

## Contribute
Dev doors are always open!
Got a small idea or a feature that might help others too?
Open a PR and let’s make it happen.

### A final note
Sure — there are tons of features you could add.
But honestly? This project is meant to stay small, file-based, and simple.

Let’s not over-architect something that works beautifully in its simplicity.

That said, if you’ve built something cool and think it belongs here —
send a PR. I'd love to see it.
