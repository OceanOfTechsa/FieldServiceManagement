using FieldServiceManagement.ViewModels.Audit;
using FieldServiceManagement.ViewModels.User;

namespace FieldServiceManagement.ViewModels.Equipment
{
    public class EquipmentDetailsViewModel
    {
        public EquipmentViewModel? Equipment { get; set; }
        public AppUserProfileViewModel? EquipmentOwner { get; set; }
        public AppUserProfileViewModel? CreatedBy { get; set; }
        public AppUserProfileViewModel? UpdatedBy { get; set; }
        public List<AuditLogViewModel>? AuditLogs { get; set; }
    }
}
