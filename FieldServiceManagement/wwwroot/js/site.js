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


const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]')
const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl))