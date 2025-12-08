namespace Ticketing.Query.Domain.Abstractions;

public interface IGenericRepository<T> where T : class
{
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    void AddEntity(T entity);
    void UpdateEntity(T entity);
    void DeleteEntity(T entity);
}