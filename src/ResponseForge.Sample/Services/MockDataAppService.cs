using ResponseForge.Sample.Models;

namespace ResponseForge.Sample.Services;

public class MockDataAppService : IMockDataAppService
{
    private readonly List<Product> _products =
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

    public List<Product> GetAll()
    {
        return _products;
    }

    public Product? GetById(Guid id)
    {
        return _products.FirstOrDefault(p => p.Id == id);
    }

    public Product Create(CreateProductRequest request)
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

        _products.Add(product);
        return product;
    }

    public bool Delete(Guid id)
    {
        var product = GetById(id);
        if (product is null) return false;

        _products.Remove(product);
        return true;
    }
}
