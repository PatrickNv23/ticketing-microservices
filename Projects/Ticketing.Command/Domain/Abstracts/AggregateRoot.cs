using Common.Core.Events;

namespace Ticketing.Command.Domain.Abstracts;

public abstract class AggregateRoot
{
    protected string _id = string.Empty;

    public string Id
    {
        get { return _id; }
    }
    public int Version { get; set; }
    private readonly List<BaseEvent> _changes = new();
    
    public IEnumerable<BaseEvent> GetUncommittedChanges()
    {
        return _changes;
    }

    public void MarkChangesAsCommitted()
    {
        _changes.Clear();
    }

    public void ApplyChanges(BaseEvent @event, bool isNewEvent)
    {
        // obtengo la clase que está ejecutando este apply de manera dinámica obtengo su método apply de reflection
        var method = GetType().GetMethod("Apply", [@event.GetType()]);

        if (method is null)
        {
            throw new ArgumentNullException(
                nameof(method),
                $"The apply method is not found within {@event.GetType().Name}");
        }

        method.Invoke(this, [@event]);

        if (isNewEvent)
        {
            // agregar a la colección de eventos
            _changes.Add(@event);
        }
    }

    public void RaiseEvent(BaseEvent @event)
    {
        ApplyChanges(@event, true);
    }
}