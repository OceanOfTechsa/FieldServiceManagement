using FieldServiceManagement.ViewModels.Audit;

namespace FieldServiceManagement.ViewModels.Crew
{
    public class CrewFullProfileViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public int? CrewSize { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public Guid OrganisationId { get; set; }

        public Guid? CB_Id { get; set; }
        public string? CB_Name { get; set; }
        public string? CB_Surname { get; set; }
        public string? CB_Email { get; set; }
        public string? CB_AvatarUrl { get; set; }
        public bool? CB_IsActive { get; set; }

        public Guid? UB_Id { get; set; }
        public string? UB_Name { get; set; }
        public string? UB_Surname { get; set; }
        public string? UB_Email { get; set; }
        public string? UB_AvatarUrl { get; set; }
        public bool? UB_IsActive { get; set; }

        public List<AuditLogViewModel> AuditLogs { get; set; } = new();
        public List<CrewMemberViewModel> Members { get; set; } = new();
    }
}
