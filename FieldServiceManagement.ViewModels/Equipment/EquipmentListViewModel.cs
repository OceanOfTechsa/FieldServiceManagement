using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations;

namespace FieldServiceManagement.ViewModels.Equipment
{
    public class EquipmentListViewModel : BaseGuidPrimaryKeyViewModel
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int? StatusId { get; set; }

        public int? Type { get; set; }

        [Display(Name = "Serial Number")]
        public string? SerialNumber { get; set; }

        [Display(Name = "Warranty Status")]
        public int? WarrantyStatus { get; set; }

        [Display(Name = "Warranty Expiry Date")]
        public DateTime? WarrantyExpiryDate { get; set; }

        [Display(Name = "Purchase Date")]
        public DateTime? PurchaseDate { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid OrganisationId { get; set; }

        public string? CreatedByName { get; set; }

        public string? CreatedByAvatar { get; set; }

        public Guid? CreatedById { get; set; }
    }
}
