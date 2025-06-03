using ToDoListApi.Models;
using ToDoListApi.DTOs;
using ToDoListApi.Mappers;
using ToDoListApi.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Microsoft.AspNetCore.Identity;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ToDoListApi.Services
{
    public class UsersService
    {
        private readonly IMongoCollection<User> _usersCollection;
        private readonly ItemsService _itemsService;
        private readonly UserMapper _mapper;
        private readonly PasswordHasher<User> _passwordHasher = new();
        private readonly JwtSettings _jwtSettings;

        public UsersService(
            IOptions<ToDoListDatabaseSettings> todoListdatabaseSettings,
            IOptions<JwtSettings> jwtSettings,
            UserMapper mapper,
            ItemsService itemsService)
        {
            var mongoClient = new MongoClient(
                todoListdatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                todoListdatabaseSettings.Value.DatabaseName);

            _usersCollection = mongoDatabase.GetCollection<User>(
                todoListdatabaseSettings.Value.UsersCollectionName);

            _itemsService = itemsService;
            _mapper = mapper;
            _jwtSettings = jwtSettings.Value;
        }


        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            var user = await _usersCollection.Find(u => u.UserName == loginDto.UserName).FirstOrDefaultAsync();

            if (user == null)
                throw new UnauthorizedAccessException("Invalid username or password");

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.Password, loginDto.Password);

            if (result == PasswordVerificationResult.Failed)
                throw new UnauthorizedAccessException("Invalid username or password");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
            }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<ReadUserDto> RegisterAsync(RegisterDto createDto)
        {
            var existingUser = await _usersCollection.Find(x => x.UserName == createDto.UserName).FirstOrDefaultAsync();
            if (existingUser != null)
                throw new ArgumentException($"User '{createDto.UserName}' already exists");
            var user = _mapper.ToEntity(createDto);

            var passwordHasher = new PasswordHasher<User>();
            user.Password = passwordHasher.HashPassword(user, createDto.Password);

            await _usersCollection.InsertOneAsync(user);

            return _mapper.ToReadDto(user);
        }

        public async Task<ReadUserDto?> GetAsync(string userId)
        {
            var user = await _usersCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
        if (user == null) return null;

        var dto = _mapper.ToReadDto(user);

        var items = await _itemsService.GetItemsByUserIdAsync(userId);

        dto.ItemNames = items.Select(i => i.ItemTitle).ToList();

        return dto;
        }

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

        public async Task<User?> GetByUserNameAsync(string userName)
        {
            return await _usersCollection.Find(u => u.UserName == userName).FirstOrDefaultAsync();
        }

    }
}
