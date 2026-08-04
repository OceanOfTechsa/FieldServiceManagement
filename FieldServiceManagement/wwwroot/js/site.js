// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
'use strict';
(function () {
    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    })

})();


(function () {
    $(function () {
        $('[data-toggle="switch"]').bootstrapSwitch();
    })

})();

(function () {
    const loader = document.getElementById('fsm-page-loader');

    function hideLoader() {
        loader.classList.add('fsm-loader-hidden');
    }

    if (document.readyState === 'complete') {
        hideLoader();
    } else {
        window.addEventListener('load', hideLoader);
    }

    // Safety fallback — hide after 8s no matter what
    setTimeout(hideLoader, 8000);
})();

function showLoader(text = 'Loading', sub = 'Please wait a moment...') {
    document.querySelector('.fsm-loader-text').textContent = text;
    document.querySelector('.fsm-loader-sub').textContent = sub;
    document.getElementById('fsm-page-loader').classList.remove('fsm-loader-hidden');
}

function hideLoader() {
    document.getElementById('fsm-page-loader').classList.add('fsm-loader-hidden');
}
//Then call showLoader('Saving changes', 'Hang tight...') before any slow operation and hideLoader() when done.

document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.fsm-dialog-backdrop').forEach(backdrop => {
        backdrop.addEventListener('click', function (e) {
            if (e.target === this) {
                const dialogContent = this.querySelector('.fsm-dialog');
                if (dialogContent) {
                    dialogContent.classList.remove('focus-pulse');
                    void dialogContent.offsetWidth; 
                    dialogContent.classList.add('focus-pulse');
                }
            }
        });
    });
});


const fsmTooltipPopperConfig = (defaultBsPopperConfig) => ({
    ...defaultBsPopperConfig,
    modifiers: [
        ...defaultBsPopperConfig.modifiers,
        { name: 'offset', options: { offset: [0, 10] } }
    ]
});

document.querySelectorAll('[data-fsm-tooltip], [data-bs-toggle="tooltip"]').forEach(el => {
    if (el.dataset.fsmTooltip !== undefined) {
        const title = el.dataset.fsmTitle ?? '';
        const description = el.dataset.fsmDescription ?? '';

        new bootstrap.Tooltip(el, {
            html: true,
            placement: 'top',
            customClass: 'fsm-tooltip',
            title: `<div class="fsm-tooltip-card">
                        <div class="fsm-tooltip-title"> ${title}</div>
                        <div class="fsm-tooltip-description">${description}</div>
                    </div>`,
            template: `<div class="tooltip fsm-tooltip" role="tooltip">
                        <div class="tooltip-arrow"></div>
                        <div class="tooltip-inner"></div>
                    </div>`,
            popperConfig: fsmTooltipPopperConfig
        });
    } else {
        new bootstrap.Tooltip(el);
    }
});
const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]')
const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl))


// button group
document.querySelectorAll('.fsm-btn-group__trigger').forEach(trigger => {
    trigger.addEventListener('click', (e) => {
        e.stopPropagation();
        const group = trigger.closest('.fsm-btn-group');
        const menu = group.querySelector('.fsm-btn-group__menu');
        const isOpen = menu.classList.contains('show');

        document.querySelectorAll('.fsm-btn-group__menu.show').forEach(m => m.classList.remove('show'));

        if (!isOpen) {
            menu.classList.add('show');
            trigger.setAttribute('aria-expanded', 'true');
        } else {
            trigger.setAttribute('aria-expanded', 'false');
        }
    });
});

document.addEventListener('click', () => {
    document.querySelectorAll('.fsm-btn-group__menu.show').forEach(m => m.classList.remove('show'));
    document.querySelectorAll('.fsm-btn-group__trigger').forEach(t => t.setAttribute('aria-expanded', 'false'));
});



(function () {
    function initTabBar(bar) {
        const isVertical = bar.classList.contains('fsm-tab-bar-vertical');
        const suffix = isVertical ? '-vertical' : '';

        const track = bar.querySelector(`.fsm-tab-track${suffix}`);
        const moreWrap = bar.querySelector(`.fsm-tab-more${suffix}`);
        const moreBtn = bar.querySelector(`.fsm-tab-more-btn${suffix}`);
        const moreMenu = bar.querySelector(`.fsm-tab-more-menu${suffix}`);
        const tabs = Array.from(track?.querySelectorAll(`.fsm-tab${suffix}`) ?? []);
        const panelScope = bar.closest('[data-tab-scope]') || document;
        const panels = Array.from(panelScope.querySelectorAll(`.fsm-tab-panel${suffix}`));

        if (!track || !tabs.length) return;

        function activate(tabName) {
            tabs.forEach(t => t.classList.toggle('active', t.dataset.tab === tabName));
            panels.forEach(p => p.classList.toggle('active', p.dataset.panel === tabName));
            moreMenu?.querySelectorAll('button').forEach(btn => {
                btn.classList.toggle('active', btn.dataset.tab === tabName);
            });
        }

        tabs.forEach(tab => tab.addEventListener('click', () => activate(tab.dataset.tab)));

        function recalc() {
            if (!moreWrap || isVertical) {
                if (moreWrap) moreWrap.style.display = 'none';
                return;
            }

            tabs.forEach(t => { t.style.display = ''; });
            moreWrap.style.display = 'none';
            moreWrap.style.visibility = 'hidden';
            moreMenu.innerHTML = '';

            requestAnimationFrame(() => {
                const barSize = track.parentElement.offsetWidth;
                const moreBtnSize = 44;
                let used = 0;
                const overflowed = [];

                tabs.forEach(tab => {
                    used += tab.getBoundingClientRect().width;
                    if (used > barSize - moreBtnSize) {
                        tab.style.display = 'none';
                        overflowed.push(tab);
                    }
                });

                if (overflowed.length === 0) {
                    moreWrap.style.display = 'none';
                    moreWrap.style.visibility = '';
                    return;
                }

                moreWrap.style.display = '';
                moreWrap.style.visibility = '';

                overflowed.forEach(tab => {
                    const btn = document.createElement('button');
                    btn.type = 'button';
                    btn.role = 'menuitem';
                    btn.dataset.tab = tab.dataset.tab;
                    btn.textContent = tab.textContent.trim();
                    btn.className = 'dropdown-item' + (tab.classList.contains('active') ? ' active' : '');
                    btn.addEventListener('click', () => {
                        activate(tab.dataset.tab);
                        moreMenu.classList.remove('show');
                        moreBtn.setAttribute('aria-expanded', 'false');
                    });
                    moreMenu.appendChild(btn);
                });
            });
        }

        moreBtn?.addEventListener('click', (e) => {
            e.stopPropagation();
            const isOpen = moreMenu.classList.contains('show');
            document.querySelectorAll('.fsm-tab-more-menu.show, .fsm-tab-more-menu-vertical.show').forEach(m => {
                m.classList.remove('show');
                m.closest('.fsm-tab-more, .fsm-tab-more-vertical')
                    ?.querySelector('[aria-expanded]')
                    ?.setAttribute('aria-expanded', 'false');
            });

            if (!isOpen) {
                moreMenu.classList.add('show');
                moreBtn.setAttribute('aria-expanded', 'true');
            }
        });

        moreMenu?.addEventListener('click', (e) => e.stopPropagation());

        requestAnimationFrame(() => requestAnimationFrame(recalc));
        window.addEventListener('resize', recalc);
    }

    document.addEventListener('click', () => {
        document.querySelectorAll('.fsm-tab-more-menu.show, .fsm-tab-more-menu-vertical.show').forEach(m => m.classList.remove('show'));
        document.querySelectorAll('.fsm-tab-more-btn[aria-expanded="true"], .fsm-tab-more-btn-vertical[aria-expanded="true"]')
            .forEach(b => b.setAttribute('aria-expanded', 'false'));
    });

    document.querySelectorAll('.fsm-tab-bar, .fsm-tab-bar-vertical').forEach(initTabBar);
})();