using AutoMapper;
using Common.Core.Events;
using FluentValidation;
using MediatR;
using MongoDB.Driver;
using Ticketing.Command.Domain.Abstracts;
using Ticketing.Command.Domain.EventModels;
using Ticketing.Command.Features.Apis;

namespace Ticketing.Command.Features.Tickets;

public sealed class TicketCreate : IMinimalApi // sealed es que no permita herencia
{
    public void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapPost("/api/ticket", async (
            TicketCreateRequest ticketCreateRequest,
            IMediator mediator,
            CancellationToken cancellationToken
        ) =>
        {
            var id = Guid.CreateVersion7(DateTimeOffset.UtcNow).ToString();
            var command = new TicketCreateCommand(id, ticketCreateRequest);
            var result = await mediator.Send(command, cancellationToken);
            return Results.Ok(result);
        });
    }
    
    public sealed class TicketCreateRequest(string userName, string typeError, string detailError)
    {
        public string UserName { get; set; } = userName;
        public string TypeError { get; set; } = typeError;
        public string DetailError { get; set; } = detailError;
    }
    
    public record TicketCreateCommand(string Id, TicketCreateRequest ticketCreateRequest) 
        : IRequest<bool>;

    public class TicketCreateCommandValidator : AbstractValidator<TicketCreateCommand>
    {
        public TicketCreateCommandValidator()
        {
            RuleFor(x => x.ticketCreateRequest).SetValidator(new TicketCreateRequestValidator());
            RuleFor(x => x.Id).NotEmpty().WithMessage("The event id is required");
        }
    }

    private class TicketCreateRequestValidator : AbstractValidator<TicketCreateRequest>
    {
        public TicketCreateRequestValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("El nombre de usuario es obligatorio");
            RuleFor(x => x.DetailError).NotEmpty().WithMessage("El detalle de error es obligatorio");
        }
    }

    public sealed class TicketCreateCommandHandler(
        IEventModelRepository eventModelRepository,
        IMapper mapper
        ) : IRequestHandler<TicketCreateCommand, bool>
    {
        public async Task<bool> Handle(TicketCreateCommand request, CancellationToken cancellationToken)
        {
            var ticketEventData = mapper.Map<TicketCreatedEvent>(request.ticketCreateRequest);

            var eventModel = new EventModel
            {
                Timestamp = DateTime.UtcNow,
                AggregateIdentifier = Guid.CreateVersion7(DateTimeOffset.UtcNow).ToString(),
                AggregateType = "TicketAggregate",
                Version = 1,
                EventType = "TicketCreatedEvent",
                EventData = ticketEventData
            };
            
            var session = await eventModelRepository.BeginSessionAsync(cancellationToken);
            
            try
            {
                eventModelRepository.BeginTransaction(session);
                await eventModelRepository.InsertOneAsync(eventModel, session, cancellationToken);
                await eventModelRepository.CommitTransactionAsync(session, cancellationToken);
                eventModelRepository.DisposeSession(session);
                return true;
            }
            catch (Exception e)
            {
                await eventModelRepository.RollbackTransactionAsync(session, cancellationToken);
                eventModelRepository.DisposeSession(session);
                return false;
            }
        }
    }
}