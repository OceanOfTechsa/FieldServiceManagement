using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FieldServiceManagement.ViewModels.Announcement
{
    public class CreateAnnouncementViewModel
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        public string? Notes { get; set; }

        [DisplayName("Announcement Date")]
        [Required(ErrorMessage = "Announcement date is required.")]
        public DateTime AnnouncementDate { get; set; } = DateTime.UtcNow;

        [DisplayName("Announcement Status")]
        [Required(ErrorMessage = "Status is required.")]
        public int StatusId { get; set; }

        public bool IsActive { get; set; } = true;

        // Comma-separated role IDs, e.g. "1,3,5". Null/empty = visible to everyone.
        [DisplayName("Visible To")]
        public string? VisibleToRoleIds { get; set; }

        // Bound from a multi-select on the view; joined into VisibleToRoleIds before insert
        public List<int> SelectedRoleIds { get; set; } = new();

        public string? CreatedByEmail { get; set; }

        [DisplayName("Delivery Type")]
        public int AnnouncementDeliveryType { get; set; }

        public int? Severity { get; set; }

        public List<AnnouncementDeliveryTypeViewModel> AnnouncementDeliveryTypes { get; set; } = new();
    }
}