namespace FieldServiceManagement.Data.DataModels.Crew
{
    public class CrewMemberResults
    {
        public Guid MembershipId { get; set; }      // non-nullable Guid
        public bool IsLead { get; set; }
        public DateTime JoinedAt { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = null!;    // non-nullable string (null-forgiving default)
        public string? Surname { get; set; }
        public string Email { get; set; } = null!;
        public string? AvatarUrl { get; set; }
        public bool UserIsActive { get; set; }
    }
}
