namespace FieldServiceManagement.ViewModels.DataImport
{
    public class FileImportInfoViewModel
    {
        public string FileName { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;
        public int Records { get; set; }
    }
}
