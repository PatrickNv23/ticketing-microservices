using Common.Core.Events;
using MongoDB.Driver;
using Ticketing.Command.Domain.Abstracts;
using Ticketing.Command.Domain.EventModels;

namespace Ticketing.Command.Infrastructure.Persistence;

public class EventStore(IEventModelRepository eventModelRepository) : IEventStore 
{
    public async Task<List<BaseEvent>> GetEventsAsync(string aggregateId, CancellationToken cancellationToken)
    {
        var eventStream = await eventModelRepository
            .FilterByAsync(doc => doc.AggregateIdentifier == aggregateId, cancellationToken);

        if (eventStream is null || eventStream.Count() == 0)
        {
            throw new Exception("No events found");
        }

        return eventStream.OrderBy(x => x.Version).Select(x => x.EventData).ToList();
    }

    public async Task SaveEventAsync(string aggregateId, IEnumerable<BaseEvent> events, int expectedVersion, CancellationToken cancellationToken)
    {
        var eventStream = await eventModelRepository
            .FilterByAsync(doc => doc.AggregateIdentifier == aggregateId, cancellationToken);

        if (eventStream.Any() && expectedVersion != -1 && eventStream.Last().Version != expectedVersion)
        {
            throw new Exception("Concurrency error");
        }

        var version = expectedVersion;

        foreach (var @event in events)
        {
            version++;
            @event.Version = version;
            var eventType = @event.GetType().Name;
            var eventModel = new EventModel
            {
                Timestamp = DateTime.UtcNow,
                AggregateIdentifier = aggregateId,
                Version = version,
                EventType = eventType,
                EventData = @event
            };
            await AddEventStore(eventModel, cancellationToken);
        }
    }

    private async Task AddEventStore(EventModel eventModel, CancellationToken cancellationToken)
    {
        IClientSessionHandle session = await eventModelRepository.BeginSessionAsync(cancellationToken);
        try
        {
            eventModelRepository.BeginTransaction(session);
            await eventModelRepository.InsertOneAsync(eventModel, session, cancellationToken);
            await eventModelRepository.CommitTransactionAsync(session, cancellationToken);
            eventModelRepository.DisposeSession(session);
        }
        catch (Exception e)
        {
            await eventModelRepository.RollbackTransactionAsync(session, cancellationToken);
            eventModelRepository.DisposeSession(session);
        }
    }
}