(function () {
    function activate(tabName) {
        document.querySelectorAll('.fsm-announcement-tab').forEach(t => {
            t.classList.toggle('active', t.dataset.tab === tabName);
        });
        document.querySelectorAll('.fsm-announcement-tab-panel').forEach(p => {
            p.classList.toggle('active', p.dataset.panel === tabName);
        });
    }

    document.addEventListener('click', (e) => {
        const tab = e.target.closest('.fsm-announcement-tab');
        if (!tab) return;

        activate(tab.dataset.tab);
    });
})();
