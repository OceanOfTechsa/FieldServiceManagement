using FieldServiceManagement.Business.Configuration;
using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Data.DataModels.Settings;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.ViewModels.Settings;

namespace FieldServiceManagement.Business.SettingsBusiness
{
    public class SettingsBusiness
    {
        public List<SettingsViewModel> GetAllSettings()
        {
            using (var repo = new SettingsRepository())
            {
                var result = repo.GetAll();
                return ObjectMapper.Mapper.Map<List<SettingsViewModel>>(result);
            }
        }

        public async Task<SettingsDetails> GetSearchByList(string searchName, int pageIndex)
        {
            using var repo = new SettingsRepository();
            return await repo.GetSearchByList(searchName, pageIndex, AppSettings.staffPageSize);
        }


        public void AddNewSettings(SettingsViewModel model)
        {
            using (var repo = new SettingsRepository())
            {
                var entity = ObjectMapper.Mapper.Map<Settings>(model);
                repo.Insert(entity);
            }
        }

        public void EditSettings(SettingsViewModel model)
        {
            using (var repo = new SettingsRepository())
            {
                var form = ObjectMapper.Mapper.Map<Settings>(model);
                repo.Update(form);
            }
        }

        public SettingsViewModel GetSettingsById(int id)
        {
            using (var repo = new SettingsRepository())
            {
                var entity = repo.GetById(id);
                return ObjectMapper.Mapper.Map<SettingsViewModel>(entity);
            }
        }

        public SettingsViewModel GetSettingsByKey(string key)
        {
            using (var repo = new SettingsRepository())
            {
                var entity = repo.GetByKey(key);
                if (entity == null)
                {
                    return null;
                }
                return !entity.isActive ? null : ObjectMapper.Mapper.Map<SettingsViewModel>(entity);
            }
        }

        public string GetMOUContactPersonEmails()
        {
            var list = GetSettingByKey("MOUContactPerson");
            if (list == null || list.Count == 0) return string.Empty;

            var emails = list.Select(x => x.value)
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Distinct(StringComparer.OrdinalIgnoreCase);
            return string.Join(",", emails);
        }

        public string GetAssetEmails()
        {
            var list = GetSettingByKey("AssetEmail");
            if (list == null || list.Count == 0) return string.Empty;

            var emails = list.Select(x => x.value)
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Distinct(StringComparer.OrdinalIgnoreCase);
            return string.Join(",", emails);
        }

        public List<SettingsViewModel> GetSettingByKey(string key)
        {
            using (var repo = new SettingsRepository())
            {
                var entity = repo.GetSettingByKey(key);
                if (entity == null)
                {
                    return null;
                }
                return ObjectMapper.Mapper.Map<List<SettingsViewModel>>(entity);
            }
        }

        public List<SettingsViewModel> GetGrantsAssistantEmailOnSettings()
        {
            using (var repo = new SettingsRepository())
            {
                var entity = repo.GetGrantsAssistantEmailOnSettings();
                return ObjectMapper.Mapper.Map<List<SettingsViewModel>>(entity);
            }
        }

    }
}
