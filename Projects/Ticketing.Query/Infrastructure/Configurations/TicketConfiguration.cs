using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ticketing.Query.Domain.Tickets;
using Ticketing.Query.Domain.TicketTypes;

namespace Ticketing.Query.Infrastructure.Configurations;

public class TicketConfiguration: IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("tickets");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.TicketType) // ticketType solo es a nivel de C#, en BD ticket debe tener un ticketId
            .HasConversion(
                ticketType => ticketType!.Id, // en BD conviértete en número -> en ticket un ticketId
                value => TicketType.Create(value) // en C# conviértete en objeto
            );
        
        // un ticket tiene muchos empleados y un empleado tiene muchos tickets
        builder
            .HasMany(t => t.Employees)
            .WithMany(t => t.Tickets)
            .UsingEntity<TicketEmployee>(
                j => j
                                .HasOne(te => te.Employee)
                                .WithMany(e => e.TicketEmployees)
                                .HasForeignKey(te => te.EmployeeId),
                j => j
                                .HasOne(te => te.Ticket)
                                .WithMany(t => t.TicketEmployees)
                                .HasForeignKey(te => te.TicketId),
                j =>
                {
                    j.HasKey(te => new { te.TicketId, te.EmployeeId });
                }
            );
    }
}

public class TicketEmployeeConfiguration : IEntityTypeConfiguration<TicketEmployee>
{
    public void Configure(EntityTypeBuilder<TicketEmployee> builder)
    {
        builder.ToTable("ticket_employees");
    }
}