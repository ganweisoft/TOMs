// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Ganweisoft.IoTCenter.Module.EquipList;

public class EGroupStaticStruct
{
    private static EGroupCollection<OneEGroup> _GroupCollection;

    public static bool NeedUpdate { get; set; }
    private readonly GWDbContext _context;

    public EGroupStaticStruct(GWDbContext context)
    {
        _context = context;

        if (_GroupCollection == null)
        {
            _GroupCollection = new EGroupCollection<OneEGroup>();
        }

        if (!_GroupCollection.Any())
        {
            IEnumerable<OneEGroup> groups = SynchronizationAllEquip(_context).Result;
            _GroupCollection.CopyTo(groups);

            CompoundStruct(_GroupCollection);
        }
    }

    #region  外部调用

    #region 对外接口

    public static (int TotalCount, OneEGroup[] Struct) GetRootGroup(int pageNo, int pageSize)
    {
        if (!_GroupCollection.Any())
            return (0, Array.Empty<OneEGroup>());

        int min = _GroupCollection.Min(x => x.ParentGroupId);
        var dic = _GroupCollection.Where(x => x.ParentGroupId == min);
        if (!dic.Any())
            return (0, Array.Empty<OneEGroup>());
        return (dic.Count(), dic.Skip((pageNo - 1) * pageSize).Take(pageSize).ToArray());
    }

    public static OneEGroup GetOneGroup(int groupId)
    {
        if (_GroupCollection.Get(groupId, out var eGroup))
        {
            return eGroup;
        }
        return default;
    }


    #endregion

    #region 事件总线通知刷新缓存

    #endregion

    #region  内部函数

    /*
     * 客户端通过依赖注入使用事件总线，通知事件，根据事件以及参数，更新缓存
     */

    public static async Task AddGroupAsync(GWDbContext dbContext, int groupId)
    {
        NeedUpdate = true;

        var resultQuery = EquipFromDatabase(dbContext, groupId);

        var group = await dbContext.EGroup.FirstOrDefaultAsync(x => x.GroupId == groupId);

        if (group == null)
            return;

        OneEGroup eGroup = new OneEGroup(group.GroupId, group.GroupName, group.ParentGroupId.GetValueOrDefault(), await resultQuery);
        _GroupCollection.Add(eGroup);

        if (_GroupCollection.Get(eGroup.ParentGroupId, out var parent))
        {
            parent.Children.Add(eGroup);
            eGroup.SetParent(parent);
        }
    }

    public static void DeleteAllGroupAsync(int groupId)
    {
        NeedUpdate = true;

        _ = _GroupCollection.Get(groupId, out var current);
        current.Equips.Clear();
        _GroupCollection.Remove(current);
        var parent = current.Parent;
        if (parent != null)
            parent.Children.Remove(current);

        current.Children.AsParallel().ForAll(child =>
        {
            child.RemoveParent();
            _GroupCollection.Remove(child);
        });

    }

    public static void DeleteGroupAsync(int groupId)
    {
        NeedUpdate = true;
        _ = _GroupCollection.Get(groupId, out var current);
        _GroupCollection.Remove(current);
        var parent = current.Parent;
        if (parent != null)
            parent.Children.Remove(current);

        current.Equips.AsParallel().ForAll(item =>
        {
            parent.Equips.Add(item);
        });

        current.Children.AsParallel().ForAll(child =>
        {
            child.SetParent(parent);
        });
    }

    public static async Task RenameGroupAsync(GWDbContext dbContext, int groupId)
    {
        NeedUpdate = true;
        var group = await dbContext.EGroup.AsTracking().FirstOrDefaultAsync(x => x.GroupId == groupId);
        _ = _GroupCollection.Get(groupId, out var current);
        current.ReName(group.GroupName);
    }

    public static async Task UpdateGroupEquipAsync(GWDbContext dbContext, int groupId)
    {
        NeedUpdate = true;
        IEnumerable<OneEGroupEquip> result = await EquipFromDatabase(dbContext, groupId);
        _ = _GroupCollection.Get(groupId, out var current);
        current.Equips.Clear();
        current.Equips.CopyTo(result);
    }

    #endregion

    #region 初始化或刷新使用


    private static async Task<IEnumerable<OneEGroup>> SynchronizationAllEquip(GWDbContext context)
    {
        var equipAndDevice = GetEquipAndDevice(context);

        var groupListAndEquip = GetGroupListAndEquip(context, equipAndDevice);

        var list = await GroupsFromDatabase(context, groupListAndEquip);

        return list.Select(item =>
        new OneEGroup(item.GroupId, item.GroupName, item.ParentGroupId.GetValueOrDefault(), item.Equips
        .Select(x => new OneEGroupEquip
        {
            GroupId = x.GroupId,
            EGroupListId = x.EGroupListId,
            EquipName = x.EquipNm,
            EquipNo = x.EquipNo,
            RelatedView = x.RelatedPic,
            StaNo = x.StaN,
            SystemName = x.SystemName
        })));
    }

    private static void CompoundStruct(EGroupCollection<OneEGroup> groups)
    {

        groups.AsParallel()
            .ForAll(item =>
            {
                if (groups.Get(item.ParentGroupId, out OneEGroup parent))
                {
                    parent.Children.Add(item);
                    item.SetParent(parent);
                }
            });
    }


    #endregion


    #region 获取查询表达式

    private static IQueryable<EquipAndIoTDeviceTmp> GetEquipAndDevice(GWDbContext context)
    {
        var equipAndDevice = from e in context.Equip.AsNoTracking()
                             join iot in context.IoTDevice.AsNoTracking()
                             on e.EquipNo equals iot.EquipNo
                             into re
                             from r in re.DefaultIfEmpty()
                             select new EquipAndIoTDeviceTmp
                             {
                                 StaN = e.StaN,
                                 EquipNo = e.EquipNo,
                                 EquipNm = e.EquipNm,
                                 RelatedPic = e.RelatedPic,
                                 SystemName = r.SystemName
                             };
        return equipAndDevice;
    }

    private static IQueryable<EquipAndIoTDeviceTmp> GetGroupListAndEquip(GWDbContext context, IQueryable<EquipAndIoTDeviceTmp> equipAndDevice)
    {
        var groupListAndEquip = context.EGroupList.AsNoTracking()
            .Join(equipAndDevice, el => el.EquipNo, e => e.EquipNo, (el, e) => new EquipAndIoTDeviceTmp
            {
                GroupId = el.GroupId,
                EGroupListId = el.EGroupListId,
                StaN = e.StaN,
                EquipNo = e.EquipNo,
                EquipNm = e.EquipNm,
                RelatedPic = e.RelatedPic,
                SystemName = e.SystemName,
            });
        return groupListAndEquip;
    }

    private static async Task<IEnumerable<OneEGroupTmp>> GroupsFromDatabase(GWDbContext context, IQueryable<EquipAndIoTDeviceTmp> groupListAndEquip)
    {
        var equips = await groupListAndEquip.ToArrayAsync();
        var groups = await context.EGroup.ToArrayAsync();

        var list = groups
            .GroupJoin(equips.DefaultIfEmpty(),
            el => el.GroupId,
            e => e.GroupId,
            (el, e) => new OneEGroupTmp
            {
                GroupId = el.GroupId,
                GroupName = el.GroupName,
                ParentGroupId = el.ParentGroupId,
                Equips = e
            });

        return list;
    }


    #endregion


    #region 分组数据结构化

    private static async Task<IEnumerable<OneEGroupEquip>> EquipFromDatabase(GWDbContext context, int groupId)
    {
        var equipAndDevice = GetEquipAndDevice(context);

        var groupListAndEquip = GetGroupListAndEquip(context, equipAndDevice);

        var list = await groupListAndEquip.Where(x => x.GroupId == groupId).ToArrayAsync();
        return list.Select(x => new OneEGroupEquip
        {
            EGroupListId = x.EGroupListId,
            EquipName = x.EquipNm,
            EquipNo = x.EquipNo,
            GroupId = x.GroupId,
            RelatedView = x.RelatedPic,
            StaNo = x.StaN,
            SystemName = x.SystemName
        });
    }

    #endregion

    #region  EFCore 查询中间结构

    private struct EquipAndIoTDeviceTmp
    {
        public int GroupId { get; set; }
        public int EGroupListId { get; set; }
        public int StaN { get; set; }
        public int EquipNo { get; set; }
        public string EquipNm { get; set; }
        public string RelatedPic { get; set; }
        public string SystemName { get; set; }
    }

    private struct OneEGroupTmp
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int? ParentGroupId { get; set; }
        public IEnumerable<EquipAndIoTDeviceTmp> Equips { get; set; }
    }

    #endregion

    #endregion

}