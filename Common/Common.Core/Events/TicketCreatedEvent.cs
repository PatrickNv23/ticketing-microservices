namespace Common.Core.Events;

public class TicketCreatedEvent() : BaseEvent(nameof(TicketCreatedEvent))
{
    public required string UserName { get; set; }
    public int TypeError { get; set; }
    public required string DetailError { get; set; }
}