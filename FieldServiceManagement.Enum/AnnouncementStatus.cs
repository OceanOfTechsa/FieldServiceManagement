
using System.ComponentModel.DataAnnotations;

namespace FieldServiceManagement.Enum
{
    public enum AnnouncementStatus
    {
        [Display(Name = "AnnouncementInDraft")]
        AnnouncementInDraft = 26,
        [Display(Name = "AnnouncementPublished")]
        AnnouncementPublished = 27,
        [Display(Name = "AnnouncementArchived")]
        AnnouncementArchived = 28,
    }
}
