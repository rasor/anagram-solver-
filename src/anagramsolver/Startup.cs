using anagramsolver.containers;
using anagramsolver.models;
using anagramsolver.services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace anagramsolver
{
    /// <summary>
    /// https://stackoverflow.com/a/41411310/750989
    /// https://pioneercode.com/post/dependency-injection-logging-and-configuration-in-a-dot-net-core-console-app
    /// </summary>
    public class Startup
    {
        IConfigurationRoot Configuration { get; }

        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add basic/common services
            //services.AddLogging();
            services.AddSingleton<IConfigurationRoot>(Configuration);

            // Add Main() dependencies
            services.AddTransient<ConsoleLogger, ConsoleLogger>();
            services.AddSingleton<IAnagramContainer, AnagramContainer>();
            services.AddSingleton<IWordlistContainer, WordlistContainer>();
            services.AddSingleton(MD5.Create());
            services.AddSingleton<LoopSetsHelper<CurrentSetOf2Pos>, LoopSetsHelper<CurrentSetOf2Pos>>();
            services.AddSingleton<LoopSetsHelper<CurrentSetOf3Pos>, LoopSetsHelper<CurrentSetOf3Pos>>();
            services.AddSingleton<LoopSetsHelper<CurrentSetOf4Pos>, LoopSetsHelper<CurrentSetOf4Pos>>();
            services.AddSingleton<LoopSetsHelper<CurrentSetOf5Pos>, LoopSetsHelper<CurrentSetOf5Pos>>();

            // Main() is moved to ProgramTransactionScript, so the Program.cs is only used for setting up DI
            services.AddSingleton<ProgramTransactionScript, ProgramTransactionScript>();
        }
    }
}
