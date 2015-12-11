using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;
#if !DNXCORE50
using Microsoft.Extensions.Logging.Log4Net;
#endif

namespace Log4netConsole
{
    public class Program
    {
        private readonly ILogger _logger;

        public Program()
        {
            // a DI based application would get ILoggerFactory injected instead
            var factory = new LoggerFactory();

            // getting the logger immediately using the class's name is conventional
            _logger = factory.CreateLogger<Program>();

            // providers may be added to an ILoggerFactory at any time, existing ILoggers are updated
#if !DNXCORE50
            factory.AddLog4Net(log4net.LogManager.GetLogger(typeof(Program)));
            factory.AddEventLog();
#endif

            // How to configure the console logger to reload based on a configuration file.
            //
            //
            var loggingConfiguration = new ConfigurationBuilder().AddJsonFile("logging.json").Build();
            factory.AddConsole(loggingConfiguration);
            loggingConfiguration.ReloadOnChanged("logging.json");

        }

        public void Main(string[] args)
        {
            _logger.LogInformation("Starting");

            var startTime = DateTimeOffset.Now;
            _logger.LogInformation(1, "Started at '{StartTime}' and 0x{Hello:X} is hex of 42", startTime, 42);
            // or
            //_logger.ProgramStarting(startTime, 42);

            //using (_logger.PurchaseOrderScope("00655321"))
            //{
            try
            {
                throw new Exception("Boom");
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Unexpected critical error starting application", ex);
                _logger.LogError("Unexpected error", ex);
                _logger.LogWarning("Unexpected warning", ex);
            }

            using (_logger.BeginScopeImpl("Main"))
            {

                _logger.LogInformation("Waiting for user input");

                string input;
                do
                {
                    Console.WriteLine("Enter some test to log more, or 'quit' to exit.");
                    input = Console.ReadLine();

                    _logger.LogInformation("User typed '{input}' on the command line", input);
                    _logger.LogWarning("The time is now {Time}, it's getting late!", DateTimeOffset.Now);
                }
                while (input != "quit");
            }
            //}

            var endTime = DateTimeOffset.Now;
            _logger.LogInformation(2, "Stopping at '{StopTime}'", endTime);
            // or
            //_logger.ProgramStopping(endTime);

            _logger.LogInformation("Stopping");
            Console.ReadLine();
        }
    }
}
