using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.Language
{
    [Table("Languages")]
    public class Language : BaseIntPrimaryKey
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
    }
}