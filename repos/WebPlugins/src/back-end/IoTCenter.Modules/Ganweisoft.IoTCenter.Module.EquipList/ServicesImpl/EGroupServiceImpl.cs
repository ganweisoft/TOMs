// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Data;
using IoTCenterHost.Core.Abstraction.Services;
using IoTCenterWebApi.BaseCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.EquipList;

public class EGroupServiceImpl : IEGroupService
{

    private readonly GWDbContext _context;
    private readonly PermissionCacheService _permissionCacheService;  
    private readonly IotCenterHostService _proxy;
    private readonly EGroupStaticStruct _struct;                  // 缓存功能
    private static readonly List<EGroupStructResponse> _rootCache = new List<EGroupStructResponse>();   // 根缓存响应
    private static int _rootCacheEquipTotalCount;           // 设备总数

    public EGroupServiceImpl(GWDbContext context,
        PermissionCacheService permissionCacheService,
        IotCenterHostService alarmCenterService)
    {
        _context = context;
        _permissionCacheService = permissionCacheService;
        _struct = new EGroupStaticStruct(context);
        _proxy = alarmCenterService;
    }


    #region 接口实现区

    public (int Count, IEnumerable<EGroupStructResponse> Result) GetRoot(GroupListRequest model, string userName)
    {
        const int Count = 20;
        if (model.PageNo == 0 || model.PageSize == 0)
        {
            model.PageNo = 1;
            model.PageSize = 5;
        }

        if (!_rootCache.Any() || EGroupStaticStruct.NeedUpdate)
        {
            EGroupStaticStruct.NeedUpdate = false;

            (int count, OneEGroup[] list) = EGroupStaticStruct.GetRootGroup(model.PageNo, model.PageSize);
            _rootCacheEquipTotalCount = count;

            foreach (var item in list)
            {
                _rootCache.Add(new EGroupStructResponse
                {
                    GroupId = item.GroupId,
                    GroupName = item.GroupName,
                    ParentGroupId = item.ParentGroupId,
                    Children = item.Children.Select(x => new EGroupStructResponse.Child
                    {
                        GroupId = x.GroupId,
                        GroupName = x.GroupName
                    }).ToList(),
                    EquipTotalCount = item.Equips.Count,
                    Equips = item.Equips.Take(Count).ToArray()
                });
            }
        }

        IEnumerable<int> userCanSeeEquips = _permissionCacheService.GetPermissionObj(userName)?.BrowseEquips;
        var groupList = _rootCache.Skip((model.PageNo - 1) * model.PageSize).Take(model.PageSize);
        foreach (var item in groupList)
        {
            var tmp = item.Equips;
            if (!userName.Equals("admin", StringComparison.OrdinalIgnoreCase))
            {
                tmp = tmp.Where(e => userCanSeeEquips.Contains(e.EquipNo)).ToArray();
                item.Equips = tmp;
            }
        }

        Dictionary<int, OneEGroupEquip> dic = new Dictionary<int, OneEGroupEquip>();
        foreach (var item in groupList)
        {
            item.Equips.AsParallel().ForAll(item =>
            {
                dic.Add(item.EquipNo, item);
            });
        }

        try
        {
            var states = _proxy.GetEquipStateDict(dic.Keys);
            foreach (var item in dic)
            {
                if (states.TryGetValue(item.Key, out var state))
                    item.Value.EquipState = states;
            }
        }
        catch
        {
            return (_rootCacheEquipTotalCount, _rootCache);
        }
        return (_rootCacheEquipTotalCount, _rootCache);
    }


    public EGroupStructResponse GetOneGroup(OneGroupListRequest model, string userName)
    {
        IEnumerable<int> userCanSeeEquips = _permissionCacheService.GetPermissionObj(userName)?.BrowseEquips;

        OneEGroup eGroup = EGroupStaticStruct.GetOneGroup(model.GroupId);
        if (eGroup == null)
            return default;

        IEnumerable<OneEGroupEquip> equipList = eGroup.Equips;
        if (!userName.Equals("admin", StringComparison.OrdinalIgnoreCase))
            equipList = equipList.Where(e => userCanSeeEquips.Contains(e.EquipNo));

        EGroupStructResponse response = new EGroupStructResponse()
        {
            GroupId = eGroup.GroupId,
            GroupName = eGroup.GroupName,
            ParentGroupId = eGroup.ParentGroupId,
            Children = eGroup.Children.Select(x => new EGroupStructResponse.Child
            {
                GroupId = x.GroupId,
                GroupName = x.GroupName
            }).ToList(),
            EquipTotalCount = equipList.Count(),
            Equips = equipList.Skip((model.EquipPageNo - 1) * model.EquipPageSize).Take(model.EquipPageSize).ToArray()
        };

        Dictionary<int, OneEGroupEquip> dic = new Dictionary<int, OneEGroupEquip>();
        equipList.AsParallel().ForAll(item =>
        {
            dic.Add(item.EquipNo, item);
        });

        try
        {
            var states = _proxy.GetEquipStateDict(dic.Keys);
            foreach (var item in dic)
            {
                if (states.TryGetValue(item.Key, out var state))
                    item.Value.EquipState = states;
            }
        }
        catch
        {
            return response;
        }
        return response;
    }

    #region 设备搜索相关结果
    public async Task<(int Count, IEnumerable<EGroupSearchResponse>)> SearchEquip(string userName, string equipName, int pageNo, int pageSize)
    {
        IEnumerable<int> userCanSeeEquips = _permissionCacheService.GetPermissionObj(userName)?.BrowseEquips;

        var equipQuery = _context.Equip
            .Select(x => new EquipSearchTmp { EquipNo = x.EquipNo, EquipNm = x.EquipNm })
            .Where(x => x.EquipNm.Contains(equipName) || equipName.Contains(x.EquipNm));

        var Count = equipQuery.CountAsync();

        if (!userName.Equals("admin", StringComparison.OrdinalIgnoreCase))
            equipQuery = equipQuery.Where(x => userCanSeeEquips.Contains(x.EquipNo));

        return (await Count, await Search(equipQuery, pageNo, pageSize));
    }

    public async Task<(int Count, IEnumerable<EGroupSearchResponse>)> SearchSystem(string userName, string systemName, int pageNo, int pageSize)
    {
        IEnumerable<int> userCanSeeEquips = _permissionCacheService.GetPermissionObj(userName)?.BrowseEquips;

        var equipQuery = _context.IoTDevice.Join(_context.Equip, iot => iot.EquipNo, e => e.EquipNo, (iot, e) => new EquipSearchTmp
        {
            EquipNo = e.EquipNo,
            EquipNm = e.EquipNm,
            SystemName = iot.SystemName
        }).Where(x => x.SystemName.Contains(systemName) || systemName.Contains(x.SystemName));

        var Count = equipQuery.CountAsync();

        if (!userName.Equals("admin", StringComparison.OrdinalIgnoreCase))
            equipQuery = equipQuery.Where(x => userCanSeeEquips.Contains(x.EquipNo));

        return (await Count, await Search(equipQuery, pageNo, pageSize));
    }

    #endregion

    #endregion

    #region 搜索功能的实现，私有函数
    private async Task<IEnumerable<EGroupSearchResponse>> Search(IQueryable<EquipSearchTmp> query, int pageNo, int pageSize)
    {
        var groupQuery = _context.EGroupList.Join(query, g => g.EquipNo, e => e.EquipNo, (g, e) => new
        {
            g.GroupId,
            e.EquipNo,
            e.EquipNm
        });

        var group = _context.EGroup.Join(groupQuery, g => g.GroupId, e => e.GroupId, (g, e) => new EGroupSearchResponse
        {
            GroupId = g.GroupId,
            GroupName = g.GroupName,
            EquipNo = e.EquipNo,
            EquipNm = e.EquipNm
        });

        return await group.Skip((pageNo - 1) * pageSize).Take(pageSize).ToArrayAsync();
    }

    #endregion

    #region 临时匿名结构

    private class EquipSearchTmp
    {
        public int EquipNo { get; set; }
        public string EquipNm { get; set; }

        public string SystemName { get; set; }
    }

    #endregion
}
