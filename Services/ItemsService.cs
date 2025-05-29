using ToDoListApi.Models;
using ToDoListApi.Mappers;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

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

        // public async Task<List<Item>> GetAsync() =>
        //     await _itemsCollection.Find(_ => true).ToListAsync();

        public async Task<List<ReadItemDto>> GetAsync()
        {
            var items = await _itemsCollection.Find(_ => true).ToListAsync();
            var readDtos = new List<ReadItemDto>();

            foreach (var item in items)
            {
                var category = await _categoriesCollection.Find(x => x.Id == item.CategoryId).FirstOrDefaultAsync();
                var user = await _usersCollection.Find(x => x.Id == item.UserId).FirstOrDefaultAsync();

                var readDto = _mapper.ToReadDto(
                    item,
                    category?.CategoryName ?? "Unknown Category",
                    user?.UserName ?? "Unknown User"
                );

                readDtos.Add(readDto);
            }

            return readDtos;
        }

        // public async Task<Item?> GetAsync(string id) =>
        //     await _itemsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<ReadItemDto?> GetAsync(string id)
        {
            var item = await _itemsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (item == null) return null;

            var category = await _categoriesCollection.Find(x => x.Id == item.CategoryId).FirstOrDefaultAsync();
            var user = await _usersCollection.Find(x => x.Id == item.UserId).FirstOrDefaultAsync();

            return _mapper.ToReadDto(
                item,
                category?.CategoryName ?? "Unknown Category",
                user?.UserName ?? "Unknown User"
            );
        }

        // public async Task CreateAsync(Item newItem) =>
        //     await _itemsCollection.InsertOneAsync(newItem);

        // public async Task CreateAsync(CreateItemDto dto, string categoryId, string userId)
        // {
        //     var item = _mapper.ToEntity(dto, categoryId, userId);
        //     await _itemsCollection.InsertOneAsync(item);
        // }

        public async Task<ReadItemDto> CreateAsync(CreateItemDto createDto)
        {
            var category = await _categoriesCollection.Find(x => x.CategoryName == createDto.CategoryName).FirstOrDefaultAsync();
            if (category == null)
                throw new ArgumentException($"Category '{createDto.CategoryName}' not found");

            var user = await _usersCollection.Find(x => x.UserName == createDto.Username).FirstOrDefaultAsync();
            if (user == null)
                throw new ArgumentException($"User '{createDto.Username}' not found");

            var item = _mapper.ToEntity(createDto, category.Id, user.Id);

            await _itemsCollection.InsertOneAsync(item);

            return _mapper.ToReadDto(item, category.CategoryName, user.UserName);
        }

        // public async Task UpdateAsync(string id, Item updatedItem) =>
        //     await _itemsCollection.ReplaceOneAsync(x => x.Id == id, updatedItem);

        public async Task<ReadItemDto?> UpdateAsync(string id, UpdateItemDto updateDto)
        {
            var existingItem = await _itemsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (existingItem == null) return null;

            var category = await _categoriesCollection.Find(x => x.CategoryName == updateDto.CategoryName).FirstOrDefaultAsync();
            if (category == null)
                throw new ArgumentException($"Category '{updateDto.CategoryName}' not found");

            var user = await _usersCollection.Find(x => x.UserName == updateDto.Username).FirstOrDefaultAsync();
            if (user == null)
                throw new ArgumentException($"User '{updateDto.Username}' not found");

            _mapper.UpdateEntity(updateDto, existingItem, category.Id, user.Id);

            await _itemsCollection.ReplaceOneAsync(x => x.Id == id, existingItem);

            return _mapper.ToReadDto(existingItem, category.CategoryName, user.UserName);
        }


        public async Task RemoveAsync(string id) =>
            await _itemsCollection.DeleteOneAsync(x => x.Id == id);
    }
}
