using DMS.API.Extensions;
using DMS.API.Hubs;
using DMS.API.Middleware;
using DMS.API.Service;
using DMS.Infrastructure;
using DMS.Infrastructure.Data;
using DMS.Infrastructure.Data.Config; // Add this for IdentitySeed
using DMS.Core.Entities; // Add this for User
using Microsoft.AspNetCore.Identity; // Add this
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Try DATABASE_URL first
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

// If DATABASE_URL doesn't exist, build from individual vars
if (string.IsNullOrEmpty(databaseUrl))
{
    var host = Environment.GetEnvironmentVariable("PGHOST");
    var port = Environment.GetEnvironmentVariable("PGPORT");
    var database = Environment.GetEnvironmentVariable("PGDATABASE");
    var username = Environment.GetEnvironmentVariable("PGUSER");
    var password = Environment.GetEnvironmentVariable("PGPASSWORD");

    Console.WriteLine($"🔍 Building connection from individual vars:");
    Console.WriteLine($"   Host: {host}");
    Console.WriteLine($"   Port: {port}");
    Console.WriteLine($"   Database: {database}");
    Console.WriteLine($"   User: {username}");

    if (!string.IsNullOrEmpty(host))
    {
        var npgsqlBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = host,
            Port = int.Parse(port ?? "5432"),
            Username = username,
            Password = password,
            Database = database,
            SslMode = SslMode.Require,
            TrustServerCertificate = true
        };

        builder.Configuration["ConnectionStrings:DefaultConnection"] = npgsqlBuilder.ConnectionString;
        Console.WriteLine("✅ Connection string built from individual variables");
    }
}
else
{
    // Parse DATABASE_URL
    try
    {
        var databaseUri = new Uri(databaseUrl);
        var userInfo = databaseUri.UserInfo.Split(':');
        
        var npgsqlBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = databaseUri.Host,
            Port = databaseUri.Port,
            Username = userInfo[0],
            Password = userInfo[1],
            Database = databaseUri.LocalPath.TrimStart('/'),
            SslMode = SslMode.Require,
            TrustServerCertificate = true
        };
        
        builder.Configuration["ConnectionStrings:DefaultConnection"] = npgsqlBuilder.ConnectionString;
        Console.WriteLine("✅ Railway DATABASE_URL parsed successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error parsing DATABASE_URL: {ex.Message}");
        throw;
    }
}

// Get port from Railway
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Enable legacy timestamp for PostgreSQL
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// SignalR
builder.Services.AddSignalR(o => o.EnableDetailedErrors = true);

// Controllers
builder.Services.AddControllers();

// Register custom services
builder.Services.AddApiRegistration();

// RabbitMQ connection factory using config (NOT localhost)
builder.Services.AddSingleton(
    new ConnectionFactory
    {
        HostName = builder.Configuration["RabbitMQ:Host"],
        UserName = builder.Configuration["RabbitMQ:Username"],
        Password = builder.Configuration["RabbitMQ:Password"],
        VirtualHost = builder.Configuration["RabbitMQ:VHost"]
    });

// Background consumer service
builder.Services.AddHostedService<LogConsumerService>();

// EF Core + MySQL
builder.Services.InfrastructureConfiguration(builder.Configuration);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(s =>
{
    var securitySchema = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Auth Bearer",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        Reference = new OpenApiReference
        {
            Id = "Bearer",
            Type = ReferenceType.SecurityScheme,
        }
    };
    s.AddSecurityDefinition("Bearer", securitySchema);
    var securityRequirement = new OpenApiSecurityRequirement { { securitySchema, new[] { "bearer" } } };
    s.AddSecurityRequirement(securityRequirement);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Auto-create database and run migrations + SEED DATA
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<DataContext>();

    try
    {
        // Apply migrations
        db.Database.Migrate();
        Console.WriteLine("✔ Database migrations applied.");

        // Seed roles and users
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();

        await IdentitySeed.SeedUserAsync(userManager, roleManager);
        Console.WriteLine("✔ Identity seed completed.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Database setup failed: {ex.Message}");
        throw;
    }
}

// Run migrations if --migrate is passed
if (args.Contains("--migrate"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    db.Database.Migrate();
    Console.WriteLine("✔ EF Core migrations applied.");
    return;
}

app.UseRouting();


// Add static files middleware BEFORE CORS
app.UseStaticFiles(); // Add this line

// Swagger only in dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Custom exception middleware
app.UseMiddleware<ExceptionMiddleware>();
app.UseStatusCodePagesWithReExecute("/errors/{0}");

// DO NOT USE HTTPS REDIRECTION INSIDE A DOCKER CONTAINER (unless using proxy)
// app.UseHttpsRedirection();

// CORS
app.UseCors("AllowAngularApp");

// Auth middleware
app.UseAuthentication();
app.UseAuthorization();

// SignalR
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<NotificationHub>("/notificationHub");
});

app.MapControllers();

// Infrastructure middleware
InfrastructureRegistration.InfrastructureConfigMiddleWare(app);

await app.RunAsync();