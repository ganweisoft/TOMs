// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IoTCenterWebApi.BaseCore;

public class CookieAuthenticationEventsExtensions : CookieAuthenticationEvents
{
    private const string TicketIssuedTicks = nameof(TicketIssuedTicks);

    public override async Task SigningIn(CookieSigningInContext context)
    {
        context.Properties.SetString(
            TicketIssuedTicks,
            DateTimeOffset.UtcNow.Ticks.ToString());

        await base.SigningIn(context);
    }

    public override async Task SigningOut(CookieSigningOutContext context)
    {
        var httpContext = context.HttpContext;

        var claims = context.HttpContext.User.Claims;

        var userName = claims.FirstOrDefault(m => m.Type == ClaimTypes.Name)?.Value;

        var gid = claims.FirstOrDefault(m => m.Type == ClaimTypes.Sid)?.Value;

        if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(gid))
        {
            PermissionCache.RemoveUserLogin(userName, gid);
        }

        await base.SigningOut(context);
    }

    public override async Task ValidatePrincipal(
        CookieValidatePrincipalContext context)
    {
        var claims = context.HttpContext.User.Claims;

        var userName = claims.FirstOrDefault(m => m.Type == ClaimTypes.Name)?.Value;

        if (!string.IsNullOrEmpty(userName) && !PermissionCache.CheckUserSession(userName))
        {
            await RejectPrincipalAsync(context);
            return;
        }

        var ticketIssuedTicksValue = context
            .Properties.GetString(TicketIssuedTicks);

        if (ticketIssuedTicksValue is null ||
            !long.TryParse(ticketIssuedTicksValue, out var ticketIssuedTicks))
        {
            await RejectPrincipalAsync(context);
            return;
        }

        var ticketIssuedUtc =
            new DateTimeOffset(ticketIssuedTicks, TimeSpan.FromHours(0));

        if (DateTimeOffset.UtcNow - ticketIssuedUtc > TimeSpan.FromDays(7))
        {
            await RejectPrincipalAsync(context);
            return;
        }

        await base.ValidatePrincipal(context);
    }

    private static async Task RejectPrincipalAsync(
        CookieValidatePrincipalContext context)
    {
        foreach (var key in context.Request.Cookies.Keys)
        {
            context.Response.Cookies.Delete(key);
        }

        context.RejectPrincipal();

        await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}
