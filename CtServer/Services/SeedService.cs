using CtServer.Data.Models;

namespace CtServer.Services;

public class SeedService
{
    private readonly IHostEnvironment _hostEnvironment;
    private readonly CtDbContext _ctx;
    private readonly PasswordService _passwordService;

    public SeedService(
        IHostEnvironment hostEnvironment,
        CtDbContext ctx,
        PasswordService passwordService)
    {
        _hostEnvironment = hostEnvironment;
        _ctx = ctx;
        _passwordService = passwordService;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!_hostEnvironment.IsDevelopment())
            return;

        const string username = "admin";
        var adminExists = await _ctx.Users.AnyAsync(x => x.Username == username, cancellationToken).ConfigureAwait(false);

        if (!adminExists)
        {
            var password = _passwordService.Hash(username);

            var user = new User
            {
                Username = username,
                IsAdmin = true,
                PasswordHash = password.hash,
                PasswordSalt = password.salt,
            };

            _ctx.Users.Add(user);

            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        var seeded = await _ctx.Events.AnyAsync(cancellationToken).ConfigureAwait(false);

        if (seeded) return;

        var now = DateTimeOffset.UtcNow;

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
            Description = "Event A Description",
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

        _ctx.Events.Add(evt);

        await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
