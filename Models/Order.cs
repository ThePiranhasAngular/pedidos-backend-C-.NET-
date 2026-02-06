using System.Text.Json.Serialization;

namespace OrdersBackend.Models;
public enum OrderStatus { PENDING, PREPARING, DELIVERED, CANCELLED }
public class Order {
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.PENDING;
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [JsonPropertyName("items")]
    public List<OrderItem> OrderItems { get; set; } = new();
}