using System.ComponentModel.DataAnnotations;

public class CreateUserDto
{
    [Required]
    public string Username { get; set; }
    
    [Required]
    public string Password { get; set; }
    
    public List<string> Permissions { get; set; } = new List<string>();
}
