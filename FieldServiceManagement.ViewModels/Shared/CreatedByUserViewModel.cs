
namespace FieldServiceManagement.ViewModels.Shared
{

    /// <summary>
    /// Slim projection of the user who created another record.
    /// Reused wherever a "created-by" reference is needed.
    /// </summary>
    public class CreatedByUserViewModel
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string FullName => $"{Name} {Surname}".Trim();
        public string Email { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
    }
}
