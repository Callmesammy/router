using router.Models;

namespace router.Services;

public interface ITokenService
{
    Task<string> CreateJwtAsync(ApplicationUser user);
}
