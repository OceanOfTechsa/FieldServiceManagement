(function () {
    "use strict";

    const DIALOG_ID = "fsm-remove-link-dialog";
    const TRIGGER_SELECTOR = "[data-fsm-delete-link-trigger]";
    const CANCEL_SELECTOR = "[data-fsm-delete-link-cancel]";
    const FORM_ID = "fsm-unlink-form";
    const ID_INPUT_ID = "fsm-remove-link-id-input";
    const NAME_LABEL_ID = "fsm-remove-link-item-name";
    const ERROR_ID = "fsm-remove-link-error";
    const ERROR_TEXT_ID = "fsm-remove-link-error-text";

    let dialogEl = null;
    let formEl = null;
    let idInput = null;
    let nameLabel = null;
    let confirmBtn = null;
    let errorEl = null;
    let errorTextEl = null;

    function init() {
        dialogEl = document.getElementById(DIALOG_ID);
        formEl = document.getElementById(FORM_ID);
        idInput = document.getElementById(ID_INPUT_ID);
        nameLabel = document.getElementById(NAME_LABEL_ID);
        confirmBtn = document.getElementById("fsm-unlink-confirm");
        errorEl = document.getElementById(ERROR_ID);
        errorTextEl = document.getElementById(ERROR_TEXT_ID);

        if (!dialogEl || !formEl || !idInput) return;

        document.addEventListener("click", onDocumentClick);
        document.addEventListener("keydown", onKeyDown);

        formEl.addEventListener("submit", onSubmit);
    }

    // ---- Delegated clicks (trigger + cancel; markup may be re-rendered) ---

    function onDocumentClick(e) {
        const trigger = e.target.closest(TRIGGER_SELECTOR);
        if (trigger) {
            openDialog(trigger.dataset.id, trigger.dataset.name);
            return;
        }

        const cancelBtn = e.target.closest(CANCEL_SELECTOR);
        if (cancelBtn) {
            closeDialog();
        }
    }

    function onKeyDown(e) {
        if (e.key === "Escape" && isDialogOpen()) closeDialog();
    }

    // ---- Open/close ---------------------------------------------------

    function isDialogOpen() {
        return dialogEl.classList.contains("show");
    }

    function openDialog(id, name) {
        idInput.value = id || "";
        if (nameLabel) nameLabel.textContent = name || "";
        clearError();
        dialogEl.classList.add("show");
    }

    function closeDialog() {
        dialogEl.classList.remove("show");
        idInput.value = "";
        if (nameLabel) nameLabel.textContent = "";
        clearError();
    }

    // ---- Error helpers --------------------------------------------------

    function showError(message) {
        if (!errorEl) return;
        if (errorTextEl) errorTextEl.textContent = message;
        errorEl.classList.remove("d-none");
    }

    function clearError() {
        if (!errorEl) return;
        if (errorTextEl) errorTextEl.textContent = "";
        errorEl.classList.add("d-none");
    }

    // ---- Submit -----------------------------------------------------------

    async function onSubmit(e) {
        e.preventDefault();

        if (!idInput.value) {
            showError("No address link selected.");
            return;
        }

        const formData = new FormData(formEl);
        const token = formEl.querySelector("input[name='__RequestVerificationToken']")?.value;

        setSubmitting(true);

        try {
            const response = await fetch(formEl.action, {
                method: "POST",
                headers: token ? { "RequestVerificationToken": token } : {},
                body: formData
            });

            let result = null;
            try {
                result = await response.json();
            } catch {
                // non-JSON response; fall through to status check below
            }

            if (!response.ok || (result && result.success === false)) {
                showError((result && result.message) || "Couldn't remove the address link. Please try again.");
                return;
            }

            document.dispatchEvent(new CustomEvent("address:unlinked", { detail: result }));
            window.location.reload();
        } catch (err) {
            showError("Network error — please try again.");
        } finally {
            setSubmitting(false);
        }
    }

    function setSubmitting(isSubmitting) {
        if (!confirmBtn) return;
        confirmBtn.disabled = isSubmitting;
        confirmBtn.classList.toggle("disabled", isSubmitting);
    }

    document.addEventListener("DOMContentLoaded", init);
})();