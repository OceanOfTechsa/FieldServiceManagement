const dialog = document.getElementById('fsm-disable-2fa-dialog');
const triggerBtn = document.getElementById('fsm-disable-2fa-trigger');
const closeBtn = document.getElementById('fsm-disable-2fa-close');
const confirmBtn = document.getElementById('fsm-disable-2fa-confirm');
const form = document.getElementById('disable-2fa-form');

triggerBtn.addEventListener('click', function () {
    dialog.classList.add('show');
});

closeBtn.addEventListener('click', function () {
    dialog.classList.remove('show');
});

confirmBtn.addEventListener('click', function () {
    form.submit();
});