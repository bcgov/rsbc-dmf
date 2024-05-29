using pdipadapter.Infrastructure.Auth;

namespace pdipadapter.Kafka.Constants;
public class KafkaTopics
{
    private readonly PdipadapterConfiguration _config;
    public KafkaTopics(PdipadapterConfiguration config)
    {
        _config = config;
        UserProvisioned = config.KafkaCluster.TopicName;
    }

    private string UserProvisioned { get;set; }
}
