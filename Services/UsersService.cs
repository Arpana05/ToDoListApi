using ToDoListApi.Models;
using ToDoListApi.Mappers;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ToDoListApi.Services
{
    public class UsersService
    {
        private readonly IMongoCollection<User> _usersCollection;
        private readonly UserMapper _mapper;

        public UsersService(
            IOptions<ToDoListDatabaseSettings> todoListdatabaseSettings,
            UserMapper mapper)
        {
            _mapper = mapper;
            var mongoClient = new MongoClient(
                todoListdatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                todoListdatabaseSettings.Value.DatabaseName);

            _usersCollection = mongoDatabase.GetCollection<User>(
                todoListdatabaseSettings.Value.UsersCollectionName);
        }

        // public async Task<List<User>> GetAsync() =>
        //     await _usersCollection.Find(_ => true).ToListAsync();

        public async Task<List<ReadUserDto>> GetAsync()
        {
            var users = await _usersCollection.Find(_ => true).ToListAsync();
            return users.Select(_mapper.ToReadDto).ToList();
        }

        // public async Task<User?> GetAsync(string id) =>
        //     await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<ReadUserDto?> GetAsync(string id)
        {
            var user = await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            return user == null ? null : _mapper.ToReadDto(user);
        }

        // public async Task CreateAsync(User newUser) =>
        //     await _usersCollection.InsertOneAsync(newUser);

        public async Task<ReadUserDto> CreateAsync(CreateUserDto createDto)
        {
            var existingUser = await _usersCollection.Find(x => x.UserName == createDto.UserName).FirstOrDefaultAsync();
            if (existingUser != null)
                throw new ArgumentException($"User '{createDto.UserName}' already exists");

            var user = _mapper.ToEntity(createDto);
            
            await _usersCollection.InsertOneAsync(user);

            return _mapper.ToReadDto(user);
        }

        // public async Task UpdateAsync(string id, User updatedUser) =>
        //     await _usersCollection.ReplaceOneAsync(x => x.Id == id, updatedUser);

        public async Task<ReadUserDto?> UpdateAsync(string id, UpdateUserDto updateDto)
        {
            var existingUser = await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (existingUser == null) return null;

            var duplicateUser = await _usersCollection.Find(x => x.UserName == updateDto.UserName && x.Id != id).FirstOrDefaultAsync();
            if (duplicateUser != null)
                throw new ArgumentException($"User '{updateDto.UserName}' already exists");

            _mapper.UpdateEntity(updateDto, existingUser);

            await _usersCollection.ReplaceOneAsync(x => x.Id == id, existingUser);

            return _mapper.ToReadDto(existingUser);
        }

        public async Task RemoveAsync(string id) =>
            await _usersCollection.DeleteOneAsync(x => x.Id == id);
    }
}
