// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using IoTCenterHost.Proxy.Core;
using IoTCenterWebApi.BaseCore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ganweisoft.IoTCenter.Module.EquipList;

public class YcYxEquipStatusChangeNotify
{
    private readonly IConnectPoolManager _connectPoolManager;
    private readonly ILoggingService _apiLog;
    private readonly string YxpDataChanged = "yxStatusChange";
    private readonly string YcpDataChanged = "ycStatusChange";
    private readonly string EquipChangeStatus = "GetEquipChangeStatus";

    private readonly IHubContext<EquipStatusMonitorHub> _equipStatusMonitorContext;

    private readonly IHubContext<PointStatusMonitorHub> _pointStatusMonitorContext;

    private readonly PermissionCacheService _permissionCacheService;

    public YcYxEquipStatusChangeNotify(IConnectPoolManager connectPoolManager, IServiceScopeFactory serviceProvider)
    {
        _connectPoolManager = connectPoolManager;
        using var scope = serviceProvider.CreateScope();
        _apiLog = scope.ServiceProvider.GetService<ILoggingService>();
        _equipStatusMonitorContext = scope.ServiceProvider.GetService<IHubContext<EquipStatusMonitorHub>>();
        _pointStatusMonitorContext = scope.ServiceProvider.GetService<IHubContext<PointStatusMonitorHub>>();
        _permissionCacheService = scope.ServiceProvider.GetService<PermissionCacheService>();
        SubscripeEvents();
    }

    public void SubscripeEvents()
    {
        _connectPoolManager.EquipManager.OnEquipStateChanged += EquipManager_OnEquipStateChanged;
        _connectPoolManager.EquipManager.OnYcValuedChanged += EquipManager_OnYcValuedChanged;
        _connectPoolManager.EquipManager.OnYxValuedChanged += EquipManager_OnYxValuedChanged;
    }

    private void EquipManager_OnYxValuedChanged(object sender, IoTCenterHost.Core.Abstraction.BaseModels.GrpcYxItem e)
    {
        if (!PointStatusMonitorHub.cache.Values.Any(m => m == e.m_iEquipNo.ToString())) return;

        _pointStatusMonitorContext.Clients
            .Groups(e.m_iEquipNo.ToString())
            .SendAsync(YxpDataChanged,
            HubMsgModel.Success(new List<YxChangeValueModel> { new YxChangeValueModel {
                EquipNo=e.m_iEquipNo,
                YxNo=e.m_iYXNo,
                IsAlarm=e.m_IsAlarm.b,
                Value=e.m_YXState
            }
            }));
    }

    private void EquipManager_OnYcValuedChanged(object sender, IoTCenterHost.Core.Abstraction.BaseModels.GrpcYcItem e)
    {
        if (!PointStatusMonitorHub.cache.Values.Any(m => m == e.m_iEquipNo.ToString())) return;

        object value = e.m_YCValue.s;
        if (value == null)
        {
            value = e.m_YCValue.f;
        }
        _pointStatusMonitorContext.Clients
            .Groups(e.m_iEquipNo.ToString())
            .SendAsync(YcpDataChanged,
            HubMsgModel.Success(new List<YcChangeValueModel> { new YcChangeValueModel {
                EquipNo=e.m_iEquipNo,
                Unit=e.m_Unit,
                Value=value,
                YcNo=e.m_iYCNo,
                IsAlarm=e.m_IsAlarm
            }
            }));
    }

    private void EquipManager_OnEquipStateChanged(object sender, IoTCenterHost.Core.Abstraction.ProxyModels.GrpcEquipStateItem e)
    {
        var roleNames = _permissionCacheService.GetPermissionObjList().Where(x => x.BrowseEquips.Contains(e.m_iEquipNo)).Select(x => x.Name);
        _equipStatusMonitorContext.Clients
            .Groups(new List<string>(roleNames) { "admin" })
            .SendAsync(EquipChangeStatus,
           HubMsgModel.Success(new EquipChangeStatusModel { equipNo = e.m_iEquipNo, status = (int)e.m_State }));
    }
}
