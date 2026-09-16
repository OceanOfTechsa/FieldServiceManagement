using FieldServiceManagement.Data.DataModels.BaseClass;

namespace FieldServiceManagement.ViewModels.Address
{
    public class AddressViewModel : BaseGuidPrimaryKeyViewModel
    {
        public Guid OrganisationId { get; set; }
        public string Street { get; set; } = string.Empty;
        public string? City { get; set; }
        public int? StateId { get; set; }
        public int? CountryId { get; set; }
        public string? ZipCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid CreatedById { get; set; }
        public Guid? UpdatedById { get; set; }

        // New fields coming from the procedure
        public int? UsageCount { get; set; }
        public string? UsedBy { get; set; }

        // Optional nice display property (can also be done in the view)
        public string DisplayText
        {
            get
            {
                // Base address
                var parts = new List<string> { Street };

                if (!string.IsNullOrWhiteSpace(City))
                    parts.Add(City);

                if (!string.IsNullOrWhiteSpace(ZipCode))
                    parts.Add(ZipCode);

                var text = string.Join(", ", parts);

                // Usage information
                if (UsageCount > 0 && !string.IsNullOrWhiteSpace(UsedBy))
                {
                    text += $"  •  {UsedBy}";
                }
                else if (UsageCount > 0)
                {
                    text += $"  •  used by {UsageCount} other record{(UsageCount > 1 ? "s" : "")}";
                }

                return text;
            }
        }
    }
}