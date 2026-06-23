const editToggleBtn = document.getElementById('toggleEdit');
const cancelEditBtn = document.getElementById('cancelEdit');

const infoDiv = document.getElementById('infoDiv');
const editFormDiv = document.getElementById('editForm');

const form = document.getElementById('edit-org-form');

const cancelDialog = document.getElementById('fsm-cancel-edit-dialog');
const cancelDialogClose = document.getElementById('fsm-cancel-edit-close');
const cancelDialogConfirm = document.getElementById('fsm-cancel-edit-confirm');

const avatarWrapper = document.querySelector('.avatar-upload-wrapper');
const avatarInput = document.getElementById('avatar-upload');
const avatarPreview = document.getElementById('avatar-preview');
const avatarLoader = document.getElementById('avatar-loader');

let originalValues = {};
const originalAvatarSrc = avatarPreview.src;

function captureInitialValues() {
    originalValues = {};
    form.querySelectorAll('input, select, textarea').forEach(field => {
        if (field.type === 'hidden') return;
        originalValues[field.name] = field.type === 'checkbox'
            ? field.checked
            : field.value;
    });
}

function hasChanges() {
    let changed = false;
    form.querySelectorAll('input, select, textarea').forEach(field => {
        if (field.type === 'hidden') return;
        const currentValue = field.type === 'checkbox'
            ? field.checked
            : field.value;
        if (originalValues[field.name] !== currentValue) {
            changed = true;
        }
    });
    return changed;
}

function restoreOriginalValues() {
    Object.keys(originalValues).forEach(key => {
        const field = form.querySelector(`[name="${key}"]`);
        if (!field) return;
        if (field.type === 'checkbox') {
            field.checked = originalValues[key];
        } else {
            field.value = originalValues[key];
        }
    });
}

function enterEditMode() {
    captureInitialValues();
    infoDiv.classList.add('d-none');
    editFormDiv.classList.remove('d-none');
    editToggleBtn.classList.add('d-none');
    cancelEditBtn.classList.remove('d-none');
    avatarWrapper.classList.add('avatar-editable');
}

function exitEditMode(discardChanges = false) {
    restoreOriginalValues();
    if (discardChanges) {
        avatarPreview.src = originalAvatarSrc;
        avatarInput.value = '';
    }
    infoDiv.classList.remove('d-none');
    editFormDiv.classList.add('d-none');
    editToggleBtn.classList.remove('d-none');
    cancelEditBtn.classList.add('d-none');
    avatarWrapper.classList.remove('avatar-editable');
}

editToggleBtn.addEventListener('click', () => enterEditMode());

cancelEditBtn.addEventListener('click', function () {
    if (!hasChanges()) {
        exitEditMode(true);
        return;
    }
    cancelDialog.classList.add('show');
});

cancelDialogClose.addEventListener('click', () => cancelDialog.classList.remove('show'));

cancelDialogConfirm.addEventListener('click', function () {
    cancelDialog.classList.remove('show');
    exitEditMode(true);
});

avatarWrapper.addEventListener('click', () => {
    const isEditMode = !editFormDiv.classList.contains('d-none');
    if (!isEditMode) return;
    avatarInput.click();
});

avatarInput.addEventListener('change', function () {
    const file = this.files[0];
    if (!file) return;
    avatarLoader.classList.remove('d-none');
    const reader = new FileReader();
    reader.onload = function (e) {
        avatarPreview.src = e.target.result;
        avatarLoader.classList.add('d-none');
    };
    reader.readAsDataURL(file);
});