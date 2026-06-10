using FieldServiceManagement.Data.DataModels.BaseClass;

namespace FieldServiceManagement.Data.DataModels.Company
{
    public class Company : BaseGuidPrimaryKey
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Mobile { get; set; }
        public string? Website { get; set; }
        public int Type { get; set; }
        public Guid ServiceAddressId { get; set; }
        public Guid BillingAddressId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public Guid OrganisationId { get; set; }
    }
}
