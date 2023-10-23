﻿namespace Collector.Contracts;

public class ParserTaskCollectMessage
{
    public Guid ParserTaskId { get; set; }
    public ParserTaskStatusChangedMessage? ParserTaskStatusChangedMessage { get; set; }
    public ParserTaskErrorMessage? ParserTaskErrorMessage { get; set; }
}

public class ParserTaskErrorMessage
{
    public string ErrorMessage { get; set; }
}

public class ParserTaskStatusChangedMessage
{
    public int NewTaskStatus { get; set; }
}
