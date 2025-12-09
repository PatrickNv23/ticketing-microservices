using Ticketing.Query.Domain.Employees;

namespace Ticketing.Query.Domain.Abstractions;

public interface IUnitOfWork
{
    IEmployeeRepository EmployeeRepository { get; }
    IGenericRepository<TEntity> RepositoryGeneric<TEntity>() where TEntity : class;
    Task<int> Complete();
}