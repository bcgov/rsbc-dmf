namespace pdipadapter.Kafka.Interfaces;
public interface IKafkaConsumer<TKey, TValue> where TValue : class
{
    /// <summary>
    ///  Triggered when the service is ready to consume the Kafka topic.
    /// </summary>
    /// <param name="topic">Indicates the message's key for Kafka Topic</param>
    /// <param name="stoppingToken">Indicates cancellation token</param>
    /// <returns></returns>
    Task Consume(string topic, CancellationToken stoppingToken);

    /// <summary>
    /// This will close the consumer, commit offsets and leave the group cleanly.
    /// </summary>
    void Close();
    /// <summary>
    /// Releases all resources used by the current instance of the consumer
    /// </summary>
    void Dispose();
}
