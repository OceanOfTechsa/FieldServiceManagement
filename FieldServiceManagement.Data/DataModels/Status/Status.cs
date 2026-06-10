using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.Status
{
    [Table("Statuses")]
    public class Status : BaseIntPrimaryKey
    {
        public string StatusName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}