const form = document.getElementById('change-password-form');
const newPassword = document.getElementById('NewPassword');
const confirmPassword = document.getElementById('ConfirmPassword');

function validatePasswordMatch() {
    if (confirmPassword.value === '') return;

    if (newPassword.value !== confirmPassword.value) {
        confirmPassword.setCustomValidity('Passwords do not match.');
        confirmPassword.classList.add('is-invalid');
        confirmPassword.classList.remove('is-valid');
    } else {
        confirmPassword.setCustomValidity('');
        confirmPassword.classList.remove('is-invalid');
        confirmPassword.classList.add('is-valid');
    }
}

newPassword.addEventListener('input', validatePasswordMatch);
confirmPassword.addEventListener('input', validatePasswordMatch);

form.addEventListener('submit', function (e) {
    if (newPassword.value !== confirmPassword.value) {
        e.preventDefault();
        confirmPassword.setCustomValidity('Passwords do not match.');
        confirmPassword.classList.add('is-invalid');
        confirmPassword.focus();
    }
});