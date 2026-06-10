// FieldServiceManagement.Data/ApplicationRole.cs
using Microsoft.AspNetCore.Identity;

namespace FieldServiceManagement.Data
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }

        public ApplicationRole(string roleName) : base(roleName) { }
    }
}