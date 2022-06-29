using System.Data;
using Dapper;
using Infrastructure.Interfaces;

namespace Infrastructure;

public abstract class GenericRepository<T> : IRepository<T>
    where T : BaseEntity
{
    private readonly IUnitOfWork unitOfWork;
    private readonly string tableName;

    protected GenericRepository(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
        tableName = typeof(T).Name.ToLower() + "s";
    }

    public virtual async Task<T?> Get(int id)
    {
        var query = $"SELECT * FROM {tableName} WHERE id=@id";

        var entity = await unitOfWork.Connection.QueryFirstOrDefaultAsync<T>(query, new { id }, unitOfWork.Transaction);

        return entity;
    }

    public virtual async Task<IReadOnlyCollection<T>> GetAll()
    {
        var query = $"SELECT * FROM {tableName}";

        IEnumerable<T>? entities = await unitOfWork.Connection.QueryAsync<T>(query, transaction: unitOfWork.Transaction);

        return entities.ToList().AsReadOnly();
    }

    // Generic methods are not written for save time
    public virtual Task Update(T entity) => throw new NotImplementedException();

    public virtual Task Insert(T entity) => throw new NotImplementedException();

    public virtual Task InsertBulk(IEnumerable<T> entities) => throw new NotImplementedException();
}
