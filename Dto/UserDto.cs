namespace CQRS_Microservice.Dtos
{
    public class UserDto
    {
        public string Username { get; set; }
        public List<string> Permissions { get; set; } = new List<string>();
    }
}
