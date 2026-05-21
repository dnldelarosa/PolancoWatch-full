using System.ComponentModel.DataAnnotations;

namespace PolancoWatch.Application.DTOs;

public class LoginRequest
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
}

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}

public class UpdateProfileRequest
{
    [Required]
    [MinLength(6)]
    public string CurrentPassword { get; set; } = string.Empty;



    [MinLength(6)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$", ErrorMessage = "Passphrase must be at least 8 characters long, containing uppercase, lowercase, numbers, and a special character.")]
    public string? NewPassword { get; set; }
}

public class ForgotPasswordRequest
{
    [EmailAddress]
    public string? Email { get; set; }

    [StringLength(50, MinimumLength = 3)]
    public string? Username { get; set; }
}

public class ResetPasswordRequest
{
    [Required]
    public string Token { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$", ErrorMessage = "Passphrase must be at least 8 characters long, containing uppercase, lowercase, numbers, and a special character.")]
    public string NewPassword { get; set; } = string.Empty;
}
