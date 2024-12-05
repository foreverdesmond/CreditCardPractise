using Microsoft.OpenApi.Models;
using CreditCard.API.ErrorHandling;
using CreditCard.API.Filters;
using System.Reflection;
using NLog;
using NLog.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var logger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();
        try
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

            // Register the Filter globally
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<LogFilter>();
                options.Filters.Add<ExceptionFilter>();
                options.Filters.Add<ModelValidationFilter>();
            });

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Credit Card API", Version = "v1" });
                // Add XML comments if available
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            builder.Services.AddHealthChecks();

            var app = builder.Build();

            app.UseMiddleware<GlobalExceptionHandler>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.MapHealthChecks("/health");

            app.Run();
        }
        catch (Exception exception)
        {
            logger.Error(exception, "Stopped program because of exception");
            throw;
        }
        finally
        {
            LogManager.Shutdown();
        }
    }
}