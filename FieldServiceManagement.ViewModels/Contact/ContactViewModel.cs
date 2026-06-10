using FieldServiceManagement.Data.DataModels.BaseClass;

namespace FieldServiceManagement.ViewModels.Contact
{
    public class ContactViewModel : BaseGuidPrimaryKeyViewModel
    {
        public int Salutation { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Surname { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Mobile { get; set; } = string.Empty;

        public Guid CompanyId { get; set; }

        public Guid? ServiceAddressId { get; set; }

        public Guid? BillingAddressId { get; set; }

        public Guid OrganisationId { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid UpdatedBy { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
