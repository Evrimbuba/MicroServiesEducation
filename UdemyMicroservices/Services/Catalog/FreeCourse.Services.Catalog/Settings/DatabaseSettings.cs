namespace FreeCourse.Services.Catalog.Settings
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string CourseCollectionName { get; set; }
        public string CategoryCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
/*
    public interface ICatalogContext
    {
    IMongoCollection<Product> Products { get; }
    }

    public class CatalogContext : ICatalogContext
     {
     public CatalogContext(ICatalogDatabaseSettings settings)
     {
     var client = new MongoClient(settings.ConnectionString);
     var database = client.GetDatabase(settings.DatabaseName);
    Products = database.GetCollection<Product>(settings.CollectionName);
     CatalogContextSeed.SeedData(Products);
     }
    public IMongoCollection<Product> Products { get; }
     }
 */