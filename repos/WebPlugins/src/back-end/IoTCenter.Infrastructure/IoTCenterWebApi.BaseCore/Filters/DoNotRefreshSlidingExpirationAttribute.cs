// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTCenterWebApi.BaseCore;

public class DoNotRefreshSlidingExpirationAttribute : ActionFilterAttribute
{
    public const string DontRenewSlidingCookie = "dontRenewSlidingCookie";
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        context.HttpContext.Items.Add(DontRenewSlidingCookie, true);
    }
}
