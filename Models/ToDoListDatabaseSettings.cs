namespace ToDoListApi.Models;

public class ToDoListDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string ItemsCollectionName { get; set; } = null!;

    public string CategoriesCollectionName { get; set; } = null!;

    public string UsersCollectionName { get; set; } = null!;
}