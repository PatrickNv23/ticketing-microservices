using System.Text.Json;
using Common.Core.Events;
using Confluent.Kafka;
using Ticketing.Query.Domain.Abstractions;
using Ticketing.Query.Infrastructure.Converters;

namespace Ticketing.Query.Infrastructure.Consumers;

// esta clase será en background, revisando los mensajes que lleguen al kafka
public class ConsumerHostedService(
    ConsumerConfig config, 
    ILogger<ConsumerHostedService> logger, 
    IServiceProvider serviceProvider
    ) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("El Event Consumer service está trabajando");
        var topic = "KAFKA_TOPIC";

        // creo el consumer
        using var consumer = new ConsumerBuilder<string, string>(config)
            .SetKeyDeserializer(Deserializers.Utf8)
            .SetValueDeserializer(Deserializers.Utf8)
            .Build();
        
        // ahora me subscribo al consumer
        consumer.Subscribe(topic);
        
        // tomo el mensaje, lo deserealizo y lo envío a mi método genérico
        while(true)
        {
            var consumeResult = consumer.Consume();
            if (consumeResult is null) continue;
            if (consumeResult.Message is null) continue;
            
            // para los options para ver que evento se tiene que serializar
            var options = new JsonSerializerOptions
            {
                Converters = { new EventJsonConverter() }
            };
            
            var @event = JsonSerializer.Deserialize<BaseEvent>(consumeResult.Message.Value, options);
            if (@event is null) throw new ArgumentNullException(nameof(@event), "No se pudo procesar el json parser");
            
            // enviar el evento al handler genérico
            using IServiceScope scope = serviceProvider.CreateScope();
            var eventHandler = scope.ServiceProvider.GetRequiredService<IEventHandler>();
            var handlerMethod = eventHandler.GetType()
                .GetMethod("On", new Type[] { @event.GetType() });

            if (handlerMethod is null)
            {
                throw new ArgumentNullException(nameof(handlerMethod), "No encontró el método handler correspondiente");
            }

            handlerMethod.Invoke(eventHandler, new object[] { @event });
            
            // quitar del topic a ese mensaje que ya he procesado
            consumer.Commit(consumeResult);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}