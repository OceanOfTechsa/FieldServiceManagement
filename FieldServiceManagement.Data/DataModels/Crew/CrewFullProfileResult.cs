using FieldServiceManagement.ViewModels.Crew;

namespace FieldServiceManagement.Data.DataModels.Crew
{
    public class CrewFullProfileResult
    {
        public List<CrewDetails> CrewRows { get; set; } = new();
        public List<CrewMemberResults> Members { get; set; } = new();
    }
}
