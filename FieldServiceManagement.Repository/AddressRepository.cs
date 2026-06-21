using FieldServiceManagement.Data;
using FieldServiceManagement.Data.DataModels.Address;
using FieldServiceManagement.Data.DataModels.Contact;
using FieldServiceManagement.Data.DataModels.Language;
using FieldServiceManagement.Data.RepositoryServices;
using FieldServiceManagement.Data.RepositoryServices.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FieldServiceManagement.Repository
{
    public class AddressRepository
    {
        private DataContext _dbContext;
        private readonly IRepository<Address> _repository;
        private bool _disposed = false;

        public AddressRepository()
        {
            _dbContext = DataContext.Create();
            _repository = new RepositoryService<Address>(_dbContext);
        }

        public List<Address> GetAllAddressesByOrganisaId(Guid OrganisationId)
        {
            return [.. _repository.Find(x => x.OrganisationId == OrganisationId & x.IsActive == true & x.IsDeleted == false)];
        }

        public List<Address> GetAddressesBySearchTerm(string SearchTerm, string Email)
        {
            SqlParameter[] parameters = [new("@SearchTerm", SearchTerm),new("@Email", Email)];
            const string query = "EXEC [dbo].[GetAddressesBySearchTerm] @SearchTerm, @Email";
            return [.. _dbContext.Set<Address>().FromSqlRaw(query, parameters)];
        }

        public async Task<List<AddressListItem>> GetAddressesByUserEmailAsync(string email)
        {
            var param = new SqlParameter("@Email", email);
            return await _dbContext.Database.SqlQueryRaw<AddressListItem>("EXEC [dbo].[GetAddressesByUserEmail] @Email", param).ToListAsync();
        }
        public Task<Address> GetAddressByIdAsync(Guid Id)
        {
            var entity = _repository.Find(x => x.Id == Id).FirstOrDefault();
            return Task.FromResult(entity);
        }

        public async Task<(Guid Id, bool Success)> CreateAddress(CreateAddress model)
        {
            var pNewId = new SqlParameter("@NewId", SqlDbType.UniqueIdentifier)
            {
                Direction = ParameterDirection.Output
            };

            var pSuccess = new SqlParameter("@Success", SqlDbType.Bit)
            {
                Direction = ParameterDirection.Output
            };

            object[] parameters =
            [
                new SqlParameter("@Street",         SqlDbType.NVarChar) { Value = model.Street },
                new SqlParameter("@City",           SqlDbType.NVarChar) { Value = model.City },
                new SqlParameter("@ZipCode",        SqlDbType.NVarChar) { Value = model.ZipCode },
                new SqlParameter("@CountryId",      SqlDbType.Int)      { Value = (object?)model.CountryId  ?? DBNull.Value },
                new SqlParameter("@StateId",        SqlDbType.Int)      { Value = (object?)model.StateId    ?? DBNull.Value },
                new SqlParameter("@Latitude",       SqlDbType.Decimal)  { Value = (object?)model.Latitude   ?? DBNull.Value, Precision = 9, Scale = 6 },
                new SqlParameter("@Longitude",      SqlDbType.Decimal)  { Value = (object?)model.Longitude  ?? DBNull.Value, Precision = 9, Scale = 6 },
                new SqlParameter("@CreatedById",    SqlDbType.UniqueIdentifier) { Value = model.CreatedById },
                new SqlParameter("@OrganisationId", SqlDbType.UniqueIdentifier) { Value = model.OrganisationId },
                new SqlParameter("@IpAddress",      SqlDbType.NVarChar) { Value = (object?)model.IpAddress  ?? DBNull.Value },
                pNewId,
                pSuccess,
            ];

            await _dbContext.Database.ExecuteSqlRawAsync(@" EXEC [CreateAddress] @Street, @City, @ZipCode, @CountryId, @StateId, @Latitude, @Longitude, @CreatedById, @OrganisationId, @IpAddress, @NewId OUTPUT, @Success OUTPUT",parameters);
            return (Id: (Guid)pNewId.Value, Success: (bool)pSuccess.Value);
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

        ~AddressRepository() => Dispose(false);
        #endregion
    }
}
