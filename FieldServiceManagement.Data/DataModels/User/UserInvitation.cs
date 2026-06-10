using FieldServiceManagement.Data.DataModels.BaseClass;
using FieldServiceManagement.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.User
{
    [Table("UserInvitations")]
    public class UserInvitation: BaseIntPrimaryKey
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public int UserType { get; set; }
        public Guid OrganisationId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(10);
    }
}