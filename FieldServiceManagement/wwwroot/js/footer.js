/**
* FSM Footer — vanilla JS
* Mirrors React FSMFooter:
*  - loading → offline → issue → announcement → online priority
*  - offline countdown + manual reload
*  - announcements offcanvas
*  - dialog triggers for Support / Bug / Feedback / Notes
*/
(function () {
    'use strict';

    // ─── State ──────────────────────────────────────────────────────────────────
    let isOnline = navigator.onLine;
    let isLoading = document.readyState !== 'complete';
    let isIssue = false;          // set to true from server via window.fsmHasIssue
    let announcementCount = 0;             // set from server via window.fsmAnnouncementCount
    let countdown = 30;
    let countdownTimer = null;
    let isReloading = false;

    // Pull server-injected values if present
    if (typeof window.fsmHasIssue !== 'undefined') isIssue = window.fsmHasIssue;
    if (typeof window.fsmAnnouncementCount !== 'undefined') announcementCount = window.fsmAnnouncementCount;

    // ─── Elements ───────────────────────────────────────────────────────────────
    const statusPanel = document.getElementById('fsm-footer-status');

    // Status sub-panels (shown/hidden via display)
    const panelLoading = document.getElementById('fsm-fp-loading');
    const panelOffline = document.getElementById('fsm-fp-offline');
    const panelIssue = document.getElementById('fsm-fp-issue');
    const panelAnnouncement = document.getElementById('fsm-fp-announcement');
    const panelOnline = document.getElementById('fsm-fp-online');

    const countdownEl = document.getElementById('fsm-footer-countdown');
    const reloadBtns = document.querySelectorAll('[data-fsm-footer-reload]');
    const announceLbl = document.getElementById('fsm-footer-announce-label');

    // Announcement offcanvas
    const announceSheet = document.getElementById('fsm-announce-sheet');
    const announceTrig = document.getElementById('fsm-footer-announce-trigger');
    const announceClose = document.querySelectorAll('[data-fsm-announce-close]');

    // Dialog triggers — wire to whatever dialog system you use
    const supportBtn = document.getElementById('fsm-footer-support');
    const feedbackBtn = document.getElementById('fsm-footer-feedback');
    const bugBtn = document.getElementById('fsm-footer-bug');
    const aiBtn = document.getElementById('fsm-footer-ai');
    const notesBtn = document.querySelectorAll('[data-fsm-notes-trigger]');

    // Shared backdrop (reuse header backdrop if present, else create)
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
        return n === 1 ? 'You have a new announcement'
            : `You have ${formatCount(n)} new announcements`;
    }

    function resolveStatus() {
        if (isLoading) return 'loading';
        if (!isOnline) return 'offline';
        if (isIssue) return 'issue';
        if (announcementCount > 0) return 'announcement';
        return 'online';
    }

    // ─── Render ──────────────────────────────────────────────────────────────────
    const panels = {
        loading: panelLoading,
        offline: panelOffline,
        issue: panelIssue,
        announcement: panelAnnouncement,
        online: panelOnline,
    };

    function render() {
        const status = resolveStatus();

        // Show only the matching panel
        Object.entries(panels).forEach(([key, el]) => {
            if (el) el.style.display = key === status ? '' : 'none';
        });

        if (statusPanel) statusPanel.dataset.status = status;

        // Update countdown text
        if (countdownEl) countdownEl.textContent = countdown;

        // Update announcement label
        if (announceLbl) announceLbl.textContent = announcementLabel(announcementCount);

        // Reload button state
        reloadBtns.forEach(btn => {
            btn.disabled = isReloading;
            btn.classList.toggle('spinning', isReloading);
        });
    }

    // ─── Page load ───────────────────────────────────────────────────────────────
    if (document.readyState === 'complete') {
        isLoading = false;
        render();
    } else {
        document.addEventListener('readystatechange', () => {
            if (document.readyState === 'complete') {
                isLoading = false;
                render();
            }
        });
    }

    // ─── Online / offline ────────────────────────────────────────────────────────
    window.addEventListener('online', () => {
        isOnline = true;
        countdown = 30;
        stopCountdown();
        render();
    });

    window.addEventListener('offline', () => {
        isOnline = false;
        countdown = 30;
        startCountdown();
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
            }
        }, 1000);
    }

    function stopCountdown() {
        if (countdownTimer) { clearInterval(countdownTimer); countdownTimer = null; }
    }

    if (!isOnline) startCountdown();

    // ─── Reload ──────────────────────────────────────────────────────────────────
    reloadBtns.forEach(btn => {
        btn.addEventListener('click', () => {
            isReloading = true;
            render();
            window.location.reload();
        });
    });

    // ─── Announcements sheet ─────────────────────────────────────────────────────
    function openAnnouncements() {
        announceSheet?.classList.add('show');
        backdrop?.classList.add('show');
        document.body.style.overflow = 'hidden';
    }

    function closeAnnouncements() {
        announceSheet?.classList.remove('show');
        backdrop?.classList.remove('show');
        document.body.style.overflow = '';
    }

    announceTrig?.addEventListener('click', openAnnouncements);
    announceClose.forEach(el => el.addEventListener('click', closeAnnouncements));
    backdrop?.addEventListener('click', () => {
        closeAnnouncements();
        // also close header sheets if they exist
        document.querySelectorAll('.fsm-offcanvas.show, .fsm-mobile-offcanvas.show')
            .forEach(s => s.classList.remove('show'));
        document.body.style.overflow = '';
    });

    // ─── Dialog triggers ─────────────────────────────────────────────────────────
    // These dispatch custom events — wire them up in your dialog partial's own JS,
    // or replace with whatever your dialog system needs (Bootstrap modal, etc.)
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

})();