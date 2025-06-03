namespace ToDoListApi.Models
{
    public class RegisterDto
    {
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
    } 

    public class LoginDto
    {
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class ReadUserDto
    {
        public required string Id { get; set; }
        public required string UserName { get; set; }
        public List<string> ItemNames { get; set; } = new();
    }

    public class UpdateUserDto
    {
        public required string Id { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }
}