using System.ComponentModel.DataAnnotations;
using FieldServiceManagement.Data.DataModels.BaseClass;

namespace FieldServiceManagement.ViewModels.Announcement
{
    public class EditAnnouncementViewModel : BaseGuidPrimaryKeyViewModel
    {
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        public string? Description { get; set; }

        public string? Notes { get; set; }

        [Required(ErrorMessage = "Announcement date is required.")]
        [Display(Name = "Announcement Date")]
        public DateTime AnnouncementDate { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [Display(Name = "Status")]
        public int StatusId { get; set; }

        public string? VisibleToRoleIds { get; set; }

        public List<int>? SelectedRoleIds { get; set; }

        public bool IsActive { get; set; }
    }
}