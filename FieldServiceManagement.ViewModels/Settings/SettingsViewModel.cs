using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations;

namespace FieldServiceManagement.ViewModels.Settings
{
    public class SettingsViewModel: BaseIntPrimaryKeyViewModel
    {
        [Required]
        public string key { get; set; }
        [Required]
        public string value { get; set; }
        [Required]
        public string? description { get; set; }

        public bool isActive { get; set; }
    }
}
