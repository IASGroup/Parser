﻿namespace Reporter.ParserTask.Share;

public interface IParserTaskUtilService
{
	IEnumerable<string> GetParserTaskUrls(global::Share.Tables.ParserTask parserTaskInAction);

	HttpMethod GetHttpMethodByName(string name);
}