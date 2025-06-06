using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;


namespace ToDoListApi.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("Name")]
    [JsonPropertyName("Name")]
    public required string UserName { get; set; }

    [BsonElement("Password")]
    public required string Password { get; set; }
}
