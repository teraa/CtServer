using System.IO;
using System.Text;
using CtServer.Data;
using CtServer.Services;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

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

        services.AddSwaggerGen(x =>
        {
            x.IncludeXmlComments(Path.Combine(System.AppContext.BaseDirectory, $"{nameof(CtServer)}.xml"));
            x.CustomSchemaIds(x => x.FullName);
            // x.CustomOperationIds(x => $"{x.HttpMethod} {x.RelativePath}");
            // x.CustomOperationIds(x => $"{x.ActionDescriptor.Id}");
            // x.CustomOperationIds(x => $"{x.ActionDescriptor.RouteValues["controller"]}_{x.HttpMethod}");

            x.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "JWT Authorization header using the bearer scheme",
                In = ParameterLocation.Header,
                // For Http "Bearer " is automatically prepended to the value provided in the Swagger UI
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
            });
            x.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        }
                    },
                    new string[0]
                }
            });
        });

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration[TokenService.SecretName])),

                // For now
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true,
            };
        });

        services.AddAuthorization();

        services.AddSingleton<TokenService>();
        services.AddSingleton<PasswordService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
            });

            app.UseReDoc(x =>
            {
                x.RoutePrefix = "redoc";
                x.ExpandResponses("200,201");
                x.NativeScrollbars();
            });
        }

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        if (env.IsDevelopment())
        {
            app.UseCors(policy =>
            {
                policy.AllowAnyOrigin();
                policy.AllowAnyHeader();
            });
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
