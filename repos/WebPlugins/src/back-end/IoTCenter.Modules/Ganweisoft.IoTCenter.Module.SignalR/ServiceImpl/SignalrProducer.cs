// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using IoTCenterHost.Proxy.Core;
using IoTCenterWebApi.BaseCore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Ganweisoft.IoTCenter.Module.SignalR;

public class SignalrProducer
{
    private IConnectPoolManager _connectPoolManager;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILoggingService _apiLog;
    private readonly string YxpDataChanged = "YxpDataChanged";
    private readonly string YcpDataChanged = "YcpDataChanged";
    private readonly string EquipDataChanged = "EquipDataChanged";

    private IHubContext<MonitorHub> _monitorHubContext
    {
        get
        {
            return _serviceScopeFactory.CreateScope().ServiceProvider.GetService<IHubContext<MonitorHub>>();
        }
    }
    private PermissionCacheService _permissionCacheService
    {
        get
        {
            return _serviceScopeFactory.CreateScope().ServiceProvider.GetService<PermissionCacheService>();
        }
    }
    public SignalrProducer(IConnectPoolManager connectPoolManager, IServiceScopeFactory serviceProvider)
    {
        _connectPoolManager = connectPoolManager;
        _serviceScopeFactory = serviceProvider;
        _apiLog = _serviceScopeFactory.CreateScope().ServiceProvider.GetService<ILoggingService>();
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
        _monitorHubContext.Clients
            .Groups(e.m_iEquipNo.ToString())
            .SendAsync(YxpDataChanged,
            new List<SignalrPushYXData> { new SignalrPushYXData {
                equipNo=e.m_iEquipNo,
                yxNo=e.m_iYXNo,
                state=e.m_IsAlarm.b,
                value=e.m_YXState
            }
            }.ToJson());
    }

    private void EquipManager_OnYcValuedChanged(object sender, IoTCenterHost.Core.Abstraction.BaseModels.GrpcYcItem e)
    {
        object value = e.m_YCValue.s;
        if (value == null)
        {
            value = e.m_YCValue.f;
        }

        _monitorHubContext.Clients
            .Groups(e.m_iEquipNo.ToString())
            .SendAsync(YcpDataChanged,
           new List<SignalrPushYCData> { new SignalrPushYCData {
                equipNo=e.m_iEquipNo,
                unit=e.m_Unit,
                value=value,
                ycNo=e.m_iYCNo,
                state=e.m_IsAlarm
            }
            }.ToJson());
    }

    private void EquipManager_OnEquipStateChanged(object sender, IoTCenterHost.Core.Abstraction.ProxyModels.GrpcEquipStateItem e)
    {
        _monitorHubContext.Clients
            .Groups(e.m_iEquipNo.ToString())
            .SendAsync(EquipDataChanged,
           new SignalrPushEquipData { equipNo = e.m_iEquipNo, status = (int)e.m_State });
    }
}
