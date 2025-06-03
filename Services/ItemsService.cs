using ToDoListApi.Models;
using ToDoListApi.DTOs;
using ToDoListApi.Mappers;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
namespace ToDoListApi.Services
{
    public class ItemsService
    {
        private readonly IMongoCollection<Item> _itemsCollection;
        private readonly IMongoCollection<Category> _categoriesCollection;
        private readonly IMongoCollection<User> _usersCollection;
        private readonly ItemMapper _mapper;

        public ItemsService(
            IOptions<ToDoListDatabaseSettings> todoListdatabaseSettings,
            ItemMapper mapper)
        {
            _mapper = mapper;

            var mongoClient = new MongoClient(
                todoListdatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                todoListdatabaseSettings.Value.DatabaseName);

            _itemsCollection = mongoDatabase.GetCollection<Item>(
                todoListdatabaseSettings.Value.ItemsCollectionName);

            _categoriesCollection = mongoDatabase.GetCollection<Category>(
            todoListdatabaseSettings.Value.CategoriesCollectionName);
            _usersCollection = mongoDatabase.GetCollection<User>(
            todoListdatabaseSettings.Value.UsersCollectionName);
        }

        public async Task<List<ReadItemDto>> GetAsync(string userId)
        {
            var items = await _itemsCollection.Find(i => i.UserId == userId).ToListAsync();

            var categoryIds = items.Select(i => i.CategoryId).Distinct().ToList();
            var categories = await _categoriesCollection.Find(c => categoryIds.Contains(c.Id)).ToListAsync();
            var categoryDict = categories.ToDictionary(c => c.Id, c => c.CategoryName);

            return items.Select(item =>
            {
                var categoryName = categoryDict.TryGetValue(item.CategoryId, out var name) ? name : "Unknown";
                return _mapper.ToReadDto(item, categoryName);
            }).ToList();
        }

        public async Task<List<Item>> GetItemsByUserIdAsync(string userId)
        {
            return await _itemsCollection.Find(i => i.UserId == userId).ToListAsync();
        }


        public async Task<ReadItemDto?> GetByIdAsync(string itemId, string userId)
        {
            var item = await _itemsCollection.Find(i => i.Id == itemId && i.UserId == userId).FirstOrDefaultAsync();
            if (item == null) return null;

            var category = await _categoriesCollection.Find(c => c.Id == item.CategoryId).FirstOrDefaultAsync();
            var user = await _usersCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();

            return _mapper.ToReadDto(
                item,
                category?.CategoryName ?? "Unknown Category"
                );
        }

        public async Task<ReadItemDto> CreateAsync(CreateItemDto createDto, string userId)
        {
            var category = await _categoriesCollection.Find(c => c.CategoryName == createDto.CategoryName && c.UserId == userId).FirstOrDefaultAsync();
            if (category == null)
            {
                category = new Category
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    CategoryName = createDto.CategoryName,
                    UserId = userId
                };
                await _categoriesCollection.InsertOneAsync(category);
            }

            var user = await _usersCollection.Find(x => x.Id == userId).FirstOrDefaultAsync();
            if (user == null)
                throw new ArgumentException("User not found");

            var item = _mapper.ToEntity(createDto, category.Id, user.Id);
            await _itemsCollection.InsertOneAsync(item);

            return _mapper.ToReadDto(item, category.CategoryName);
        }

        public async Task<ReadItemDto?> UpdateAsync(string id, UpdateItemDto updateDto, string userId)
        {
            var existingItem = await _itemsCollection.Find(i => i.Id == id && i.UserId == userId).FirstOrDefaultAsync();
            if (existingItem == null) return null;

            var category = await _categoriesCollection.Find(c => c.CategoryName == updateDto.CategoryName && c.UserId == userId).FirstOrDefaultAsync();
            if (category == null)
                throw new ArgumentException($"Category '{updateDto.CategoryName}' not found");

            var user = await _usersCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();

            _mapper.UpdateEntity(updateDto, existingItem, category.Id, user.Id);

            await _itemsCollection.ReplaceOneAsync(i => i.Id == id && i.UserId == userId, existingItem);

            return _mapper.ToReadDto(existingItem, category.CategoryName);
        }


        public async Task<bool> RemoveAsync(string id, string userId)
        {
            var result = await _itemsCollection.DeleteOneAsync(x => x.Id == id && x.UserId == userId);
            return result.DeletedCount > 0;
        }
    }
}
