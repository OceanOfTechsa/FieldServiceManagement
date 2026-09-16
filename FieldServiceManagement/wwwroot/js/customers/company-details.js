(function () {
    const transferDialog = document.getElementById('fsm-change-owner-dialog');
    const transferTriggers = document.querySelectorAll('[data-fsm-transfer-trigger]');
    const transferCancels = document.querySelectorAll('[data-fsm-transfer-cancel]');

    const deleteDialog = document.getElementById('fsm-delete-dialog');
    const deleteTriggers = document.querySelectorAll('[data-fsm-delete-trigger]');
    const deleteCancels = document.querySelectorAll('[data-fsm-delete-cancel]');

    const unlinkDialog = document.getElementById('fsm-unlink-dialog');
    const unlinkTriggers = document.querySelectorAll('[data-fsm-unlink-trigger]');
    const unlinkCancels = document.querySelectorAll('[data-fsm-unlink-cancel]');
    const unlinkIdInput = document.getElementById('fsm-unlink-id-input');
    const unlinkNameEl = document.getElementById('fsm-unlink-item-name');

    transferTriggers.forEach(btn => btn.addEventListener('click', (e) => {
        e.stopPropagation();
        transferDialog?.classList.add('show');
    }));

    transferCancels.forEach(btn => btn.addEventListener('click', () => {
        transferDialog?.classList.remove('show');
    }));

    deleteTriggers.forEach(btn => btn.addEventListener('click', (e) => {
        e.stopPropagation();
        deleteDialog?.classList.add('show');
    }));

    deleteCancels.forEach(btn => btn.addEventListener('click', () => {
        deleteDialog?.classList.remove('show');
    }));


    unlinkTriggers.forEach(btn => btn.addEventListener('click', (e) => {
        e.stopPropagation();

        const id = btn.dataset.id;
        const name = btn.dataset.name;

        unlinkIdInput.value = id;
        unlinkNameEl.textContent = name;

        unlinkDialog?.classList.add('show');
    }));

    unlinkCancels.forEach(btn => btn.addEventListener('click', () => {
        unlinkDialog?.classList.remove('show');
        unlinkIdInput.value = '';
        unlinkNameEl.textContent = '';
    }));

    const track = document.getElementById('fsm-tab-track');
    const moreWrap = document.getElementById('fsm-tab-more');
    const moreBtn = document.getElementById('fsm-tab-more-btn');
    const moreMenu = document.getElementById('fsm-tab-more-menu');
    const tabs = Array.from(track?.querySelectorAll('.fsm-tab') ?? []);
    const panels = Array.from(document.querySelectorAll('.fsm-tab-panel'));

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
        if (!moreWrap) return;

        tabs.forEach(t => { t.style.display = ''; });
        moreWrap.style.display = 'none';
        moreWrap.style.visibility = 'hidden';
        moreMenu.innerHTML = '';

        requestAnimationFrame(() => {
            const barWidth = track.parentElement.offsetWidth;
            const moreBtnW = 44;
            let used = 0;
            const overflowed = [];

            tabs.forEach(tab => {
                used += tab.getBoundingClientRect().width;
                if (used > barWidth - moreBtnW) {
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
        document.querySelectorAll('.fsm-dropdown-menu.show').forEach(m => {
            m.classList.remove('show');
            m.closest('.fsm-dropdown')
                ?.querySelector('[aria-expanded]')
                ?.setAttribute('aria-expanded', 'false');
        });

        if (!isOpen) {
            moreMenu.classList.add('show');
            moreBtn.setAttribute('aria-expanded', 'true');
        }
    });

    moreMenu?.addEventListener('click', (e) => e.stopPropagation());

    document.addEventListener('click', () => {
        moreMenu?.classList.remove('show');
        moreBtn?.setAttribute('aria-expanded', 'false');
    });

    requestAnimationFrame(() => requestAnimationFrame(recalc));
    window.addEventListener('resize', recalc);
})();