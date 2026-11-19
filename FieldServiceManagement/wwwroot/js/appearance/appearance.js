
/**
 * FieldServiceManagement - Appearance Manager
 *
 * Handles:
 * - Theme colour
 * - Font family
 * - Colour mode (light, dark, system)
 * - Border radius
 * - Page layout (default, floating)
 * - Content layout (full, compact)
 * - Interface scale
 * - Sidebar variant (flush, inset, floating)
 * - Sidebar mode (expanded, icon, hidden)
 *
 * Preferences are stored per user in localStorage.
 */

(() => {
    'use strict';

    const STORAGE_PREFIX = 'fsm:appearance:';
    const root = document.documentElement;

    // ---------------------------------------------------------
    // 1. USER STORAGE
    // ---------------------------------------------------------

    const userKey = root.dataset.userKey || 'anonymous';
    const storageKey = `${STORAGE_PREFIX}${encodeURIComponent(userKey)}`;

    // ---------------------------------------------------------
    // 2. DEFAULT SETTINGS
    // ---------------------------------------------------------

    const DEFAULTS = Object.freeze({
        themeColor: '#0B9944',
        font: 'Roboto',
        colorMode: 'system',
        radius: 'md',

        pageLayout: 'default',
        contentLayout: 'full',
        scale: 'md',

        sidebarVariant: 'inset',
        sidebarMode: 'expanded'
    });

    // ---------------------------------------------------------
    // 3. ALLOWED VALUES
    // ---------------------------------------------------------

    const OPTIONS = Object.freeze({
        font: Object.freeze([
            'Roboto',
            'Inter',
            'Heebo'
        ]),

        colorMode: Object.freeze([
            'light',
            'dark',
            'system'
        ]),

        radius: Object.freeze([
            'none',
            'sm',
            'md',
            'lg'
        ]),

        pageLayout: Object.freeze([
            'default',
            'floating'
        ]),

        contentLayout: Object.freeze([
            'compact',
            'full'
        ]),

        scale: Object.freeze([
            'sm',
            'md',
            'lg'
        ]),

        sidebarVariant: Object.freeze([
            'flush',
            'inset',
            'floating'
        ]),

        sidebarMode: Object.freeze([
            'expanded',
            'icon',
            'hidden'
        ])
    });

    // ---------------------------------------------------------
    // 4. SYSTEM THEME
    // ---------------------------------------------------------

    const systemTheme = window.matchMedia(
        '(prefers-color-scheme: dark)'
    );

    // ---------------------------------------------------------
    // 5. STORAGE HELPERS
    // ---------------------------------------------------------

    const storage = {
        parse(value) {
            if (!value) {
                return {};
            }

            try {
                const parsed = JSON.parse(value);

                return parsed &&
                    typeof parsed === 'object' &&
                    !Array.isArray(parsed)
                    ? parsed
                    : {};
            } catch (error) {
                console.warn(
                    '[FSM Appearance] Unable to parse preferences.',
                    error
                );

                return {};
            }
        },

        read() {
            try {
                const value = localStorage.getItem(storageKey);

                return this.parse(value);
            } catch (error) {
                console.warn(
                    '[FSM Appearance] Unable to read preferences.',
                    error
                );

                return {};
            }
        },

        write(value) {
            try {
                localStorage.setItem(
                    storageKey,
                    JSON.stringify(value)
                );
            } catch (error) {
                console.warn(
                    '[FSM Appearance] Unable to save preferences.',
                    error
                );
            }
        },

        remove() {
            try {
                localStorage.removeItem(storageKey);
            } catch (error) {
                console.warn(
                    '[FSM Appearance] Unable to reset preferences.',
                    error
                );
            }
        }
    };

    // ---------------------------------------------------------
    // 6. VALIDATION
    // ---------------------------------------------------------

    function isValidColor(value) {
        return typeof value === 'string' &&
            /^#[0-9A-Fa-f]{6}$/.test(value);
    }

    function validate(key, value) {
        if (key === 'themeColor') {
            return isValidColor(value);
        }

        return OPTIONS[key]?.includes(value) ?? false;
    }

    function sanitize(value = {}) {
        const result = { ...DEFAULTS };

        if (
            !value ||
            typeof value !== 'object' ||
            Array.isArray(value)
        ) {
            return result;
        }

        for (const [key, item] of Object.entries(value)) {
            if (validate(key, item)) {
                result[key] = item;
            }
        }

        return result;
    }

    // ---------------------------------------------------------
    // 7. LOAD PREFERENCES
    // ---------------------------------------------------------

    let settings = sanitize(storage.read());

    // ---------------------------------------------------------
    // 8. THEME APPLICATION
    // ---------------------------------------------------------

    function applyColorMode() {
        const mode = settings.colorMode;

        const isDark = mode === 'dark' ||
            (mode === 'system' && systemTheme.matches);

        root.classList.toggle('dark', isDark);

        root.dataset.theme = isDark ? 'dark' : 'light';
        root.dataset.colorMode = mode;
    }

    function applyThemeColor() {
        const color = settings.themeColor;

        root.style.setProperty('--brand', color);

        root.style.setProperty(
            '--brand-hover',
            `color-mix(in oklab, ${color} 85%, black)`
        );

        root.style.setProperty(
            '--brand-muted',
            `color-mix(in srgb, ${color} 10%, transparent)`
        );

        root.style.setProperty(
            '--brand-subtle',
            `color-mix(in srgb, ${color} 6%, transparent)`
        );
    }

    // ---------------------------------------------------------
    // 9. FONT APPLICATION
    // ---------------------------------------------------------

    function applyFont() {
        root.style.setProperty(
            '--font-body',
            `"${settings.font}", sans-serif`
        );
    }

    // ---------------------------------------------------------
    // 10. LAYOUT APPLICATION
    // ---------------------------------------------------------

    function applyLayout() {
        // Overall application layout.
        root.dataset.pageLayout = settings.pageLayout;

        // Individual interface preferences.
        root.dataset.radius = settings.radius;
        root.dataset.scale = settings.scale;
        root.dataset.contentLayout = settings.contentLayout;

        // Sidebar settings remain independent.
        root.dataset.sidebarVariant = settings.sidebarVariant;
        root.dataset.sidebarMode = settings.sidebarMode;
    }

    // ---------------------------------------------------------
    // 11. PAGE LAYOUT OPTIONS
    // ---------------------------------------------------------

    function applyPageLayoutSelection() {
        document.querySelectorAll('[data-page-layout]').forEach(button => {
            const isActive =
                button.dataset.pageLayout === settings.pageLayout;

            button.classList.toggle('is-active', isActive);

            button.setAttribute(
                'aria-pressed',
                String(isActive)
            );
        });
    }

    function bindPageLayoutOptions() {
        // Event delegation allows buttons to be added after
        // this script has loaded.
        document.addEventListener('click', event => {
            const target = event.target;

            if (!(target instanceof Element)) {
                return;
            }

            const button = target.closest('[data-page-layout]');

            if (!button) {
                return;
            }

            const pageLayout = button.dataset.pageLayout;

            if (!OPTIONS.pageLayout.includes(pageLayout)) {
                return;
            }

            set({ pageLayout });
        });
    }


    // ---------------------------------------------------------
// SIDEBAR TOGGLE
// ---------------------------------------------------------

function bindSidebarToggle() {

    const mobileBreakpoint = window.matchMedia('(max-width: 767px)');

    document.addEventListener('click', event => {

        const target = event.target;

        if (!(target instanceof Element)) {
            return;
        }

        const toggle = target.closest(
            '[data-sidebar-toggle], #sidebarToggle'
        );

        if (!toggle) {
            return;
        }

        // Mobile sidebar: control the drawer independently
        // of the user's saved desktop appearance preferences.
        if (mobileBreakpoint.matches) {

            const isOpen = root.dataset.mobileSidebarOpen === 'true';

            root.dataset.mobileSidebarOpen = String(!isOpen);

            window.dispatchEvent(
                new CustomEvent('fsm:sidebar-toggled', {
                    detail: {
                        isOpen: !isOpen,
                        mobile: true
                    }
                })
            );

            return;
        }

        // Desktop sidebar behaviour.
        switch (settings.sidebarMode) {

            case 'expanded':
                set({
                    sidebarMode: 'hidden'
                });
                break;

            case 'icon':
            case 'hidden':
                set({
                    sidebarMode: 'expanded'
                });
                break;

            default:
                set({
                    sidebarMode: 'expanded'
                });
                break;
        }
    });
}

    // Add near your other helper functions (e.g., under Section 11)
    function hasCustomSettings() {
        return Object.keys(DEFAULTS).some(key => {
            if (key === 'themeColor') {
                return settings[key].toUpperCase() !== DEFAULTS[key].toUpperCase();
            }
            return settings[key] !== DEFAULTS[key];
        });
    }

    function applyResetIndicator() {
        const badge = document.getElementById('appearanceResetBadge');
        if (!badge) {
            return;
        }

        badge.classList.toggle('hidden', !hasCustomSettings());
    }

    // ---------------------------------------------------------
    // 12. APPLY ALL SETTINGS
    // ---------------------------------------------------------

    function apply() {
        applyColorMode();
        applyThemeColor();
        applyFont();
        applyLayout();

        applyPageLayoutSelection();
        applyResetIndicator();


        window.dispatchEvent(
            new CustomEvent('fsm:appearance-changed', {
                detail: { ...settings }
            })
        );
    }

    // ---------------------------------------------------------
    // 13. UPDATE SETTINGS
    // ---------------------------------------------------------

    function set(updates = {}) {
        if (
            !updates ||
            typeof updates !== 'object' ||
            Array.isArray(updates)
        ) {
            return get();
        }

        const next = { ...settings };

        for (const [key, value] of Object.entries(updates)) {
            if (validate(key, value)) {
                next[key] = value;
            }
        }

        settings = sanitize(next);

        storage.write(settings);

        apply();

        return get();
    }

    // ---------------------------------------------------------
    // 14. RESET SETTINGS
    // ---------------------------------------------------------

    function reset() {
        settings = { ...DEFAULTS };

        storage.remove();

        apply();

        return get();
    }

    // ---------------------------------------------------------
    // 15. GET SETTINGS
    // ---------------------------------------------------------

    function get() {
        return { ...settings };
    }

    // ---------------------------------------------------------
    // 16. SYSTEM THEME LISTENER
    // ---------------------------------------------------------

    systemTheme.addEventListener('change', () => {
        if (settings.colorMode === 'system') {
            applyColorMode();

            window.dispatchEvent(
                new CustomEvent('fsm:appearance-changed', {
                    detail: { ...settings }
                })
            );
        }
    });

    // ---------------------------------------------------------
    // 17. CROSS-TAB SYNCHRONISATION
    // ---------------------------------------------------------

    window.addEventListener('storage', event => {
        if (event.key !== storageKey) {
            return;
        }

        settings = event.newValue
            ? sanitize(storage.parse(event.newValue))
            : { ...DEFAULTS };

        apply();
    });

    // ---------------------------------------------------------
    // 18. INITIALISE
    // ---------------------------------------------------------

    bindPageLayoutOptions();
    bindSidebarToggle();

    // Apply preferences immediately to prevent a visual flash.
    apply();

    // The Appearance Drawer may not exist yet when this script
    // loads in the document head. Synchronise its buttons once
    // the DOM has been parsed.
    function initialiseAppearanceControls() {
        applyPageLayoutSelection();
        applyResetIndicator();
    }

    if (document.readyState === 'loading') {
        document.addEventListener(
            'DOMContentLoaded',
            initialiseAppearanceControls,
            { once: true }
        );
    } else {
        initialiseAppearanceControls();
    }

    // ---------------------------------------------------------
    // 19. PUBLIC API
    // ---------------------------------------------------------

    window.FsmAppearance = Object.freeze({
        get,
        set,
        reset,

        setThemeColor(color) {
            return set({ themeColor: color });
        },

        setFont(font) {
            return set({ font });
        },

        setColorMode(colorMode) {
            return set({ colorMode });
        },

        setRadius(radius) {
            return set({ radius });
        },

        setPageLayout(pageLayout) {
            return set({ pageLayout });
        },

        setContentLayout(contentLayout) {
            return set({ contentLayout });
        },

        setScale(scale) {
            return set({ scale });
        },

        setSidebarVariant(sidebarVariant) {
            return set({ sidebarVariant });
        },

        setSidebarMode(sidebarMode) {
            return set({ sidebarMode });
        },

        storageKey,

        defaults: { ...DEFAULTS },

        options: OPTIONS
    });

})();
