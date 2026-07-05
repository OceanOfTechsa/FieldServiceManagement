(function () {
    const content = document.getElementById('fsm-announcements-content');
    const triggers = document.querySelectorAll('[data-fsm-announcements-trigger]');
    const banner = document.getElementById('announcementsSupportBanner');

    if (!content) return;

    let loaded = false;
    let loading = false;

    function loadAnnouncements(force = false) {
        if (loading) return;
        if (loaded && !force) return;

        loading = true;

        fetch('/Announcements/GetAnnouncementsPartial')
            .then(res => {
                if (!res.ok) throw new Error('Failed to load announcements');
                return res.text();
            })
            .then(html => {
                content.innerHTML = html;
                loaded = true;
                loading = false;
                banner.classList.add("d-none");
                // Let other scripts (collapsible, tabs, etc.) know fresh
                // content just landed in the DOM so they can re-init anything
                // that needs actual layout to calculate (e.g. line-clamp overflow).
                document.dispatchEvent(new CustomEvent('fsm:announcements:loaded'));
            })
            .catch(err => {
                content.innerHTML = `
                    <div class="fsm-empty-state">
                        <div class="fsm-empty-icon">
                            <i class="bi bi-megaphone-fill"></i>
                        </div>

                        <p class="text-danger small mb-3">
                            Could not load announcements.
                        </p>
                    </div>
                    `;
                loading = false;
                banner.classList.remove("d-none");
            });
    }

    triggers.forEach(t => {
        t.addEventListener('click', () => loadAnnouncements(false));
    });

    // Expose for other scripts to trigger a forced refresh
    window.FsmAnnouncements = {
        refresh: () => loadAnnouncements(true)
    };
})();