using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.Currency
{
    [Table("Currencies")]
    public class Currency : BaseIntPrimaryKey
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Symbol { get; set; }
        public bool IsActive { get; set; }
    }
}