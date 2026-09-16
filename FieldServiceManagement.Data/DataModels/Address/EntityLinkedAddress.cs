using FieldServiceManagement.Data.DataModels.BaseClass;
using Microsoft.EntityFrameworkCore;

namespace FieldServiceManagement.Data.DataModels.Address
{
    [Keyless]
    public class EntityLinkedAddress : BaseGuidPrimaryKey
    {
        // Address fields
        public Guid OrganisationId { get; set; }
        public string Street { get; set; } = string.Empty;
        public string? City { get; set; }
        public int? StateId { get; set; }
        public int? CountryId { get; set; }
        public string? ZipCode { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }                 // Address.IsDeleted
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid CreatedById { get; set; }
        public Guid? UpdatedById { get; set; }

        // EntityAddress (link) fields
        public Guid EntityAddressId { get; set; }
        public int AddressTypeId { get; set; }
        public string AddressTypeName { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public DateTime LinkedAt { get; set; }
        public bool IsEntityAddressDeleted { get; set; }    // ← ea.IsDeleted
    }
}