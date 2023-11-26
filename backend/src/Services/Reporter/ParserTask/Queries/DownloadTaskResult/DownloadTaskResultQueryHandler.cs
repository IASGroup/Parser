using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reporter.Contexts;
using Reporter.ParserTask.Contracts;

namespace Reporter.ParserTask.Queries.DownloadTaskResult;

public class DownloadTaskResultQueryHandler : IRequestHandler<DownloadTaskResultQuery, Result<byte[]>>
{
	private readonly AppDbContext _context;
	private readonly ILogger<DownloadTaskResultQueryHandler> _logger;

	public DownloadTaskResultQueryHandler(AppDbContext context, ILogger<DownloadTaskResultQueryHandler> logger)
	{
		_context = context;
		_logger = logger;
	}

	public async Task<Result<byte[]>> Handle(DownloadTaskResultQuery request, CancellationToken cancellationToken)
	{
		try
		{
			var task = await _context.ParserTasks.FirstOrDefaultAsync(
				predicate: x => x.Id == request.TaskId,
				cancellationToken: cancellationToken
			);
			if (task is null)
			{
				return Result<byte[]>.Failure("Задача не найдена");
			}

			var result = await _context.ParserTaskPartialResults.FirstOrDefaultAsync(
				predicate: x => x.ParserTaskId == request.TaskId && x.Id == request.ResultId,
				cancellationToken: cancellationToken
			);

			return result is null
				? Result<byte[]>.Failure("Результат не найден")
				: Result<byte[]>.Success(Encoding.UTF8.GetBytes(string.Concat(result.Content)));
		}
		catch (Exception e)
		{
			var errorMessage = "Ошибка при загрузке частичного результата задачи";
			_logger.LogError(
				message: errorMessage,
				exception: e,
				args: new { request, GetType().Name }
			);
			return Result<byte[]>.Failure(errorMessage);
		}
	}
}