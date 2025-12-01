// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using AutoMapper;
using IoTCenter.Data.Model;
using IoTCenterCore.Environment.Shell.Scope;
using IoTCenterHost.Core.Abstraction.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Ganweisoft.IoTCenter.Module.EquipList;


public class EquipYx : Yxp
{
    public string ZiChanName { get; set; }
    public int? MergeAlarmCount { get; set; }

    private static IotCenterHostService _proxy = ShellScope.Services.GetRequiredService<IotCenterHostService>();

    private static readonly MapperConfiguration configuration = new MapperConfiguration(cfg =>
    {
        cfg.CreateMap<Yxp, EquipYx>()
        .ForMember(ec => ec.ZiChanName, yx => yx.MapFrom(src => string.Empty));

        cfg.CreateMap<Yxp, EquipYx>()
      .ForMember(ec => ec.MergeAlarmCount, yx => yx.MapFrom(src => MergeAlarmCountMappingFun(src)));
    });

    public static EquipYx Copy(Yxp yxp)
    {
        var mapper = configuration.CreateMapper();
        return mapper.Map<EquipYx>(yxp);
    }


    private static int? MergeAlarmCountMappingFun(Yxp yx)
    {
        var alarmShield = yx.AlarmShield;
        if (string.IsNullOrEmpty(alarmShield))
        {
            return null;
        }
        var equipAndYcNos = alarmShield.Split("+", StringSplitOptions.RemoveEmptyEntries);

        var equipAndYcNoArray = equipAndYcNos.Select(d => d.Split(",", StringSplitOptions.RemoveEmptyEntries)
        .ToArray()).Where(x => x.Length > 1);

        if (!equipAndYcNoArray.Any())
        {
            return null;
        }

        var equipAndYcNoGroup = equipAndYcNoArray.GroupBy(d => d[0])
        .ToDictionary(k => k.Key,  // key
         v => v.Select(x => int.TryParse(x[1], out var val) ? val : 0).Where(r => r != 0).ToArray());

        var count = 0;
        foreach (var item in equipAndYcNoGroup)
        {
            if (!int.TryParse(item.Key, out var key))
            {
                continue;
            };

            count += _proxy.GetYXAlarmStateDictFromEquip(key).Count(d => item.Value.Contains(d.Key) && d.Value);

        }
        return count;


    }
}
