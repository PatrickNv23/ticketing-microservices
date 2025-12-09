using System.Collections;
using MediatR;
using Ticketing.Query.Domain.Abstractions;
using Ticketing.Query.Domain.Employees;
using Ticketing.Query.Domain.Tickets;
using Ticketing.Query.Infrastructure.Repositories;

namespace Ticketing.Query.Infrastructure.Persistence;

// administrador de conjunto de instancias de repositorios
public class UnitOfWork : IUnitOfWork
{
    // debe existir una colección de repositorios
    private Hashtable _repositories = new(); // esto representa la sesión dentro del IUnitOfWork
    private readonly DatabaseContextFactory _contextFactory;
    private readonly TicketDbContext _context;
    private IEmployeeRepository? _employeeRepository;
    
    public IEmployeeRepository EmployeeRepository => _employeeRepository ??= new EmployeeRepository(_context);

    public UnitOfWork(DatabaseContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
        _context = _contextFactory.CreateDbContext();
    }
    
    // crea instancia de un generic repository
    public IGenericRepository<TEntity> RepositoryGeneric<TEntity>() where TEntity : class
    {
        if(_repositories is null)
        {
            // crea instancia de esa colección
            _repositories = new Hashtable();
        }
        
        var type = typeof(TEntity).Name;

        if (!_repositories.ContainsKey(type))
        {
            var repositoryType = typeof(GenericRepository<>);
            // instanciar un tipo repository usando reflection
            var repositoryInstance = Activator
                .CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context);
            
            // agregarlo al hashtable
            _repositories.Add(type, repositoryInstance);
        }

        return (IGenericRepository<TEntity>) _repositories[type]!; // devuelve ese repository que acabo de agregar.
    }

    public async Task<int> Complete()
    {
        // confirmar cambios en memoria de EF y enviarlos a BD
        return await _context.SaveChangesAsync(); // devuelve el número de transacciones que se ejecutaron
    }
}