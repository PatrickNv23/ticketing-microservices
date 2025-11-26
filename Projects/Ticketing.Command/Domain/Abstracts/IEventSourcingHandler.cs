namespace Ticketing.Command.Domain.Abstracts;

public interface IEventSourcingHandler<T>
{
    // devolver último estado del aggregate
    Task<T> GetByIdAsync(string aggregateId, CancellationToken cancellationToken);
    
    // disparar evento de persistencia y almacenar el aggregate
    Task SaveAsync(AggregateRoot aggregate, CancellationToken cancellationToken);
}