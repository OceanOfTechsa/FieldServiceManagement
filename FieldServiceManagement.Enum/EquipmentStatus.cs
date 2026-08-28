using System.ComponentModel.DataAnnotations;

namespace FieldServiceManagement.Enum
{
    public enum EquipmentStatus
    {
        [Display(Name = "Active")]
        Active = 29,

        [Display(Name = "InActive")]
        EquipementInActive = 30,

        [Display(Name = "OutOfService")]
        EquipementOutOfService = 31,

        [Display(Name = "UnderMaintenance")]
        UnderMaintenance = 32
    }
}
