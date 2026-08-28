using System.ComponentModel.DataAnnotations;

namespace FieldServiceManagement.Enum
{
    public enum EquipmentType
    {
        [Display(Name = "PowerTool")]
        PowerTool = 1,

        [Display(Name = "Vehicle")]
        Vehicle =2
    }
}
