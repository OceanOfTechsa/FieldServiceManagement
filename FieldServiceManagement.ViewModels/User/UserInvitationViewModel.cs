using System.ComponentModel.DataAnnotations;

namespace FieldServiceManagement.ViewModels.User
{
    public class UserInvitationViewModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "First Name")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Surname")]
        [StringLength(100)]
        public string Surname { get; set; }

        [Display(Name = "Employee No")]
        [StringLength(50)]
        public string? EmployeeNumber { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        [StringLength(256)]
        public string Email { get; set; }

        [Required]
        [Display(Name = "User Type")]
        public int UserType { get; set; }

        public Guid OrganisationId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(10);

        [Display(Name = "Salutation")]
        public int SalutationId { get; set; }
    }
}
