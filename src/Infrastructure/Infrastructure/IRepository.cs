namespace Infrastructure;

public interface IRepository<T> where T: BaseEntity
{
    Task<T?> Get(int id);

    Task<ICollection<T>> GetAll();

    Task Update(T entity);

    Task Insert(T entity);
}
