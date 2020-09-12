using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConsoleTemplateCore
{
    public class App
    {
        private readonly ILogger<App> _logger;
        public App(ILogger<App> logger)
        {
            _logger = logger;
        }
        public async Task Run()
        {
            //await Task.Delay(1000);
            _logger.LogTrace("Hello!");
        }
    }
}
