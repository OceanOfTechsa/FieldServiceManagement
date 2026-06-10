using System.ComponentModel.DataAnnotations;

namespace FieldServiceManagement.Areas.Identity.Models;

public class OnboardingViewModel
{
    public string Email { get; set; }
    // ─── Step 1 — User details ────────────────────────────────────────────────
    [Required(ErrorMessage = "Name is required.")]
    [Display(Name = "First name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Surname is required.")]
    [Display(Name = "Surname")]
    public string Surname { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required.")]
    [Display(Name = "Phone number")]
    public string Phone { get; set; } = string.Empty;

    // ─── Step 2 — Organisation ────────────────────────────────────────────────
    [Required(ErrorMessage = "Organisation name is required.")]
    [Display(Name = "Organisation name")]
    public string OrganisationName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Industry is required.")]
    [Display(Name = "Industry")]
    public int? IndustryId { get; set; }

    [Display(Name = "Industry category")]
    public int? IndustryCategoryId { get; set; }

    [Required(ErrorMessage = "Country is required.")]
    [Display(Name = "Country")]
    public int? CountryId { get; set; }

    [Display(Name = "State / Province")]
    public int? StateId { get; set; }

    [Required(ErrorMessage = "Currency is required.")]
    [Display(Name = "Currency")]
    public int? CurrencyId { get; set; }

    // ─── Step 3 — Locale ──────────────────────────────────────────────────────
    [Required(ErrorMessage = "Timezone is required.")]
    [Display(Name = "Timezone")]
    public int? TimezoneId { get; set; }

    [Required(ErrorMessage = "Language is required.")]
    [Display(Name = "Language")]
    public int? LanguageId { get; set; }
}