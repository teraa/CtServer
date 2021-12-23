using CtServer.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CtServer.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasMany(x => x.Events)
            .WithMany(x => x.Users)
            .UsingEntity<UserEvent>(
                entity => entity
                    .HasOne(x => x.Event)
                    .WithMany(x => x.UserEvents),
                entity => entity
                    .HasOne(x => x.User)
                    .WithMany(x => x.UserEvents)
            );
    }
}
