using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
            //services.AddLogging();
            services.AddSingleton<IConfigurationRoot>(Configuration);
            // Main() is moved to ProgramTransactionScript, so the Program.cs is only used for setting up DI
            services.AddSingleton<ProgramTransactionScript, ProgramTransactionScript>();
        }
    }
}
