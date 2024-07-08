
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TaxCalculator.Model.Entities;

namespace TaxCalculator.Model.DbService
{
    public class TaxDbService
    {
        private readonly IMongoCollection<Tax> _taxesCollection;

        public TaxDbService(
            IOptions<TaxesStoreDatabaseSettings> taxesStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                taxesStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                taxesStoreDatabaseSettings.Value.DatabaseName);

            _taxesCollection = mongoDatabase.GetCollection<Tax>(
                taxesStoreDatabaseSettings.Value.TaxesCollectionName);
        }

        public async Task<List<Tax>> GetAsync() =>
            await _taxesCollection.Find(_ => true).ToListAsync();

        public async Task<Tax?> GetAsync(string id) =>
            await _taxesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Tax newTax) =>
            await _taxesCollection.InsertOneAsync(newTax);

        public async Task UpdateAsync(string id, Tax updatedTax) =>
            await _taxesCollection.ReplaceOneAsync(x => x.Id == id, updatedTax);

        public async Task RemoveAsync(string id) =>
            await _taxesCollection.DeleteOneAsync(x => x.Id == id);
    }
}
