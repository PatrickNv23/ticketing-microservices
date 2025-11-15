using FluentValidation;
using MediatR;

namespace Ticketing.Command.Features.Tickets;

public class TicketCreate
{
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

    public sealed class TicketCreateCommandHandler : IRequestHandler<TicketCreateCommand, bool>
    {
        public Task<bool> Handle(TicketCreateCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}