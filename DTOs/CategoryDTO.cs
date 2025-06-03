namespace ToDoListApi.DTOs
{
    public class CreateCategoryDto
    {
        public required string CategoryName { get; set; }
        public string? Description { get; set; }
    }

    public class ReadCategoryDto
    {
        public required string Id { get; set; }
        public required string CategoryName { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateCategoryDto
    {
        public required string CategoryName { get; set; }
        public string? Description { get; set; }
    }
}