namespace FieldServiceManagement.ViewModels.Crew
{
    public class CrewMemberViewModel
    {
        public Guid MembershipId { get; set; }
        public bool IsLead { get; set; }
        public DateTime JoinedAt { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = null!;
        public string? Surname { get; set; }
        public string Email { get; set; } = null!;
        public string? AvatarUrl { get; set; }
        public bool UserIsActive { get; set; }
    }
}
