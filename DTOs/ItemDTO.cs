namespace ToDoListApi.DTOs
{
    public class CreateItemDto
    {
        public string ItemTitle { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public string CategoryName { get; set; } = null!;
    }

    public class ReadItemDto
    {
        public string? Id { get; set; }
        public string ItemTitle { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime DueDate { get; set; }
        public string CategoryName { get; set; } = null!;
    }

    public class UpdateItemDto
    {
        public string ItemTitle { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime DueDate { get; set; }
        public required string CategoryName { get; set; }
    }
}