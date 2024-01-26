using MediatR;
using Microsoft.EntityFrameworkCore;
using Reporter.Contexts;
using Reporter.ParserTask.Contracts;

namespace Reporter.ParserTask.Queries.GetPartTaskResult;

public class GetPartTaskResultQueryHandler : IRequestHandler<GetPartTaskResultQuery, Result<ParserTaskPartialResultDto>>
{
	private readonly AppDbContext _context;
	private readonly ILogger<GetPartTaskResultQueryHandler> _logger;

	public GetPartTaskResultQueryHandler(AppDbContext context, ILogger<GetPartTaskResultQueryHandler> logger)
	{
		_context = context;
		_logger = logger;
	}

	public async Task<Result<ParserTaskPartialResultDto>> Handle(
		GetPartTaskResultQuery request,
		CancellationToken cancellationToken
	)
	{
		try
		{
			var taskModel = await _context.ParserTasks.FirstOrDefaultAsync(
				predicate: x => x.Id == request.TaskId,
				cancellationToken: cancellationToken
			);
			if (taskModel is null)
			{
				return Result<ParserTaskPartialResultDto>.Failure("Задача парсинга не найдена");
			}

			var partTaskResult = await _context.ParserTaskPartialResults
				.FirstOrDefaultAsync(
					predicate: x => x.Id == request.ResultId && x.ParserTaskId == request.TaskId,
					cancellationToken: cancellationToken
				);
			return Result<ParserTaskPartialResultDto>.Success(new ParserTaskPartialResultDto()
			{
				ParserTaskId = partTaskResult.ParserTaskId,
				Id = partTaskResult.Id,
				Content = partTaskResult.Content,
				StatusId = partTaskResult.StatusId,
				Url = partTaskResult.Url
			});
		}
		catch (Exception e)
		{
			var errorMessage = "Ошибка при получении части результата задачи";
			_logger.LogError(
				message: errorMessage,
				exception: e,
				args: new {request, GetType().Name}
			);
			return Result<ParserTaskPartialResultDto>.Failure(errorMessage);
		}
	}
}