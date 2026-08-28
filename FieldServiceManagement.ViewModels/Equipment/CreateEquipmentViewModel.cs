using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FieldServiceManagement.ViewModels.Equipment
{
    public class CreateEquipmentViewModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int IsActive { get; set; } = 1;

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

        public string CreatedByEmail { get; set; }
        public Guid OrganisationId { get; set; }
    }
}