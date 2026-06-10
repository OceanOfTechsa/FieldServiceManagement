using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.Form
{
    [Table("Forms")]
    public class Form : BaseIntPrimaryKey
    {
        public string FormName { get; set; }
        public int Version { get; set; }
        public bool IsCurrent { get; set; }
    }
}