namespace API_Doodles_2._0.Dto.OrdersDto;

public class CreateOrderItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class CreateOrderDto
{
    public int UserId { get; set; }
    public List<CreateOrderItemDto> Items { get; set; } = new();
}