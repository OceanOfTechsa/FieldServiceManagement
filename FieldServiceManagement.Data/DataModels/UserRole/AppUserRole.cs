using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.UserRole
{
    [Table("UserRoles")]
    public class AppUserRole : BaseIntPrimaryKey
    {
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
    }
}