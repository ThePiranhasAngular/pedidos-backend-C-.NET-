using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrdersBackend.Data;
using OrdersBackend.Models;

namespace OrdersBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "ADMIN")]
public class UsersController : ControllerBase {
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context) {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetUsers() {
        return await _context.Users
            .Select(u => new {
                u.Id,
                u.Name,
                u.Email,
                u.Role,
                u.CreatedAt,
                u.UpdatedAt
            })
            .ToListAsync();
    }

    [HttpPut("{id}/role")]
    public async Task<IActionResult> UpdateUserRole(Guid id, [FromBody] string role) {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        if (!Enum.TryParse<UserRole>(role, true, out var newRole)) {
            return BadRequest("Rol inválido.");
        }

        user.Role = newRole;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
