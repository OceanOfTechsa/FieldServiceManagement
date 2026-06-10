using FieldServiceManagement.Data.DataModels.BaseClass;

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
        public int? UserRoleId { get; set; }
        public int? PreferredLanguageId { get; set; }
        public bool IsActive { get; set; }
        public int? StatusId { get; set; }
        public bool IsOwner { get; set; }
        public Guid UserId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid CreatedById { get; set; }
        public Guid? UpdatedById { get; set; }

        public string? EmployeeNumber { get; set; }
    }
}
