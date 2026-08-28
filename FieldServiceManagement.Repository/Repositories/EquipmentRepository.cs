using FieldServiceManagement.Data;
using FieldServiceManagement.Data.DataModels.Company;
using FieldServiceManagement.Data.DataModels.Equipment;
using FieldServiceManagement.Data.DataModels.Shared;
using FieldServiceManagement.Data.RepositoryServices;
using FieldServiceManagement.Data.RepositoryServices.Contracts;
using FieldServiceManagement.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FieldServiceManagement.Repository.Repositories
{
    public class EquipmentRepository
    {
        private DataContext _dbContext;
        private readonly IRepository<Equipment> _repository;
        private bool _disposed = false;

        public EquipmentRepository()
        {
            _dbContext = DataContext.Create();
            _repository = new RepositoryService<Equipment>(_dbContext);
        }

        public async Task<List<EquipmentListItem>> GetEquipmentsByOrganisationIdAsync(Guid OrgId)
        {
            var param = new SqlParameter("@OrganisationId", OrgId);
            return await _dbContext.Database.SqlQueryRaw<EquipmentListItem>("EXEC [GetEquipmentsByOrganisationId] @OrganisationId", param).ToListAsync();
        }

        public async Task<Guid?> CreateEquipmentAsync(CreateEquipment Model)
        {
            var parameters = new[]
            {
                new SqlParameter("@Name", Model.Name),
                new SqlParameter("@Description", (object?)Model.Description ?? DBNull.Value),
                new SqlParameter("@StatusId", (object?)Model.StatusId ?? DBNull.Value),
                new SqlParameter("@Type", (object?)Model.Type ?? DBNull.Value),
                new SqlParameter("@SerialNumber", (object?)Model.SerialNumber ?? DBNull.Value),
                new SqlParameter("@ModelNumber", (object?)Model.ModelNumber ?? DBNull.Value),
                new SqlParameter("@WarrantyStatus", (object?)Model.WarrantyStatus ?? DBNull.Value),
                new SqlParameter("@WarrantyExpiryDate", (object?)Model.WarrantyExpiryDate ?? DBNull.Value),
                new SqlParameter("@PurchaseDate", (object?)Model.PurchaseDate ?? DBNull.Value),
                new SqlParameter("@OrganisationId", Model.OrganisationId),
                new SqlParameter("@CreatedByEmail", Model.CreatedByEmail),
            };

            var query = @"EXEC [dbo].[CreateEquipment] @Name, @Description, @StatusId, @Type, @SerialNumber, @ModelNumber, @WarrantyStatus, @WarrantyExpiryDate, @PurchaseDate, @OrganisationId, @CreatedByEmail";

            var results = await _dbContext.Database.SqlQueryRaw<RepoResults>(query, parameters).ToListAsync();
            var row = results.FirstOrDefault();
            if (row is null || row.Success != true)
                throw new InvalidOperationException(
                    $"Equipment Create failed: {row?.ErrorMessage} (line {row?.ErrorLine} in {row?.ErrorProcedure})");
            return row.Id;
        }

        public async Task<List<EquipmentDetails>> GetEquipmentFullDetailsByIdAsync(Guid Id)
        {
            var param = new SqlParameter("@Id", Id);
            return await _dbContext.Database.SqlQueryRaw<EquipmentDetails>("EXEC [dbo].[GetEquipmentById] @Id", param).ToListAsync();
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

        ~EquipmentRepository() => Dispose(false);
        #endregion
    }
}
