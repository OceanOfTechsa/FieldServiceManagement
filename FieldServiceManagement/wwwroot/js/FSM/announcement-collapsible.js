(function () {
    const CLAMP_LINES = 2;

    function getLineHeight(block) {
        const cs = getComputedStyle(block);
        let lh = parseFloat(cs.lineHeight);
        if (isNaN(lh)) {
            // fallback if line-height is "normal"
            lh = parseFloat(cs.fontSize) * 1.2;
        }
        return lh;
    }

    function setup(block) {
        const toggle = block.nextElementSibling;
        if (!toggle || !toggle.classList.contains('fsm-collapsible-toggle')) return;

        const lineHeight = getLineHeight(block);
        const collapsedMax = Math.ceil(lineHeight * CLAMP_LINES);
        const fullHeight = block.scrollHeight;

        block.dataset.collapsedHeight = collapsedMax;
        block.dataset.fullHeight = fullHeight;

        const isOverflowing = fullHeight > collapsedMax + 1;

        if (!block.classList.contains('expanded')) {
            block.style.maxHeight = isOverflowing ? collapsedMax + 'px' : fullHeight + 'px';
        } else {
            block.style.maxHeight = fullHeight + 'px';
        }

        toggle.style.display = isOverflowing || block.classList.contains('expanded') ? '' : 'none';
    }

    function initAll() {
        document.querySelectorAll('.fsm-clamp').forEach(setup);
    }

    document.addEventListener('click', (e) => {
        const toggleBtn = e.target.closest('.fsm-collapsible-toggle');
        if (toggleBtn) {
            const block = toggleBtn.previousElementSibling;
            if (!block || !block.classList.contains('fsm-clamp')) return;

            const expanded = block.classList.toggle('expanded');

            if (expanded) {
                block.style.maxHeight = block.dataset.fullHeight + 'px';
            } else {
                block.style.maxHeight = block.dataset.collapsedHeight + 'px';
            }

            toggleBtn.textContent = expanded ? 'Show less' : 'Show more';
            return;
        }

        const tabBtn = e.target.closest('.fsm-announcement-tab');
        if (tabBtn) {
            requestAnimationFrame(initAll);
        }
    });

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initAll);
    } else {
        initAll();
    }
    document.addEventListener('fsm:announcements:loaded', () => {
        requestAnimationFrame(initAll);
    });

    window.addEventListener('resize', initAll);
})();