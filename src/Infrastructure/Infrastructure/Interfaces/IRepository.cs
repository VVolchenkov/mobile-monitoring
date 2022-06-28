namespace Infrastructure.Interfaces;

public interface IRepository<T>
    where T : BaseEntity
{
    Task<T?> Get(int id);

    Task<IReadOnlyCollection<T>> GetAll();

    Task Update(T entity);

    Task Insert(T entity);

    Task InsertBulk(IEnumerable<T> entities);
}
