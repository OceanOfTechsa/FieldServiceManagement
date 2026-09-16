(function () {
    "use strict";

    const DIALOG_ID = "fsm-delete-dialog";
    const CANCEL_SELECTOR = "[data-fsm-delete-cancel]";
    const FORM_ID = "fsm-delete-company-form";
    const ERROR_ID = "fsm-delete-company-error";
    const ERROR_TEXT_ID = "fsm-delete-company-error-text";

    let dialogEl = null;
    let formEl = null;
    let confirmBtn = null;
    let errorEl = null;
    let errorTextEl = null;

    function init() {
        dialogEl = document.getElementById(DIALOG_ID);
        formEl = document.getElementById(FORM_ID);
        confirmBtn = document.getElementById("fsm-delete-confirm");
        errorEl = document.getElementById(ERROR_ID);
        errorTextEl = document.getElementById(ERROR_TEXT_ID);

        if (!dialogEl || !formEl) return;

        document.addEventListener("click", onDocumentClick);
        document.addEventListener("keydown", onKeyDown);

        formEl.addEventListener("submit", onSubmit);
    }

    // ---- Cancel / escape (dialog is assumed opened elsewhere) -------------

    function onDocumentClick(e) {
        const cancelBtn = e.target.closest(CANCEL_SELECTOR);
        if (cancelBtn) closeDialog();
    }

    function onKeyDown(e) {
        if (e.key === "Escape" && isDialogOpen()) closeDialog();
    }

    function isDialogOpen() {
        return dialogEl.classList.contains("show");
    }

    function closeDialog() {
        dialogEl.classList.remove("show");
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

    // ---- Submit -------------------------------------------------------

    async function onSubmit(e) {
        e.preventDefault();

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
                showError((result && result.message) || "Failed to delete this company. Please try again.");
                return;
            }

            document.dispatchEvent(new CustomEvent("company:deleted", { detail: result }));
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