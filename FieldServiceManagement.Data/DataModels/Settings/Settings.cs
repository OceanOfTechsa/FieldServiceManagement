using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.Settings
{
    [Table("Settings")]
    public class Settings: BaseIntPrimaryKey
    {
        public string key { get; set; }

        public string value { get; set; }

        public string description { get; set; }

        public bool isActive { get; set; }
    }
}
