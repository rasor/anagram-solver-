using System;
using anagramsolver.containers;
using anagramsolver.models;
using System.Linq;
using System.Security.Cryptography;
using anagramsolver.helpers;
using Microsoft.Extensions.DependencyInjection;

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

            ////configure console logging
            //serviceProvider
            //    .GetService<ILoggerFactory>()
            //    .AddConsole(LogLevel.Debug);

            //var logger = serviceProvider.GetService<ILoggerFactory>()
            //    .CreateLogger<Program>();

            //logger.LogDebug("Logger is working!");

            // Main() is moved to ProgramTransactionScript, so the Program.cs is only used for setting up DI
            var mainService = serviceProvider.GetService<ProgramTransactionScript>();
            mainService.Main(args);
        }
    }
}
