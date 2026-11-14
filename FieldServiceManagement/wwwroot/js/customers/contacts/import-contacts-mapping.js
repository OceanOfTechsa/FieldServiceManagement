(function () {
    "use strict";

    // TODO: create this endpoint. It receives the files plus a mapping JSON per file.
    const IMPORT_URL = "/Customers/Contacts/Import";
    const IMPORT_HISTORY_URL = "/ImportHistory?module=Contacts";
    const SAMPLE_ROWS = 2;

    const $ = id => document.getElementById(id);
    const norm = s => String(s ?? "").toLowerCase().replace(/[^a-z0-9]/g, "");
    const esc = s => String(s ?? "").replace(/[&<>"']/g, c =>
        ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" }[c]));

    // ─────────────────────── Field definitions ───────────────────────
    // Taken from the data models:
    //   Contact  : Salutation, Name, Surname, Email, Phone, Mobile, CompanyId (not nullable),
    //              ServiceAddressId?, BillingAddressId?, IsActive
    //   Address  : Street, City, StateId?, CountryId?, ZipCode (the Street/City/ZipCode strings are not nullable)
    //   Note     : Title, Content, Parent (not built yet)
    //
    // required:     shows the red * and blocks Import until mapped
    // requiredWith: required only while another field is mapped
    // matchBy:      "Select" under the field name = what the file value identifies (sent as matchBy)
    // aliases:      normalised header names (lowercase, letters/digits only) used by auto-mapping

    const COMPANY_MATCH = [["id", "Id"], ["name", "Name"]];
    const LOOKUP_MATCH = [["id", "Id"], ["name", "Name"]];                     // State / Country (StateId, CountryId)
    const CONTACT_MATCH = [["id", "Contact Id"], ["email", "Email"], ["name", "Contact Name"]];

    // One block of address fields: used for Service/Billing in the contacts file
    const addressFields = (section, prefix, isDefault) => [
        { key: prefix + "Street", label: "Street", section, aliases: [prefix + "street", prefix + "street1", prefix + "address", prefix + "addressline1", ...(isDefault ? ["street", "street1", "address", "addressline1", "streetaddress"] : [])] },
        { key: prefix + "City", label: "City", section, aliases: [prefix + "city", ...(isDefault ? ["city", "town"] : [])] },
        { key: prefix + "State", label: "State", section, matchBy: LOOKUP_MATCH, defaultMatch: "name", aliases: [prefix + "state", prefix + "stateid", prefix + "province", ...(isDefault ? ["state", "stateid", "province"] : [])] },
        { key: prefix + "Zip", label: "Postal Code", section, aliases: [prefix + "zip", prefix + "zipcode", prefix + "postalcode", prefix + "postcode", ...(isDefault ? ["zip", "zipcode", "postalcode", "postcode"] : [])] },
        { key: prefix + "Country", label: "Country", section, matchBy: LOOKUP_MATCH, defaultMatch: "name", aliases: [prefix + "country", prefix + "countryid", ...(isDefault ? ["country", "countryid"] : [])] }
    ];

    const MODULES = {
        contacts: {
            label: "Contacts",
            charset: "utf-8",
            uploader: () => contactsUploader,
            fields: [
                { key: "id", label: "Contact Id", aliases: ["contactid", "id"] },
                { key: "salutation", label: "Salutation", aliases: ["salutation", "prefix", "title"] },
                { key: "name", label: "First Name", required: true, aliases: ["firstname", "first", "name", "givenname", "forename"] },
                { key: "surname", label: "Last Name", required: true, aliases: ["lastname", "last", "surname", "familyname"] },
                { key: "email", label: "Email", required: true, aliases: ["email", "emailaddress", "mail"] },
                { key: "phone", label: "Phone", aliases: ["phone", "phonenumber", "telephone", "tel", "workphone"] },
                { key: "mobile", label: "Mobile", aliases: ["mobile", "mobilenumber", "mobilephone", "cell", "cellphone"] },
                // CompanyId is not nullable on Contact, so every contact needs a company
                { key: "company", label: "Company", required: true, matchBy: COMPANY_MATCH, defaultMatch: "name", aliases: ["company", "companyname", "companyid", "organisation", "organization"] },
                { key: "isActive", label: "Active", aliases: ["active", "isactive", "status"] },
                ...addressFields("Service Address", "service", true),
                ...addressFields("Billing Address", "billing", false)
            ]
        },
        addresses: {
            label: "Addresses",
            charset: "iso-8859-1",
            uploader: () => addressesUploader,
            fields: [
                // Mandatory: links the address to a Contact
                { key: "contact", label: "Contact", required: true, matchBy: CONTACT_MATCH, defaultMatch: "id", aliases: ["contactid", "contact", "companyid", "contactemail", "contactname", "email", "emailaddress"] },
                // Service or Billing: needed to know which address slot to link
                { key: "addressType", label: "Address Type", required: true, aliases: ["addresstype", "type"] },
                { key: "street", label: "Street", required: true, aliases: ["street", "street1", "address", "addressline1", "streetaddress"] },
                { key: "city", label: "City", required: true, aliases: ["city", "town"] },
                { key: "state", label: "State", matchBy: LOOKUP_MATCH, defaultMatch: "name", aliases: ["state", "stateid", "province"] },
                { key: "zip", label: "Postal Code", required: true, aliases: ["zip", "zipcode", "postalcode", "postcode"] },
                { key: "country", label: "Country", matchBy: LOOKUP_MATCH, defaultMatch: "name", aliases: ["country", "countryid"] }
            ]
        },
        notes: {
            label: "Notes",
            charset: "iso-8859-1",
            uploader: () => notesUploader,
            fields: [
                // Parent = the contact the note belongs to
                { key: "parent", label: "Parent (Contact)", required: true, matchBy: CONTACT_MATCH, defaultMatch: "id", aliases: ["parentid", "parent", "contactid", "contact", "contactemail", "contactname", "email", "emailaddress"] },
                { key: "title", label: "Title", aliases: ["title", "notetitle", "subject"] },
                { key: "content", label: "Content", required: true, aliases: ["content", "notecontent", "note", "notes", "body", "text", "description"] }
            ]
        }
    };

    // key -> { signature, headers, normHeaders, rows, mapping, replaceEmpty, matchBy }
    const state = {};
    let active = "contacts";
    let filter = "all";

    // ─────────────────────── CSV parsing ───────────────────────

    function detectDelimiter(text) {
        const firstLine = text.split(/\r?\n/, 1)[0] || "";
        const counts = [",", ";", "\t"].map(d => [d, firstLine.split(d).length - 1]);
        counts.sort((a, b) => b[1] - a[1]);
        return counts[0][1] > 0 ? counts[0][0] : ",";
    }

    // Handles quoted fields, escaped quotes (""), delimiters and line breaks inside quotes, and a BOM
    function parseCsv(text, delimiter) {
        if (text.charCodeAt(0) === 0xFEFF) text = text.slice(1);

        const rows = [];
        let row = [], field = "", inQuotes = false;

        for (let i = 0; i < text.length; i++) {
            const c = text[i];

            if (inQuotes) {
                if (c === '"') {
                    if (text[i + 1] === '"') { field += '"'; i++; }
                    else inQuotes = false;
                } else field += c;
            } else if (c === '"') {
                inQuotes = true;
            } else if (c === delimiter) {
                row.push(field); field = "";
            } else if (c === "\n" || c === "\r") {
                if (c === "\r" && text[i + 1] === "\n") i++;
                row.push(field); field = "";
                rows.push(row); row = [];
            } else field += c;
        }

        if (field !== "" || row.length) { row.push(field); rows.push(row); }

        const result = [];
        let blanks = 0;
        for (const r of rows) {
            if (r.some(v => v.trim() !== "")) {
                if (blanks > 10) break;
                result.push(r);
                blanks = 0;
            } else blanks++;
        }
        return result;
    }

    async function loadModules() {
        for (const [key, mod] of Object.entries(MODULES)) {
            const file = mod.uploader()?.currentFile;
            if (!file) { delete state[key]; continue; }

            const charset = $("charset-" + key)?.value || mod.charset;
            const signature = `${file.name}|${file.size}|${file.lastModified}|${charset}`;
            if (state[key]?.signature === signature) continue;   // unchanged file and charset: keep the user's mapping

            const text = new TextDecoder(charset).decode(await file.arrayBuffer());
            const rows = parseCsv(text, detectDelimiter(text));

            if (rows.length < 2) {
                const err = new Error(`${mod.label}: ${rows.length === 0 ? "the file is empty." : "the file has a header row but no data rows."}`);
                err.key = key;   // so the error shows under the right upload card
                throw err;
            }

            if (rows.length - 1 > IMPORT_LIMITS.maxRecords) {
                const err = new Error(`${mod.label}: the file has ${(rows.length - 1).toLocaleString()} records, but your plan allows up to ${IMPORT_LIMITS.maxRecords.toLocaleString()} per file.`);
                err.key = key;
                throw err;
            }

            const headers = rows[0].map((h, i) => h.trim() || `Column ${i + 1}`);
            const dataRows = rows.slice(1);

            state[key] = {
                signature,
                charset,
                headers,
                normHeaders: headers.map(norm),
                rows: dataRows,
                // Zoho: a column with no value in any row is not listed (indexes stay the original CSV positions)
                empty: headers.map((_, i) => dataRows.every(r => (r[i] ?? "").trim() === "")),
                mapping: {},        // fieldKey -> column index
                replaceEmpty: {},   // fieldKey -> text used when the cell is empty
                matchBy: {}         // fieldKey -> how the cell value identifies the record (company, state, country, contact)
            };

            autoMap(key);
        }
    }

    // ─────────────────────── Mapping logic ───────────────────────

    function autoMap(key) {
        const st = state[key];
        const used = new Set(Object.values(st.mapping).filter(v => v != null));

        for (const field of MODULES[key].fields) {
            if (st.mapping[field.key] != null) continue;

            for (const alias of field.aliases) {
                const idx = st.normHeaders.findIndex((h, i) => h === alias && !used.has(i) && !st.empty[i]);
                if (idx > -1) {
                    st.mapping[field.key] = idx;
                    setDefaultMatch(st, field, idx);
                    used.add(idx);
                    break;
                }
            }
        }
    }

    // Picks the "match by" option from the header name (Contact Email -> email), else the field default
    function setDefaultMatch(st, field, idx) {
        if (!field.matchBy || st.matchBy[field.key]) return;

        const header = st.normHeaders[idx];
        const values = field.matchBy.map(o => o[0]);
        let pick = null;

        if (header.includes("email") && values.includes("email")) pick = "email";
        else if (header.endsWith("id") && values.includes("id")) pick = "id";
        else if (header.includes("name") && values.includes("name")) pick = "name";

        st.matchBy[field.key] = pick || field.defaultMatch || values[0];
    }

    function resetMapping(key) {
        const st = state[key];
        st.mapping = {};
        st.replaceEmpty = {};
        st.matchBy = {};
    }

    function isRequired(st, field) {
        return !!field.required || (!!field.requiredWith && st.mapping[field.requiredWith] != null);
    }

    function fieldStatus(st, field) {
        if (st.mapping[field.key] != null) return "mapped";
        return isRequired(st, field) ? "mandatory" : "unmapped";
    }

    function validate() {
        const problems = [];

        for (const key of Object.keys(state)) {
            const st = state[key];
            const mod = MODULES[key];

            const missing = mod.fields.filter(f => isRequired(st, f) && st.mapping[f.key] == null);
            if (missing.length)
                problems.push({ key, message: `Map the mandatory fields for ${mod.label}: ${missing.map(f => f.label).join(", ")}.` });

            const noMatch = mod.fields.filter(f => f.matchBy && st.mapping[f.key] != null && !st.matchBy[f.key]);
            if (noMatch.length)
                problems.push({ key, message: `${mod.label}: choose how to match ${noMatch.map(f => f.label).join(", ")} (the select under the field name).` });
        }

        const opts = typeof getImportData === "function" ? getImportData() : {};
        const matchBy = opts.importOption === "new" ? opts.skipExistingBasedOn : opts.findExistingBasedOn;
        if (state.contacts && matchBy === "id" && state.contacts.mapping.id == null)
            problems.push({ key: "contacts", message: "Contacts: map Contact Id, which is used to find existing contacts." });

        return problems;
    }

    // ─────────────────────── Rendering ───────────────────────

    function render() {
        const mod = MODULES[active];
        const st = state[active];
        if (!st) return;

        const usedBy = {};
        Object.entries(st.mapping).forEach(([k, i]) => { if (i != null) usedBy[i] = k; });

        let mapped = 0, mandatory = 0;
        let html = "";
        let lastSection;

        for (const field of mod.fields) {
            const status = fieldStatus(st, field);
            if (status === "mapped") mapped++;
            if (status === "mandatory") mandatory++;

            const visible =
                filter === "all" ||
                (filter === "mapped" && status === "mapped") ||
                (filter === "unmapped" && status !== "mapped") ||
                (filter === "mandatory" && status === "mandatory");
            if (!visible) continue;

            if (field.section && field.section !== lastSection)
                html += `<div class="fsm-map-section">${esc(field.section)}</div>`;
            lastSection = field.section;

            html += rowHtml(st, field, status, usedBy);
        }

        $("mapRows").innerHTML = html || `<div class="p-4 text-center text-muted small">No fields match this filter.</div>`;

        const total = mod.fields.length;
        $("mapStatTotal").textContent = total;
        $("mapStatMapped").textContent = mapped;
        $("mapStatUnmapped").textContent = total - mapped;
        $("mapStatMandatory").textContent = mandatory;
    }

    function rowHtml(st, field, status, usedBy) {
        const idx = st.mapping[field.key];
        const isMapped = idx != null;

        const icon = {
            mapped: "bi-check-circle-fill text-success",
            mandatory: "bi-exclamation-circle-fill text-danger",
            unmapped: "bi-circle text-muted"
        }[status];

        const options = [`<option value="">Select</option>`]
            .concat(st.headers.map((h, i) => {
                if (st.empty[i]) return "";   // no data in this column: not listed
                const takenElsewhere = usedBy[i] != null && usedBy[i] !== field.key;
                return `<option value="${i}" ${i === idx ? "selected" : ""} ${takenElsewhere ? "disabled" : ""}>${esc(h)}</option>`;
            }))
            .join("");

        const matchSelect = field.matchBy
            ? `<select class="form-select form-select-sm mt-1 w-auto ${isMapped && !st.matchBy[field.key] ? "is-invalid" : ""}"
                       style="font-size:.75rem;padding-top:.1rem;padding-bottom:.1rem;"
                       data-match-select="${field.key}" aria-label="${esc(field.label)} match by">
                   <option value="">Select</option>
                   ${field.matchBy.map(([value, label]) => `<option value="${value}" ${st.matchBy[field.key] === value ? "selected" : ""}>${esc(label)}</option>`).join("")}
               </select>`
            : "";

        const replaceInput = isMapped && !field.matchBy   // match-by fields (company, state, country, contact) have no replace-empty value
            ? `<input type="text" class="form-control form-control-sm" placeholder="Type here"
                      data-replace-input="${field.key}" value="${esc(st.replaceEmpty[field.key] ?? "")}" />`
            : "";

        const samples = Array.from({ length: SAMPLE_ROWS }, (_, n) => {
            const v = isMapped ? (st.rows[n]?.[idx] ?? "").trim() : "";
            return `<div class="fsm-map-cell fsm-map-sample" title="${esc(v)}">${v ? esc(v) : "--"}</div>`;
        }).join("");

        return `
            <div class="fsm-map-grid fsm-map-row" data-field="${field.key}">
                <div class="fsm-map-cell">
                    <div class="d-flex align-items-start gap-2">
                        <i class="bi ${icon} mt-1"></i>
                        <div>
                            <span>${esc(field.label)}</span>${isRequired(st, field) ? `<span class="text-danger">*</span>` : ""}
                            ${matchSelect ? `<div>${matchSelect}</div>` : ""}
                        </div>
                    </div>
                </div>
                <div class="fsm-map-cell">
                    <select class="form-select form-select-sm" data-map-select="${field.key}">${options}</select>
                </div>
                <div class="fsm-map-cell">${replaceInput}</div>
                ${samples}
            </div>`;
    }

    function buildModuleSelect() {
        const keys = Object.keys(MODULES).filter(k => state[k]);
        if (!keys.includes(active)) active = keys[0];

        $("mapModuleSelect").innerHTML = keys
            .map(k => `<option value="${k}" ${k === active ? "selected" : ""}>${MODULES[k].label}</option>`)
            .join("");
    }

    // ─────────────────────── Steps ───────────────────────

    function showStep(n) {
        $("uploadStep").classList.toggle("d-none", n !== 1);
        $("mapStep").classList.toggle("d-none", n !== 2);
        $("doneStep").classList.toggle("d-none", n !== 3);
        $("step1Actions").classList.toggle("d-none", n !== 1);
        $("step2Actions").classList.toggle("d-none", n !== 2);

        [$("stepUpload"), $("stepMap"), $("stepDone")].forEach((el, i) => {
            if (!el) return;
            const done = i < n - 1 || n === 3;       // earlier steps, and all steps on the last step
            el.classList.toggle("done", done);
            el.classList.toggle("active", i === n - 1 && n < 3);
        });
    }

    function showMapError(message) {
        $("mapErrorText").textContent = message;
        $("mapError").classList.remove("d-none");
    }

    function hideMapError() {
        $("mapError").classList.add("d-none");
    }

    async function handleNext() {
        const btn = $("nextBtn");
        btn.disabled = true;

        try {
            await loadModules();submitImport
            buildModuleSelect();
            hideMapError();
            render();
            showStep(2);
        } catch (err) {
            (MODULES[err.key]?.uploader() ?? contactsUploader).showError(err.message || "Couldn't read the file.");
        } finally {
            enableNextButton();
        }
    }

    // ─────────────────────── Import ───────────────────────

    function buildMapping(key) {
        const st = state[key];
        const fields = {};

        for (const field of MODULES[key].fields) {
            const idx = st.mapping[field.key];
            if (idx != null) {
                fields[field.key] = {
                    column: st.headers[idx],
                    index: idx,
                    replaceEmpty: st.replaceEmpty[field.key] || null,
                    matchBy: field.matchBy ? (st.matchBy[field.key] || null) : undefined
                };
            }
        }

        return fields;
    }

    async function submitImport() {
        const problems = validate();
        if (problems.length) {
            active = problems[0].key;
            buildModuleSelect();
            render();
            showMapError(problems[0].message);
            return;
        }
        hideMapError();

        const opts = typeof getImportData === "function" ? getImportData() : {};
        const importOption = opts.importOption || "new";

        const fd = new FormData();
        fd.append("importOption", importOption);
        fd.append("skipExistingBasedOn", opts.skipExistingBasedOn || "none");
        fd.append("findExistingBasedOn", opts.findExistingBasedOn || "");
        fd.append("dontUpdateEmpty", importOption === "new" ? "false" : String(opts.dontUpdateEmpty?.[importOption] ?? true));

        for (const key of Object.keys(state)) {
            fd.append(`${key}File`, MODULES[key].uploader().currentFile);
            fd.append(`${key}Mapping`, JSON.stringify(buildMapping(key)));
        }

        // Plain object for the console (FormData itself prints as empty)
        const payload = {
            url: IMPORT_URL,
            options: {
                importOption,
                skipExistingBasedOn: opts.skipExistingBasedOn || "none",
                findExistingBasedOn: opts.findExistingBasedOn || "",
                dontUpdateEmpty: importOption === "new" ? false : (opts.dontUpdateEmpty?.[importOption] ?? true)
            },
            files: {},
            mappings: {}
        };
        for (const key of Object.keys(state)) {
            const file = MODULES[key].uploader().currentFile;
            payload.files[key] = { name: file.name, size: file.size, type: file.type };
            payload.mappings[key] = buildMapping(key);

            for (const key of Object.keys(state)) {
                fd.append(`${key}File`, MODULES[key].uploader().currentFile);
                fd.append(`${key}Mapping`, JSON.stringify(buildMapping(key)));
                fd.append(`${key}Charset`, state[key].charset);
            }
        }
        console.log("Import payload:", payload);
        console.log("Import form data entries:", [...fd.entries()]);

        // Show the "initiated" screen straight away: the import itself runs in the background
        $("viewImportHistoryBtn").href = IMPORT_HISTORY_URL;
        showStep(3);

        // Sent as a form field as well as a header, so [ValidateAntiForgeryToken] passes with the default
        // antiforgery settings (a header alone only works if AddAntiforgery sets HeaderName).
        // Added after the console logs above so the token is not printed.
        const token = document.querySelector("#mapStep input[name='__RequestVerificationToken']")?.value;
        if (token) fd.append("__RequestVerificationToken", token);

        try {
            const response = await fetch(IMPORT_URL, {
                method: "POST",
                headers: token ? { "RequestVerificationToken": token } : {},
                body: fd
            });

            let result = null;
            try { result = await response.json(); } catch { /* non-JSON response */ }

            console.log("Import response:", response.status, result);

            if (!response.ok || result?.success === false)
                throw new Error(result?.message || "The import could not be started. Please try again.");
        } catch (err) {
            // The import did not start, so go back to the mapping with the reason
            console.error("Import failed to start:", err);
            showStep(2);
            showMapError(err.message || "Network error. Please try again.");
        }
    }

    // ─────────────────────── Events ───────────────────────

    function init() {
        if (!$("mapStep")) return;

        $("mapModuleSelect").addEventListener("change", e => { active = e.target.value; render(); });
        $("mapFilter").addEventListener("change", e => { filter = e.target.value; render(); });

        $("mapResetBtn").addEventListener("click", () => {
            if (!state[active]) return;
            resetMapping(active);
            hideMapError();
            render();
        });

        // Re-applies auto-mapping from scratch for the current file
        $("mapAutoBtn").addEventListener("click", () => {
            if (!state[active]) return;
            resetMapping(active);
            autoMap(active);
            hideMapError();
            render();
        });

        $("mapRows").addEventListener("change", e => {
            const st = state[active];

            const select = e.target.closest("[data-map-select]");
            if (select) {
                const key = select.dataset.mapSelect;
                const field = MODULES[active].fields.find(f => f.key === key);

                if (select.value === "") {
                    st.mapping[key] = null;
                    delete st.replaceEmpty[key];
                    delete st.matchBy[key];
                } else {
                    st.mapping[key] = Number(select.value);
                    setDefaultMatch(st, field, st.mapping[key]);
                }

                hideMapError();
                render();
                return;
            }

            const match = e.target.closest("[data-match-select]");
            if (match) {
                if (match.value === "") delete st.matchBy[match.dataset.matchSelect];
                else st.matchBy[match.dataset.matchSelect] = match.value;
                hideMapError();
                render();
            }
        });

        // No re-render while typing, so the input keeps focus
        $("mapRows").addEventListener("input", e => {
            const input = e.target.closest("[data-replace-input]");
            if (input) state[active].replaceEmpty[input.dataset.replaceInput] = input.value;
        });

        $("prevBtn").addEventListener("click", () => showStep(1));
        // Zoho: "Yes, Initiate Import" confirmation. Problems are shown straight away, the dialog only opens when valid.
        $("importBtn").addEventListener("click", () => {
            if (validate().length) { submitImport(); return; }
            $("fsm-confirm-import-dialog").classList.add("show");
        });
        $("fsm-confirm-import-cancel").addEventListener("click", () => $("fsm-confirm-import-dialog").classList.remove("show"));
        $("fsm-confirm-import-yes").addEventListener("click", () => {
            $("fsm-confirm-import-dialog").classList.remove("show");
            submitImport();
        });
    }

    // Replaces the placeholder handleNext in the page's inline script
    window.handleNext = handleNext;

    document.addEventListener("DOMContentLoaded", init);
})();