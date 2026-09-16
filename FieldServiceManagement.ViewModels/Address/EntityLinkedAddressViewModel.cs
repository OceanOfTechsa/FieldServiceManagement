using FieldServiceManagement.Data.DataModels.BaseClass;

namespace FieldServiceManagement.ViewModels.Address
{
    public class EntityLinkedAddressViewModel : BaseGuidPrimaryKeyViewModel
    {
        public string Street { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? ZipCode { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public Guid EntityAddressId { get; set; }
        public int AddressTypeId { get; set; }
        public string AddressTypeName { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public DateTime LinkedAt { get; set; }

        public bool IsDeleted { get; set; }                  // Address
        public bool IsEntityAddressDeleted { get; set; }     // Link

        public string DisplayText
        {
            get
            {
                var text = Street;
                if (!string.IsNullOrWhiteSpace(City)) text += $", {City}";
                if (!string.IsNullOrWhiteSpace(ZipCode)) text += $" {ZipCode}";
                return text;
            }
        }
    }
}