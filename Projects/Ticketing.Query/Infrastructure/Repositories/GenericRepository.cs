using Microsoft.EntityFrameworkCore;
using Ticketing.Query.Domain.Abstractions;
using Ticketing.Query.Infrastructure.Persistence;

namespace Ticketing.Query.Infrastructure.Repositories;

public class GenericRepository<T>: IGenericRepository<T> where T : class
{
    protected readonly TicketDbContext _context;

    public GenericRepository(TicketDbContext context)
    {
        this._context = context;
    }
    
    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _context.Set<T>().FindAsync();
    }

    public void AddEntity(T entity)
    {
        _context.Set<T>().Add(entity);
    }

    public void UpdateEntity(T entity)
    {
        _context.Set<T>().Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    public void DeleteEntity(T entity)
    {
        _context.Set<T>().Remove(entity);
    }
}