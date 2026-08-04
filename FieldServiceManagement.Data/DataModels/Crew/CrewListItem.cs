using FieldServiceManagement.Data.DataModels.BaseClass;

namespace FieldServiceManagement.Data.DataModels.Crew
{
    public class CrewListItem : BaseGuidPrimaryKey
    {
        public string Name { get; set; }

        public int CrewSize { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid OrganisationId { get; set; }

        public string? CreatedByName { get; set; }

        public string? CreatedByAvatar { get; set; }

        public Guid? CreatedById { get; set; }
    }
}