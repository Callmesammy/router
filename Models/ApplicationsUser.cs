using Microsoft.AspNetCore.Identity;

namespace router.Models;

public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
}
