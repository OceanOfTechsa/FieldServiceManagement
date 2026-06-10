using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.Country
{
    [Table("Countries")]
    public class Country : BaseIntPrimaryKey
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
    }
}