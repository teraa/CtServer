using CtServer.Services;

namespace CtServer;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        using (var scope = host.Services.CreateScope())
        {
            var ctx = scope.ServiceProvider.GetRequiredService<CtDbContext>();
            await ctx.Database.MigrateAsync();

            var seeder = scope.ServiceProvider.GetRequiredService<SeedService>();
            await seeder.SeedAsync();
        }

        await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
