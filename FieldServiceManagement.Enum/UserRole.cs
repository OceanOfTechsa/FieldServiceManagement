using System.ComponentModel.DataAnnotations;

namespace FieldServiceManagement.Enum
{
    public enum UserRole
    {
        [Display(Name = "Administrator")]
        Administrator = 1,

        [Display(Name = "Dispatcher")]
        Dispatcher = 2,

        [Display(Name = "FieldAgent")]
        FieldAgent = 3,

        [Display(Name = "CallCenterAgent")]
        CallCenterAgent = 4,

        [Display(Name = "LimitedFieldAgent")]
        LimitedFieldAgent = 5,

        [Display(Name = "SuperAdmin")]
        SuperAdmin = 6,

        [Display(Name = "CustomerPortalUser")]
        CustomerPortalUser = 7
    }
}
