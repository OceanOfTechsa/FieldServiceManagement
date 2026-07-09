(function () {
    const backBtn = document.getElementById('fsm-breadcrumb-back');
    if (!backBtn) return;

    backBtn.addEventListener('click', () => {
        // If there's actual history to go back to, use it
        // Otherwise fall back to the referrer or home
        if (window.history.length > 1) {
            window.history.back();
        } else if (document.referrer) {
            window.location.href = document.referrer;
        } else {
            window.location.href = '/';
        }
    });
}());