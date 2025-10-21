using API_Doodles_2._0.Data;
using API_Doodles_2._0.Dto.ProductsDto;
using API_Doodles_2._0.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Doodles_2._0.Controllers.BaseControllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly DatabaseContext _context;
    public ProductsController(DatabaseContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductsDto>>> GetAll()
    {
        var products = await _context.Products
            .Select(p => new ProductsDto()
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductDescription = p.ProductDescription,
                ProductImage = p.ProductImage,
                ProductPrice = p.ProductPrice,
                Stock = p.Stock
            })
            .ToListAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductsDto>> GetById(int id)
    {
        var p = await _context.Products.FindAsync(id);
        if (p == null) return NotFound();
        return Ok(new ProductsDto
        {
            ProductId = p.ProductId,
            ProductName = p.ProductName,
            ProductDescription = p.ProductDescription,
            ProductImage = p.ProductImage,
            ProductPrice = p.ProductPrice,
            Stock = p.Stock
        });
    }

    [HttpPost]
    public async Task<ActionResult<ProductsDto>> Register([FromBody] ProductsDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Basic validation to keep data sane
        if (dto.ProductPrice < 0)
            return BadRequest(new { error = "ProductPrice cannot be negative" });
        if (dto.Stock < 0)
            return BadRequest(new { error = "Stock cannot be negative" });

        var product = new Products
        {
            ProductName = dto.ProductName,
            ProductDescription = dto.ProductDescription,
            ProductImage = dto.ProductImage,
            ProductPrice = dto.ProductPrice,
            Stock = dto.Stock
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var result = new ProductsDto
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            ProductDescription = product.ProductDescription,
            ProductImage = product.ProductImage,
            ProductPrice = product.ProductPrice,
            Stock = product.Stock
        };

        return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, result);
    }

    // Add stock to an existing product to make requests cleaner
    [HttpPost("{id}/stock")]
    public async Task<ActionResult<ProductsDto>> AddStock(int id, [FromBody] int quantity)
    {
        if (quantity <= 0)
            return BadRequest(new { error = "Quantity must be greater than zero" });

        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return NotFound(new { error = "Product not found" });

        checked
        {
            product.Stock += quantity;
        }

        await _context.SaveChangesAsync();

        var result = new ProductsDto
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            ProductDescription = product.ProductDescription,
            ProductImage = product.ProductImage,
            ProductPrice = product.ProductPrice,
            Stock = product.Stock
        };
        
        return Ok(result);
    }
}