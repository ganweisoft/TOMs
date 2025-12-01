// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using ICSharpCode.SharpZipLib.Zip;
using IoTCenter.Data;
using IoTCenter.Data.Model;
using IoTCenter.Utilities;
using IoTCenter.Utilities.Extensions;
using IoTCenterCore.AutoMapper;
using IoTCenterCore.ExcelHelper;
using IoTCenterHost.Core.Abstraction.AppServices;
using IoTCenterHost.Core.Abstraction.BaseModels;
using IoTCenterHost.Core.Abstraction.Services;
using IoTCenterHost.Proxy;
using IoTCenterHost.Proxy.Models;
using IoTCenterWebApi.BaseCore;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.EquipList;

public class EquipListServiceImpl : IEquipListService
{
    private readonly Session _session;

    private readonly ILoggingService _apiLog;
    private readonly IotCenterHostService proxy;
    private readonly ICurveClientAppService _curveServices;
    private readonly IEquipBaseClientAppService _equipBaseAppService;
    private readonly GWDbContext _context;
    private readonly PermissionCacheService _permissionCacheService;
    private readonly IExportManager _exportManager;
    private readonly IImportManager _importManager;
    private readonly IHttpClientFactory _httpFactory;
    private readonly string[] COMMANDTYPE = { "V", "X", "C", "S", "J" };

    private const string EquipListCacheKey = "EquipListCacheKey";


    public EquipListServiceImpl(
        Session session,
        ICurveClientAppService curveServices,
        IHttpContextAccessor httpContextAccessor,
        IotCenterHostService alarmCenterService,
        IEquipBaseClientAppService equipBaseAppService,
        IExportManager exportManager,
        ILoggingService apiLog,
        GWDbContext context,
        PermissionCacheService permissionCacheService,
        IImportManager importManager,
        IHttpClientFactory httpClientFactory,
        IServiceScopeFactory serviceScopeFactory)
    {
        _session = session;
        proxy = alarmCenterService;
        _equipBaseAppService = equipBaseAppService;

        _exportManager = exportManager;
        _apiLog = apiLog;
        _context = context;
        _permissionCacheService = permissionCacheService;
        _importManager = importManager;
        _httpFactory = httpClientFactory;
        _curveServices = curveServices;
    }

    private void GetEquipStateToList(Dictionary<int, GrpcEquipState> keyValues, List<EquipListModel> equipLists)
    {
        foreach (EquipListModel equipItem in equipLists)
        {
            if (keyValues.TryGetValue(equipItem.EquipID, out var state))
            {
                equipItem.EquipState = state.ToString();
            }
        }
    }

    public async Task<OperateResult> GetRealEquipListByPage(GetEquipListModel getEquipListModel)
    {
        if (getEquipListModel == null)
        {
            return OperateResult.Failed("请求参数为空");
        }

        var equipNoWithYxCount = await _context.Yxp.AsNoTracking()
            .GroupBy(x => x.EquipNo)
            .Select(g => new { EquipNo = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.EquipNo, x => x.Count);

        var equipNoWithYcCount = await _context.Ycp.AsNoTracking()
            .GroupBy(y => y.EquipNo)
            .Select(g => new { EquipNo = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.EquipNo, x => x.Count);

        var equipNoWithSetCount = await _context.SetParm.AsNoTracking()
            .GroupBy(s => s.EquipNo)
            .Select(g => new { EquipNo = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.EquipNo, x => x.Count);

        var equipName = getEquipListModel.EquipName;

        var deviceQuery = from i in _context.EGroupList.AsNoTracking()
                          join e in _context.Equip.AsNoTracking() on i.EquipNo equals e.EquipNo into ej
                          from e in ej.DefaultIfEmpty()
                          where e != null
                          select new
                          {
                              EquipID = i.EquipNo,
                              ParentID = i.GroupId,
                              Name = e.EquipNm ?? "",
                              Equip = e
                          };

        if (!string.IsNullOrWhiteSpace(equipName))
        {
            var pattern = $"%{equipName}%";
            deviceQuery = deviceQuery.Where(x => EF.Functions.Like(x.Name, pattern));
        }

        var equips = await deviceQuery.ToListAsync();

        var devices = equips
            .Select(x => new EquipListModel
            {
                EquipID = x.EquipID,
                ParentID = x.ParentID,
                Name = x.Name,
                EquipState = "",
                yxNum = equipNoWithYxCount.GetValueOrDefault(x.EquipID, 0).ToString(),
                ycNum = equipNoWithYcCount.GetValueOrDefault(x.EquipID, 0).ToString(),
                setNum = equipNoWithSetCount.GetValueOrDefault(x.EquipID, 0).ToString()
            }).ToList();

        if (getEquipListModel.PageNo == null && getEquipListModel.PageSize == null)
        {
            getEquipListModel.PageNo = 1;
            getEquipListModel.PageSize = 9999;
        }

        var totalCount = devices.Count;

        var equipList = devices.Skip((getEquipListModel.PageNo - 1).Value * getEquipListModel.PageSize.Value)
             .Take(getEquipListModel.PageSize.Value).ToList();

        var groupQuery = from g in _context.EGroup.AsNoTracking()
                         select new EGroup
                         {
                             GroupId = g.GroupId,
                             GroupName = g.GroupName,
                             ParentGroupId = g.ParentGroupId
                         };

        if (!string.IsNullOrWhiteSpace(equipName))
        {
            var pattern = $"%{equipName}%";
            groupQuery = groupQuery.Where(x => EF.Functions.Like(x.GroupName, pattern));
        }

        var groups = await groupQuery.OrderBy(x => x.GroupId).ToListAsync();

        var keyValues = _equipBaseAppService.GetEquipStateDict();

        if (keyValues != null && keyValues.Count > 0)
        {
            GetEquipStateToList(keyValues, equipList);
        }

        var page = new Page
        {
            pageNo = getEquipListModel.PageNo ?? 1,
            pageSize = getEquipListModel.PageSize.Value == 9999 ? getEquipListModel.PageSize.Value : totalCount,
            totalCount = totalCount,
            list = new
            {
                groupList = groups.ToJson(),
                equipList
            }
        };

        return OperateResult.Successed(page);
    }

    public OperateResult<Dictionary<int, GrpcEquipState>> GetEquipListStateByPage(GetEquipListStateModel getEquipListStateModel)
    {
        if (getEquipListStateModel == null)
        {
            return OperateResult.Failed<Dictionary<int, GrpcEquipState>>("请求参数为空");
        }
        Dictionary<int, GrpcEquipState> equipItem = null;

        if (!_session.IsAdmin)
        {
            equipItem = GetEquipListStateByPageNode(equipItem);
        }
        else
        {
            equipItem = proxy.GetEquipStateDict();

        }

        return OperateResult.Successed(equipItem);
    }

    private Dictionary<int, GrpcEquipState> GetEquipListStateByPageNode(Dictionary<int, GrpcEquipState> equipItem = null)
    {

        var currentRolePermissionInfos = _permissionCacheService.GetPermissionObj(_session.RoleName);
        var browseEquips = currentRolePermissionInfos?.BrowseEquips ?? new List<int>();
        equipItem = proxy.GetEquipStateDict(browseEquips);

        if (!browseEquips.Any())
        {
            return new Dictionary<int, GrpcEquipState>();
        }

        var result = equipItem.Where(d => browseEquips.Contains(d.Key)).ToDictionary(k => k.Key, v => v.Value);

        return result;
    }

    public OperateResult<Page> GetEquipItemStateByPage(GetEquipItemStateModel getEquipItemStateModel)
    {
        if (getEquipItemStateModel == null)
        {
            return OperateResult.Failed<Page>("请求参数为空");
        }

        var equipNo = getEquipItemStateModel.EquipNo;

        var GrpcEquipState = proxy.GetEquipStateFromEquipNo(equipNo);

        var YCItemDict = proxy.GetYCValueDictFromEquip(equipNo);

        var YXItemDict = proxy.GetYXValueDictFromEquip(equipNo);

        var SetItemDict = proxy.GetSetListStr(equipNo);

        var page = new Page
        {
            pageNo = getEquipItemStateModel.PageNo.Value,
            pageSize = getEquipItemStateModel.PageSize.Value,
            totalCount = 100,
            totalPage = 10,
            list = new
            {
                GrpcEquipState = GrpcEquipState.ToString(),
                YCItemDict = JsonConvert.SerializeObject(YCItemDict),
                YXItemDict = JsonConvert.SerializeObject(YXItemDict),
                SetItemDict
            }
        };

        return OperateResult.Successed(page);
    }

    public OperateResult GetEquipItemStateByIds(GetEquipItemStateByIds getEquipItemStateByIds)
    {
        List<object> list = new List<object>();
        if (getEquipItemStateByIds == null)
        {
            return OperateResult.Failed("请求参数为空");
        }
        var rootEquipNoList = _context.Equip.Select(o => o.EquipNo).ToList();

        var equipStates = proxy.GetEquipStateDict(getEquipItemStateByIds.EquipNos);

        foreach (var equipNo in getEquipItemStateByIds.EquipNos)
        {
            if (rootEquipNoList.Exists(o => o == equipNo))
            {
                var GrpcEquipState = proxy.GetEquipStateFromEquipNo(equipNo);

                list.Add(new
                {
                    GrpcEquipState = GrpcEquipState.ToString(),
                });
            }
        }
        return OperateResult.Successed(list);
    }

    public OperateResult GetYcpItemValueAndState(GetEquipItemStateModel getEquipItemStateModel)
    {
        List<object> ycValueList = new List<object>();
        if (getEquipItemStateModel == null)
        {
            return OperateResult.Failed<List<object>>("请求参数为空");
        }

        var equipNo = getEquipItemStateModel.EquipNo;
        var rootEquipNoList = _context.Equip.Select(o => o.EquipNo).ToList();
        if (rootEquipNoList.Exists(o => o == equipNo))
        {
            GrpcEquipState GrpcEquipState = proxy.GetEquipStateFromEquipNo(equipNo);
            Dictionary<int, object> YCItemDict = proxy.GetYCValueDictFromEquip(equipNo);
            var ycList = _context.Ycp.Where(o => o.EquipNo == equipNo).ToList();
            foreach (var dict in YCItemDict)
            {
                var ycUnit = ycList.Where(o => o.YcNo == dict.Key).Select(o => o.Unit).FirstOrDefault();
                ycValueList.Add(new
                {
                    YcNo = dict.Key,
                    Value = dict.Value,
                    Unit = ycUnit
                });
            }
        }
        return OperateResult.Successed(ycValueList);
    }

    public OperateResult<Page> GetYcpItemStateByPage(GetEquipItemStateModel getEquipItemStateModel)
    {
        if (getEquipItemStateModel == null)
        {
            return OperateResult.Failed<Page>("请求参数为空");
        }

        var equipNo = getEquipItemStateModel.EquipNo;

        var GrpcEquipState = proxy.GetEquipStateFromEquipNo(equipNo);

        var YCItemDict = proxy.GetYCValueDictFromEquip(equipNo);

        var page = new Page
        {
            pageNo = getEquipItemStateModel.PageNo.Value,
            pageSize = getEquipItemStateModel.PageSize.Value,
            totalCount = 100,
            totalPage = 10,
            list = new
            {
                GrpcEquipState,
                YCItemDict = JsonConvert.SerializeObject(YCItemDict),
            }
        };

        return OperateResult.Successed(page);
    }

    public OperateResult GetYxpItemValueAndState(GetEquipItemStateModel getEquipItemStateModel)
    {
        List<object> yxValueList = new List<object>();
        if (getEquipItemStateModel == null)
        {
            return OperateResult.Failed<List<object>>("请求参数为空");
        }

        var equipNo = getEquipItemStateModel.EquipNo;
        var rootEquipNoList = _context.Equip.Select(o => o.EquipNo).ToList();
        if (rootEquipNoList.Exists(o => o == equipNo))
        {
            var yxItemDict = proxy.GetYXValueDictFromEquip(equipNo);
            foreach (var dict in yxItemDict)
            {
                yxValueList.Add(new
                {
                    YxNo = dict.Key,
                    Value = dict.Value,
                });
            }
        }
        return OperateResult.Successed(yxValueList);
    }

    public OperateResult<Page> GetYxpItemStateByPage(GetEquipItemStateModel getEquipItemStateModel)
    {
        if (getEquipItemStateModel == null)
        {
            return OperateResult.Failed<Page>("请求参数为空");
        }

        var equipNo = getEquipItemStateModel.EquipNo;

        var GrpcEquipState = proxy.GetEquipStateFromEquipNo(equipNo);

        var YXItemDict = proxy.GetYXValueDictFromEquip(equipNo);

        var page = new Page
        {
            pageNo = getEquipItemStateModel.PageNo.Value,
            pageSize = getEquipItemStateModel.PageSize.Value,
            totalCount = 100,
            totalPage = 10,
            list = new
            {
                GrpcEquipState,
                YXItemDict = JsonConvert.SerializeObject(YXItemDict),
            }
        };

        return OperateResult.Successed(page);
    }

    public OperateResult<Page> GetSetItemStateByPage(GetEquipItemStateModel getEquipItemStateModel)
    {
        if (getEquipItemStateModel == null)
        {
            return OperateResult.Failed<Page>("请求参数为空");
        }

        var equipNo = getEquipItemStateModel.EquipNo;

        var GrpcEquipState = proxy.GetEquipStateFromEquipNo(equipNo);

        var SetItemDict = proxy.GetSetListStr(equipNo);

        var page = new Page
        {
            pageNo = getEquipItemStateModel.PageNo.Value,
            pageSize = getEquipItemStateModel.PageSize.Value,
            totalCount = 100,
            totalPage = 10,
            list = new
            {
                GrpcEquipState,
                SetItemDict,
            }
        };

        return OperateResult.Successed(page);
    }

    public OperateResult<object> GetEquipYcpState(GetEquipYcYxModel getEquipYcYxModel)
    {
        if (getEquipYcYxModel == null)
        {
            return OperateResult.Failed<object>("请求参数为空");
        }

        var equipNo = getEquipYcYxModel.EquipNo;
        var ycyxNo = getEquipYcYxModel.YcyxNo;

        return OperateResult.Successed(proxy.GetYCValue(equipNo, ycyxNo));
    }

    public async Task<OperateResult> SetCommandBySetNo(CommandBySetNoModel commandBySetNoModel)
    {
        if (commandBySetNoModel == null)
        {
            return OperateResult.Failed("请求参数为空，请检查");
        }

        if (!this.COMMANDTYPE.Contains(commandBySetNoModel.SetType))
        {
            return OperateResult.Failed("设置量设置类型参数不正确");
        }

        var equipNo = commandBySetNoModel.EquipNo;
        var setNo = commandBySetNoModel.SetNo;
        var value = commandBySetNoModel.Value;
        var setType = commandBySetNoModel.SetType;

        var setparm = await _context.SetParm.AsNoTracking()
            .FirstOrDefaultAsync(x => x.EquipNo == equipNo && x.SetNo == setNo && x.SetType == setType);

        if (setparm == null)
        {
            return OperateResult.Failed<Page>("设备命令不存在");
        }

        var strValue = setparm.Value;
        var set_type = setparm.SetType;

        if (set_type.IsEmpty())
        {
            return OperateResult.Failed("执行失败，设备设置项配置信息错误");
        }

        value = SetCommandBySetNoTwo(value, strValue);

        proxy.SetParm1_1(equipNo, setNo, value, _session.UserName, true);

        await _apiLog.Audit(new AuditAction()
        {
            ResourceName = "设备列表",
            EventType = "设置",
            Result = new AuditResult()
            {
                Default = $"设置号({setNo})下发成功，设备号({equipNo})，设置值({value})"
            }
        });

        return OperateResult.Success;
    }

    private static string SetCommandBySetNoTwo(string value, string str_value)
    {
        if (value.HasValue())
        {
            return value;
        }
        if (str_value.HasValue())
        {
            return str_value;
        }

        return "1";
    }
    public OperateResult<object> GetEquipYxpState(GetEquipYcYxModel getEquipYcYxModel)
    {
        if (getEquipYcYxModel == null)
        {
            return OperateResult.Failed<object>("请求参数为空");
        }

        var equipNo = getEquipYcYxModel.EquipNo;
        var ycyxNo = getEquipYcYxModel.YcyxNo;

        var data = proxy.GetYXValue(equipNo, ycyxNo);

        return OperateResult.Successed(data);
    }

    public OperateResult SetCommandByParameter(CommandByParameterModel commandByParameterModel)
    {
        if (commandByParameterModel == null)
        {
            return OperateResult.Failed<Page>("请求参数为空");
        }

        var equipNo = commandByParameterModel.EquipNo;

        var equip = _context.Equip.AsNoTracking().FirstOrDefault(e => e.EquipNo == equipNo);

        if (equip == null)
        {
            return OperateResult.Failed<Page>("设备不存在");
        }

        var mainInstr = commandByParameterModel.MainInstr;
        var minoInstr = commandByParameterModel.MinoInstr;
        var value = commandByParameterModel.Value;
        var userName = commandByParameterModel.UserName;

        if (string.IsNullOrEmpty(mainInstr) || string.IsNullOrEmpty(minoInstr)
            || string.IsNullOrEmpty(userName))
        {
            return OperateResult.Failed<Page>("请求参数为空");
        }

        proxy.SetParm(equipNo, mainInstr, minoInstr, value, userName);

        _apiLog.Audit(new AuditAction()
        {
            ResourceName = "设备接口",
            EventType = "编辑设备",
            Result = new AuditResult<object, object>()
            {
                Old = equipNo,
                New = commandByParameterModel,
                Default = "指令下发"
            }
        });
        return OperateResult.Success;
    }

    public OperateResult<Page> GetYcpHistroyByTimeAsync(GetYcpHistroyModel getYcpHistroyModel)
    {
        if (getYcpHistroyModel == null || string.IsNullOrWhiteSpace(getYcpHistroyModel.BeginTime)
            || string.IsNullOrWhiteSpace(getYcpHistroyModel.EndTime))
        {
            return OperateResult.Failed<Page>("请求参数为空");
        }

        if (!DateTime.TryParse(getYcpHistroyModel.BeginTime, out var beginTime))
        {
            return OperateResult.Failed<Page>("开始时间格式不正确");
        }

        if (!DateTime.TryParse(getYcpHistroyModel.EndTime, out var endTime))
        {
            return OperateResult.Failed<Page>("结束时间格式不正确");
        }

        var staNo = getYcpHistroyModel.StaNo;
        var equipNo = getYcpHistroyModel.EquipNo;
        var ycpNo = getYcpHistroyModel.YcpNo;
        string type = getYcpHistroyModel.Type;

        if (string.IsNullOrEmpty(type))
        {
            type = "C";
        }

        var ycpData = _context.Ycp.AsNoTracking().FirstOrDefault(x => x.EquipNo == getYcpHistroyModel.EquipNo && x.YcNo == getYcpHistroyModel.YcpNo);

        if (ycpData == null)
        {
            return OperateResult.Failed<Page>("设备或测点不存在");
        }

        int totalRecords = 0;

        Page page = new Page();

        if (ycpData == null)
        {
            return OperateResult.Successed<Page>(page);
        }

        var result = proxy.GetChangedDataFromCurveAsync(beginTime, endTime, staNo, equipNo, ycpNo, type).Result;
        totalRecords = result.Count;

        int realCurrentPage = getYcpHistroyModel.PageNo > 0 ? (getYcpHistroyModel.PageNo - 1).Value : 0;
        int realPageSize = getYcpHistroyModel.PageSize > 0 ? getYcpHistroyModel.PageSize.Value : totalRecords;

        int skipRow = realCurrentPage * realPageSize;
        var lists = result.Skip(skipRow).Take(realPageSize).ToList();

        var times = lists.Select(d => d.datetime.ToString(@"MM/dd HH:mm:ss"));
        var values = lists.Select(d => d.value);
        page = new Page
        {
            pageNo = realCurrentPage,
            pageSize = realPageSize,
            totalCount = totalRecords,
            list = new
            {
                times,
                values
            }
        };

        return OperateResult.Successed(page);
    }

    public OperateResult<List<MyCurveCommonData>> GetYcpHistroyChartByTimeAsync(GetYcpHistroyModel getYcpHistroyModel)
    {
        if (getYcpHistroyModel == null || string.IsNullOrWhiteSpace(getYcpHistroyModel.BeginTime)
            || string.IsNullOrWhiteSpace(getYcpHistroyModel.EndTime))
        {
            return OperateResult.Failed<List<MyCurveCommonData>>("请求参数为空");
        }

        if (!DateTime.TryParse(getYcpHistroyModel.BeginTime, out var beginTime))
        {
            return OperateResult.Failed<List<MyCurveCommonData>>("开始时间格式不正确");
        }

        if (!DateTime.TryParse(getYcpHistroyModel.EndTime, out var endTime))
        {
            return OperateResult.Failed<List<MyCurveCommonData>>("结束时间格式不正确");
        }

        var staNo = getYcpHistroyModel.StaNo;
        var equipNo = getYcpHistroyModel.EquipNo;
        var ycpNo = getYcpHistroyModel.YcpNo;
        var type = getYcpHistroyModel.Type;

        if (string.IsNullOrEmpty(type))
        {
            type = "C";
        }

        var lists = new List<MyCurveCommonData>();

        var ts = endTime - beginTime;

        var dayType = GetYcpHistroyChartByTimeAsyncOne(ts);

        var result = proxy.GetChangedDataFromCurveAsync(beginTime, endTime, staNo, equipNo, ycpNo, type).Result;

        if (result == null || !result.Any())
        {
            return OperateResult.Successed(lists);
        }

        foreach (var item in result)
        {
            if (item.datetime >= beginTime && item.datetime <= endTime)
            {
                GetYcpHistroyChartByTimeAsyncTwo(beginTime, endTime, dayType, lists, item);
            }
        }
        return OperateResult.Successed(lists);
    }

    private static void GetYcpHistroyChartByTimeAsyncTwo(DateTime beginTime, DateTime endTime, string dayType, List<MyCurveCommonData> lists, GrpcMyCurveData item)
    {
        switch (dayType)
        {
            case "day":
                if (GetDay(beginTime, endTime, lists, item.datetime))
                {
                    lists.Add(new MyCurveCommonData
                    {
                        Datetime = Convert.ToDateTime(item.datetime.ToString("yyyy-MM-dd HH:00:00")),
                        Value = item.value
                    });
                }
                break;
            case "week":
                if (GetWeek(beginTime, endTime, lists, item.datetime))
                {
                    lists.Add(new MyCurveCommonData
                    {
                        Datetime = Convert.ToDateTime(item.datetime.ToString("yyyy-MM-dd 00:00:00")),
                        Value = item.value
                    });
                }
                break;
            case "month":
                if (GetWeek(beginTime, endTime, lists, item.datetime))
                {
                    lists.Add(new MyCurveCommonData
                    {
                        Datetime = Convert.ToDateTime(item.datetime.ToString("yyyy-MM-dd 00:00:00")),
                        Value = item.value
                    });
                }
                break;
            case "quarter":
                if (GetMonth(beginTime, endTime, lists, item.datetime))
                {
                    lists.Add(new MyCurveCommonData
                    {
                        Datetime = Convert.ToDateTime(item.datetime.ToString("yyyy-MM-01 00:00:00")),
                        Value = item.value
                    });
                }
                break;
            case "halfYear":
                if (GetMonth(beginTime, endTime, lists, item.datetime))
                {
                    lists.Add(new MyCurveCommonData
                    {
                        Datetime = Convert.ToDateTime(item.datetime.ToString("yyyy-MM-01 00:00:00")),
                        Value = item.value
                    });
                }
                break;
            case "year":
                if (GetMonth(beginTime, endTime, lists, item.datetime))
                {
                    lists.Add(new MyCurveCommonData
                    {
                        Datetime = Convert.ToDateTime(item.datetime.ToString("yyyy-MM-01 00:00:00")),
                        Value = item.value
                    });
                }
                break;
            case "other":
                lists.Add(new MyCurveCommonData
                {
                    Datetime = Convert.ToDateTime(item.datetime.ToString("yyyy-MM-dd HH:mm:ss")),
                    Value = item.value
                });
                break;
        }
    }

    private static string GetYcpHistroyChartByTimeAsyncOne(TimeSpan ts)
    {
        string dayType;
        if (ts.TotalDays >= 1 && ts.TotalDays < 7)
        {
            dayType = "day";
        }
        else if (ts.TotalDays >= 7 && ts.TotalDays < 28)
        {
            dayType = "week";
        }
        else if (ts.TotalDays >= 28 && ts.TotalDays < 84)
        {
            dayType = "month";
        }
        else if (ts.TotalDays >= 84 && ts.TotalDays < 170)
        {
            dayType = "quarter";
        }
        else if (ts.TotalDays >= 170 && ts.TotalDays < 350)
        {
            dayType = "halfYear";
        }
        else if (ts.TotalDays >= 350)
        {
            dayType = "year";
        }
        else
        {
            dayType = "other";
        }

        return dayType;
    }

    public static bool GetDay(DateTime bgn, DateTime end, List<MyCurveCommonData> lists, DateTime dateTime)
    {
        if (lists == null || lists.Count <= 0)
        {
            return false;
        }

        foreach (var item in lists)
        {
            if (dateTime.ToString("yyyy-MM-dd HH:00:00") == item.Datetime.ToString("yyyy-MM-dd HH:00:00"))
            {
                return false;
            }
        }

        bool flag = false;
        var dTime = Convert.ToDateTime(dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
        var bTime = Convert.ToDateTime(bgn.ToString("yyyy-MM-dd HH:00:00"));
        var eTime = Convert.ToDateTime(end.ToString("yyyy-MM-dd HH:59:59"));
        if (dTime >= bTime && dateTime <= eTime)
        {
            flag = true;
        }

        return flag;
    }

    public static bool GetWeek(DateTime bgn, DateTime end, List<MyCurveCommonData> lists, DateTime dateTime)
    {
        if (lists == null || lists.Count <= 0)
        {
            return false;
        }

        foreach (var item in lists)
        {
            if (dateTime.ToString("yyyy-MM-dd 00:00:00") == item.Datetime.ToString("yyyy-MM-dd 00:00:00"))
            {
                return false;
            }
        }

        bool flag = false;
        var dTime = Convert.ToDateTime(dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
        var bTime = Convert.ToDateTime(bgn.ToString("yyyy-MM-dd 00:00:00"));
        var eTime = Convert.ToDateTime(end.ToString("yyyy-MM-dd 23:59:59"));
        if (dTime >= bTime && dateTime <= eTime)
        {
            flag = true;
        }

        return flag;
    }

    public static bool GetMonth(DateTime bgn, DateTime end, List<MyCurveCommonData> lists, DateTime dateTime)
    {
        if (lists == null || lists.Count <= 0)
        {
            return false;
        }

        foreach (var item in lists)
        {
            if (dateTime.ToString("yyyy-MM-01 00:00:00") == item.Datetime.ToString("yyyy-MM-01 00:00:00"))
            {
                return false;
            }
        }

        bool flag = false;
        var dTime = Convert.ToDateTime(dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
        var bTime = Convert.ToDateTime(bgn.ToString("yyyy-MM-01 00:00:00"));
        var eTime = Convert.ToDateTime(end.ToString("yyyy-MM-28 23:59:59"));
        if (dTime >= bTime && dateTime <= eTime)
        {
            flag = true;
        }

        return flag;
    }

    public OperateResult<PagedResult<GetEquipNoAndNameResponse>> GetEquipNoAndName(GetEquipListStateModel getEquipListStateModel)
    {
        if (getEquipListStateModel == null)
        {
            return OperateResult.Failed<PagedResult<GetEquipNoAndNameResponse>>("1003");
        }

        var equipList = _context.Equip.Where(o => true);
        var currentRolePermissionInfos = _permissionCacheService.GetPermissionObj(_session.RoleName);
        var browseEquips = currentRolePermissionInfos?.BrowseEquips ?? new List<int>();

        if (!_session.IsAdmin)
        {
            if (!browseEquips.Any())
            {
                return OperateResult.Successed(PagedResult<GetEquipNoAndNameResponse>.Create(json: ""));
            }
            equipList = equipList.Where(o => browseEquips.Contains(o.EquipNo));
        }

        if (!equipList.Any())
        {
            return OperateResult.Successed(PagedResult<GetEquipNoAndNameResponse>.Create(json: ""));
        }

        var totalEquipList = equipList.Select(o => new GetEquipNoAndNameResponse()
        {
            staN = o.StaN,
            equipNo = o.EquipNo,
            equipNm = o.EquipNm,
            ycNum = _context.Ycp.AsNoTracking().Count(e => e.EquipNo == o.EquipNo),
            yxNum = _context.Yxp.AsNoTracking().Count(e => e.EquipNo == o.EquipNo)
        }).ToList();

        var pageNo = getEquipListStateModel.PageNo;
        var pageSize = getEquipListStateModel.PageSize;
        if (pageNo == null && pageSize == null)
        {
            pageNo = 1;
            pageSize = 9999;
        }

        var skipCount = (pageNo - 1) * pageSize;

        var paginEquipList = totalEquipList.Skip(skipCount.Value).Take(pageSize.Value).ToList();

        return OperateResult.Successed(PagedResult<GetEquipNoAndNameResponse>.Create(totalEquipList.Count, paginEquipList, JsonConvert.SerializeObject(paginEquipList)));
    }

    public OperateResult<PagedResult<string>> GetSetEquip(GetEquipListStateModel getEquipListStateModel)
    {
        if (getEquipListStateModel == null)
        {
            return OperateResult.Failed<PagedResult<string>>("1003");
        }

        IQueryable<SetParm> setParamFilterResult = _context.SetParm.AsNoTracking();

        if (!_session.IsAdmin)
        {
            setParamFilterResult = setParamFilterResult.ApplyRoleEquipFilter(_permissionCacheService.GetPermissionObj(_session.RoleName));
        }

        var query = (from a in setParamFilterResult
                     join b in _context.Equip.AsNoTracking() on a.EquipNo equals b.EquipNo
                     select new GetSetEquipModel
                     {
                         EquipNo = a.EquipNo,
                         EquipNm = b.EquipNm,
                         SetNo = a.SetNo
                     }).ToList();

        var result = query.GroupBy(query => new { query.EquipNo, query.EquipNm })
             .Select(g => new GetSetEquipModel
             {
                 EquipNo = g.Key.EquipNo,
                 EquipNm = g.Key.EquipNm
             }).ToList();

        var pageNo = getEquipListStateModel.PageNo;
        var pageSize = getEquipListStateModel.PageSize;

        if (!pageNo.HasValue)
        {
            pageNo = 1;
        }

        if (!pageSize.HasValue)
        {
            pageSize = 9999;
        }

        var totalCount = result.Count;

        var pageResult = result.Skip((pageNo.Value - 1) * pageSize.Value)
            .Take(pageSize.Value);

        return OperateResult.Successed(PagedResult<string>.Create(totalCount, pageResult.ToJson()));
    }

    public OperateResult<PagedResult<string>> GetEquipAndSet(GetEquipListStateModel getEquipListStateModel)
    {
        if (getEquipListStateModel == null)
        {
            return OperateResult.Failed<PagedResult<string>>("请求参数为空");
        }

        IQueryable<SetParm> setParamFilterResult = _context.SetParm.AsNoTracking();

        if (!_session.IsAdmin)
        {
            setParamFilterResult = setParamFilterResult.ApplyRoleEquipFilter(_permissionCacheService.GetPermissionObj(_session.RoleName));
        }

        var query = (from a in setParamFilterResult
                     join b in _context.Equip.AsNoTracking() on a.EquipNo equals b.EquipNo
                     select new GetSetEquipModel
                     {
                         EquipNo = a.EquipNo,
                         EquipNm = b.EquipNm,
                         SetNo = a.SetNo
                     }).ToList();

        var result = query.GroupBy(query => new { query.EquipNo, query.EquipNm })
             .Select(g => new GetSetEquipModel
             {
                 EquipNo = g.Key.EquipNo,
                 EquipNm = g.Key.EquipNm
             }).ToList();

        var pageNo = getEquipListStateModel.PageNo;
        var pageSize = getEquipListStateModel.PageSize;

        if (!pageNo.HasValue)
        {
            pageNo = 1;
        }

        if (!pageSize.HasValue)
        {
            pageSize = 9999;
        }

        var totalCount = result.Count;

        var pageResult = result.Skip((pageNo.Value - 1) * pageSize.Value)
            .Take(pageSize.Value);

        return OperateResult.Successed(PagedResult<string>.Create(totalCount, pageResult.ToJson()));
    }

    public async Task<OperateResult<PageResult<YcpList>>> GetYcpByEquipNo(GetEquipItemStateModel getEquipItemStateModel)
    {

        if (getEquipItemStateModel == null)
        {
            return OperateResult.Successed(PageResult<YcpList>.Create());
        }

        if (!await _context.Ycp.AnyAsync(x => x.EquipNo == getEquipItemStateModel.EquipNo))
        {
            return OperateResult.Successed(PageResult<YcpList>.Create());
        }

        var equipNo = getEquipItemStateModel.EquipNo;
        var currentRolePermissionInfos = _permissionCacheService.GetPermissionObj(_session.RoleName);
        var browseEquips = currentRolePermissionInfos?.BrowseEquips ?? new List<int>();

        if (!_session.IsAdmin && !browseEquips.Any(d => d == equipNo))
        {
            return OperateResult.Successed(PageResult<YcpList>.Create());
        }

        var ycpLists = (from yc in _context.Ycp.AsNoTracking()
                        join zc in _context.GwziChanTable.AsNoTracking()
                            on yc.ZiChanId equals zc.ZiChanId into zcGroup
                        from zc in zcGroup.DefaultIfEmpty()
                        where yc.EquipNo == equipNo
                        select new YcpList
                        {
                            StaN = yc.StaN,
                            EquipNo = yc.EquipNo,
                            YcNo = yc.YcNo,
                            YcNm = yc.YcNm,
                            ProcAdvice = yc.ProcAdvice,
                            RelatedPic = yc.RelatedPic,
                            RelatedVideo = yc.RelatedVideo,
                            ZiChanID = yc.ZiChanId,
                            PlanNo = yc.PlanNo,
                            Unit = yc.Unit
                        }).ToList();

        int? pageNo = getEquipItemStateModel.PageNo;
        int? pageSize = getEquipItemStateModel.PageSize;

        if (!pageNo.HasValue)
        {
            pageNo = 1;
        }

        if (!pageSize.HasValue)
        {
            pageSize = 9999;
        }

        if (!string.IsNullOrEmpty(getEquipItemStateModel.SearchName))
        {
            ycpLists = ycpLists.Where(x => x.YcNm.Contains(getEquipItemStateModel.SearchName)).ToList();
        }

        var total = ycpLists.Count;

        ycpLists = ycpLists.Skip((pageNo.Value - 1) * pageSize.Value)
            .Take(pageSize.Value).ToList();

        Dictionary<int, object> ycValueDict = null;

        Dictionary<int, bool> ycStateDict = null;

        GetYcpByEquipNo(equipNo, ref ycValueDict, ref ycStateDict);

        foreach (YcpList ycpItem in ycpLists)
        {
            GetYcpValueAndStateByEquipNo(ycValueDict, ycStateDict, ycpItem);
        }

        return OperateResult.Successed(PageResult<YcpList>.Create(total, ycpLists));
    }

    private void GetYcpValueAndStateByEquipNo(Dictionary<int, object> ycValueDict, Dictionary<int, bool> ycStateDict, YcpList ycpItem)
    {
        if (ycValueDict != null && ycValueDict.Count > 0)
        {
            if (ycValueDict.TryGetValue(ycpItem.YcNo, out var value))
            {
                ycpItem.Value = value?.ToString();
            }
            else
            {
                ycpItem.Value = proxy.GetYCValue(ycpItem.EquipNo, ycpItem.YcNo)?.ToString();
            }
        }

        if (ycStateDict != null && ycStateDict.Count > 0)
        {
            if (ycStateDict.TryGetValue(ycpItem.YcNo, out var state))
            {
                ycpItem.State = state;
            }
            else
            {
                ycpItem.State = proxy.GetYCAlarmState(ycpItem.EquipNo, ycpItem.YcNo);
            }
        }
    }

    private void GetYcpByEquipNo(int realEquipNo, ref Dictionary<int, object> YCValueDict, ref Dictionary<int, bool> YCStateDict)
    {
        try
        {
            YCValueDict = proxy.GetYCValueDictFromEquip(realEquipNo);
            YCStateDict = proxy.GetYCAlarmStateDictFromEquip(realEquipNo);
        }
        catch (Exception ex)
        {
            _apiLog.Error("GetYCValueDictFromEquip:" + ex.ToString());
        }
    }

    public async Task<OperateResult<PageResult<YxpList>>> GetYxpByEquipNo(GetEquipItemStateModel getEquipItemStateModel)
    {
        if (getEquipItemStateModel == null)
        {
            return OperateResult.Successed(PageResult<YxpList>.Create());
        }

        if (!await _context.Yxp.AsNoTracking().AnyAsync(x => x.EquipNo == getEquipItemStateModel.EquipNo))
        {
            return OperateResult.Successed(PageResult<YxpList>.Create());
        }

        var equipNo = getEquipItemStateModel.EquipNo;
        var currentRolePermissionInfos = _permissionCacheService.GetPermissionObj(_session.RoleName);
        var browseEquips = currentRolePermissionInfos?.BrowseEquips ?? new List<int>();

        if (!_session.IsAdmin && !browseEquips.Any(d => d == equipNo))
        {
            return OperateResult.Successed(PageResult<YxpList>.Create());
        }

        var yxpLists = (from yx in _context.Yxp.AsNoTracking()
                        join zc in _context.GwziChanTable.AsNoTracking()
                            on yx.ZiChanId equals zc.ZiChanId into zcGroup
                        from zc in zcGroup.DefaultIfEmpty()
                        where yx.EquipNo == equipNo
                        select new YxpList
                        {
                            StaN = yx.StaN,
                            EquipNo = yx.EquipNo,
                            YxNo = yx.YxNo,
                            YxNm = yx.YxNm,
                            ProcAdvice = yx.ProcAdviceR,
                            RelatedPic = yx.RelatedPic,
                            RelatedVideo = yx.RelatedVideo,
                            ZiChanID = yx.ZiChanId,
                            PlanNo = yx.PlanNo
                        }).ToList();

        var pageNo = getEquipItemStateModel.PageNo;
        var pageSize = getEquipItemStateModel.PageSize;

        if (!pageNo.HasValue)
        {
            pageNo = 1;
        }

        if (!pageSize.HasValue)
        {
            pageSize = 9999;
        }

        if (!string.IsNullOrEmpty(getEquipItemStateModel.SearchName))
        {
            yxpLists = yxpLists.Where(x => x.YxNm.Contains(getEquipItemStateModel.SearchName)).ToList();
        }

        var total = yxpLists.Count;

        yxpLists = yxpLists.Skip((pageNo.Value - 1) * pageSize.Value)
            .Take(pageSize.Value).ToList();

        try
        {
            var values = proxy.GetYXValueDictFromEquip(equipNo);
            var states = proxy.GetYXAlarmStateDictFromEquip(equipNo);
            foreach (YxpList yxpItem in yxpLists)
            {
                yxpItem.State = states.TryGetValue(yxpItem.YxNo, out var sval) ? sval : false;
                yxpItem.Value = values.TryGetValue(yxpItem.YxNo, out var val) ? val : string.Empty;
            }
        }
        catch (Exception e)
        {
            _apiLog.Error($"获取当前设备状态量集合失败,GRPC日志情况:{e}");
        }

        return OperateResult.Successed(PageResult<YxpList>.Create(total, yxpLists));
    }

    public async Task<OperateResult<PageResult<EquipSetParmQuery>>> GetSetParmByEquipNo(GetEquipItemStateModel getEquipItemStateModel)
    {
        if (getEquipItemStateModel == null)
        {
            return OperateResult.Successed(PageResult<EquipSetParmQuery>.Create());
        }

        if (!await _context.SetParm.AnyAsync(x => x.EquipNo == getEquipItemStateModel.EquipNo))
        {
            return OperateResult.Successed(PageResult<EquipSetParmQuery>.Create());
        }

        var equipNo = getEquipItemStateModel.EquipNo;

        var equipSetParms = (from a in _context.SetParm.AsNoTracking()
                             join b in _context.Equip.AsNoTracking() on a.EquipNo equals b.EquipNo
                             where a.EquipNo == equipNo
                             select new EquipSetParmQuery
                             {
                                 EquipNo = a.EquipNo,
                                 SetNo = a.SetNo,
                                 StaN = a.StaN,
                                 SetNm = a.SetNm,
                                 SetType = a.SetType,
                                 Value = a.Value
                             }).ToList();

        if (!_session.IsAdmin)
        {
            var currentRolePermissionInfos = _permissionCacheService.GetPermissionObj(_session.RoleName);

            var controlEquips = currentRolePermissionInfos?.ControlEquips ?? new List<int>();

            var controlEquipsUnit = currentRolePermissionInfos?.ControlEquipsUnitOfDict ?? new Dictionary<int, IEnumerable<int>>();

            var unionEquips = controlEquips.Union(controlEquipsUnit.Keys);

            if (unionEquips.All(d => d != equipNo))
            {
                return OperateResult.Successed(PageResult<EquipSetParmQuery>.Create());
            }

            if (controlEquipsUnit.TryGetValue(equipNo, out var setList))
            {
                equipSetParms = equipSetParms.Where(s => setList.Contains(s.SetNo)).ToList();
            }
        }

        var pageNo = getEquipItemStateModel.PageNo;
        var pageSize = getEquipItemStateModel.PageSize;

        if (!pageNo.HasValue)
        {
            pageNo = 1;
        }

        if (!pageSize.HasValue)
        {
            pageSize = 9999;
        }

        if (!string.IsNullOrEmpty(getEquipItemStateModel.SearchName))
        {
            equipSetParms = equipSetParms.Where(x => x.SetNm.Contains(getEquipItemStateModel.SearchName)).ToList();
        }

        var total = equipSetParms.Count;

        equipSetParms = equipSetParms.Skip((pageNo.Value - 1) * pageSize.Value)
            .Take(pageSize.Value).ToList();

        return OperateResult.Successed(PageResult<EquipSetParmQuery>.Create(total, equipSetParms));
    }

    public async Task<OperateResult<PageResult<GetSetParmNewByEquipNoModel>>> NewGetSetParmByEquipNo(GetEquipItemStateModel getEquipItemStateModel)
    {
        if (getEquipItemStateModel == null)
        {
            return OperateResult.Failed<PageResult<GetSetParmNewByEquipNoModel>>("请求参数为空，请检查");
        }

        if (!await _context.SetParm.AnyAsync(x => x.EquipNo == getEquipItemStateModel.EquipNo))
        {
            return OperateResult.Successed<PageResult<GetSetParmNewByEquipNoModel>>(default);
        }

        var equipNo = getEquipItemStateModel.EquipNo;

        var setParm = _context.SetParm.AsNoTracking().Select(x => new GetSetParmNewByEquipNoModel
        {
            staN = x.StaN,
            equipNo = x.EquipNo,
            setNo = x.SetNo,
            setNm = x.SetNm,
            setType = x.SetType,
            mainInstruction = x.MainInstruction,
            value = x.Value
        }).Where(x => x.equipNo == equipNo);

        setParm = setParm.OrderBy(x => x.setNo);
        if (getEquipItemStateModel.SearchName.HasValue())
        {
            setParm = setParm.Where(x => x.setNm.Contains(getEquipItemStateModel.SearchName));
        }

        if (!_session.IsAdmin)
        {
            var condition = string.Empty; // 可控设备权限

            var currentRolePermissionInfos = _permissionCacheService.GetPermissionObj(_session.RoleName);
            var controlEquips = currentRolePermissionInfos?.ControlEquips ?? new List<int>();
            var controlEquipsUnit = currentRolePermissionInfos?.ControlEquipsUnitOfDict ?? new Dictionary<int, IEnumerable<int>>();
            var unionEquips = controlEquips.Union(controlEquipsUnit.Keys);
            if (unionEquips.All(d => d != equipNo))
            {
                return OperateResult.Successed((PageResult<GetSetParmNewByEquipNoModel>.Create(0)));
            }

            if (controlEquipsUnit.TryGetValue(equipNo, out var setList))
            {
                setParm = setParm.Where(x => setList.Contains(x.setNo));
            }

        }

        var count = await setParm.CountAsync();

        if (!getEquipItemStateModel.IsGetAll)
        {
            var pageNo = getEquipItemStateModel.PageNo.HasValue ? getEquipItemStateModel.PageNo.Value : 1;
            
            var pageSize = getEquipItemStateModel.PageSize.HasValue ? getEquipItemStateModel.PageSize.Value : 9999;

            setParm = setParm.Skip((pageNo - 1) * pageSize).Take(pageSize);
        }

        var res = await setParm.ToListAsync();

        return OperateResult.Successed(PageResult<GetSetParmNewByEquipNoModel>.Create(count, res));
    }

    public async Task<OperateResult<IEnumerable<SetParmByEquipNosResponse>>> GetSetParmByEquipNos(List<int> equipList)
    {
        List<SetParm> lists;

        var query = _context.SetParm.AsNoTracking().Where(d => equipList.Contains(d.EquipNo));
        var roleinfos = _permissionCacheService.GetPermissionObj(_session.RoleName);

        if (roleinfos == null && !_session.IsAdmin)
        {
            return OperateResult.Failed<IEnumerable<SetParmByEquipNosResponse>>("用户信息异常");
        }

        if (_session.IsAdmin)
        {
            var controlEquips = roleinfos?.ControlEquips ?? new List<int>();

            var ControlEquipsUnit = roleinfos?.ControlEquipsUnitOfDict ?? new Dictionary<int, IEnumerable<int>>();

            var equipsFilterList = controlEquips.Union(ControlEquipsUnit.Keys);

            query = query.Where(d => equipsFilterList.Contains(d.EquipNo));

            lists = await query.ToListAsync();

            lists = lists
            .Where(d => !(ControlEquipsUnit.TryGetValue(d.EquipNo, out var sets) && !sets.Contains(d.SetNo))).ToList();
        }
        else
        {
            lists = await query.ToListAsync();
        }

        var result = lists.GroupBy(d => d.EquipNo).Select(d => new SetParmByEquipNosResponse()
        {
            equipNo = d.Key,
            SetParmList = d.Select(x => x.MapTo<SetParmModel>())
        });

        return OperateResult.Successed<IEnumerable<SetParmByEquipNosResponse>>(result);
    }

    public OperateResult<PagedResult<string>> GetEquipSetParmList(CommonSearchPageModel commonSearchPageModel)
    {
        if (commonSearchPageModel == null)
        {
            return OperateResult.Failed<PagedResult<string>>("请求参数为空");
        }

        var equipSetParms = (from a in _context.SetParm.AsNoTracking()
                             join b in _context.Equip.AsNoTracking() on a.EquipNo equals b.EquipNo
                             select new EquipSetParmQuery
                             {
                                 EquipNo = a.EquipNo,
                                 SetNo = a.SetNo,
                                 EquipNm = b.EquipNm,
                                 StaN = a.StaN,
                                 SetNm = a.SetNm,
                                 SetType = a.SetType,
                                 Value = a.Value
                             }).ToList();

        var searchName = commonSearchPageModel.SearchName;

        if (!_session.IsAdmin)
        {
            var setParms = (_context.SetParm.AsNoTracking().ApplyRoleEquipFilter(_permissionCacheService.GetPermissionObj(_session.RoleName))).ToList();

            equipSetParms = equipSetParms
                .Where(d => setParms.Any(s => s.EquipNo == d.EquipNo && s.SetNo == d.SetNo)).ToList();
        }

        if (!string.IsNullOrEmpty(searchName))
        {
            equipSetParms = equipSetParms.Where(x => x.SetNm.Contains(searchName,
                StringComparison.OrdinalIgnoreCase) || x.EquipNm.Contains(searchName,
                StringComparison.OrdinalIgnoreCase)).ToList();
        }

        var pageNo = commonSearchPageModel.PageNo;
        var pageSize = commonSearchPageModel.PageSize;

        if (!pageNo.HasValue)
        {
            pageNo = 1;
        }

        if (!pageSize.HasValue)
        {
            pageSize = 9999;
        }

        var total = equipSetParms.Count;

        equipSetParms = equipSetParms.Skip((pageNo.Value - 1) * pageSize.Value)
            .Take(pageSize.Value).ToList();

        return OperateResult.Successed(PagedResult<string>.Create(total, equipSetParms.ToJson()));
    }

    public OperateResult<PagedResult<EquipSetParmResonse>> GetEquipSetParmTreeList(CommonSearchPageModel commonSearchPageModel)
    {
        if (commonSearchPageModel == null)
        {
            return OperateResult.Failed<PagedResult<EquipSetParmResonse>>("请求参数为空");
        }

        var equipSetParms = (from a in _context.SetParm.AsNoTracking()
                             join b in _context.Equip.AsNoTracking() on a.EquipNo equals b.EquipNo
                             select new EquipSetParmQuery
                             {
                                 EquipNo = a.EquipNo,
                                 SetNo = a.SetNo,
                                 EquipNm = b.EquipNm,
                                 StaN = a.StaN,
                                 SetNm = a.SetNm,
                                 SetType = a.SetType,
                                 Value = a.Value
                             }).ToList();

        var searchName = commonSearchPageModel.SearchName;

        if (!_session.IsAdmin)
        {
            var setParms = (_context.SetParm.AsNoTracking().ApplyRoleEquipFilter(_permissionCacheService.GetPermissionObj(_session.RoleName))).ToList();

            equipSetParms = equipSetParms
                .Where(d => setParms.Any(s => s.EquipNo == d.EquipNo && s.SetNo == d.SetNo)).ToList();
        }

        if (!string.IsNullOrEmpty(searchName))
        {
            equipSetParms = equipSetParms.Where(x => x.SetNm.Contains(searchName,
                StringComparison.OrdinalIgnoreCase) || x.EquipNm.Contains(searchName,
                StringComparison.OrdinalIgnoreCase)).ToList();
        }

        var pageNo = commonSearchPageModel.PageNo;
        var pageSize = commonSearchPageModel.PageSize;

        if (!pageNo.HasValue)
        {
            pageNo = 1;
        }

        if (!pageSize.HasValue)
        {
            pageSize = 9999;
        }

        var response = equipSetParms.GroupBy(d => d.EquipNm).Select(d => new EquipSetParmResonse
        {
            StaN = d.FirstOrDefault().StaN,
            EquipNo = d.FirstOrDefault().EquipNo,
            EquipNm = d.FirstOrDefault().EquipNm,
            SetInfos = d.Select(e => new SetList()
            {
                SetNo = e.SetNo,
                SetNm = e.SetNm,
                SetType = e.SetType,
                Value = e.Value
            }).ToList()
        }).ToList();

        var total = response.Count;

        response = response.Skip((pageNo.Value - 1) * pageSize.Value)
            .Take(pageSize.Value).ToList();

        return OperateResult.Successed(PagedResult<EquipSetParmResonse>.Create(total, response));
    }

    public async Task<OperateResult<PagedResult<RealEquipSetParmListModel>>> GetRealEquipSetParmList(CommonSearchPageModel commonSearchPageModel)
    {
        if (commonSearchPageModel == null)
        {
            return OperateResult.Failed<PagedResult<RealEquipSetParmListModel>>("请求参数为空");
        }

        var searchName = commonSearchPageModel.SearchName;

        var query = from a in _context.SetParm
                    from b in _context.Equip
                    where a.EquipNo == b.EquipNo
                    select new RealEquipSetParmListModel
                    {
                        StaN = a.StaN,
                        EquipNo = a.EquipNo,
                        EquipNm = b.EquipNm,
                        SetNo = a.SetNo,
                        SetNm = a.SetNm,
                        SetType = a.SetType
                    };

        if (!string.IsNullOrEmpty(searchName))
        {
            query = query.Where(x => x.SetNm.Contains(searchName,
                StringComparison.OrdinalIgnoreCase) || x.EquipNm.Contains(searchName,
                StringComparison.OrdinalIgnoreCase));
        }

        var result = await query.ToListAsync();
        var count = result.Count;

        if (commonSearchPageModel.PageNo != null && commonSearchPageModel.PageSize != null)
        {
            result = result.Skip(((commonSearchPageModel.PageNo - 1).Value * commonSearchPageModel.PageSize.Value))
            .Take(commonSearchPageModel.PageSize.Value).ToList();

        }
        return OperateResult.Successed(PagedResult<RealEquipSetParmListModel>.Create(count, result.ToJson()));
    }

    public async Task<OperateResult<byte[]>> ExportAbnormalRecord(int deviceStatus)
    {
        if (deviceStatus == -1 || deviceStatus > 2)
        {
            return OperateResult.Failed<byte[]>("目前仅开放【正常、离线、告警】三种状态下载");
        }

        if (!Enum.IsDefined(typeof(GrpcEquipState), deviceStatus))
        {
            return OperateResult.Failed<byte[]>("请求参数异常");
        }

        var GrpcEquipState = (GrpcEquipState)deviceStatus;

        var keyValues = _equipBaseAppService.GetEquipStateDict();
        if (keyValues == null || keyValues.Count <= 0)
        {
            return OperateResult.Successed(Array.Empty<byte>());
        }

        var equipNos = keyValues.Where(p => p.Value == GrpcEquipState).Select(k => k.Key);
        if (!_session.IsAdmin)
        {
            var currentRolePermissionInfos = _permissionCacheService.GetPermissionObj(_session.RoleName);
            var browseEquips = currentRolePermissionInfos?.BrowseEquips ?? new List<int>();

            if (!browseEquips.Any())
            {
                return OperateResult.Successed(Array.Empty<byte>());
            }

            equipNos = equipNos.Where(d => browseEquips.Contains(d));
        }

        IWorkbook workbook = new XSSFWorkbook();
        var result = await this.GenerateAbnormalEquipXlsx(equipNos, workbook);
        if (result)
        {
            await this.GenerateAbnormalEquipYcpXlsx(equipNos, workbook);

            await this.GenerateAbnormalEquipYxpXlsx(equipNos, workbook);
        }

        using var stream = new MemoryStream();

        workbook.Write(stream);
        await _apiLog.Audit(new AuditAction()
        {
            ResourceName = "设备列表接口",
            EventType = "导出异常设备事件",
            Result = new AuditResult { Default = "导出成功" }
        });

        return OperateResult.Successed(stream.ToArray());
    }

    #region 生成导出xlsx模型

    private async Task<bool> GenerateAbnormalEquipXlsx(IEnumerable<int> equipNos, IWorkbook workbook)
    {
        var devices = await _context.Equip.Where(p => equipNos.Any(q => q == p.EquipNo)).ToArrayAsync();
        if (devices == null || !devices.Any())
        {
            return false;
        }

        var egroupList = await _context.EGroup.OrderBy(m => m.GroupId).ToArrayAsync();

        var equipGroupList = await _context.EGroupList.ToArrayAsync();

        var exportModels = devices.Select(p => p.MapTo<AbnormalDeviceExportModel>()).ToList();

        foreach (var item in exportModels)
        {
            var equipGroup = equipGroupList.FirstOrDefault(m => m.EquipNo == item.EquipNo);
            if (equipGroup == null)
            {
                item.GroupName = egroupList.FirstOrDefault()?.GroupName;
            }
            var groupName = egroupList.FirstOrDefault(m => m.GroupId == equipGroup.GroupId)?.GroupName;
            item.GroupName = groupName;

            if (string.IsNullOrEmpty(item.GroupName))
            {
                item.GroupName = egroupList.FirstOrDefault()?.GroupName;
            }
        }
        var deviceExportNames = CreateAbnormalDeviceExportModelList();
        _exportManager.ExportToXlsx(workbook, deviceExportNames.ToArray(), exportModels, "equip");
        return true;
    }

    private static PropertyByName<AbnormalDeviceExportModel>[] CreateAbnormalDeviceExportModelList()
    {
        return new PropertyByName<AbnormalDeviceExportModel>[]
        {
            new PropertyByName<AbnormalDeviceExportModel>("站点号", d => d.StaN),
            new PropertyByName<AbnormalDeviceExportModel>("设备分组", d => d.GroupName),
            new PropertyByName<AbnormalDeviceExportModel>("设备号", d => d.EquipNo),
            new PropertyByName<AbnormalDeviceExportModel>("设备名称", d => d.EquipNm),
            new PropertyByName<AbnormalDeviceExportModel>("设备状态", d => d.EquipStatus),
            new PropertyByName<AbnormalDeviceExportModel>("设备属性", d => d.EquipDetail),
            new PropertyByName<AbnormalDeviceExportModel>("设备地址", d => d.EquipAddr),
            new PropertyByName<AbnormalDeviceExportModel>("产品Id", d => d.RawEquipNo),
            new PropertyByName<AbnormalDeviceExportModel>("关联页面", d => d.RelatedPic),
            new PropertyByName<AbnormalDeviceExportModel>("关联视频", d => d.RelatedVideo),
            new PropertyByName<AbnormalDeviceExportModel>("通讯故障处理意见", d => d.ProcAdvice),
            new PropertyByName<AbnormalDeviceExportModel>("故障信息", d => d.OutOfContact),
            new PropertyByName<AbnormalDeviceExportModel>("故障恢复提示", d => d.Contacted),
            new PropertyByName<AbnormalDeviceExportModel>("报警声音文件", d => d.EventWav),
            new PropertyByName<AbnormalDeviceExportModel>("报警方式", d => d.AlarmScheme),
            new PropertyByName<AbnormalDeviceExportModel>("驱动文件", d => d.CommunicationDrv),
            new PropertyByName<AbnormalDeviceExportModel>("通讯端口", d => d.LocalAddr),
            new PropertyByName<AbnormalDeviceExportModel>("通讯刷新周期", d => d.AccCyc),
            new PropertyByName<AbnormalDeviceExportModel>("通讯参数", d => d.CommunicationParam),
            new PropertyByName<AbnormalDeviceExportModel>("通讯时间参数", d => d.CommunicationTimeParam),
            new PropertyByName<AbnormalDeviceExportModel>("报警升级周期（分钟）", d => d.AlarmRiseCycle),
            new PropertyByName<AbnormalDeviceExportModel>("资产编号", d => d.ZiChanID),
            new PropertyByName<AbnormalDeviceExportModel>("预案号", d => d.PlanNo),
            new PropertyByName<AbnormalDeviceExportModel>("安全时段", d => d.SafeTime)

        };
    }

    private async Task GenerateAbnormalEquipYcpXlsx(IEnumerable<int> equipNos, IWorkbook workbook)
    {
        var ycps = await _context.Ycp.Where(p => equipNos.Any(q => q == p.EquipNo)).ToArrayAsync();
        if (ycps == null || !ycps.Any())
        {
            return;
        }

        var exportModels = ycps.Select(p => p.MapTo<AbnormalDeviceYcpExportModel>());
        PropertyByName<AbnormalDeviceYcpExportModel>[] deviceycpExportNames = CreateAbnormalDeviceYcpExportModelList();

        _exportManager.ExportToXlsx(workbook, deviceycpExportNames.ToArray(), exportModels, "ycp");
    }

    private static PropertyByName<AbnormalDeviceYcpExportModel>[] CreateAbnormalDeviceYcpExportModelList()
    {
        return new PropertyByName<AbnormalDeviceYcpExportModel>[]
        {
             new PropertyByName<AbnormalDeviceYcpExportModel>("站点号", d => d.StaNo),
             new PropertyByName<AbnormalDeviceYcpExportModel>("设备号", d => d.EquipNo),
             new PropertyByName<AbnormalDeviceYcpExportModel>("模拟量号", d => d.YcNo),
             new PropertyByName<AbnormalDeviceYcpExportModel>("模拟量名称", d => d.YcNm),
             new PropertyByName<AbnormalDeviceYcpExportModel>("下限值", d => d.ValMin),
             new PropertyByName<AbnormalDeviceYcpExportModel>("回复下限值", d => d.RestoreMin),
             new PropertyByName<AbnormalDeviceYcpExportModel>("回复上限值", d => d.RestoreMax),
             new PropertyByName<AbnormalDeviceYcpExportModel>("上限值", d => d.ValMax),
             new PropertyByName<AbnormalDeviceYcpExportModel>("资产编号", d => d.ZiChanId),
             new PropertyByName<AbnormalDeviceYcpExportModel>("预案号", d => d.PlanNo),
             new PropertyByName<AbnormalDeviceYcpExportModel>("曲线记录", d => d.CurveRcd),
             new PropertyByName<AbnormalDeviceYcpExportModel>("曲线记录阈值", d => d.CurveLimit),
             new PropertyByName<AbnormalDeviceYcpExportModel>("单位", d => d.Unit),
             new PropertyByName<AbnormalDeviceYcpExportModel>("越上限事件", d => d.OutmaxEvt),
             new PropertyByName<AbnormalDeviceYcpExportModel>("越下限事件", d => d.OutminEvt),
             new PropertyByName<AbnormalDeviceYcpExportModel>("报警级别", d => d.LvlLevel),
             new PropertyByName<AbnormalDeviceYcpExportModel>("比例变换", d => d.Mapping),
             new PropertyByName<AbnormalDeviceYcpExportModel>("最小值", d => d.YcMin),
             new PropertyByName<AbnormalDeviceYcpExportModel>("最大值", d => d.YcMax),
             new PropertyByName<AbnormalDeviceYcpExportModel>("操作命令", d => d.MainInstruction),
             new PropertyByName<AbnormalDeviceYcpExportModel>("操作参数", d => d.MinorInstruction),
             new PropertyByName<AbnormalDeviceYcpExportModel>("越线滞纳时间（秒）", d => d.AlarmAcceptableTime),
             new PropertyByName<AbnormalDeviceYcpExportModel>("恢复滞纳时间（秒）", d => d.RestoreAcceptableTime),
             new PropertyByName<AbnormalDeviceYcpExportModel>("重复报警时间（分钟）", d => d.AlarmRepeatTime),
             new PropertyByName<AbnormalDeviceYcpExportModel>("报警升级周期", d => d.AlarmRiseCycle),
             new PropertyByName<AbnormalDeviceYcpExportModel>("安全时段", d => d.SafeTime),
             new PropertyByName<AbnormalDeviceYcpExportModel>("报警方式", d => d.AlarmScheme),
             new PropertyByName<AbnormalDeviceYcpExportModel>("遥测编码", d => d.YC_Code)
         };
    }

    private async Task GenerateAbnormalEquipYxpXlsx(IEnumerable<int> equipNos, IWorkbook workbook)
    {
        var yxps = await _context.Yxp.Where(p => equipNos.Any(q => q == p.EquipNo)).ToArrayAsync();
        if (yxps == null || !yxps.Any())
        {
            return;
        }

        var exportModels = yxps.Select(p => p.MapTo<AbnormalDeviceYxpExportModel>());
        PropertyByName<AbnormalDeviceYxpExportModel>[] deviceYcpExportNames = CreateAbnormalDeviceYxpExportModelList();

        _exportManager.ExportToXlsx(workbook, deviceYcpExportNames.ToArray(), exportModels, "yxp");
    }

    private static PropertyByName<AbnormalDeviceYxpExportModel>[] CreateAbnormalDeviceYxpExportModelList()
    {
        return new PropertyByName<AbnormalDeviceYxpExportModel>[]
        {
             new PropertyByName<AbnormalDeviceYxpExportModel>("站点号", d => d.StaNo),
             new PropertyByName<AbnormalDeviceYxpExportModel>("设备号", d => d.EquipNo),
             new PropertyByName<AbnormalDeviceYxpExportModel>("状态量号", d => d.YxNo),
             new PropertyByName<AbnormalDeviceYxpExportModel>("状态量名称", d => d.YxNm),
             new PropertyByName<AbnormalDeviceYxpExportModel>("资产编号", d => d.ZiChanId),
             new PropertyByName<AbnormalDeviceYxpExportModel>("预案号", d => d.PlanNo),
             new PropertyByName<AbnormalDeviceYxpExportModel>("操作命令", d => d.MainInstruction),
             new PropertyByName<AbnormalDeviceYxpExportModel>("操作参数", d => d.MinorInstruction),
             new PropertyByName<AbnormalDeviceYxpExportModel>("越线滞纳时间（秒）", d => d.AlarmAcceptableTime),
             new PropertyByName<AbnormalDeviceYxpExportModel>("恢复滞纳时间（秒）", d => d.RestoreAcceptableTime),
             new PropertyByName<AbnormalDeviceYxpExportModel>("重复报警时间（分钟）", d => d.AlarmRepeatTime),
             new PropertyByName<AbnormalDeviceYxpExportModel>("报警升级周期", d => d.AlarmRiseCycle),
             new PropertyByName<AbnormalDeviceYxpExportModel>("安全时段", d => d.SafeTime),
             new PropertyByName<AbnormalDeviceYxpExportModel>("报警方式", d => d.AlarmScheme),
             new PropertyByName<AbnormalDeviceYxpExportModel>("遥信编码", d => d.YX_Code)
         };
    }
    private async Task GenerateAbnormalEquipSetParmXlsx(IEnumerable<int> equipNos, IWorkbook workbook)
    {
        var setParms = await _context.SetParm.Where(p => equipNos.Any(q => q == p.EquipNo)).ToArrayAsync();
        if (setParms == null || !setParms.Any())
        {
            return;
        }
        var exportModels = setParms.Select(p => p.MapTo<AbnormalDeviceSetParmExportModel>());
        var deviceSetParmExportNames = CreateAbnormalDeviceSetParmExportModelList();

        _exportManager.ExportToXlsx(workbook, deviceSetParmExportNames.ToArray(), exportModels, "setParm");
    }
    private static PropertyByName<AbnormalDeviceSetParmExportModel>[] CreateAbnormalDeviceSetParmExportModelList()
    {
        return new PropertyByName<AbnormalDeviceSetParmExportModel>[]
        {
              new PropertyByName<AbnormalDeviceSetParmExportModel>("站点号", d => d.StaNo),
              new PropertyByName<AbnormalDeviceSetParmExportModel>("设备号", d => d.EquipNo),
              new PropertyByName<AbnormalDeviceSetParmExportModel>("设置号", d => d.SetNo),
              new PropertyByName<AbnormalDeviceSetParmExportModel>("设置名称", d => d.SetNm),
              new PropertyByName<AbnormalDeviceSetParmExportModel>("设置类型", d => d.SetType),
              new PropertyByName<AbnormalDeviceSetParmExportModel>("操作命令", d => d.MainInstruction),
              new PropertyByName<AbnormalDeviceSetParmExportModel>("操作参数", d => d.MinorInstruction),
              new PropertyByName<AbnormalDeviceSetParmExportModel>("设置编码", d => d.Set_Code)
        };
    }

    #endregion

    public async Task<OperateResult<byte[]>> ExportEquipHistroyCurves(ExportHistoryCuresModelInternal model)
    {
        var deviceYcYxExportNames = new PropertyByName<YcYxHistoryCure>[]
        {
            new PropertyByName<YcYxHistoryCure>("时间", d => d.DateTime),
            new PropertyByName<YcYxHistoryCure>("实时值", d => d.Value),
        };

        using var stream = new MemoryStream();
        using var zipOutStream = new ZipOutputStream(stream);

        var equipYcCures = model.YcHistory;
        if (equipYcCures.Any())
        {
            var equipYcModels = equipYcCures.GroupBy(d => (d.EquipNo, d.EquipName));
            foreach (var equipYcModel in equipYcModels)
            {
                await PackageYcExcelToZip(
                    equipYcModel,
                    model,
                    zipOutStream,
                    deviceYcYxExportNames);
            }
        }

        var equipYxCures = model.YxHistory;
        if (equipYxCures.Any())
        {
            var equipYxModels = equipYxCures.GroupBy(d => (d.EquipNo, d.EquipName));
            foreach (var equipYxModel in equipYxModels)
            {
                await PackageYxExcelToZip(
                    equipYxModel,
                    model,
                    zipOutStream,
                    deviceYcYxExportNames);
            }
        }

        zipOutStream.Finish();
        return OperateResult.Successed(stream.ToArray());
    }


    private async Task PackageYcExcelToZip(IGrouping<(int EquipNo, string EquipName), YcHistoryCureEquipModel> equipModel,
        ExportHistoryCuresModelInternal model,
        ZipOutputStream zipOutStream,
        PropertyByName<YcYxHistoryCure>[] deviceYcYxExportNames)
    {
        IWorkbook workbook = new XSSFWorkbook();

        foreach (var equip in equipModel)
        {
            var curveDatas = await proxy.GetChangedDataFromCurveAsync(model.BeginTime, model.EndTime,
                model.StaNo, equip.EquipNo, equip.YcpNo, "C");

            if (!curveDatas.Any())
                continue;

            var curves = curveDatas.Select(d => new YcYxHistoryCure()
            {
                DateTime = d.datetime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                Value = d.value
            }).Take(980000);

            _exportManager.ExportToXlsx(workbook,
                deviceYcYxExportNames.ToArray(),
                curves,
                $"{equip.EquipNo}-{equip.YcpNo}-{equip.YcpName}");
        }

        if (workbook.NumberOfSheets > 0)
        {
            ZipEntry entry = new ZipEntry(equipModel.Key.EquipNo + "-" + equipModel.Key.EquipName + "-遥测" + ".xlsx");
            entry.IsUnicodeText = true;
            entry.DateTime = DateTime.Now;
            zipOutStream.PutNextEntry(entry);

            using ByteArrayOutputStream bos = new ByteArrayOutputStream();
            workbook.Write(bos);

            var buffer = bos.ToByteArray();
            await zipOutStream.WriteAsync(buffer.AsMemory(0, buffer.Length));
        }
    }


    private async Task PackageYxExcelToZip(IGrouping<(int EquipNo, string EquipName), YxHistoryCureEquipModel> equipYxModel,
        ExportHistoryCuresModelInternal model,
        ZipOutputStream zipOutStream,
        PropertyByName<YcYxHistoryCure>[] deviceYcYxExportNames)
    {
        IWorkbook workbook = new XSSFWorkbook();

        foreach (var equip in equipYxModel)
        {
            var curveDatas = await proxy.GetChangedDataFromCurveAsync(model.BeginTime, model.EndTime,
                model.StaNo, equip.EquipNo, equip.YxpNo, "X");

            if (!curveDatas.Any())
                continue;

            var curves = curveDatas.Select(d => new YcYxHistoryCure()
            {
                DateTime = d.datetime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                Value = d.value
            }).Take(980000);

            _exportManager.ExportToXlsx(workbook,
                deviceYcYxExportNames.ToArray(),
                curves,
                $"{equip.EquipNo}-{equip.YxpNo}-{equip.YxpName}");
        }

        if (workbook.NumberOfSheets > 0)
        {
            ZipEntry entry = new ZipEntry(equipYxModel.Key.EquipNo + "-" + equipYxModel.Key.EquipName + "-遥信" + ".xlsx");
            entry.IsUnicodeText = true;
            entry.DateTime = DateTime.Now;
            zipOutStream.PutNextEntry(entry);

            using ByteArrayOutputStream bos = new ByteArrayOutputStream();
            workbook.Write(bos);

            var buffer = bos.ToByteArray();
            await zipOutStream.WriteAsync(buffer.AsMemory(0, buffer.Length));
        }
    }

    public async Task<OperateResult<GetExportEquipYcYxpsRespones>> GetExportEquipYcYxps(GetExportEquipYcYxpsRequest request)
    {
        if (request.EquipNos == null)
        {
            return OperateResult.Failed<GetExportEquipYcYxpsRespones>("传入参数错误");
        }

        var query = _context.Equip.AsNoTracking();
        if (!_session.IsAdmin)
        {
            var browerEquips = _permissionCacheService.GetPermissionObj(_session.RoleName)?.BrowseEquips ?? new List<int>();
            query = query.Where(x => browerEquips.Contains(x.EquipNo));
        }

        var ycRespones = await (from e in query
                                join yc in _context.Ycp
                                on e.EquipNo equals yc.EquipNo
                                where request.EquipNos.Contains(e.EquipNo)
                                select new GetExportEquipYcpsRespones
                                {
                                    StaNo = e.StaN,
                                    EquipNo = e.EquipNo,
                                    YcNo = yc.YcNo,
                                    YcName = yc.YcNm
                                }).ToArrayAsync();


        var yxRespones = await (from e in query
                                join yx in _context.Yxp
                                on e.EquipNo equals yx.EquipNo
                                where request.EquipNos.Contains(e.EquipNo)
                                select new GetExportEquipYxpsRespones
                                {
                                    StaNo = e.StaN,
                                    EquipNo = e.EquipNo,
                                    YxNo = yx.YxNo,
                                    YxName = yx.YxNm
                                }).ToArrayAsync();



        return OperateResult.Successed(new GetExportEquipYcYxpsRespones
        {
            Ycps = ycRespones,
            Yxps = yxRespones
        });
    }


    public async Task<OperateResult<byte[]>> ExportAllEquipList()
    {
        var keyValues = _equipBaseAppService.GetEquipStateDict();
        if (keyValues == null || keyValues.Count <= 0)
        {
            return OperateResult.Successed(Array.Empty<byte>());
        }

        var equipNos = keyValues.Select(k => k.Key);
        if (!_session.IsAdmin)
        {
            var currentRolePermissionInfos = _permissionCacheService.GetPermissionObj(_session.RoleName);
            var browseEquips = currentRolePermissionInfos?.BrowseEquips ?? new List<int>();

            if (!browseEquips.Any())
            {
                return OperateResult.Successed(Array.Empty<byte>());
            }

            equipNos = equipNos.Where(d => browseEquips.Contains(d));
        }

        IWorkbook workbook = new XSSFWorkbook();
        var result = await this.GenerateAbnormalEquipXlsx(equipNos, workbook);
        if (result)
        {
            await this.GenerateAbnormalEquipYcpXlsx(equipNos, workbook);

            await this.GenerateAbnormalEquipYxpXlsx(equipNos, workbook);

            await this.GenerateAbnormalEquipSetParmXlsx(equipNos, workbook);
        }

        using var stream = new MemoryStream();

        workbook.Write(stream);
        await _apiLog.Audit(new AuditAction()
        {
            ResourceName = "导出设备接口",
            EventType = "导出设备",
            Result = new AuditResult { Default = "导出成功" }
        });

        return OperateResult.Successed(stream.ToArray());
    }

    public async Task<OperateResult<byte[]>> DownloadEquipTemplateFile()
    {
        IWorkbook workbook = new XSSFWorkbook();
        var equipSetNames = CreateAbnormalDeviceExportModelList();
        var ycpSetNames = CreateAbnormalDeviceYcpExportModelList();
        var yxpSetNames = CreateAbnormalDeviceYxpExportModelList();
        var setParmSetNames = CreateAbnormalDeviceSetParmExportModelList();

        _exportManager.ExportToXlsx(workbook, equipSetNames, new List<AbnormalDeviceExportModel>() { }, "equip");
        _exportManager.ExportToXlsx(workbook, ycpSetNames, new List<AbnormalDeviceYcpExportModel>() { }, "ycp");
        _exportManager.ExportToXlsx(workbook, yxpSetNames, new List<AbnormalDeviceYxpExportModel>() { }, "yxp");
        _exportManager.ExportToXlsx(workbook, setParmSetNames, new List<AbnormalDeviceSetParmExportModel>() { }, "setParm");
        using var stream = new MemoryStream();
        workbook.Write(stream);

        return OperateResult.Successed(stream.ToArray());
    }

    public async Task<OperateResult> ImportEquipList(IFormFile excelFile)
    {
        try
        {
            if (excelFile == null || excelFile.Length <= 0)
            {
                return OperateResult.Failed("请求参数不能为空");
            }
            var types = new[] { typeof(AbnormalDeviceExportModel), typeof(AbnormalDeviceYcpExportModel)
            , typeof(AbnormalDeviceYxpExportModel) , typeof(AbnormalDeviceSetParmExportModel) };
            var templates = _importManager.ImportFromXlsx(excelFile.FileName, excelFile.OpenReadStream(), types);
            if (templates.Count <= 0)
            {
                return OperateResult.Failed("导入终端设备数量为0，操作终止");
            }
            var equipTemplateList = templates[0].OfType<AbnormalDeviceExportModel>();
            var ycpTemplateList = templates[1].OfType<AbnormalDeviceYcpExportModel>();
            var yxpTemplateList = templates[2].OfType<AbnormalDeviceYxpExportModel>();
            var setTemplateList = templates[3].OfType<AbnormalDeviceSetParmExportModel>();

            await SaveEquipList(equipTemplateList);
            await SaveYcpList(ycpTemplateList);
            await SaveYxpList(yxpTemplateList);
            await SaveSetParmList(setTemplateList);

            _context.SaveChanges();

            await UpdateEquipGroup(equipTemplateList);

            return OperateResult.Successed("导入成功");
        }
        catch (Exception ex)
        {
            _apiLog.Error($"导入设备-失败，原因：{ex}");
            return OperateResult.Failed("导入失败");
        }
    }

    #region 导入设备操作
    private async Task SaveEquipList(IEnumerable<AbnormalDeviceExportModel> equipModels)
    {
        List<Equip> addEquipList = new List<Equip>();
        var oldEquips = await _context.Equip.ToListAsync();
        foreach (var equipModel in equipModels)
        {
            var oldEquip = oldEquips.FirstOrDefault(o => o.EquipNo == equipModel.EquipNo);
            if (oldEquip != null)
            {
                oldEquip.StaN = equipModel.StaN <= 0 ? 1 : equipModel.StaN;
                oldEquip.EquipNm = equipModel.EquipNm;
                oldEquip.EquipDetail = equipModel.EquipDetail;
                oldEquip.AccCyc = equipModel.AccCyc.HasValue ? equipModel.AccCyc.Value : 0;
                oldEquip.RelatedPic = equipModel.RelatedPic;
                oldEquip.ProcAdvice = equipModel.ProcAdvice;
                oldEquip.OutOfContact = equipModel.OutOfContact;
                oldEquip.Contacted = equipModel.Contacted;
                oldEquip.EventWav = equipModel.EventWav;
                oldEquip.CommunicationDrv = equipModel.CommunicationDrv;
                oldEquip.LocalAddr = equipModel.LocalAddr;
                oldEquip.EquipAddr = equipModel.EquipAddr;
                oldEquip.CommunicationParam = equipModel.CommunicationParam;
                oldEquip.CommunicationTimeParam = equipModel.CommunicationTimeParam;
                oldEquip.RawEquipNo = equipModel.RawEquipNo;
                oldEquip.AlarmScheme = equipModel.AlarmScheme;
                oldEquip.AlarmRiseCycle = equipModel.AlarmRiseCycle;
                oldEquip.RelatedVideo = equipModel.RelatedVideo;
                oldEquip.PlanNo = equipModel.PlanNo;
                oldEquip.SafeTime = equipModel.SafeTime;

                _context.Equip.Update(oldEquip);
            }
            else
                addEquipList.Add(equipModel.MapTo<Equip>());
        }
        if (addEquipList.Any())
            await _context.Equip.AddRangeAsync(addEquipList);

        var groupNames = equipModels.GroupBy(m => m.GroupName).Select(m => m.Key);
        var groups = await _context.EGroup.OrderBy(m => m.GroupId).ToListAsync();
        foreach (var groupName in groupNames)
        {
            if (!groups.Any(m => m.GroupName == groupName))
            {
                await _context.EGroup.AddAsync(new EGroup()
                {
                    GroupName = groupName,
                    ParentGroupId = groups.FirstOrDefault().GroupId
                });
            }
        }
    }
    private async Task SaveYcpList(IEnumerable<AbnormalDeviceYcpExportModel> ycpModels)
    {
        List<Ycp> addYcpList = new List<Ycp>();
        var oldYcps = await _context.Ycp.ToListAsync();
        foreach (var ycpModel in ycpModels)
        {
            var oldYcp = oldYcps.FirstOrDefault(o => o.EquipNo == ycpModel.EquipNo && o.YcNo == ycpModel.YcNo);
            if (oldYcp != null)
            {
                oldYcp.StaN = ycpModel.StaNo <= 0 ? 1 : ycpModel.StaNo;
                oldYcp.YcNm = ycpModel.YcNm;
                oldYcp.Mapping = ycpModel.Mapping == 1;
                oldYcp.YcMin = ycpModel.YcMin;
                oldYcp.YcMax = ycpModel.YcMax;
                oldYcp.ValMin = ycpModel.ValMin;
                oldYcp.RestoreMin = ycpModel.RestoreMin;
                oldYcp.RestoreMax = ycpModel.RestoreMax;
                oldYcp.ValMax = ycpModel.ValMax;
                oldYcp.MainInstruction = ycpModel.MainInstruction;
                oldYcp.MinorInstruction = ycpModel.MinorInstruction;
                oldYcp.AlarmAcceptableTime = ycpModel.AlarmAcceptableTime;
                oldYcp.RestoreAcceptableTime = ycpModel.RestoreAcceptableTime;
                oldYcp.AlarmRepeatTime = ycpModel.AlarmRepeatTime;
                oldYcp.LvlLevel = ycpModel.LvlLevel;
                oldYcp.OutminEvt = ycpModel.OutminEvt;
                oldYcp.OutmaxEvt = ycpModel.OutmaxEvt;
                oldYcp.AlarmScheme = ycpModel.AlarmScheme;
                oldYcp.CurveRcd = ycpModel.CurveRcd != 0;
                oldYcp.CurveLimit = ycpModel.CurveLimit;
                oldYcp.Unit = ycpModel.Unit;
                oldYcp.AlarmRiseCycle = ycpModel.AlarmRiseCycle;
                oldYcp.ZiChanId = ycpModel.ZiChanId;
                oldYcp.PlanNo = ycpModel.PlanNo;
                oldYcp.SafeTime = ycpModel.SafeTime;
                oldYcp.YcCode = ycpModel.YC_Code;
            }
            else
                addYcpList.Add(ycpModel.MapTo<Ycp>());
        }
        if (addYcpList.Any())
            await _context.Ycp.AddRangeAsync(addYcpList);
    }
    private async Task SaveYxpList(IEnumerable<AbnormalDeviceYxpExportModel> yxpModels)
    {
        List<Yxp> addYxpList = new List<Yxp>();
        var oldYxps = await _context.Yxp.ToListAsync();
        foreach (var yxpModel in yxpModels)
        {
            var oldYxp = oldYxps.FirstOrDefault(o => o.EquipNo == yxpModel.EquipNo && o.YxNo == yxpModel.YxNo);
            if (oldYxp != null)
            {
                oldYxp.StaN = yxpModel.StaNo <= 0 ? 1 : yxpModel.StaNo;
                oldYxp.YxNm = yxpModel.YxNm;
                oldYxp.MainInstruction = yxpModel.MainInstruction;
                oldYxp.MinorInstruction = yxpModel.MinorInstruction;
                oldYxp.AlarmAcceptableTime = yxpModel.AlarmAcceptableTime;
                oldYxp.RestoreAcceptableTime = yxpModel.RestoreAcceptableTime;
                oldYxp.AlarmRepeatTime = yxpModel.AlarmRepeatTime;
                oldYxp.AlarmScheme = yxpModel.AlarmScheme;
                oldYxp.AlarmRiseCycle = yxpModel.AlarmRiseCycle;
                oldYxp.ZiChanId = yxpModel.ZiChanId;
                oldYxp.PlanNo = yxpModel.PlanNo;
                oldYxp.SafeTime = yxpModel.SafeTime;
                oldYxp.YxCode = yxpModel.YX_Code;
            }
            else
            {
                addYxpList.Add(yxpModel.MapTo<Yxp>());
            }
        }

        if (addYxpList.Any())
        {
            await _context.Yxp.AddRangeAsync(addYxpList);
        }
    }
    private async Task SaveSetParmList(IEnumerable<AbnormalDeviceSetParmExportModel> setParmModels)
    {
        List<SetParm> addSetParmList = new List<SetParm>();
        foreach (var setParmModel in setParmModels)
        {
            var oldSetParm = await _context.SetParm.FirstOrDefaultAsync(o => o.EquipNo == setParmModel.EquipNo && o.SetNo == setParmModel.SetNo);
            if (oldSetParm != null)
            {
                oldSetParm.SetNm = setParmModel.SetNm;
                oldSetParm.SetType = setParmModel.SetType;
                oldSetParm.MainInstruction = setParmModel.MainInstruction;
                oldSetParm.MinorInstruction = setParmModel.MinorInstruction;
                oldSetParm.SetCode = setParmModel.Set_Code;
            }
            else
            {
                addSetParmList.Add(setParmModel.MapTo<SetParm>());
            }
        }

        if (addSetParmList.Any())
        {
            await _context.SetParm.AddRangeAsync(addSetParmList);
        }
    }

    private async Task UpdateEquipGroup(IEnumerable<AbnormalDeviceExportModel> equipModels)
    {
        var groups = await _context.EGroup.OrderBy(m => m.GroupId).ToListAsync();

        var equipGroupList = await _context.EGroupList.OrderBy(m => m.EquipNo).ToListAsync();

        List<EGroupList> addGroupLists = new List<EGroupList>(),
            updateGroupLists = new List<EGroupList>();

        foreach (var groupItem in equipModels.GroupBy(m => m.GroupName))
        {
            var groupName = groupItem.Key;
            var equipNos = groupItem.OrderBy(m => m.EquipNo).Select(m => m.EquipNo);

            var groupId = groups.FirstOrDefault(m => m.GroupName == groupName).GroupId;

            foreach (var equipNo in equipNos)
            {
                var existEquipGroup = equipGroupList.FirstOrDefault(m => m.EquipNo == equipNo);
                if (existEquipGroup == null)
                {
                    addGroupLists.Add(new EGroupList()
                    {
                        GroupId = groupId,
                        EquipNo = equipNo,
                        StaNo = 1
                    });
                }
                else
                {
                    if (existEquipGroup.GroupId != groupId)
                    {
                        existEquipGroup.GroupId = groupId;
                        updateGroupLists.Add(existEquipGroup);
                    }
                }
            }
        }

        if (addGroupLists.Count > 0)
        {
            await _context.EGroupList.AddRangeAsync(addGroupLists);
        }

        if (updateGroupLists.Count > 0)
        {
            _context.EGroupList.UpdateRange(updateGroupLists);
        }

        if (_context.ChangeTracker.HasChanges())
        {
            await _context.SaveChangesAsync();
        }
    }
    #endregion

    public OperateResult SetCommandWithRespone(CommandWithResponeModel commandModel)
    {
        if (commandModel == null)
        {
            return OperateResult.Failed<Page>("请求参数为空");
        }

        var equipNo = commandModel.EquipNo;
        var mainInstr = commandModel.MainInstr;
        var minoInstr = commandModel.MinoInstr;
        var value = commandModel.Value;
        var userName = commandModel.UserName;

        if (string.IsNullOrEmpty(userName))
        {
            return OperateResult.Failed<Page>("请求参数 userName 不能为空");
        }

        _apiLog.Info("SetCommandWithResponse【指令下发】:" + equipNo + ";" + mainInstr + ";" + minoInstr + ";" + value + ";" + userName);

        proxy.SetParmEx(equipNo,
            mainInstr,
            minoInstr,
            value,
            userName,
            commandModel.RquestId,
            (e) =>
            {
                _apiLog.Error($"SetCommandWithRespone【指令下发】: " + e);

                var iotSetParmModel = e.FromJson<IotSetParmModel>();

                Uri uri = null;
                if (iotSetParmModel.SetItemModel != null && !string.IsNullOrEmpty(commandModel.ResultNotifyUrl))
                {
                    uri = new Uri(commandModel.ResultNotifyUrl);
                }

                if (iotSetParmModel.SetResponseEventArgResp != null && !string.IsNullOrEmpty(commandModel.ResponeNotifyUrl))
                {
                    uri = new Uri(commandModel.ResponeNotifyUrl);
                }

                if (uri == null)
                {
                    _apiLog.Error($"SetCommandWithRespone【指令下发】: callback Uri is null");
                    return;
                }

                var client = _httpFactory.CreateClient("setNotify");
                using var content = new StringContent(e,
                    Encoding.UTF8, "application/json");

                var response = client.PostAsync(uri, content).ConfigureAwait(false)
                     .GetAwaiter()
                     .GetResult();

                if (response.IsSuccessStatusCode)
                {
                    var responseStr = response.Content.ReadAsStringAsync().Result;
                    _apiLog.Error("SetCommandWithResponse【指令下发】ReadAsString: " + responseStr);
                }
                else
                {
                    _apiLog.Error("SetCommandWithResponse【指令下发异步结果返回】Failed");
                }
            });

        return OperateResult.Success;
    }

    public OperateResult SyncCommand(CommandWithResponeModel commandModel)
    {

        if (commandModel == null)
        {
            return OperateResult.Failed<Page>("请求参数为空");
        }

        var equipNo = commandModel.EquipNo;
        string mainInstr = commandModel.MainInstr;
        string minoInstr = commandModel.MinoInstr;
        string value = commandModel.Value;
        string userName = commandModel.UserName;

        if (string.IsNullOrEmpty(userName))
        {
            return OperateResult.Failed<Page>("请求参数 userName 不能为空");
        }

        _apiLog.Info("SetCommandWithRespone【指令下发】:" + equipNo + ";" + mainInstr + ";" + minoInstr + ";" + value + ";" + userName);

        string requestId = Guid.NewGuid().ToString("N");

        var result = proxy.DoEquipSetItem(equipNo,
            commandModel.SetNo,
            commandModel.Value,
            "",
            false,
            "",
           requestId);
        return OperateResult.Successed(result);
    }

    public async Task<OperateResult> GetEquipListByPageNoState(GetEquipListModel equipListModel)
    {
        if (equipListModel == null)
        {
            return OperateResult.Failed("返回请求参数为空");
        }

        var setParms = _context.SetParm.AsNoTracking();
        if (!_session.IsAdmin)
        {
            var permissionObj = _permissionCacheService.GetPermissionObj(_session.RoleName);
            if (!permissionObj.BrowseEquips.Any())
            {
                return OperateResult.Successed(new Page()
                {
                    list = new
                    {
                        groupList = "",
                        equipList = new List<EquipListModel>()
                    }
                });
            }

            setParms = (_context.SetParm.AsNoTracking().ApplyRoleEquipFilter(permissionObj));
        }

        var browserEquips = setParms.Select(x => x.EquipNo).Distinct().ToList();

        var equipNoWithYxCount = await _context.Yxp.AsNoTracking()
            .GroupBy(x => x.EquipNo)
            .Select(g => new { EquipNo = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.EquipNo, x => x.Count);

        var equipNoWithYcCount = await _context.Ycp.AsNoTracking()
            .GroupBy(y => y.EquipNo)
            .Select(g => new { EquipNo = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.EquipNo, x => x.Count);

        var equipNoWithSetCount = await _context.SetParm.AsNoTracking()
            .GroupBy(s => s.EquipNo)
            .Select(g => new { EquipNo = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.EquipNo, x => x.Count);

        var equipName = equipListModel.EquipName;

        var deviceQuery = from i in _context.EGroupList.AsNoTracking()
                          join e in _context.Equip.AsNoTracking() on i.EquipNo equals e.EquipNo into ej
                          from e in ej.DefaultIfEmpty()
                          where e != null
                          select new
                          {
                              EquipID = i.EquipNo,
                              ParentID = i.GroupId,
                              Name = e.EquipNm ?? "",
                              Equip = e
                          };

        if (!string.IsNullOrWhiteSpace(equipName))
        {
            var pattern = $"%{equipName}%";
            deviceQuery = deviceQuery.Where(x => EF.Functions.Like(x.Name, pattern));
        }

        var equips = await deviceQuery.ToListAsync();

        var devices = equips.Where(e => browserEquips.Contains(e.EquipID))
            .Select(x => new EquipListModel
            {
                EquipID = x.EquipID,
                ParentID = x.ParentID,
                Name = x.Name,
                EquipState = "",
                yxNum = equipNoWithYxCount.GetValueOrDefault(x.EquipID, 0).ToString(),
                ycNum = equipNoWithYcCount.GetValueOrDefault(x.EquipID, 0).ToString(),
                setNum = equipNoWithSetCount.GetValueOrDefault(x.EquipID, 0).ToString()
            }).ToList();

        if (equipListModel.PageNo == null && equipListModel.PageSize == null)
        {
            equipListModel.PageNo = 1;
            equipListModel.PageSize = 9999;
        }

        var totalCount = devices.Count;

        var equipList = devices.Skip((equipListModel.PageNo - 1).Value * equipListModel.PageSize.Value)
             .Take(equipListModel.PageSize.Value).ToList();

        var groupQuery = from g in _context.EGroup.AsNoTracking()
                         select new EGroup
                         {
                             GroupId = g.GroupId,
                             GroupName = g.GroupName,
                             ParentGroupId = g.ParentGroupId
                         };

        if (!string.IsNullOrWhiteSpace(equipName))
        {
            var pattern = $"%{equipName}%";
            groupQuery = groupQuery.Where(x => EF.Functions.Like(x.GroupName, pattern));
        }

        var groups = await groupQuery.OrderBy(x => x.GroupId).ToListAsync();

        var page = new Page
        {
            pageNo = equipListModel.PageNo ?? 1,
            pageSize = equipListModel.PageSize ?? totalCount,
            totalCount = totalCount,
            list = new
            {
                groupList = groups.ToJson(),
                equipList = devices
            }
        };

        return OperateResult.Successed(page);
    }
}
