using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.Asset
{
    [Table("Assets")]
    public class Asset : BaseGuidPrimaryKey
    {
        public Guid OrganisationId { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public DateTime? WarrantyExpiry { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid CreatedById { get; set; }
        public Guid? UpdatedById { get; set; }
    }
}