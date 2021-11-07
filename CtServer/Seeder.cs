using System;
using System.Threading;
using System.Threading.Tasks;
using CtServer.Data;
using CtServer.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CtServer
{
    public static class Seeder
    {
        public static async Task SeedAsync(CtDbContext ctx, CancellationToken cancellationToken = default)
        {
            var seeded = await ctx.Events.AnyAsync(cancellationToken).ConfigureAwait(false);
            if (seeded) return;

            var now = DateTimeOffset.Now;

            var locA = new Location
            {
                Name = "Location A",
            };

            var locB = new Location
            {
                Name = "Location B",
            };


            var evt = new Event
            {
                Title = "Event A",
                StartAt = now,
                EndAt = now + TimeSpan.FromDays(5),
                Sections = new Section[]
                {
                    new Section
                    {
                        Title = "Section A",
                        Location = locA,
                        Chairs = new[] { "Chair AA", "Chair AB" },
                        StartAt = now + TimeSpan.FromHours(1),
                        EndAt = now + TimeSpan.FromHours(5),
                        BackgroundColor = 0x20e6ff,
                        Presentations = new Presentation[]
                        {
                            new Presentation
                            {
                                Title = "Presentation AA",
                                Authors = new[] { "Author AAA", "Author AAB" },
                                Description = "Description",
                                Position = 1,
                                Duration = TimeSpan.FromMinutes(30),
                            },
                            new Presentation
                            {
                                Title = "Presentation AB",
                                Authors = new[] { "Author ABA", "Author ABB" },
                                Description = "Description",
                                Position = 2,
                                Duration = TimeSpan.FromMinutes(45),
                            },
                        }
                    },
                    new Section
                    {
                        Title = "Section B",
                        Location = locB,
                        Chairs = new[] { "Chair BA", "Chair BB" },
                        StartAt = now + TimeSpan.FromHours(1),
                        EndAt = now + TimeSpan.FromHours(5),
                        BackgroundColor = 0xA071F1,
                        Presentations = new Presentation[]
                        {
                            new Presentation
                            {
                                Title = "Presentation BA",
                                Authors = new[] { "Author BAA", "Author BAB" },
                                Description = "Description",
                                Position = 1,
                                Duration = TimeSpan.FromMinutes(15),
                            },
                            new Presentation
                            {
                                Title = "Presentation BB",
                                Authors = new[] { "Author BBA", "Author BBB" },
                                Description = "Description",
                                Position = 2,
                                Duration = TimeSpan.FromMinutes(60),
                            },
                        }
                    },
                },
                Locations = new Location[]
                {
                    locA,
                    locB,
                },
            };

            ctx.Events.Add(evt);

            await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
