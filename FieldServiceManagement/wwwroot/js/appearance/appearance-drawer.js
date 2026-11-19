window.FsmAppearanceDrawer = (() => {
    const drawer = document.getElementById('appearanceDrawer');
    const overlay = document.getElementById('appearanceOverlay');

    if (!drawer || !overlay || !window.FsmAppearance) {
        return;
    }

    const colorPicker = document.getElementById('appearanceThemeColor');
    const colorInput = document.getElementById('appearanceThemeColorHex');

    const open = () => {
        drawer.classList.remove('translate-x-full');
        overlay.classList.remove('hidden');
        document.body.classList.add('overflow-hidden');
    };

    const close = () => {
        drawer.classList.add('translate-x-full');
        overlay.classList.add('hidden');
        document.body.classList.remove('overflow-hidden');
    };

    const update = (property, value) => {
        window.FsmAppearance.set({ [property]: value });
        sync();
    };

    const sync = () => {
        const settings = window.FsmAppearance.get();

        colorPicker.value = settings.themeColor;
        colorInput.value = settings.themeColor;

        const selectors = {
            font: '[data-font]',
            colorMode: '[data-color-mode]',
            radius: '[data-radius]',
            contentLayout: '[data-content-layout]',
            scale: '[data-scale]',
            sidebarVariant: '[data-sidebar-variant]',
            sidebarMode: '[data-sidebar-mode]'
        };

        Object.entries(selectors).forEach(([property, selector]) => {
            drawer.querySelectorAll(selector).forEach(button => {
                const attribute = selector.match(/\[data-([a-z-]+)\]/)[1];
                const value = button.dataset[attribute.replace(/-([a-z])/g, (_, letter) => letter.toUpperCase())];

                button.classList.toggle('is-active', value === settings[property]);
            });
        });
    };

    // Open drawer
    document.querySelectorAll('[data-appearance-open]').forEach(button => {
        button.addEventListener('click', open);
    });

    // Close drawer
    document.querySelectorAll('[data-appearance-close]').forEach(button => {
        button.addEventListener('click', close);
    });

    // Appearance options
    drawer.addEventListener('click', event => {
        const button = event.target.closest('button[data-font], button[data-color-mode], button[data-radius], button[data-content-layout], button[data-scale], button[data-sidebar-variant], button[data-sidebar-mode]');

        if (!button) return;

        const attributes = [
            ['data-font', 'font'],
            ['data-color-mode', 'colorMode'],
            ['data-radius', 'radius'],
            ['data-content-layout', 'contentLayout'],
            ['data-scale', 'scale'],
            ['data-sidebar-variant', 'sidebarVariant'],
            ['data-sidebar-mode', 'sidebarMode']
        ];

        const match = attributes.find(([attribute]) => button.hasAttribute(attribute));

        if (match) {
            update(match[1], button.getAttribute(match[0]));
        }
    });

    // Theme colour
    const setColor = value => {
        if (!/^#[0-9A-Fa-f]{6}$/.test(value)) return;

        update('themeColor', value.toUpperCase());
    };

    colorPicker.addEventListener('input', event => {
        setColor(event.target.value);
    });

    colorInput.addEventListener('change', event => {
        setColor(event.target.value.trim());
    });

    document.getElementById('appearanceResetColor')?.addEventListener('click', () => {
        update('themeColor', window.FsmAppearance.defaults.themeColor);
    });

    // Reset all settings
    document.getElementById('appearanceResetAll')?.addEventListener('click', () => {
        window.FsmAppearance.reset();
        sync();
    });

    // Escape key
    document.addEventListener('keydown', event => {
        if (event.key === 'Escape' && !drawer.classList.contains('translate-x-full')) {
            close();
        }
    });

    // Sync after settings change elsewhere
    window.addEventListener('fsm:appearance-changed', sync);

    sync();

    return { open, close, sync };
})();