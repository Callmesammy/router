using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using router.DTOs;
using router.Models;
using router.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace router.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/signup", async (
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            SignUpDto req) =>
        {
            if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
                return Results.BadRequest(new { error = "Email and password are required." });

            var user = new ApplicationUser
            {
                Email = req.Email,
                UserName = req.Email,
                DisplayName = req.DisplayName
            };

            var result = await userManager.CreateAsync(user, req.Password);
            if (!result.Succeeded) return Results.BadRequest(result.Errors);

            // Optionally: add email confirmation token and send email
            var token = await tokenService.CreateJwtAsync(user);

            return Results.Ok(new AuthResponseDto(token, null));
        });

        group.MapPost("/login", async (
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            LoginDto req) =>
        {
            var user = await userManager.FindByEmailAsync(req.Email);
            if (user == null) return Results.Unauthorized();

            var valid = await userManager.CheckPasswordAsync(user, req.Password);
            if (!valid) return Results.Unauthorized();

            var token = await tokenService.CreateJwtAsync(user);
            return Results.Ok(new AuthResponseDto(token, null));
        });

        group.MapGet("/me", [Authorize] async (UserManager<ApplicationUser> userManager, ClaimsPrincipal user) =>
        {
            var id = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                     ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (id == null) return Results.Unauthorized();

            var u = await userManager.FindByIdAsync(id);
            if (u == null) return Results.NotFound();

            return Results.Ok(new { u.Id, u.Email, u.DisplayName });
        });
    }
}
