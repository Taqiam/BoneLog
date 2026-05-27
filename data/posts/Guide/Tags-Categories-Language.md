---
title: "Tags, categories & language"
date: "2026-05-27"
tags: ["BoneLog", "documentation"]
language: "EN"
shortDescription: "Organize posts with folders, tags, language codes, and sidebar toggles."
---

## Language

- Set `language` in front matter (e.g. `"EN"`, `"FA"`).
- Enable in `config.json`: `"EnableMultilanguage": true` and `"LanguageSidebar": true`.
- Search with `Lang:EN` (see [Search & filters](Search-and-Filter)).

## Tags

```yaml
tags: ["BoneLog", "docs"]
```

- `"TagSidebar": true` shows tags on the home page.
- Search: `Tag:BoneLog` or click a tag in the UI.

## Categories

- **Automatic:** folder structure → category name.  
  `posts/Tutorials/Intro.md` → category **Tutorials**.
- Search: `Cat:Tutorials` or use the category sidebar.

## Toggle sidebars

In `config.json` → `Features`:

```json
"Features": {
  "CategorySidebar": true,
  "LanguageSidebar": true,
  "TagSidebar": true,
  "EnableMultilanguage": true
}
```

Set any to `false` to hide that UI.

## See also

- [Search & filters](Search-and-Filter)
- [Writing posts](Writing-Posts)
- [Documentation index](Index)
