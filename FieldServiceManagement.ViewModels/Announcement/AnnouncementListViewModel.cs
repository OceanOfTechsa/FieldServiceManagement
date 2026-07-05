namespace FieldServiceManagement.ViewModels.Announcement
{
    public class AnnouncementListViewModel
    {
        public List<UserAnnouncementViewModel> Announcements { get; set; } = new();

        public int TotalCount => Announcements.Count;
        public int SeenCount => Announcements.Count(a => a.IsSeen);
        public int NewCount => Announcements.Count(a => !a.IsSeen);
    }
}
