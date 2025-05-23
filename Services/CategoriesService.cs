using ToDoListApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ToDoListApi.Services
{
    public class CategoriesService
    {
        private readonly IMongoCollection<Category> _categoriesCollection;

        public CategoriesService(
            IOptions<ToDoListDatabaseSettings> todoListdatabaseSettings)
        {
            var mongoClient = new MongoClient(
                todoListdatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                todoListdatabaseSettings.Value.DatabaseName);

            _categoriesCollection = mongoDatabase.GetCollection<Category>(
                todoListdatabaseSettings.Value.CategoriesCollectionName);
        }

        public async Task<List<Category>> GetAsync() =>
            await _categoriesCollection.Find(_ => true).ToListAsync();

        public async Task<Category?> GetAsync(string id) =>
            await _categoriesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Category newCategory) =>
            await _categoriesCollection.InsertOneAsync(newCategory);

        public async Task UpdateAsync(string id, Category updatedCategory) =>
            await _categoriesCollection.ReplaceOneAsync(x => x.Id == id, updatedCategory);

        public async Task RemoveAsync(string id) =>
            await _categoriesCollection.DeleteOneAsync(x => x.Id == id);
    }
}
