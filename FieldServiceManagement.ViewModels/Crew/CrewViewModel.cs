using FieldServiceManagement.Data.DataModels.BaseClass;

namespace FieldServiceManagement.ViewModels.Crew
{
    public class CrewViewModel : BaseGuidPrimaryKeyViewModel
    {
        public string Name { get; set; }
        public int CrewSize { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public Guid OrganisationId { get; set; }
    }
}
