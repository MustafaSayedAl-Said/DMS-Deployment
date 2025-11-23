using CodeChamp.RabbitMQ;
using DMS.API.Errors;
using DMS.Services.Interfaces;
using DMS.Services.Repositories;
using DMS.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace DMS.API.Extensions
{
    public static class ApiRegistration
    {
        public static IServiceCollection AddApiRegistration(this IServiceCollection services)
        {

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            //configure token services
            services.AddSingleton<IRabbitMQService, RabbitMQService>();
            services.AddSingleton<MqConsumer>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<IDirectoryService, DirectoryService>();
            services.AddScoped<IWorkspaceService, WorkspaceService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IActionLogService, ActionLogService>();
            //Configure IFileProvider
            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));
            services.Configure<ApiBehaviorOptions>(opt =>
            {
                opt.InvalidModelStateResponseFactory = context =>
                {
                    var errorResponse = new ApiValidationErrorResponse
                    {
                        Errors = context.ModelState
                        .Where(x => x.Value.Errors.Count() > 0)
                        .SelectMany(x => x.Value.Errors)
                        .Select(x => x.ErrorMessage).ToArray()
                    };
                    return new BadRequestObjectResult(errorResponse);
                };
            });

            //Enable Cors
            // Configure CORS to allow requests from http://localhost:4200
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp", builder =>
                {
                    builder.WithOrigins("http://localhost:4200")
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                });
            });
            return services;
        }
    }
}
