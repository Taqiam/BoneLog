(function () {
    function combineRelative(baseDirectory, relativePath) {
        var stack = baseDirectory.replace(/\\/g, "/").replace(/^\/+|\/+$/g, "").split("/").filter(Boolean);
        var parts = relativePath.replace(/\\/g, "/").split("/");
        for (var i = 0; i < parts.length; i++) {
            var part = parts[i];
            if (part === "..") {
                if (stack.length > 0) {
                    stack.pop();
                }
            } else if (part && part !== ".") {
                stack.push(part);
            }
        }
        return stack.join("/");
    }

    function getPostMarkdownDirectory(postPath, postsPrefix) {
        var normalized = postPath.replace(/\\/g, "/").replace(/^\/+|\/+$/g, "");
        var segments = normalized.split("/");
        if (segments.length > 1) {
            segments.pop();
        } else {
            segments = [];
        }
        var folder = segments.join("/");
        return folder ? postsPrefix + "/" + folder : postsPrefix;
    }

    function getPageDirectory(pagePath) {
        var normalized = pagePath.replace(/\\/g, "/");
        var lastSlash = normalized.lastIndexOf("/");
        return lastSlash >= 0 ? normalized.substring(0, lastSlash) : "";
    }

    function shouldKeepHref(href) {
        if (!href) {
            return true;
        }
        if (href.charAt(0) === "#") {
            return true;
        }
        var lower = href.toLowerCase();
        if (lower.indexOf("http://") === 0 || lower.indexOf("https://") === 0 || lower.indexOf("mailto:") === 0 || lower.indexOf("tel:") === 0 || lower.indexOf("//") === 0) {
            return true;
        }
        if (href.charAt(0) === "/") {
            return true;
        }
        if (lower.indexOf("post/") === 0) {
            return true;
        }
        return false;
    }

    function resolveContentHref(href, contentPath, contentKind, postsPrefix) {
        if (shouldKeepHref(href)) {
            return href;
        }

        var fragment = "";
        var path = href;
        var hashIndex = href.indexOf("#");
        if (hashIndex >= 0) {
            fragment = href.substring(hashIndex);
            path = href.substring(0, hashIndex);
            if (!path) {
                return href;
            }
        }

        var baseDir = contentKind === "post"
            ? getPostMarkdownDirectory(contentPath || "", postsPrefix)
            : getPageDirectory(contentPath || "");

        var dataRelative = combineRelative(baseDir, path);
        var normalized = dataRelative.replace(/^\/+/, "");
        if (normalized.length > 3 && normalized.substring(normalized.length - 3).toLowerCase() === ".md") {
            normalized = normalized.substring(0, normalized.length - 3);
        }

        if (normalized === postsPrefix) {
            return "post/" + fragment;
        }
        if (normalized.indexOf(postsPrefix + "/") === 0) {
            return "post/" + normalized.substring(postsPrefix.length + 1) + fragment;
        }

        return normalized + fragment;
    }

    function fixLinks(container) {
        var contentPath = container.getAttribute("data-content-path") || "";
        var contentKind = container.getAttribute("data-content-kind") || "post";
        var postsPrefix = (container.getAttribute("data-posts-prefix") || "posts").replace(/^\/+|\/+$/g, "");

        container.querySelectorAll("a[href]").forEach(function (anchor) {
            var href = anchor.getAttribute("href");
            if (!href) {
                return;
            }
            var resolved = resolveContentHref(href, contentPath, contentKind, postsPrefix);
            if (resolved !== href) {
                anchor.setAttribute("href", resolved);
            }
        });
    }

    async function renderMermaid() {
        if (!window.mermaid) {
            return;
        }
        var isDark = document.getElementById("body") && document.getElementById("body").classList.contains("dark");
        mermaid.initialize({
            startOnLoad: false,
            theme: isDark ? "dark" : "default",
            securityLevel: "loose"
        });
        var nodes = document.querySelectorAll(".markdown-content .mermaid");
        nodes.forEach(function (node) {
            var source = node.getAttribute("data-source");
            if (source) {
                node.textContent = source;
            }
            node.removeAttribute("data-processed");
        });
        if (nodes.length > 0) {
            await mermaid.run({ nodes: Array.from(nodes) });
        }
    }

    window.boneLogMarkdown = {
        render: async function () {
            if (window.Prism) {
                Prism.highlightAll();
            }
            document.querySelectorAll(".markdown-content").forEach(fixLinks);
            await renderMermaid();
        }
    };
})();
