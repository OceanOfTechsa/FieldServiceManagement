// ── Part 1: Track navigation on every page load ────────────────────────────
(function () {
    const BASE_KEY = 'navHistory';
    const MAX_ENTRIES = 20;

    function hashString(str) {
        let hash = 5381;
        for (let i = 0; i < str.length; i++) {
            hash = ((hash << 5) + hash) + str.charCodeAt(i);
            hash = hash & hash;
        }
        return (hash >>> 0).toString(36);
    }

    function getUserKey() {
        const raw = (document.body?.dataset?.userKey || 'anonymous').trim().toLowerCase();
        return raw === 'anonymous' ? raw : hashString(raw);
    }

    function getStorageKey() {
        return `${BASE_KEY}:${getUserKey()}`;
    }

    function getPointerKey() {
        return `${BASE_KEY}:pointer:${getUserKey()}`;
    }

    function getFlagKey() {
        return `${BASE_KEY}:navflag:${getUserKey()}`;
    }

    function getHistory() {
        try {
            const raw = sessionStorage.getItem(getStorageKey());
            return raw ? JSON.parse(raw) : [];
        } catch (e) {
            console.warn('Failed to parse navHistory, resetting.', e);
            return [];
        }
    }

    function getPointer(history) {
        const raw = sessionStorage.getItem(getPointerKey());
        const idx = raw !== null ? parseInt(raw, 10) : history.length - 1;
        if (history.length === 0) return -1;
        return Math.max(0, Math.min(idx, history.length - 1));
    }

    function setPointer(idx) {
        sessionStorage.setItem(getPointerKey(), String(idx));
    }

    function pushCurrentPage() {
        const history = getHistory();

        // If this load came from goBack/goForward, don't push — just consume the flag.
        if (sessionStorage.getItem(getFlagKey()) === '1') {
            sessionStorage.removeItem(getFlagKey());
            return;
        }

        const currentEntry = {
            url: window.location.pathname + window.location.search,
            title: document.title,
            timestamp: new Date().toISOString()
        };

        let pointer = getPointer(history);

        // Normal (non back/forward) navigation drops any "forward" entries ahead of us —
        // same behavior as a real browser history stack.
        if (pointer < history.length - 1) {
            history.length = pointer + 1;
        }

        const last = history[history.length - 1];
        if (last && last.url === currentEntry.url) {
            setPointer(history.length - 1);
            return;
        }

        history.push(currentEntry);

        if (history.length > MAX_ENTRIES) {
            history.shift();
        }

        sessionStorage.setItem(getStorageKey(), JSON.stringify(history));
        setPointer(history.length - 1);
    }

    pushCurrentPage();

    function goBack() {
        const history = getHistory();
        const pointer = getPointer(history);
        if (pointer <= 0) return false;
        const target = history[pointer - 1];
        setPointer(pointer - 1);
        sessionStorage.setItem(getFlagKey(), '1');
        window.location.href = target.url;
        return true;
    }

    function goForward() {
        const history = getHistory();
        const pointer = getPointer(history);
        if (pointer === -1 || pointer >= history.length - 1) return false;
        const target = history[pointer + 1];
        setPointer(pointer + 1);
        sessionStorage.setItem(getFlagKey(), '1');
        window.location.href = target.url;
        return true;
    }

    function goToIndex(idx) {
        const history = getHistory();
        if (idx < 0 || idx >= history.length) return false;
        setPointer(idx);
        sessionStorage.setItem(getFlagKey(), '1');
        window.location.href = history[idx].url;
        return true;
    }

    window.NavHistory = {
        get: getHistory,
        clear: () => {
            sessionStorage.removeItem(getStorageKey());
            sessionStorage.removeItem(getPointerKey());
            sessionStorage.removeItem(getFlagKey());
        },
        goBack,
        goForward,
        goToIndex,
        canGoBack: () => getPointer(getHistory()) > 0,
        canGoForward: () => {
            const h = getHistory();
            const p = getPointer(h);
            return p !== -1 && p < h.length - 1;
        },
        getPointer: () => getPointer(getHistory())
    };
})();

// ── Part 2: Render the history dropdown ─────────────────────────────────────
document.addEventListener('DOMContentLoaded', function () {
    const listEl = document.getElementById('fsm-nav-history-list');
    const toggleEl = document.getElementById('fsm-nav-history-toggle');
    const backBtn = document.getElementById('fsm-nav-back-btn');
    const forwardBtn = document.getElementById('fsm-nav-forward-btn');

    function updateNavButtons() {
        const canBack = window.NavHistory.canGoBack();
        const canForward = window.NavHistory.canGoForward();

        if (backBtn) {
            backBtn.classList.toggle('disabled', !canBack);
            backBtn.setAttribute('aria-disabled', String(!canBack));
        }

        if (forwardBtn) {
            forwardBtn.classList.toggle('disabled', !canForward);
            forwardBtn.setAttribute('aria-disabled', String(!canForward));
        }
    }

    updateNavButtons();

    if (backBtn) {
        backBtn.addEventListener('click', function () {
            if (backBtn.classList.contains('disabled')) return;
            window.NavHistory.goBack();
        });
    }

    if (forwardBtn) {
        forwardBtn.addEventListener('click', function () {
            if (forwardBtn.classList.contains('disabled')) return;
            window.NavHistory.goForward();
        });
    }

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

        history.forEach(function (entry, idx) {
            const isCurrent = entry.url === currentUrl;

            const li = document.createElement('li');
            const a = document.createElement('a');
            a.className = 'fsm-nav-history__item' + (isCurrent ? ' fsm-nav-history__item--current' : '');
            a.href = entry.url;

            a.addEventListener('click', function (e) {
                e.preventDefault();
                window.NavHistory.goToIndex(idx);
            });

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
            updateNavButtons();
        });
        listEl.appendChild(clearLi);
    }

    toggleEl.addEventListener('click', renderHistory);
});