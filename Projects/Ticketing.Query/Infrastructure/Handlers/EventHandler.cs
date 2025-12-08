using Common.Core.Events;
using MediatR;
using Ticketing.Query.Domain.Abstractions;
using Ticketing.Query.Features.Tickets;

namespace Ticketing.Query.Infrastructure.Handlers;

public class EventHandler(IMediator mediator) : IEventHandler       
{
    public async Task On(TicketCreatedEvent @event)
    {
        // crear un command para colocar la data del mensaje del evento
        var command = new TicketCreate.CreateTicketCommand
        (
            @event.Id,
            @event.UserName,
            @event.TypeError,
            @event.DetailError
        );
        
        await mediator.Send(command);
    }
}