// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Security.Claims;

namespace IoTCenterWebApi.BaseCore;

public class PermissionHandlerAttribute : ActionFilterAttribute
{
    private readonly IConfiguration _configuration;

    private readonly PermissionCacheService _permissionCacheService;

    public PermissionHandlerAttribute(
        PermissionCacheService permissionCacheService,
        IConfiguration configuration)
    {
        _permissionCacheService = permissionCacheService;
        _configuration = configuration;
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        var hasAllowAnonymous = context.ActionDescriptor.EndpointMetadata
                             .Any(meta => meta.GetType() == typeof(AllowAnonymousAttribute));

        if (hasAllowAnonymous)
        {
            return;
        }

        var claims = context.HttpContext?.User?.Claims;

        if (claims == null || !claims.Any())
        {
            context.Result = new Http401Result();
            return;
        }

        var userName = claims.FirstOrDefault(m => m.Type == ClaimTypes.Name)?.Value;
        var roleName = claims.FirstOrDefault(m => m.Type == ClaimTypes.Role)?.Value;
        var userSessionGid = claims.FirstOrDefault(m => m.Type == ClaimTypes.Sid)?.Value;

        _ = bool.TryParse(_configuration["WebApi:IsManyLoginEnabled"], out var allowSameAccountLogin);

        if (string.IsNullOrWhiteSpace(userName))
        {
            context.Result = new Http401Result();
        }
        else if (!allowSameAccountLogin && PermissionCache.CheckUsrHasLoginedOrHasUpdated(userName, userSessionGid))
        {
            context.Result = new Http401Result();
        }

        base.OnActionExecuted(context);
    }

    private class Http401Result : ActionResult
    {
        public override void ExecuteResult(ActionContext context)
        {
            context.HttpContext.Response.StatusCode = 401;
        }
    }

    private class Http403Result : ActionResult
    {
        public override void ExecuteResult(ActionContext context)
        {
            context.HttpContext.Response.StatusCode = 403;
        }
    }
}