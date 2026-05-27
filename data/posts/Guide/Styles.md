---
title: "Styles & customization"
date: "2026-05-27"
tags: ["BoneLog", "documentation", "css"]
language: "EN"
shortDescription: "Themes, app.css, Prism, dark mode, and RTL markdown."
---

| Area | Location |
|------|----------|
| Global theme, layout, markdown | `wwwroot/css/app.css` |
| Code highlighting | `wwwroot/css/prism-tomorrow-night.css` |
| Dark mode | Header toggle → `dark` class on `<body>` |
| RTL text | Auto `dir="rtl"` on paragraphs starting with Arabic/Persian |

## Tips

- Prefer CSS variables in `app.css` for colors (light/dark and markdown blocks).
- Edit source files, not the published `gh-pages` output.
- Rebuild and redeploy after style changes.

## See also

- [Configuration](Configuration)
- [Documentation index](Index)
