using FitForge.Api.Middleware;
using FitForge.Infrastructure;
using FitForge.Infrastructure.Data;
using HealthChecks.NpgSql;
using HealthChecks.RabbitMQ;
using HealthChecks.Redis;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

// Health Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "postgres",
        tags: new[] { "db", "ready" })
    .AddRedis(
        builder.Configuration.GetConnectionString("Redis")!,
        name: "redis",
        tags: new[] { "cache", "ready" })
    .AddRabbitMQ(
        $"amqp://{builder.Configuration["RabbitMQ:Host"]}:{builder.Configuration["RabbitMQ:Port"]}",
        name: "rabbitmq",
        tags: new[] { "messaging", "ready" });

// API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "FitForge AI API",
        Version = "v1",
        Description = "AI-powered workout planning platform"
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://localhost:3000", "http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Middleware Pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FitForge AI API v1");
    });
}

app.UseCors();
app.UseRouting();

app.MapControllers();
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new()
{
    Predicate = check => check.Tags.Contains("ready")
});
app.MapHealthChecks("/health/live", new()
{
    Predicate = _ => false
});

// Apply migrations on startup (development only, not testing)
if (app.Environment.IsDevelopment() && !app.Environment.EnvironmentName.Equals("Testing", StringComparison.OrdinalIgnoreCase))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<FitForgeDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

await app.RunAsync();

// Make the implicit Program class public so test projects can access it
public partial class Program { }
