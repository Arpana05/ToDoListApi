using ToDoListApi.Models;
using ToDoListApi.DTOs;
using ToDoListApi.Mappers;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ToDoListApi.Services
{
    public class CategoriesService
    {
        private readonly IMongoCollection<Category> _categoriesCollection;
        private readonly IMongoCollection<Item> _itemsCollection;
        private readonly CategoryMapper _mapper;

        public CategoriesService(
            IOptions<ToDoListDatabaseSettings> todoListdatabaseSettings,
            CategoryMapper mapper)
        {
            _mapper = mapper;

            var mongoClient = new MongoClient(
                todoListdatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                todoListdatabaseSettings.Value.DatabaseName);

            _categoriesCollection = mongoDatabase.GetCollection<Category>(
                todoListdatabaseSettings.Value.CategoriesCollectionName);

            _itemsCollection = mongoDatabase.GetCollection<Item>(
            todoListdatabaseSettings.Value.ItemsCollectionName);
        }


        public async Task<List<ReadCategoryDto>> GetAsync(string userId)
        {
            var categories = await _categoriesCollection.Find(c => c.UserId == userId).ToListAsync();
            return categories.Select(_mapper.ToReadDto).ToList();
        }


        public async Task<ReadCategoryDto?> GetAsync(string id, string userId)
        {
            var category = await _categoriesCollection.Find(c => c.Id == id && c.UserId == userId).FirstOrDefaultAsync();
            return category == null ? null : _mapper.ToReadDto(category);
        }


        public async Task<ReadCategoryDto> CreateAsync(CreateCategoryDto createDto, string userId)
        {
            var existingCategory = await _categoriesCollection.Find(c => c.CategoryName == createDto.CategoryName && c.UserId == userId).FirstOrDefaultAsync();
            if (existingCategory != null)
                throw new ArgumentException($"Category '{createDto.CategoryName}' already exists");

            var category = _mapper.ToEntity(createDto, userId);
            category.UserId = userId;

            await _categoriesCollection.InsertOneAsync(category);

            return _mapper.ToReadDto(category);
        }


        public async Task<ReadCategoryDto?> UpdateAsync(string id, UpdateCategoryDto updateDto, string userId)
        {
            var existingCategory = await _categoriesCollection.Find(c => c.Id == id && c.UserId == userId).FirstOrDefaultAsync();
            if (existingCategory == null) return null;

            var duplicateCategory = await _categoriesCollection.Find(c => c.CategoryName == updateDto.CategoryName && c.Id != id && c.UserId == userId).FirstOrDefaultAsync();
            if (duplicateCategory != null)
                throw new ArgumentException($"Category '{updateDto.CategoryName}' already exists");

            _mapper.UpdateEntity(updateDto, existingCategory);

            await _categoriesCollection.ReplaceOneAsync(c => c.Id == id && c.UserId == userId, existingCategory);

            return _mapper.ToReadDto(existingCategory);
        }

        public async Task RemoveAsync(string id, string userId)
        {
            var categoryInUse = await _itemsCollection.Find(i => i.CategoryId == id).AnyAsync();

            if (categoryInUse)
                throw new InvalidOperationException("Category cannot be deleted because it is used by one or more items.");

            await _categoriesCollection.DeleteOneAsync(c => c.Id == id && c.UserId == userId);
        }
    }
}
