using Riok.Mapperly.Abstractions;
using ToDoListApi.Models;

namespace ToDoListApi.Mappers;

[Mapper]
public partial class UserMapper
{
    public partial ReadUserDto ToReadDto(User user);

    [MapperIgnoreTarget(nameof(User.Id))]
    public partial User ToEntity(CreateUserDto dto);
    public partial void UpdateEntity(UpdateUserDto dto, User user);
}
