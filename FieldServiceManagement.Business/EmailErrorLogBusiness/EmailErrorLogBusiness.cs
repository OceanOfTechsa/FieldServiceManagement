using FieldServiceManagement.Business.EmailBusiness;
using FieldServiceManagement.Business.Infrastructure;
using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.ViewModels.EmailErrorLogViewModel;
using Microsoft.Extensions.DependencyInjection;
using Resend;
using System.Text;

namespace FieldServiceManagement.Business.EmailErrorLogBusiness;

public class EmailErrorLogBusiness
{
    public List<EmailErrorLogViewModel> GetActiveEmailErrors()
    {
        using var repo = new EmailErrorLogRepository();
        return ObjectMapper.Mapper.Map<List<EmailErrorLogViewModel>>(
            repo.Find(x => x.SuccessfullySent == false).ToList());
    }

    public List<EmailErrorLogViewModel> GetAllEmailErrors()
    {
        using var repo = new EmailErrorLogRepository();
        return ObjectMapper.Mapper.Map<List<EmailErrorLogViewModel>>(
            repo.GetAll().OrderByDescending(x => x.EmailDate));
    }

    public void ResendEmails() =>
        GetActiveEmailErrors().ForEach(e => MapAndSend(e));

    public bool ResendEmailById(int id)
    {
        var email = GetEmailErrorsById(id);
        return !email.SuccessfullySent && MapAndSend(email);
    }

    public bool DeleteEmailError(int id)
    {
        using var repo = new EmailErrorLogRepository();
        var record = repo.GetById(id); if (record is null) return false;
        repo.Delete(record); return true;
    }

    public EmailErrorLogViewModel GetEmailErrorsById(int id)
    {
        using var repo = new EmailErrorLogRepository();
        return ObjectMapper.Mapper.Map<EmailErrorLogViewModel>(repo.GetById(id));
    }

    public MemoryStream DownloadProtocolLogStream(int id, bool resent)
    {
        var log = resent ? GetEmailErrorsById(id).ResentProtocolLog
                         : GetEmailErrorsById(id).ProtocolLog;
        return new MemoryStream(Encoding.UTF8.GetBytes(log ?? string.Empty));
    }

    private static bool MapAndSend(EmailErrorLogViewModel email)
    {
        using var scope = ServiceProviderAccessor.Services.CreateScope();
        var resend = scope.ServiceProvider.GetRequiredService<IResend>();
        var notifier = new EmailNotifier(resend)
        {
            To = email.SendTo,
            cc = email.SendCC,
            bcc = email.SendBCC,
            Subject = email.Subject,
            Body = email.Body,
            fileName = email.FileName,
            attachments = email.AttachmentFiles ?? string.Empty,
        };
        if (email.Attachment?.Length > 0)
            notifier.memoryAttachment = email.Attachment;
        return notifier.ResendEmail(email.Id).success;
    }
}