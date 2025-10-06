using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Doodles_2._0.Models;

[Table("users")]
public class Users
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required string UserName { get; set; }

    [Required]
    public required string Email { get; set; }

    [Required]
    public string Password { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
