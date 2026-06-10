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
