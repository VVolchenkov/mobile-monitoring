using System.Data;
using Dapper;
using Infrastructure.Interfaces;

namespace Infrastructure;

public abstract class GenericRepository<T> : IRepository<T>
    where T : BaseEntity
{
    private readonly DataContextFactory contextFactory;
    private readonly string tableName;

    protected GenericRepository(DataContextFactory contextFactory)
    {
        this.contextFactory = contextFactory;
        tableName = typeof(T).Name.ToLower() + "s";
    }

    public virtual async Task<T?> Get(int id)
    {
        var query = $"SELECT * FROM {tableName} WHERE id=@id";

        IDbConnection connection = contextFactory.CreateConnection();
        var entity = await connection.QueryFirstOrDefaultAsync<T>(query, new { id });

        return entity;
    }

    public virtual async Task<IReadOnlyCollection<T>> GetAll()
    {
        var query = $"SELECT * FROM {tableName}";

        IDbConnection connection = contextFactory.CreateConnection();
        IEnumerable<T>? entities = await connection.QueryAsync<T>(query);

        return entities.ToList().AsReadOnly();
    }

    // Generic methods are not written for save time
    public virtual Task Update(T entity) => throw new NotImplementedException();

    public virtual Task Insert(T entity) => throw new NotImplementedException();

    public virtual Task InsertBulk(IEnumerable<T> entities) => throw new NotImplementedException();
}
