using FieldServiceManagement.Data.DataModels.BaseClass;

namespace FieldServiceManagement.ViewModels.Contact
{
    public class ContactListItemViewModel : BaseGuidPrimaryKeyViewModel
    {
        public int Salutation { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public string? Phone { get; set; }

        public string? Mobile { get; set; }

        public Guid CompanyId { get; set; }

        public string? CompanyName { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? CreatedByName { get; set; }

        public string? CreatedByAvatar { get; set; }
        public Guid? CreatedById { get; set; }
    }
}
