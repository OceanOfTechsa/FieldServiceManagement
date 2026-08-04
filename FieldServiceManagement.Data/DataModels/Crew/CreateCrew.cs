namespace FieldServiceManagement.Data.DataModels.Crew
{
    public class CreateCrew
    {
        public string Name { get; set; }
        public int CrewSize { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid CreatedById { get; set; }
        public Guid OrganisationId { get; set; }
    }
}
