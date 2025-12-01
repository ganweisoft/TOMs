// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using AutoMapper;
using IoTCenter.Data.Model;
using IoTCenterCore.Environment.Shell.Scope;
using IoTCenterHost.Core.Abstraction.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Ganweisoft.IoTCenter.Module.EquipList;


public class EquipYc : Ycp
{
    private static IotCenterHostService _proxy = ShellScope.Services.GetRequiredService<IotCenterHostService>();
    public string ZiChanName
    { get; set; }

    public int? MergeAlarmCount { get; set; }
    private static readonly MapperConfiguration configuration = new MapperConfiguration(cfg =>
    {
        cfg.CreateMap<Ycp, EquipYc>()
        .ForMember(ec => ec.ZiChanName, yc => yc.MapFrom(src => string.Empty));

        cfg.CreateMap<Ycp, EquipYc>()
       .ForMember(ec => ec.MergeAlarmCount, yc => yc.MapFrom(src => MergeAlarmCountMappingFun(src)

       ));

    });
    private static readonly IMapper mapper = configuration.CreateMapper();

    public static EquipYc Copy(Ycp ycp)
    {
        return mapper.Map<EquipYc>(ycp);
    }

    private static int? MergeAlarmCountMappingFun(Ycp yc)
    {
        var alarmShield = yc.AlarmShield;
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

            count += _proxy.GetYCAlarmStateDictFromEquip(key).Count(d => item.Value.Contains(d.Key) && d.Value);

        }
        return count;


    }

}