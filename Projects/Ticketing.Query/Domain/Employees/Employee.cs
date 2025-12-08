using System.Diagnostics.CodeAnalysis;
using Ticketing.Query.Domain.Abstractions;
using Ticketing.Query.Domain.Addresses;
using Ticketing.Query.Domain.Tickets;

namespace Ticketing.Query.Domain.Employees;

public class Employee : Entity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required Address Address { get; set; }
    public required string Email { get; set; }
    public virtual ICollection<Ticket> Tickets { get; set; } = [];
    public virtual ICollection<TicketEmployee> TicketEmployees { get; set; } = [];

    public Employee(){}
    
    [SetsRequiredMembers]
    private Employee(Guid id, string firstName, string lastName, Address address, string email) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Address = address;
        Email = email;
    }

    public static Employee Create(string firstName, string lastName, Address address, string email)
    {
        var id = new Guid();
        
        return new Employee(id, firstName,  lastName, address, email);
    }
}