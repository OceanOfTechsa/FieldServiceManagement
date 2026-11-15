(function () {
    "use strict";

    const F = window.ImportFields;

    // How an Addresses or Notes row finds its company
    const COMPANY_MATCH = [["id", "Company Id"], ["name", "Company Name"]];

    window.IMPORT_CONFIG = {
        primary: "companies",
        importUrl: "/Customers/Companies/Import",
        historyUrl: "/ImportHistory?module=Companies",

        // "Skip / find existing based on" option -> the field that must be mapped for it to work
        existingMatchFields: { id: "id", name: "name" },
        // A new company needs these fields mapped (both address ids and the type are not nullable on Company)
        createRequiredFields: ["type", "serviceStreet", "serviceCity", "serviceZip", "billingStreet", "billingCity", "billingZip"],

        modules: {
            companies: {
                label: "Companies",
                charset: "utf-8",
                uploader: () => companiesUploader,
                fields: [
                    { key: "id", label: "Company Id", aliases: ["companyid", "id"] },
                    // Name, Email and Phone are not nullable on Company
                    { key: "name", label: "Company Name", required: true, aliases: ["companyname", "name", "company", "organisation", "organization"] },
                    { key: "email", label: "Email", required: true, aliases: ["email", "emailaddress", "mail"] },
                    { key: "phone", label: "Phone", required: true, aliases: ["phone", "phonenumber", "telephone", "tel", "workphone"] },
                    { key: "mobile", label: "Mobile", aliases: ["mobile", "mobilenumber", "mobilephone", "cell", "cellphone"] },
                    { key: "website", label: "Website", aliases: ["website", "websiteurl", "web", "url"] },
                    { key: "type", label: "Company Type", aliases: ["companytype", "type"] },
                    { key: "isActive", label: "Active", aliases: ["active", "isactive", "status"] },
                    ...F.addressFields("Service Address", "service", true),
                    ...F.addressFields("Billing Address", "billing", false)
                ]
            },

            addresses: F.addressesModule({
                uploader: () => addressesUploader,
                parentKey: "company",
                parentLabel: "Company",
                parentMatch: COMPANY_MATCH,
                parentAliases: ["companyid", "company", "companyname", "parentid", "parent"]
            }),

            notes: F.notesModule({
                uploader: () => notesUploader,
                parentLabel: "Parent (Company)",
                parentMatch: COMPANY_MATCH,
                parentAliases: ["parentid", "parent", "companyid", "company", "companyname"]
            })
        }
    };
})();