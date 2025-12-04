namespace router.DTOs;

public record SignUpDto(string Email, string Password, string DisplayName);
public record LoginDto(string Email, string Password);
public record AuthResponseDto(string Token, string? RefreshToken);
