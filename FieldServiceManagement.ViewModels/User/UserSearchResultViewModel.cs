using FieldServiceManagement.Data.DataModels.BaseClass;

namespace FieldServiceManagement.ViewModels.User
{
    public class UserSearchResultViewModel : BaseGuidPrimaryKeyViewModel
    {
        public string Name { get; set; } = null!;
        public string? Surname { get; set; }
        public string Email { get; set; } = null!;
        public string? AvatarUrl { get; set; }
    }
}
