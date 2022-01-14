using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CtServer;
using CtServer.Authorization;
using CtServer.Options;
using CtServer.Services;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure services

builder.Services.AddDbContext<CtDbContext>(ctxOpt =>
{
    // ctxOpt.ConfigureWarnings(w => w.Throw(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.MultipleCollectionIncludeWarning));
    // ctxOpt.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.RowLimitingOperationWithoutOrderByWarning));
    ctxOpt.UseNpgsql(builder.Configuration["DB_STRING"], npgsqlOpt => npgsqlOpt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
});

builder.Services.AddControllers()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    });

builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        JwtOptions options = builder.Configuration.GetOptions<JwtOptions>();

        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(options.Secret)),

            // For now
            ValidateIssuer = false,
            ValidateAudience = false,
            RequireExpirationTime = false,
            ValidateLifetime = true,
        };
    });

builder.Services
    .AddOptionsWithSection<JwtOptions>(builder.Configuration)
    .AddOptionsWithSection<WebPushOptions>(builder.Configuration)
    .AddOptionsWithSection<StorageOptions>(builder.Configuration)
    .AddScoped<SeedService>()
    .AddScoped<IAuthorizationHandler, AdminAuthorizationHandler>()
    .AddSingleton<TokenService>()
    .AddSingleton<PasswordService>()
    .AddSingleton<NotificationService>()
    .AddSingleton<JsonSerializerOptions>(new JsonSerializerOptions(JsonSerializerDefaults.Web)
    {
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
        },
    })
    .AddMediatR(typeof(Program))
    .AddFluentValidation(options => options.RegisterValidatorsFromAssemblyContaining(typeof(Program)))
    .AddAuthorization(x =>
    {
        x.AddPolicy("CurrentUser", builder =>
        {
            builder.RequireAssertion(ctx =>
            {
                return ctx.Resource is HttpContext httpContext
                    && httpContext.Request.RouteValues.TryGetValue("id", out object? idObj)
                    && idObj is string id
                    && httpContext.User.HasUserId(id);
            });
        });

        x.AddPolicy("Admin", builder =>
        {
            builder.AddRequirements(new AdminRequirement());
        });
    })
    .AddSwaggerGen(x =>
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
    })
    ;

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<CtDbContext>();
    await ctx.Database.MigrateAsync();

    var seeder = scope.ServiceProvider.GetRequiredService<SeedService>();
    await seeder.SeedAsync();
}

// Configure

if (app.Environment.IsDevelopment())
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

if (app.Environment.IsDevelopment())
{
    app.UseCors(policy =>
    {
        policy.AllowAnyOrigin();
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();

    if (app.Environment.IsDevelopment())
    {
        endpoints.MapGet("/", async context =>
        {
            await context.Response.WriteAsync("Hello World!");
        });
    }
});

await app.RunAsync();
