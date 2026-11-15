namespace FieldServiceManagement.ViewModels.DataImport
{
    public class ImportStartResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public Guid? ImportId { get; set; }
    }
}