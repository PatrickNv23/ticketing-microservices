using Ticketing.Query.Domain.Abstractions;

namespace Ticketing.Query.Domain.Employees;

public interface IEmployeeRepository : IGenericRepository<Employee>
{
    Task<Employee?> GetByUsernameAsync(string username);
}