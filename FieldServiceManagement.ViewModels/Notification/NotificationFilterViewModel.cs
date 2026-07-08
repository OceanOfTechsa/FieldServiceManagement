namespace FieldServiceManagement.ViewModels.Notification
{
    public class NotificationFilterViewModel
    {
        public bool? IsRead { get; set; }
        public byte? Severity { get; set; }
        public string Type { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string SearchTerm { get; set; }
    }
}
