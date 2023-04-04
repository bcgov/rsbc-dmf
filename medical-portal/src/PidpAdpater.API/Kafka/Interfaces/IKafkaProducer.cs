namespace pdipadapter.Kafka.Producer.Interfaces;
public interface IKafkaProducer<TKey, TValue> where TValue : class
{
    /// <summary>
    ///  Triggered when the service is ready to produce the Kafka topic.
    /// </summary>
    /// <param name="topic">Indicates topic name</param>
    /// <param name="key">Indicates message's key in Kafka topic</param>
    /// <param name="value">Indicates message's value in Kafka topic</param>
    /// <returns></returns>
    Task ProduceAsync(string topic, TKey key, TValue value);
}