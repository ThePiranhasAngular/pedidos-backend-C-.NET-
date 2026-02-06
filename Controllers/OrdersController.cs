using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrdersBackend.Data;
using OrdersBackend.DTOs;
using OrdersBackend.Models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace OrdersBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrdersController : ControllerBase {
    private readonly AppDbContext _context;

    public OrdersController(AppDbContext context) {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(CreateOrderDto dto) {
        // Obtener ID del usuario desde el token
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst(JwtRegisteredClaimNames.Sub);
        if (userIdClaim == null) return Unauthorized("Usuario no identificado");
        
        var userId = Guid.Parse(userIdClaim.Value);

        using var transaction = await _context.Database.BeginTransactionAsync();

        try {
            var order = new Order {
                UserId = userId,
                Status = OrderStatus.PENDING,
                CreatedAt = DateTime.UtcNow,
                OrderItems = new List<OrderItem>()
            };

            decimal total = 0;

            foreach (var itemDto in dto.Items) {
                var product = await _context.Products.FindAsync(itemDto.ProductId);

                if (product == null) {
                    throw new Exception($"Producto {itemDto.ProductId} no encontrado");
                }

                if (!product.IsActive) {
                    throw new Exception($"Producto {product.Name} no está activo");
                }

                if (product.Stock < itemDto.Quantity) {
                    throw new Exception($"Stock insuficiente para {product.Name}");
                }

                // Descontar stock
                product.Stock -= itemDto.Quantity;

                var orderItem = new OrderItem {
                    ProductId = product.Id,
                    Quantity = itemDto.Quantity,
                    Price = product.Price // Snapshot del precio
                };

                order.OrderItems.Add(orderItem);
                total += orderItem.Quantity * orderItem.Price;
            }

            order.Total = total;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return CreatedAtAction(nameof(GetOrders), new { id = order.Id }, order);
        } catch (Exception ex) {
            await transaction.RollbackAsync();
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrders() {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst(JwtRegisteredClaimNames.Sub);
        var roleClaim = User.FindFirst(ClaimTypes.Role) ?? User.FindFirst("role");

        if (userIdClaim == null) return Unauthorized();

        var userId = Guid.Parse(userIdClaim.Value);
        var isAdmin = roleClaim?.Value == "ADMIN";

        if (isAdmin) {
            return await _context.Orders.Include(o => o.OrderItems).ToListAsync();
        } else {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ToListAsync();
        }
    }
}
