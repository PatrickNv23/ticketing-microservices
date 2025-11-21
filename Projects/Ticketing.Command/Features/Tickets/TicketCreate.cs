using AutoMapper;
using Common.Core.Events;
using FluentValidation;
using MediatR;
using MongoDB.Driver;
using Ticketing.Command.Domain.Abstracts;
using Ticketing.Command.Domain.EventModels;
using Ticketing.Command.Features.Apis;

namespace Ticketing.Command.Features.Tickets;

public class TicketCreate : IMinimalApi
{
    public void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        throw new NotImplementedException();
    }
    
    public sealed class TicketCreateRequest(string userName, string typeError, string detailError)
    {
        public string UserName { get; set; } = userName;
        public string TypeError { get; set; } = typeError;
        public string DetailError { get; set; } = detailError;
    }
    
    public record TicketCreateCommand(TicketCreateRequest ticketCreateRequest) 
        : IRequest<bool>;

    public class TicketCreateCommandValidator : AbstractValidator<TicketCreateCommand>
    {
        public TicketCreateCommandValidator()
        {
            RuleFor(x => x.ticketCreateRequest).SetValidator(new TicketCreateRequestValidator());
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