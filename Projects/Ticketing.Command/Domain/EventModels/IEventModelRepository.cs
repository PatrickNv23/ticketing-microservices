using Ticketing.Command.Domain.EventModels;

namespace Ticketing.Command.Domain.Abstracts;

public interface IEventModelRepository : IMongoRepository<EventModel>
{
}