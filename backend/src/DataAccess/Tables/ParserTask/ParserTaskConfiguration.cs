using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Tables.ParserTask;

public class ParserTaskConfiguration : IEntityTypeConfiguration<ParserTask>
{
    public void Configure(EntityTypeBuilder<ParserTask> builder)
    {
    }
}