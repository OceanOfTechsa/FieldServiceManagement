using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Data.DataModels.EmailLog;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.ViewModels.EmailLogViewModel;
using FieldServiceManagement.ViewModels.Extensions;

namespace FieldServiceManagement.Business.EmailLogBusiness;

public class EmailLogBusiness
{
    public EmailLogViewModel GetById(int id)
    {
        using var repo = new EmailLogRepository();
        return ObjectMapper.Mapper.Map<EmailLogViewModel>(repo.GetById(id));
    }

    public Task<int> UpdateEmailLogsAsync(int id, string userAgent)
    {
        var model = new EmailLogViewModel
        {
            FormsSubmissionAuditId = id,
            ReadReceipt = true,
            DateAccessed = DateTime.Now.SaDateTime(),
            UserAgent = userAgent
        };
        return UpdateEmailTracking(model);
    }

    public void Insert(EmailLogViewModel model)
    {
        using var repo = new EmailLogRepository();
        repo.Insert(ObjectMapper.Mapper.Map<EmailLog>(model));
    }

    public Task<int> UpdateEmailTracking(EmailLogViewModel model)
    {
        using var repo = new EmailLogRepository();
        return repo.UpdateEmailTracking(ObjectMapper.Mapper.Map<EmailLog>(model));
    }
}