namespace FieldServiceManagement.ViewModels.Company
{
    public class CompanyListItemViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? Mobile { get; set; }
        public string? Website { get; set; }
        public int Type { get; set; }
        public string? ServiceAddressName { get; set; }
        public string? BillingAddressName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedByName { get; set; }
    }
}
