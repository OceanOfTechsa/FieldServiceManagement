using Microsoft.AspNetCore.Identity;

namespace FieldServiceManagement.Data.DataModels.User
{
    public class ApplicationUser : IdentityUser
    {
        public Guid? OrganisationId { get; set; } // nullable initially — critical for existing rows
    }
}
