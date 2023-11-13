namespace TaskManager.ParserTasks.Contracts;

public class Result<T> where T : class
{
	public T? Value { get; set; }
	public string? ErrorMessage { get; set; }
	public bool IsSuccess { get; set; }

	public static Result<T> Failure(string errorMessage) => new()
	{
		Value = null,
		ErrorMessage = errorMessage,
		IsSuccess = false
	};

	public static Result<T> Success(T value) => new()
	{
		Value = value,
		ErrorMessage = null,
		IsSuccess = true
	};
}