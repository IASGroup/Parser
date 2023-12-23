using MediatR;
using Microsoft.EntityFrameworkCore;
using Reporter.Contexts;
using Reporter.ParserTask.Contracts;
using Reporter.ParserTask.Share;
using Share.Contracts;

namespace Reporter.ParserTask.Queries.GetTask;

public class GetTaskQueryHandler : IRequestHandler<GetTaskQuery, Result<ParserTaskDto>>
{
	private readonly AppDbContext _context;
	private readonly ILogger<GetTaskQueryHandler> _logger;
	private readonly IParserTaskUtilService _parserTaskUtilService;

	public GetTaskQueryHandler(
		AppDbContext context,
		ILogger<GetTaskQueryHandler> logger,
		IParserTaskUtilService parserTaskUtilService
	)
	{
		_context = context;
		_logger = logger;
		_parserTaskUtilService = parserTaskUtilService;
	}

	public async Task<Result<ParserTaskDto>> Handle(GetTaskQuery request, CancellationToken cancellationToken)
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
				return Result<ParserTaskDto>.Failure("Задача парсинга не найдена");
			}

			string? inProgressUrl = null;
			if (task.StatusId == (int) ParserTaskStatuses.InProgress)
			{
				var allUrls = _parserTaskUtilService.GetParserTaskUrls(task).ToList();
				var handledUrls = await _context.ParserTaskPartialResults
					.Where(x => x.ParserTaskId == task.Id && x.Url != null)
					.Select(x => x.Url!)
					.ToListAsync(cancellationToken);
				inProgressUrl = allUrls.Except(handledUrls).First();
			}

			var allPartsNumber = _parserTaskUtilService.GetParserTaskUrls(task).Count();
			var completedPartsNumber = await _context.ParserTaskPartialResults
				.Where(x => x.ParserTaskId == task.Id)
				.CountAsync(cancellationToken);

			return Result<ParserTaskDto>.Success(new ParserTaskDto
			{
				Id = task.Id,
				StatusId = task.StatusId,
				Url = task.Url,
				Name = task.Name,
				TypeId = task.TypeId,
				InProgressUrl = inProgressUrl,
				AllPartsNumber = allPartsNumber,
				CompletedPartsNumber = completedPartsNumber
			});
		}
		catch (Exception e)
		{
			var errorMessage = "Ошибка при получении задачи парсинга";
			_logger.LogError(
				message: errorMessage,
				exception: e,
				args: new { request, GetType().Name }
			);
			return Result<ParserTaskDto>.Failure(errorMessage);
		}
	}
}