using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API_Doodles_2._0.Models.Items;

namespace API_Doodles_2._0.Models;

[Table("products")]
public class Products
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProductId { get; set; }
    
    [Required]
    [Range(0, int.MaxValue)]
    public decimal ProductPrice { get; set; }
    
    [MaxLength(200)]
    public string? ProductName { get; set; }
    
    [MaxLength(1000)]
    public string? ProductDescription { get; set; }
    
    [MaxLength(1000)]
    public string? ProductImage { get; set; }

    [Required] [Range(0, int.MaxValue)] public int Stock { get; set; } = 0;

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}