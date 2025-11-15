using Common.Core.Events;
using MongoDB.Bson.Serialization.Attributes;
using Ticketing.Command.Domain.Common;

namespace Ticketing.Command.Domain.EventModels;

[BsonCollection("eventStores")]
// evento que representa la estructura general de un documento que se guardará en Bson en mongoDB
public class EventModel : Document
{
    [BsonElement("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.Now;

    [BsonElement("aggregateIdentifier")]
    // identificar del grupo de elementos
    public required string AggregateIdentifier { get; set; } = string.Empty;

    [BsonElement("aggregateType")] 
    public string AggregateType { get; set; } = string.Empty;
    
    [BsonElement("version")]
    public int Version { get; set; }
    
    [BsonElement("eventType")]
    public string EventType { get; set; } = string.Empty;
    
    // atributo importante, aquí está el evento
    public BaseEvent? EventData { get; set; }
}