using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reporter.Contexts;
using Reporter.ParserTask.Contracts;
using Share.Contracts;

namespace Reporter.ParserTask.Queries.DownloadTaskResults;

public class DownloadTaskResultsQueryHandler : IRequestHandler<DownloadTaskResultsQuery, Result<byte[]>>
{
	private readonly AppDbContext _context;
	private readonly ILogger<DownloadTaskResultsQueryHandler> _logger;

	public DownloadTaskResultsQueryHandler(AppDbContext context, ILogger<DownloadTaskResultsQueryHandler> logger)
	{
		_context = context;
		_logger = logger;
	}

	public async Task<Result<byte[]>> Handle(DownloadTaskResultsQuery request, CancellationToken cancellationToken)
	{
		try
		{
			var taskModel = await _context.ParserTasks.FirstOrDefaultAsync(
				predicate: x => x.Id == request.TaskId,
				cancellationToken: cancellationToken
			);
			if (taskModel is null)
			{
				return Result<byte[]>.Failure("Задача парсинга не найдена");
			}
			var taskResults = await _context.ParserTaskPartialResults
				.Where(x =>
					x.ParserTaskId == request.TaskId
					&& x.StatusId == (int) ParserTaskPartialResultStatuses.Success
				)
				.Select(x => x.Content)
				.ToListAsync(cancellationToken);
			return Result<byte[]>.Success(Encoding.ASCII.GetBytes(string.Concat(taskResults)));
		}
		catch (Exception e)
		{
			var errorMessage = "Ошибка при получении результатов задачи";
			_logger.LogError(
				message: errorMessage,
				exception: e,
				args: new { request, GetType().Name }
			);
			return Result<byte[]>.Failure(errorMessage);
		}

	}
}