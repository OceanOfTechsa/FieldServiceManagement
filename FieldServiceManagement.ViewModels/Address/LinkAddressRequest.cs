namespace FieldServiceManagement.ViewModels.Address
{
    public class LinkAddressRequest
    {
        public string CompanyId { get; set; } = default!;
        public string? ServiceAddressId { get; set; }
        public string? BillingAddressId { get; set; }
    }
}
