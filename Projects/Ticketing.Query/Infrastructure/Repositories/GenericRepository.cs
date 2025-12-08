using Microsoft.EntityFrameworkCore;
using Ticketing.Query.Domain.Abstractions;
using Ticketing.Query.Infrastructure.Persistence;

namespace Ticketing.Query.Infrastructure.Repositories;

public class GenericRepository<T>(TicketDbContext context) : IGenericRepository<T> where T : class
{
    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await context.Set<T>().ToListAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await context.Set<T>().FindAsync();
    }

    public void AddEntity(T entity)
    {
        context.Set<T>().Add(entity);
    }

    public void UpdateEntity(T entity)
    {
        context.Set<T>().Attach(entity);
        context.Entry(entity).State = EntityState.Modified;
    }

    public void DeleteEntity(T entity)
    {
        context.Set<T>().Remove(entity);
    }
}