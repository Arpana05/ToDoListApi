using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace ToDoListApi.Models;

public class Category
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("Name")]
    [JsonPropertyName("Name")]
    public required string CategoryName { get; set; }
    public string? Description { get; set; }
    public string UserId { get; set; } = null!;
}
