using Hubs.Contracts;

namespace Hubs.Hubs;

public interface IParserTaskCollectMessagesHandler
{
	Task SendParserTaskCollectMessageAsync(ParserTaskCollectMessage collectMessage);
}