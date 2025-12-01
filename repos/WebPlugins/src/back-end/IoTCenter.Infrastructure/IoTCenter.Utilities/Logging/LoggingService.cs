// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using Serilog.Events;
using System;
using System.Threading.Tasks;

namespace IoTCenter.Utilities
{
    public class LoggingService : ILoggingService
    {
        private readonly HttpContext _httpContext;

        private readonly Serilogger _serilogger;

        public LoggingService(Serilogger serilogger, IHttpContextAccessor httpContextAccessor)
        {
            _serilogger = serilogger;
            _httpContext = httpContextAccessor?.HttpContext;
        }
        public void Debug(string message)
        {
            _serilogger.Logger.Debug(message);
        }

        public void Debug(string message, params object[] args)
        {
            _serilogger.Logger.Debug(message, args);
        }

        public void Debug(string message, Exception exception, params object[] args)
        {
            _serilogger.Logger.Debug(message, exception, args);
        }

        public void Debug(object item)
        {
            _serilogger.Logger.Debug(item.ToString());
        }

        public void Error(string message, params object[] args)
        {
            _serilogger.Logger.Error(message, args);
        }

        public void Error(Exception exception)
        {
            _serilogger.Logger.Error(exception, null);
        }

        public void Error(string message, Exception exception, params object[] args)
        {
            _serilogger.Logger.Error(exception, message, args);
        }

        public void Fatal(string message, params object[] args)
        {
            _serilogger.Logger.Fatal(message, args);
        }

        public void Fatal(string message, Exception exception, params object[] args)
        {
            _serilogger.Logger.Fatal(exception, message, args);
        }

        public void Info(string message, params object[] args)
        {
            _serilogger.Logger.Information(message);
        }

        public async Task Audit(AuditAction auditAction, params object[] args)
        {
            if (auditAction == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(auditAction.UserName))
            {
                auditAction.UserName = _httpContext.User?.Identity.Name;

                if (string.IsNullOrEmpty(auditAction.UserName))
                {
                    return;
                }
            }

            auditAction.TraceId = _httpContext?.TraceIdentifier;
            auditAction.RemoteIpAddress = _httpContext.Connection?.RemoteIpAddress?.ToString();
            auditAction.RemotePort = _httpContext.Connection?.RemotePort ?? 0;
            auditAction.RequestUrl = _httpContext?.Request?.GetDisplayUrl();

            if (auditAction.Result == null)
            {
                auditAction.Result = new AuditResult()
                {
                    Default = await AuditApiUtil.GetRequestBody(_httpContext)
                };
            }

            _serilogger.Logger.Warning($"产生关键操作：{auditAction.ToString()}", args);
        }

        public void Warn(string message, params object[] args)
        {
            _serilogger.Logger.Warning(message, args);
        }

        public bool IsDebugEnable()
        {
            return _serilogger.Logger.IsEnabled(LogEventLevel.Debug);
        }
    }
}
