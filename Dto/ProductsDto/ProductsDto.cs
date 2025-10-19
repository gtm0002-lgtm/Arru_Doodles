namespace API_Doodles_2._0.Dto.ProductsDto;

public class ProductsDto
{
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductDescription { get; set; }
    public string? ProductImage { get; set; }
    public decimal ProductPrice { get; set; }
    public int Stock { get; set; } 
}