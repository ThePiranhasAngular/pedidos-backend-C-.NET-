using System.ComponentModel.DataAnnotations;

namespace OrdersBackend.DTOs;

public class CreateProductDto {
    [Required]
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? Category { get; set; }
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }
}

public class UpdateProductDto {
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? Category { get; set; }
    [Range(0.01, double.MaxValue)]
    public decimal? Price { get; set; }
    [Range(0, int.MaxValue)]
    public int? Stock { get; set; }
    public bool? IsActive { get; set; }
}
