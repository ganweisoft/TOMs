// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace IoTCenterWebApi.BaseCore;

public class WebGlobalExecptionFilter : IExceptionFilter
{
    private readonly ILoggingService _log;

    public WebGlobalExecptionFilter(ILoggingService log)
    {
        _log = log;
    }

    public void OnException(ExceptionContext context)
     {
        _log.Error($"系统内部错误异常：{context.Exception}");
        context.Result = new ObjectResult(new
        {
            Succeeded = false,
            Code = 400,
            Message = "系统内部异常，请联系平台管理人员处理",
            Description = "系统内部异常，请联系平台管理人员处理",
        });
        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.ExceptionHandled = true;
    }
}
