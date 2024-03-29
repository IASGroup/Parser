﻿namespace Unlocker.Contracts;

public class HttpRequestDto
{
	public string Url { get; set; } = null!;
	public string MethodName { get; set; } = null!;
	public string? Body { get; set; }
	public IEnumerable<HeaderDto> Headers { get; set; } = new List<HeaderDto>();
}