namespace Collector.Contracts.Core;

public class ValueOptions
{
    public Guid Id { get; set; }
    public Range Range { get; set; }
    public IList<ValueItem> Values { get; set; }
    public string Value { get; set; }
}

public class ValueItem
{
    public Guid Id { get; set; }
    public string Value { get; set; }
}

public class Range
{
    public Guid Id { get; set; }
    public int Start { get; set; }
    public int End { get; set; }
}