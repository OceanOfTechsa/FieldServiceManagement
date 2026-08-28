using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FieldServiceManagement.Data.DataModels.Equipment
{
    public class CreateEquipment
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int IsActive { get; set; }

        [DisplayName("Status")]
        public int? StatusId { get; set; }

        public int? Type { get; set; }

        [DisplayName("Serial Number")]
        public string? SerialNumber { get; set; }

        [DisplayName("Model Number")]
        public string? ModelNumber { get; set; }

        [DisplayName("Warranty Status")]
        public int? WarrantyStatus { get; set; }

        [DisplayName("Warranty Expiry Date")]
        public DateTime? WarrantyExpiryDate { get; set; }

        [DisplayName("Purchase Date")]
        public DateTime? PurchaseDate { get; set; }
        public string CreatedByEmail { get; set; } = string.Empty;
        public Guid OrganisationId { get; set; }
    }
}
