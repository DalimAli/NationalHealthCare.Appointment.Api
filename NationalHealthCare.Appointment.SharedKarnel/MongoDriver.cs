using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace SampleConnectionTest.Services;

/// <summary>
/// Service for testing MongoDB connectivity
/// </summary>
public interface IMongoDBConnectionService
{
    /// <summary>
    /// Check connectivity to MongoDB asynchronously
    /// </summary>
    /// <param name="connectionString">The MongoDB connection string</param>
    /// <returns>Connection test result</returns>
    Task<MongoDBConnectionResult> CheckConnectionAsync(string connectionString);
}

/// <summary>
/// Implementation of MongoDB connection service
/// </summary>
public class MongoDBConnectionService : IMongoDBConnectionService
{
    private readonly ILogger<MongoDBConnectionService> _logger;

    /// <summary>
    /// Initializes a new instance of the MongoDBConnectionService class
    /// </summary>
    /// <param name="logger">The logger instance</param>
    public MongoDBConnectionService(ILogger<MongoDBConnectionService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Check connectivity to MongoDB asynchronously
    /// </summary>
    /// <param name="connectionString">The MongoDB connection string in MongoDB+SRV or standard MongoDB format</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains connection details.</returns>
    public async Task<MongoDBConnectionResult> CheckConnectionAsync(string connectionString)
    {
        var result = new MongoDBConnectionResult();

        try
        {
            _logger.LogInformation("Attempting to connect to MongoDB with connection string: {ConnectionString}",
                MaskConnectionString(connectionString));

            var client = new MongoClient(connectionString);

            // Attempt to ping the server to verify connectivity
            var adminDatabase = client.GetDatabase("admin");
            var pingCommand = new MongoDB.Bson.BsonDocument("ping", 1);
            await adminDatabase.RunCommandAsync<MongoDB.Bson.BsonDocument>(pingCommand);

            result.IsConnected = true;
            result.ConnectionStatus = "Connected successfully";
            result.Timestamp = DateTime.UtcNow;

            _logger.LogInformation("Successfully connected to MongoDB");

            // Get server information
            try
            {
                var serverInfo = client.GetDatabase("admin").RunCommand<MongoDB.Bson.BsonDocument>(
                    new MongoDB.Bson.BsonDocument("serverStatus", 1));

                if (serverInfo != null)
                {
                    result.ServerVersion = client.Cluster.Description.Servers.FirstOrDefault()?.Version?.ToString() ?? "Unknown";
                    result.ServerStatus = "Running";
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not retrieve detailed server information");
                result.ServerVersion = "Unknown";
                result.ServerStatus = "Connected but could not retrieve server details";
            }
        }
        catch (Exception ex)
        {
            result.IsConnected = false;
            result.ConnectionStatus = $"Connection failed: {ex.Message}";
            result.Timestamp = DateTime.UtcNow;
            result.ErrorDetails = ex.GetType().Name;
            result.ServerStatus = "Unavailable";

            _logger.LogError(ex, "Failed to connect to MongoDB");
        }

        return result;
    }

    private string MaskConnectionString(string connectionString)
    {
        // Mask sensitive information in the connection string for logging
        try
        {
            if (connectionString.Contains("mongodb+srv://"))
            {
                var startIdx = connectionString.IndexOf("://");
                var atIdx = connectionString.IndexOf("@");

                if (startIdx != -1 && atIdx != -1)
                {
                    var masked = connectionString[..(startIdx + 3)] + "***:***" + connectionString[atIdx..];
                    return masked;
                }
            }
            return "***MASKED***";
        }
        catch
        {
            return "***INVALID***";
        }
    }
}

/// <summary>
/// Result of a MongoDB connectivity check
/// </summary>
public class MongoDBConnectionResult
{
    /// <summary>
    /// Indicates whether the connection was successful
    /// </summary>
    public bool IsConnected { get; set; }

    /// <summary>
    /// Status message describing the connection result
    /// </summary>
    public string ConnectionStatus { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp of when the connection was tested
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// MongoDB server version if successfully connected
    /// </summary>
    public string? ServerVersion { get; set; }

    /// <summary>
    /// MongoDB server status
    /// </summary>
    public string? ServerStatus { get; set; }

    /// <summary>
    /// Error details if connection failed
    /// </summary>
    public string? ErrorDetails { get; set; }
}
