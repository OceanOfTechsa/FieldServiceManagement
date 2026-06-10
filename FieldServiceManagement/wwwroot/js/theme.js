(function () {
    const STORAGE_KEY = 'fsm-theme';

    function applyMode(mode) {
        document.documentElement.setAttribute('data-bs-theme', mode);
        document.querySelectorAll('[data-theme-icon]').forEach(el => {
            el.style.display = el.getAttribute('data-theme-icon') === mode ? 'block' : 'none';
        });
    }

    const stored = localStorage.getItem(STORAGE_KEY)
        ?? (window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light');

    applyMode(stored);

    document.addEventListener('DOMContentLoaded', function () {
        applyMode(localStorage.getItem(STORAGE_KEY)
            ?? (window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light'));
    });

    window.toggleTheme = function () {
        const next = document.documentElement.getAttribute('data-bs-theme') === 'dark' ? 'light' : 'dark';
        localStorage.setItem(STORAGE_KEY, next);
        applyMode(next);
    };
})();