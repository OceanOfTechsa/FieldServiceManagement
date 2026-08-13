(function () {
    const STORAGE_KEY = 'fsm-theme';

    function getTheme() {
        const stored = localStorage.getItem(STORAGE_KEY);

        if (stored === 'dark' || stored === 'light') {
            return stored;
        }

        return 'light';
    }

    function applyMode(mode) {
        document.documentElement.setAttribute('data-bs-theme', mode);

        document.querySelectorAll('[data-theme-icon]').forEach(el => {
            el.style.display =
                el.getAttribute('data-theme-icon') === mode ? 'block' : 'none';
        });
    }

    applyMode(getTheme());

    document.addEventListener('DOMContentLoaded', function () {
        applyMode(getTheme());
    });

    window.toggleTheme = function () {
        const current = document.documentElement.getAttribute('data-bs-theme');
        const next = current === 'dark' ? 'light' : 'dark';

        localStorage.setItem(STORAGE_KEY, next);
        applyMode(next);
    };
})();