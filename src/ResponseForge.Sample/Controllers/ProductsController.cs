using Microsoft.AspNetCore.Mvc;
using ResponseForge.Attributes;
using ResponseForge.Sample.Models;
using ResponseForge.Sample.Services;

namespace ResponseForge.Sample.Controllers;

/// <summary>
/// Demonstrates successful CRUD operations with ResponseForge wrapping using SOLID principles.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMockDataAppService _productService;

    public ProductsController(IMockDataAppService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Retrieves all products. Demonstrates the [ResponseMessage] attribute.
    /// </summary>
    [HttpGet]
    [ResponseMessage("Products retrieved successfully.", "Failed to retrieve products.")]
    public ActionResult<List<Product>> GetAll()
    {
        return Ok(_productService.GetAll());
    }

    /// <summary>
    /// Retrieves a product by ID. Demonstrates NotFound scenario.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ResponseMessage("Product retrieved successfully.", "Product not found.")]
    public ActionResult<Product> GetById(Guid id)
    {
        var product = _productService.GetById(id);

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
        var product = _productService.Create(request);

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    /// <summary>
    /// Deletes a product by ID. Demonstrates void success response.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ResponseMessage("Product deleted successfully.", "Product not found.")]
    public ActionResult Delete(Guid id)
    {
        var isDeleted = _productService.Delete(id);

        if (!isDeleted)
        {
            return NotFound();
        }

        return Ok(new { Deleted = true, ProductId = id });
    }

    /// <summary>
    /// Retrieves products without a custom message attribute.
    /// The default message from ResponseForgeOptions will be used.
    /// </summary>
    [HttpGet("no-attribute")]
    public ActionResult<List<Product>> GetAllWithoutAttribute()
    {
        return Ok(_productService.GetAll().Where(p => p.IsActive).ToList());
    }
}
