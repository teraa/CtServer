using CtServer.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CtServer.Data.Configurations;

public class PresentationConfiguration : IEntityTypeConfiguration<Presentation>
{
    public void Configure(EntityTypeBuilder<Presentation> builder)
    {
        builder.HasOne(x => x.Attachment)
            .WithOne(x => x.Presentation)
            .HasForeignKey<Attachment>(x => x.PresentationId);

        builder.HasOne(x => x.Photo)
            .WithOne(x => x.Presentation)
            .HasForeignKey<Photo>(x => x.PresentationId);
    }
}
