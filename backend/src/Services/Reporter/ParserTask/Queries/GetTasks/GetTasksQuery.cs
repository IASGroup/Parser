using MediatR;
using Reporter.ParserTask.Contracts;

namespace Reporter.ParserTask.Queries.GetTasks;

public class GetTasksQuery : IRequest<Result<IEnumerable<ParserTaskListItemDto>>> { }