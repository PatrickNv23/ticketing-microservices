using Common.Core.Events;
using Ticketing.Command.Domain.Abstracts;
using static Ticketing.Command.Features.Tickets.TicketCreate;

namespace Ticketing.Command.Application.Aggregates;

public class TicketAggregate : AggregateRoot
{
    public bool Active { get; set; }
    public TicketAggregate(){}
    public TicketAggregate(TicketCreateCommand command)
    {
        var ticketCreatedEvent = new TicketCreatedEvent
        {
            Id = command.Id,
            UserName = command.ticketCreateRequest.UserName,
            TypeError = command.ticketCreateRequest.TypeError,
            DetailError = command.ticketCreateRequest.DetailError
        };
        
        RaiseEvent(ticketCreatedEvent); // del aggregate root
    }

    public void Apply(TicketCreatedEvent @event)
    {
        _id = @event.Id;
        Active = true;
    }
}