// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Data;
using IoTCenter.Data.Model;
using IoTCenter.Utilities;
using IoTCenterHost.Core.Abstraction;
using IoTCenterHost.Core.Abstraction.AppServices;
using IoTCenterHost.Core.Abstraction.BaseModels;
using IoTCenterHost.Core.Abstraction.Services;
using IoTCenterWebApi.BaseCore;
using IoTCenterWebApi.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.RealTime;

public class RealTimeServiceImpl : IRealTimeService
{
    private readonly Session _session;

    private readonly IotCenterHostService proxy;

    private readonly IAlarmEventClientAppService _alarmEventAppServiceImpl;
    private readonly EquipBaseImpl equipBaseImpl;
    private readonly ILoggingService _apiLog;
    private readonly GWDbContext _context;
    private readonly PermissionCacheService _permissionCacheService;

    public RealTimeServiceImpl(
        Session session,
        IotCenterHostService alarmCenterService,
        IAlarmEventClientAppService alarmEventAppServiceImpl,
        EquipBaseImpl equipBaseImpl,
        ILoggingService apiLog,
        GWDbContext context,
        PermissionCacheService permissionCacheService)
    {
        _session = session;
        proxy = alarmCenterService;
        _alarmEventAppServiceImpl = alarmEventAppServiceImpl;
        this.equipBaseImpl = equipBaseImpl;
        _apiLog = apiLog;
        _context = context;
        _permissionCacheService = permissionCacheService;
    }

    public async Task<OperateResult<IEnumerable<GwsnapshotConfig>>> GetRealTimeEventTypeConfig()
    {
        try
        {
            return OperateResult.Successed<IEnumerable<GwsnapshotConfig>>(await _context.GwsnapshotConfig
                .ToListAsync());
        }
        catch (Exception ex)
        {
            _apiLog.Error("GetRealTimeEventTypeConfig【获取实时快照事件的类型配置信息】:" + ex.ToString() + "\r\n");
            return OperateResult.Failed<IEnumerable<GwsnapshotConfig>>("获取实时快照事件的类型配置信息失败");
        }
    }

    public async Task<OperateResult<IEnumerable<RealTimeEventCount>>> GetRealTimeEventCount()
    {
        var realTimeCount = _alarmEventAppServiceImpl.GetRealTimeGroupCount();

        var shotConfigList = await _context.GwsnapshotConfig.AsNoTracking().ToListAsync();

        var shotConfigRangeInfos = shotConfigList
        .ToDictionary(k => k.SnapshotName, v => Enumerable.Range(v.SnapshotLevelMin, v.SnapshotLevelMax - v.SnapshotLevelMin + 1).ToArray());

        var result = shotConfigRangeInfos
        .Select(configRangeInfo =>
        new RealTimeEventCount()
        {
            Name = configRangeInfo.Key,
            Value = realTimeCount.Where(item => configRangeInfo.Value.Contains(item.Level)).Sum(res => res.Total)
        }).ToList();

        return OperateResult.Successed<IEnumerable<RealTimeEventCount>>(result);
    }


    public OperateResult<RealTimeEventByType> GetRealTimeEventByType(
       RealTimePageModel realTimePageModel)
    {

        if (realTimePageModel == null)
        {
            return OperateResult.Failed<RealTimeEventByType>("返回请求参数为空");
        }

        if (string.IsNullOrEmpty(realTimePageModel.EventType))
        {
            return OperateResult.Successed(new RealTimeEventByType());
        }

        var paginationData = _alarmEventAppServiceImpl.GetRealEventItems(new IoTCenterHost.Core.Abstraction.IotModels.Pagination
        {
            PageIndex = realTimePageModel.PageNo.Value,
            PageSize = realTimePageModel.PageSize.Value,
            WhereCause = realTimePageModel.ToJson()

        });

        var query = paginationData.Data.FromJson<IEnumerable<WcfRealTimeEventItem>>();

        var count = paginationData.Total;

        int[] tmp = query.Select(x => x.Equipno).ToArray();
        var equipDic = _context.Equip.Where(x => tmp.Contains(x.EquipNo)).ToDictionaryAsync(x => x.EquipNo, x => x.EquipNm).Result;

        var list = query
        .Select(d => new RealTimeEventList
        {
            GUID = d.GUID,
            RelatedVideo = d.m_related_video,
            ZiChanID = d.ZiChanID,
            PlanNo = d.PlanNo,
            bConfirmed = d.bConfirmed,
            Time = d.Time,
            Ycyxno = d.Ycyxno,
            Type = d.Type,
            Equipno = d.Equipno,
            EquipName = equipDic.Keys.Contains(d.Equipno) ? equipDic[d.Equipno] : string.Empty,
            ProcAdviceMsg = d.Proc_advice_Msg,
            RelatedPic = d.Related_pic,
            UserConfirm = d.User_Confirmed,
            EventMsg = d.EventMsg,
            Level = d.Level,
            Wavefile = d.Wavefile,
            DTConfirm = d.DT_Confirmed,
            TimeID = d.TimeID
        })
        .OrderByDescending(d => d.Time) as IEnumerable<RealTimeEventList> ?? Array.Empty<RealTimeEventList>();

        var result = new RealTimeEventByType
        {
            PageNo = realTimePageModel.PageNo ?? 1,
            PageSize = realTimePageModel.PageSize ?? count,
            TotalCount = count,
            TotalPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble((double)count / realTimePageModel.PageSize ?? count))),
            IsbConfirmed = query.Count(d => d.bConfirmed),
            NotbConfirmed = query.Count(d => !d.bConfirmed),
            List = list.ToList()
        };
        return OperateResult.Successed(result);
    }

    public OperateResult<RealTimeEventByType> GetRealTimeEventFitter(
        RealTimeFilterPageModel realTimePageModel)
    {

        if (realTimePageModel == null)
        {
            return OperateResult.Failed<RealTimeEventByType>("返回请求参数为空");
        }

        if (string.IsNullOrEmpty(realTimePageModel.EventType))
        {
            return OperateResult.Successed(new RealTimeEventByType());
        }

        var paginationData = _alarmEventAppServiceImpl.GetRealEventItems(new IoTCenterHost.Core.Abstraction.IotModels.Pagination
        {
            PageIndex = realTimePageModel.PageNo.Value,
            PageSize = realTimePageModel.PageSize.Value,
            WhereCause = realTimePageModel.ToJson()
        });

        var query = paginationData.Data.FromJson<IEnumerable<WcfRealTimeEventItem>>();

        var count = paginationData.Total;

        int[] tmp = query.Select(x => x.Equipno).ToArray();
        var equipDic = _context.Equip.Where(x => tmp.Contains(x.EquipNo)).ToDictionaryAsync(x => x.EquipNo, x => x.EquipNm).Result;

        var list = query
        .Select(d => new RealTimeEventList
        {
            GUID = d.GUID,
            RelatedVideo = d.m_related_video,
            ZiChanID = d.ZiChanID,
            PlanNo = d.PlanNo,
            bConfirmed = d.bConfirmed,
            Time = d.Time,
            Ycyxno = d.Ycyxno,
            Type = d.Type,
            Equipno = d.Equipno,
            EquipName = equipDic.Keys.Contains(d.Equipno) ? equipDic[d.Equipno] : string.Empty,
            ProcAdviceMsg = d.Proc_advice_Msg,
            RelatedPic = d.Related_pic,
            UserConfirm = d.User_Confirmed,
            EventMsg = d.EventMsg,
            Level = d.Level,
            Wavefile = d.Wavefile,
            DTConfirm = d.DT_Confirmed,
        })
        .OrderByDescending(d => d.Time) as IEnumerable<RealTimeEventList> ?? Array.Empty<RealTimeEventList>();

        var result = new RealTimeEventByType
        {
            PageNo = realTimePageModel.PageNo ?? 1,
            PageSize = realTimePageModel.PageSize ?? count,
            TotalCount = count,
            TotalPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble((double)count / realTimePageModel.PageSize ?? count))),
            IsbConfirmed = query.Count(d => d.bConfirmed),
            NotbConfirmed = query.Count(d => !d.bConfirmed),
            List = list.ToList()
        };
        return OperateResult.Successed(result);
    }

    public OperateResult<PagedResult<RealTimeEventList>> GetConfirmedRealTimeEventByType(
        RealTimePageModel realTimePageModel)
    {
        if (realTimePageModel == null)
        {
            return OperateResult.Failed<PagedResult<RealTimeEventList>>("请求参数为空");
        }

        if (string.IsNullOrEmpty(realTimePageModel.EventType))
        {
            return OperateResult.Successed(PagedResult<RealTimeEventList>.Create(0, rows: Array.Empty<RealTimeEventList>()));
        }

        var paginationData = _alarmEventAppServiceImpl.GetRealEventItems(new IoTCenterHost.Core.Abstraction.IotModels.Pagination
        {
            PageIndex = realTimePageModel.PageNo.Value,
            PageSize = realTimePageModel.PageSize.Value,
            WhereCause = realTimePageModel.ToJson()

        });

        var query = paginationData.Data.FromJson<IEnumerable<WcfRealTimeEventItem>>();

        var result = query.Skip((realTimePageModel.PageNo - 1).Value * realTimePageModel.PageSize.Value)
            .Take(realTimePageModel.PageSize.Value)
            .Select(d => new RealTimeEventList
            {
                GUID = d.GUID,
                RelatedVideo = d.m_related_video,
                ZiChanID = d.ZiChanID,
                PlanNo = d.PlanNo,
                bConfirmed = d.bConfirmed,
                Time = d.Time,
                Ycyxno = d.Ycyxno,
                Type = d.Type,
                Equipno = d.Equipno,
                ProcAdviceMsg = d.Proc_advice_Msg,
                RelatedPic = d.Related_pic,
                UserConfirm = d.User_Confirmed,
                EventMsg = d.EventMsg,
                Level = d.Level,
                Wavefile = d.Wavefile,
                DTConfirm = d.DT_Confirmed,
                TimeID = d.TimeID
            }).ToList();

        return OperateResult.Successed(PagedResult<RealTimeEventList>.Create(result.Count, result));
    }

    public OperateResult ConfirmRealTimeEvent(ConfirmRealTimeModel confirmRealTimeModel)
    {
        if (confirmRealTimeModel == null || confirmRealTimeModel.GUID == null)
        {
            return OperateResult.Failed("请求参数为空");
        }

        var contain = _alarmEventAppServiceImpl.Contains(confirmRealTimeModel.GUID);
        if (!contain)
        {
            return OperateResult.Failed("请求参数不正确，指定的实时快照不存在.");
        }

        var wcfItem = _alarmEventAppServiceImpl.GetRealTimeEventItem(confirmRealTimeModel.GUID);

        if (wcfItem == null)
        {
            return OperateResult.Failed("请求参数不正确，指定的实时快照不存在.");
        }

        wcfItem.bConfirmed = true;
        wcfItem.Proc_advice_Msg = confirmRealTimeModel.ProcMsg;
        wcfItem.User_Confirmed = _session.UserName;
        wcfItem.DT_Confirmed = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        if (wcfItem.Ycyxno > 0 && wcfItem.Type != null && confirmRealTimeModel.WuBao == 1)
        {
            wcfItem.Level = (int)GrpcMessageLevel.Wubao;
            proxy.SetWuBao(wcfItem.Equipno, wcfItem.Type, wcfItem.Ycyxno);
        }

        if (wcfItem.Equipno > 0 && wcfItem.Type != null &&
            (wcfItem.Type.ToUpper(CultureInfo.InvariantCulture) == "C" ||
             wcfItem.Type.ToUpper(CultureInfo.InvariantCulture) == "X"))
        {
            proxy.Confirm2NormalState(wcfItem.Equipno, wcfItem.Type, wcfItem.Ycyxno);
        }

        _alarmEventAppServiceImpl.ConfirmedRealTimeEventItem(wcfItem);

        _apiLog.Audit(new AuditAction()
        {
            ResourceName = "实时快照",
            EventType = "确认实时快照事件",
            Result = new AuditResult<object, object>()
            {
                Default = "确认成功",
                New = confirmRealTimeModel
            }
        });

        return OperateResult.Success;
    }

    public OperateResult<List<BatchImportRealTimeModel>> GetBatchImportRealTimes(List<int> types)
    {
        var realTimes = new List<BatchImportRealTimeModel>();

        var query = _alarmEventAppServiceImpl.FirstGetRealEventItemExAsync().Result.Where(d => types.Contains(d.Level)).ToList();

        if (!query.Any())
        {
            query = _alarmEventAppServiceImpl.FirstGetRealEventItemExAsync().Result.Where(d => types.Contains(d.Level)).ToList();
        }

        var currentRolePermissionInfos = _permissionCacheService.GetPermissionObj(_session.RoleName);
        
        var browseEquips = currentRolePermissionInfos?.BrowseEquips ?? new List<int>();
        
        if (!_session.IsAdmin && !browseEquips.Any())
        {
            query = query.Where(d => d.Equipno == 0).ToList();
        }
        
        if (!_session.IsAdmin && browseEquips.Any())
        {
            query = query.Where(d => d.Equipno == 0 || browseEquips.Contains(d.Equipno)).ToList();
        }

        var equipNos = query.Select(x => x.Equipno).ToList();

        var equipDic = _context.Equip.Where(x => equipNos.Any(d => d == x.EquipNo))
            .ToDictionary(d => d.EquipNo, d => d.EquipNm);

        realTimes = query
        .Select(d => new BatchImportRealTimeModel
        {
            RelatedVideo = d.m_related_video,
            ZiChanID = d.ZiChanID,
            bConfirmed = d.bConfirmed ? "是" : "否",
            Time = d.Time,
            Ycyxno = d.Ycyxno <= 0 ? "-" : d.Ycyxno.ToString(),
            Type = d.Type?.ToUpper(CultureInfo.InvariantCulture) == "C" ? "遥测"
            : (d.Type?.ToUpper(CultureInfo.InvariantCulture) == "X" ? "遥信" : "-"),
            Equipno = d.Equipno == 0 ? "-" : d.Equipno.ToString(),
            EquipName = equipDic.Keys.Contains(d.Equipno) ? equipDic[d.Equipno] : "-",
            ProcAdviceMsg = d.Proc_advice_Msg,
            UserConfirm = d.User_Confirmed,
            EventMsg = d.EventMsg,
            Level = d.Level,
            DTConfirm = d.DT_Confirmed.ToString() == "1970/1/1 0:00:00" ? "-" : d.DT_Confirmed.ToString(),
        }).OrderByDescending(d => d.Time).ToList();

        return OperateResult.Successed(realTimes);
    }
}