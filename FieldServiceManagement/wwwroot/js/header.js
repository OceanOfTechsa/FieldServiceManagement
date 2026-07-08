'use strict';
(function() {

// ─── Constants ───────────────────────────────────────────────────────────────
const SCROLL_THRESHOLD = 80;
const SLIDE_BACK_DELAY = 220;

const VISIBLE_COUNTS = [
    {breakpoint: 1536, count: 9 },
    {breakpoint: 1280, count: 5 },
    {breakpoint: 1024, count: 5 },
];

// ─── Elements ─────────────────────────────────────────────────────────────────
const header        = document.getElementById('fsm-header');
const navItems      = Array.from(document.querySelectorAll('.fsm-nav-item'));
const moreWrapper   = document.getElementById('fsm-more-wrapper');
const moreMenu      = document.getElementById('fsm-more-menu');
const backdrop      = document.getElementById('fsm-backdrop');

// Offcanvas: profile
const profileSheet  = document.getElementById('fsm-profile-sheet');
const profileTriggers = document.querySelectorAll('[data-fsm-profile-trigger]');
const profileClose  = document.querySelectorAll('[data-fsm-profile-close]');

// Offcanvas: notifications
const notifSheet    = document.getElementById('fsm-notif-sheet');
const notifTriggers = document.querySelectorAll('[data-fsm-notif-trigger]');
const notifClose = document.querySelectorAll('[data-fsm-notif-close]');

// Offcanvas: New User
const newUserSheet = document.getElementById('fsm-new-user-sheet');
const newUserTriggers = document.querySelectorAll('[data-fsm-new-user-trigger]');
const newUserClose = document.querySelectorAll('[data-fsm-new-user-close]');


// Offcanvas: mobile menu
const mobileSheet   = document.getElementById('fsm-mobile-sheet');
const mobileTrigger = document.getElementById('fsm-mobile-trigger');
const mobileClose   = document.querySelectorAll('[data-fsm-mobile-close]');

// Sign-out dialog
const signoutDialog  = document.getElementById('fsm-signout-dialog');
const signoutTriggers = document.querySelectorAll('[data-fsm-signout-trigger]');
const signoutCancel  = document.querySelectorAll('[data-fsm-signout-cancel]');
const signoutConfirm = document.getElementById('fsm-signout-confirm');

// ─── Scroll behaviour ─────────────────────────────────────────────────────────
let ticking          = false;
let effectTriggered  = false;
let animationTimeout = null;

function handleScroll() {
    if (ticking || !header) return;
    window.requestAnimationFrame(() => {
        const y = window.scrollY;
        if (y < SCROLL_THRESHOLD) {
            effectTriggered = false;
            header.classList.remove('is-hidden', 'is-scrolled');
            if (animationTimeout) {clearTimeout(animationTimeout); animationTimeout = null; }
        }
        else {
            header.classList.add('is-scrolled');
            if (!effectTriggered) {
                effectTriggered = true;
                header.classList.add('is-hidden');
                animationTimeout = setTimeout(() => {
                    header.classList.remove('is-hidden');
                    animationTimeout = null;
                }, SLIDE_BACK_DELAY);
            }
        }
        ticking = false;
    });
    ticking = true;
}

window.addEventListener('scroll', handleScroll, {passive: true });

// ─── Responsive visible count ─────────────────────────────────────────────────
function getVisibleCount(width) {
    for (const {breakpoint, count} of VISIBLE_COUNTS) {
        if (width >= breakpoint) return count;
    }
    return 9;
}

function applyVisibleCount() {
    if (!navItems.length) return;
    const count = getVisibleCount(window.innerWidth);

    // Items beyond the more menu that were previously hidden in the main menu
    const moreMenuItems = document.querySelectorAll('.fsm-more-item');

    navItems.forEach((item, i) => {
        item.style.display = i < count ? '' : 'none';
    });

    moreMenuItems.forEach((item, i) => {
        // These are the overflow items — show only those past count
        const globalIndex = parseInt(item.dataset.index, 10);
        item.style.display = globalIndex >= count ? '' : 'none';
    });

    // Hide/show the "More" button itself
    if (moreWrapper) {
        const hasHidden = navItems.some((item, i) => i >= count);
        moreWrapper.style.display = hasHidden ? '' : 'none';
    }
}

window.addEventListener('resize', applyVisibleCount);
applyVisibleCount();
// ─── Desktop dropdowns ──────────────────────────────────────────
const dropdowns = document.querySelectorAll('.fsm-dropdown');
let activeDropdown = null;

function closeAllDropdowns() {
    dropdowns.forEach(dd => {
        const menu = dd.querySelector('.fsm-dropdown-menu');
        const tog = dd.querySelector('.nav-dropdown-toggle, .nav-more-btn');
        menu?.classList.remove('show');
        tog?.setAttribute('aria-expanded', 'false');
    });
    activeDropdown = null;
}

dropdowns.forEach(dd => {
    const toggle = dd.querySelector('.nav-dropdown-toggle, .nav-more-btn');
    const menu   = dd.querySelector('.fsm-dropdown-menu');
    if (!toggle || !menu) return;

    let hoverTimer = null;

    // Hover open (desktop only)
    dd.addEventListener('mouseenter', () => {
        clearTimeout(hoverTimer);
        if (activeDropdown && activeDropdown !== dd) closeAllDropdowns();
        menu.classList.add('show');
        toggle.setAttribute('aria-expanded', 'true');
        activeDropdown = dd;
    });

    dd.addEventListener('mouseleave', () => {
        hoverTimer = setTimeout(() => {
            menu.classList.remove('show');
            toggle.setAttribute('aria-expanded', 'false');
            if (activeDropdown === dd) activeDropdown = null;
        }, 120);
    });

    // Click toggle (fallback for keyboard/touch)
    toggle.addEventListener('click', (e) => {
        e.stopPropagation();
        const isOpen = menu.classList.contains('show');
        closeAllDropdowns();
        if (!isOpen) {
            menu.classList.add('show');
            toggle.setAttribute('aria-expanded', 'true');
            activeDropdown = dd;
        }
    });
});

// Close on outside click — stopPropagation on toggle prevents this firing immediately
document.addEventListener('click', closeAllDropdowns);

// ─── Offcanvas helpers ────────────────────────────────────────────────────────
function openSheet(sheet) {
    if (!sheet) return;
        sheet.classList.add('show');
        backdrop?.classList.add('show');
        document.body.style.overflow = 'hidden';
    }

    function closeSheet(sheet) {
        if (!sheet) return;
        sheet.classList.remove('show');
        // Only remove backdrop if no other sheet is open
        const anyOpen = [profileSheet, notifSheet, mobileSheet].some(s => s && s.classList.contains('show'));
        if (!anyOpen) {
            backdrop?.classList.remove('show');
            document.body.style.overflow = '';
        }
    }

    function closeAllSheets() {
        [profileSheet, notifSheet, mobileSheet].forEach(s => s?.classList.remove('show'));
        backdrop?.classList.remove('show');
        document.body.style.overflow = '';
    }

    backdrop?.addEventListener('click', closeAllSheets);

    // Profile sheet
    profileTriggers.forEach(t => t.addEventListener('click', () => openSheet(profileSheet)));
    profileClose.forEach(t => t.addEventListener('click', () => closeSheet(profileSheet)));

    // Notification sheet
    notifTriggers.forEach(t => t.addEventListener('click', () => openSheet(notifSheet)));
    notifClose.forEach(t => t.addEventListener('click', () => closeSheet(notifSheet)));

    //New User Sheet
    newUserTriggers.forEach(t => t.addEventListener('click', () => openSheet(newUserSheet)));
    newUserClose.forEach(t => t.addEventListener('click', () => closeSheet(newUserSheet)));

    // Mobile sheet
    mobileTrigger?.addEventListener('click', () => openSheet(mobileSheet));
    mobileClose.forEach(t => t.addEventListener('click', () => closeSheet(mobileSheet)));

    // ─── Mobile sub-menus ─────────────────────────────────────────────────────────
    document.querySelectorAll('.mobile-dropdown-toggle').forEach(btn => {
        const sub = btn.nextElementSibling;
        if (!sub) return;
        btn.addEventListener('click', () => {
            const open = sub.classList.contains('show');
            // close all others
            document.querySelectorAll('.mobile-sub-menu').forEach(m => m.classList.remove('show'));
            if (!open) sub.classList.add('show');
        });
    });

    // ─── Sign-out dialog ──────────────────────────────────────────────────────────
    signoutTriggers.forEach(t => {
        t.addEventListener('click', (e) => {
            e.stopPropagation();
            signoutDialog?.classList.add('show');
        });
    });

    signoutCancel.forEach(btn => {
        btn.addEventListener('click', () => signoutDialog?.classList.remove('show'));
    });

    signoutDialog?.addEventListener('click', (e) => {
        // if (e.target === signoutDialog) signoutDialog.classList.remove('show');
    });

    if (signoutConfirm) {
        signoutConfirm.addEventListener('click', () => {
            signoutConfirm.disabled = true;
            signoutConfirm.textContent = 'Signing out…';
            fetch('/Identity/Account/Logout', {
                method: 'POST',
                headers: {
                    'RequestVerificationToken': document.querySelector('[name=__RequestVerificationToken]')?.value ?? ''
                }
            })
            .finally(() => {
                window.location.replace('/Identity/Account/Login');
            });
        });
    }

    // ─── Keyboard: close sheets on Escape ────────────────────────────────────────
    document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape') {
        closeAllSheets();
        signoutDialog?.classList.remove('show');
            dropdowns.forEach(dd => dd.querySelector('.fsm-dropdown-menu')?.classList.remove('show'));
        }
    });
})();


(function () {
    const searchInput = document.getElementById('actionsSearchInput');
    if (!searchInput) return;

    const menu = searchInput.closest('.actions-dropdown');
    const items = menu.querySelectorAll('.searchable-item');
    const groupLabels = menu.querySelectorAll('.actions-group-label');
    const noResultsMsg = menu.querySelector('.actions-no-results');

    searchInput.addEventListener('input', function () {
        const term = this.value.trim().toLowerCase();
        let anyVisible = false;

        items.forEach(item => {
            const text = item.textContent.trim().toLowerCase();
            const match = text.includes(term);
            item.classList.toggle('d-none', !match);
            if (match) anyVisible = true;
        });

        groupLabels.forEach(label => {
            let sibling = label.nextElementSibling;
            let hasVisibleItem = false;
            while (sibling && !sibling.classList.contains('actions-group-label')) {
                if (sibling.classList.contains('searchable-item') && !sibling.classList.contains('d-none')) {
                    hasVisibleItem = true;
                    break;
                }
                sibling = sibling.nextElementSibling;
            }
            label.classList.toggle('d-none', !hasVisibleItem);
        });

        noResultsMsg.classList.toggle('d-none', anyVisible);
    });

    const dropdownToggle = document.getElementById('actionsDropdown');
    if (dropdownToggle) {
        dropdownToggle.addEventListener('hidden.bs.dropdown', function () {
            searchInput.value = '';
            searchInput.dispatchEvent(new Event('input'));
        });
    }
})();


(function () {
    const dropdownToggle = document.getElementById('notifDropdown');
    const menu = dropdownToggle?.nextElementSibling;
    if (!dropdownToggle || !menu) return;

    const listContainer = menu.querySelector('.notification-list');
    const noResultsMsg = menu.querySelector('.notif-no-results');
    const markAllReadBtn = menu.querySelector('.notif-mark-read');
    const notifDot = dropdownToggle.querySelector('.notification-dot');

    const searchInput = menu.querySelector('#notifSearchInput');
    const readStatusSelect = menu.querySelector('#filterReadStatus');
    const severitySelect = menu.querySelector('#filterSeverity');
    const typeChecks = menu.querySelectorAll('.notif-filter-checks input[type="checkbox"]');
    const dateFromInput = menu.querySelector('#filterDateFrom');
    const dateToInput = menu.querySelector('#filterDateTo');
    const filterApplyBtn = menu.querySelector('#filterApplyBtn');
    const filterClearBtn = menu.querySelector('#filterClearBtn');
    const filterToggle = menu.querySelector('#notifFilterToggle');
    const filterPanel = menu.querySelector('#notifFilterPanel');
    const filterBadge = menu.querySelector('.notif-filter-badge');

    let hasLoadedOnce = false;
    let currentNotifications = [];

    const severityIcons = {
        info: '<circle cx="12" cy="12" r="10" /><line x1="12" y1="16" x2="12" y2="12" /><line x1="12" y1="8" x2="12.01" y2="8" />',
        success: '<path d="M3 9l9-7 9 7v11a2 2 0 01-2 2H5a2 2 0 01-2-2z" /><polyline points="9 22 9 12 15 12 15 22" />',
        warning: '<path d="M12 9v4M12 17h.01M10.29 3.86 1.82 18a2 2 0 001.71 3h16.94a2 2 0 001.71-3L13.71 3.86a2 2 0 00-3.42 0z" />',
        danger: '<line x1="12" y1="1" x2="12" y2="23" /><path d="M17 5H9.5a3.5 3.5 0 000 7h5a3.5 3.5 0 010 7H6" />'
    };

    function timeAgo(dateString) {
        const date = new Date(dateString);
        const seconds = Math.floor((Date.now() - date.getTime()) / 1000);
        if (seconds < 60) return 'Just now';
        const minutes = Math.floor(seconds / 60);
        if (minutes < 60) return `${minutes}m ago`;
        const hours = Math.floor(minutes / 60);
        if (hours < 24) return `${hours}h ago`;
        const days = Math.floor(hours / 24);
        return `${days}d ago`;
    }

    // Returns a "YYYY-MM-DD" key (local time) so items can be grouped by calendar day
    function dateKey(dateString) {
        const d = new Date(dateString);
        return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
    }

    // Returns a friendly group label: "Today", "Yesterday", or a formatted date
    function groupLabel(dateString) {
        const itemDate = new Date(dateString);
        const today = new Date();
        const yesterday = new Date();
        yesterday.setDate(today.getDate() - 1);

        if (dateKey(dateString) === dateKey(today)) return 'Today';
        if (dateKey(dateString) === dateKey(yesterday)) return 'Yesterday';

        return itemDate.toLocaleDateString(undefined, { month: 'short', day: 'numeric', year: 'numeric' });
    }

    function showLoader() {
        listContainer.innerHTML = `

            <div class="fsm-empty-state">
                <div class="fsm-loader-spinner" role="status"></div>
                <span class="text-muted small mt-2">Loading notifications...</span>
            </div>
        `;
        noResultsMsg?.classList.add('d-none');
    }

    function showError() {
        listContainer.innerHTML = `
            <div class="fsm-empty-state">
                <div class="fsm-empty-icon">
                    <i class="bi bi-bell"></i>
                </div>

                <p class="text-danger small mb-3">
                    Couldn't load notifications.
                </p>
                <a href="#" class="notif-retry">Try again</a>
            </div>
         `;
        listContainer.querySelector('.notif-retry')?.addEventListener('click', function (e) {
            e.preventDefault();
            e.stopPropagation();
            loadNotifications();
        });
    }

    function buildQueryParams() {
        const params = new URLSearchParams();

        const searchTerm = searchInput?.value.trim();
        if (searchTerm) params.append('searchTerm', searchTerm);

        const isRead = readStatusSelect?.value;
        if (isRead && isRead !== 'all') params.append('isRead', isRead === 'read' ? 'true' : 'false');

        const severity = severitySelect?.value;
        if (severity && severity !== 'all') params.append('severity', severity);

        const dateFrom = dateFromInput?.value;
        if (dateFrom) params.append('dateFrom', dateFrom);

        const dateTo = dateToInput?.value;
        if (dateTo) params.append('dateTo', dateTo);

        const checkedTypes = typeChecks ? Array.from(typeChecks).filter(c => c.checked) : [];
        if (typeChecks && checkedTypes.length === 1) {
            params.append('type', checkedTypes[0].value);
        }

        return params.toString();
    }

    function escapeHtml(str) {
        const div = document.createElement('div');
        div.textContent = str ?? '';
        return div.innerHTML;
    }

    function escapeRegExp(str) {
        return str.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    }

    function highlightText(text, term) {
        if (!term) return escapeHtml(text);
        const escaped = escapeHtml(text);
        const regex = new RegExp('(' + escapeRegExp(escapeHtml(term)) + ')', 'gi');
        return escaped.replace(regex, '<mark class="bg-warning">$1</mark>');
    }

    function renderNotifications(notifications) {
        currentNotifications = notifications;

        if (!notifications.length) {
            listContainer.innerHTML = '';
            // noResultsMsg?.classList.remove('d-none');
            listContainer.innerHTML = `
                <div class="fsm-empty-state">
                    <div class="fsm-empty-icon">
                        <i class="bi bi-bell"></i>
                    </div>

                    <p class="text-muted small mb-3">
                        You're all caught up! No notifications yet.
                    </p>
                </div>
            `;
            markAllReadBtn.classList.add("d-none")
            return;
        }
        noResultsMsg?.classList.add('d-none');
        markAllReadBtn.classList.remove("d-none")
        const term = searchInput?.value.trim();
        let html = '';
        let lastGroupKey = null;

        notifications.forEach((n, index) => {
            const groupKey = dateKey(n.createdAt);

            if (groupKey !== lastGroupKey) {
                html += `<div class="notif-section-label px-2 py-1">${escapeHtml(groupLabel(n.createdAt))}</div>`;
                lastGroupKey = groupKey;
            }

            // const unreadClass = n.isRead ? '' : ' notification-item-unread';
            const iconPath = severityIcons[n.severityCode] || severityIcons.info;
            const hasLink = !!n.actionUrl;

            // Link present -> clicking it marks as read automatically, no separate button needed
            const actionLink = hasLink
                ? `<a href="${escapeHtml(n.actionUrl)}" class="notif-action-link small" data-id="${escapeHtml(n.id)}" data-index="${index}">${escapeHtml(n.actionText || 'View')}</a>`
                : '';

            // No link -> show an explicit "mark as read" checkmark instead
            const markReadBtn = (!hasLink && !n.isRead)
                ? `<button type="button" class="fsm-mark-notification-seen-btn" data-id="${escapeHtml(n.id)}" data-index="${index}" aria-label="Mark as read">
                    <span class="fsm-btn-content">
                        Mark as read <i class="bi bi-hand-thumbs-up"></i>
                    </span>
               </button>`
                : '';

            const borderClass = index === notifications.length - 1 ? '' : ' border-bottom';

            html += `
            <div class="notification-item${borderClass} searchable-notif d-flex px-2 py-2"
                 data-id="${escapeHtml(n.id)}"
                 data-index="${index}">
                <span class="notif-icon me-2">
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">${iconPath}</svg>
                </span>
                <div class="flex-grow-1">
                    <div class="d-flex align-items-center">
                        <span class="fw-semibold small notif-title">${highlightText(n.title, term)}</span>
                        <span class="text-muted small ms-auto">${timeAgo(n.createdAt)}</span>
                    </div>
                    <div class="small text-muted notif-message">${highlightText(n.message, term)}</div>
                    <div class="d-flex align-items-center justify-content-between mt-1">
                        ${actionLink}
                        ${markReadBtn}
                    </div>
                    <div id="notification-error-${escapeHtml(n.id)}"class="bd-callout bd-callout-danger mt-1 p-1 mb-0 bg-opacity-10 smaller d-none" role="alert">
                        Failed to mark notification as read, Kindly try again.
                    </div>
                </div>
            </div>`;
        });

        listContainer.innerHTML = html;

        // Link click -> mark as read (fire-and-forget), then let the browser navigate normally
        listContainer.querySelectorAll('.notif-action-link').forEach(link => {
            link.addEventListener('click', function (e) {
                e.stopPropagation();
                const index = parseInt(this.dataset.index, 10);
                const notification = currentNotifications[index];
                const itemEl = this.closest('.notification-item');
                if (notification && !notification.isRead) {
                    markAsRead(notification.id, itemEl); // don't block navigation on this
                }
            });
        });

        // No-link items -> explicit checkmark button
        listContainer.querySelectorAll('.fsm-mark-notification-seen-btn').forEach(btn => {
            btn.addEventListener('click', function (e) {
                e.stopPropagation();

                const originalContent = btn.innerHTML;

                btn.disabled = true;
                btn.innerHTML = '<span class="fsm-btn-spinner"></span> Marking as read...';

                const index = parseInt(this.dataset.index, 10);
                const notification = currentNotifications[index];
                const itemEl = this.closest('.notification-item');
                if (notification) {
                    markAsRead(notification.id, itemEl, originalContent, btn);
                }
            });
        });
    }

    async function loadNotifications() {
        showLoader();

        try {
            const query = buildQueryParams();
            const response = await fetch(`/Notifications/GetUserNotifications?${query}`, {
                method: 'GET',
                headers: { 'Accept': 'application/json' }
            });

            if (!response.ok) throw new Error('Request failed');

            const data = await response.json();
            renderNotifications(data.notifications || []);
            updateBadge(data.unreadCount || 0);
        } catch (err) {
            console.error('Failed to load notifications:', err);
            showError();
        }
    }

    async function fetchUnreadCount() {
        try {
            const response = await fetch('/Notifications/GetUserNotifications?isRead=false', {
                method: 'GET',
                headers: { 'Accept': 'application/json' }
            });

            if (!response.ok) throw new Error('Request failed');

            const data = await response.json();
            updateBadge(data.unreadCount || 0);
        } catch (err) {
            console.error('Failed to fetch unread notification count:', err);
        }
    }

    fetchUnreadCount();

    function updateBadge(unreadCount) {
        if (notifDot) {
            notifDot.classList.toggle('d-none', unreadCount <= 0);
        }
    }

    async function markAsRead(id, itemEl, originalContent, btn) {
        try {
            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
            const response = await fetch(`/Notifications/MarkNotificationAsRead?id=${encodeURIComponent(id)}`, {
                method: 'POST',
                headers: {
                    'RequestVerificationToken': token || ''
                }
            });

            if (!response.ok) {
                throw new Error(`Request failed with status ${response.status}`);
            }

            const result = await response.json();

            if (result.success) {
                await loadNotifications(); // re-fetch so the marked item drops out of the list
            } else {
                document
                    .getElementById(`notification-error-${id}`)
                    ?.classList.remove('d-none');

                if (btn) {
                    btn.disabled = false;
                    btn.innerHTML = originalContent;
                }
            }

        } catch (err) {
            document
                .getElementById(`notification-error-${id}`)
                ?.classList.remove('d-none');

            if (btn) {
                btn.disabled = false;
                btn.innerHTML = originalContent;
            }

            console.error('Failed to mark notification as read:', err);
        }
    }

    async function markAllAsRead(btn) {
        const originalContent = btn?.innerHTML;

        if (btn) {
            btn.disabled = true;
            btn.innerHTML = '<span class="fsm-btn-spinner"></span> Marking as seen...';
        }

        document.getElementById('mark-all-notifications-error')?.classList.add('d-none');
        try {
            const response = await fetch('/Notifications/MarkAllNotificationsAsRead', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' }
            });

            if (!response.ok) {
                throw new Error(`Request failed with status ${response.status}`);
            }
            const result = await response.json();
            if (result.success) {
                await loadNotifications(); // refetch so read items reflect the server state
            } else {
                document
                    .getElementById('mark-all-notifications-error')
                    ?.classList.remove('d-none');

                if (btn) {
                    btn.disabled = false;
                    btn.innerHTML = originalContent;
                }
            }
        } catch (err) {
            document
                .getElementById('mark-all-notifications-error')
                ?.classList.remove('d-none');

            if (btn) {
                btn.disabled = false;
                btn.innerHTML = originalContent;
            }

            console.error('Failed to mark all notifications as read:', err);
        }
    }

    dropdownToggle.addEventListener('show.bs.dropdown', function () {
        if (!hasLoadedOnce) {
            hasLoadedOnce = true;
            loadNotifications();
        }
    });

    let debounceTimer;
    searchInput?.addEventListener('input', function () {
        clearTimeout(debounceTimer);
        debounceTimer = setTimeout(loadNotifications, 350);
    });
    searchInput?.addEventListener('click', e => e.stopPropagation());

    filterApplyBtn?.addEventListener('click', function (e) {
        e.stopPropagation();
        loadNotifications();
        filterPanel?.classList.remove('show');
        filterToggle?.setAttribute('aria-expanded', 'false');

        const isRead = readStatusSelect?.value;
        const severity = severitySelect?.value;
        const checkedCount = typeChecks ? Array.from(typeChecks).filter(c => c.checked).length : 0;
        const filtersActive = (isRead && isRead !== 'all') || (severity && severity !== 'all') ||
            (typeChecks && checkedCount !== typeChecks.length) || dateFromInput?.value || dateToInput?.value;

        filterBadge?.classList.toggle('d-none', !filtersActive);
        filterToggle?.classList.toggle('active', !!filtersActive);
    });

    filterClearBtn?.addEventListener('click', function (e) {
        e.stopPropagation();
        if (readStatusSelect) readStatusSelect.value = 'all';
        if (severitySelect) severitySelect.value = 'all';
        typeChecks?.forEach(c => c.checked = true);
        if (dateFromInput) dateFromInput.value = '';
        if (dateToInput) dateToInput.value = '';
        loadNotifications();
        filterBadge?.classList.add('d-none');
        filterToggle?.classList.remove('active');
    });

    filterToggle?.addEventListener('click', function (e) {
        e.stopPropagation();
        const isOpen = filterPanel.classList.toggle('show');
        filterToggle.setAttribute('aria-expanded', isOpen);
    });

    filterPanel?.addEventListener('click', e => e.stopPropagation());

    markAllReadBtn?.addEventListener('click', function (e) {
        e.stopPropagation();
        markAllAsRead(this);
    });

    document.addEventListener('click', function (e) {
        if (filterPanel && !filterPanel.contains(e.target) && e.target !== filterToggle) {
            filterPanel.classList.remove('show');
            filterToggle?.setAttribute('aria-expanded', 'false');
        }
    });

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape' && filterPanel?.classList.contains('show')) {
            filterPanel.classList.remove('show');
            filterToggle?.setAttribute('aria-expanded', 'false');
        }
    });
})();