using FieldServiceManagement.Data;
using FieldServiceManagement.Data.DataModels.Company;
using FieldServiceManagement.Data.DataModels.Shared;
using FieldServiceManagement.Data.RepositoryServices;
using FieldServiceManagement.Data.RepositoryServices.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FieldServiceManagement.Repository.Repositories
{
    public class CompanyRepository
    {
        private DataContext _dbContext;
        private readonly IRepository<Company> _repository;
        private bool _disposed = false;

        public CompanyRepository()
        {
            _dbContext = DataContext.Create();
            _repository = new RepositoryService<Company>(_dbContext);
        }

        public Company? GetById(Guid Id)
        {
            return _repository.Find(x => x.Id == Id)?.FirstOrDefault();
        }

        public async Task<Guid?> CreateCompanyAsync(Company Model, string? IpAddress = null)
        {
            var parameters = new[]
            {
                new SqlParameter("@Name",             Model.Name),
                new SqlParameter("@Email",            Model.Email),
                new SqlParameter("@Phone",            Model.Phone),
                new SqlParameter("@Mobile",           (object?)Model.Mobile        ?? DBNull.Value),
                new SqlParameter("@Website",          (object?)Model.Website       ?? DBNull.Value),
                new SqlParameter("@Type",             Model.Type),
                new SqlParameter("@ServiceAddressId", Model.ServiceAddressId),
                new SqlParameter("@BillingAddressId", Model.BillingAddressId),
                new SqlParameter("@CreatedBy",        Model.CreatedBy),
                new SqlParameter("@OrganisationId",   Model.OrganisationId),
                new SqlParameter("@IpAddress",        (object?)IpAddress             ?? DBNull.Value),
            };

            var query = @"EXEC [dbo].[CreateCompany] @Name, @Email, @Phone, @Mobile, @Website, @Type, @ServiceAddressId, @BillingAddressId, @CreatedBy, @OrganisationId, @IpAddress";
            var result = await _dbContext.Database.SqlQueryRaw<RepoResults>(query, parameters).ToListAsync();
            var row = result.FirstOrDefault();
            return row?.Success == true ? row.Id : null;
        }

        public async Task<List<CompanyResult>> GetCompaniesByUserEmailAsync(string email)
        {
            var param = new SqlParameter("@Email", email);
            return  await _dbContext.Database.SqlQueryRaw<CompanyResult>("EXEC [dbo].[GetCompaniesByUserEmail] @Email", param).ToListAsync();
        }

        public async Task<Guid?> UpdateCompanyAsync(Company Model, string? UpdatedByEmail, string? IpAddress = null)
        {
            var parameters = new[]
            {
                new SqlParameter("@Id", Model.Id),
                new SqlParameter("@Name", Model.Name),
                new SqlParameter("@Email", Model.Email),
                new SqlParameter("@Phone", Model.Phone),
                new SqlParameter("@Mobile", (object?)Model.Mobile ?? DBNull.Value),
                new SqlParameter("@Website", (object?)Model.Website ?? DBNull.Value),
                new SqlParameter("@Type", Model.Type),
                new SqlParameter("@ServiceAddressId", Model.ServiceAddressId),
                new SqlParameter("@BillingAddressId", Model.BillingAddressId),
                new SqlParameter("@UpdatedByEmail", UpdatedByEmail!),
                new SqlParameter("@IpAddress", (object?)IpAddress ?? DBNull.Value),
               new SqlParameter("@IsActive", Model.IsActive)
            };

            var query = @"EXEC [dbo].[UpdateCompany] @Id, @Name, @Email, @Phone, @Mobile, @Website, @Type, @ServiceAddressId, @BillingAddressId, @UpdatedByEmail, @IpAddress, @IsActive";
            var result = await _dbContext.Database.SqlQueryRaw<RepoResults>(query, parameters).ToListAsync();
            var row = result.FirstOrDefault();
            return row?.Success == true ? row.Id : null;
        }

        public async Task<RepoResults> LinkCompanyAddressesAsync(
          Guid companyId, Guid? serviceAddressId, Guid? billingAddressId, string userEmail)
        {
            var results = await _dbContext.Database.SqlQueryRaw<RepoResults>(
                "EXEC LinkCompanyAddresses @CompanyId, @ServiceAddressId, @BillingAddressId, @UserEmail",
                new SqlParameter("@CompanyId", companyId),
                new SqlParameter("@ServiceAddressId", (object?)serviceAddressId ?? DBNull.Value),
                new SqlParameter("@BillingAddressId", (object?)billingAddressId ?? DBNull.Value),
                new SqlParameter("@UserEmail", userEmail)
            ).ToListAsync();

            return results.FirstOrDefault()
                ?? new RepoResults { Success = false, ErrorMessage = "No result returned." };
        }

        public async Task<RepoResults> DeleteCompanyAsync(Guid companyId, string userEmail, string? ipAddress = null)
        {
            var results = await _dbContext.Database.SqlQueryRaw<RepoResults>(
                "EXEC DeleteCompany @CompanyId, @UserEmail, @IpAddress",
                new SqlParameter("@CompanyId", companyId),
                new SqlParameter("@UserEmail", userEmail),
                new SqlParameter("@IpAddress", (object?)ipAddress ?? DBNull.Value)
            ).ToListAsync();

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

        ~CompanyRepository() => Dispose(false);
        #endregion
    }
}
