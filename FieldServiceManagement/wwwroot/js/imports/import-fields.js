(function () {
    "use strict";

    // What the select under a lookup field offers: "does the value in the file identify the record by Id or by Name?"
    const IdName = [["id", "Id"], ["name", "Name"]];

    // One block of address fields: the Service or Billing address inside a module's own file
    function addressFields(section, prefix, isDefault) {
        return [
            { key: prefix + "Street", label: "Street", section, aliases: [prefix + "street", prefix + "street1", prefix + "address", prefix + "addressline1", ...(isDefault ? ["street", "street1", "address", "addressline1", "streetaddress"] : [])] },
            { key: prefix + "City", label: "City", section, aliases: [prefix + "city", ...(isDefault ? ["city", "town"] : [])] },
            { key: prefix + "State", label: "State", section, matchBy: IdName, defaultMatch: "name", aliases: [prefix + "state", prefix + "stateid", prefix + "province", ...(isDefault ? ["state", "stateid", "province"] : [])] },
            { key: prefix + "Zip", label: "Postal Code", section, aliases: [prefix + "zip", prefix + "zipcode", prefix + "postalcode", prefix + "postcode", ...(isDefault ? ["zip", "zipcode", "postalcode", "postcode"] : [])] },
            { key: prefix + "Country", label: "Country", section, matchBy: IdName, defaultMatch: "name", aliases: [prefix + "country", prefix + "countryid", ...(isDefault ? ["country", "countryid"] : [])] }
        ];
    }

    // The separate Addresses file of a module: one row per address, linked to its parent record
    //   o.parentKey      : the field key the server reads ("contact", "company", ...)
    //   o.parentLabel    : what the field is called on screen
    //   o.parentMatch    : the Id / Email / Name options for finding the parent
    //   o.parentAliases  : header names that auto-map to the parent
    function addressesModule(o) {
        return {
            label: "Addresses",
            charset: "iso-8859-1",
            uploader: o.uploader,
            fields: [
                { key: o.parentKey, label: o.parentLabel, required: true, matchBy: o.parentMatch, defaultMatch: o.parentDefaultMatch ?? "id", aliases: o.parentAliases },
                { key: "addressType", label: "Address Type", required: true, aliases: ["addresstype", "type"] },
                { key: "street", label: "Street", required: true, aliases: ["street", "street1", "address", "addressline1", "streetaddress"] },
                { key: "city", label: "City", required: true, aliases: ["city", "town"] },
                { key: "state", label: "State", matchBy: IdName, defaultMatch: "name", aliases: ["state", "stateid", "province"] },
                { key: "zip", label: "Postal Code", required: true, aliases: ["zip", "zipcode", "postalcode", "postcode"] },
                { key: "country", label: "Country", matchBy: IdName, defaultMatch: "name", aliases: ["country", "countryid"] }
            ]
        };
    }

    // The separate Notes file of a module: a note attached to a parent record (Contact, Company, Asset, ...)
    function notesModule(o) {
        return {
            label: "Notes",
            charset: "iso-8859-1",
            uploader: o.uploader,
            fields: [
                { key: "parent", label: o.parentLabel, required: true, matchBy: o.parentMatch, defaultMatch: o.parentDefaultMatch ?? "id", aliases: o.parentAliases },
                { key: "title", label: "Title", aliases: ["title", "notetitle", "subject"] },
                { key: "content", label: "Content", required: true, aliases: ["content", "notecontent", "note", "notes", "body", "text", "description"] }
            ]
        };
    }

    window.ImportFields = { IdName, addressFields, addressesModule, notesModule };
})();