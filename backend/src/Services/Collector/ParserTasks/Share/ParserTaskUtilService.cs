using Share.RabbitMessages.ParserTaskAction;

namespace Collector.ParserTasks.Share;

public class ParserTaskUtilService : IParserTaskUtilService
{
	private enum UrlPartTypes
	{
		Path = 0,
		Query = 1
	}

	private class UrlPart
	{
		public string Name { get; set; } = null!;
		public string Value { get; set; } = null!;
		public UrlPartTypes PartType { get; set; }
	}
	
	public IEnumerable<string> GetParserTaskUrls(ParserTask parserTaskInAction)
	{
		var paths = parserTaskInAction.ParserTaskUrlOptions!.Paths!.Select(x =>
		{
			if (x.ValueOptions.Value is not null) return new[]
			{
				new UrlPart()
				{
					Name = x.Name,
					Value = x.ValueOptions.Value,
					PartType = UrlPartTypes.Path
				}
			};
			if (x.ValueOptions.Values is not null && x.ValueOptions.Values.Any())
			{
				return x.ValueOptions.Values.Select(y => new UrlPart()
				{
					Name = x.Name,
					Value = y.Value,
					PartType = UrlPartTypes.Path
				}).ToArray();
			}

			return Enumerable.Range(
					start: x.ValueOptions.Range!.Start,
					count: x.ValueOptions.Range.End - x.ValueOptions.Range.Start + 1
				)
				.Select(y => new UrlPart()
				{
					Name = x.Name,
					Value = y.ToString(),
					PartType = UrlPartTypes.Path
				}).ToArray();
		});

		var queries = parserTaskInAction.ParserTaskUrlOptions!.Queries!.Select(x =>
		{
			if (x.ValueOptions.Value is not null) return new[]
			{
				new UrlPart()
				{
					Name = x.Name,
					Value = x.ValueOptions.Value,
					PartType = UrlPartTypes.Query
				}
			};
			if (x.ValueOptions.Values is not null && x.ValueOptions.Values.Any())
			{
				return x.ValueOptions.Values.Select(y => new UrlPart()
				{
					Name = x.Name,
					Value = y.Value,
					PartType = UrlPartTypes.Query
				}).ToArray();
			}

			return Enumerable.Range(
					start: x.ValueOptions.Range!.Start,
					count: x.ValueOptions.Range.End - x.ValueOptions.Range.Start + 1
				)
				.Select(y => new UrlPart()
				{
					Name = x.Name,
					Value = y.ToString(),
					PartType = UrlPartTypes.Query
				}).ToArray();
		});

		var parts = paths.Concat(queries).ToArray();

		var indexes = new int[parts.Length];
		var currentArrayIndex = indexes.Length - 1;
		for (var i = 0; i < indexes.Length; i++)
		{
			indexes[i] = 0;
		}

		bool IsIndexesEnds()
		{
			for (var i = 0; i < indexes.Length; i++)
			{
				if (indexes[i] != parts[i].Length - 1) return false;
			}

			return true;
		}

		string GetCurrentUrl()
		{
			var defaultPath = parserTaskInAction.Url;
			var indexOfQuery = defaultPath!.IndexOf('?');
			defaultPath = indexOfQuery == -1
				? defaultPath
				: defaultPath.Remove(indexOfQuery, defaultPath.Length - indexOfQuery);
			for (var i = 0; i < parts.Length; i++)
			{
				var part = parts[i][indexes[i]];
				if (part.PartType is UrlPartTypes.Path)
				{
					defaultPath = defaultPath.Replace($"{{{part.Name}}}", part.Value);
				}
				else
				{
					defaultPath += defaultPath.Contains('?') ? "&" : "?" + $"{part.Name}={part.Value}";
				}
			}

			return defaultPath;
		}

		while (!IsIndexesEnds())
		{
			var subArrayIndex = indexes[currentArrayIndex];
			if (subArrayIndex < parts[currentArrayIndex].Length - 1)
			{
				yield return GetCurrentUrl();
				indexes[currentArrayIndex] = subArrayIndex + 1;
				if (currentArrayIndex < parts.Length - 1)
				{
					for (int i = currentArrayIndex + 1; i < parts.Length; i++)
					{
						indexes[i] = 0;
					}

					currentArrayIndex = indexes.Length - 1;
				}
			}
			else if (currentArrayIndex != 0)
			{
				currentArrayIndex = currentArrayIndex - 1;
			}
		}

		yield return GetCurrentUrl();
	}

	public HttpMethod GetHttpMethodByName(string name)
	{
		return name switch
		{
			"GET" => HttpMethod.Get,
			"POST" => HttpMethod.Post,
			"PUT" => HttpMethod.Put,
			"DELETE" => HttpMethod.Delete,
			"PATCH" => HttpMethod.Patch,
			_ => throw new ArgumentException()
		};
	}
}