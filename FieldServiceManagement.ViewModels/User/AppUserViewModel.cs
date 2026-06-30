using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel;

namespace FieldServiceManagement.ViewModels.User
{
    public class AppUserViewModel : BaseGuidPrimaryKeyViewModel
    {
        public Guid OrganisationId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? AvatarUrl { get; set; }
        [DisplayName("User Role")]
        public int? UserRoleId { get; set; }
        [DisplayName("Language")]
        public int? PreferredLanguageId { get; set; }
        [DisplayName("IsActice?")]
        public bool IsActive { get; set; }
        [DisplayName("Status")]
        public int? StatusId { get; set; }
        public bool IsOwner { get; set; }
        public Guid UserId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid CreatedById { get; set; }
        public Guid? UpdatedById { get; set; }

        [DisplayName("Employee Number")]
        public string? EmployeeNumber { get; set; }
        public bool? TwoFactorEnabled { get; set; }
    }
}
