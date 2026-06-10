using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.EmailQueue;

[Table("EmailQueue")]
public class EmailQueue : BaseIntPrimaryKey
{
    public int RecipientCount { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? SentDate { get; set; }
    public byte[]? Attachment { get; set; }

    [MaxLength(2000)] public string Recipient { get; set; }
    [MaxLength(500)] public string Subject { get; set; }
    [MaxLength(2000)] public string? Cc { get; set; }
    [MaxLength(2000)] public string? Bcc { get; set; }
    [MaxLength(2000)] public string? AttachmentFiles { get; set; }
    [MaxLength(500)] public string? Filename { get; set; }
    [MaxLength(50)] public string Status { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    public string Body { get; set; }
}