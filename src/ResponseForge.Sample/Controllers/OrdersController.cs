using Microsoft.AspNetCore.Mvc;
using ResponseForge.Attributes;
using ResponseForge.Sample.Models;

namespace ResponseForge.Sample.Controllers;

/// <summary>
/// Demonstrates various error scenarios handled by ResponseForge.
/// Each endpoint triggers a different type of exception to showcase
/// the middleware's exception-to-response mapping capabilities.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    /// <summary>
    /// Demonstrates a successful order retrieval.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ResponseMessage("Order retrieved successfully.")]
    public ActionResult<Order> GetById(Guid id)
    {
        var order = new Order
        {
            Id = id,
            OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-001",
            ProductId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
            Quantity = 2,
            TotalAmount = 299.98m,
            Status = "Confirmed",
            OrderDate = DateTime.UtcNow
        };

        return Ok(order);
    }

    /// <summary>
    /// Throws a KeyNotFoundException to demonstrate 404 error mapping.
    /// The middleware will return a 404 with a standardized error response.
    /// </summary>
    [HttpGet("not-found")]
    [ResponseMessage("Order found.", "The requested order does not exist.")]
    public ActionResult<Order> TriggerNotFound()
    {
        throw new KeyNotFoundException("Order with the specified ID was not found.");
    }

    /// <summary>
    /// Throws an UnauthorizedAccessException to demonstrate 401 error mapping.
    /// </summary>
    [HttpGet("unauthorized")]
    public ActionResult<Order> TriggerUnauthorized()
    {
        throw new UnauthorizedAccessException("You do not have permission to access this order.");
    }

    /// <summary>
    /// Throws an InvalidOperationException to demonstrate 409 Conflict error mapping.
    /// </summary>
    [HttpPost("conflict")]
    [ResponseMessage("Order processed.", "Order processing failed.")]
    public ActionResult TriggerConflict()
    {
        throw new InvalidOperationException("Cannot process order: the item is already reserved by another customer.");
    }

    /// <summary>
    /// Throws an ArgumentException to demonstrate 400 Bad Request error mapping.
    /// </summary>
    [HttpPost("bad-request")]
    public ActionResult TriggerBadRequest()
    {
        throw new ArgumentException("The provided quantity must be greater than zero.", nameof(Order.Quantity));
    }

    /// <summary>
    /// Throws a generic Exception to demonstrate 500 Internal Server Error mapping.
    /// In Development, the stack trace will be included in the response.
    /// In Production, the stack trace will be null.
    /// </summary>
    [HttpGet("server-error")]
    public ActionResult TriggerServerError()
    {
        throw new Exception("An unexpected database connection failure occurred.");
    }

    /// <summary>
    /// Throws a NotImplementedException to demonstrate 501 Not Implemented mapping.
    /// </summary>
    [HttpPost("not-implemented")]
    [ResponseMessage("Feature available.", "This feature is coming soon.")]
    public ActionResult TriggerNotImplemented()
    {
        throw new NotImplementedException("Bulk order processing is not yet available.");
    }
}
