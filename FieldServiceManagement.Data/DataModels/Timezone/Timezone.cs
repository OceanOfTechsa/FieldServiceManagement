using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.Timezone
{
    [Table("Timezones")]
    public class Timezone : BaseIntPrimaryKey
    {
        public string Name { get; set; }
        public string UtcOffset { get; set; }
        public bool IsActive { get; set; }
    }
}