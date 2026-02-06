namespace OrdersBackend.Models;
public enum UserRole { USER, ADMIN }
public class User {
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public UserRole Role { get; set; } = UserRole.USER;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}