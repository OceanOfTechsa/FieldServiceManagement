using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.Industry
{
    [Table("IndustryCategories")]
    public class IndustryCategory : BaseIntPrimaryKey
    {
        public int IndustryId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}