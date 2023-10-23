namespace TaskManager.ParserTasks.Commands.CreateParserTask.ParserTaskForCreation;

public class ValueOptions
{
    public Range Range { get; set; }
    public IList<ValueItem> Values { get; set; }
    public string Value { get; set; }
}

public class ValueItem
{
    public string Value { get; set; }
}

public class Range
{
    public int Start { get; set; }
    public int End { get; set; }
}