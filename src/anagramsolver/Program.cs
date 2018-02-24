using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;

namespace anagramsolver
{
    class Program
    {
        /// <summary>
        /// https://stackoverflow.com/a/41411310/750989
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Setup Dependency Injection
            IServiceCollection services = new ServiceCollection();
            Startup startup = new Startup();
            startup.ConfigureServices(services);
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            //ConfigureLogging(serviceProvider);

            // Main() is moved to ProgramTransactionScript, so the Program.cs is only used for setting up DI
            var mainService = serviceProvider.GetService<ProgramTransactionScript>();
            mainService.Main(args);
        }

        private static void ConfigureLogging(IServiceProvider serviceProvider)
        {
            //configure console logging
            serviceProvider
                .GetService<ILoggerFactory>()
                // Write to Console
                .AddConsole(LogLevel.Information);
            // Write to debugger
            //.AddDebug(LogLevel.Trace);
            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();
            logger.LogError("Logging Error");
        }
    }
}
