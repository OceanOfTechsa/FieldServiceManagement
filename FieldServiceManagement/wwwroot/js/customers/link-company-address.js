(function () {
    "use strict";

    const TOGGLE_SELECTOR = "[data-fsm-filter-toggle='linkAddress']";
    const PANEL_SELECTOR = "[data-fsm-filter-panel='linkAddress']";
    const CLEAR_SELECTOR = "[data-fsm-filter-clear='linkAddress']";
    const FORM_ID = "linkAddressForm";
    const CHECKBOX_SELECTOR = "[data-address-type-checkbox]";
    const ERROR_SELECTOR = "[data-link-address-error]";

    let panelEl = null;
    let toggleEl = null;
    let formEl = null;
    let errorEl = null;
    let submitBtn = null;

    function init() {
        toggleEl = document.getElementById("linkAddressToggle");
        panelEl = document.querySelector(PANEL_SELECTOR);
        formEl = document.getElementById(FORM_ID);
        errorEl = document.querySelector(ERROR_SELECTOR);
        submitBtn = document.getElementById("linkAddressApplyBtn");

        if (!toggleEl || !panelEl || !formEl) return;

        toggleEl.addEventListener("click", onToggleClick);
        document.addEventListener("click", onDocumentClick);
        document.addEventListener("keydown", onKeyDown);

        document.querySelectorAll(CHECKBOX_SELECTOR).forEach(function (checkbox) {
            checkbox.addEventListener("change", onAddressTypeChange);
        });

        const clearBtn = document.querySelector(CLEAR_SELECTOR);
        if (clearBtn) clearBtn.addEventListener("click", onClear);

        formEl.addEventListener("submit", onSubmit);
    }

    // ---- Panel open/close --------------------------------------------

    function onToggleClick(e) {
        e.stopPropagation();
        isPanelOpen() ? closePanel() : openPanel();
    }

    function onDocumentClick(e) {
        if (!isPanelOpen()) return;
        if (panelEl.contains(e.target) || toggleEl.contains(e.target)) return;
        closePanel();
    }

    function onKeyDown(e) {
        if (e.key === "Escape" && isPanelOpen()) closePanel();
    }

    function isPanelOpen() {
        return panelEl.classList.contains("show");
    }

    function openPanel() {
        panelEl.classList.add("show");
        toggleEl.setAttribute("aria-expanded", "true");
    }

    function closePanel() {
        panelEl.classList.remove("show");
        toggleEl.setAttribute("aria-expanded", "false");
        clearError();
    }

    // ---- Checkbox -> select group toggle --------------------------------

    function onAddressTypeChange(e) {
        const type = e.target.value;
        const group = formEl.querySelector(`[data-address-select-group="${type}"]`);
        if (!group) return;

        const select = group.querySelector("select");
        const isChecked = e.target.checked;

        group.classList.toggle("d-none", !isChecked);
        if (select) {
            select.disabled = !isChecked;
            if (!isChecked) select.value = "";
        }

        clearError();
    }

    // ---- Clear ------------------------------------------------------------

    function onClear(e) {
        e.preventDefault();
        formEl.reset();
        document.querySelectorAll(CHECKBOX_SELECTOR).forEach(function (checkbox) {
            checkbox.dispatchEvent(new Event("change"));
        });
        clearError();
    }

    // ---- Validation + submit ----------------------------------------------

    function validate() {
        const checked = Array.from(formEl.querySelectorAll(CHECKBOX_SELECTOR)).filter(c => c.checked);

        if (checked.length === 0) {
            showError("Select at least one address type.");
            return false;
        }

        for (const checkbox of checked) {
            const group = formEl.querySelector(`[data-address-select-group="${checkbox.value}"]`);
            const select = group ? group.querySelector("select") : null;
            if (!select || !select.value) {
                showError(`Select a ${checkbox.value.toLowerCase()} address.`);
                if (select) select.focus();
                return false;
            }
        }

        clearError();
        return true;
    }

    function showError(message) {
        if (!errorEl) return;
        errorEl.textContent = message;
        errorEl.classList.remove("d-none");
    }

    function clearError() {
        if (!errorEl) return;
        errorEl.textContent = "";
        errorEl.classList.add("d-none");
    }

    async function onSubmit(e) {
        e.preventDefault();
        if (!validate()) return;

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
            console.log(response)
            if (!response.ok || (result && result.success === false)) {
                showError((result && result.message) || "Couldn't link the address. Please try again.");
                return;
            }

            closePanel();
            formEl.reset();
            document.querySelectorAll(CHECKBOX_SELECTOR).forEach(c => c.dispatchEvent(new Event("change")));

            document.dispatchEvent(new CustomEvent("address:linked", { detail: result }));
            window.location.reload();
        } catch (err) {
            showError("Network error — please try again.");
        } finally {
            setSubmitting(false);
        }
    }

    function setSubmitting(isSubmitting) {
        if (!submitBtn) return;
        submitBtn.disabled = isSubmitting;
        submitBtn.classList.toggle("disabled", isSubmitting);
    }

    document.addEventListener("DOMContentLoaded", init);
})();