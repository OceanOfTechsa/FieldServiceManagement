namespace FieldServiceManagement.ViewModels.EmailLogViewModel;

public class EmailLogViewModel
{
    public int Id { get; set; }
    public int? FormsSubmissionAuditId { get; set; }
    public string MessageId { get; set; }
    public string ProtocolLog { get; set; }
    public bool ReadReceipt { get; set; }
    public DateTime? DateAccessed { get; set; }
    public string UserAgent { get; set; }
}