using Microsoft.EntityFrameworkCore;

namespace FieldServiceManagement.Data.DataModels.Shared
{
    [Keyless]
    public class RepoResults
    {
        public Guid Id { get; set; }
        public bool Success { get; set; }
    }
}
