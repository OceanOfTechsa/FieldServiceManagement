namespace FieldServiceManagement.ViewModels.Audit
{
    public static class AuditEntity
    {
        public const string Company = "Company";
        public const string Organisation = "Organisation";
        public const string User = "User";
        public const string Address = "Address";
        // add more as needed
    }

    public static class AuditAction
    {
        public const string Created = "Created";
        public const string Updated = "Updated";
        public const string Deleted = "Deleted";
        public const string Restored = "Restored";
        public const string Viewed = "Viewed";
    }
}
