window.localStorageFunctions = {
    setItem: function (key, value) {
        localStorage.setItem(key, value);
    },
    getItem: function (key) {
        return localStorage.getItem(key);
    }
};

window.themeHelper = {
    applyDarkClass: function (isDark) {
        if (isDark) {
            document.getElementById("body").classList.add('dark');
        } else {
            document.getElementById("body").classList.remove('dark');
        }
        if (window.boneLogMarkdown?.render) {
            window.boneLogMarkdown.render();
        }
    }
};
