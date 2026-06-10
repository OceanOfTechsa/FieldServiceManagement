
document.addEventListener("DOMContentLoaded", function () {

    document.querySelectorAll("form").forEach(form => {

        form.addEventListener("submit", function (e) {

            const button = document.activeElement;

            if (!button || button.type !== "submit" || button.disabled) return;

            const hasInputs = form.querySelectorAll("input, select, textarea").length > 0;

            if (hasInputs && typeof $(form).valid === "function") {

                if (!$(form).valid()) return;
            }

            const originalText = button.innerHTML;
            button.disabled = true;

            // ✅ Get loading text or fallback
            const text = button.dataset.loadingText || button.innerText.trim();

            // ✅ Detect spinner color from button
            let spinnerClass = button.dataset.spinner || "";

            if (button.classList.contains("btn-light") ||
                button.classList.contains("btn-warning")) {
                spinnerClass = "text-dark";
            }
            else if (button.classList.contains("color-accent"))
            {
                spinnerClass = "color-accent"
            }
            else {
                spinnerClass = "text-white";
            }

            // ✅ Apply loading state (inherits button styling)
            button.innerHTML = `
                <span 
                    class="spinner-border spinner-xs ${spinnerClass} me-2"
                    role="status">
                </span>
                ${text}
            `;
        });

    })

});
