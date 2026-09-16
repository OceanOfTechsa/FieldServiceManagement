using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.Entity
{
    [Table("EntityAddresses")]
    public class EntityAddress : BaseGuidPrimaryKey
    {
        [Required]
        public Guid OrganisationId { get; set; }

        /// <summary>
        /// FK → EntityTypes.Id
        /// </summary>
        [Required]
        public int EntityTypeId { get; set; }

        [Required]
        public Guid EntityId { get; set; }

        [Required]
        public Guid AddressId { get; set; }

        /// <summary>
        /// FK → AddressTypes.Id
        /// </summary>
        [Required]
        public int AddressTypeId { get; set; }

        public bool IsPrimary { get; set; } = false;

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Guid? CreatedById { get; set; }
        public Guid? UpdatedById { get; set; }
    }
}