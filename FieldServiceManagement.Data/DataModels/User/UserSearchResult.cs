using FieldServiceManagement.Data.DataModels.BaseClass;

namespace FieldServiceManagement.Data.DataModels.User
{
    public class UserSearchResult : BaseGuidPrimaryKey
    {
        public string Name { get; set; } = null!;
        public string? Surname { get; set; }
        public string Email { get; set; } = null!;
        public string? AvatarUrl { get; set; }
    }
}
