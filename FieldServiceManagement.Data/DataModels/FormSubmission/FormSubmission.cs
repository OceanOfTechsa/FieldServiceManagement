using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.FormSubmission
{
    [Table("FormSubmissions")]
    public class FormSubmission : BaseGuidPrimaryKey
    {
        public Guid OrganisationId { get; set; }
        public Guid? UserId { get; set; }
        public int FormId { get; set; }
        public int? StatusId { get; set; }
        public bool AwaitingResponse { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid CreatedById { get; set; }
        public Guid? UpdatedById { get; set; }
    }
}