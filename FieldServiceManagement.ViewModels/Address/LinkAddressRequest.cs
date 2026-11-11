namespace FieldServiceManagement.ViewModels.Address
{
    public class LinkAddressRequest
    {
        public string EntityId { get; set; } = string.Empty;
        public string? ServiceAddressId { get; set; }
        public string? BillingAddressId { get; set; }
    }
}
