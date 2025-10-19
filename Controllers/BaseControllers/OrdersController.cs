using API_Doodles_2._0.Data;
using API_Doodles_2._0.Dto.OrdersDto;
using API_Doodles_2._0.Models;
using API_Doodles_2._0.Models.Items;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Doodles_2._0.Controllers.BaseControllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    protected readonly DatabaseContext _context;
    public OrdersController(DatabaseContext context) => _context = context;

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderDto dto)
    {
        var user = await _context.Users.FindAsync(dto.UserId);
        if (user == null) return NotFound( new {message = "User not found"});
        var productIds = dto.Items.Select(i => i.ProductId).ToList();
        var products = await _context.Products.Where(p => productIds.Contains(p.ProductId)).ToListAsync();

        if (products.Count != productIds.Count)
        {
            return BadRequest(new {message = "Products not found due an Invalid Product or Products Id"});
        }
        
        var order = new Orders {UserId = dto.UserId, Status = "Pending"};
        foreach (var item in dto.Items)
        {
            var product = products.First(p => p.ProductId == item.ProductId);
            if (item.Quantity <= 0) return BadRequest(new { message = $"Invalid quantity for {product}" });

            order.Items.Add(new OrderItem
            {
                ProductId = product.ProductId,
                OrderId = order.OrderId,
                UnitPrice = product.ProductPrice,
            });
        }
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return Ok(new {message = "Order created successfully"});

        var result = new OrderDto
        {
            OrderId = order.OrderId,
            UserId = order.UserId,
            CreatedAt = order.CreatedAt,
            Status = order.Status,
            Items = order.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = products.First(p => p.ProductId == i.ProductId).ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
            }).ToList()
        };

        return CreatedAtAction(nameof(GetById), new { id = order.OrderId }, result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.OrderId == id);
        if (order == null) return NotFound();

        var dto = new OrderDto
        {
            OrderId = order.OrderId,
            UserId = order.UserId,
            CreatedAt = order.CreatedAt,
            Status = order.Status,
            Items = order.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };
        
        return Ok(dto);
    }

    [HttpGet("/user/{userId:int}")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetByUserId(int userId)
    {
        var orders = await _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        var result = orders.Select(order => new OrderDto
        {
            OrderId = order.OrderId,
            UserId = order.UserId,
            CreatedAt = order.CreatedAt,
            Status = order.Status,
            Items = order.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity
            }).ToList()
        });
        return Ok(result);
    }
    
}