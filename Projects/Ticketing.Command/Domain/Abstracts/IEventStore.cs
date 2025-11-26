using Common.Core.Events;

namespace Ticketing.Command.Domain.Abstracts;

public interface IEventStore
{
    // listar elementos en BD del aggregate
    Task<List<BaseEvent>> GetEventsAsync(string aggregateId, CancellationToken cancellationToken);
    
    // almacenar elementos que no están en BD
    Task SaveEventAsync(string aggregateId, IEnumerable<BaseEvent> events, int expectedVersion, CancellationToken cancellationToken);
}