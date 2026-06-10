namespace FieldServiceManagement.ViewModels.Settings
{
    public class SettingsDetailsForDisplay
    {
        public long RowNumber { get; set; }
        public int id { get; set; }
        public string key { get; set; }
        public string value { get; set; }
        public string description { get; set; }
        public bool isActive { get; set; }
        public int RecordCount { get; set; }
    }
}
