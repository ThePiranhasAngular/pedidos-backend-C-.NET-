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
    [Authorize(Roles = "USER")] // Regla 2: ADMIN no crea pedidos
    public async Task<ActionResult<Order>> CreateOrder(CreateOrderDto dto) {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst(JwtRegisteredClaimNames.Sub);
        if (userIdClaim == null) return Unauthorized("Usuario no identificado");
        
        var userId = Guid.Parse(userIdClaim.Value);

        if (!dto.Items.Any()) return BadRequest("El pedido debe tener al menos 1 detalle");

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

                if (product == null) throw new Exception($"Producto {itemDto.ProductId} no encontrado");
                if (!product.IsActive) throw new Exception($"Producto {product.Name} no está activo");
                if (product.Stock < itemDto.Quantity) throw new Exception($"Stock insuficiente para {product.Name}");

                product.Stock -= itemDto.Quantity;

                var orderItem = new OrderItem {
                    ProductId = product.Id,
                    Quantity = itemDto.Quantity,
                    Price = product.Price // Copiar precio histórico
                };

                order.OrderItems.Add(orderItem);
                total += orderItem.Quantity * orderItem.Price;
            }

            order.Total = total;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
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
            return await _context.Orders
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        } else {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrder(Guid id) {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return NotFound();

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst(JwtRegisteredClaimNames.Sub);
        var roleClaim = User.FindFirst(ClaimTypes.Role) ?? User.FindFirst("role");
        
        var userId = Guid.Parse(userIdClaim!.Value);
        var isAdmin = roleClaim?.Value == "ADMIN";

        if (!isAdmin && order.UserId != userId) return Forbid();

        return order;
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "ADMIN")] // Regla 4: Solo ADMIN cambia estado
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] OrderStatus status) {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return NotFound();

        order.Status = status;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{id}/cancel")]
    [Authorize(Roles = "USER")] // Regla 5: USER cancela si PENDING
    public async Task<IActionResult> CancelOrder(Guid id) {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

        if (order == null) return NotFound();
        if (order.Status != OrderStatus.PENDING) return BadRequest("Solo se pueden cancelar pedidos pendientes.");

        order.Status = OrderStatus.CANCELLED;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("stats")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetStats() {
        var totalOrders = await _context.Orders.CountAsync();
        var pendingOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.PENDING);
        var todayRevenue = await _context.Orders
            .Where(o => o.CreatedAt.Date == DateTime.UtcNow.Date && o.Status != OrderStatus.CANCELLED)
            .SumAsync(o => o.Total);

        return Ok(new { totalOrders, pendingOrders, todayRevenue });
    }
}
