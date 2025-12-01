// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Serilog.Events;
using System;
using System.Threading.Tasks;

namespace IoTCenter.Utilities
{
    public interface ILoggingService
    {
        void Debug(string message);
        void Debug(string message, params object[] args);
        void Debug(string message, Exception exception, params object[] args);
        void Debug(object item);
        void Error(string message, params object[] args);
        void Error(Exception exception);
        void Error(string message, Exception exception, params object[] args);
        void Fatal(string message, params object[] args);
        void Fatal(string message, Exception exception, params object[] args);
        void Info(string message, params object[] args);
        Task Audit(AuditAction auditAction, params object[] args);
        void Warn(string message, params object[] args);

        bool IsDebugEnable();

    }
}
