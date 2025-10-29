// NOTE: localStorage key must match that in _Layout.cshtml <head>
const COLOR_SCHEME_KEY = 'color-scheme';

function setTheme(theme, persist) {
    document.documentElement.setAttribute('data-bs-theme', theme);
    if (persist) localStorage.setItem(COLOR_SCHEME_KEY, theme);
    syncIcon(theme);
}

function syncIcon(theme) {
    var moonIcon = document.getElementById('moon-icon');
    var sunIcon = document.getElementById('sun-icon');
    if (!moonIcon || !sunIcon) return;
    moonIcon.classList.toggle('active-theme', theme === 'dark');
    sunIcon.classList.toggle('active-theme', theme !== 'dark');
}

document.addEventListener('DOMContentLoaded', function () {
    syncIcon(document.documentElement.getAttribute('data-bs-theme') || 'light');

    var themeToggle = document.getElementById('theme-toggle');
    if (themeToggle) {
        themeToggle.addEventListener('click', function () {
            var current = document.documentElement.getAttribute('data-bs-theme') || 'light';
            setTheme(current === 'dark' ? 'light' : 'dark', true);
        });
    }
});

// Follow system preference changes only when no manual preference is stored
window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', function (e) {
    if (!localStorage.getItem(COLOR_SCHEME_KEY)) {
        setTheme(e.matches ? 'dark' : 'light', false);
    }
});