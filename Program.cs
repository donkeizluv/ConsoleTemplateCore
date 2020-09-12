using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConsoleTemplateCore
{
    static class Program
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private const string APPSETTING_NAME = "appsettings.json";
        private static IServiceProvider BuildServiceProvider(IConfiguration config)
        {
            return new ServiceCollection()
               .AddSingleton<App>()
               .AddLogging(loggingBuilder =>
               {
                   loggingBuilder
                       .ClearProviders()
                       .SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace)
                       // MAGIC: this is not doing what its supposed to do
                       // need to use LogManager.Configuration
                       .AddNLog(config.GetSection(nameof(NLog)));
               })
               .BuildServiceProvider();
        }
        private static IConfiguration GetConfiguration(string configName)
        {
            return new ConfigurationBuilder()
                   .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                   .AddJsonFile(configName, optional: true, reloadOnChange: true)
                   .Build();
        }
        public static async Task Main(string[] args)
        {
            Console.WriteLine("::::::::::::::::::: START :::::::::::::::::::");
            try
            {
                var config = GetConfiguration(APPSETTING_NAME);
                LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));

                var servicesProvider = BuildServiceProvider(config);
                using (servicesProvider as IDisposable)
                {
                    var app = servicesProvider.GetRequiredService<App>();
                    await app.Run();
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
                //Console.ReadLine();
            }
            Console.WriteLine("::::::::::::::::::: END :::::::::::::::::::");
        }
    }
}
