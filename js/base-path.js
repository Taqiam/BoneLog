(function () {
    function normalizeBaseDir(value) {
        var dir = (value || "/").trim();
        if (!dir)
            dir = "/";
        if (dir.charAt(0) !== "/")
            dir = "/" + dir;
        if (dir.charAt(dir.length - 1) !== "/")
            dir += "/";
        return dir;
    }

    function applyBaseHref(dir) {
        var base = document.querySelector("base");
        if (!base) {
            base = document.createElement("base");
            document.head.insertBefore(base, document.head.firstChild);
        }
        base.setAttribute("href", dir);
    }

    function loadBaseDir() {
        try {
            var xhr = new XMLHttpRequest();
            xhr.open("GET", "config.json", false);
            xhr.send(null);
            if (xhr.status >= 200 && xhr.status < 300) {
                var config = JSON.parse(xhr.responseText);
                return normalizeBaseDir(config.BaseDir);
            }
        } catch (e) {
            console.warn("BoneLog: could not read BaseDir from config.json", e);
        }
        return "/";
    }

    applyBaseHref(loadBaseDir());
})();
