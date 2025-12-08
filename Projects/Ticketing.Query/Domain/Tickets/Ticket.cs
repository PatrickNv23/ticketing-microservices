using System.Diagnostics.CodeAnalysis;
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
    
    public Ticket(){}
    
    private Ticket(Guid id, string description, TicketType? ticketType) : base(id)
    {
        Description = description;
        TicketType = ticketType;
    }

    public static Ticket Create(Guid id, string description, TicketType? ticketType)
    {
        return new Ticket(id, description,  ticketType);
    }
}