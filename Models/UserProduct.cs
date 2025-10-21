using System.ComponentModel.DataAnnotations.Schema;

namespace API_Doodles_2._0.Models;

// Join entity representing a granted product (e.g., a badge) owned by a user
[Table("user_products")]
public class UserProduct
{
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public DateTime GrantedAt { get; set; } = DateTime.UtcNow;

    // Navigations (optional but useful)
    public Users User { get; set; } = null!;
    public Products Product { get; set; } = null!;
}
