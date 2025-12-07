using Common.Core.Events;
using Common.Core.Producers;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Ticketing.Command.Application.Models;

namespace Ticketing.Command.Infrastructure.Persistence;

public class TicketEventProducer(IOptions<KafkaSettings> kafkaSettings) : IEventProducer
{
    private readonly KafkaSettings _kafkaSettings = kafkaSettings.Value;

    public async Task ProduceAsync(string topic, BaseEvent @event)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = $"{_kafkaSettings.HostName}:{_kafkaSettings.Port}"
        };

        using var producer = new ProducerBuilder<string, string>(config)
            .SetKeySerializer(Serializers.Utf8)
            .SetValueSerializer(Serializers.Utf8)
            .Build();

        var eventMessage = new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = JsonConvert.SerializeObject(@event)
        };
        
        // enviar al broker, kafka si recibe el mensaje devuelve un estado
        var deliveryStatus = await producer.ProduceAsync(topic, eventMessage);

        if (deliveryStatus.Status == PersistenceStatus.NotPersisted)
        {
            throw new Exception($"No se pudo enviar el mensaje {@event.GetType().Name} " +
                                $"hacia el topic {topic}, " +
                                $"por la siguiente razón: {deliveryStatus.Message}");
        }
    }
}