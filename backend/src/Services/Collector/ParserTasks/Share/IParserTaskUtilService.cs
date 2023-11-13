using Share.RabbitMessages.ParserTaskAction;

namespace Collector.ParserTasks.Share;

public interface IParserTaskUtilService
{
	IEnumerable<string> GetParserTaskUrls(ParserTask parserTaskInAction);
	
	HttpMethod GetHttpMethodByName(string name);
}