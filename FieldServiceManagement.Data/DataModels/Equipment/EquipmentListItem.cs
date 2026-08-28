using FieldServiceManagement.Data.DataModels.BaseClass;
using Microsoft.EntityFrameworkCore;

namespace FieldServiceManagement.Data.DataModels.Equipment
{

    [Keyless]
    public class EquipmentListItem : BaseGuidPrimaryKey
    {

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int? StatusId { get; set; }

        public int? Type { get; set; }

        public string? SerialNumber { get; set; }

        public int? WarrantyStatus { get; set; }

        public DateTime? WarrantyExpiryDate { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid OrganisationId { get; set; }

        public string? CreatedByName { get; set; }

        public string? CreatedByAvatar { get; set; }

        public Guid? CreatedById { get; set; }
    }
}
