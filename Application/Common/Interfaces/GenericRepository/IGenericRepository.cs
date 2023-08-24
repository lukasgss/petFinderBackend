namespace Application.Common.Interfaces.GenericRepository;

public interface IGenericRepository<T>
{
    Task<T?> GetByIdAsync(object id);
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task CommitAsync();
}