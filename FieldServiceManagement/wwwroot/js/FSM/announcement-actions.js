document.addEventListener('click', (e) => {
    const btn = e.target.closest('.fsm-mark-seen-btn');
    if (!btn || btn.disabled) return;

    const id = btn.dataset.announcementId;
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

    const originalContent = btn.innerHTML;

    btn.disabled = true;
    btn.innerHTML = '<span class="fsm-btn-spinner"></span> Marking as seen...';

    fetch(`/Announcements/MarkAsSeen?id=${id}`, {
        method: 'POST',
        headers: {
            'RequestVerificationToken': token || ''
        }
    })
        .then(res => {
            if (!res.ok) throw new Error('Failed to mark as seen');


            // Re-fetch the whole partial so tabs, counts, and row
            // placement stay correct — button state resets naturally
            // since the DOM is fully replaced.
            window.FsmAnnouncements?.refresh();
        })
        .catch(err => {
            btn.disabled = false;
            btn.innerHTML = originalContent; // restore on failure so user can retry
        });
});

function updateTabCounts() {
    const allCount = document.querySelectorAll('.fsm-announcement-tab-panel[data-panel="all"] .fsm-announcement-row').length;
    const seenCount = document.querySelectorAll('.fsm-announcement-tab-panel[data-panel="seen"] .fsm-announcement-row').length;
    const newCount = document.querySelectorAll('.fsm-announcement-tab-panel[data-panel="new"] .fsm-announcement-row').length;

    document.querySelector('.fsm-announcement-tab[data-tab="all"] .fsm-tab-count').textContent = `(${allCount})`;
    document.querySelector('.fsm-announcement-tab[data-tab="seen"] .fsm-tab-count').textContent = `(${seenCount})`;
    document.querySelector('.fsm-announcement-tab[data-tab="new"] .fsm-tab-count').textContent = `(${newCount})`;
}

(function () {

    function getToken() {
        return document.querySelector('input[name="__RequestVerificationToken"]')?.value ?? '';
    }

    function getCheckedIds() {
        const ids = new Set();

        document.querySelectorAll('.fsm-row-checkbox:checked').forEach(cb => {
            const id = cb.dataset.announcementId;
            if (id) ids.add(id);
        });

        return [...ids];
    }

    function syncDuplicateCheckboxes(id, checked) {
        document.querySelectorAll(`.fsm-row-checkbox[data-announcement-id="${id}"]`)
            .forEach(cb => cb.checked = checked);
    }

    function syncBulkButton() {
        const bulkBtn = document.getElementById('fsm-mark-seen-bulk-btn');
        if (!bulkBtn) return;

        const selected = getCheckedIds();

        bulkBtn.disabled = selected.length === 0;
        bulkBtn.textContent = selected.length
            ? `Mark ${selected.length} as seen`
            : 'Mark as seen';
    }

    // Select All
    document.addEventListener('change', e => {

        if (e.target.id !== 'fsm-select-all-checkbox')
            return;

        const checked = e.target.checked;

        document.querySelectorAll('.fsm-row-checkbox:not([disabled])')
            .forEach(cb => cb.checked = checked);

        syncBulkButton();
    });

    // Individual checkbox
    document.addEventListener('change', e => {

        if (!e.target.classList.contains('fsm-row-checkbox'))
            return;

        syncDuplicateCheckboxes(
            e.target.dataset.announcementId,
            e.target.checked
        );

        syncBulkButton();
    });

    // Bulk Mark Seen
    document.addEventListener('click', async e => {

        const bulkBtn = document.getElementById('fsm-mark-seen-bulk-btn');

        if (!bulkBtn || (e.target !== bulkBtn && !bulkBtn.contains(e.target)))
            return;

        const ids = getCheckedIds();

        if (!ids.length)
            return;

        const original = bulkBtn.innerHTML;

        bulkBtn.disabled = true;
        bulkBtn.innerHTML =
            `<span class="fsm-btn-spinner"></span> Marking ${ids.length}...`;

        try {

            const res = await fetch('/Announcements/MarkBulkAsSeen', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': getToken()
                },
                body: JSON.stringify({ ids })
            });

            if (!res.ok)
                throw new Error();

            window.FsmAnnouncements?.refresh();

            setTimeout(() => {
                syncBulkButton();
            }, 100);

        } catch {

            bulkBtn.disabled = false;
            bulkBtn.innerHTML = original;
        }
    });

    // Single Mark Seen
    document.addEventListener('click', e => {

        const btn = e.target.closest('.fsm-mark-seen-btn');

        if (!btn || btn.disabled || btn.classList.contains('seen'))
            return;

        const id = btn.dataset.announcementId;
        const original = btn.innerHTML;

        btn.disabled = true;
        btn.innerHTML =
            '<span class="fsm-btn-spinner"></span> Marking as seen...';

        fetch(`/Announcements/MarkAsSeen?id=${id}`, {
            method: 'POST',
            headers: {
                'RequestVerificationToken': getToken()
            }
        })
            .then(r => {

                if (!r.ok)
                    throw new Error();

                window.FsmAnnouncements?.refresh();

            })
            .catch(() => {

                btn.disabled = false;
                btn.innerHTML = original;
            });
    });

    window.FsmAnnouncementsBulk = {
        sync: syncBulkButton
    };

    syncBulkButton();

})();