using FieldServiceManagement.Data.DataModels.Settings;
using System.Linq.Expressions;

namespace FieldServiceManagement.Repository.Contracts
{
    public interface ISettingsRepository : IDisposable
    {
        List<Settings> GetAll();

        void Update(Settings model);

        void Insert(Settings model);

        Settings GetById(int id);

        IEnumerable<Settings> Find(Expression<Func<Settings, bool>> predicate);
        Settings GetByKey(string key);
        List<Settings> GetSettingByKey(string key);

        List<Settings> GetGrantsAssistantEmailOnSettings();
    }
}
