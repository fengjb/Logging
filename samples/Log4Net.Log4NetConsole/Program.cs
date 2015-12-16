using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;
#if !DNXCORE50
using Microsoft.Extensions.Logging.Log4Net;
#endif

namespace Log4Net.Log4NetConsole
{
    public class Program
    {

        private static ILogger _logger;

        public Program()
        {
        }

        public static void Main(string[] args)
        {
            #region 代码方式启动log4net

#if !DNXCORE50
            string LOG_PATTERN = "%d [%t] %-5p %c [%x] - %m%n";
            string LOG_FILE_PATH = "D:\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";

            log4net.Repository.Hierarchy.Hierarchy hierarchy = (log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository();
            hierarchy.Name = "log4net";
            log4net.Appender.TraceAppender tracer = new log4net.Appender.TraceAppender();

            log4net.Layout.PatternLayout patternLayout = new log4net.Layout.PatternLayout();
            patternLayout.ConversionPattern = LOG_PATTERN;
            patternLayout.ActivateOptions();

            tracer.Layout = patternLayout;
            tracer.ActivateOptions();
            hierarchy.Root.AddAppender(tracer);

            log4net.Appender.RollingFileAppender roller = new log4net.Appender.RollingFileAppender();
            roller.Layout = patternLayout;
            roller.AppendToFile = true;
            roller.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Size;
            roller.MaxSizeRollBackups = 4;
            roller.MaximumFileSize = "100KB";
            roller.StaticLogFileName = true;
            roller.File = LOG_FILE_PATH;
            roller.ActivateOptions();
            hierarchy.Root.AddAppender(roller);

            hierarchy.Root.Level = log4net.Core.Level.All;
            hierarchy.Configured = true;

#endif
            #endregion


            #region 代码方式读取log4net配置文件，启动log4net
            //var logCfg = new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config");
            //log4net.Config.XmlConfigurator.ConfigureAndWatch(logCfg);
            #endregion



            // a DI based application would get ILoggerFactory injected instead
            var factory = new LoggerFactory();

            // getting the logger immediately using the class's name is conventional
            _logger = factory.CreateLogger<Program>();

            // providers may be added to an ILoggerFactory at any time, existing ILoggers are updated
#if !DNXCORE50
            factory.AddLog4Net(log4net.LogManager.GetLogger(typeof(Program)));
#endif

            _logger.LogInformation("Starting");

            var startTime = DateTimeOffset.Now;
            _logger.LogInformation(1, "Started at '{StartTime}' and 0x{Hello:X} is hex of 42", startTime, 42);
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

                    var isEnabled = _logger.IsEnabled(LogLevel.Error);
                    _logger.LogInformation("User typed '{input}' on the command line", input.ToString());
                    _logger.LogWarning("The time is now {Time}, it's getting late!", DateTimeOffset.Now);
                }
                while (input != "quit");
            }

            var endTime = DateTimeOffset.Now;
            _logger.LogInformation(2, "Stopping at '{StopTime}'", endTime);
            _logger.LogInformation("Stopping");
        }
    }
}
