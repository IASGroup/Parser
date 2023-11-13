namespace Share.Tables;

public sealed class ValueOptions
{
	public Guid Id { get; set; }
	public Guid? RangeId { get; set; }
	public string? Value { get; set; }

	public Range? Range { get; set; }
	public IEnumerable<ValueItem>? Values { get; set; }
}

public sealed class ValueItem
{
	public Guid Id { get; set; }
	public string Value { get; set; } = null!;
	public Guid ValueOptionsId { get; set; }

	public ValueOptions? ValueOptions { get; set; }
}

public sealed class Range
{
	public Guid Id { get; set; }
	public int Start { get; set; }
	public int End { get; set; }
}