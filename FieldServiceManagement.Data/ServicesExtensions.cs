// FieldServiceManagement.Data/ServicesExtensions.cs
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;

namespace FieldServiceManagement.Data
{
    public static class ServicesExtensions
    {
        public static string FSMConnectionString { get; set; }
        public static void FSMConnectionStringService(this IServiceCollection services, string connectionString)
        {
            FSMConnectionString = connectionString;

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString)
            {
                ColumnEncryptionSetting = SqlConnectionColumnEncryptionSetting.Enabled
            };
            FSMConnectionString = builder.ConnectionString;
        }
    }
}