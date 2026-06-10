namespace FieldServiceManagement.ViewModels.EmailErrorLogViewModel;

public class EmailErrorLogViewModel
{
    public int Id { get; set; }
    public DateTime EmailDate { get; set; }
    public string ProtocolLog { get; set; }
    public string ResentProtocolLog { get; set; }
    public string Body { get; set; }
    public string Subject { get; set; }
    public string SendTo { get; set; }
    public string SendCC { get; set; }
    public string SendBCC { get; set; }
    public string FileName { get; set; }
    public byte[] Attachment { get; set; }
    public string AttachmentFiles { get; set; }
    public bool SuccessfullySent { get; set; }
}
