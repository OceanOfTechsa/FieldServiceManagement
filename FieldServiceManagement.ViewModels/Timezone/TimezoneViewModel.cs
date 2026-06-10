namespace FieldServiceManagement.ViewModels.Timezone
{
    public class TimezoneViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UtcOffset { get; set; }
        public bool IsActive { get; set; }
    }
}
