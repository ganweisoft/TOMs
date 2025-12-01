// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Ganweisoft.IoTCenter.Module.EquipList.Jobs;
using IoTCenter.Data;
using IoTCenter.Data.Model;
using IoTCenter.Utilities;
using IoTCenterCore.DynamicCache;
using IoTCenterCore.Environment.Shell.Scope;
using IoTCenterHost.Core.Abstraction.BaseModels;
using IoTCenterHost.Core.Abstraction.Services;
using IoTCenterWebApi.BaseCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.EquipList;

public class BAServicelmpl : IBAService
{
    private readonly Session _session;

    private readonly GWDbContext _context;
    private readonly IotCenterHostService _proxy;
    private readonly ILoggingService _apiLog;
    private readonly PermissionCacheService _permissionCacheService;

    private readonly string equipName = "虚拟计算设备";

    public BAServicelmpl(
        Session session,
        GWDbContext context,
        IotCenterHostService alarmCenterService,
        ILoggingService apiLog,
        PermissionCacheService permissionCacheService)
    {
        _session = session;
        _context = context;
        _proxy = alarmCenterService;
        _apiLog = apiLog;
        _permissionCacheService = permissionCacheService;
    }


    #region 分组

    public async Task<EquipListModelEx> GetOneGroupAsync(OneGroupRequest model)
    {
        var group = _context.EGroup.FirstOrDefaultAsync(x => x.GroupId == model.GroupId);
        if (group == null)
        {
            return default;
        }

        var query = _context.EGroupList.AsNoTracking().Where(x => x.GroupId == model.GroupId);

        if (!_session.IsAdmin)
        {
            var browseEquips = _permissionCacheService.GetPermissionObj(_session.RoleName)?.BrowseEquips ?? new List<int>();

            query = query.Where(x => browseEquips.Contains(x.EquipNo));
        }

        var equipQuery = _context.Equip.AsNoTracking()
            .SelectMany(e => _context.IoTDevice.Where(i => i.EquipNo == e.EquipNo)
                    .DefaultIfEmpty(),
                (e, i) => new
                {
                    e,
                    i.SystemName
                });


        if (!string.IsNullOrEmpty(model.SystemName))
        {
            equipQuery = equipQuery.Where(d => d.SystemName == model.SystemName);
        }

        var equipJoin = query.Join(equipQuery,
            el1 => el1.EquipNo,
            el2 => el2.e.EquipNo,
            (el1, el2) => new
            {
                el1.EGroupListId,
                el1.GroupId,
                el2.e.StaN,
                el1.EquipNo,
                el2.e.EquipNm,
                RelatedView = el2.e.RelatedPic,
                el2.SystemName,
            });

        var allCount = await equipJoin.CountAsync();

        query = query.Skip((model.PageNo - 1).Value * model.PageSize.Value).Take(model.PageSize.Value);

        var result = await equipJoin.ToArrayAsync();

        List<int> equipNos = result.Select(x => x.EquipNo).ToList();
        List<EquipListExInfo> resultList = new List<EquipListExInfo>();
        foreach (var item in result)
        {
            resultList.Add(new EquipListExInfo
            {
                Id = item.EGroupListId,
                StaNo = item.StaN,
                EquipNo = item.EquipNo,
                EquipName = item.EquipNm,
                SystemName = item.SystemName
            });
        }

        return new EquipListModelEx()
        {
            GroupId = group.Result.GroupId,
            GroupName = group.Result.GroupName,
            ParentGroupId = group.Result.ParentGroupId,
            EquipLists = resultList,
            AllCount = allCount
        };
    }

    public async Task<bool> MoveEquips(int oldGroupId, int newGroupId)
    {
        if (!(await _context.EGroup.AnyAsync(x => x.GroupId == oldGroupId)) ||
            !(await _context.EGroup.AnyAsync(x => x.GroupId == newGroupId)))
        {
            return false;
        }

        var equipList = await _context.EGroupList
            .Where(x => x.GroupId == oldGroupId).ToArrayAsync();

        foreach (var listItem in equipList)
        {
            listItem.GroupId = newGroupId;
        }

        _context.EGroupList.UpdateRange(equipList);

        var groupList = await _context.EGroup
            .Where(x => x.ParentGroupId == oldGroupId).ToArrayAsync();

        foreach (var item in groupList)
        {
            item.ParentGroupId = newGroupId;
        }

        _context.EGroup.UpdateRange(groupList);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> MoveEquipsToParentGroup(int oldGroupId)
    {
        var group = await _context.EGroup.FirstOrDefaultAsync(x => x.GroupId == oldGroupId);
        if (group == null || group.ParentGroupId == null)
        {
            return false;
        }

        return await MoveEquips(oldGroupId, group.ParentGroupId.Value);
    }

    public async Task<EquipYcYxList<EquipListModelEx>> GetGroupListAsync(int? pageNo, int? pageSize,
        string searchWorld, string systemName)
    {
        var equipGroups = await _context.EGroup.AsNoTracking()
            .GroupJoin(
                _context.EGroupList,
                g => g.GroupId,
                l => l.GroupId,
                (g, lg) => new { EGroup = g, EGroupLists = lg.DefaultIfEmpty() }
            )
            .SelectMany(
                x => x.EGroupLists,
                (g, l) => new EquipListGroup
                {
                    GroupId = g.EGroup.GroupId,
                    GroupName = g.EGroup.GroupName,
                    ParentGroupId = g.EGroup.ParentGroupId ?? 0,
                    EquipNo = l.EquipNo,
                    StaNo = l.StaNo,
                    EGroupListId = l.EGroupListId
                })
            .ToListAsync();

        var equips = await _context.Equip.AsNoTracking().Where(e => e.EquipNo > 0)
             .Select(e => new EquipSimpleExInfo()
             {
                 EquipNo = e.EquipNo,
                 EquipName = e.EquipNm,
                 StaNo = e.StaN,
                 RelatedPicture = e.RelatedPic,
                 ProcAdvice = e.ProcAdvice,
             }).ToListAsync();

        if (!_session.IsAdmin)
        {
            var browserEquips = _permissionCacheService.GetPermissionObj(_session.RoleName)?.BrowseEquips ?? new List<int>();

            equips = equips.Where(x => browserEquips.Contains(x.EquipNo)).ToList();
        }

        if (!string.IsNullOrEmpty(searchWorld))
        {
            equips = equips.Where(x => x.EquipName.Contains(searchWorld)).ToList();
        }

        var list = (from g in equipGroups
                    join e in equips on g.EquipNo equals e.EquipNo into gs
                    from ge in gs.DefaultIfEmpty()
                    select new
                    {
                        g.GroupId,
                        g.GroupName,
                        g.ParentGroupId,
                        g.EGroupListId,
                        StaNo = ge == null ? 0 : ge.StaNo,
                        EquipNo = ge == null ? 0 : ge.EquipNo,
                        EquipName = ge == null ? "" : ge.EquipName,
                        RelatedView = ge == null ? "" : ge.RelatedPicture,
                        SystemName = ge == null ? "" : ge.SystemName,
                        ProcAdvice = ge == null ? "" : ge.ProcAdvice,
                    }).OrderBy(d => d.EquipNo).ToList();

        var equipNos = list.Select(d => d.EquipNo).ToArray();

        var result = list.GroupBy(d => d.GroupId).Select((g, index) =>
        {
            var model = new EquipListModelEx
            {
                GroupId = g.Key,
                GroupName = g.FirstOrDefault()?.GroupName,
                ParentGroupId = g.FirstOrDefault()?.ParentGroupId,
            };

            var equipLists = g.Where(d => d.EquipNo > 0).Select(e => new EquipListExInfo()
            {
                Id = e.EGroupListId,
                StaNo = e.StaNo,
                EquipNo = e.EquipNo,
                EquipName = e.EquipName,
                SystemName = e.SystemName,
                ProcAdvice = e.ProcAdvice
            }).ToList();

            if (!string.IsNullOrEmpty(systemName))
            {
                equipLists = equipLists.Where(d => !string.IsNullOrEmpty(d.SystemName) && d.SystemName.Equals(systemName)).ToList();
            }

            model.EquipLists = equipLists;

            return model;
        }).ToList();

        _ = GetEquipListModelExEquipStates(result);

        var total = new List<int>();

        var resultTree = GenerateEquipListTree(result, 0, false, total);

        foreach (var childNode in resultTree)
        {
            childNode.Count = childNode.Children.Sum(d => d.Count) + childNode.EquipLists.Count;
        }

        if (pageSize != null && pageNo != null)
        {
            resultTree = resultTree.Skip((pageNo - 1).Value * pageSize.Value).Take(pageSize.Value).ToList();
        }

        return new EquipYcYxList<EquipListModelEx>
        {
            Count = result.Count,
            ListNos = total.ToArray(),
            List = resultTree
        };
    }
    private static List<EquipListModelEx> RemoveEmptyNode(List<EquipListModelEx> resultNode)
    {
        var emptyNode = resultNode.Where(item => !item.Children.Any()).ToList();
        var notEmptyNode = resultNode.Where(item => item.Children.Any()).ToList();
        foreach (var item in emptyNode)
        {
            if (!item.EquipLists.Any())
                resultNode.Remove(item);
        }

        foreach (var item in notEmptyNode)
        {
            if (item.Children.Any())
                RemoveEmptyNode(item.Children);

            if (!item.EquipLists.Any() && !item.Children.Any())
                resultNode.Remove(item);
        }

        return resultNode;
    }

    private static int[] GetEquipListModelExEquipNo(IEnumerable<EquipListModelEx> equipLists)
    {
        List<int> equips = new List<int>();
        foreach (var equipList in equipLists)
        {
            foreach (var item in equipList.EquipLists)
            {
                equips.Add(item.EquipNo);
            }

            if (equipList.Children != null && equipList.Children.Any())
                equips.AddRange(GetEquipListModelExEquipNo(equipList.Children));
        }

        return equips.ToArray();
    }


    private static IEnumerable<EquipListModelEx> GetEquipListModelExEquipStates(
        IEnumerable<EquipListModelEx> equipLists, Dictionary<int, GrpcEquipState> states)
    {
        foreach (var equipList in equipLists)
        {
            foreach (var item in equipList.EquipLists)
            {
                if (states.TryGetValue(item.EquipNo, out var equipState))
                {
                    item.EquipState = equipState;
                }
            }

            if (equipList.Children != null && equipList.Children.Any())
                _ = GetEquipListModelExEquipStates(equipList.Children, states);
        }

        return equipLists;
    }

    private static async Task<IEnumerable<EquipListModelEx>> GetEquipListModelExEquipStates(
        IEnumerable<EquipListModelEx> equipLists)
    {
        using var scope = ShellScope.Services.CreateScope();
        var dynamicCacheService = scope.ServiceProvider.GetRequiredService<IDynamicCacheService>();


        foreach (var equipList in equipLists)
        {
            foreach (var item in equipList.EquipLists)
            {

                var cacheContext = new CacheContext($"{item.EquipNo}".ToString());
                cacheContext.AddContext(CacheKey.EquipStateCacheKeySuffix);

                var equipState = await dynamicCacheService.GetCachedValueAsync(cacheContext);
                if (string.IsNullOrEmpty(equipState))
                    continue;

                item.EquipState = JsonConvert.DeserializeObject<EquipCacheModel>(equipState)?.EquipStateInt;
            }

            if (equipList.Children != null && equipList.Children.Any())
                _ = GetEquipListModelExEquipStates(equipList.Children);
        }

        return equipLists;
    }


    public async Task<EquipYcYxList<EquipListModelEx>> GetEquipTreeList(
        int? pageNo, int? pageSize, string searchWorld, string systemName)
    {
        var dbContext = ServiceLocator.Current.GetRequiredService<GWDbContext>();

        var connection = dbContext.Database.GetDbConnection();

        if (connection.State == ConnectionState.Closed)
        {
            connection.Open();
        }

        var equipGroups = await _context.EGroup.AsNoTracking()
            .GroupJoin(
                _context.EGroupList,
                g => g.GroupId,
                l => l.GroupId,
                (g, lg) => new { EGroup = g, EGroupLists = lg.DefaultIfEmpty() }
            )
            .SelectMany(
                x => x.EGroupLists,
                (g, l) => new EquipListGroup
                {
                    GroupId = g.EGroup.GroupId,
                    GroupName = g.EGroup.GroupName,
                    ParentGroupId = g.EGroup.ParentGroupId ?? 0,
                    EquipNo = l.EquipNo,
                    StaNo = l.StaNo,
                    EGroupListId = l.EGroupListId
                })
            .ToListAsync();

        var equips = await _context.Equip.AsNoTracking().Where(e => e.EquipNo > 0)
             .Select(e => new EquipSimpleExInfo()
             {
                 EquipNo = e.EquipNo,
                 EquipName = e.EquipNm,
                 StaNo = e.StaN,
                 RelatedPicture = e.RelatedPic,
                 ProcAdvice = e.ProcAdvice,
             }).ToListAsync();

        if (!_session.IsAdmin)
        {
            var browseEquips = _permissionCacheService.GetPermissionObj(_session.RoleName)?.BrowseEquips ?? new List<int>();

            equips = equips.Where(x => browseEquips.Contains(x.EquipNo)).ToList();
        }

        if (!string.IsNullOrEmpty(searchWorld))
        {
            equips = equips.Where(x => x.EquipName.Contains(searchWorld)).ToList();
        }

        if (!string.IsNullOrEmpty(systemName))
        {
            var subSystems = await _context.IoTDevice.AsNoTracking()
               .Select(d => new IotDeviceSubSystemInfo()
               {
                   EquipNo = d.EquipNo,
                   SystemName = d.SystemName
               }).ToListAsync();

            equips = (from c in equips
                      join p in subSystems on c.EquipNo equals p.EquipNo into pc
                      from p in pc.DefaultIfEmpty()
                      select new EquipSimpleExInfo
                      {
                          StaNo = c.StaNo,
                          SystemName = p == null ? string.Empty : p.SystemName,
                          EquipName = c.EquipName,
                          EquipNo = c.EquipNo,
                          RelatedPicture = c.RelatedPicture,
                          ProcAdvice = c.ProcAdvice
                      }).ToList();
        }

        var list = (from g in equipGroups
                    join e in equips on g.EquipNo equals e.EquipNo into gs
                    from ge in gs.DefaultIfEmpty()
                    select new
                    {
                        g.GroupId,
                        g.GroupName,
                        g.ParentGroupId,
                        g.EGroupListId,
                        StaNo = ge == null ? 0 : ge.StaNo,
                        EquipNo = ge == null ? 0 : ge.EquipNo,
                        EquipName = ge == null ? "" : ge.EquipName,
                        RelatedView = ge == null ? "" : ge.RelatedPicture,
                        SystemName = ge == null ? "" : ge.SystemName,
                        ProcAdvice = ge == null ? "" : ge.ProcAdvice
                    }).OrderBy(d => d.EquipNo).ToList();

        var result = list.GroupBy(d => d.GroupId).Select((g, index) =>
        {
            var model = new EquipListModelEx
            {
                GroupId = g.Key,
                GroupName = g.FirstOrDefault()?.GroupName,
                ParentGroupId = g.FirstOrDefault()?.ParentGroupId,
            };

            var equipLists = g.Where(d => d.EquipNo > 0)
                .Select(e => new EquipListExInfo()
                {
                    Id = e.EGroupListId,
                    StaNo = e.StaNo,
                    EquipNo = e.EquipNo,
                    EquipName = e.EquipName,
                    SystemName = e.SystemName,
                    ProcAdvice = e.ProcAdvice
                }).ToList();

            if (!string.IsNullOrEmpty(systemName))
            {
                equipLists = equipLists.Where(d => !string.IsNullOrEmpty(d.SystemName) && d.SystemName.Equals(systemName)).ToList();
            }

            model.EquipLists = equipLists;
            return model;
        }).ToList();

        var total = new List<int>();

        var resultTree = GenerateEquipListTree(result, 0, false, total);

        foreach (var childNode in resultTree)
        {
            childNode.Count = childNode.Children.Sum(d => d.Count) + childNode.EquipLists.Count;
        }

        if (pageSize != null && pageNo != null)
        {
            resultTree = resultTree.Skip((pageNo - 1).Value * pageSize.Value).Take(pageSize.Value).ToList();
        }

        return new EquipYcYxList<EquipListModelEx>
        {
            Count = result.Count,
            ListNos = total.ToArray(),
            List = resultTree
        };
    }

    private static List<EquipListModelEx> GenerateEquipListTree(List<EquipListModelEx> input, int nodeId,
        bool isFilterEmptyNode = true, List<int> totalEquipNos = null)
    {
        var node = input.Where(x => x.ParentGroupId == nodeId).ToList();
        var notNode = input.Where(x => x.ParentGroupId != nodeId).ToList();
        foreach (var item in node)
        {
            if (totalEquipNos != null)
            {
                totalEquipNos.AddRange(item.EquipLists.Select(d => d.EquipNo));
            }

            var children = GenerateEquipListTree(notNode, item.GroupId, isFilterEmptyNode, totalEquipNos);
            if (isFilterEmptyNode)
            {
                children = children.Where(d => d.EquipLists.Any() || d.Children.Any()).ToList();
            }

            item.Children = children;

            ComputedGroupCount(item);
        }

        return node;
    }

    private static void ComputedGroupCount(EquipListModelEx node)
    {
        foreach (var children in node.Children)
        {
            children.Count = children.EquipLists.Count;

            ComputedGroupCount(children);

            node.Count += children.Count;
        }
    }


    public async Task<(bool IsSuccess, int GroupId, string Message)> AddGroupAsync(string name, int parentId,
        IEnumerable<int> EquipNos)
    {
        if (await _context.EGroupList.AnyAsync(x => EquipNos.Contains(x.EquipNo)))
        {
            return (false, 0, "要求插入的设备已经存在其他分组中");
        }

        if (await _context.EGroup.AnyAsync(x => x.GroupName == name))
        {
            return (false, default, "分组已经存在");
        }

        if (parentId != 0)
        {
            var condition = await _context.EGroup.AnyAsync(x => x.GroupId == parentId && parentId != 0);
            if (!condition)
            {
                return (false, default, "父分组不存在");
            }
        }

        var result = new EGroup
        {
            GroupName = name,
            ParentGroupId = parentId
        };

        await _context.EGroup.AddAsync(result);
        if (await _context.SaveChangesAsync() <= 0)
        {
            return (false, default, "添加失败");
        }

        if (EquipNos != null && EquipNos.Any())
        {
            var equips = EquipNos.ToArray();
            if (equips.Length != await _context.Equip.Where(x => equips.Contains(x.EquipNo)).CountAsync())
            {
                return (false, default, "创建分组成功，但要添加的设备不存在");
            }
        }

        await InsertEquip(result.GroupId, EquipNos);

        _apiLog.Info($"用户\"[{_session.UserName}({_session.IpAddress})]\"-添加设备分组-分组名:{name}-成功");
        return (true, result.GroupId, "添加成功");
    }


    public async Task<bool> DeleteGroupAsync(int groupId, bool isDeleteEquip = false)
    {
        if (groupId == 0)
        {
            return false;
        }

        var equipGroup = await _context.EGroup.Where(x => x.GroupId == groupId || x.ParentGroupId == groupId)
            .ToArrayAsync();

        if (!equipGroup.Any())
        {
            return false;
        }

        var equips = await _context.EGroupList.Where(x => equipGroup.Select(x => x.GroupId).Contains(x.GroupId))
            .ToArrayAsync();

        var equipNos = equips.Select(d => d.EquipNo).ToList();
        if (isDeleteEquip && equipNos.Any())
        {
            var equipList = await _context.Equip.Where(d => equipNos.Contains(d.EquipNo)).ToListAsync();
            
            var ycps = await _context.Ycp.Where(d => equipNos.Contains(d.EquipNo)).ToListAsync();
            
            var yxps = await _context.Yxp.Where(d => equipNos.Contains(d.EquipNo)).ToListAsync();
            
            var sets = await _context.SetParm.Where(d => equipNos.Contains(d.EquipNo)).ToListAsync();
           
            _context.Equip.RemoveRange(equipList);
            _context.Ycp.RemoveRange(ycps);
            _context.Yxp.RemoveRange(yxps);
            _context.SetParm.RemoveRange(sets);
        }

        string name = equipGroup.FirstOrDefault(x => x.GroupId == groupId)?.GroupName;
        _context.EGroup.RemoveRange(equipGroup);

        if (equips.Any())
            _context.EGroupList.RemoveRange(equips);

        await _context.SaveChangesAsync();

        _apiLog.Info($"用户\"[{_session.UserName}({_session.IpAddress})]\"-删除设备分组-分组名:{name}-成功");
        return true;
    }

    public async Task<bool> ReNameGroupAsync(int groupId, string newName)
    {
        var equipGroup = await _context.EGroup.FirstOrDefaultAsync(x => x.GroupId == groupId);
        if (await _context.EGroup.AnyAsync(x => x.GroupName == newName))
        {
            return false;
        }

        if (equipGroup == null)
        {
            return false;
        }

        var oldName = equipGroup.GroupName;

        equipGroup.GroupName = newName;
        if (await _context.SaveChangesAsync() >= 0)
        {
            _apiLog.Info(
                $"用户\"[{_session.UserName}({_session.IpAddress})]\"-重命名设备分组-旧分组名:{oldName},新分组名{newName}-成功");
            return true;
        }

        return false;
    }

    #endregion


    #region 分组设备管理

    public async Task<(bool IsSuccess, string Message)> GroupInsertEquipAsync(int groupId,
        IEnumerable<int> equipNos)
    {
        if (!await _context.EGroup.AnyAsync(x => x.GroupId == groupId))
        {
            return (false, "分组号不存在");
        }

        if (await _context.EGroupList.AnyAsync(x => equipNos.Contains(x.EquipNo)))
        {
            return (false, "要求插入的设备已经存在其他分组中");
        }

        return (await InsertEquip(groupId, equipNos) ? (true, "插入成功") : (false, "插入失败"));
    }


    public async Task<bool> GroupDeleteEquipAsync(int groupId, int equipNo)
    {
        var result = await _context.EGroupList
            .FirstOrDefaultAsync(x => x.GroupId == groupId && x.EquipNo == equipNo);

        if (result == null)
        {
            return false;
        }

        _context.EGroupList.Remove(result);

        await _context.SaveChangesAsync();

        _apiLog.Info(
            $"用户\"[{_session.UserName} ( {_session.IpAddress})]\"-分组中移除设备-组号:{groupId}，移除设备号:{equipNo}-成功");

        return true;
    }

    #endregion


    #region 设备信息

    public async Task<EquipYcYxList<EquipYc>> EquipYcListAsync(int staN, int equipNo, int pageNo, int pageSize,
        string word = null)
    {
        IQueryable<Ycp> query = _context.Ycp.Where(x => x.EquipNo == equipNo);

        if (!string.IsNullOrWhiteSpace(word))
        {
            query = query.Where(x => x.YcNm.Contains(word));
        }

        var count = await query.CountAsync();

        var result = await query
            .Skip((pageNo - 1) * pageSize).Take(pageSize)
            .ToListAsync();

        var queryZc = result.Select(left => EquipYc.Copy(left)).ToList();

        List<int> listNo = result.Select(x => x.YcNo).ToList();

        return new EquipYcYxList<EquipYc>
        {
            Count = count,
            ListNos = listNo.ToArray(),
            List = queryZc
        };
    }

    public async Task<EquipYcYxList<EquipYx>> EquipYxListAsync(int staN, int equipNo, int pageNo, int pageSize,
        string word = null)
    {
        var query = _context.Yxp.AsNoTracking().Where(x => x.StaN == staN && x.EquipNo == equipNo);

        if (!string.IsNullOrWhiteSpace(word))
        {
            query = query.Where(x => x.YxNm.Contains(word));
        }

        var count = await query.CountAsync();

        var result = await query.Skip((pageNo - 1) * pageSize).Take(pageSize)
            .ToListAsync();

        var queryZc = result.Select(left => EquipYx.Copy(left)).ToList();

        var listNo = result.Select(x => x.YxNo).ToList();

        return new EquipYcYxList<EquipYx>
        {
            Count = count,
            ListNos = listNo.ToArray(),
            List = queryZc
        };
    }

    public async Task<EquipYcYxList<SetParm>> EquipSetparmListAsync(int staN, int equipNo, int pageNo, int pageSize,
        string word = null)
    {
        var roleInfo = _permissionCacheService.GetPermissionObj(_session.RoleName);

        if (!_session.IsAdmin && roleInfo == null)
        {
            return null;
        }

        if (!_session.IsAdmin && !roleInfo.BrowseEquips.Contains(equipNo))
        {
            return null;
        }

        var query = _context.SetParm.AsNoTracking().Where(x => x.StaN == staN && x.EquipNo == equipNo);

        if (!string.IsNullOrWhiteSpace(word))
        {
            query = query.Where(d => d.SetNm.Contains(word));
        }

        if (!_session.IsAdmin)
        {
            var controlEquipsUnitDict = roleInfo.ControlEquipsUnitOfDict;
            var isExist = controlEquipsUnitDict.TryGetValue(equipNo, out var setUnits);
            if (!isExist && !roleInfo.ControlEquips.Contains(equipNo))
            {
                return null;
            }

            if (isExist)
            {
                query = query.Where(x => setUnits.Contains(x.SetNo));
            }
        }

        var list = await query.ToListAsync();
        var count = list.Count;
        var result = list.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();

        return new EquipYcYxList<SetParm>
        {
            Count = count,
            ListNos = result.Select(x => x.SetNo).ToArray(),
            List = result
        };
    }

    #endregion


    #region 实时数据

    public List<EquipValueResponse<GrpcEquipState>> GetEquipStatus(IEnumerable<int> nos)
    {
        if (nos == null)
        {
            throw new ArgumentNullException(nameof(nos));
        }

        var equipItem = _proxy.GetEquipStateDict();

        var list = new List<EquipValueResponse<GrpcEquipState>>();

        foreach (var item in nos)
        {
            bool tmp = equipItem.TryGetValue(item, out var state);
            list.Add(new EquipValueResponse<GrpcEquipState>
            {
                No = item,
                Status = tmp ? state : GrpcEquipState.NoCommunication
            });
        }

        return list;
    }

    public EquipValueResponse<IEnumerable<EquipValueResponse<object>>> GetYcStatus(int equipNo,
        IEnumerable<int> ycps)
    {
        if (ycps == null)
        {
            throw new ArgumentNullException(nameof(ycps));
        }

        var list = new List<EquipValueResponse<object>>();

        foreach (var ycNo in ycps)
        {
            var state = _proxy.GetYCAlarmState(equipNo, ycNo);

            list.Add(new EquipValueResponse<object>
            {
                No = ycNo,
                Status = state
            });
        }

        return new EquipValueResponse<IEnumerable<EquipValueResponse<object>>>()
        {
            No = equipNo,
            Status = list
        };
    }

    public EquipValueResponse<IEnumerable<EquipValueResponse<object>>> GetYxStatus(int equipNo,
        IEnumerable<int> yxps)
    {
        if (yxps == null)
        {
            throw new ArgumentNullException(nameof(yxps));
        }

        var result = _proxy.GetYXValueDictFromEquip(equipNo);
        if (result == null)
        {
            return default;
        }

        var list = new List<EquipValueResponse<object>>();

        foreach (var item in yxps)
        {
            bool tmp = result.TryGetValue(item, out var state);
            list.Add(new EquipValueResponse<object>
            {
                No = item,
                Status = tmp ? state : null
            });
        }

        return new EquipValueResponse<IEnumerable<EquipValueResponse<object>>>()
        {
            No = equipNo,
            Status = list
        };
    }

    #endregion

    #region 曲线

    public async Task<IEnumerable<CurDataResponse>> GetCurAsyncAsync(CurDataRequest model)
    {
        if (model == null)
        {
            return default;
        }

        try
        {
            List<GrpcMyCurveData> result;
            result = await _proxy.GetChangedDataFromCurveAsync(model.Bgn, model.End, model.Stano, model.Eqpno,
                model.Ycyxno, model.Type);
            var re = new List<CurDataResponse>();
            foreach (var item in result)
            {
                re.Add(new CurDataResponse
                {
                    Datetime = item.datetime,
                    State = (int)item.state,
                    Value = item.value
                });
            }

            return re;
        }
        catch (Exception ex)
        {
            _apiLog.Error("获取曲线失败，异常信息：" + ex);
            return Array.Empty<CurDataResponse>();
        }
    }

    public Task<(int Count, IEnumerable<EquipDetailResponse> Result)> GetEquipDetail(GroupQueryModel model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        return GetEquipDetailAsync(model);
    }

    private async Task<(int Count, IEnumerable<EquipDetailResponse> Result)> GetEquipDetailAsync(
        GroupQueryModel model)
    {
        var query = _context.Equip.AsQueryable();
        if (!_session.IsAdmin)
        {
            var browerEquips = _permissionCacheService.GetPermissionObj(_session.RoleName)?.BrowseEquips ?? new List<int>();

            query = query.Where(x => browerEquips.Contains(x.EquipNo));
        }

        if (!string.IsNullOrEmpty(model.SearchName))
        {
            query = query.Where(x => x.EquipNm.Contains(model.SearchName));
        }

        if (model.IsFittle)
        {
            var hasGroupEquip = _context.EGroupList.Select(x => x.EquipNo);
            query = query.Where(x => !_context.EGroupList.Select(x => x.EquipNo).Contains(x.EquipNo));
        }

        var tmp = query.Select(x => new EquipDetailResponse
        {
            StaN = x.StaN,
            EquipNo = x.EquipNo,
            EquipNm = x.EquipNm,
            EquipDetail = x.EquipDetail
        });
        if (!(model.PageNo == 0 && model.PageSize == 0))
        {
            tmp = tmp.Skip((model.PageNo - 1).Value * model.PageSize.Value).Take(model.PageSize.Value);
        }

        var result = await tmp.ToListAsync();
        var count = result.Count;

        return (count, result);
    }

    #endregion

    #region 检查

    private async Task<bool> InsertEquip(int groupId, IEnumerable<int> equipNos)
    {
        if (equipNos == null || !equipNos.Any())
        {
            return false;
        }

        var equipNosOfDb = await _context.EGroupList
            .Where(d => d.GroupId == groupId)
            .Select(d => d.EquipNo)
            .ToListAsync();

        var list = equipNos.Where(d => equipNosOfDb.All(x => x != d))
            .Select(x => new EGroupList
            {
                GroupId = groupId,
                StaNo = 1,
                EquipNo = x
            }).ToList();

        await _context.EGroupList.AddRangeAsync(list);
        await _context.SaveChangesAsync();
        foreach (var item in list)
        {
            _apiLog.Info(
                $"用户\"[{_session.UserName}({_session.IpAddress})]\"-分组中移入设备-组号:{groupId},新增设备号:{item.EquipNo}-成功");
        }

        return true;
    }

    #endregion

    #region 设备分组重构

    public async Task<IEnumerable<GroupEquipModel>> GetGroupEquipAsync(int? pageNo, int? pageSize, string searchWorld, int groupId = 0)
    {
        if (!await _context.EGroup.AnyAsync(eg => eg.ParentGroupId == groupId || eg.GroupId == groupId))
        {
            return Enumerable.Empty<GroupEquipModel>();
        }

        var equipQuery = _context.Equip.AsNoTracking();

        if (!_session.IsAdmin && await _context.Equip.AnyAsync())
        {
            var browseEquips = _permissionCacheService.GetPermissionObj(_session.RoleName)?.BrowseEquips ?? new List<int>();

            equipQuery = equipQuery.Where(x => browseEquips.Contains(x.EquipNo));
        }

        if (!string.IsNullOrEmpty(searchWorld))
        {
            equipQuery = equipQuery.Where(x => x.EquipNm.Contains(searchWorld));
        }

        var query = from g in _context.EGroup.Where(eg => eg.ParentGroupId == groupId || eg.GroupId == groupId).AsNoTracking()
                    from egl in _context.EGroupList.AsNoTracking().Where(d => d.GroupId == g.GroupId).DefaultIfEmpty()
                    from e in equipQuery.Where(d => d.EquipNo == egl.EquipNo)
                    select new GroupEquipModel
                    {
                        IsGroup = false,
                        GroupId = g.GroupId,
                        StaNo = e != null ? e.StaN : 0,
                        Key = e != null ? e.EquipNo : 0,
                        Title = e != null ? e.EquipNm : string.Empty,
                        RelatedView = e != null ? e.RelatedPic : string.Empty,
                        ProcAdvice = e != null ? e.ProcAdvice : string.Empty,
                        Children = new List<GroupEquipModel>()
                    };

        var list = await query.OrderBy(d => d.Key).ToListAsync();

        _ = PaddingGroupEquipStates(list);

        var group = _context.EGroup.AsNoTracking()
            .Where(eg => eg.ParentGroupId == groupId)
            .Select(g => new GroupEquipModel
            {
                IsGroup = true,
                Key = g.GroupId,
                Title = g.GroupName,
                Status = 1
            });

        var groupQuery = await group.ToListAsync();
        foreach (var pg in groupQuery)
        {
            pg.Status = list.Any(e => e.Status == 2) ? 2 : list.Any(e => e.Status == 6) ? 6 : 1;
            pg.Children = list.Where(e => e.GroupId == pg.Key).ToList();

            var childGroup = _context.EGroup.AsNoTracking()
                .Where(eg => eg.ParentGroupId == pg.Key)
                .Select(g => new GroupEquipModel
                {
                    IsGroup = true,
                    Key = g.GroupId,
                    Title = g.GroupName,
                    Status = 1
                }).ToList();

            childGroup.ForEach(cg =>
            {
                cg.Children = new List<GroupEquipModel>();
            });

            pg.Children.AddRange(childGroup);
        }

        groupQuery.AddRange(list.Where(e => e.GroupId == groupId));

        return groupQuery;
    }

    private static async Task<IEnumerable<GroupEquipModel>> PaddingGroupEquipStates(
        IEnumerable<GroupEquipModel> models)
    {
        using var scope = ShellScope.Services.CreateScope();
        var dynamicCacheService = scope.ServiceProvider.GetRequiredService<IDynamicCacheService>();

        var paddingGroupEquipStates = models.ToList();
        foreach (var model in paddingGroupEquipStates)
        {
            var cacheContext = new CacheContext($"{model.Key}".ToString());
            cacheContext.AddContext(CacheKey.EquipStateCacheKeySuffix);

            var equipState = await dynamicCacheService.GetCachedValueAsync(cacheContext);
            if (string.IsNullOrEmpty(equipState))
                continue;

            model.Status = JsonConvert.DeserializeObject<EquipCacheModel>(equipState)?.EquipStateInt;
        }

        return paddingGroupEquipStates;
    }


    public async Task<int> GroupListCount()
    {
        var query = _context.Equip.AsNoTracking().Where(e => e.EquipNm != equipName);

        if (!_session.IsAdmin)
        {
            var roleInfo = _permissionCacheService.GetPermissionObj(_session.RoleName);

            if (roleInfo == null)
            {
                return 0;
            }

            query = query.Where(x => roleInfo.BrowseEquips.Contains(x.EquipNo));
        }

        return await query.CountAsync();

    }

    public async Task<IEnumerable<GroupEquipModel>> GetGroupEquipAsync(string searchWorld)
    {
        if (!await _context.EGroup.AnyAsync())
        {
            return Enumerable.Empty<GroupEquipModel>();
        }

        var roleName = _session.RoleName;
        var equipQuery = _context.Equip.AsNoTracking();

        if (!_session.IsAdmin)
        {
            var browseEquips = _permissionCacheService.GetPermissionObj(roleName)?.BrowseEquips ?? new List<int>();

            equipQuery = equipQuery.Where(x => browseEquips.Contains(x.EquipNo));
        }

        if (!string.IsNullOrEmpty(searchWorld))
        {
            equipQuery = equipQuery.Where(x => x.EquipNm.Contains(searchWorld));
        }

        var query = from g in _context.EGroup.AsNoTracking()
                    select new GroupEquipModel
                    {
                        GroupId = g.ParentGroupId.Value,
                        Title = g.GroupName,
                        Key = g.GroupId,
                        IsGroup = true
                    };

        var result = await query.ToListAsync();

        var resultTree = GenerateEquipListTree(result, 0, true);

        return resultTree;
    }

    private static List<GroupEquipModel> GenerateEquipListTree(List<GroupEquipModel> input, int nodeId,
        bool isFilterEmptyNode = true)
    {
        var node = input.Where(x => x.GroupId == nodeId).ToList();
        var notNode = input.Where(x => x.GroupId != nodeId).ToList();
        foreach (var item in node)
        {
            var children = GenerateEquipListTree(notNode, item.Key, isFilterEmptyNode);

            item.Children = children;
        }

        return node;
    }

    #endregion
}