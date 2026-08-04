using FieldServiceManagement.Data.DataModels.BaseClass;

namespace FieldServiceManagement.ViewModels.Crew
{
    public class CrewSearchResultViewModel : BaseGuidPrimaryKeyViewModel
    {
        public string Name { get; set; } = null!;
        public int? CrewSize { get; set; }
    }
}
