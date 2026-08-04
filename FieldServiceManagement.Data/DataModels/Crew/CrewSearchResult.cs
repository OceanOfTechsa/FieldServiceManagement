using FieldServiceManagement.Data.DataModels.BaseClass;

namespace FieldServiceManagement.Data.DataModels.Crew
{
    public class CrewSearchResult : BaseGuidPrimaryKey
    {
        public string Name { get; set; } = null!;
        public int? CrewSize { get; set; }
    }
}
