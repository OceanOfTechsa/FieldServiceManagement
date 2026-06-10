using Microsoft.EntityFrameworkCore.Design;

namespace FieldServiceManagement.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            // Set connection string for migrations
            ServicesExtensions.FieldServiceManagementConnectionString =
                "Server=localhost\\SQLEXPRESS;Database=FieldServiceManagement;Trusted_Connection=True;Encrypt=False;";

            return new DataContext();
        }
    }
}