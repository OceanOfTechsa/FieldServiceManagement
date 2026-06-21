document.querySelectorAll('input#cell, input.cell').forEach(function (input) {
    input.addEventListener('input', function (e) {
        let digits = input.value.replace(/\D/g, '');

        // Cap at 11 digits (international: 27 + 9) or 10 (local: 0 + 9)
        if (digits.length > 11) digits = digits.slice(0, 11);

        let formatted = '';

        if (digits.startsWith('27') && digits.length > 2) {
            // International format: (+27) 82 686 7925
            const rest = digits.slice(2); // strip country code
            if (rest.length <= 2) {
                formatted = `(+27) ${rest}`;
            } else if (rest.length <= 5) {
                formatted = `(+27) ${rest.slice(0, 2)} ${rest.slice(2)}`;
            } else if (rest.length <= 8) {
                formatted = `(+27) ${rest.slice(0, 2)} ${rest.slice(2, 5)} ${rest.slice(5)}`;
            } else {
                formatted = `(+27) ${rest.slice(0, 2)} ${rest.slice(2, 5)} ${rest.slice(5, 9)}`;
            }
        } else if (digits.startsWith('0') && digits.length > 1) {
            // Local format: (082) 686 7925
            const areaCode = digits.slice(0, 3);
            const rest = digits.slice(3);
            if (digits.length <= 3) {
                formatted = `(${areaCode}`;
            } else if (rest.length <= 3) {
                formatted = `(${areaCode}) ${rest}`;
            } else if (rest.length <= 6) {
                formatted = `(${areaCode}) ${rest.slice(0, 3)} ${rest.slice(3)}`;
            } else {
                formatted = `(${areaCode}) ${rest.slice(0, 3)} ${rest.slice(3, 7)}`;
            }
        } else {
            // Still typing — just show raw digits
            formatted = digits;
        }

        input.value = formatted;
    });

// On blur — clear if incomplete
input.addEventListener('blur', function () {
const digits = input.value.replace(/\D/g, '');
const validLocal         = digits.startsWith('0')  && digits.length === 10;
const validInternational = digits.startsWith('27') && digits.length === 11;

        if (!validLocal && !validInternational && digits.length > 0) {
    input.classList.add('is-invalid');
        } else {
    input.classList.remove('is-invalid');
        }
    });

input.addEventListener('focus', function () {
    input.classList.remove('is-invalid');
    });
});
