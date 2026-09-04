using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FieldServiceManagement.ViewModels.Equipment
{
    public class EquipmentViewModel : BaseGuidPrimaryKeyViewModel
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [DisplayName("Status")]
        public int? StatusId { get; set; }

        public int? Type { get; set; }

        [DisplayName("Serial Number")]
        [MaxLength(200)]
        public string? SerialNumber { get; set; }

        [DisplayName("Warranty Status")]
        public int? WarrantyStatus { get; set; }

        [DisplayName("Warranty Expiry Date")]
        public DateTime? WarrantyExpiryDate { get; set; }

        [DisplayName("Purchase Date")]
        public DateTime? PurchaseDate { get; set; }

        public bool IsDeleted { get; set; }

        public int IsActive { get; set; }

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

        [DisplayName("Model Number")]
        public string? ModelNumber { get; set; }
    }


    public class DeleteEquipmentRequest
    {
        [Required]
        public Guid EquipmentId { get; set; }
    }

}