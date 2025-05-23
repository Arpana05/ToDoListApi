using ToDoListApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ToDoListApi.Services
{
    public class ItemsService
    {
        private readonly IMongoCollection<Item> _itemsCollection;

        public ItemsService(
            IOptions<ToDoListDatabaseSettings> todoListdatabaseSettings)
        {
            var mongoClient = new MongoClient(
                todoListdatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                todoListdatabaseSettings.Value.DatabaseName);

            _itemsCollection = mongoDatabase.GetCollection<Item>(
                todoListdatabaseSettings.Value.ItemsCollectionName);
        }

        public async Task<List<Item>> GetAsync() =>
            await _itemsCollection.Find(_ => true).ToListAsync();

        public async Task<Item?> GetAsync(string id) =>
            await _itemsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Item newItem) =>
            await _itemsCollection.InsertOneAsync(newItem);

        public async Task UpdateAsync(string id, Item updatedItem) =>
            await _itemsCollection.ReplaceOneAsync(x => x.Id == id, updatedItem);

        public async Task RemoveAsync(string id) =>
            await _itemsCollection.DeleteOneAsync(x => x.Id == id);
    }
}
