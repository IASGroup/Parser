namespace DataAccess.Tables.ParserTask;

public class ValueOptions : BaseTable
{
    public Range Range { get; set; }
    public IList<ValueOptionListItem> List { get; set; }
    public string Value { get; set; }
}