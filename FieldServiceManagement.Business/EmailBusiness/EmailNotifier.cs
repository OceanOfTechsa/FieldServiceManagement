using ElmahCore;
using FieldServiceManagement.Business.Configuration;
using FieldServiceManagement.Business.Infrastructure;
using FieldServiceManagement.Data.DataModels.EmailErrorLog;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.ViewModels.EmailLogViewModel;
using FieldServiceManagement.ViewModels.Extensions;
using FieldServiceManagement.ViewModels.ResponseViewModel;
using Resend;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace FieldServiceManagement.Business.EmailBusiness;

public class EmailNotifier
{
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public string Error { get; set; }
    public Attachment Attachment { get; set; }
    public string fileName { get; set; }
    public string cc { get; set; }
    public string bcc { get; set; }
    public string attachments { get; set; }
    public byte[] memoryAttachment { get; set; }
    public List<EmailLogViewModel> emailLogs { get; set; }

    private readonly IResend _resend;

    public EmailNotifier(IResend resend)
    {
        _resend = resend ?? throw new ArgumentNullException(nameof(resend));
    }

    public ResponseViewModel SendEmail()
    {
        var response = new ResponseViewModel { success = true };
        try { SendCoreAsync().GetAwaiter().GetResult(); }
        catch (Exception ex) { LogResendError(response, ex, null); }
        return response;
    }

    public ResponseViewModel ResendEmail(int errorLogId)
    {
        var response = new ResponseViewModel { success = true };
        try
        {
            SendCoreAsync().GetAwaiter().GetResult();
            MarkErrorLogAsSent(errorLogId);
        }
        catch (Exception ex)
        {
            var log = ex.ToString();
            LogResendError(response, ex, log);
            UpdateResentProtocolLog(errorLogId, log);
        }
        return response;
    }

    private async Task SendCoreAsync()
    {
        var result = await _resend.EmailSendAsync(BuildEmailMessage());
        if (emailLogs != null) SaveMessageId(result.Content.ToString());
    }

    // BuildEmailMessage() — validate From before assigning
    private EmailMessage BuildEmailMessage()
    {
        var from = AppSettings.GetFSMFromEmail();
        if (string.IsNullOrWhiteSpace(from))
            throw new InvalidOperationException("Configured From email (GetFSMFromEmail) is missing or empty.");

        var msg = new EmailMessage
        {
            From = from,
            Subject = Subject,
            HtmlBody = Body,
            TextBody = StripTagsRegex(Body),
        };

        if (!string.IsNullOrEmpty(To)) msg.To = EmailAddressList.From(Split(To));
        if (!string.IsNullOrEmpty(cc)) msg.Cc = EmailAddressList.From(Split(cc));
        if (!string.IsNullOrEmpty(bcc)) msg.Bcc = EmailAddressList.From(Split(bcc));

        if (!string.IsNullOrEmpty(attachments))
        {
            msg.Attachments ??= new List<EmailAttachment>();
            foreach (var path in attachments.Split(',').Select(p => p.Trim()).Where(p => p.Length > 0))
                msg.Attachments.Add(new EmailAttachment
                { Filename = Path.GetFileName(path), Content = Convert.ToBase64String(File.ReadAllBytes(path)) });
        }

        if (memoryAttachment != null && !string.IsNullOrEmpty(fileName))
        {
            msg.Attachments ??= new List<EmailAttachment>();
            msg.Attachments.Add(new EmailAttachment
            { Filename = fileName, Content = Convert.ToBase64String(memoryAttachment) });
        }

        return msg;
    }

    //private void AddFormViewerIfEmailIsSentToApprover()
    //{
    //    var addresses = To.Split(',').Select(e => e.Trim()).ToList();
    //    var biz = new StaffLinkedToApproverBusiness.StaffLinkedToApproverBusiness();
    //    var toCC = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    //    foreach (var email in addresses)
    //    {
    //        var linked = biz.GetLinkedStaffEmailsByApproverEmail(email).GetAwaiter().GetResult();
    //        if (linked is not null) toCC.UnionWith(linked.Select(r => r.email));
    //    }
    //    if (!string.IsNullOrEmpty(cc)) toCC.ExceptWith(cc.Split(',').Select(e => e.Trim()));
    //    if (toCC.Any())
    //        cc = string.IsNullOrEmpty(cc) ? string.Join(",", toCC) : $"{cc},{string.Join(",", toCC)}";
    //}

    private void LogResendError(ResponseViewModel response, Exception ex, string resentLog)
    {
        response.success = false; response.errorMessage = ex.Message;
        ElmahExtensions.RaiseError(new Exception($"Resend email to: {To}", ex));
        using var repo = new EmailErrorLogRepository();
        repo.Insert(new EmailErrorLog
        {
            EmailDate = DateTime.Now.SaDateTime(),
            ProtocolLog = ex.ToString(),
            ResentProtocolLog = resentLog,
            Body = Body,
            Subject = Subject,
            SendTo = To,
            SendCC = cc,
            SendBCC = bcc,
            FileName = fileName,
            Attachment = memoryAttachment,
            AttachmentFiles = attachments,
            SuccessfullySent = false
        });
    }

    private static void MarkErrorLogAsSent(int id)
    {
        using var repo = new EmailErrorLogRepository();
        var r = repo.GetById(id); if (r is null) return;
        r.SuccessfullySent = true; repo.Update(r);
    }

    private static void UpdateResentProtocolLog(int id, string log)
    {
        using var repo = new EmailErrorLogRepository();
        var r = repo.GetById(id); if (r is null) return;
        r.ResentProtocolLog = log; repo.Update(r);
    }

    private void SaveMessageId(string messageId) =>
        emailLogs.ForEach(log => {
            log.ProtocolLog = string.Empty; log.MessageId = messageId;
            new EmailLogBusiness.EmailLogBusiness().Insert(log);
        });

    private static List<string> Split(string s) =>
        s.Split(',').Select(e => e.Trim()).Where(e => e.Length > 0).ToList();

    private static string StripTagsRegex(string s) =>
        Regex.Replace(s, "<.*?>", string.Empty);
}