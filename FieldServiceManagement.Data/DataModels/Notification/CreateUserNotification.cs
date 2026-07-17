namespace FieldServiceManagement.Data.DataModels.Notification
{
    public class CreateUserNotification
    {
        public Guid? OrganisationId { get; set; }
        public List<int> RoleIds { get; set; } = new();
        public string Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string RelatedEntityType { get; set; }
        public Guid? RelatedEntityId { get; set; }
        public byte Severity { get; set; } = 0;
        public string ActionText { get; set; }
        public string ActionUrl { get; set; }
        public Guid? CreatedById { get; set; }
        public string CreatedByEmail { get; set; }
    }
}
