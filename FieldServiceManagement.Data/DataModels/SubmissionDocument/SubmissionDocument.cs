using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.SubmissionDocument
{
    [Table("SubmissionDocuments")]
    public class SubmissionDocument : BaseGuidPrimaryKey
    {
        public Guid FormSubmissionId { get; set; }
        public int? DocumentTypeId { get; set; }
        public string DocumentName { get; set; }
        public DateTime? Date { get; set; }
        public bool? IsManager { get; set; }
        public Guid? StaffId { get; set; }
        public string FilePath { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid CreatedById { get; set; }
        public Guid? UpdatedById { get; set; }
    }
}