// Infrastructure/Sql/IDbConnectionFactory.cs
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

public interface IDbConnectionFactory
{
    SqlConnection Create(string key); // "AWS" or "AOM"
}

public sealed class DbConnectionFactory : IDbConnectionFactory
{
    private readonly IConfiguration _cfg;
    public DbConnectionFactory(IConfiguration cfg) => _cfg = cfg;

    public SqlConnection Create(string key)
    {
        var cs = _cfg.GetConnectionString(key)
                 ?? _cfg[$"ConnectionStrings:{key}"];
        if (string.IsNullOrWhiteSpace(cs))
            throw new InvalidOperationException($"Missing connection string '{key}'.");
        return new SqlConnection(cs);
    }
}
