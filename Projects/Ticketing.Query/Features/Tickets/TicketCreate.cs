using MediatR;
using Ticketing.Query.Domain.Abstractions;
using Ticketing.Query.Domain.Employees;
using Ticketing.Query.Domain.Tickets;
using Ticketing.Query.Domain.TicketTypes;

namespace Ticketing.Query.Features.Tickets;

public sealed class TicketCreate
{
    public record TicketCreateCommand(string Id, string Username, int TicketType, string DetailError)
        : IRequest<string>;

    public class TicketCreateCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<TicketCreateCommand, string>
    {
        public async Task<string> Handle(TicketCreateCommand request, CancellationToken cancellationToken)
        {
            // insertar data de employee
            var employee = await unitOfWork.EmployeeRepository.GetByUsernameAsync(request.Username);

            if (employee is null)
            {
                employee = Employee.Create(string.Empty, string.Empty, null!, request.Username);
                unitOfWork.EmployeeRepository.AddEntity(employee);
            }

            // insertar data de ticket
            var ticket = Ticket.Create(new Guid(request.Id), request.DetailError, TicketType.Create(request.TicketType));
            unitOfWork.RepositoryGeneric<Ticket>().AddEntity(ticket);

            // insertar data de ticket_employee
            var ticketEmployee = TicketEmployee.Create(ticket, employee);
            unitOfWork.RepositoryGeneric<TicketEmployee>().AddEntity(ticketEmployee);
            
            // insertar a la BD y guardar cambios
            await unitOfWork.Complete();

            return Convert.ToString(ticket.Id)!;
        }
    }
}