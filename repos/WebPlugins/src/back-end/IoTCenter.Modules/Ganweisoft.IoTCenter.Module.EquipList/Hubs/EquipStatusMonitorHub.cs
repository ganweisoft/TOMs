// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Data;
using IoTCenter.Utilities;
using IoTCenter.Utilities.Extensions;
using IoTCenterHost.Core.Abstraction.BaseModels;
using IoTCenterHost.Core.Abstraction.Services;
using IoTCenterWebApi.BaseCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.EquipList;

[Authorize(AuthenticationSchemes = "Bearer,Cookies")]
public class EquipStatusMonitorHub : Hub
{
    private readonly ILoggingService _apiLog;
    private readonly Session _session;
    private readonly GWDbContext _dbContext;
    private readonly PermissionCacheService _permissionCacheService;
    private readonly IotCenterHostService _iotCenterHostService;

    public EquipStatusMonitorHub(
        Session session,
        ILoggingService apiLog,
        GWDbContext dbContext,
        IotCenterHostService iotCenterHostService,
        PermissionCacheService permissionCacheService
        )
    {
        _apiLog = apiLog;
        _session = session;
        _dbContext = dbContext;
        _permissionCacheService = permissionCacheService;
        _iotCenterHostService = iotCenterHostService;
    }

    public override async Task OnConnectedAsync()
    {
        string method = "OnConnected";
        if (!await CheckSession(method))
        {
            return;
        }
        try
        {
            await base.OnConnectedAsync();
            await SendMessage(method, HubMsgModel.Success());
        }
        catch (Exception ex)
        {
            _apiLog.Error($"On Connect EquipStatusMonitor SignalR Throw Exception:{ex}");
            await SendMessage(method, HubMsgModel.Error($"On Connect EquipStatusMonitor SignalR Throw Exception:{ex}"));
        }
    }


    private static ConcurrentDictionary<string, HubSessionInfo> cache = new ConcurrentDictionary<string, HubSessionInfo>();

    public async Task GetAllEquipStatus()
    {
        if (!await CheckSession(nameof(GetAllEquipStatus)))
        {
            return;
        }
        Dictionary<int, GrpcEquipState> equipStatus;
        try
        {
            if (_session.IsAdmin)
            {
                equipStatus = _iotCenterHostService.GetEquipStateDict();
            }
            else
            {
                var browseEquips = _permissionCacheService.GetPermissionObj(_session.RoleName)?.BrowseEquips ?? new List<int>();
                equipStatus = _iotCenterHostService.GetEquipStateDict(browseEquips);
            }
        }
        catch (Exception ex)
        {
            _apiLog.Error($"网关获取设备状态接口 异常:{ex}");
            await SendMessage(nameof(GetAllEquipStatus), HubMsgModel.Error(ex.Message));
            return;
        }

        await SendMessage(nameof(GetAllEquipStatus), HubMsgModel.Success(equipStatus));
    }

    public async Task GetEquipChangeStatus()
    {
        if (!cache.TryGetValue(Context.ConnectionId, out var hubSessionInfo))
        {
            if (!await CheckSession(nameof(GetEquipChangeStatus)))
            {
                return;
            }
            cache[Context.ConnectionId] = new HubSessionInfo
            {
                IsAdmin = _session.IsAdmin,
                RoleName = _session.IsAdmin ? _session.RoleName.ToLower() : _session.RoleName,
            };
            await Groups.AddToGroupAsync(Context.ConnectionId, cache[Context.ConnectionId].RoleName);
        }
        await SendMessage(nameof(GetEquipChangeStatus), HubMsgModel.Success());
    }


    public override async Task OnDisconnectedAsync(Exception exception)
    {
        if (cache.TryRemove(Context.ConnectionId, out var role))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, role.RoleName);

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
