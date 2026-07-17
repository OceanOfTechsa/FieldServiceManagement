
using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.Announcement
{
    [Table("AnnouncementDeliveryTypes")]
    public class AnnouncementDeliveryType : BaseIntPrimaryKey
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}