using System.ComponentModel.DataAnnotations;

namespace FieldServiceManagement.Enum
{
    public enum EquipmentWarrantyStatus
    {
        [Display(Name = "Active")]
        Active = 1,
        [Display(Name = "Expired")]
        Expired = 2
    }
}
