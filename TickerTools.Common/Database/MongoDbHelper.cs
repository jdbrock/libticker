using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TickerTools.Common
{
    public static class MongoDbHelper
    {
        // ===========================================================================
        // = Private Fields
        // ===========================================================================
        
        private static string _connectionString;
        private static bool _initialized;

        // ===========================================================================
        // = Public Methods
        // ===========================================================================
        
        public static void Initialize(string connectionString)
        {
            _connectionString = connectionString;
            _initialized = true;
        }

        public static async Task Upsert<TData>(IMongoCollection<TData> collection, TData document, String keyName, String keyValue)
        {
            var options = new UpdateOptions { IsUpsert = true };
            var filter = new BsonDocument { { keyName, keyValue } };

            await collection.ReplaceOneAsync(filter, document, options);
        }

        public static async Task Insert<TData>(IMongoCollection<TData> collection, TData document)
        {
            await collection.InsertOneAsync(document);
        }

        public static async Task<TData> Select<TData>(IMongoCollection<TData> collection, String keyName, String keyValue)
            where TData : class
        {
            var filter = new BsonDocument { { keyName, keyValue } };

            var results = await collection.FindAsync<TData>(filter);
            var result = await results.FirstOrDefaultAsync();

            return result;
        }

        public static IMongoCollection<T> GetCollection<T>(String name)
        {
            CheckInitialized();

            var client = new MongoClient(_connectionString);

            var db = _connectionString.Contains("edintaps-dev")
                ? client.GetDatabase("edintaps-dev")
                : client.GetDatabase("edintaps");

            return db.GetCollection<T>(name);
        }

        // ===========================================================================
        // = Private Methods
        // ===========================================================================
        
        private static void CheckInitialized()
        {
            if (!_initialized)
                throw new ApplicationException("MongoDB helper has not been initialized.");
        }
    }
}
