using FieldServiceManagement.Business.Configuration;
using FieldServiceManagement.Business.Infrastructure;
using FieldServiceManagement.ViewModels.EmailLogViewModel;
using FieldServiceManagement.ViewModels.ResponseViewModel;
using Microsoft.Extensions.DependencyInjection;
using Resend;
using System.Net.Mail;

namespace FieldServiceManagement.Business.EmailBusiness;

public abstract class EmailNotification
{
    protected string _subject; protected string _email;
    protected Attachment _attachment; protected byte[] _memoryAttachment;
    protected string _cc; protected string _bcc;
    protected string _attachments; protected string _fileName;
    protected List<EmailLogViewModel> _emailLogs;

    protected EmailNotification(string subject, string email)
    { _subject = subject; _email = email; _fileName = string.Empty; }

    protected EmailNotification(string subject, string email, Attachment attachment)
    { _subject = subject; _email = email; _attachment = attachment; _fileName = string.Empty; }

    protected EmailNotification(string subject, string email, string cc, string attachments)
    { _subject = subject; _email = email; _cc = cc; _attachments = attachments; _fileName = string.Empty; }

    protected EmailNotification(byte[] memoryAttachment, string fileName, string subject, string email, string cc)
    { _subject = subject; _email = email; _memoryAttachment = memoryAttachment; _fileName = fileName; _cc = cc; }

    protected EmailNotification(string subject, string email, string cc, string bcc, string fileName)
    { _subject = subject; _email = email; _cc = cc; _bcc = bcc; _fileName = fileName; }

    protected EmailNotification(string subject, string email, string cc, string bcc, string fileName, byte[] memoryAttachment)
    { _subject = subject; _email = email; _cc = cc; _bcc = bcc; _fileName = fileName; _memoryAttachment = memoryAttachment; }

    protected EmailNotification(string subject, string email, List<EmailLogViewModel> emailLogs)
    { _subject = subject; _email = email; _emailLogs = emailLogs; _fileName = string.Empty; }

    protected EmailNotification(string subject, string email, List<EmailLogViewModel> emailLogs, string fileName, byte[] memoryAttachment)
    { _subject = subject; _email = email; _emailLogs = emailLogs; _fileName = fileName; _memoryAttachment = memoryAttachment; }

    protected virtual string GetTemplate()
    {
        using var reader = new StreamReader(AppSettings.EmailTemplatePathFSM);
        return reader.ReadToEnd();
    }

    // Routes through queue (preferred path)
    public ResponseViewModel SendNotification()
    {
        PopulateSendNotification();
        return EmailQueueBusiness.EmailQueueBusiness.AddEmailToQueue(
            _email, _subject, MergeEmailTemplate(),
            _cc, _bcc, _memoryAttachment, _attachments, _fileName);
    }

    // Bypasses queue — sends immediately
    public ResponseViewModel SendNotificationWithoutQueue()
    {
        using var scope = ServiceProviderAccessor.Services.CreateScope();
        var resend = scope.ServiceProvider.GetRequiredService<IResend>();

        var notifier = new EmailNotifier(resend)
        {
            To = _email,
            Subject = _subject,
            Body = MergeEmailTemplate(),
            Attachment = _attachment,
            cc = _cc,
            bcc = _bcc,
            attachments = _attachments,
            memoryAttachment = _memoryAttachment,
            fileName = _fileName,
            emailLogs = _emailLogs
        };

        return notifier.SendEmail();
    }

    protected abstract void PopulateSendNotification();
    protected abstract string MergeEmailTemplate();
}