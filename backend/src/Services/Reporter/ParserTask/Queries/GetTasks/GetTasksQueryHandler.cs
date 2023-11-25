using MediatR;
using Microsoft.EntityFrameworkCore;
using Reporter.Contexts;
using Reporter.ParserTask.Contracts;
using Reporter.ParserTask.Share;
using Share.Contracts;

namespace Reporter.ParserTask.Queries.GetTasks;

public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, Result<IEnumerable<ParserTaskListItemDto>>>
{
	private readonly AppDbContext _context;
	private readonly ILogger<GetTasksQueryHandler> _logger;
	private readonly IParserTaskUtilService _parserTaskUtilService;

	public GetTasksQueryHandler(
		AppDbContext context,
		ILogger<GetTasksQueryHandler> logger,
		IParserTaskUtilService parserTaskUtilService
	)
	{
		_context = context;
		_logger = logger;
		_parserTaskUtilService = parserTaskUtilService;
	}

	public async Task<Result<IEnumerable<ParserTaskListItemDto>>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
	{
		try
		{
			var tasks = await _context.ParserTasks
				.Include(x => x.ParserTaskUrlOptions!.PostMethodOptions)
				.Include(x => x.ParserTaskUrlOptions!.Queries)!
				.ThenInclude(x => x.ValueOptions!.Values)
				.Include(x => x.ParserTaskUrlOptions!.Queries)!
				.ThenInclude(x => x.ValueOptions!.Range)
				.Include(x => x.ParserTaskUrlOptions!.Paths)!
				.ThenInclude(x => x.ValueOptions!.Values)
				.Include(x => x.ParserTaskUrlOptions!.Paths)!
				.ThenInclude(x => x.ValueOptions!.Range)
				.Include(x => x.ParserTaskUrlOptions!.Headers)
				.ToListAsync(cancellationToken);
			var tasksResults = await _context.ParserTaskPartialResults
				.GroupBy(x => x.ParserTaskId)
				.Select(x => new
				{
					ParserTaskId = x.Key,
					HasError = x.Any(y => y.StatusId == (int) ParserTaskPartialResultStatuses.Error),
					ParsedResultsNumber = x.Count()
				})
				.ToListAsync(cancellationToken);
			var taskListItems = tasks.Select(x =>
			{
				var taskResults = tasksResults.FirstOrDefault(y => y.ParserTaskId == x.Id);
				var allPartsNumber = _parserTaskUtilService.GetParserTaskUrls(x).Count();
				var hasError = taskResults?.HasError ?? false;
				var competedPartsNumber = taskResults?.ParsedResultsNumber ?? 0;
				return new ParserTaskListItemDto
				{
					Id = x.Id,
					StatusId = x.StatusId,
					Name = x.Name,
					Url = x.Url,
					TypeId = x.TypeId,
					HasErrors = hasError,
					CompletedPartsNumber = competedPartsNumber,
					AllPartsNumber = allPartsNumber
				};
			}).ToList();
			return Result<IEnumerable<ParserTaskListItemDto>>.Success(taskListItems);
		}
		catch (Exception e)
		{
			var errorMessage = "Ошибка при получении задач парсинга";
			_logger.LogError(
				message: errorMessage,
				exception: e,
				args: new { request, GetType().Name }
			);
			return Result<IEnumerable<ParserTaskListItemDto>>.Failure(errorMessage);
		}
	}
}