// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using IoTCenterHost.Core.Abstraction;
using IoTCenterHost.Core.Abstraction.AppServices;
using IoTCenterHost.Core.Abstraction.IotModels;
using IoTCenterWebApi.BaseCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.RealTime;

public class RealTimeCacheService : IRealTimeCacheService
{
    private readonly Session _session;
    private readonly IDistributedCache _cache;

    private readonly IAlarmEventClientAppService _alarmEventAppServiceImpl;

    private readonly EquipBaseImpl _equipBaseImpl;
    private readonly PermissionCacheService _permissionCacheService;

    public RealTimeCacheService(IDistributedCache cache,
                                IAlarmEventClientAppService alarmEventAppServiceImpl,
                                EquipBaseImpl equipBaseImpl,
                                Session session,
                                PermissionCacheService permissionCacheService)
    {
        _session = session;

        _cache = cache;

        _alarmEventAppServiceImpl = alarmEventAppServiceImpl;

        _equipBaseImpl = equipBaseImpl;
        _permissionCacheService = permissionCacheService;
    }

    public async Task<List<WcfRealTimeEventItem>> HandleRealTimeData(string role)
    {
        List<WcfRealTimeEventItem> actualItems;
        if (string.IsNullOrEmpty(role)) { await Task.CompletedTask; }

        actualItems = new List<WcfRealTimeEventItem>();
        var paginationData = _alarmEventAppServiceImpl.GetRealEventItems(new Pagination
        {
            PageIndex = 1,
            PageSize = 20,
        });
        var items = paginationData.Data.FromJson<List<WcfRealTimeEventItem>>();

        items = PermissionFilter(role, items);

        if (items.Count <= 0)
        {
            return actualItems;
        }

        var real_last_time = System.DateTime.Now.AddSeconds(-5);

        var realTimeString = await _cache.GetStringAsync("real_last_time");
        if (!string.IsNullOrEmpty(realTimeString))
        {
            real_last_time = DateTime.Parse(realTimeString);
        }
        actualItems = items.Where(u => u.Time > real_last_time).ToList();

        real_last_time = items.Max(m => m.Time);
        await _cache.SetStringAsync("real_last_time", real_last_time.ToString("yyyy-MM-dd HH:mm:ss"));

        return actualItems;
    }

    List<WcfRealTimeEventItem> PermissionFilter(string role, List<WcfRealTimeEventItem> items)
    {
        var roleItems = items.Where(d => d.Equipno > 0).ToList();

        if (role.ToUpperInvariant() == "ADMIN")
        {
            return roleItems;
        }

        var browseEquips = _permissionCacheService.GetPermissionObj(_session.RoleName)?.BrowseEquips ?? new List<int>();

        if (!browseEquips.Any())
        {
            return roleItems;
        }

        roleItems = roleItems.Where(d => browseEquips.Contains(d.Equipno)).ToList();

        return roleItems;
    }
}
