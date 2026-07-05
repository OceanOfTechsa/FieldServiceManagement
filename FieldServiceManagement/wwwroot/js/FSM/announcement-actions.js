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