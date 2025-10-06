using System.ComponentModel.DataAnnotations;

namespace API_Doodles_2._0.Dto;

public class UserDto
{
    [EmailAddress]
    [StringLength(100, MinimumLength = 5)] 
    public string Email { get; set; } = string.Empty;
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    
}