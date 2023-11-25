using MediatR;
using Microsoft.EntityFrameworkCore;
using Reporter.Contexts;
using Reporter.ParserTask.Contracts;
using Reporter.ParserTask.Share;

namespace Reporter.ParserTask.Queries.GetTaskResults;

public class GetTaskResultsQueryHandler : IRequestHandler<GetTaskResultsQuery, Result<IEnumerable<ParserTaskResult>>>
{
	private readonly AppDbContext _context;
	private readonly ILogger<GetTaskResultsQueryHandler> _logger;
	private readonly IParserTaskUtilService _parserTaskUtilService;

	public GetTaskResultsQueryHandler(
		AppDbContext context,
		ILogger<GetTaskResultsQueryHandler> logger,
		IParserTaskUtilService parserTaskUtilService
	)
	{
		_context = context;
		_logger = logger;
		_parserTaskUtilService = parserTaskUtilService;
	}

	public async Task<Result<IEnumerable<ParserTaskResult>>> Handle(GetTaskResultsQuery request, CancellationToken cancellationToken)
	{
		try
		{
			var task = await _context.ParserTasks
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
				.FirstOrDefaultAsync(
					predicate: x => x.Id == request.ParserTaskId,
					cancellationToken: cancellationToken
				);
			if (task is null)
			{
				return Result<IEnumerable<ParserTaskResult>>.Failure("Задача парсинга не найдена");
			}

			var results = await _context.ParserTaskPartialResults
				.Where(x => x.ParserTaskId == task.Id)
				.Select(x => new ParserTaskResult
				{
					ParserTaskId = x.ParserTaskId,
					StatusId = x.StatusId,
					Id = x.Id,
					Url = x.Url!
				})
				.ToListAsync(cancellationToken);
			return Result<IEnumerable<ParserTaskResult>>.Success(results);
		}
		catch (Exception e)
		{
			var errorMessage = "Ошибка при получении результатов задачи парсинга";
			_logger.LogError(
				message: errorMessage,
				exception: e,
				args: new { request, GetType().Name }
			);
			return Result<IEnumerable<ParserTaskResult>>.Failure(errorMessage);
		}
	}
}