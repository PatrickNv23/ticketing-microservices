using Common.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ticketing.Query.Domain.TicketTypes;

namespace Ticketing.Query.Infrastructure.Configurations;

public class TicketTypeConfiguration: IEntityTypeConfiguration<TicketType>
{
    public void Configure(EntityTypeBuilder<TicketType> builder)
    {
        builder.ToTable("ticket_type");
        builder.HasKey(tt => tt.Id);
        
        // obtener la data del enum de ticket types
        IEnumerable<TicketType> ticketTypes = Enum
            .GetValues<TicketTypeEnum>()
            .Select(tte => TicketType.Create((int)tte));
        
        // crear el código de la data de ticketTypes en formato SQL para insertar en postgresql cuando ejecute la migración
        builder.HasData(ticketTypes);
    }
}