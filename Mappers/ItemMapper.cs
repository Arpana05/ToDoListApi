using Riok.Mapperly.Abstractions;
using ToDoListApi.Models;
using ToDoListApi.DTOs;

namespace ToDoListApi.Mappers;

[Mapper]
public partial class ItemMapper
{
    [MapperIgnoreSource(nameof(Item.CategoryId))]
    [MapperIgnoreSource(nameof(Item.UserId))]
    public partial ReadItemDto ToReadDto(Item item, string categoryName);

    [MapperIgnoreTarget(nameof(Item.Id))]
    [MapperIgnoreSource(nameof(CreateItemDto.CategoryName))]
    [MapperIgnoreTarget(nameof(Item.IsCompleted))]
    public partial Item ToEntity(CreateItemDto dto, string categoryId, string userId);

    [MapperIgnoreTarget(nameof(Item.Id))]
    [MapperIgnoreSource(nameof(UpdateItemDto.CategoryName))]
    public partial void UpdateEntity(UpdateItemDto dto, Item item, string categoryId, string userId);
}