using System.ComponentModel;

namespace FieldServiceManagement.ViewModels.Crew
{
    public class CreateCrewViewModel
    {
        public string Name { get; set; }
        [DisplayName("Size")]
        public int CrewSize { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
