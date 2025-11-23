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

// Parse Railway's DATABASE_URL if it exists
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrEmpty(databaseUrl))
{
    // Railway format: postgresql://user:password@host:port/database
    var uri = new Uri(databaseUrl);
    var connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.Trim('/')};Username={uri.UserInfo.Split(':')[0]};Password={uri.UserInfo.Split(':')[1]};SSL Mode=Require;Trust Server Certificate=true";
    
    // Override the connection string
    builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;
    
    Console.WriteLine($"✅ Using Railway database connection");
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