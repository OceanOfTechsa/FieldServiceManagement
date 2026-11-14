using FieldServiceManagement.Data.Extensions;
using FieldServiceManagement.ViewModels.DataImport;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Data.DataModels.DataImportHistory;

namespace FieldServiceManagement.Business
{
    public class DataImportBusiness
    {
        private const string AlreadyRunning =
            "An import is already in progress. Please wait for it to complete before starting another.";

        public async Task<string?> CheckCanStartAsync(Guid OrganisationId)
        {
            var repository = new DataImportRepository();
            await repository.FailStaleImportsAsync(OrganisationId, DateTime.Now.SaDateTime().AddHours(-2));

            var hasActiveImport = await repository.HasActiveImportAsync(OrganisationId);

            return hasActiveImport ? AlreadyRunning : null;
        }

        public async Task<string?> AddAsync(DataImportHistory Model)
        {
            var added = await new DataImportRepository().AddAsync(Model);
            return added ? null : AlreadyRunning;
        }

        public async Task<List<DataImportHistoryViewModel>> GetHistoryAsync(Guid OrganisationId,string? Module)
        {
            var dbModel = await new DataImportRepository().GetHistoryAsync(OrganisationId, Module);

            var model = ObjectMapper.Mapper.Map<List<DataImportHistoryViewModel>>(dbModel);

            await Task.WhenAll(model.Select(async item =>
            {
                item.CreatedBy = await new UserBusiness.UserBusiness().GetUserDetailsByUserNameAsync(
                    item.UserEmail);
            }));

            return model;
        }

        public async Task<string?> GetErrorFileAsync(Guid Id, string Email)
        {
            var user = await new UserBusiness.UserBusiness()
                .GetUserDetailsByUserNameAsync(Email);
            return await new DataImportRepository().GetErrorFileAsync(Id, user.OrganisationId);
        }
    }
}
