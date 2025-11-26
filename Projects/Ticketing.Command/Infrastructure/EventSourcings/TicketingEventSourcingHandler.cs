using Ticketing.Command.Application.Aggregates;
using Ticketing.Command.Domain.Abstracts;

namespace Ticketing.Command.Infrastructure.EventSourcings;

public class TicketingEventSourcingHandler(IEventStore eventStore) : IEventSourcingHandler<TicketAggregate> // pasarle el aggregate
{
    public async Task<TicketAggregate> GetByIdAsync(string aggregateId, CancellationToken cancellationToken)
    {
        var aggregate = new TicketAggregate();
        var events = await eventStore.GetEventsAsync(aggregateId, cancellationToken);
        
        if(events is null || events.Count == 0) return aggregate;
        aggregate.ReplayEvents(events); // sobrecargar los eventos en memoria
        
        // buscar la última version
        aggregate.Version = events.Select(x => x.Version).Max();
        
        return aggregate;
    }

    public async Task SaveAsync(AggregateRoot aggregate, CancellationToken cancellationToken)
    {
        await eventStore.SaveEventAsync(
            aggregate.Id, 
            aggregate.GetUncommittedChanges(), 
            aggregate.Version,
            cancellationToken);
        aggregate.MarkChangesAsCommitted();
    }
}