namespace ClassroomServer
{
    using ClassroomServer.Logic; // Import ClassroomLogic
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using NLog;
    using System.Net;

    /// <summary>
    /// Entry point for the Classroom Server program, configuring and running the web application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Logger instance for logging application events and errors.
        /// </summary>
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Configures the logging subsystem using NLog.
        /// </summary>
        private static void ConfigureLogging()
        {
            var config = new NLog.Config.LoggingConfiguration();

            // Create console logging target
            var consoleTarget = new NLog.Targets.ConsoleTarget("console")
            {
                Layout = @"${date:format=HH\:mm\:ss}|${level}| ${message} ${exception}"
            };
            config.AddTarget(consoleTarget);
            config.AddRuleForAllLevels(consoleTarget);

            LogManager.Configuration = config;
        }

        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            var self = new Program();
            self.Run(args);
        }

        /// <summary>
        /// Main program execution body, configuring logging and starting the server.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        private void Run(string[] args)
        {
            // Configure logging
            ConfigureLogging();

            // Log that the server is about to start
            log.Info("Server is about to start");

            // Start the server
            StartServer(args);
        }

        /// <summary>
        /// Starts the web server and configures dependency injection and application services.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        private void StartServer(string[] args)
        {
            // Create the web app builder
            var builder = WebApplication.CreateBuilder(args);

            // Configure Kestrel to listen on specific IP and port
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Listen(IPAddress.Loopback, 5000); // Modify port as needed
            });

            // Add services to the container (Dependency Injection)
            builder.Services.AddControllers();

            // Register ClassroomLogic directly for dependency injection
            builder.Services.AddSingleton<ClassroomLogic>();

            // Add Swagger for API documentation
            builder.Services.AddSwaggerGen();

            // Build the application
            var app = builder.Build();

            // Enable Swagger UI for API documentation in development environment
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Enable HTTPS redirection if needed
            // app.UseHttpsRedirection();

            // Enable routing for controllers
            app.UseRouting();

            // Map controllers to handle API requests
            app.MapControllers();

            // Enable CORS (optional, modify as needed for application security requirements)
            app.UseCors(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });

            // Run the server
            app.Run();
        }
    }
}
