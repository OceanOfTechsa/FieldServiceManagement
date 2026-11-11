using FieldServiceManagement.Data;
using FieldServiceManagement.Data.DataModels.Company;
using FieldServiceManagement.Data.DataModels.Contact;
using FieldServiceManagement.Data.DataModels.Shared;
using FieldServiceManagement.Data.RepositoryServices;
using FieldServiceManagement.Data.RepositoryServices.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FieldServiceManagement.Repository.Repositories
{
    public class ContactRepository
    {
        private DataContext _dbContext;
        private readonly IRepository<Contact> _repository;
        private bool _disposed = false;

        public ContactRepository()
        {
            _dbContext = DataContext.Create();
            _repository = new RepositoryService<Contact>(_dbContext);
        }


        public Contact? GetById(Guid Id)
        {
            return _repository.Find(x => x.Id == Id)?.FirstOrDefault();
        }

        public async Task<Guid?> CreateContactAsync(Contact model, string? ipAddress = null)
        {
            var parameters = new[]
            {
                new SqlParameter("@Salutation",       model.Salutation),
                new SqlParameter("@Name",             model.Name),
                new SqlParameter("@Surname",          model.Surname),
                new SqlParameter("@Email",            model.Email),
                new SqlParameter("@Phone",            model.Phone),
                new SqlParameter("@Mobile",           (object?)model.Mobile ?? DBNull.Value),
                new SqlParameter("@CompanyId",        model.CompanyId),
                new SqlParameter("@ServiceAddressId", (object?)model.ServiceAddressId ?? DBNull.Value),
                new SqlParameter("@BillingAddressId", (object?)model.BillingAddressId ?? DBNull.Value),
                new SqlParameter("@CreatedBy",       model.CreatedBy),
                new SqlParameter("@OrganisationId",   model.OrganisationId),
                new SqlParameter("@IpAddress",       (object?)ipAddress ?? DBNull.Value),
            };

            var query = @"EXEC [dbo].[CreateContact] @Salutation, @Name, @Surname, @Email, @Phone, @Mobile, @CompanyId, @ServiceAddressId, @BillingAddressId, @CreatedBy, @OrganisationId, @IpAddress";
            var result = await _dbContext.Database.SqlQueryRaw<RepoResults>(query, parameters).ToListAsync();
            var row = result.FirstOrDefault();
            return row?.Success == true ? row.Id : null;
        }

        public async Task<List<ContactDetails>> GetContactFullDetailsByIdAsync(Guid Id)
        {
            var param = new SqlParameter("@Id", Id);
            return await _dbContext.Database.SqlQueryRaw<ContactDetails>("EXEC [dbo].[GetContactById] @Id", param).ToListAsync();
        }

        public async Task<List<ContactListItem>> GetContactsByUserEmailAsync(string email)
        {
            var param = new SqlParameter("@Email", email);
            return await _dbContext.Database.SqlQueryRaw<ContactListItem>("EXEC [dbo].[GetContactsByUserEmail] @Email", param).ToListAsync();
        }

        public async Task<List<Contact>> GetContactsByCompanyId(Guid companyId, Guid organisationId)
        {
            return _repository.Find(c => c.CompanyId == companyId && c.OrganisationId == organisationId).ToList();
        }
        
        public async Task<RepoResults> DeleteContactAsync(Guid contactId, string userEmail, string? ipAddress = null)
        {
            var results = await _dbContext.Database.SqlQueryRaw<RepoResults>(
                "EXEC DeleteContact @ContactId, @UserEmail, @IpAddress",
                new SqlParameter("@ContactId", contactId),
                new SqlParameter("@UserEmail", userEmail),
                new SqlParameter("@IpAddress", (object?)ipAddress ?? DBNull.Value)
            ).ToListAsync();
 
            return results.FirstOrDefault()
                   ?? new RepoResults { Success = false, ErrorMessage = "No result returned." };
        }
        
        public async Task<RepoResults> UpdateContactAsync(Contact model, string? updatedByEmail, string? ipAddress = null)
        {
            var parameters = new[]
            {
                new SqlParameter("@Id", model.Id),
                new SqlParameter("@Salutation", model.Salutation),
                new SqlParameter("@Name", model.Name),
                new SqlParameter("@Surname", model.Surname),
                new SqlParameter("@Email", model.Email),
                new SqlParameter("@Phone", model.Phone),
                new SqlParameter("@Mobile", (object?)model.Mobile ?? DBNull.Value),
                new SqlParameter("@CompanyId", model.CompanyId),
                new SqlParameter("@ServiceAddressId", (object?)model.ServiceAddressId ?? DBNull.Value),
                new SqlParameter("@BillingAddressId", (object?)model.BillingAddressId ?? DBNull.Value),
                new SqlParameter("@IsActive", model.IsActive),
                new SqlParameter("@UpdatedByEmail", updatedByEmail!),
                new SqlParameter("@IpAddress", (object?)ipAddress ?? DBNull.Value)
            };
 
            var query = @"EXEC [dbo].[UpdateContact]
                @Id, @Salutation, @Name, @Surname, @Email, @Phone, @Mobile, @CompanyId,
                @ServiceAddressId, @BillingAddressId, @IsActive, @UpdatedByEmail, @IpAddress";
 
            var results = await _dbContext.Database.SqlQueryRaw<RepoResults>(query, parameters).ToListAsync();
 
            return results.FirstOrDefault()
                   ?? new RepoResults { Success = false, ErrorMessage = "No result returned." };
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

        ~ContactRepository() => Dispose(false);
        #endregion
    }
}
