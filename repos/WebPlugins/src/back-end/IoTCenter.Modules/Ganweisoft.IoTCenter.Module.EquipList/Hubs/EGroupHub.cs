// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Data;
using IoTCenter.Data.Model;
using IoTCenter.Utilities;
using IoTCenter.Utilities.Extensions;
using IoTCenterHost.Core.Abstraction.Services;
using IoTCenterWebApi.BaseCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.EquipList;

[Authorize(AuthenticationSchemes = "Bearer,Cookies")]
public class EGroupHub : Hub
{
    private readonly ILoggingService _apiLog;
    private readonly Session _session;
    private readonly GWDbContext _dbContext;
    private readonly PermissionCacheService _permissionCacheService;
    private readonly IotCenterHostService _proxy;

    public EGroupHub(ILoggingService apiLog,
        Session session,
        GWDbContext dbContext,
        PermissionCacheService permissionCacheService,
        IotCenterHostService proxy)
    {
        _apiLog = apiLog;
        _session = session;
        _dbContext = dbContext;
        _permissionCacheService = permissionCacheService;
        _proxy = proxy;
    }

    public override async Task OnConnectedAsync()
    {
        string method = "OnConnected";

        if (!await CheckSession(method))
        {
            return;
        }

        await Clients.Clients(Context.ConnectionId).SendAsync(method, HubMsgModel.Success(null));

        await base.OnConnectedAsync();
    }

    private static ConcurrentDictionary<string, HubSessionInfo> cache = new ConcurrentDictionary<string, HubSessionInfo>();

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        if (cache.TryRemove(Context.ConnectionId, out var role))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, role.RoleName);
            cache.TryRemove(Context.ConnectionId, out _);
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task GetEquipGroupTree()
    {
        var method = nameof(GetEquipGroupTree);

        if (!await CheckSession(method))
        {
            return;
        }

        var result = await GetGroupTree(EGroupQueryEnum.List);

        await SendAsync(method, HubMsgModel.Success(result));
    }

    public async Task GetGroupEquips()
    {
        var method = nameof(GetGroupEquips);

        if (!cache.TryGetValue(Context.ConnectionId, out var hubSessionInfo))
        {
            if (!await CheckSession(method))
            {
                return;
            }
            cache[Context.ConnectionId] = new HubSessionInfo
            {
                IsAdmin = _session.IsAdmin,
                RoleName = _session.RoleName,
            };
            hubSessionInfo = cache[Context.ConnectionId];
        }

        var equipGroupListQuery = _dbContext.EGroupList.AsNoTracking();

        if (!hubSessionInfo.IsAdmin)
        {
            var browseEquips = _permissionCacheService.GetPermissionObj(hubSessionInfo.RoleName)?.BrowseEquips ?? new List<int>();
            equipGroupListQuery = equipGroupListQuery.Where(x => browseEquips.Contains(x.EquipNo));
        }
        await Groups.AddToGroupAsync(Context.ConnectionId, hubSessionInfo.RoleName);

        if (!await equipGroupListQuery.AnyAsync())
        {
            await SendAsync(method, HubMsgModel.Success(new List<EGroupHubEquipModel>()));
            return;
        }

        var equipModels = from egList in equipGroupListQuery
                          join eq in _dbContext.Equip.AsNoTracking() on egList.EquipNo equals eq.EquipNo
                          select new
                          {
                              Id = eq.EquipNo,
                              Name = eq.EquipNm,
                              GroupId = egList.GroupId
                          };
        var vals = await equipModels.ToListAsync();

        foreach (var item in vals.GroupBy(x => x.GroupId))
        {
            await SendAsync(method, HubMsgModel.Success(new EGroupHubGroupModel
            {
                GroupId = item.Key,
                Equips = item.Select(x => new EGroupHubEquipModel
                {
                    Id = x.Id,
                    Name = x.Name,
                })
            }));
        }
    }

    public async Task GetAllEquipGroupTree()
    {
        var method = nameof(GetAllEquipGroupTree);

        if (!await CheckSession(method))
        {
            return;
        }

        var result = await GetGroupTree(EGroupQueryEnum.Manage);

        await SendAsync(method, HubMsgModel.Success(result));
    }

    public async Task ReConnect()
    {
        var method = nameof(ReConnect);

        await CheckSession(method);

        await Clients.Clients(Context.ConnectionId).SendAsync(method, HubMsgModel.Success(null));

        cache[Context.ConnectionId] = new HubSessionInfo
        {
            IsAdmin = _session.IsAdmin,
            RoleName = _session.RoleName,
        };

        await Groups.AddToGroupAsync(Context.ConnectionId, _session.RoleName);
    }

    #region 私有实现


    private async Task<bool> CheckSession(string method)
    {
        var userName = _session.UserName;
        if (userName.IsEmpty())
        {
            await SendAsync(method, HubMsgModel.Error("会话用户名获取失败"));
            return false;
        }

        var roleName = _session.RoleName;
        if (roleName.IsEmpty())
        {
            await SendAsync(method, HubMsgModel.Error("会话用户角色获取失败"));
            return false;
        }
        return true;
    }

    private async Task<IEnumerable<EGroupHubGroupTreeModel>> GetGroupTree(EGroupQueryEnum queryType)
    {
        var queryResult = new List<EGroupHubGroupQueryModel>();
        switch (queryType)
        {
            case EGroupQueryEnum.Manage when _session.IsAdmin:
                {
                    var query = from eg in _dbContext.EGroup.AsNoTracking()
                                join eglist in _dbContext.EGroupList.AsNoTracking() on eg.GroupId equals eglist.GroupId into eglistInfo
                                from eglist in eglistInfo.DefaultIfEmpty()
                                group eg by eg.GroupId into g
                                select new EGroupHubGroupQueryModel
                                {
                                    GroupId = g.Key,
                                    EquipCount = g.Count()
                                };
                    queryResult = await query.ToListAsync();
                    break;
                }
            default:
                {
                    var egList = _dbContext.EGroupList.AsNoTracking();
                    if (!_session.IsAdmin)
                    {
                        var browseEquips = _permissionCacheService.GetPermissionObj(_session.RoleName)?.BrowseEquips ?? new List<int>();
                        egList = egList.Where(x => browseEquips.Contains(x.EquipNo));
                    }
                    if (await egList.AnyAsync())
                    {
                        var query = from eqlist in egList
                                    group eqlist by eqlist.GroupId into g
                                    select new EGroupHubGroupQueryModel
                                    {
                                        GroupId = g.Key,
                                        EquipCount = g.Count()
                                    };

                        queryResult = await query.ToListAsync();
                    }
                    break;
                }
        }

        if (queryResult.IsEmpty())
        {
            return Enumerable.Empty<EGroupHubGroupTreeModel>();
        }

        var groups = await _dbContext.EGroup.AsNoTracking().ToDictionaryAsync(x => x.GroupId);
        var allTree = new Dictionary<int, EGroupHubGroupTreeModel>();
        foreach (var item in queryResult)
        {
            GetParents(item.GroupId, item.EquipCount, groups, ref allTree);
        }
        var result = allTree.OrderBy(x => x.Key).Select(x => x.Value).ToList();
        return result;
    }


    private async Task SendAsync(string method, object arg)
    {
        await Clients.Clients(Context.ConnectionId).SendAsync(method, arg);
    }

    private void GetParents(int groupId, int? eqCount, Dictionary<int, EGroup> sources, ref Dictionary<int, EGroupHubGroupTreeModel> groupTrees)
    {
        if (!sources.ContainsKey(groupId))
        {
            return;
        }

        if (groupTrees.ContainsKey(groupId) && eqCount.HasValue)
        {
            groupTrees[groupId].EquipCount = eqCount.Value;
        }

        var nowGroup = sources[groupId];
        var group = new EGroupHubGroupTreeModel
        {
            Id = nowGroup.GroupId,
            Name = nowGroup.GroupName,
            ParentId = nowGroup.ParentGroupId.Value,
            EquipCount = eqCount ?? 0
        };
        groupTrees[groupId] = group;
        GetParents(group.ParentId, null, sources, ref groupTrees);
    }

    #endregion
}