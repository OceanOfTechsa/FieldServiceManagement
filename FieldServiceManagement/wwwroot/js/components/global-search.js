
/**
 * FieldServiceManagement - Global Search Manager
 *
 * Handles:
 * - Opening and closing the global search modal
 * - Ctrl + K / Cmd + K keyboard shortcuts
 * - Backdrop click
 * - Escape key
 * - Focus management
 * - Result rendering
 */

(() => {
    'use strict';

    // ---------------------------------------------------------
    // MODULE CONFIGURATION
    // ---------------------------------------------------------

    const FSM_MODULE_MAP = Object.freeze({
        contacts: {
            icon: 'user',
            colorClass: 'bg-blue-500/10 text-blue-500 dark:bg-blue-500/20'
        },
        companies: {
            icon: 'building-2',
            colorClass: 'bg-indigo-500/10 text-indigo-500 dark:bg-indigo-500/20'
        },
        requests: {
            icon: 'circle-help',
            colorClass: 'bg-amber-500/10 text-amber-500 dark:bg-amber-500/20'
        },
        estimates: {
            icon: 'file-text',
            colorClass: 'bg-purple-500/10 text-purple-500 dark:bg-purple-500/20'
        },
        workOrders: {
            icon: 'wrench',
            colorClass: 'bg-emerald-500/10 text-emerald-500 dark:bg-emerald-500/20'
        },
        servicesAppointments: {
            icon: 'calendar',
            colorClass: 'bg-teal-500/10 text-teal-500 dark:bg-teal-500/20'
        },
        serviceReports: {
            icon: 'clipboard-list',
            colorClass: 'bg-cyan-500/10 text-cyan-500 dark:bg-cyan-500/20'
        },
        scheduledAppointments: {
            icon: 'clock',
            colorClass: 'bg-sky-500/10 text-sky-500 dark:bg-sky-500/20'
        },
        invoices: {
            icon: 'receipt',
            colorClass: 'bg-rose-500/10 text-rose-500 dark:bg-rose-500/20'
        },
        payments: {
            icon: 'credit-card',
            colorClass: 'bg-green-500/10 text-green-500 dark:bg-green-500/20'
        },
        users: {
            icon: 'users',
            colorClass: 'bg-violet-500/10 text-violet-500 dark:bg-violet-500/20'
        },
        crew: {
            icon: 'hard-hat',
            colorClass: 'bg-orange-500/10 text-orange-500 dark:bg-orange-500/20'
        },
        equipment: {
            icon: 'cpu',
            colorClass: 'bg-zinc-500/10 text-zinc-500 dark:bg-zinc-400'
        },
        attendance: {
            icon: 'user-check',
            colorClass: 'bg-emerald-600/10 text-emerald-600 dark:bg-emerald-600/20'
        },
        timeOff: {
            icon: 'calendar-off',
            colorClass: 'bg-rose-600/10 text-rose-600 dark:bg-rose-600/20'
        },
        trips: {
            icon: 'navigation',
            colorClass: 'bg-blue-600/10 text-blue-600 dark:bg-blue-600/20'
        },
        reports: {
            icon: 'chart-no-axes-combined',
            colorClass: 'bg-fuchsia-500/10 text-fuchsia-500 dark:bg-fuchsia-500/20'
        },
        addresses: {
            icon: 'map-pin',
            colorClass: 'bg-red-500/10 text-red-500 dark:bg-red-500/20'
        }
    });

    // ---------------------------------------------------------
    // GLOBAL SEARCH MANAGER
    // ---------------------------------------------------------

    class GlobalSearchManager {
        constructor() {
            this.modal = document.getElementById('globalSearchModal');
            this.container = document.getElementById('globalSearchContainer');
            this.input = document.getElementById('globalSearchInput');
            this.resultsContainer = document.getElementById('globalSearchResults');

            this.isOpen = false;
            this.previousFocus = null;
            this.closeTimer = null;

            if (
                !this.modal ||
                !this.container ||
                !this.input ||
                !this.resultsContainer
            ) {
                console.error(
                    '[Global Search] Required modal elements were not found.'
                );

                return;
            }

            this.init();
        }

        // -----------------------------------------------------
        // INITIALISATION
        // -----------------------------------------------------

        init() {
            this.setInitialState();

            this.bindKeyboardEvents();
            this.bindClickEvents();
            this.bindFocusEvents();

            console.info('[Global Search] Initialised.');
        }

        setInitialState() {
            this.modal.classList.remove('is-open');
            this.modal.classList.add('opacity-0');
            this.modal.setAttribute('aria-hidden', 'true');
            this.modal.setAttribute('hidden', '');

            this.isOpen = false;
        }

        open() {
    if (this.isOpen) {
        this.input.focus();
        return;
    }

    clearTimeout(this.closeTimer);

    this.isOpen = true;

    this.previousFocus =
        document.activeElement instanceof HTMLElement
            ? document.activeElement
            : null;

    // Make modal visible.
    this.modal.hidden = false;
    this.modal.setAttribute('aria-hidden', 'false');

    document.body.classList.add('overflow-hidden');

    // Trigger initial closed state.
    this.modal.classList.remove('is-open');

    void this.modal.offsetWidth;

    // Animate open.
    this.modal.classList.add('is-open');

    requestAnimationFrame(() => {
        this.input.focus();
    });
}

close() {
    if (!this.isOpen) return;

    this.isOpen = false;

    // Animate closed.
    this.modal.classList.remove('is-open');
    this.modal.setAttribute('aria-hidden', 'true');

    document.body.classList.remove('overflow-hidden');

    this.input.value = '';

    this.closeTimer = setTimeout(() => {
        if (!this.isOpen) {
            this.modal.hidden = true;

            this.previousFocus?.focus();
        }
    }, 200);
}

        // -----------------------------------------------------
        // TOGGLE
        // -----------------------------------------------------

        toggle() {
            if (this.isOpen) {
                this.close();
            } else {
                this.open();
            }
        }

        // -----------------------------------------------------
        // KEYBOARD EVENTS
        // -----------------------------------------------------

        bindKeyboardEvents() {
            document.addEventListener('keydown', event => {
                // Ctrl + K / Cmd + K
                if (
                    (event.ctrlKey || event.metaKey) &&
                    event.key.toLowerCase() === 'k'
                ) {
                    event.preventDefault();
                    this.toggle();
                    return;
                }

                if (!this.isOpen) return;

                // Escape
                if (event.key === 'Escape') {
                    event.preventDefault();
                    this.close();
                    return;
                }

                // Focus trapping
                if (event.key === 'Tab') {
                    this.trapFocus(event);
                }
            });
        }

        // -----------------------------------------------------
        // CLICK EVENTS
        // -----------------------------------------------------

        bindClickEvents() {
            // Close when clicking outside the search container.
            this.modal.addEventListener('click', event => {
                if (event.target === this.modal) {
                    this.close();
                }
            });

            // Open from any global search trigger.
            document.addEventListener('click', event => {
                const target = event.target;

                if (!(target instanceof Element)) return;

                const trigger = target.closest(
                    '[data-action="open-global-search"]'
                );

                if (!trigger) return;

                event.preventDefault();
                this.open();
            });
        }

        // -----------------------------------------------------
        // FOCUS MANAGEMENT
        // -----------------------------------------------------

        bindFocusEvents() {
            this.modal.addEventListener('transitionend', event => {
                if (
                    event.target === this.modal &&
                    event.propertyName === 'opacity' &&
                    !this.isOpen
                ) {
                    this.modal.setAttribute('hidden', '');
                }
            });
        }

        trapFocus(event) {
            const focusableElements = [
                ...this.modal.querySelectorAll(
                    'input:not([disabled]), button:not([disabled]), a[href], [tabindex]:not([tabindex="-1"])'
                )
            ].filter(element => element.offsetParent !== null);

            if (!focusableElements.length) return;

            const first = focusableElements[0];
            const last = focusableElements[focusableElements.length - 1];

            if (
                event.shiftKey &&
                document.activeElement === first
            ) {
                event.preventDefault();
                last.focus();
            } else if (
                !event.shiftKey &&
                document.activeElement === last
            ) {
                event.preventDefault();
                first.focus();
            }
        }

        // -----------------------------------------------------
        // RESULT RENDERING
        // -----------------------------------------------------

        renderResultItem(moduleKey, title, subtitle, url) {
            const config = FSM_MODULE_MAP[moduleKey] || {
                icon: 'file',
                colorClass: 'bg-muted text-muted-foreground'
            };

            const safeTitle = this.escapeHtml(title);
            const safeSubtitle = this.escapeHtml(subtitle);
            const safeUrl = this.escapeHtml(url);

            return `
                <a href="${safeUrl}"
                   class="global-search-result group flex items-center gap-3 rounded-lg px-3 py-2.5 transition-colors hover:bg-muted/60 focus-visible:bg-muted/60 focus-visible:outline-none">

                    <div class="flex size-9 shrink-0 items-center justify-center rounded-md ${config.colorClass}">
                        <i data-lucide="${config.icon}"
                           class="size-[18px]"
                           aria-hidden="true"></i>
                    </div>

                    <div class="min-w-0 flex-1">
                        <p class="truncate text-sm font-medium text-foreground group-hover:text-primary">
                            ${safeTitle}
                        </p>

                        <p class="truncate text-xs text-muted-foreground">
                            ${safeSubtitle}
                        </p>
                    </div>

                    <i data-lucide="corner-down-left"
                       class="size-3.5 shrink-0 text-muted-foreground opacity-0 transition-opacity group-hover:opacity-100"
                       aria-hidden="true"></i>
                </a>
            `;
        }

        // -----------------------------------------------------
        // HTML ESCAPING
        // -----------------------------------------------------

        escapeHtml(value) {
            return String(value ?? '').replace(/[&<>"']/g, character => ({
                '&': '&amp;',
                '<': '&lt;',
                '>': '&gt;',
                '"': '&quot;',
                "'": '&#039;'
            })[character]);
        }
    }

    // ---------------------------------------------------------
    // INITIALISE ONCE THE DOM IS READY
    // ---------------------------------------------------------

    function initialiseGlobalSearch() {
        if (window.fsmGlobalSearch) return;

        window.fsmGlobalSearch = new GlobalSearchManager();
    }

    if (document.readyState === 'loading') {
        document.addEventListener(
            'DOMContentLoaded',
            initialiseGlobalSearch,
            { once: true }
        );
    } else {
        initialiseGlobalSearch();
    }

})();
