using Microsoft.AspNetCore.Mvc;
using ResponseForge.Attributes;
using ResponseForge.Sample.Models;

namespace ResponseForge.Sample.Controllers;

/// <summary>
/// Demonstrates successful CRUD operations with ResponseForge wrapping.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    // In-memory store for demonstration purposes
    private static readonly List<Product> Products =
    [
        new()
        {
            Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
            Name = "Mechanical Keyboard",
            Description = "RGB backlit mechanical keyboard with Cherry MX switches",
            Price = 149.99m,
            Category = "Electronics",
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-30)
        },
        new()
        {
            Id = Guid.Parse("b2c3d4e5-f678-90ab-cdef-123456789012"),
            Name = "Wireless Mouse",
            Description = "Ergonomic wireless mouse with 16000 DPI sensor",
            Price = 79.99m,
            Category = "Electronics",
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-15)
        },
        new()
        {
            Id = Guid.Parse("c3d4e5f6-7890-abcd-ef12-345678901234"),
            Name = "USB-C Hub",
            Description = "7-in-1 USB-C hub with HDMI, USB 3.0, and SD card reader",
            Price = 49.99m,
            Category = "Accessories",
            IsActive = false,
            CreatedAt = DateTime.UtcNow.AddDays(-7)
        }
    ];

    /// <summary>
    /// Retrieves all products. Demonstrates the [ResponseMessage] attribute.
    /// </summary>
    [HttpGet]
    [ResponseMessage("Products retrieved successfully.", "Failed to retrieve products.")]
    public ActionResult<List<Product>> GetAll()
    {
        return Ok(Products);
    }

    /// <summary>
    /// Retrieves a product by ID. Demonstrates NotFound scenario.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ResponseMessage("Product retrieved successfully.", "Product not found.")]
    public ActionResult<Product> GetById(Guid id)
    {
        var product = Products.FirstOrDefault(p => p.Id == id);

        if (product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    /// <summary>
    /// Creates a new product. Demonstrates validation error wrapping.
    /// Try sending an invalid payload to see validation errors in the response.
    /// </summary>
    [HttpPost]
    [ResponseMessage("Product created successfully.")]
    public ActionResult<Product> Create([FromBody] CreateProductRequest request)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Category = request.Category,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        Products.Add(product);

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    /// <summary>
    /// Deletes a product by ID. Demonstrates void success response.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ResponseMessage("Product deleted successfully.", "Product not found.")]
    public ActionResult Delete(Guid id)
    {
        var product = Products.FirstOrDefault(p => p.Id == id);

        if (product is null)
        {
            return NotFound();
        }

        Products.Remove(product);

        return Ok(new { Deleted = true, ProductId = id });
    }

    /// <summary>
    /// Retrieves products without a custom message attribute.
    /// The default message from ResponseForgeOptions will be used.
    /// </summary>
    [HttpGet("no-attribute")]
    public ActionResult<List<Product>> GetAllWithoutAttribute()
    {
        return Ok(Products.Where(p => p.IsActive).ToList());
    }
}
