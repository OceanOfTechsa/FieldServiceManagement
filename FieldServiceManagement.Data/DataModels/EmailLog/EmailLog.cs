using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.EmailLog;

[Table("EmailLog")]
public class EmailLog : BaseIntPrimaryKey
{
    public int? FormsSubmissionAuditId { get; set; }

    [MaxLength(500)]
    public string MessageId { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    public string? ProtocolLog { get; set; }

    public bool ReadReceipt { get; set; } = false;
    public DateTime? DateAccessed { get; set; }

    [MaxLength(1000)]
    public string? UserAgent { get; set; }
}