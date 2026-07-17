using FieldServiceManagement.Data.DataModels.BaseClass;

namespace FieldServiceManagement.ViewModels.Shared
{
    public class ActionUserViewModel : BaseGuidPrimaryKeyViewModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string AvatarUrl { get; set; } = string.Empty;
    }
}
