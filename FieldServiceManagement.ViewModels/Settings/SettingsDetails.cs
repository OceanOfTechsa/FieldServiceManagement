namespace FieldServiceManagement.ViewModels.Settings
{
    public class SettingsDetails
    {
        public string SearchTerm { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int RecordCount { get; set; }
        public List<SettingsDetailsForDisplay> settingsDetails { get; set; }
    }
}
