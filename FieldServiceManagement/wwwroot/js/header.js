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
            } else {
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
            const anyOpen = [profileSheet, notifSheet, mobileSheet]
                        .some(s => s && s.classList.contains('show'));
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
