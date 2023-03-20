using pdipadapter.Infrastructure.Auth;

namespace pdipadapter.Kafka.Constants;
public class KafkaTopics
{
    private readonly pdipadapterConfiguration _config;
    public KafkaTopics(pdipadapterConfiguration config)
    {
        _config = config;
        UserProvisioned = config.KafkaCluster.TopicName;
    }

    private string UserProvisioned { get;set; }
}
