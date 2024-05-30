namespace pdipadapter.Kafka.Producer.Interfaces;
/// <summary>
/// Provides mechanism to create Kafka Handler
/// </summary>
/// <typeparam name="Tk">Indicates the message's key for Kafka Topic</typeparam>
/// <typeparam name="Tv">Indicates the message's value for Kafka Topic</typeparam>
public interface IKafkaHandler<Tk, Tv>
{
    /// <summary>
    /// Provide mechanism to handle the consumer message from Kafka
    /// </summary>
    /// <param name="key">Indicates the message's key for Kafka Topic</param>
    /// <param name="value">Indicates the message's value for Kafka Topic</param>
    /// <returns></returns>
    Task HandleAsync(Tk key, Tv value);
}

