using FieldServiceManagement.Data.DataModels.Address;
using FieldServiceManagement.Data.DataModels.Company;
using FieldServiceManagement.Data.DataModels.Contact;
using FieldServiceManagement.Data.DataModels.Entity;
using FieldServiceManagement.Data.DataModels.Note;

namespace FieldServiceManagement.Repository.Repositories
{
    // One batch of changes from an import, saved together in one database transaction.
    // A module fills only the lists it uses.
    public class ImportBatch
    {
        public List<Contact> NewContacts { get; set; } = new();
        public List<Contact> UpdatedContacts { get; set; } = new();

        public List<Company> NewCompanies { get; set; } = new();
        public List<Company> UpdatedCompanies { get; set; } = new();

        public List<Address> NewAddresses { get; set; } = new();
        public List<Address> UpdatedAddresses { get; set; } = new();

        public List<EntityAddress> NewLinks { get; set; } = new();
        public List<EntityAddress> UpdatedLinks { get; set; } = new();

        public List<Note> NewNotes { get; set; } = new();
    }
}