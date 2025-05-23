namespace ToDoListApi.Models
{
    public class CreateUserDto
    {
        public required string UserName { get; set; }
    }

    public class ReadUserDto
    {
        public required string Id { get; set; }
        public required string UserName { get; set; }
    }

    public class UpdateUserDto
    {
        public required string Id { get; set; }
        public required string UserName { get; set; }
    }
}