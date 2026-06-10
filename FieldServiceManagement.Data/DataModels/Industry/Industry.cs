using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.Industry
{
    [Table("Industries")]
    public class Industry : BaseIntPrimaryKey
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}