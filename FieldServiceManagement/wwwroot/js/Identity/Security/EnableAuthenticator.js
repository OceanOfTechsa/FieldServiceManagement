const twoFactorInput = document.getElementById('TwoFactorCode');
const verifyBtn = document.getElementById('verify-code-btn');
const verifyError = document.getElementById('verify-error');
const step3 = document.getElementById('step-3');
const step3Divider = document.getElementById('step-3-divider');
const setupComplete = document.getElementById('setup-complete');
const warn = document.getElementById('warn');
const downloadBtn = document.getElementById('download-codes-btn');
const footer = document.getElementById('form-footer');

twoFactorInput.addEventListener('input', function (e) {
    let value = e.target.value.replace(/\D/g, '');
    if (value.length > 3) {
        value = value.substring(0, 3) + ' ' + value.substring(3, 6);
    }
    e.target.value = value;
});

twoFactorInput.addEventListener('keydown', function (e) {
    if (e.key === 'Enter') {
        e.preventDefault();
        verifyBtn.click();
    }
});

verifyBtn.addEventListener('click', async function () {
    verifyError.classList.add('d-none');
    verifyError.textContent = '';

    const code = twoFactorInput.value.trim();
    if (!code) {
        verifyError.textContent = 'Please enter your verification code.';
        verifyError.classList.remove('d-none');
        return;
    }

    const form = document.getElementById('enable-2fa-form');
    const formData = new FormData(form);

    verifyBtn.disabled = true;
    verifyBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span> Verifying...';

    try {
        const response = await fetch('/Identity/Account/Manage/Security/EnableAuthenticator', {
            method: 'POST',
            body: formData
        });

        const result = await response.json();

        if (result.success) {
            step3Divider.classList.remove('d-none');
            step3.classList.remove('d-none');

            twoFactorInput.disabled = true;
            verifyBtn.innerHTML = '<i class="bi bi-check-circle-fill me-1"></i> Verified';
            verifyBtn.classList.replace('btn-primary', 'btn-success');
            verifyBtn.disabled = true;

            step3.scrollIntoView({ behavior: 'smooth', block: 'start' });
        } else {
            verifyError.textContent = result.error ?? 'Invalid verification code, please try again.';
            verifyError.classList.remove('d-none');
            verifyBtn.disabled = false;
            verifyBtn.innerHTML = '<i class="bi bi-check-circle me-1"></i> Verify Code';
        }
    } catch {
        verifyError.textContent = 'Something went wrong. Please try again.';
        verifyError.classList.remove('d-none');
        verifyBtn.disabled = false;
        verifyBtn.innerHTML = '<i class="bi bi-check-circle me-1"></i> Verify Code';
    }
});

downloadBtn.addEventListener('click', function () {
    setTimeout(() => {
        step3.classList.add('d-none');
        step3Divider.classList.add('d-none');
        setupComplete.classList.remove('d-none');
        warn.classList.add("d-none");
        footer.classList.add('d-none');
        setupComplete.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }, 800);
});