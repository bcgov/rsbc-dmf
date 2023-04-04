

using Confluent.Kafka;
using pdipadapter.Kafka.Interfaces;
using pdipadapter.Kafka.Producer.Interfaces;

namespace pdipadapter.Kafka.Consumers.NotificationConsumer;
public class NotificationConsumer<TKey, TValue> : IKafkaConsumer<TKey, TValue> where TValue : class
{
    private readonly ConsumerConfig _config;
    private IKafkaHandler<TKey, TValue> _handler;
    private IConsumer<TKey, TValue> _consumer;
    private string _topic;

    private readonly IServiceScopeFactory _serviceScopeFactory;

    public NotificationConsumer(ConsumerConfig config, IKafkaHandler<TKey, TValue> handler, IConsumer<TKey, TValue> consumer, string topic, IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _config = config;
        _consumer = consumer;
        _handler = handler;
        _topic = topic;
    }

    public async Task Consume(string topic, CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        _handler = scope.ServiceProvider.GetRequiredService<IKafkaHandler<TKey, TValue>>();
        _consumer = new ConsumerBuilder<TKey, TValue>(_config).SetValueDeserializer(new KafkaDeserializer<TValue>()).Build();
        _topic = topic;

        await Task.Run(() => StartConsumerLoop(stoppingToken), stoppingToken);
    }
    /// <summary>
    /// This will close the consumer, commit offsets and leave the group cleanly.
    /// </summary>
    public void Close()
    {
        _consumer.Close();
    }
    /// <summary>
    /// Releases all resources used by the current instance of the consumer
    /// </summary>
    public void Dispose()
    {
        _consumer.Dispose();
    }
    private async Task StartConsumerLoop(CancellationToken cancellationToken)
    {
        _consumer.Subscribe(_topic);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var result = _consumer.Consume(cancellationToken);

                if (result != null)
                {
                   await _handler.HandleAsync(result.Message.Key, result.Message.Value);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (ConsumeException e)
            {
                // Consumer errors should generally be ignored (or logged) unless fatal.
                Console.WriteLine($"Consume error: {e.Error.Reason}");

                if (e.Error.IsFatal)
                {
                    break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected error: {e}");
                break;
            }
        }
    }
}
