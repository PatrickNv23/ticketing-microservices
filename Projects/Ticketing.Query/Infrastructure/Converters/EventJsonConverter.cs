using System.Text.Json;
using System.Text.Json.Serialization;
using Common.Core.Events;
using Ticketing.Query.Domain.Tickets;

namespace Ticketing.Query.Infrastructure.Converters;

public class EventJsonConverter : JsonConverter<BaseEvent>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsAssignableFrom(typeof(BaseEvent));
    }
    
    public override BaseEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (!JsonDocument.TryParseValue(ref reader, out var doc))
        {
            throw new JsonException($"Falló haciendo el parse a json {nameof(JsonDocument)}");
        }
        
        // quiero leer la propiedad de type
        if (!doc.RootElement.TryGetProperty("Type", out var type))
        {
            throw new JsonException($"No puedo detectar la propiedad type");
        }

        var typeDiscriminator = type.GetString();
        var json = doc.RootElement.GetRawText();
        
        // dependiendo del evento hago el parseo
        return typeDiscriminator switch
        {
            nameof(TicketCreatedEvent) => JsonSerializer.Deserialize<TicketCreatedEvent>(json, options),
            nameof(TicketUpdatedEvent) => JsonSerializer.Deserialize<TicketUpdatedEvent>(json, options),
            _ => throw new JsonException($"{typeDiscriminator}: no es soportado")
        };
    }

    public override void Write(Utf8JsonWriter writer, BaseEvent value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}