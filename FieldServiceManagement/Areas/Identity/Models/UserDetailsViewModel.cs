using System.ComponentModel.DataAnnotations;

namespace FieldServiceManagement.Areas.Identity.Models;

public class UserDetailsViewModel
{
    public string Email { get; set; }

    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Surname is required")]
    public string Surname { get; set; }

    [Required(ErrorMessage = "Phone is required")]
    public string Phone { get; set; }
    public string Browser { get; set; } = "";
}
