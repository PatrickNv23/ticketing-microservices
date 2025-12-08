namespace Ticketing.Query.Domain.Abstractions;

public interface IUnitOfWork
{
    IGenericRepository<TEntity> RepositoryGeneric<TEntity>() where TEntity : class;
    Task<int> CompleteAsync();
}