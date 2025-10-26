using System.Text.Json;
using API_Doodles_2._0.Data;
using API_Doodles_2._0.Dto.ProductsDto;
using API_Doodles_2._0.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Doodles_2._0.Controllers.BaseControllers
{
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

        // JSON endpoint
        [HttpPost]
        [Consumes("application/json", "multipart/form-data")]
        public async Task<ActionResult<ProductsDto>> Register()
        {
            ProductsDto dto;

            if (Request.HasFormContentType)
            {
                var form = await Request.ReadFormAsync();

                dto = new ProductsDto
                {
                    ProductName = form["ProductName"].ToString(),
                    ProductDescription = form["ProductDescription"].ToString(),
                    ProductImage = form["ProductImage"].ToString(),
                    ProductPrice = decimal.TryParse(form["ProductPrice"], out var p) ? p : 0m,
                    Stock = int.TryParse(form["Stock"], out var s) ? s : 0
                };

                // Si en el futuro se sube un archivo real:
                // var file = form.Files.GetFile("ProductImage");
                // procesar file y obtener URL si es necesario.
            }
            else
            {
                dto = await JsonSerializer.DeserializeAsync<ProductsDto>(Request.Body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (dto == null) return BadRequest(new { error = "Invalid JSON payload" });
            }

            // Validaciones de modelo
            if (!TryValidateModel(dto))
                return BadRequest(ModelState);

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound(new { message = "Product not found" });

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Product deleted successfully" });
        }
    }
}