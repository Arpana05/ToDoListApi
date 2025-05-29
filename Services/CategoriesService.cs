using ToDoListApi.Models;
using ToDoListApi.Mappers;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ToDoListApi.Services
{
    public class CategoriesService
    {
        private readonly IMongoCollection<Category> _categoriesCollection;
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
        }

        // public async Task<List<Category>> GetAsync() =>
        //     await _categoriesCollection.Find(_ => true).ToListAsync();

        public async Task<List<ReadCategoryDto>> GetAsync()
        {
            var categories = await _categoriesCollection.Find(_ => true).ToListAsync();
            return categories.Select(_mapper.ToReadDto).ToList();
        }

        // public async Task<Category?> GetAsync(string id) =>
        //     await _categoriesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<ReadCategoryDto?> GetAsync(string id)
        {
            var category = await _categoriesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            return category == null ? null : _mapper.ToReadDto(category);
        }

        // public async Task CreateAsync(Category newCategory) =>
        //     await _categoriesCollection.InsertOneAsync(newCategory);

         public async Task<ReadCategoryDto> CreateAsync(CreateCategoryDto createDto)
        {
            var existingCategory = await _categoriesCollection.Find(x => x.CategoryName == createDto.CategoryName).FirstOrDefaultAsync();
            if (existingCategory != null)
                throw new ArgumentException($"Category '{createDto.CategoryName}' already exists");

            var category = _mapper.ToEntity(createDto);
            
            await _categoriesCollection.InsertOneAsync(category);

            return _mapper.ToReadDto(category);
        }

        // public async Task UpdateAsync(string id, Category updatedCategory) =>
        //     await _categoriesCollection.ReplaceOneAsync(x => x.Id == id, updatedCategory);

        public async Task<ReadCategoryDto?> UpdateAsync(string id, UpdateCategoryDto updateDto)
        {
            var existingCategory = await _categoriesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (existingCategory == null) return null;

            var duplicateCategory = await _categoriesCollection.Find(x => x.CategoryName == updateDto.CategoryName && x.Id != id).FirstOrDefaultAsync();
            if (duplicateCategory != null)
                throw new ArgumentException($"Category '{updateDto.CategoryName}' already exists");

            _mapper.UpdateEntity(updateDto, existingCategory);

            await _categoriesCollection.ReplaceOneAsync(x => x.Id == id, existingCategory);

            return _mapper.ToReadDto(existingCategory);
        }

        public async Task RemoveAsync(string id) =>
            await _categoriesCollection.DeleteOneAsync(x => x.Id == id);
    }
}
