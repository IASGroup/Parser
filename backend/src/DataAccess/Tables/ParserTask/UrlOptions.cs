namespace DataAccess.Tables.ParserTask;

public class UrlOptions : BaseTable
{
    public string RequestMethod { get; set; }
    public PostMethodOptions PostMethodOptions { get; set; }
    public IList<Query> Queries { get; set; }
    public IList<Path> Paths { get; set; }
    public IList<Header> Headers { get; set; }
}