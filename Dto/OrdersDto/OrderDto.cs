namespace API_Doodles_2._0.Dto.OrdersDto;

public class OrderItemDto
{
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}

public class OrderDto
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = null!;
    public List<OrderItemDto> Items { set; get; } = new();
    
    public decimal TotalPrice => Items.Sum(x => x.UnitPrice * x.Quantity);

}