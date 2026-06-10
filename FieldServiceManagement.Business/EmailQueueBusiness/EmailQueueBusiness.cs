using ElmahCore;
using FieldServiceManagement.Business.Configuration;
using FieldServiceManagement.Business.EmailBusiness;
using FieldServiceManagement.Business.Infrastructure;
using FieldServiceManagement.Data.DataModels.EmailQueue;
using FieldServiceManagement.Enum;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.ViewModels.Extensions;
using FieldServiceManagement.ViewModels.ResponseViewModel;
using Microsoft.Extensions.DependencyInjection;
using Resend;

namespace FieldServiceManagement.Business.EmailQueueBusiness;

public static class EmailQueueBusiness
{
    private static int _cachedTotal = -1;
    private static DateTime _cacheDate = DateTime.MinValue;
    private static readonly object _lock = new();

    public static ResponseViewModel AddEmailToQueue(
        string recipients, string subject, string body,
        string cc = null, string bcc = null, byte[] attachment = null,
        string attachmentFiles = null, string filename = null)
    {
        int count = recipients.Split([','], StringSplitOptions.RemoveEmptyEntries).Length;
        if (!string.IsNullOrWhiteSpace(cc))
            count += cc.Split([','], StringSplitOptions.RemoveEmptyEntries).Length;
        if (!string.IsNullOrWhiteSpace(bcc))
            count += bcc.Split([','], StringSplitOptions.RemoveEmptyEntries).Length;

        using var repo = new EmailQueueRepository();
        repo.Insert(new EmailQueue
        {
            Recipient = recipients,
            RecipientCount = count,
            Subject = subject,
            Body = body,
            Cc = cc,
            Bcc = bcc,
            Attachment = attachment,
            AttachmentFiles = attachmentFiles,
            Filename = filename,
            CreatedDate = DateTime.Now.SaDateTime(),
            Status = EmailQueueEnum.Pending.ToString()
        });
        return new ResponseViewModel { success = true };
    }

    public static void ProcessEmailQueue()
    {
        var today = DateTime.Today.SaDateTime();
        RefreshCacheIfNeeded(today);
        if (_cachedTotal >= AppSettings.DailyEmailSendingLimit) return;

        using var repo = new EmailQueueRepository();
        var pending = repo
            .Find(e => e.Status == EmailQueueEnum.Pending.ToString()
                    || e.Status == EmailQueueEnum.Queued.ToString())
            .OrderBy(e => e.RecipientCount).ThenBy(e => e.CreatedDate).ToList();

        foreach (var email in pending)
        {
            RefreshCacheIfNeeded(today);
            if (_cachedTotal >= AppSettings.DailyEmailSendingLimit) break;
            if (_cachedTotal + email.RecipientCount > AppSettings.DailyEmailSendingLimit)
            { LogSkipped(email); continue; }
            if (string.IsNullOrWhiteSpace(email.Recipient)) continue;
            using var scope = ServiceProviderAccessor.Services.CreateScope();
            var resend = scope.ServiceProvider.GetRequiredService<IResend>();
            var notifier = new EmailNotifier(resend)
            {
                To = email.Recipient,
                Subject = email.Subject,
                Body = email.Body,
                cc = email.Cc,
                bcc = email.Bcc,
                memoryAttachment = email.Attachment,
                fileName = email.Filename,
                attachments = email.AttachmentFiles
            };
            var result = notifier.SendEmail();
            email.Status = result.success ? EmailQueueEnum.Sent.ToString()
                                            : EmailQueueEnum.Failed.ToString();
            email.SentDate = DateTime.Now;
            repo.Update(email);
            if (result.success) UpdateCache(email.RecipientCount);
        }
    }

    public static (bool thresholdReached, int totalRecipients) HaveWeReachedEmailThresholdForTheDay()
    {
        var today = DateTime.Today.SaDateTime();
        RefreshCacheIfNeeded(today);
        using var repo = new EmailQueueRepository();
        int pending = repo.Find(e => e.Status == EmailQueueEnum.Pending.ToString())
            .AsEnumerable().Where(e => e.CreatedDate >= today).Sum(e => e.RecipientCount);
        int total = _cachedTotal + pending;
        return (total >= AppSettings.DailyEmailSendingLimit, total);
    }

    private static void RefreshCacheIfNeeded(DateTime today)
    {
        lock (_lock)
        {
            if (_cacheDate == today.SaDateTime()) return;
            using var repo = new EmailQueueRepository();
            _cachedTotal = repo.Find(e => e.SentDate >= today
                               && e.Status == EmailQueueEnum.Sent.ToString())
                              .Sum(e => e.RecipientCount);
            _cacheDate = today;
        }
    }

    private static void UpdateCache(int count)
    { lock (_lock) { _cachedTotal += count; } }

    private static void LogSkipped(EmailQueue email) =>
        ElmahExtensions.RaiseError(new Exception(
            $"Skipping email ID {email.Id}. Count: {_cachedTotal}/{AppSettings.DailyEmailSendingLimit}."));
}