using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.Crew
{
    [Table("CrewMembers")]
    public class CrewMember : BaseGuidPrimaryKey
    {
        public Guid CrewId { get; set; }
        public Guid UserId { get; set; }
        public Guid OrganisationId { get; set; }
        public bool IsLead { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime? RemovedAt { get; set; }
        public bool IsActive { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
