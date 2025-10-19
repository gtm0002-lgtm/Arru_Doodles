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
    
    [HttpPost]
    public async Task<ActionResult<ProductsDto>> Register([FromBody] RegisterDto dto)
    
    var products = new Products {ProductName }
}