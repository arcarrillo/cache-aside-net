namespace cacheaside.library.implementations.redis;
public record RedisConnectionConfiguration
{

    public RedisConnectionConfiguration(string connectionString, int connectionTimeout)
    {
        ConnectionString = connectionString;
        ConnectionTimeout = connectionTimeout;
    }

    public string ConnectionString { get; private set; }
    public int ConnectionTimeout { get; private set; }
}
