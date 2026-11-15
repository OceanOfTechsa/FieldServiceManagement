(function () {
    "use strict";

    const F = window.ImportFields;

    const CONTACT_MATCH = [["id", "Contact Id"], ["email", "Email"], ["name", "Contact Name"]];

    window.IMPORT_CONFIG = {
        primary: "contacts",                                // the main file: the one that must be uploaded first
        importUrl: "/Customers/Contacts/Import",
        historyUrl: "/ImportHistory?module=Contacts",

        // "Skip / find existing based on" option -> the field that must be mapped for it to work
        existingMatchFields: { id: "id", email: "email" },

        modules: {
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
                    { key: "company", label: "Company", required: true, matchBy: F.IdName, defaultMatch: "name", aliases: ["company", "companyname", "companyid", "organisation", "organization"] },
                    { key: "isActive", label: "Active", aliases: ["active", "isactive", "status"] },
                    ...F.addressFields("Service Address", "service", true),
                    ...F.addressFields("Billing Address", "billing", false)
                ]
            },

            addresses: F.addressesModule({
                uploader: () => addressesUploader,
                parentKey: "contact",
                parentLabel: "Contact",
                parentMatch: CONTACT_MATCH,
                parentAliases: ["contactid", "contact", "contactemail", "contactname", "email", "emailaddress"]
            }),

            notes: F.notesModule({
                uploader: () => notesUploader,
                parentLabel: "Parent (Contact)",
                parentMatch: CONTACT_MATCH,
                parentAliases: ["parentid", "parent", "contactid", "contact", "contactemail", "contactname", "email", "emailaddress"]
            })
        }
    };
})();