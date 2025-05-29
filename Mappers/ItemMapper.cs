using Riok.Mapperly.Abstractions;
using ToDoListApi.Models;

namespace ToDoListApi.Mappers;

[Mapper]
public partial class ItemMapper
{
    [MapperIgnoreSource(nameof(Item.CategoryId))]
    [MapperIgnoreSource(nameof(Item.UserId))]
    public partial ReadItemDto ToReadDto(Item item, string categoryName, string username);

    [MapperIgnoreTarget(nameof(Item.Id))]
    [MapperIgnoreSource(nameof(CreateItemDto.CategoryName))]
    [MapperIgnoreSource(nameof(CreateItemDto.Username))]
    public partial Item ToEntity(CreateItemDto dto, string categoryId, string userId);

    [MapperIgnoreTarget(nameof(Item.Id))]
    [MapperIgnoreSource(nameof(UpdateItemDto.CategoryName))]
    [MapperIgnoreSource(nameof(UpdateItemDto.Username))]
    public partial void UpdateEntity(UpdateItemDto dto, Item item, string categoryId, string userId);
}