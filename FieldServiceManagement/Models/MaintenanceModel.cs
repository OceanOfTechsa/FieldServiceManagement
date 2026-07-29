namespace FieldServiceManagement.Models
{
    public class MaintenanceModel
    {
        public bool isSystemMaintenance { get; set; } = false;
        public string? MaintenanceEstimatedTime { get; set; } = "30-60 minutes";
        public  string? MaintenanceMessage { get; set; } = null;
    }
}
