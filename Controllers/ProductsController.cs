using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrdersBackend.Data;
using OrdersBackend.DTOs;
using OrdersBackend.Models;

namespace OrdersBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase {
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context) {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts() {
        return await _context.Products.Where(p => p.IsActive).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(Guid id) {
        var product = await _context.Products.FindAsync(id);

        if (product == null) {
            return NotFound();
        }

        return product;
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")] // Regla 1: Solo ADMIN crea/modifica
    public async Task<ActionResult<Product>> PostProduct(CreateProductDto dto) {
        var product = new Product {
            Name = dto.Name,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            Category = dto.Category,
            Price = dto.Price,
            Stock = dto.Stock
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetProduct", new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> PutProduct(Guid id, UpdateProductDto dto) {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        if (dto.Name != null) product.Name = dto.Name;
        if (dto.Description != null) product.Description = dto.Description;
        if (dto.ImageUrl != null) product.ImageUrl = dto.ImageUrl;
        if (dto.Category != null) product.Category = dto.Category;
        if (dto.Price.HasValue) product.Price = dto.Price.Value;
        if (dto.Stock.HasValue) product.Stock = dto.Stock.Value;
        if (dto.IsActive.HasValue) product.IsActive = dto.IsActive.Value;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteProduct(Guid id) {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        // Regla: No eliminar productos con pedidos
        var hasOrders = await _context.OrderItems.AnyAsync(oi => oi.ProductId == id);
        if (hasOrders) {
            return BadRequest("No se puede eliminar un producto que ya tiene pedidos asociados.");
        }

        product.IsActive = false; // Soft delete
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
