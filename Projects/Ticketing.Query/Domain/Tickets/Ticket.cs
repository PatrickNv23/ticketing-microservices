using Ticketing.Query.Domain.Abstractions;
using Ticketing.Query.Domain.Employees;
using Ticketing.Query.Domain.TicketTypes;

namespace Ticketing.Query.Domain.Tickets;

public class Ticket : Entity
{
    public string? Description { get; set; }
    
    // traerá el ticketType solo bajo demanda cuando yo lo necesite
    public virtual TicketType? TicketType { get; set; } // virtual para lazy loading
    public virtual ICollection<Employee> Employees { get; set; } = [];
    public virtual ICollection<TicketEmployee> TicketEmployees { get; set; } = [];
}