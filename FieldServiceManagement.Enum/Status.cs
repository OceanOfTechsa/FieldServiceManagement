using System.ComponentModel.DataAnnotations;


namespace FieldServiceManagement.Enum
{
    public enum Status
    {
        [Display(Name = "Invited")]
        Invited = 1,

        [Display(Name = "Active")]
        Active = 2,

        [Display(Name = "Inactive")]
        Inactive = 3,

        [Display(Name = "Suspended")]
        Suspended = 4,

        [Display(Name = "Deleted")]
        Deleted = 5,

        [Display(Name = "NewRequest")]
        NewRequest = 6,

        [Display(Name = "EstimateCreated")]
        EstimateCreated = 7,

        [Display(Name = "Converted")]
        Converted = 8,

        [Display(Name = "WorkOrderNew")]
        WorkOrderNew = 9,

        [Display(Name = "Scheduled")]
        Scheduled = 10,

        [Display(Name = "Dispatched")]
        Dispatched = 11,

        [Display(Name = "Accepted")]
        Accepted = 12,

        [Display(Name = "OnRoute")]
        OnRoute = 13,

        [Display(Name = "InProgress")]
        InProgress = 14,

        [Display(Name = "Completed")]
        Completed = 15,

        [Display(Name = "Closed")]
        Closed = 16,

        [Display(Name = "Cancelled")]
        Cancelled = 17,

        [Display(Name = "Terminated")]
        Terminated = 18,

        [Display(Name = "Draft")]
        Draft = 19,

        [Display(Name = "PendingApproval")]
        PendingApproval = 20,

        [Display(Name = "Approved")]
        Approved = 21,

        [Display(Name = "Rejected")]
        Rejected = 22,

        [Display(Name = "Sent")]
        Sent = 23,

        [Display(Name = "PartiallyPaid")]
        PartiallyPaid = 24,

        [Display(Name = "Paid")]
        Paid = 25,

        [Display(Name = "AnnouncementInDraft")]
        AnnouncementInDraft = 26,

        [Display(Name = "AnnouncementPublished")]
        AnnouncementPublished = 27,

        [Display(Name = "AnnouncementArchived")]
        AnnouncementArchived = 28,
        [Display(Name = "EquipementActive")]
        EquipementActive = 29,

        [Display(Name = "EquipementInActive")]
        EquipementInActive = 30,

        [Display(Name = "EquipementOutOfService")]
        EquipementOutOfService = 31,

        [Display(Name = "EquipementUnderMaintenance")]
        EquipementUnderMaintenance = 32
    }
}
