using Riok.Mapperly.Abstractions;
using ToDoListApi.Models;
using ToDoListApi.DTOs;

namespace ToDoListApi.Mappers;

[Mapper]
public partial class UserMapper
{
    [MapperIgnoreSource(nameof(User.Password))]
    [MapperIgnoreTarget(nameof(ReadUserDto.ItemNames))]
    public partial ReadUserDto ToReadDto(User user);

    [MapperIgnoreTarget(nameof(User.Id))]
    public partial User ToEntity(RegisterDto dto);
    public partial void UpdateEntity(UpdateUserDto dto, User user);
}
