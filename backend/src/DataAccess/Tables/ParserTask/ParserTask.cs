namespace DataAccess.Tables.ParserTask;

public class ParserTask : BaseTable
{
    public string Url { get; set; } = null!;
    public ParserTaskType Type { get; set; } = null!;
    public string Name { get; set; } = null!;
    public WebsiteTagOptions? WebsiteTagOptions { get; set; }
    public UrlOptions? UrlOptions { get; set; }
}