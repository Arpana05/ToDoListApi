using Riok.Mapperly.Abstractions;
using ToDoListApi.Models;

namespace ToDoListApi.Mappers;

[Mapper]
public partial class CategoryMapper
{
    public partial ReadCategoryDto ToReadDto(Category category);

    [MapperIgnoreTarget(nameof(Category.Id))]
    public partial Category ToEntity(CreateCategoryDto dto);

    [MapperIgnoreTarget(nameof(Category.Id))]
    public partial void UpdateEntity(UpdateCategoryDto dto, Category category);
}
