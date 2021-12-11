using CtServer.Data;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CtServer;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<CtDbContext>(ctxOpt =>
        {
            // ctxOpt.ConfigureWarnings(w => w.Throw(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.MultipleCollectionIncludeWarning));
            // ctxOpt.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.RowLimitingOperationWithoutOrderByWarning));
            ctxOpt.UseNpgsql(Configuration["DB_STRING"], npgsqlOpt => npgsqlOpt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
        });

        services.AddMediatR(typeof(Startup));

        services.AddFluentValidation(options => options.RegisterValidatorsFromAssemblyContaining(typeof(Startup)));

        services.AddControllers();

        services.AddOpenApiDocument();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            app.UseOpenApi();

            app.UseSwaggerUi3();
            app.UseReDoc(x => x.Path = "/redoc");
        }

        app.UseRouting();

        if (env.IsDevelopment())
        {
            app.UseCors(policy => policy.AllowAnyOrigin());
        }

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();

            if (env.IsDevelopment())
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            }
        });
    }
}
