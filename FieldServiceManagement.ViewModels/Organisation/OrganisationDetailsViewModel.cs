using FieldServiceManagement.Data.DataModels.BaseClass;

namespace FieldServiceManagement.ViewModels.Organisation
{
    public class OrganisationDetailsViewModel : BaseGuidPrimaryKeyViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid CreatedById { get; set; }

        // Industry
        public int? IndustryId { get; set; }
        public string? IndustryName { get; set; }

        // Country
        public int? CountryId { get; set; }
        public string? CountryName { get; set; }
        public string? CountryCode { get; set; }

        // State
        public int? StateId { get; set; }
        public string? StateName { get; set; }
        public string? StateCode { get; set; }

        // Currency
        public int? CurrencyId { get; set; }
        public string? CurrencyName { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencySymbol { get; set; }

        // Timezone
        public int? TimezoneId { get; set; }
        public string? TimezoneName { get; set; }
        public string? TimezoneUtcOffset { get; set; }

        // Language
        public int? LanguageId { get; set; }
        public string? LanguageName { get; set; }
        public string? LanguageCode { get; set; }
        
        public string StatusLabel => IsActive ? "Active" : "Inactive";
        public string StatusCss => IsActive ? "bg-success" : "bg-secondary";
    }
}
