// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace IoTCenter.Utilities
{
    public class Serilogger
    {
        private readonly string LogPath;
        public ILogger Logger { get; private set; }

        private static bool hasInitLog = false;

        public Serilogger()
        {
            LogPath = Path.Combine(AppContext.BaseDirectory, "Logs");
            if (hasInitLog)
            {
                Logger = Log.Logger;
                return;
            }
            var _configuration = new ConfigurationBuilder()
             .SetBasePath(AppContext.BaseDirectory)
             .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
             .Add(new
             JsonConfigurationSource
             { Path = "logsetting.json", ReloadOnChange = true })
             .Build();
            string strLevel = _configuration.GetSection("Serilog:MinimumLevel").Value;
            string msLevel = _configuration.GetSection("Logging:LogLevel:Default").Value;


            var logEventLevel = (LogEventLevel)Enum.Parse(typeof(LogEventLevel), strLevel);
            var msEventLevel = (LogEventLevel)Enum.Parse(typeof(LogEventLevel), msLevel);

            Log.Logger = Logger = new LoggerConfiguration()
               .MinimumLevel.Override("Microsoft", msEventLevel)
               .MinimumLevel.Override("Microsoft.AspNetCore", msEventLevel)
               .MinimumLevel.Override("Quartz", msEventLevel)
               .MinimumLevel.Override("Grpc", msEventLevel)
               .MinimumLevel.Override("System", msEventLevel)
               .ReadFrom.Configuration(_configuration)
               .WriteTo.Map(le => new Tuple<DateTime, LogEventLevel>(
                new DateTime(le.Timestamp.Year, le.Timestamp.Month, le.Timestamp.Day, le.Timestamp.Hour, le.Timestamp.Minute, le.Timestamp.Second), le.Level),
                (key, log) => log.File(
                    Path.Combine(AppContext.BaseDirectory, "Logs", $"{key.Item1:yyyy-MM-dd}/{key.Item2}-{key.Item1:yyyyMMdd}.txt"),
                    outputTemplate: _configuration.GetSection("Serilog:WriteTo:0:Args:outputTemplate").Value,
                    fileSizeLimitBytes: long.Parse(_configuration.GetSection("Serilog:WriteTo:0:Args:fileSizeLimitBytes").Value),
                    rollOnFileSizeLimit: Convert.ToBoolean(_configuration.GetSection("Serilog:WriteTo:0:Args:rollOnFileSizeLimit").Value),
                    retainedFileCountLimit: Convert.ToInt32(_configuration.GetSection("Serilog:WriteTo:0:Args:retainedFileCountLimit").Value),
                    shared: true
                ),
                sinkMapCountLimit: 1)
               .CreateLogger();
            hasInitLog = true;
        }
    }
}