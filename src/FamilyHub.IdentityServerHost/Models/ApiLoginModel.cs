using System.ComponentModel.DataAnnotations;

namespace FamilyHub.IdentityServerHost.Models;

public class ApiLoginModel
{
    [Required(ErrorMessage = "User Name is required")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; set; }
}
