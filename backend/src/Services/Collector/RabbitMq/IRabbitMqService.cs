using Collector.Contracts;

namespace Collector.RabbitMq;

public interface IRabbitMqService
{
    void SendParserTaskCollectMessage(ParserTaskCollectMessage parserTaskCollectMessage);
}