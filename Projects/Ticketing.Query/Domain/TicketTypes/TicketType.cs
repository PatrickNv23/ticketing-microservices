using System.Diagnostics.CodeAnalysis;
using Common.Core.Domain;

namespace Ticketing.Query.Domain.TicketTypes;

public class TicketType
{
    public int Id { get; set; }
    public required string Name { get; set; }

    private TicketType() { }

    [SetsRequiredMembers] // porque name es required
    private TicketType(int id, string name) => (Id, Name) = (id, name);

    public static TicketType Create(int id)
    {
        var ticketTypeEnum = (TicketTypeEnum)id;
        string stringValue = ticketTypeEnum.ToString();
        return new TicketType(id, stringValue);
    }
}