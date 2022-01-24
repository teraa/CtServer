using System.Text.Json;
using CtServer.Data.Models;

namespace CtServer.Features.Data;

public static class Import
{
    public record Command
    (
        IFormFile File
    ) : IRequest<Success>;

    public class Handler : IRequestHandler<Command, Success>
    {
        private readonly CtDbContext _ctx;
        private readonly JsonSerializerOptions _jsonOptions;

        public Handler(CtDbContext ctx, JsonSerializerOptions jsonOptions)
        {
            _ctx = ctx;
            _jsonOptions = jsonOptions;
        }

        public async Task<Success> Handle(Command request, CancellationToken cancellationToken)
        {
            Event[] events;

            using (var stream = request.File.OpenReadStream())
            {
                events = await JsonSerializer.DeserializeAsync<Event[]>(stream, _jsonOptions, cancellationToken)
                    ?? null!;
            }

            await using (var transaction = await _ctx.Database.BeginTransactionAsync())
            {
                var oldEvents = await _ctx.Events
                    .ToArrayAsync(cancellationToken);

                _ctx.Events.RemoveRange(oldEvents);
                await _ctx.SaveChangesAsync(cancellationToken);

                _ctx.Events.AddRange(events);
                await _ctx.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }

            return new Success();
        }
    }
}
