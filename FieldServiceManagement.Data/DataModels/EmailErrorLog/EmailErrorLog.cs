using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.EmailErrorLog;

[Table("EmailErrorLog")]
public class EmailErrorLog: BaseIntPrimaryKey
{
    public DateTime EmailDate { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    public string ProtocolLog { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    public string? ResentProtocolLog { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    public string Body { get; set; }

    [MaxLength(500)] public string Subject { get; set; }
    [MaxLength(2000)] public string SendTo { get; set; }
    [MaxLength(2000)] public string? SendCC { get; set; }
    [MaxLength(2000)] public string? SendBCC { get; set; }
    [MaxLength(500)] public string? FileName { get; set; }
    [MaxLength(2000)] public string? AttachmentFiles { get; set; }

    public byte[]? Attachment { get; set; }
    public bool SuccessfullySent { get; set; } = false;
}