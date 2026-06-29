using FieldServiceManagement.Data;
using FieldServiceManagement.Data.DataModels.Organisation;
using FieldServiceManagement.Data.RepositoryServices;
using FieldServiceManagement.Data.RepositoryServices.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FieldServiceManagement.Repository.Repositories
{
    public class OrganisationRepository
    {
        private DataContext _dbContext;
        private readonly IRepository<Organisation> _repository;
        private bool _disposed = false;

        public OrganisationRepository()
        {
            _dbContext = DataContext.Create();
            _repository = new RepositoryService<Organisation>(_dbContext);
        }

        public async void InsertAsync(Organisation model)
        {
            _repository.Insert(model);
        }

        public Organisation? GetById(Guid OrgId)
        {
            return _repository.Find(o => o.Id == OrgId)?.FirstOrDefault();
        }


        public async Task<OrganisationDetails?> GetOrganisationDetailsByIdAsync(Guid organisationId)
        {
            var param = new SqlParameter("@OrganisationId", organisationId);
            var query = "EXEC [dbo].[GetOrganisationById] @OrganisationId";
            return _dbContext.Database.SqlQueryRaw<OrganisationDetails>(query, param).AsEnumerable().FirstOrDefault();
        }

        #region DISPOSE
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext?.Dispose();
                }
                _disposed = true;
            }
        }

        ~OrganisationRepository() => Dispose(false);
        #endregion
    }
}
