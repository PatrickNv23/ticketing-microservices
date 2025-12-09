using Common.Core.Events;
using MediatR;
using Ticketing.Query.Domain.Abstractions;
using Ticketing.Query.Features.Tickets;

namespace Ticketing.Query.Infrastructure.Handlers;

public class EventHandler : IEventHandler
{
    private readonly IMediator _mediator;

    public EventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task On(TicketCreatedEvent @event)
    {
        var command = new TicketCreate.TicketCreateCommand(
            @event.Id,
            @event.UserName,
            @event.TypeError,
            @event.DetailError
        );

        await _mediator.Send(command);
    }

    public Task On(TicketUpdatedEvent @event)
    {
        throw new NotImplementedException();
    }
}
