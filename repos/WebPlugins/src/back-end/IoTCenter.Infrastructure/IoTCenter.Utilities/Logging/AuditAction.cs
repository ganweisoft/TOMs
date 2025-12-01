// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTCenter.Utilities
{
    public class AuditAction
    {
        public AuditAction()
        {

        }

        public AuditAction(string userName = null)
        {
            UserName = userName;
        }

        public string TraceId { get; internal set; }
        public string RemoteIpAddress { get; internal set; }
        public int RemotePort { get; internal set; }
        public string UserName { get; internal set; }
        public string RequestUrl { get; internal set; }
        public string ResourceName { get; set; }
        public string EventType { get; set; }
        public AuditResult Result { get; set; }

        public override string ToString()
        {
            return $"用户Id：{UserName}，发起地址：{RemoteIpAddress}，请求URL：{RequestUrl}，资源名称：{ResourceName}，事件类型：{EventType}，事件处理结果：{Result.Default}-{(Result != null ? Result.ToJson() : "")}";
        }
    }


    public class AuditResult
    {
        public string Default { get; set; }
    }


    public class AuditResult<T1, T2> : AuditResult
    {
        public T1 Old { get; set; }
        public T2 New { get; set; }
    }
}
