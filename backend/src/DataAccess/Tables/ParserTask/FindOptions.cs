namespace DataAccess.Tables.ParserTask;

public class FindOptions : BaseTable
{
    public string Name { get; set; }
    public IList<TagAttribute> Attributes { get; set; }
}