using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.UploadDocument
{
    [Table("UploadDocuments")]
    public class UploadDocument : BaseIntPrimaryKey
    {
        public string DocumentType { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
    }
}