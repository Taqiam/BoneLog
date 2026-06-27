---
id: "00012"
slug: "full-custom-hosting"
categoryPath: "Guide"
title: "Custom hosting"
date: "2026-06-27"
tags: ["BoneLog", "documentation", "hosting"]
shortDescription: "Release zip, any host, optional separate data repo."
---

Use this when you are **not** using the built-in GitHub Actions deploy.

## 1. Get a build

Download a [release zip](https://github.com/Taqiam/BoneLog/releases) or run locally:

```bash
dotnet publish src/BoneLog.Blazor/BoneLog.Blazor.csproj -c Release
```

Upload the publish output to your static host.

## 2. SPA fallback

Unknown routes must serve `index.html` so `/posts/...` works on refresh.

## 3. config.json

```json
{
  "BaseDir": "/",
  "Paths": {
    "BaseDataPath": "data/",
    "PostsPath": "posts/",
    "IndexPath": "index.json",
    "AboutMePath": "AboutMe.md"
  }
}
```

For a subpath site: `"BaseDir": "/my-blog/"` and match `<base href>`.

## 4. Same host vs split data

**Same host** — `"BaseDataPath": "data/"`. Upload `data/` next to the WASM app.

**Split data** — host markdown + `index.json` elsewhere; set `BaseDataPath` to that URL. Enable CORS on the data host.

## 5. Regenerate index

After editing posts:

```bash
dotnet run scripts/GenerateIndex.cs -- \
  wwwroot/data/posts \
  wwwroot/data/index.json
```

Upload `index.json`, `posts/`, and `index.generation.log.json`.

Full reference: [Configuration](/posts/en/00009/configuration) · [Paths](/posts/en/00007/paths)

## Next

- [Publishing](/posts/en/00011/publishing)
- [Workflows](/posts/en/00013/workflows)
