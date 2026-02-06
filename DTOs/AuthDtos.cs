using System.ComponentModel.DataAnnotations;

namespace OrdersBackend.DTOs;

public class RegisterUserDto {
    [Required]
    public string Name { get; set; } = null!;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = null!;
    public string? Role { get; set; }
}

public class LoginUserDto {
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
}
