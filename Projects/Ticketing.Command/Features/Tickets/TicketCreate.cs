using FluentValidation;
using MediatR;
using Ticketing.Command.Application.Aggregates;
using Ticketing.Command.Domain.Abstracts;
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
    
    public sealed class TicketCreateRequest(string userName, int typeError, string detailError)
    {
        public string UserName { get; set; } = userName;
        public int TypeError { get; set; } = typeError;
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
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("El nombre de usuario es obligatorio")
                .EmailAddress().WithMessage("Debe ser un email");
            RuleFor(x => x.TypeError)
                .NotEmpty().WithMessage("El tipo de error es obligatorio")
                .InclusiveBetween(1,5).WithMessage("El rango de error es de 1 a 5");
            RuleFor(x => x.DetailError).NotEmpty().WithMessage("El detalle de error es obligatorio");
        }
    }

    public sealed class TicketCreateCommandHandler(
        IEventSourcingHandler<TicketAggregate> eventSourcingHandler
        ) : IRequestHandler<TicketCreateCommand, bool>
    {
        public async Task<bool> Handle(TicketCreateCommand request, CancellationToken cancellationToken)
        {
            var aggregate = new TicketAggregate(request);
            await eventSourcingHandler.SaveAsync(aggregate, cancellationToken);
            return true;
        }
    }
}