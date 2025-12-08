namespace Common.Core.Events;

public class TicketUpdatedEvent: BaseEvent
{
    public TicketUpdatedEvent(string type) : base(type) { }
    public string? Status { get; set; }
    public string? Description { get; set; }
    public string? Username { get; set; }
}