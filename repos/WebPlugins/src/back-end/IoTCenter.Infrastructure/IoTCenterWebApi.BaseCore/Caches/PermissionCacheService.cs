// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Data;
using IoTCenter.Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace IoTCenterWebApi.BaseCore;

public class PermissionCacheService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public PermissionCacheService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }
    private ConcurrentDictionary<string, PermissionObj> PermissionCache { get; set; } = new ConcurrentDictionary<string, PermissionObj>();

    public void ReSet()
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            
            var dbContext = scope.ServiceProvider.GetRequiredService<GWDbContext>();
            
            PermissionCache.Clear();
            
            var model = dbContext.Gwrole.AsNoTracking().ToList();
            if (!model.Any())
            {
                return;
            }

            var cacheValues = model.Select(d => new PermissionObj(d)).ToList();
            cacheValues.ForEach(d =>
            {
                PermissionCache.AddOrUpdate(d.Name, d, (e, v) => v);
            });
        }
        catch (Exception ex)
        {
            Serilog.Log.Error("应用缓存初始化失败", ex);
        }
    }


    public void ReSetByRoleNames(params string[] names)
    {
        using var scope = _scopeFactory.CreateScope();
        
        var dbContext = scope.ServiceProvider.GetRequiredService<GWDbContext>();
        
        var model = dbContext.Gwrole.AsNoTracking()
        .Where(d => names.Select(d => d).Contains(d.Name))
        .ToList();

        if (!model.Any())
        {
            return;
        }

        var cacheValues = model.Select(d => new PermissionObj(d)).ToList();
        cacheValues.ForEach(d =>
        {
            PermissionCache.AddOrUpdate(d.Name, d, (e, v) => d);
        });
    }

    public void RemoveByRoleNames(params string[] names)
    {
        using var scope = _scopeFactory.CreateScope();
        
        var dbContext = scope.ServiceProvider.GetRequiredService<GWDbContext>();
        
        var model = dbContext.Gwrole.AsNoTracking()
       .Where(d => names.Select(d => d).Contains(d.Name))
       .ToList();

        var noExist = names.Where(d => model.Select(e => e.Name).Contains(d)).ToList();
        noExist.ForEach(d =>
        {
            PermissionCache.TryRemove(d, out var v);
        });
    }

    public PermissionObj GetPermissionObj(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return default;
        }

        if (PermissionCache.TryGetValue(name, out var value))
        {
            return value;
        }

        ReSetByRoleNames(new string[] { name });

        PermissionCache.TryGetValue(name, out var value2);

        return value2;
    }

    public IEnumerable<PermissionObj> GetPermissionObjList()
    {
        if (!PermissionCache.Any())
        {
            return Array.Empty<PermissionObj>();
        }

        return PermissionCache.Values;
    }

    public void AddEquipPermission(string roleName, params int[] equipNos)
    {
        if (string.IsNullOrEmpty(roleName) || !equipNos.Any())
        {
            return;
        }

        if (!PermissionCache.TryGetValue(roleName, out var obj))
        {
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        
        var dbContext = scope.ServiceProvider.GetRequiredService<GWDbContext>();
        
        var entity = dbContext.Gwrole.FirstOrDefault(d => d.Name == roleName);
        if (entity == null)
        {
            return;
        }

        var controlEquips = obj.ControlEquips as List<int>;
        var browseEquips = obj.BrowseEquips as List<int>;

        foreach (var no in equipNos)
        { 
            controlEquips.Add(no);
            browseEquips.Add(no);
        }

        entity.ControlEquips = string.Join("#", obj.ControlEquips);
        entity.BrowseEquips = string.Join("#", obj.BrowseEquips);
        
        dbContext.SaveChanges();
    }

    public void RomveEquipPermission(params int[] equipNos)
    {
        if (!equipNos.Any())
        {
            return;
        }

        var objs = GetPermissionObjList()
        .Where(d => d.BrowseEquips.Any(d => equipNos.Contains(d)) ||
                    d.ControlEquips.Any(d => equipNos.Contains(d)) ||
                    d.ControlEquipsUnitOfDict.Any(d => equipNos.Contains(d.Key)))
        .ToList();

        if (!objs.Any())
        {
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GWDbContext>();

        objs.ForEach(d =>
        {
            d.BrowseEquips = d.BrowseEquips.Where(x => !equipNos.Contains(x));
            d.ControlEquips = d.ControlEquips.Where(x => !equipNos.Contains(x)).ToList();
            d.ControlEquipsUnit = d.ControlEquipsUnit.Where(c => !equipNos.Any(x => c.Contains(x.ToString())));
            d.ReverseCacheToDbEntity();

        });

        var models = objs.Select(d => d.ReverseModel);

        models.ToList().ForEach(d =>
        {
            var a = d.BrowseEquips;
            var b = d.ControlEquips;
        });

        dbContext.Gwrole.UpdateRange(models);
        dbContext.SaveChanges();
    }
}


public class PermissionObj
{
    private Gwrole _roleModel;

    public Gwrole ReverseModel { get; set; }
    public PermissionObj(Gwrole roleModel)
    {
        if (roleModel == null)
        {
            return;
        }

        _roleModel = roleModel;
        ReverseModel = roleModel;

        ResetRoleModel();
    }

    public string Name { get; set; } = string.Empty;

    public IEnumerable<int> ControlEquips { get; set; } = new List<int>();

    public IEnumerable<int> BrowseEquips { get; set; } = new List<int>();

    public IEnumerable<int> SystemModule { get; set; } = new List<int>();

    public IEnumerable<int> BrowsePages { get; set; } = new List<int>();


    public IEnumerable<string> ControlEquipsUnit { get; set; } = new List<string>();

    public Dictionary<int, IEnumerable<int>> ControlEquipsUnitOfDict { get; set; } = new Dictionary<int, IEnumerable<int>>();


    static IEnumerable<int> ConvertIntListResult(string source) => source?.Split("#", StringSplitOptions.RemoveEmptyEntries)
   .Select(d => int.TryParse(d, out var value) ? value : 0)
   .Where(d => d != 0)
   ?? new List<int>();


    private void ResetRoleModel()
    {
        Name = _roleModel.Name ?? string.Empty;

        ControlEquips = ConvertIntListResult(_roleModel.ControlEquips).ToList();

        BrowseEquips = ConvertIntListResult(_roleModel.BrowseEquips).ToList();

        SystemModule = ConvertIntListResult(_roleModel.SystemModule);

        BrowsePages = ConvertIntListResult(_roleModel.BrowsePages);

        ControlEquipsUnit = _roleModel.ControlEquipsUnit?.Split("#", StringSplitOptions.RemoveEmptyEntries) as IEnumerable<string> ?? new List<string>();

        ControlEquipsUnitOfDict = ControlEquipsUnit?
        .Select(d =>
        {
            var equipCatSetNo = d.Split(".", StringSplitOptions.RemoveEmptyEntries).Where(e => int.TryParse(e, out var v) && v > 0)
            .Select(d => Convert.ToInt32(d)).ToList();

            if (equipCatSetNo.Count < 1)
            {
                return (equipNo: 0, setNo: 0);
            }

            (int equipNo, int setNo) result = (equipCatSetNo[0], equipCatSetNo[1]);

            return result;
        })
        .Where(d => d.equipNo > 0 && d.setNo > 0)
        .GroupBy(d => d.equipNo)
        .ToDictionary(k => k.Key, v => v.Select(d => d.setNo)) ?? new Dictionary<int, IEnumerable<int>>();
    }

    public void ReverseCacheToDbEntity()
    {
        if (ReverseModel == null)
        {
            return;
        }

        ReverseModel.BrowseEquips = string.Join("#", BrowseEquips);
        
        ReverseModel.ControlEquips = string.Join("#", ControlEquips);
        
        ReverseModel.ControlEquipsUnit = string.Join("#", ControlEquipsUnit);

        _roleModel = ReverseModel;
    }
}