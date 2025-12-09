using Common.Core.Events;

namespace Ticketing.Query.Domain.Abstractions;

// procesa todos los eventos, y lo envía ese mensaje a su respectivo handler
public interface IEventHandler
{
    // métodos genéricos
    Task On(TicketCreatedEvent @event);
    Task On(TicketUpdatedEvent @event);
}