using Common.Core.Messages;

namespace Common.Core.Events;

public abstract class BaseEvent(string type) : Message
{
    public int Version { get; set; }
    public string Type { get; set; } = type;
}