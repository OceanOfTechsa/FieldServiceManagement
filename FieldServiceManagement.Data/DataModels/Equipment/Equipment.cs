using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Models
{
    [Table("Equipments")]
    public class Equipment : BaseGuidPrimaryKey
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int? StatusId { get; set; }

        public int? Type { get; set; }

        [MaxLength(200)]
        public string? SerialNumber { get; set; }

        public int? WarranyStatus { get; set; }

        public DateTime? WarrantyExpiryDate { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }

        [Required]
        public Guid OrganisationId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public Guid CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Guid? UpdatedBy { get; set; }

        [Required]
        public Guid OwnerId { get; set; }
    }
}