// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Data;
using IoTCenter.Utilities;
using IoTCenter.Utilities.Extensions;
using IoTCenterWebApi.BaseCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.EquipList;

[Authorize(AuthenticationSchemes = "Bearer,Cookies")]
public class PointStatusMonitorHub : Hub
{
    private readonly ILoggingService _apiLog;
    private readonly Session _session;
    private readonly GWDbContext _dbContext;
    private readonly PermissionCacheService _permissionCacheService;

    public PointStatusMonitorHub(
        Session session,
        ILoggingService apiLog,
        GWDbContext dbContext,
        PermissionCacheService permissionCacheService
        )
    {
        _apiLog = apiLog;
        _session = session;
        _dbContext = dbContext;
        _permissionCacheService = permissionCacheService;
    }

    public override async Task OnConnectedAsync()
    {
        string method = "OnConnected";
        if (!await CheckSession(method))
        {
            return;
        }
        await base.OnConnectedAsync();
        await SendMessage(method, HubMsgModel.Success());
    }

    public async Task ConnectEquipNo(int equipNo)
    {
        if (equipNo <= 0)
        {
            await SendMessage(nameof(ConnectEquipNo), HubMsgModel.Error("设备编号必须大于0"));
        }

        if (!_session.IsAdmin)
        {
            var browseEquips = _permissionCacheService.GetPermissionObj(_session.RoleName)?.BrowseEquips ?? new List<int>();
            if (!browseEquips.Contains(equipNo))
            {
                await SendMessage(nameof(ConnectEquipNo), HubMsgModel.Error("没有权限浏览此设备"));
                return;
            }
        }

        var equipNoStr = equipNo.ToString();
        if (cache.TryGetValue(Context.ConnectionId, out var equipNoGroup))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, equipNoGroup);
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, equipNoStr);
        cache[Context.ConnectionId] = equipNoStr;
    }

    public static ConcurrentDictionary<string, string> cache = new ConcurrentDictionary<string, string>();


    public override async Task OnDisconnectedAsync(Exception exception)
    {
        if (cache.TryGetValue(Context.ConnectionId, out var equipNo))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, equipNo);
            cache.TryRemove(Context.ConnectionId, out _);
        }
        await base.OnDisconnectedAsync(exception);
    }

    private async Task SendMessage(string method, HubMsgModel model)
    {
        await Clients.Clients(Context.ConnectionId).SendAsync(method, model);
    }

    private async Task<bool> CheckSession(string method)
    {
        var userName = _session.UserName;
        if (userName.IsEmpty())
        {
            await SendMessage(method, HubMsgModel.Error("会话用户名获取失败"));
            return false;
        }

        var roleName = _session.RoleName;
        if (roleName.IsEmpty())
        {
            await SendMessage(method, HubMsgModel.Error("会话用户名获取失败"));
            return false;
        }
        return true;
    }
}
