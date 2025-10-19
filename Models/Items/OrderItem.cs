using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Doodles_2._0.Models.Items;

[Table("order_items")]
public class OrderItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int OrderId { get; set; }

    [ForeignKey(nameof(Order))] 
    public int OrderItemId { get; set; } 
    public Orders Order { get; set; } = null!;
    
    [ForeignKey(nameof(Product))]
    public int ProductId { get; set; }
    public Products Product { get; set; } = null!;
    
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
    
    // Store unit price at time of purchase (avoid changing history if product price changes)
    public decimal UnitPrice { get; set; }
}