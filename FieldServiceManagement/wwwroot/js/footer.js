
window.FsmFooterPopup = (() => {
    let hideTimer = null;
    let cleanupTimer = null;

    const DEFAULT_DURATION = 3200;
    const EXIT_DURATION = 800;
    const HOVER_HIDE_DURATION = 2000;

    const VARIANTS = {
        info: { icon: 'bi-info-circle-fill', cls: 'is-info' },
        success: { icon: 'bi-check-circle-fill', cls: 'is-success' },
        warning: { icon: 'bi-exclamation-triangle-fill', cls: 'is-warning' },
        error: { icon: 'bi-x-circle-fill', cls: 'is-error' }
    };

    function getEls() {
        const root = document.getElementById('fsm-footer-status');
        const popup = document.getElementById('fsm-footer-popup');

        if (!root || !popup) {
            return null;
        }

        return {
            root,
            popup,
            header: popup.querySelector('.fsm-footer-popup-header'),
            icon: popup.querySelector('.fsm-footer-popup-icon'),
            title: popup.querySelector('.fsm-footer-popup-title'),
            meta: popup.querySelector('.fsm-footer-popup-meta'),
            body: popup.querySelector('.fsm-footer-popup-body')
        };
    }

    function clearHideTimer() {
        if (hideTimer) {
            clearTimeout(hideTimer);
            hideTimer = null;
        }
    }

    function clearCleanupTimer() {
        if (cleanupTimer) {
            clearTimeout(cleanupTimer);
            cleanupTimer = null;
        }
    }

    function scheduleHide(duration) {
        clearHideTimer();
        hideTimer = setTimeout(() => hide(), duration);
    }

    function show({
        title = '',
        content = '',
        justNow = 'Just now',
        type = 'info',
        duration = DEFAULT_DURATION
    } = {}) {
        const els = getEls();
        if (!els) return;

        clearHideTimer();
        clearCleanupTimer();

        // Cancel any previous exit animation.
        els.popup.classList.remove('is-hiding', 'is-visible');

        // Reset previous variants.
        els.popup.classList.remove(
            'is-info',
            'is-success',
            'is-warning',
            'is-error'
        );

        const variant = VARIANTS[type] || VARIANTS.info;
        els.popup.classList.add(variant.cls);

        // Update header.
        if (els.header) {
            els.header.classList.remove('d-none');

            if (els.icon) {
                els.icon.className = `fsm-footer-popup-icon bi ${variant.icon}`;
            }

            if (els.title) {
                els.title.textContent = title;
            }

            // if (els.meta) {
            //     els.meta.textContent = justNow;
            // }
        }

        // Update content.
        if (els.body) {
            els.body.innerHTML = content;
        }

        // Temporarily make visible for accurate measurement.
        els.popup.style.visibility = 'hidden';
        els.popup.style.opacity = '0';

        els.popup.classList.add('is-visible');
        const popupWidth = els.popup.offsetWidth || 320;

        // Reset before applying the actual position.
        els.popup.classList.remove('is-visible');
        void els.popup.offsetWidth;

        els.popup.style.visibility = '';
        els.popup.style.opacity = '';

        // Position above the status bar.
        const statusRect = els.root.getBoundingClientRect();
        const gap = 12;

        let left = statusRect.left + (statusRect.width / 2) - (popupWidth / 2);

        left = Math.max(
            16,
            Math.min(left, window.innerWidth - popupWidth - 16)
        );

        const bottom = window.innerHeight - statusRect.top + gap;

        els.popup.style.left = `${left}px`;
        els.popup.style.right = 'auto';
        els.popup.style.bottom = `${bottom}px`;
        els.popup.style.top = 'auto';

        // Trigger enter animation.
        els.popup.setAttribute('aria-hidden', 'false');

        void els.popup.offsetWidth;

        els.popup.classList.add('is-visible');

        scheduleHide(duration);
    }

    function hide() {
        const els = getEls();
        if (!els) return;

        clearHideTimer();

        if (!els.popup.classList.contains('is-visible')) {
            return;
        }

        // Start the slow fade-up animation.
        els.popup.classList.remove('is-visible');
        els.popup.classList.add('is-hiding');

        els.popup.setAttribute('aria-hidden', 'true');

        clearCleanupTimer();

        // Remove content after the exit animation.
        cleanupTimer = setTimeout(() => {
            // Only clean up if another show/hover hasn't reversed the exit.
            if (els.popup.classList.contains('is-hiding')) {
                els.popup.classList.remove('is-hiding');

                if (els.header) {
                    els.header.classList.add('d-none');
                }

                if (els.body) {
                    els.body.innerHTML = '';
                }

                els.popup.classList.remove(
                    'is-info',
                    'is-success',
                    'is-warning',
                    'is-error'
                );
            }

            cleanupTimer = null;
        }, EXIT_DURATION);
    }

    function bindHoverEvents() {
        const els = getEls();
        if (!els) return;

        els.popup.addEventListener('mouseenter', () => {
            clearHideTimer();

            // Reverse an ongoing exit animation.
            if (els.popup.classList.contains('is-hiding')) {
                clearCleanupTimer();

                els.popup.classList.remove('is-hiding');
                els.popup.classList.add('is-visible');

                els.popup.setAttribute('aria-hidden', 'false');
            }
        });

        els.popup.addEventListener('mouseleave', () => {
            if (
                els.popup.classList.contains('is-visible') &&
                els.header &&
                !els.header.classList.contains('d-none')
            ) {
                scheduleHide(HOVER_HIDE_DURATION);
            }
        });
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', bindHoverEvents, { once: true });
    } else {
        bindHoverEvents();
    }

    return { show, hide };
})();

// Short helper
window.showFooterPopup = function (opts) {
    window.FsmFooterPopup?.show(opts);
};



(function () {
    'use strict';

    // ─── State ──────────────────────────────────────────────────────────────────
    let isOnline = navigator.onLine;
    let isIssue = false;
    let isDegraded = false;
    let announcementCount = 0;
    let countdown = 30;
    let countdownTimer = null;
    let isReloading = false;
    let currentDotColor = 'var(--bs-secondary)';
    let currentHealthStatus = 'Healthy';
    let offlineAttempts = parseInt(sessionStorage.getItem('fsm_offline_attempts') ?? '0');

    const returnUrl = encodeURIComponent(window.location.pathname + window.location.search);

    // ─── Elements ───────────────────────────────────────────────────────────────
    const panelOffline = document.getElementById('fsm-fp-offline');
    const panelIssue = document.getElementById('fsm-fp-issue');
    const panelAnnouncement = document.getElementById('fsm-fp-announcement');
    const panelOnline = document.getElementById('fsm-fp-online');
    const fsmFooterOfflinePageNote = document.getElementById('fsm-footer-offline-page-note');

    const countdownEl = document.getElementById('fsm-footer-countdown');
    const reloadBtns = document.querySelectorAll('[data-fsm-footer-reload]');
    const announceLbl = document.getElementById('fsm-footer-announce-label');
    const issueMsg = document.getElementById('fsm-issue-message');

    const dot = document.getElementById('health-dot');
    const label = document.getElementById('health-label');

    const supportBtn = document.getElementById('fsm-footer-support');
    const feedbackBtn = document.getElementById('fsm-footer-feedback');
    const bugBtn = document.getElementById('fsm-footer-bug');
    const aiBtn = document.getElementById('fsm-footer-ai');
    const notesBtn = document.querySelectorAll('[data-fsm-notes-trigger]');

    let backdrop = document.getElementById('fsm-backdrop');
    if (!backdrop) {
        backdrop = document.createElement('div');
        backdrop.id = 'fsm-footer-backdrop';
        backdrop.className = 'fsm-backdrop';
        document.body.appendChild(backdrop);
    }

    // ─── Helpers ────────────────────────────────────────────────────────────────
    function formatCount(n) {
        if (n >= 1_000_000) return (n / 1_000_000).toFixed(1).replace(/\.0$/, '') + 'M';
        if (n >= 1_000) return (n / 1_000).toFixed(1).replace(/\.0$/, '') + 'K';
        return n.toString();
    }

    function announcementLabel(n) {
        return n === 1
            ? 'You have a new announcement'
            : `You have ${formatCount(n)} new announcements`;
    }

    // ─── Priority resolver ───────────────────────────────────────────────────────
    // offline > issue/degraded > announcement > online
    function resolveStatus() {
        if (!isOnline) return 'offline';
        if (isIssue || isDegraded) return 'issue';
        if (announcementCount > 0) return 'announcement';
        return 'online';
    }

    // ─── Issue messages ──────────────────────────────────────────────────────────
    const issueMessages = {
        Unhealthy: 'System outage detected.',
        Degraded: 'Service degradation detected.',
        Unreachable: 'System unreachable.'
    };

    // ─── Render ──────────────────────────────────────────────────────────────────
    const panels = {
        offline: panelOffline,
        issue: panelIssue,
        announcement: panelAnnouncement,
        online: panelOnline
    };

    function render() {
        const status = resolveStatus();
        const isRed = status === 'offline' || (status === 'issue' && !isDegraded);
        const isAmber = status === 'issue' && isDegraded;

        Object.entries(panels).forEach(([key, el]) => {
            if (el) el.style.display = key === status ? '' : 'none';
        });

        if (countdownEl) countdownEl.textContent = countdown;
        if (announceLbl) announceLbl.textContent = announcementLabel(announcementCount);

        if (issueMsg) {
            issueMsg.textContent = issueMessages[currentHealthStatus] ?? 'Something went wrong.';
            issueMsg.style.color = isAmber ? 'var(--bs-warning)' : 'var(--bs-danger)';
        }

        const overrideColor = isRed ? 'var(--bs-danger)' : isAmber ? 'var(--bs-warning)' : null;
        if (dot) dot.style.background = overrideColor ?? currentDotColor;
        if (label) label.style.color = overrideColor ?? '';

        reloadBtns.forEach(btn => {
            btn.disabled = isReloading;
            btn.classList.toggle('spinning', isReloading);
        });
    }

    // ─── Health polling ───────────────────────────────────────────────────────────
    const dotColors = {
        Healthy: 'var(--bs-success)',
        Degraded: 'var(--bs-warning)',
        Unhealthy: 'var(--bs-danger)'
    };

    const labelColors = {
        Healthy: 'var(--bs-success)',
        Degraded: 'var(--bs-warning)',
        Unhealthy: 'var(--bs-danger)'
    };

    const displayLabel = {
        Healthy: 'Operational',
        Degraded: 'Degraded',
        Unhealthy: 'Outage'
    };

    async function pingHealth() {
        try {
            const res = await fetch('/health/status');
            const data = await res.json();

            currentDotColor = dotColors[data.status] ?? 'var(--bs-secondary)';
            currentHealthStatus = data.status;

            if (label) {
                label.textContent = displayLabel[data.status] ?? data.status;
                label.style.color = labelColors[data.status] ?? '';
            }

            const hadIssue = isIssue;
            const hadDegraded = isDegraded;

            isIssue = data.status === 'Unhealthy';
            isDegraded = data.status === 'Degraded';

            if ((isIssue || isDegraded) !== (hadIssue || hadDegraded)) render();
            else {
                const status = resolveStatus();
                if (dot && status !== 'offline' && status !== 'issue')
                    dot.style.background = currentDotColor;
            }

        } catch {
            currentDotColor = 'var(--bs-danger)';
            currentHealthStatus = 'Unreachable';
            isDegraded = false;

            if (label) {
                label.textContent = 'Unreachable';
                label.style.color = 'var(--bs-danger)';
            }

            if (!isIssue) { isIssue = true; render(); }
        }
    }

    // ─── Announcement polling ─────────────────────────────────────────────────────
    async function pingAnnouncements() {
        try {
            const res = await fetch('/Announcements/GetUnseenCount');
            const data = await res.json();

            const prev = announcementCount;
            announcementCount = data.count ?? 0;

            if (announcementCount !== prev) render();
        } catch {
            // silent
        }
    }

    // pingHealth();
    pingAnnouncements();
    setInterval(pingHealth, 1 * 60 * 1000);
    setInterval(pingAnnouncements, 2 * 60 * 1000);

    // ─── Online / offline ────────────────────────────────────────────────────────
    const isOfflinePage = window.location.pathname.toLowerCase().includes('/home/offline');

    function handleOfflineCountdownEnd() {
        offlineAttempts++;
        sessionStorage.setItem('fsm_offline_attempts', offlineAttempts);

        if (offlineAttempts === 1) {
            isReloading = true;
            render();
            window.location.reload();
        } else {
            window.location.href = `/Home/Offline?ReturnUrl=${returnUrl}`;
        }
    }

    // Only start countdown if not already on the offline page
    if (!isOnline && !isOfflinePage) {
        startCountdown();
    }


    window.addEventListener('online', () => {
        isOnline = true;
        offlineAttempts = 0;
        countdown = 30;

        if (isOfflinePage) {
            const params = new URLSearchParams(window.location.search);
            const returnUrl = params.get('ReturnUrl');
            sessionStorage.removeItem('fsm_offline_attempts');
            fsmFooterOfflinePageNote.innerText = "Connection lost. Connect to use FSM.";

            if (returnUrl && returnUrl.startsWith('/')) {
                window.location.replace(returnUrl);
            } else {
                window.location.replace('/');
            }
        }
    });

    window.addEventListener('offline', () => {
        isOnline = false;
        countdown = 30;
        if (!isOfflinePage) startCountdown();
        render();
    });

    function startCountdown() {
        stopCountdown();
        countdownTimer = setInterval(() => {
            if (countdown > 0) {
                countdown--;
                if (countdownEl) countdownEl.textContent = countdown;
            } else {
                stopCountdown();
                handleOfflineCountdownEnd();
            }
        }, 1000);
    }

    document.addEventListener('fsm:announcements:loaded', () => {
        pingAnnouncements();
    });

    function stopCountdown() {
        if (countdownTimer) { clearInterval(countdownTimer); countdownTimer = null; }
    }

    // ─── Offline countdown end handler ───────────────────────────────────────────
    function handleOfflineCountdownEnd() {
        window.location.href = `/Home/Offline?ReturnUrl=${returnUrl}`;
    }

    if (!isOnline) {
        offlineAttempts = 0;
        startCountdown();
    }

    // ─── Reload ──────────────────────────────────────────────────────────────────
    reloadBtns.forEach(btn => {
        btn.addEventListener('click', () => {
            isReloading = true;
            render();
            window.location.reload();
        });
    });

    function closeAnnouncements() {
        announceSheet?.classList.remove('show');
        backdrop?.classList.remove('show');
        document.body.style.overflow = '';
    }

    backdrop?.addEventListener('click', () => {
        closeAnnouncements();
        document.querySelectorAll('.fsm-offcanvas.show, .fsm-mobile-offcanvas.show')
            .forEach(s => s.classList.remove('show'));
        document.body.style.overflow = '';
    });

    // ─── Dialog triggers ─────────────────────────────────────────────────────────
    function fireDialog(name) {
        document.dispatchEvent(new CustomEvent('fsm:dialog:open', { detail: { name } }));
    }

    supportBtn?.addEventListener('click', () => fireDialog('support'));
    feedbackBtn?.addEventListener('click', () => fireDialog('feedback'));
    bugBtn?.addEventListener('click', () => fireDialog('bug'));
    aiBtn?.addEventListener('click', () => fireDialog('ai'));
    notesBtn.forEach(btn => btn.addEventListener('click', () => fireDialog('notes')));

    // ─── Initial render ───────────────────────────────────────────────────────────
    render();

}());


const announcementsDialog = document.getElementById('fsm-announcements-dialog');
const announcementsTriggers = document.querySelectorAll('[data-fsm-announcements-trigger]');
const announcementsClose = document.querySelectorAll('[data-fsm-announcements-close]');

announcementsTriggers.forEach(t => {
    t.addEventListener('click', (e) => {
        e.stopPropagation();
        announcementsDialog?.classList.add('show');
    });
});

announcementsClose.forEach(btn => {
    btn.addEventListener('click', () => announcementsDialog?.classList.remove('show'));
});

// ─── Keyboard: close sheets on Escape ────────────────────────────────────────
document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape') {
        closeAllSheets();
        announcementsDialog?.classList.remove('show');
        dropdowns.forEach(dd => dd.querySelector('.fsm-dropdown-menu')?.classList.remove('show'));
    }
});