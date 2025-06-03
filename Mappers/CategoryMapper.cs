using Riok.Mapperly.Abstractions;
using ToDoListApi.Models;
using ToDoListApi.DTOs;

namespace ToDoListApi.Mappers;

[Mapper]
public partial class CategoryMapper
{
    [MapperIgnoreSource(nameof(Item.UserId))]
    public partial ReadCategoryDto ToReadDto(Category category);


    [MapperIgnoreTarget(nameof(Category.Id))]
    public partial Category ToEntity(CreateCategoryDto dto, string userId);

    [MapperIgnoreTarget(nameof(Category.Id))]
    [MapperIgnoreTarget(nameof(Category.UserId))]

    public partial void UpdateEntity(UpdateCategoryDto dto, Category category);
}
