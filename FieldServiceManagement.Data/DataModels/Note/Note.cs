using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.Note
{
    [Table("Notes")]
    public class Note : BaseGuidPrimaryKey
    {
        [Required]
        public Guid OrganisationId { get; set; }

        /// <summary>
        /// FK → EntityTypes.Id (the type of record the note is attached to)
        /// </summary>
        [Required]
        public int EntityTypeId { get; set; }

        [Required]
        public Guid EntityId { get; set; }

        [MaxLength(200)]
        public string? Title { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public bool IsSharedToClient { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}