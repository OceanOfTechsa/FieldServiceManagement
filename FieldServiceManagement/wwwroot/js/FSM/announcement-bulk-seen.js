(function () {
    const footerBtn = document.getElementById('fsm-announcements-footer');
    const bulkBtn = document.getElementById('fsm-mark-seen-bulk-btn');

    if (!footerBtn || !bulkBtn) return;

    function getCheckedRows() {
        return Array.from(document.querySelectorAll('.fsm-announcement-row input[type="checkbox"]:checked'));
    }

    function updateFooterVisibility() {
        const checkedCount = getCheckedRows().length;
        footerBtn.style.display = checkedCount > 0 ? '' : 'none';
    }

    // Delegated — works even though checkboxes are injected dynamically via AJAX
    document.addEventListener('change', (e) => {
        if (e.target.matches('.fsm-announcement-row input[type="checkbox"]')) {
            updateFooterVisibility();
        }
    });

    bulkBtn.addEventListener('click', async () => {
        const checked = getCheckedRows();
        if (!checked.length) return;

        bulkBtn.disabled = true;

        const ids = checked.map(cb => cb.closest('.fsm-announcement-row')?.dataset.id).filter(Boolean);

        try {
            await Promise.all(ids.map(id =>
                fetch(`/Announcements/MarkAsSeen?id=${id}`, { method: 'POST' })
            ));

            // Update each row's UI in place, same as the single mark-as-seen action
            ids.forEach(id => {
                const row = document.querySelector(`.fsm-announcement-row[data-id="${id}"]`);
                if (!row) return;

                row.classList.remove('unseen');
                row.querySelector('.fsm-unseen-dot')?.remove();

                const btn = row.querySelector('.fsm-mark-seen-btn');
                if (btn) {
                    btn.innerHTML = '<i class="bi bi-check2-all"></i> Seen';
                    btn.classList.add('seen');
                    btn.disabled = true;
                }

                const checkbox = row.querySelector('input[type="checkbox"]');
                if (checkbox) checkbox.checked = false;
            });

            updateFooterVisibility();
        } catch (err) {
            console.error('Bulk mark as seen failed', err);
        } finally {
            bulkBtn.disabled = false;
        }
    });
})();