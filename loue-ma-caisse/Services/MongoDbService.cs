using MongoDB.Driver;

namespace loue_ma_caisse.Services;

public class MongoDbService
{
    private readonly IMongoDatabase _database;

    public MongoDbService(IConfiguration configuration)
    {
        // Récupérer les variables d'environnement
        var connectionString = Environment.GetEnvironmentVariable("MongoDB__ConnectionString") 
            ?? configuration["MongoDbSettings:ConnectionString"];
        
        var databaseName = Environment.GetEnvironmentVariable("MongoDB__DatabaseName") 
            ?? configuration["MongoDbSettings:DatabaseName"];

        // Vérifier si les variables nécessaires sont définies
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "La chaîne de connexion MongoDB n'est pas définie. " +
                "Veuillez définir la variable d'environnement 'MongoDB__ConnectionString' ou la configuration 'MongoDbSettings:ConnectionString'."
            );
        }

        if (string.IsNullOrEmpty(databaseName))
        {
            throw new InvalidOperationException(
                "Le nom de la base de données MongoDB n'est pas défini. " +
                "Veuillez définir la variable d'environnement 'MongoDB__DatabaseName' ou la configuration 'MongoDbSettings:DatabaseName'."
            );
        }

        try
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Erreur lors de la connexion à MongoDB. Vérifiez vos paramètres de connexion.", ex
            );
        }
    }

    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return _database.GetCollection<T>(name);
    }
}