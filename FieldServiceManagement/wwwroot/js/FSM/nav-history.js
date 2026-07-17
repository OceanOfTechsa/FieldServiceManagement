// ── Part 1: Track navigation on every page load ────────────────────────────
(function () {
    const STORAGE_KEY = 'navHistory';
    const MAX_ENTRIES = 20;

    function getHistory() {
        try {
            const raw = sessionStorage.getItem(STORAGE_KEY);
            return raw ? JSON.parse(raw) : [];
        } catch (e) {
            console.warn('Failed to parse navHistory, resetting.', e);
            return [];
        }
    }

    function pushCurrentPage() {
        const history = getHistory();

        const currentEntry = {
            url: window.location.pathname + window.location.search,
            title: document.title,
            timestamp: new Date().toISOString()
        };

        const last = history[history.length - 1];
        if (last && last.url === currentEntry.url) return;

        history.push(currentEntry);

        if (history.length > MAX_ENTRIES) {
            history.shift();
        }

        sessionStorage.setItem(STORAGE_KEY, JSON.stringify(history));
    }

    pushCurrentPage();

    window.NavHistory = {
        get: getHistory,
        clear: () => sessionStorage.removeItem(STORAGE_KEY)
    };
})();

// ── Part 2: Render the history dropdown ─────────────────────────────────────
document.addEventListener('DOMContentLoaded', function () {
    const listEl = document.getElementById('fsm-nav-history-list');
    const toggleEl = document.getElementById('fsm-nav-history-toggle');

    if (!listEl || !toggleEl) return;

    new bootstrap.Dropdown(toggleEl, {
        popperConfig: function (defaultBsPopperConfig) {
            return {
                ...defaultBsPopperConfig,
                strategy: 'fixed'
            };
        }
    });

    function renderHistory() {
        const history = window.NavHistory.get();
        listEl.innerHTML = '';

        if (history.length === 0) {
            listEl.innerHTML = `
                <div class="fsm-empty-state-no py-3" >
                    <div class="fsm-empty-icon">
                        <i class="bi bi-file-earmark-break"></i>
                    </div>

                    <p class="fsm-nav-history__empty">
                        No recent pages.
                    </p>
                </div >
            `;
            return;
        }

        const currentUrl = window.location.pathname + window.location.search;
        const ordered = [...history].reverse();

        ordered.forEach(function (entry) {
            const isCurrent = entry.url === currentUrl;

            const li = document.createElement('li');
            const a = document.createElement('a');
            a.className = 'fsm-nav-history__item' + (isCurrent ? ' fsm-nav-history__item--current' : '');
            a.href = entry.url;

            const titleRow = document.createElement('span');
            titleRow.className = 'fsm-nav-history__item-title';

            if (isCurrent) {
                const check = document.createElement('i');
                check.className = 'bi bi-check-circle me-1 fsm-nav-history__check';
                titleRow.appendChild(check);
            }

            const titleText = document.createElement('span');
            titleText.textContent = entry.title || entry.url;
            titleRow.appendChild(titleText);

            const url = document.createElement('span');
            url.className = 'fsm-nav-history__item-url';
            url.textContent = entry.url;

            a.appendChild(titleRow);
            a.appendChild(url);
            li.appendChild(a);
            listEl.appendChild(li);
        });

        const clearLi = document.createElement('li');
        clearLi.className = 'fsm-nav-history__clear';
        clearLi.innerHTML = '<i class="bi bi-trash"></i> Clear history';
        clearLi.addEventListener('click', function (e) {
            e.stopPropagation();
            window.NavHistory.clear();
            renderHistory();
        });
        listEl.appendChild(clearLi);
    }

    toggleEl.addEventListener('click', renderHistory);
});