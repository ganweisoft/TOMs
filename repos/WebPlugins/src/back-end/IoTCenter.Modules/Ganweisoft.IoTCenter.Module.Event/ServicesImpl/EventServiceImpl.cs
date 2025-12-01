// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Data;
using IoTCenter.Data.Model;
using IoTCenter.Utilities;
using IoTCenterHost.Core.Abstraction.AppServices;
using IoTCenterWebApi.BaseCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.Event;

public class EventServiceImpl : IEventService
{
    private readonly Session _session;

    private readonly IAlarmEventClientAppService _alarmEventClientAppService;

    private readonly GWDbContext _context;
    private readonly PermissionCacheService _permissionCacheService;

    private readonly ILoggingService _apiLog;

    public EventServiceImpl(
        Session session,
        GWDbContext context,
        IAlarmEventClientAppService alarmEventClientAppService,
        PermissionCacheService permissionCacheService,
        ILoggingService apiLog)
    {
        _session = session;
        _context = context;
        _apiLog = apiLog;
        _permissionCacheService = permissionCacheService;
        _alarmEventClientAppService = alarmEventClientAppService;
    }

    public async Task<OperateResult> RecordLoginEvent()
    {
        var csEvent = $"用户-{_session.UserName}(({_session.IpAddress},{_session.Port})-登录平台-成功!";

        var confirmTime = DateTime.Parse("2000-01-01 00:00:00");

        await _context.SysEvt.AddAsync(new SysEvt()
        {
            StaN = 1,
            Event = csEvent,
            Time = DateTime.Now,
            Confirmtime = confirmTime,
            Guid = Guid.NewGuid().ToString("N")
        });

        await _context.SaveChangesAsync();

        return OperateResult.Success;
    }

    public OperateResult<PagedResult<EquipEventResponse>> GetEquipEvtByPage(EquipEvtModel equipEvtModel)
    {
        if (equipEvtModel == null)
        {
            return OperateResult.Failed<PagedResult<EquipEventResponse>>("请求参数为空");
        }

        DateTime beginTime;

        if (string.IsNullOrEmpty(equipEvtModel.BeginTime))
        {
            beginTime = SqlDateTime.MinValue.Value;
        }
        else if (!DateTime.TryParse(equipEvtModel.BeginTime, out var parseBeginTime))
        {
            return OperateResult.Failed<PagedResult<EquipEventResponse>>("开始时间格式错误");
        }
        else
        {
            beginTime = parseBeginTime;
        }

        DateTime endTime;

        if (string.IsNullOrEmpty(equipEvtModel.EndTime))
        {
            endTime = SqlDateTime.MaxValue.Value;
        }
        else if (!DateTime.TryParse(equipEvtModel.EndTime, out var parseEndTime))
        {
            return OperateResult.Failed<PagedResult<EquipEventResponse>>("结束时间格式错误");
        }
        else
        {
            endTime = parseEndTime;
        }

        var eventType = equipEvtModel.EventType;

        var equipNos = equipEvtModel.EquipNos;

        var sort = equipEvtModel.Sort;

        var eventName = equipEvtModel.EventName;

        if (!string.IsNullOrEmpty(sort))
        {
            var strSort = sort.ToUpperInvariant();
            if (strSort != "ASC" && strSort != "DESC")
            {
                return OperateResult.Failed<PagedResult<EquipEventResponse>>("请求参数错误");
            }
        }

        if (string.IsNullOrEmpty(equipNos) || string.IsNullOrWhiteSpace(eventType))
        {
            return OperateResult.Failed<PagedResult<EquipEventResponse>>("请求参数为空");
        }

        var currentRolePermissionInfos = _permissionCacheService.GetPermissionObj(_session.RoleName);

        var browseEquips = currentRolePermissionInfos?.BrowseEquips ?? new List<int>();

        if (!_session.IsAdmin && !browseEquips.Any())
        {
            return OperateResult.Successed(PagedResult<EquipEventResponse>.Create());
        }

        var equipEvents = new List<EquipEventResponse>();

        if (eventType.ToUpperInvariant() == "S")
        {
            equipEvents = (from a in _context.SetEvt.AsNoTracking()
                           join b in _context.Equip.AsNoTracking() on a.EquipNo equals b.EquipNo into gj
                           from b in gj.DefaultIfEmpty()
                           where b != null
                                 && a.Gwtime >= beginTime && a.Gwtime <= endTime
                           select new EquipEventResponse
                           {
                               Time = a.Gwtime,
                               EquipNo = a.EquipNo,
                               EquipNm = b.EquipNm,
                               Event = a.Gwevent,
                               AlarmLevel = "-",
                               YcyxNo = "-",
                               YcyxType = "S",
                               Confirmname = a.ConfirmName,
                               Confirmtime = a.ConfirmTime,
                               ConfirmRemark = a.Confirmremark,
                               RelatedPic = null as string,
                               RelatedVideo = null as string,
                               ZiChanID = null as string,
                               PlanNo = null as string
                           }).ToList();
        }
        else
        {
            equipEvents = (from a in _context.YcYxEvt.AsNoTracking()
                           join b in _context.Equip.AsNoTracking() on a.EquipNo equals b.EquipNo into gj
                           from b in gj.DefaultIfEmpty()
                           where b != null
                                 && a.Time >= beginTime && a.Time <= endTime
                                 && a.YcyxType == eventType.ToUpperInvariant()
                           select new EquipEventResponse
                           {
                               Time = a.Time,
                               EquipNo = a.EquipNo,
                               EquipNm = b.EquipNm,
                               Event = a.Event,
                               AlarmLevel = "-",
                               YcyxNo = "-",
                               YcyxType = a.YcyxType,
                               Confirmname = a.Confirmname,
                               Confirmtime = a.Confirmtime,
                               ConfirmRemark = a.Confirmremark,
                               RelatedPic = null as string,
                               RelatedVideo = null as string,
                               ZiChanID = null as string,
                               PlanNo = null as string
                           }).ToList();
        }

        if (!string.IsNullOrEmpty(eventName))
        {
            equipEvents = equipEvents.Where(e => e.Event.Contains(eventName)).ToList();
        }

        if (browseEquips.Any())
        {
            equipEvents = equipEvents.Where(e => browseEquips.Contains(e.EquipNo)).ToList();
        }

        if (sort.Equals("asc", StringComparison.OrdinalIgnoreCase))
        {
            equipEvents = equipEvents.OrderBy(d => d.Time).ToList();
        }
        else
        {
            equipEvents = equipEvents.OrderByDescending(d => d.Time).ToList();
        }

        var total = equipEvents.Count;

        var result = equipEvents.Skip((equipEvtModel.PageNo.Value - 1) * equipEvtModel.PageSize.Value).Take(equipEvtModel.PageSize.Value).ToList();
        var data = PagedResult<EquipEventResponse>.Create(total, result);
        return OperateResult.Successed(data);
    }

    public async Task<OperateResult<PagedResult<SysEventResonse>>> GetSysEvtByPage(SysEvtModel sysEvtModel)
    {
        if (sysEvtModel == null)
        {
            return OperateResult.Failed<PagedResult<SysEventResonse>>("请求参数为空");
        }

        var beginTime = string.IsNullOrWhiteSpace(sysEvtModel.BeginTime) ? SqlDateTime.MinValue.Value : Convert.ToDateTime(sysEvtModel.BeginTime);

        var endTime = string.IsNullOrWhiteSpace(sysEvtModel.EndTime) ? DateTime.Now : Convert.ToDateTime(sysEvtModel.EndTime);

        var sort = sysEvtModel.Sort;

        var eventName = sysEvtModel.EventName;

        var realSort = "Desc";
        if (!string.IsNullOrEmpty(sort))
        {
            string strSort = sort.ToUpperInvariant();
            if (strSort != "ASC" && strSort != "DESC")
            {
                return OperateResult.Failed<PagedResult<SysEventResonse>>("请求参数错误");
            }
            realSort = strSort;
        }

        var sysEventQuery = _context.SysEvt
                .AsNoTracking()
                .Where(d => d.Time >= beginTime && d.Time <= endTime)
                .OrderByDescending(d => d.Time)
                .Select(d => new SysEventResonse()
                {
                    Time = d.Time.ToString("yyyy-MM-dd HH:mm:ss"),
                    Event = d.Event,
                    Confirmname = d.Confirmname,
                    ConfirmRemark = d.Confirmremark,
                    Confirmtime = d.Confirmname == null ? "-" : d.Confirmtime.ToString("yyyy-MM-dd HH:mm:ss"),
                    IsAsc = realSort == "ASC"
                });

        if (!string.IsNullOrEmpty(eventName))
        {
            sysEventQuery = sysEventQuery.Where(d => d.Event.Contains(eventName));
        }

        var total = await sysEventQuery.CountAsync();

        var skipRows = (sysEvtModel.PageNo - 1) * sysEvtModel.PageSize;

        var rows = await sysEventQuery.Skip(skipRows.Value).Take(sysEvtModel.PageSize.Value).ToListAsync();

        var pageResult = PagedResult<SysEventResonse>.Create(total, rows);

        return OperateResult.Successed(pageResult);
    }

    public async Task<OperateResult> GetSysEvtCollection(SysEvtType sysEvtType, DateType dateType)
    {
        var list = new List<int>();

        var sysEvts = await _context.SysEvt.AsNoTracking().ToListAsync();

        var startWeek = DateTime.Now.AddDays(1 - int.Parse(DateTime.Now.DayOfWeek.ToString("d")));

        var endWeek = startWeek.AddDays(6);

        switch (dateType)
        {
            case DateType.Week: for (int i = 0; i < 7; i++) { list.Add(sysEvts.Count(s => s.Time.Year == DateTime.Now.Year && s.Time.Day == startWeek.AddDays(i).Day)); } break;
            case DateType.Month: for (int m = 1; m <= 12; m++) { list.Add(sysEvts.Count(s => s.Time.Year == DateTime.Now.Year && s.Time.Month == m)); } break;
            case DateType.Day: list.Add(sysEvts.Count(s => s.Time.Year == DateTime.Now.Year && s.Time.Day == DateTime.Now.Day)); break;
        }
        return OperateResult.Successed(list);
    }
}
