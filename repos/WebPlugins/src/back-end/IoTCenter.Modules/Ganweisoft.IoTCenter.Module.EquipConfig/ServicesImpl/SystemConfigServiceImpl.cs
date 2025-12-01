// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Data;
using IoTCenter.Data.Model;
using IoTCenter.Utilities;
using IoTCenterCore.AutoMapper;
using IoTCenterCore.ExcelHelper;
using IoTCenterHost.Core.Abstraction.BaseModels;
using IoTCenterHost.Core.Abstraction.EnumDefine;
using IoTCenterHost.Core.Abstraction.Services;
using IoTCenterWebApi.BaseCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.EquipConfig;

public class SystemConfigServiceImpl : ISystemConfigService
{
    private readonly Session _session;
    private readonly EquipBaseImpl _equipBaseImpl;
    private readonly ILoggingService _apiLog;
    private readonly IotCenterHostService _proxy;
    private readonly GWDbContext _context;
    private readonly IObjectMapping _mapper;
    private readonly IExportManager _exportManager;
    private readonly PermissionCacheService _permissionCacheService;

    public SystemConfigServiceImpl(
        Session session,
        IotCenterHostService alarmCenterService,
        EquipBaseImpl equipBaseImpl,
        ILoggingService apiLog,
        GWDbContext context,
        IObjectMapping mapper,
        IExportManager exportManager,
        PermissionCacheService permissionCacheService)
    {
        _session = session;
        _proxy = alarmCenterService;
        _equipBaseImpl = equipBaseImpl;
        _apiLog = apiLog;
        _context = context;
        _mapper = mapper;
        _exportManager = exportManager;
        _permissionCacheService = permissionCacheService;
    }

    public OperateResult<PageResult<GetEquipDataDataModel>> GetEquipDataList(GetEquipDataListModel getEquipDataListModel)
    {
        if (getEquipDataListModel == null)
        {
            return OperateResult.Successed(PageResult<GetEquipDataDataModel>.Create());
        }

        var currentRolePermissionInfos = _permissionCacheService.GetPermissionObj(_session.RoleName);
        var browseEquips = currentRolePermissionInfos?.BrowseEquips ?? new List<int>();

        if (!_session.IsAdmin && !browseEquips.Any())
        {
            return OperateResult.Successed(PageResult<GetEquipDataDataModel>.Create());
        }

        var equipName = getEquipDataListModel.EquipName;
        var equipNos = getEquipDataListModel.EquipNos;

        var query = (from a in _context.Equip.AsNoTracking()
                     where a.EquipNo > 0
                     select new GetEquipDataDataModel
                     {
                         staN = a.StaN,
                         equipNo = a.EquipNo,
                         equipNm = a.EquipNm,
                         equipDetail = a.EquipDetail,
                         accCyc = a.AccCyc,
                         procAdvice = a.ProcAdvice,
                         outOfContact = a.OutOfContact,
                         contacted = a.Contacted,
                         eventWav = a.EventWav,
                         communicationDrv = a.CommunicationDrv,
                         localAddr = a.LocalAddr,
                         equipAddr = a.EquipAddr,
                         communicationParam = a.CommunicationParam,
                         communicationTimeParam = a.CommunicationTimeParam,
                         rawEquipNo = a.RawEquipNo,
                         tabName = a.Tabname,
                         alarmScheme = a.AlarmScheme,
                         attrib = a.Attrib,
                         alarmRiseCycle = a.AlarmRiseCycle,
                         staIP = a.StaIp,
                         reserve1 = a.Reserve1,
                         reserve2 = a.Reserve2,
                         reserve3 = a.Reserve3,
                         relatedPic = a.RelatedPic,
                         relatedVideo = a.RelatedVideo,
                         ziChanID = a.ZiChanId,
                         planNo = a.PlanNo,
                         safeTime = a.SafeTime,
                         backup = a.Backup,
                         yxNum = a.Yxps.Count(),
                         ycNum = a.Ycps.Count(),
                         setNum = a.SetParms.Count()
                     }).ToList();

        if (equipNos.Any())
        {
            var equipNoList = equipNos.ToList();
            query = query.Where(a => equipNoList.Contains(a.equipNo)).ToList();
        }

        if (!_session.IsAdmin && browseEquips.Any())
        {
            query = query.Where(e => browseEquips.Contains(e.equipNo)).ToList();

            var dbSetParms = _context.SetParm.AsNoTracking()
                .ApplyRoleEquipFilter(currentRolePermissionInfos);

            query = (from a in query
                     join sp in dbSetParms on a.equipNo equals sp.EquipNo into g
                     select new GetEquipDataDataModel
                     {
                         staN = a.staN,
                         equipNo = a.equipNo,
                         equipNm = a.equipNm,
                         equipDetail = a.equipDetail,
                         accCyc = a.accCyc,
                         procAdvice = a.procAdvice,
                         outOfContact = a.outOfContact,
                         contacted = a.contacted,
                         eventWav = a.eventWav,
                         communicationDrv = a.communicationDrv,
                         localAddr = a.localAddr,
                         equipAddr = a.equipAddr,
                         communicationParam = a.communicationParam,
                         communicationTimeParam = a.communicationTimeParam,
                         rawEquipNo = a.rawEquipNo,
                         tabName = a.tabName,
                         alarmScheme = a.alarmScheme,
                         attrib = a.attrib,
                         alarmRiseCycle = a.alarmRiseCycle,
                         staIP = a.staIP,
                         reserve1 = a.reserve1,
                         reserve2 = a.reserve2,
                         reserve3 = a.reserve3,
                         relatedPic = a.relatedPic,
                         relatedVideo = a.relatedVideo,
                         ziChanID = a.ziChanID,
                         planNo = a.planNo,
                         safeTime = a.safeTime,
                         backup = a.backup,
                         yxNum = a.yxNum,
                         ycNum = a.ycNum,
                         setNum = g.Count()
                     }).ToList();
        }

        var total = query.Count;

        var result = query.Skip(((getEquipDataListModel.PageNo.HasValue ? getEquipDataListModel.PageNo.Value : 1) - 1) * (getEquipDataListModel.PageSize.HasValue ? getEquipDataListModel.PageSize.Value : 9999))
            .Take((getEquipDataListModel.PageSize.HasValue ? getEquipDataListModel.PageSize.Value : 999)).ToList();

        return OperateResult.Successed(PageResult<GetEquipDataDataModel>.Create(total, result));
    }

    public async Task<OperateResult<Equip>> GetEquipDataByEquipNo(GetEquipDataByNoModel getEquipDataByNoModel)
    {
        if (getEquipDataByNoModel == null)
        {
            return OperateResult.Failed<Equip>("请求参数为空");
        }

        var staNo = getEquipDataByNoModel.StaNo;
        var equipNo = getEquipDataByNoModel.EquipNo;

        var result = await _context.Equip.AsNoTracking()
            .FirstOrDefaultAsync(x => x.StaN == staNo && x.EquipNo == equipNo);

        return OperateResult.Successed(result);
    }

    public OperateResult<List<List<string>>> GetEquipColumnData()
    {
        var columnNames = GetColumnDatas<Equip>();
        return OperateResult.Successed(columnNames);
    }

    public async Task<OperateResult> AddEquipData(EquipDataModel equipDataModel, int groudId = 0)
    {
        if (equipDataModel == null)
        {
            return OperateResult.Failed("请求参数为空");
        }

        if (string.IsNullOrEmpty(equipDataModel.EquipNm))
        {
            return OperateResult.Failed("设备名称不能为空");
        }

        if (string.IsNullOrEmpty(equipDataModel.CommunicationDrv))
        {
            return OperateResult.Failed("驱动文件参数不能为空");
        }

        var dllPath = System.IO.Path.Combine(System.IO.Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName, "dll", equipDataModel.CommunicationDrv);

        if (!System.IO.File.Exists(dllPath))
        {
            return OperateResult.Failed("驱动文件不存在");
        }

        if (equipDataModel.AccCyc < 0)
        {
            return OperateResult.Failed("通讯刷新周期不能小于0");
        }

        if (string.IsNullOrEmpty(equipDataModel.CommunicationTimeParam))
        {
            return OperateResult.Failed("通讯时间参数不能为空");
        }

        if (equipDataModel.AlarmRiseCycle < 0)
        {
            return OperateResult.Failed("报警升级周期不能小于0");
        }

        if (!string.IsNullOrEmpty(equipDataModel.SafeTime?.Trim()))
        {
            var splitSafeTimes = equipDataModel.SafeTime?.Trim().Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
            if (splitSafeTimes.Length < 1 || splitSafeTimes.Length > 2)
            {
                return OperateResult.Failed("安全时段格式不正确");
            }

            foreach (var splitTime in splitSafeTimes)
            {
                var bgendTimes = splitTime.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                if (bgendTimes.Length != 2)
                {
                    return OperateResult.Failed("安全时段格式不正确");
                }

                if (!TimeSpan.TryParse(bgendTimes[1], out var endTimeSpan))
                {
                    return OperateResult.Failed("安全时段格式不正确");
                }

                if (!TimeSpan.TryParse(bgendTimes[0], out var beginTimeSpan))
                {
                    return OperateResult.Failed("安全时段格式不正确");
                }

                if (beginTimeSpan > endTimeSpan)
                {
                    return OperateResult.Failed("安全开始时间不能大于安全结束时间");
                }
            }
        }

        var equip = await _context.Equip.AddAsync(new Equip
        {
            StaN = equipDataModel.StaNo <= 0 ? 1 : equipDataModel.StaNo,
            EquipNm = equipDataModel.EquipNm,
            EquipDetail = equipDataModel.EquipDetail,
            AccCyc = equipDataModel.AccCyc,
            RelatedPic = equipDataModel.RelatedPic,
            ProcAdvice = equipDataModel.ProcAdvice,
            OutOfContact = equipDataModel.OutOfContact,
            Contacted = equipDataModel.Contacted,
            EventWav = equipDataModel.EventWav,
            CommunicationDrv = equipDataModel.CommunicationDrv,
            LocalAddr = equipDataModel.LocalAddr,
            EquipAddr = equipDataModel.EquipAddr,
            CommunicationParam = equipDataModel.CommunicationParam,
            CommunicationTimeParam = equipDataModel.CommunicationTimeParam,
            RawEquipNo = equipDataModel.RawEquipNo,
            Tabname = equipDataModel.Tabname,
            AlarmScheme = equipDataModel.AlarmScheme,
            Attrib = equipDataModel.Attrib,
            StaIp = equipDataModel.StaIp,
            AlarmRiseCycle = equipDataModel.AlarmRiseCycle,
            Reserve1 = equipDataModel.Reserve1,
            Reserve2 = equipDataModel.Reserve2,
            Reserve3 = equipDataModel.Reserve3,
            RelatedVideo = equipDataModel.RelatedVideo,
            ZiChanId = equipDataModel.ZiChanId,
            PlanNo = equipDataModel.PlanNo,
            SafeTime = equipDataModel.SafeTime,
            Backup = equipDataModel.Backup,
        });

        await _context.SaveChangesAsync();

        var insertEquipNo = equip.Entity.EquipNo;
        equipDataModel.EquipNo = equip.Entity.EquipNo;

        if (groudId != 0)
        {
            _context.EGroupList.Add(new EGroupList
            {
                GroupId = groudId,
                StaNo = equipDataModel.StaNo,
                EquipNo = insertEquipNo
            });
        }

        await _context.SaveChangesAsync();

        _equipBaseImpl.SysEvtLog(_session.UserName, "新增设备配置信息 - 设备号:" + insertEquipNo);

        _permissionCacheService.AddEquipPermission(_session.RoleName, insertEquipNo);

        await _apiLog.Audit(new AuditAction()
        {
            ResourceName = "设备管理",
            EventType = "新增设备",
            Result = new AuditResult<object, object>()
            {
                Default = "新增成功",
                New = equipDataModel
            }
        });

        try
        {
            _proxy.AddChangedEquip(new GrpcChangedEquip
            {
                iStaNo = equip.Entity.StaN,
                iEqpNo = insertEquipNo,
                State = ChangedEquipState.Add
            });
        }
        catch (Exception ex)
        {
            _apiLog.Error("AddChangedEquip - Add【设备重置接口异常】" + ex);
        }

        return OperateResult.Successed(new
        {
            equip.Entity.StaN,
            equip.Entity.EquipNo,
            equip.Entity.EquipNm
        });
    }

    public async Task<OperateResult> EditEquipData(EquipDataModel equipDataModel)
    {
        if (equipDataModel == null)
        {
            return OperateResult.Failed("请求参数为空");
        }

        if (equipDataModel.EquipNo < 0)
        {
            return OperateResult.Failed("设备号不能小于0");
        }

        if (string.IsNullOrEmpty(equipDataModel.EquipNm))
        {
            return OperateResult.Failed("设备名称不能为空");
        }

        if (string.IsNullOrEmpty(equipDataModel.CommunicationDrv))
        {
            return OperateResult.Failed("驱动文件参数不能为空");
        }

        var dllPath = System.IO.Path.Combine(System.IO.Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName, "dll", equipDataModel.CommunicationDrv);

        if (!System.IO.File.Exists(dllPath))
        {
            return OperateResult.Failed("驱动文件不存在");
        }

        if (equipDataModel.AccCyc < 0)
        {
            return OperateResult.Failed("通讯刷新周期不能小于0");
        }

        if (string.IsNullOrEmpty(equipDataModel.CommunicationTimeParam))
        {
            return OperateResult.Failed("通讯时间参数不能为空");
        }

        if (equipDataModel.AlarmRiseCycle < 0)
        {
            return OperateResult.Failed("报警升级周期不能小于0");
        }

        if (!string.IsNullOrEmpty(equipDataModel.SafeTime?.Trim()))
        {
            var splitSafeTimes = equipDataModel.SafeTime?.Trim().Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
            if (splitSafeTimes.Length < 1 || splitSafeTimes.Length > 2)
            {
                return OperateResult.Failed("安全时段格式不正确");
            }

            foreach (var splitTime in splitSafeTimes)
            {
                var bgendTimes = splitTime.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                if (bgendTimes.Length != 2)
                {
                    return OperateResult.Failed("安全时段格式不正确");
                }

                if (!TimeSpan.TryParse(bgendTimes[1], out var endTimeSpan))
                {
                    return OperateResult.Failed("安全时段格式不正确");
                }

                if (!TimeSpan.TryParse(bgendTimes[0], out var beginTimeSpan))
                {
                    return OperateResult.Failed("安全时段格式不正确");
                }

                if (beginTimeSpan > endTimeSpan)
                {
                    return OperateResult.Failed("安全开始时间不能大于安全结束时间");
                }
            }
        }

        var equip = await _context.Equip.FirstOrDefaultAsync(x =>
            x.StaN == equipDataModel.StaNo && x.EquipNo == equipDataModel.EquipNo);

        if (equip == null)
        {
            return OperateResult.Failed("设备配置信息不存在！");
        }

        equip.StaN = equipDataModel.StaNo <= 0 ? 1 : equipDataModel.StaNo;
        equip.EquipNm = equipDataModel.EquipNm;
        equip.EquipDetail = equipDataModel.EquipDetail;
        equip.AccCyc = equipDataModel.AccCyc;
        equip.RelatedPic = equipDataModel.RelatedPic;
        equip.ProcAdvice = equipDataModel.ProcAdvice;
        equip.OutOfContact = equipDataModel.OutOfContact;
        equip.Contacted = equipDataModel.Contacted;
        equip.EventWav = equipDataModel.EventWav;
        equip.CommunicationDrv = equipDataModel.CommunicationDrv;
        equip.LocalAddr = equipDataModel.LocalAddr;
        equip.EquipAddr = equipDataModel.EquipAddr;
        equip.CommunicationParam = equipDataModel.CommunicationParam;
        equip.CommunicationTimeParam = equipDataModel.CommunicationTimeParam;
        equip.RawEquipNo = equipDataModel.RawEquipNo;
        equip.Tabname = equipDataModel.Tabname;
        equip.AlarmScheme = equipDataModel.AlarmScheme;
        equip.Attrib = equipDataModel.Attrib;
        equip.StaIp = equipDataModel.StaIp;
        equip.AlarmRiseCycle = equipDataModel.AlarmRiseCycle;

        equip.Reserve2 = equipDataModel.Reserve2;
        equip.Reserve3 = equipDataModel.Reserve3;
        equip.RelatedVideo = equipDataModel.RelatedVideo;
        equip.ZiChanId = equipDataModel.ZiChanId;
        equip.PlanNo = equipDataModel.PlanNo;
        equip.SafeTime = equipDataModel.SafeTime;
        equip.Backup = equipDataModel.Backup;

        if (await _context.SaveChangesAsync() < 0)
        {
            return OperateResult.Failed("编辑设备配置信息失败！");
        }

        _equipBaseImpl.SysEvtLog(_session.UserName, "编辑设备 - 设备号:" + equipDataModel.EquipNo);

        await _apiLog.Audit(new AuditAction(_session.UserName)
        {
            ResourceName = "设备管理",
            EventType = "编辑设备",
            Result = new AuditResult<object, object>()
            {
                Old = equip,
                New = equipDataModel,
                Default = "编辑成功"
            }
        });

        ResetEquips(equipDataModel.EquipNo);

        return OperateResult.Successed(true);
    }

    public async Task<OperateResult> DelEquipData(DelEquipDataModel delEquipDataModel)
    {
        if (delEquipDataModel == null)
        {
            return OperateResult.Failed("请求参数为空");
        }

        var staNo = delEquipDataModel.StaNo;
        var equipNo = delEquipDataModel.EquipNo;

        var equip = await _context.Equip.FirstOrDefaultAsync(x =>
            x.EquipNo == delEquipDataModel.EquipNo && x.StaN == delEquipDataModel.StaNo);

        if (equip == null)
        {
            return OperateResult.Failed("该设备不存在！");
        }

        try
        {
            _proxy.AddChangedEquip(new GrpcChangedEquip
            {
                iStaNo = staNo,
                iEqpNo = equipNo,
                State = ChangedEquipState.Delete
            });
        }
        catch (Exception ex)
        {
            _apiLog.Error("AddChangedEquip - Delete【设备重置接口异常】" + ex);
        }

        _context.Equip.Remove(equip);

        var ycps = await _context.Ycp
            .Where(x => x.EquipNo == delEquipDataModel.EquipNo && x.StaN == delEquipDataModel.StaNo).ToArrayAsync();

        if (ycps != null)
        {
            _context.Ycp.RemoveRange(ycps);
        }

        var yxps = await _context.Yxp
            .Where(x => x.EquipNo == delEquipDataModel.EquipNo && x.StaN == delEquipDataModel.StaNo).ToArrayAsync();
        {
            _context.Yxp.RemoveRange(yxps);
        }

        var sets = await _context.SetParm
            .Where(x => x.EquipNo == delEquipDataModel.EquipNo && x.StaN == delEquipDataModel.StaNo).ToArrayAsync();

        if (sets != null)
        {
            _context.SetParm.RemoveRange(sets);
        }

        var eGroupLists = await _context.EGroupList.Where(x => x.EquipNo == delEquipDataModel.EquipNo)
            .ToArrayAsync();

        if (eGroupLists.Any())
        {
            _context.EGroupList.RemoveRange(eGroupLists);
        }

        if (await _context.SaveChangesAsync() <= 0)
        {
            return OperateResult.Failed("删除设备配置信息失败");
        }

        _equipBaseImpl.SysEvtLog(_session.UserName, "删除设备 - 设备号:" + equipNo);

        await _apiLog.Audit(new AuditAction(_session.UserName)
        {
            ResourceName = "设备管理",
            EventType = "删除设备",
            Result = new AuditResult
            {
                Default = $"删除设备名称({equip.EquipNm})成功"
            }
        });

        return OperateResult.Success;
    }

    public async Task<OperateResult<PagedResult<YcpResponesModel>>> GetYcpDataList(
        GetYcYxSetDataListModel getYcYxSetDataListModel)
    {
        if (getYcYxSetDataListModel == null)
        {
            return OperateResult.Failed<PagedResult<YcpResponesModel>>("参数为空");
        }

        var pageNo = getYcYxSetDataListModel.PageNo;
        var pageSize = getYcYxSetDataListModel.PageSize;
        var equipNo = getYcYxSetDataListModel.EquipNo;
        var ycName = getYcYxSetDataListModel.SearchName;

        var query = _context.Ycp.AsNoTracking().Where(d => d.EquipNo == equipNo);

        if (!string.IsNullOrEmpty(ycName))
        {
            query = query.Where(x => x.YcNm.Contains(ycName));
        }

        var qlist = await query.ToListAsync();

        var list = qlist.MapToList<YcpResponesModel>();

        var total = list.Count;

        if (pageSize > 0 && pageNo > 0)
        {
            list = list.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();
        }

        return OperateResult.Successed(PagedResult<YcpResponesModel>.Create(total, list));
    }

    public async Task<OperateResult<Ycp>> GetYcpDataByEquipYcNo(EquipYcYxSetNoModel equipYcYxSetNoModel)
    {
        return OperateResult.Successed(await _context.Ycp.AsNoTracking()
            .FirstOrDefaultAsync(x =>
            x.StaN == equipYcYxSetNoModel.StaNo &&
            x.EquipNo == equipYcYxSetNoModel.EquipNo &&
            x.YcNo == equipYcYxSetNoModel.YcyxSetpNo));
    }

    public OperateResult GetYcpColumnData()
    {
        var columnNames = GetColumnDatas<Ycp>();
        return OperateResult.Successed(columnNames);
    }

    public async Task<OperateResult> AddYcpData(YcpDataModel ycpDataModel)
    {
        if (ycpDataModel == null)
        {
            return OperateResult.Failed("请求参数为空");
        }

        if (string.IsNullOrEmpty(ycpDataModel.YcCode))
        {
            return OperateResult.Failed("遥测编码不能为空");
        }

        if (string.IsNullOrEmpty(ycpDataModel.YcNm))
        {
            return OperateResult.Failed("遥测名称不能为空");
        }

        if (string.IsNullOrEmpty(ycpDataModel.MainInstruction))
        {
            return OperateResult.Failed("操作命令不能为空");
        }

        if (string.IsNullOrEmpty(ycpDataModel.MinorInstruction))
        {
            return OperateResult.Failed("操作参数不能为空");
        }

        if (ycpDataModel.AlarmAcceptableTime < 0)
        {
            return OperateResult.Failed("越线滞纳时间不能小于0");
        }

        if (ycpDataModel.RestoreAcceptableTime < 0)
        {
            return OperateResult.Failed("恢复滞纳时间不能小于0");
        }

        if (ycpDataModel.RestoreMin > ycpDataModel.RestoreMax)
        {
            return OperateResult.Failed("恢复下限值不能大于等于恢复上限值");
        }

        if (ycpDataModel.ValMin > ycpDataModel.RestoreMin)
        {
            return OperateResult.Failed("下限值不能大于恢复下限值");
        }

        if (ycpDataModel.RestoreMax > ycpDataModel.ValMax)
        {
            return OperateResult.Failed("恢复上限值不能大于上限值");
        }

        if (ycpDataModel.YcMin < 0)
        {
            return OperateResult.Failed("最小值不能小于0");
        }

        if (ycpDataModel.YcMax < 0)
        {
            return OperateResult.Failed("最大值不能小于0");
        }

        if (ycpDataModel.YcMin > ycpDataModel.YcMax)
        {
            return OperateResult.Failed("最小值不能大于最大值");
        }

        if (ycpDataModel.LvlLevel > 9 || ycpDataModel.LvlLevel < 0)
        {
            return OperateResult.Failed("报警级别参数错误");
        }

        string[] ycDataType = new string[] { "S", "SEQ1", "SEQ2", "SEQ3", "SEQ4", "SEQ5", "SEQ6", "SEQ7" };

        if (ycpDataModel.DataType != null)
        {
            if (!ycDataType.Contains(ycpDataModel.DataType))
            {
                return OperateResult.Failed("数据类型参数错误");
            }
        }

        if (!string.IsNullOrEmpty(ycpDataModel.SafeTime?.Trim()))
        {
            var splitSafeTimes = ycpDataModel.SafeTime?.Trim().Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
            if (splitSafeTimes.Length < 1 || splitSafeTimes.Length > 2)
            {
                return OperateResult.Failed("安全时段格式不正确");
            }

            foreach (var splitTime in splitSafeTimes)
            {
                var bgendTimes = splitTime.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                if (bgendTimes.Length != 2)
                {
                    return OperateResult.Failed("安全时段格式不正确");
                }

                if (!TimeSpan.TryParse(bgendTimes[1], out var endTimeSpan))
                {
                    return OperateResult.Failed("安全时段格式不正确");
                }

                if (!TimeSpan.TryParse(bgendTimes[0], out var beginTimeSpan))
                {
                    return OperateResult.Failed("安全时段格式不正确");
                }

                if (beginTimeSpan > endTimeSpan)
                {
                    return OperateResult.Failed("安全开始时间不能大于安全结束时间");
                }
            }
        }

        if (ycpDataModel.PhysicMin < 0)
        {
            return OperateResult.Failed("实际最小值不能小于0");
        }

        if (ycpDataModel.PhysicMax < 0)
        {
            return OperateResult.Failed("实际最大值不能小于0");
        }

        if (ycpDataModel.PhysicMin > ycpDataModel.PhysicMax)
        {
            return OperateResult.Failed("实际最小值不能大于实际最大值");
        }

        if (await _context.Ycp.AsNoTracking()
            .AnyAsync(x => x.EquipNo == ycpDataModel.EquipNo && x.YcNo == ycpDataModel.YcNo))
        {
            return OperateResult.Failed("模拟量已存在");
        }

        if (ExistsCode(null, ycpDataModel.YcCode, "yc", ycpDataModel.EquipNo))
        {
            return OperateResult.Failed("遥测编码已经存在");
        }

        var ycs = _context.Ycp.AsNoTracking().Where(d => d.EquipNo == ycpDataModel.EquipNo).ToList();
        var ycNo = ycs.Any() ? ycs.Max(d => d.YcNo) + 1 : 1;

        var dbYcp = ycpDataModel.MapTo<Ycp>();

        await _context.Ycp.AddAsync(dbYcp);

        if (await _context.SaveChangesAsync() <= 0)
        {
            return OperateResult.Failed("新增模拟量配置信息失败");
        }

        ResetEquips(ycpDataModel.EquipNo);

        _equipBaseImpl.SysEvtLog(_session.UserName,
            "新增模拟量配置信息 - 设备号:" + ycpDataModel.EquipNo + ";遥测号:" + ycpDataModel.YcNo);

        await _apiLog.Audit(new AuditAction()
        {
            ResourceName = "设备管理",
            EventType = "新增遥测",
            Result = new AuditResult<object, object>
            {
                Default = "新增成功",
                New = ycpDataModel
            }
        });

        return OperateResult.Success;
    }

    public async Task<OperateResult> EditYcpData(YcpDataModel ycpDataModel)
    {
        if (ycpDataModel == null)
        {
            return OperateResult.Failed("请求参数为空");
        }

        if (string.IsNullOrEmpty(ycpDataModel.YcCode))
        {
            return OperateResult.Failed("遥测编码不能为空");
        }

        if (string.IsNullOrEmpty(ycpDataModel.YcNm))
        {
            return OperateResult.Failed("遥测名称不能为空");
        }

        if (string.IsNullOrEmpty(ycpDataModel.MainInstruction))
        {
            return OperateResult.Failed("操作命令不能为空");
        }

        if (string.IsNullOrEmpty(ycpDataModel.MinorInstruction))
        {
            return OperateResult.Failed("操作参数不能为空");
        }

        if (ycpDataModel.AlarmAcceptableTime < 0)
        {
            return OperateResult.Failed("越线滞纳时间不能小于0");
        }

        if (ycpDataModel.RestoreAcceptableTime < 0)
        {
            return OperateResult.Failed("恢复滞纳时间不能小于0");
        }

        if (ycpDataModel.RestoreMin > ycpDataModel.RestoreMax)
        {
            return OperateResult.Failed("恢复下限值不能大于等于恢复上限值");
        }

        if (ycpDataModel.ValMin > ycpDataModel.RestoreMin)
        {
            return OperateResult.Failed("下限值不能大于恢复下限值");
        }

        if (ycpDataModel.RestoreMax > ycpDataModel.ValMax)
        {
            return OperateResult.Failed("恢复上限值不能大于上限值");
        }

        if (ycpDataModel.YcMin > ycpDataModel.YcMax)
        {
            return OperateResult.Failed("最小值不能大于最大值");
        }

        if (ycpDataModel.LvlLevel > 9 || ycpDataModel.LvlLevel < 0)
        {
            return OperateResult.Failed("报警级别参数错误");
        }

        string[] ycDataType = new string[] { "S", "SEQ1", "SEQ2", "SEQ3", "SEQ4", "SEQ5", "SEQ6", "SEQ7" };

        if (!string.IsNullOrEmpty(ycpDataModel.DataType) && !ycDataType.Contains(ycpDataModel.DataType))
        {
            return OperateResult.Failed("数据类型参数错误");
        }

        if (!string.IsNullOrEmpty(ycpDataModel.SafeTime?.Trim()))
        {
            var splitSafeTimes = ycpDataModel.SafeTime?.Trim().Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
            if (splitSafeTimes.Length < 1 || splitSafeTimes.Length > 2)
            {
                return OperateResult.Failed("安全时段格式不正确");
            }

            foreach (var splitTime in splitSafeTimes)
            {
                var bgendTimes = splitTime.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                if (bgendTimes.Length != 2)
                {
                    return OperateResult.Failed("安全时段格式不正确");
                }

                if (!TimeSpan.TryParse(bgendTimes[1], out var endTimeSpan))
                {
                    return OperateResult.Failed("安全时段格式不正确");
                }

                if (!TimeSpan.TryParse(bgendTimes[0], out var beginTimeSpan))
                {
                    return OperateResult.Failed("安全时段格式不正确");
                }

                if (beginTimeSpan > endTimeSpan)
                {
                    return OperateResult.Failed("安全开始时间不能大于安全结束时间");
                }
            }
        }

        if (ycpDataModel.PhysicMin > ycpDataModel.PhysicMax)
        {
            return OperateResult.Failed("实际最小值不能大于实际最大值");
        }

        var ycp = await _context.Ycp.FirstOrDefaultAsync(x =>
            x.StaN == ycpDataModel.StaNo && x.EquipNo == ycpDataModel.EquipNo && x.YcNo == ycpDataModel.YcNo);

        if (ycp == null)
        {
            return OperateResult.Failed("模拟量不存在");
        }

        if (ExistsCode(ycpDataModel.YcNo, ycpDataModel.YcCode, "yc", ycpDataModel.EquipNo))
        {
            return OperateResult.Failed("遥测编码已经存在");
        }

        ycp.YcNm = ycpDataModel.YcNm;
        ycp.Mapping = ycpDataModel.Mapping;
        ycp.YcMin = ycpDataModel.YcMin;
        ycp.YcMax = ycpDataModel.YcMax;
        ycp.PhysicMin = ycpDataModel.PhysicMin;
        ycp.PhysicMax = ycpDataModel.PhysicMax;
        ycp.ValMin = ycpDataModel.ValMin;
        ycp.RestoreMin = ycpDataModel.RestoreMin;
        ycp.RestoreMax = ycpDataModel.RestoreMax;
        ycp.ValMax = ycpDataModel.ValMax;
        ycp.ValTrait = ycpDataModel.ValTrait;
        ycp.MainInstruction = ycpDataModel.MainInstruction;
        ycp.MinorInstruction = ycpDataModel.MinorInstruction;
        ycp.AlarmAcceptableTime = ycpDataModel.AlarmAcceptableTime;
        ycp.RestoreAcceptableTime = ycpDataModel.RestoreAcceptableTime;
        ycp.AlarmRepeatTime = ycpDataModel.AlarmRepeatTime;
        ycp.ProcAdvice = ycpDataModel.ProcAdvice;
        ycp.LvlLevel = ycpDataModel.LvlLevel;
        ycp.OutminEvt = ycpDataModel.OutminEvt;
        ycp.OutmaxEvt = ycpDataModel.OutmaxEvt;
        ycp.WaveFile = ycpDataModel.WaveFile;
        ycp.RelatedPic = ycpDataModel.RelatedPic;
        ycp.AlarmScheme = ycpDataModel.AlarmScheme;
        ycp.CurveRcd = ycpDataModel.CurveRcd;

        ycp.CurveLimit = ycpDataModel.CurveLimit;
        ycp.AlarmShield = ycpDataModel.AlarmShield;
        ycp.Unit = ycpDataModel.Unit;
        ycp.AlarmRiseCycle = ycpDataModel.AlarmRiseCycle;
        ycp.RelatedVideo = ycpDataModel.RelatedVideo;
        ycp.ZiChanId = ycpDataModel.ZiChanId;
        ycp.PlanNo = ycpDataModel.PlanNo;
        ycp.SafeTime = ycpDataModel.SafeTime;
        ycp.YcCode = ycpDataModel.YcCode;
        ycp.DataType = ycpDataModel.DataType;

        if (await _context.SaveChangesAsync() < 0)
        {
            return OperateResult.Failed("编辑模拟量配置信息失败！");
        }

        await _apiLog.Audit(new AuditAction(_session.UserName)
        {
            ResourceName = "设备管理",
            EventType = "编辑遥测",
            Result = new AuditResult<object, object>()
            {
                Old = ycp,
                New = ycpDataModel,
                Default = "编辑成功"
            }
        });

        _equipBaseImpl.SysEvtLog(_session.UserName,
            "编辑模拟量配置信息 - 设备号:" + ycpDataModel.EquipNo + ";遥测号:" + ycpDataModel.YcNo);

        ResetEquips(ycpDataModel.EquipNo);

        return OperateResult.Success;
    }

    public async Task<OperateResult> DelYcpData(DelYcYxSetDataModel delYcYxSetDataModel)
    {
        if (delYcYxSetDataModel == null)
        {
            return OperateResult.Failed("请求参数为空");
        }

        long staNo = delYcYxSetDataModel.StaNo;
        long equipNo = delYcYxSetDataModel.EquipNo;
        long ycNo = delYcYxSetDataModel.YcYxSetNo;

        var entity = await _context.Ycp.Where(x => x.StaN == staNo
                                                   && x.EquipNo == equipNo && x.YcNo == ycNo).ToListAsync();

        if (entity == null || entity.Count <= 0)
        {
            return OperateResult.Failed("模拟量不存在");
        }

        _context.RemoveRange(await _context.Ycp
            .Where(x => x.StaN == staNo && x.EquipNo == equipNo && x.YcNo == ycNo).ToListAsync());

        if (await _context.SaveChangesAsync() < 0)
        {
            return OperateResult.Failed("删除模拟量配置信息失败！");
        }

        _equipBaseImpl.SysEvtLog(_session.UserName, "删除模拟量配置信息 - 设备号:" + equipNo + ";遥测号:" + ycNo);

        await _apiLog.Audit(new AuditAction()
        {
            ResourceName = "设备管理",
            EventType = "删除遥测",
            Result = new AuditResult
            {
                Default = $"删除遥测({ycNo})成功，所属设备号：({equipNo})"
            }
        });

        ResetEquips(equipNo);

        return OperateResult.Success;
    }

    public OperateResult<IEnumerable<string>> GetCommunicationDrv()
    {
        try
        {
            var dllPath =
                     Path.Combine(new DirectoryInfo(AppContext.BaseDirectory).Parent.Parent.FullName, "dll");

            if (!Directory.Exists(dllPath))
            {
                return OperateResult.Successed(default(IEnumerable<string>));
            }

            var files = new DirectoryInfo(dllPath).GetFiles("*.dll", SearchOption.AllDirectories)
                .Where(x =>
                            x.Name.EndsWith(".STD.dll", StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Name)
                .AsEnumerable();

            return OperateResult.Successed(files);
        }
        catch (Exception ex)
        {
            _apiLog.Error("GetCommunicationDrv【获取驱动库文件集合】:" + ex);
            return OperateResult.Failed<IEnumerable<string>>("获取驱动库文件集合失败");
        }
    }

    public async Task<OperateResult<PagedResult<Yxp>>> GetYxpDataList(
        GetYcYxSetDataListModel getYcYxSetDataListModel)
    {
        if (getYcYxSetDataListModel == null)
        {
            return OperateResult.Failed<PagedResult<Yxp>>("请求参数为空");
        }

        var equipNo = getYcYxSetDataListModel.EquipNo;
        var yxName = getYcYxSetDataListModel.SearchName;

        var query = _context.Yxp.Where(d => d.EquipNo == equipNo);

        if (!string.IsNullOrEmpty(yxName))
        {
            query = query.Where(x => x.YxNm.Contains(yxName));
        }

        var result = await query.ToListAsync();

        var count = result.Count;

        if (getYcYxSetDataListModel.PageNo > 0 && getYcYxSetDataListModel.PageSize > 0)
        {
            result = result.Skip((getYcYxSetDataListModel.PageNo - 1) * getYcYxSetDataListModel.PageSize)
                .Take(getYcYxSetDataListModel.PageSize).ToList();
        }

        return OperateResult.Successed(PagedResult<Yxp>.Create(count, result));
    }

    public async Task<OperateResult<Yxp>> GetYxpDataByEquipYxNo(EquipYcYxSetNoModel equipYcYxSetNoModel)
    {
        if (equipYcYxSetNoModel == null)
        {
            return OperateResult.Failed<Yxp>("请求参数为空");
        }

        var retsult = await _context.Yxp.FirstOrDefaultAsync(x =>
            x.StaN == equipYcYxSetNoModel.StaNo && x.EquipNo == equipYcYxSetNoModel.EquipNo &&
            x.YxNo == equipYcYxSetNoModel.YcyxSetpNo);

        return OperateResult.Successed(retsult);
    }

    private List<List<string>> GetColumnDatas<T>() where T : class
    {
        var entityType = _context.Model.FindEntityType(typeof(T));

        var tableName = entityType.GetTableName().ToLower();

        var columnNameWithDescritions = entityType.GetProperties()
                     .Where(p => !p.IsShadowProperty())
                     .Select(p =>
                     {
                         var storeObjectId = StoreObjectIdentifier.Table(tableName);
                         var columnName = p.GetColumnName(storeObjectId).ToLower();
                         var clrProperty = typeof(T).GetProperty(p.Name);
                         var attribute = clrProperty.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
                         return new { description = attribute?.Description, columnName = columnName };
                     })
                     .ToList();

        var itemName1 = new List<string>();
        var itemName2 = new List<string>();
        foreach (var item in columnNameWithDescritions)
        {
            if (string.IsNullOrEmpty(item.description))
            {
                continue;
            }

            itemName1.Add(item.columnName);
            itemName2.Add(item.description);
        }
        var list = new List<List<string>>();
        list.Add(itemName1);
        list.Add(itemName2);

        return list;
    }

    public OperateResult<List<List<string>>> GetYxpColumnData()
    {
        var columnNames = GetColumnDatas<Yxp>();
        return OperateResult.Successed(columnNames);
    }

    public async Task<OperateResult> AddYxpData(YxpDataModel yxpDataModel)
    {
        if (yxpDataModel == null)
        {
            return OperateResult.Failed("请求参数为空");
        }

        if (string.IsNullOrEmpty(yxpDataModel.YxCode))
        {
            return OperateResult.Failed("遥信编码不能为空");
        }

        if (string.IsNullOrEmpty(yxpDataModel.YxNm))
        {
            return OperateResult.Failed("遥信名称不能为空");
        }

        if (string.IsNullOrEmpty(yxpDataModel.MainInstruction))
        {
            return OperateResult.Failed("操作命令不能为空");
        }

        if (string.IsNullOrEmpty(yxpDataModel.MinorInstruction))
        {
            return OperateResult.Failed("操作参数不能为空");
        }

        if (string.IsNullOrEmpty(yxpDataModel.Evt01))
        {
            return OperateResult.Failed("0-1事件不能为空");
        }

        if (string.IsNullOrEmpty(yxpDataModel.Evt10))
        {
            return OperateResult.Failed("1-0事件不能为空");
        }

        if (yxpDataModel.AlarmRiseCycle < 0)
        {
            return OperateResult.Failed("报警升级周期不能小于0");
        }

        if (yxpDataModel.LevelR > 9 || yxpDataModel.LevelR < 0)
        {
            return OperateResult.Failed("0-1级别参数不正确");
        }

        if (yxpDataModel.LevelD > 9 || yxpDataModel.LevelD < 0)
        {
            return OperateResult.Failed("1-0级别参数不正确");
        }

        if (yxpDataModel.AlarmRepeatTime < 0)
        {
            return OperateResult.Failed("重复报警时间不能小于0");
        }

        if (yxpDataModel.AlarmAcceptableTime < 0)
        {
            return OperateResult.Failed("越线滞纳时间不能小于0");
        }

        if (yxpDataModel.RestoreAcceptableTime < 0)
        {
            return OperateResult.Failed("恢复滞纳时间不能小于0");
        }

        if (!string.IsNullOrEmpty(yxpDataModel.AlarmShield))
        {
            var splitShields = yxpDataModel.AlarmShield.Split(new char[] { ',' });
            if (splitShields.Length <= 0)
            {
                return OperateResult.Failed("报警屏蔽格式不正确");
            }
        }

        if (!string.IsNullOrEmpty(yxpDataModel.SafeTime?.Trim()))
        {
            var splitSafeTimes = yxpDataModel.SafeTime?.Trim().Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
            if (splitSafeTimes.Length < 1 || splitSafeTimes.Length > 2)
            {
                return OperateResult.Failed("安全时段格式不正确");
            }

            foreach (var splitTime in splitSafeTimes)
            {
                var bgendTimes = splitTime.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                if (bgendTimes.Length != 2)
                {
                    return OperateResult.Failed("安全时段格式不正确");
                }

                if (!TimeSpan.TryParse(bgendTimes[1], out var endTimeSpan))
                {
                    return OperateResult.Failed("安全时段格式不正确");
                }

                if (!TimeSpan.TryParse(bgendTimes[0], out var beginTimeSpan))
                {
                    return OperateResult.Failed("安全时段格式不正确");
                }

                if (beginTimeSpan > endTimeSpan)
                {
                    return OperateResult.Failed("安全开始时间不能大于安全结束时间");
                }
            }
        }

        if (await _context.Yxp.AsNoTracking().AnyAsync(x =>
            x.StaN == yxpDataModel.StaNo && x.EquipNo == yxpDataModel.EquipNo && x.YxNo == yxpDataModel.YxNo))
        {
            return OperateResult.Failed("状态量已经存在");
        }

        if (ExistsCode(null, yxpDataModel.YxCode, "yx", yxpDataModel.EquipNo))
        {
            return OperateResult.Failed($"编码【{yxpDataModel.YxCode}】已经存在");
        }

        var yxs = await _context.Yxp.Where(d => d.EquipNo == yxpDataModel.EquipNo).ToListAsync();
        var yxNo = yxs.Any() ? yxs.Max(d => d.YxNo) + 1 : 1;


        if (yxpDataModel.DataType != null)
        {
            if (yxpDataModel.DataType != "S")
            {
                return OperateResult.Failed("数据类型错误");
            }
        }

        await _context.Yxp.AddAsync(new Yxp
        {
            StaN = yxpDataModel.StaNo,
            EquipNo = yxpDataModel.EquipNo,
            YxNo = yxNo,
            YxNm = yxpDataModel.YxNm,
            ProcAdviceR = yxpDataModel.ProcAdviceR,
            ProcAdviceD = yxpDataModel.ProcAdviceD,
            LevelR = yxpDataModel.LevelR,
            LevelD = yxpDataModel.LevelD,
            Evt01 = yxpDataModel.Evt01,
            Evt10 = yxpDataModel.Evt10,
            MainInstruction = yxpDataModel.MainInstruction,
            MinorInstruction = yxpDataModel.MinorInstruction,
            AlarmAcceptableTime = yxpDataModel.AlarmAcceptableTime,
            RestoreAcceptableTime = yxpDataModel.RestoreAcceptableTime,
            AlarmRepeatTime = yxpDataModel.AlarmRepeatTime,
            WaveFile = yxpDataModel.WaveFile,
            RelatedPic = yxpDataModel.RelatedPic,
            AlarmScheme = yxpDataModel.AlarmScheme,
            Inversion = yxpDataModel.Inversion,
            Initval = yxpDataModel.Initval,
            ValTrait = yxpDataModel.ValTrait,
            AlarmShield = yxpDataModel.AlarmShield,
            AlarmRiseCycle = yxpDataModel.AlarmRiseCycle,
            RelatedVideo = yxpDataModel.RelatedVideo,
            ZiChanId = yxpDataModel.ZiChanId,
            PlanNo = yxpDataModel.PlanNo,
            SafeTime = yxpDataModel.SafeTime,
            CurveRcd = yxpDataModel.CurveRcd,
            YxCode = yxpDataModel.YxCode,
            DataType = yxpDataModel.DataType
        });

        if (await _context.SaveChangesAsync() <= 0)
        {
            return OperateResult.Failed("新增状态量配置信息失败！");
        }

        _equipBaseImpl.SysEvtLog(_session.UserName,
            "新增状态量配置信息 - 设备号:" + yxpDataModel.EquipNo + ";遥信号:" + yxpDataModel.YxNo);

        await _apiLog.Audit(new AuditAction(_session.UserName)
        {
            ResourceName = "设备管理",
            EventType = "新增遥信",
            Result = new AuditResult<object, object>
            {
                Default = "新增成功",
                New = yxpDataModel
            }
        });

        ResetEquips(yxpDataModel.EquipNo);

        return OperateResult.Success;
    }

    public async Task<OperateResult> EditYxpData(YxpDataModel yxpDataModel)
    {
        if (yxpDataModel == null)
        {
            return OperateResult.Failed("请求参数为空");
        }

        if (string.IsNullOrEmpty(yxpDataModel.YxCode))
        {
            return OperateResult.Failed("遥信编码不能为空");
        }

        if (string.IsNullOrEmpty(yxpDataModel.YxNm))
        {
            return OperateResult.Failed("遥信名称不能为空");
        }

        if (string.IsNullOrEmpty(yxpDataModel.MainInstruction))
        {
            return OperateResult.Failed("操作命令不能为空");
        }

        if (string.IsNullOrEmpty(yxpDataModel.MinorInstruction))
        {
            return OperateResult.Failed("操作参数不能为空");
        }

        if (string.IsNullOrEmpty(yxpDataModel.Evt01))
        {
            return OperateResult.Failed("0-1事件不能为空");
        }

        if (string.IsNullOrEmpty(yxpDataModel.Evt10))
        {
            return OperateResult.Failed("1-0事件不能为空");
        }

        if (yxpDataModel.AlarmRiseCycle < 0)
        {
            return OperateResult.Failed("报警升级周期不能小于0");
        }

        if (yxpDataModel.LevelR > 9 || yxpDataModel.LevelR < 0)
        {
            return OperateResult.Failed("0-1级别参数不正确");
        }

        if (yxpDataModel.LevelD > 9 || yxpDataModel.LevelD < 0)
        {
            return OperateResult.Failed("1-0级别参数不正确");
        }

        if (yxpDataModel.AlarmRepeatTime < 0)
        {
            return OperateResult.Failed("重复报警时间不能小于0");
        }

        if (yxpDataModel.AlarmAcceptableTime < 0)
        {
            return OperateResult.Failed("越线滞纳时间不能小于0");
        }

        if (yxpDataModel.RestoreAcceptableTime < 0)
        {
            return OperateResult.Failed("恢复滞纳时间不能小于0");
        }

        if (!string.IsNullOrEmpty(yxpDataModel.AlarmShield))
        {
            var splitShields = yxpDataModel.AlarmShield.Split(new char[] { ',' });
            if (splitShields.Length <= 0)
            {
                return OperateResult.Failed("报警屏蔽格式不正确");
            }
        }

        if (!string.IsNullOrEmpty(yxpDataModel.SafeTime?.Trim()))
        {
            var splitSafeTimes = yxpDataModel.SafeTime?.Trim().Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
            if (splitSafeTimes.Length < 1 || splitSafeTimes.Length > 2)
            {
                return OperateResult.Failed("安全时段格式不正确");
            }

            foreach (var splitTime in splitSafeTimes)
            {
                var bgendTimes = splitTime.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                if (bgendTimes.Length != 2)
                {
                    return OperateResult.Failed("安全时段格式不正确");
                }

                if (!TimeSpan.TryParse(bgendTimes[1], out var endTimeSpan))
                {
                    return OperateResult.Failed("安全时段格式不正确");
                }

                if (!TimeSpan.TryParse(bgendTimes[0], out var beginTimeSpan))
                {
                    return OperateResult.Failed("安全时段格式不正确");
                }

                if (beginTimeSpan > endTimeSpan)
                {
                    return OperateResult.Failed("安全开始时间不能大于安全结束时间");
                }
            }
        }

        if (!string.IsNullOrEmpty(yxpDataModel.DataType) && yxpDataModel.DataType != "S")
        {
            return OperateResult.Failed("数据类型参数错误");
        }

        var yxp = await _context.Yxp.FirstOrDefaultAsync(x => x.StaN == yxpDataModel.StaNo
                                                              && x.EquipNo == yxpDataModel.EquipNo &&
                                                              x.YxNo == yxpDataModel.YxNo);

        var old = yxp;

        if (yxp == null)
        {
            return OperateResult.Failed("状态量不存在");
        }

        if (ExistsCode(yxpDataModel.YxNo, yxpDataModel.YxCode, "yx", yxpDataModel.EquipNo))
        {
            return OperateResult.Failed("Code已经存在");
        }

        yxp.YxNm = yxpDataModel.YxNm;
        yxp.ProcAdviceR = yxpDataModel.ProcAdviceR;
        yxp.ProcAdviceD = yxpDataModel.ProcAdviceD;
        yxp.LevelR = yxpDataModel.LevelR;
        yxp.LevelD = yxpDataModel.LevelD;
        yxp.Evt01 = yxpDataModel.Evt01;
        yxp.Evt10 = yxpDataModel.Evt10;
        yxp.MainInstruction = yxpDataModel.MainInstruction;
        yxp.MinorInstruction = yxpDataModel.MinorInstruction;
        yxp.AlarmAcceptableTime = yxpDataModel.AlarmAcceptableTime;
        yxp.RestoreAcceptableTime = yxpDataModel.RestoreAcceptableTime;
        yxp.AlarmRepeatTime = yxpDataModel.AlarmRepeatTime;
        yxp.WaveFile = yxpDataModel.WaveFile;
        yxp.RelatedPic = yxpDataModel.RelatedPic;
        yxp.AlarmScheme = yxpDataModel.AlarmScheme;
        yxp.Inversion = yxpDataModel.Inversion;
        yxp.Initval = yxpDataModel.Initval;
        yxp.ValTrait = yxpDataModel.ValTrait;
        yxp.AlarmShield = yxpDataModel.AlarmShield;
        yxp.AlarmRiseCycle = yxpDataModel.AlarmRiseCycle;
        yxp.RelatedVideo = yxpDataModel.RelatedVideo;
        yxp.ZiChanId = yxpDataModel.ZiChanId;
        yxp.PlanNo = yxpDataModel.PlanNo;
        yxp.SafeTime = yxpDataModel.SafeTime;
        yxp.CurveRcd = yxpDataModel.CurveRcd;
        yxp.YxCode = yxpDataModel.YxCode;
        yxp.DataType = yxpDataModel.DataType;

        _equipBaseImpl.SysEvtLog(_session.UserName, "编辑状态量配置信息 - 设备号:" + yxp.EquipNo + ";遥信号:" + yxp.YxNo);
        try
        {
            if (await _context.SaveChangesAsync() < 0)
            {
                return OperateResult.Failed("编辑状态量配置信息失败");
            }
        }
        catch (Exception e)
        {
            _apiLog.Error($"EditYxpData 编辑状态量配置信息失败: {e}");
            return OperateResult.Failed("编辑状态量配置信息失败");
        }

        await _apiLog.Audit(new AuditAction(_session.UserName)
        {
            ResourceName = "设备管理",
            EventType = "编辑遥信",
            Result = new AuditResult<object, object>()
            {
                Old = old,
                New = yxp,
                Default = "编辑成功"
            }
        });

        ResetEquips(yxp.EquipNo);

        return OperateResult.Success;
    }

    public async Task<OperateResult> DelYxpData(DelYcYxSetDataModel delYcYxSetDataModel)
    {
        if (delYcYxSetDataModel == null)
        {
            return OperateResult.Failed("请求参数为空");
        }

        var entity = await _context.Yxp
            .FirstOrDefaultAsync(x =>
                x.StaN == delYcYxSetDataModel.StaNo && x.EquipNo == delYcYxSetDataModel.EquipNo &&
                x.YxNo == delYcYxSetDataModel.YcYxSetNo);

        if (entity == null)
        {
            return OperateResult.Failed("状态量不存在");
        }

        _context.Remove(entity);

        if (await _context.SaveChangesAsync() <= 0)
        {
            return OperateResult.Failed("删除设置配置信息失败！");
        }

        _equipBaseImpl.SysEvtLog(_session.UserName,
            "删除状态量配置信息 - 设备号:" + delYcYxSetDataModel.EquipNo + ";遥信号:" + delYcYxSetDataModel.YcYxSetNo);

        await _apiLog.Audit(new AuditAction()
        {
            ResourceName = "设备管理",
            EventType = "删除遥信",
            Result = new AuditResult()
            {
                Default = $"删除遥信：({entity.YxNo})成功，所属设备号：({entity.EquipNo})"
            }
        });

        ResetEquips(delYcYxSetDataModel.EquipNo);

        return OperateResult.Success;
    }

    public async Task<OperateResult<PagedResult<SetParmResponesModel>>> GetSetParmDataList(
        GetYcYxSetDataListModel getYcYxSetDataListModel)
    {
        if (getYcYxSetDataListModel == null)
        {
            return OperateResult.Failed<PagedResult<SetParmResponesModel>>("请求参数为空");
        }

        var pageNo = getYcYxSetDataListModel.PageNo;
        var pageSize = getYcYxSetDataListModel.PageSize;
        var equipNo = getYcYxSetDataListModel.EquipNo;
        var setName = getYcYxSetDataListModel.SearchName;

        var query = _context.SetParm.Where(d => d.EquipNo == equipNo);

        if (!string.IsNullOrEmpty(setName))
        {
            query = query.Where(x => x.SetNm.Contains(setName));
        }

        var qlist = await query.ToListAsync();

        var list = qlist.MapToList<SetParmResponesModel>().ToList();

        var count = list.Count;
        var result = list.Skip((pageNo - 1) * pageSize).Take(pageSize);

        return OperateResult.Successed(PagedResult<SetParmResponesModel>.Create(count, result));
    }

    public async Task<OperateResult<SetParm>> GetSetParmDataByEquipSetNo(EquipYcYxSetNoModel equipYcYxSetNoModel)
    {
        return OperateResult.Successed(await _context.SetParm.FirstOrDefaultAsync(x =>
            x.StaN == equipYcYxSetNoModel.StaNo &&
            x.EquipNo == equipYcYxSetNoModel.EquipNo &&
            x.SetNo == equipYcYxSetNoModel.YcyxSetpNo
        ));
    }

    public OperateResult<List<List<string>>> GetSetParmColumnData()
    {
        var columnNames = GetColumnDatas<Equip>();
        return OperateResult.Successed(columnNames);
    }

    public async Task<OperateResult> AddSetData(SetDataModel setDataModel)
    {
        if (setDataModel == null)
        {
            return OperateResult.Failed("请求参数为空");
        }

        if (string.IsNullOrEmpty(setDataModel.Value))
        {
            return OperateResult.Failed("值不能为空");
        }

        if (string.IsNullOrEmpty(setDataModel.MainInstruction))
        {
            return OperateResult.Failed("操作命令不能为空");
        }

        if (string.IsNullOrEmpty(setDataModel.MinorInstruction))
        {
            return OperateResult.Failed("操作参数不能为空");
        }

        if (setDataModel.SetType.Length > 1)
        {
            return OperateResult.Failed("设置类型数据配置过长");
        }

        if (await _context.SetParm.AnyAsync(x => x.StaN == setDataModel.StaNo &&
                                                 x.EquipNo == setDataModel.EquipNo &&
                                                 x.SetNo == setDataModel.SetNo))
        {
            return OperateResult.Failed("配置已存在");
        }

        if (ExistsCode(null, setDataModel.SetCode, "set", setDataModel.EquipNo))
        {
            return OperateResult.Failed($"编码【{setDataModel.SetCode}】已经存在");
        }

        var sets = await _context.SetParm.Where(d => d.EquipNo == setDataModel.EquipNo).ToListAsync();
        var setNo = sets.Any() ? sets.Max(d => d.SetNo) + 1 : 1;

        var setModel = new SetParm
        {
            StaN = setDataModel.StaNo,
            EquipNo = setDataModel.EquipNo,
            SetNo = setNo,
            SetNm = setDataModel.SetNm,
            SetType = setDataModel.SetType,
            MainInstruction = setDataModel.MainInstruction,
            MinorInstruction = setDataModel.MinorInstruction,
            Record = setDataModel.Record,
            Action = setDataModel.Action,
            Value = setDataModel.Value,
            Canexecution = setDataModel.Canexecution,
            VoiceKeys = setDataModel.VoiceKeys,
            EnableVoice = setDataModel.EnableVoice,
            QrEquipNo = setDataModel.QrEquipNo,
            SetCode = setDataModel.SetCode
        };

        await _context.SetParm.AddAsync(setModel);

        if (await _context.SaveChangesAsync() <= 0)
        {
            return OperateResult.Failed("新增设置量配置信息失败");
        }

        _equipBaseImpl.SysEvtLog(_session.UserName,
            "新增设置量配置信息 - 设备号:" + setDataModel.EquipNo + ";设置号:" + setDataModel.SetNo);

        await _apiLog.Audit(new AuditAction(_session.UserName)
        {
            ResourceName = "设备管理",
            EventType = "新增设置",
            Result = new AuditResult<object, object>
            {
                Default = "新增成功",
                New = setDataModel
            }
        });

        ResetEquips(setDataModel.EquipNo);

        return OperateResult.Success;
    }

    public async Task<OperateResult> EditSetData(SetDataModel setDataModel)
    {
        if (setDataModel == null)
        {
            return OperateResult.Failed("请求参数为空");
        }

        if (string.IsNullOrEmpty(setDataModel.Value))
        {
            return OperateResult.Failed("值不能为空");
        }

        if (string.IsNullOrEmpty(setDataModel.MainInstruction))
        {
            return OperateResult.Failed("操作命令不能为空");
        }

        if (string.IsNullOrEmpty(setDataModel.MinorInstruction))
        {
            return OperateResult.Failed("操作参数不能为空");
        }

        if (setDataModel.SetType.Length > 1)
        {
            return OperateResult.Failed("设置类型数据配置过长");
        }

        if (ExistsCode(setDataModel.SetNo, setDataModel.SetCode, "set", setDataModel.EquipNo))
        {
            return OperateResult.Failed("Code已经存在");
        }

        var setParm = await _context.SetParm.FirstOrDefaultAsync(x =>
            x.StaN == setDataModel.StaNo &&
            x.EquipNo == setDataModel.EquipNo &&
            x.SetNo == setDataModel.SetNo);

        var old = setParm;

        if (setParm == null)
        {
            return OperateResult.Failed("配置不存在");
        }

        setParm.SetNm = setDataModel.SetNm;
        setParm.SetType = setDataModel.SetType;
        setParm.MainInstruction = setDataModel.MainInstruction;
        setParm.MinorInstruction = setDataModel.MinorInstruction;
        setParm.Record = setDataModel.Record;
        setParm.Action = setDataModel.Action;
        setParm.Value = setDataModel.Value;
        setParm.Canexecution = setDataModel.Canexecution;
        setParm.VoiceKeys = setDataModel.VoiceKeys;
        setParm.EnableVoice = setDataModel.EnableVoice;
        setParm.QrEquipNo = setDataModel.QrEquipNo;
        setParm.SetCode = setDataModel.SetCode;

        if (await _context.SaveChangesAsync() < 0)
        {
            return OperateResult.Failed("编辑设置配置信息失败！");
        }

        _equipBaseImpl.SysEvtLog(_session.UserName,
            "编辑设置量配置信息 - 设备号:" + setDataModel.EquipNo + ";设置号:" + setDataModel.SetNo);

        await _apiLog.Audit(new AuditAction()
        {
            ResourceName = "设置管理",
            EventType = "编辑设置",
            Result = new AuditResult<object, object>()
            {
                Old = old,
                New = setParm,
                Default = "编辑成功"
            }
        });

        ResetEquips(setDataModel.EquipNo);

        return OperateResult.Success;
    }

    public async Task<OperateResult> DelSetData(DelYcYxSetDataModel delYcYxSetDataModel)
    {
        if (delYcYxSetDataModel == null)
        {
            return OperateResult.Failed("请求参数为空");
        }

        var entity = await _context.SetParm.FirstOrDefaultAsync(x =>
            x.StaN == delYcYxSetDataModel.StaNo &&
            x.EquipNo == delYcYxSetDataModel.EquipNo &&
            x.SetNo == delYcYxSetDataModel.YcYxSetNo);

        if (entity == null)
        {
            return OperateResult.Failed("配置不存在");
        }

        _context.SetParm.Remove(entity);

        if (await _context.SaveChangesAsync() <= 0)
        {
            return OperateResult.Failed("删除设置配置信息失败！");
        }

        _equipBaseImpl.SysEvtLog(_session.UserName,
            "删除设置量配置信息 - 设备号:" + delYcYxSetDataModel.EquipNo + ";设置号:" + delYcYxSetDataModel.YcYxSetNo);

        await _apiLog.Audit(new AuditAction(_session.UserName)
        {
            ResourceName = "设备管理",
            EventType = "删除设置",
            Result = new AuditResult()
            {
                Default = $"删除设置号：({delYcYxSetDataModel.YcYxSetNo})成功，所属设备号({delYcYxSetDataModel.EquipNo})",
            }
        });

        ResetEquips(delYcYxSetDataModel.EquipNo);

        return OperateResult.Success;
    }


    private bool ExistsCode(long? ycYxSetNo, string code, string type, long equipNo)
    {
        bool exists = false;
        if (string.IsNullOrEmpty(code))
        {
            return exists;
        }
        switch (type)
        {
            case "yc":
                exists = _context.Ycp.Any(o => o.YcCode == code && o.EquipNo == equipNo && (!ycYxSetNo.HasValue || o.YcNo != ycYxSetNo.Value));
                break;
            case "yx":
                exists = _context.Yxp.Any(o => o.YxCode == code && o.EquipNo == equipNo && (!ycYxSetNo.HasValue || o.YxNo != ycYxSetNo.Value));
                break;
            case "set":
                exists = _context.SetParm.Any(o => o.SetCode == code && o.EquipNo == equipNo && (!ycYxSetNo.HasValue || o.SetNo != ycYxSetNo.Value));
                break;
        }
        return exists;
    }


    public async Task<OperateResult<AddEquipFromModelRequest[]>> AddEquipFromModelAsync(EquipSetModel equipSetModel)
    {
        if (equipSetModel == null)
        {
            return OperateResult.Failed<AddEquipFromModelRequest[]>("请求参数为空");
        }

        var iEquipNo = equipSetModel.iEquipNo;
        long equipNum = equipSetModel.equipNum;

        if (!_context.EGroup.Any(u => u.GroupId == equipSetModel.GroupId))
        {
            return OperateResult.Failed<AddEquipFromModelRequest[]>("所选子系统或分组不存在");
        }

        var curCount = await _context.EGroupList.CountAsync(u => u.GroupId == equipSetModel.GroupId);
        if (curCount + equipNum > 10000)
        {
            return OperateResult.Failed<AddEquipFromModelRequest[]>($"所选子系统或分组设备不能超过10000, 还可添加 {10000 - curCount} 设备.");
        }

        var ioTEquip = await _context.IotEquip.AsNoTracking()
            .FirstOrDefaultAsync(x => x.EquipNo == iEquipNo);

        if (ioTEquip == null)
        {
            return OperateResult.Failed<AddEquipFromModelRequest[]>("所选产品不存在！");
        }

        if (ioTEquip.EquipConnType == ProductConnType.子系统.GetHashCode())
        {
            if (equipNum > 1)
            {
                return OperateResult.Failed<AddEquipFromModelRequest[]>("该产品的设备连接类型为子系统，只需创建一个设备即可！");
            }

            var existEquip = _context.Equip.Any(m => m.RawEquipNo == iEquipNo);
            if (existEquip)
            {
                return OperateResult.Failed<AddEquipFromModelRequest[]>("该产品的设备连接类型为子系统，已经存在设备了！");
            }
        }

        var ioTEquipNum = _context.Equip.Any() ? _context.Equip.Max(d => d.EquipNo) : 0;

        var ycs = await _context.IotYcp.AsNoTracking().Where(x => x.EquipNo == iEquipNo)
            .ToListAsync();
        ycs.ForEach(d => d.EquipNo = 0);

        var yxs = await _context.IotYxp.AsNoTracking().Where(x => x.EquipNo == iEquipNo)
            .ToListAsync();
        yxs.ForEach(d => d.EquipNo = 0);

        var sets = await _context.IotSetParm.AsNoTracking().Where(x => x.EquipNo == iEquipNo)
            .ToListAsync();
        sets.ForEach(d => d.EquipNo = 0);

        var ycps = _context.Ycp.AsNoTracking().ToList();
        var ycNo = ycps.Any() ? ycps.Max(d => d.YcNo) : 1;

        var equips = new List<Equip>();

        for (int i = 0; i < equipNum; i++)
        {
            ioTEquipNum++;
            var equip = new Equip
            {
                StaN = ioTEquip.StaN <= 1 ? 1 : ioTEquip.StaN,
                EquipNm = $"{ioTEquip.EquipNm}#{ioTEquipNum}",
                EquipDetail = ioTEquip.EquipDetail,
                AccCyc = ioTEquip.AccCyc,
                RelatedPic = ioTEquip.RelatedPic,
                ProcAdvice = ioTEquip.ProcAdvice,
                OutOfContact = ioTEquip.OutOfContact,
                Contacted = ioTEquip.Contacted,
                EventWav = ioTEquip.EventWav,
                CommunicationDrv = ioTEquip.CommunicationDrv,
                LocalAddr = ioTEquip.LocalAddr,
                EquipAddr = ioTEquip.EquipAddr,
                CommunicationParam = ioTEquip.CommunicationParam,
                CommunicationTimeParam = ioTEquip.CommunicationTimeParam,
                RawEquipNo = iEquipNo,
                Tabname = ioTEquip.Tabname,
                AlarmScheme = ioTEquip.AlarmScheme,
                Attrib = ioTEquip.Attrib,
                StaIp = ioTEquip.StaIp,
                AlarmRiseCycle = ioTEquip.AlarmRiseCycle,
                Reserve1 = ioTEquip.Reserve1,
                Reserve2 = ioTEquip.Reserve2,
                Reserve3 = ioTEquip.Reserve3,
                RelatedVideo = ioTEquip.RelatedVideo,
                ZiChanId = ioTEquip.ZiChanId,
                PlanNo = ioTEquip.PlanNo,
                SafeTime = ioTEquip.SafeTime,
                Backup = ioTEquip.Backup
            };
            equip.Ycps.AddRange(ycs.MapToList<Ycp>());

            equip.Yxps.AddRange(yxs.MapToList<Yxp>());

            equip.SetParms.AddRange(sets.MapToList<SetParm>());

            equip.Ycps.ForEach(m => m.StaN = equip.StaN);
            equip.Yxps.ForEach(m => m.StaN = equip.StaN);
            equip.SetParms.ForEach(m => m.StaN = equip.StaN);
            equips.Add(equip);
            ycNo = equipSetModel.equipNum + 1;
        }

        await _context.Equip.AddRangeAsync(equips);
        if (await _context.SaveChangesAsync() <= 0)
        {
            return OperateResult.Failed<AddEquipFromModelRequest[]>("从模板设备库添加设备库失败！");
        }

        await _context.EGroupList.AddRangeAsync(equips.Select(d => new EGroupList()
        {
            EquipNo = d.EquipNo,
            GroupId = equipSetModel.GroupId <= 0 ? 1 : equipSetModel.GroupId,
            StaNo = 1
        }));

        if (await _context.SaveChangesAsync() <= 0)
        {
            return OperateResult.Failed<AddEquipFromModelRequest[]>("从模板设备库添加设备库失败！");
        }

        _permissionCacheService.AddEquipPermission(_session.RoleName, equips.Select(d => d.EquipNo).ToArray());

        var changeEquips = equips.Select(d => new GrpcChangedEquip()
        {
            iStaNo = 1,
            iEqpNo = d.EquipNo,
            State = ChangedEquipState.Add
        }).ToList();

        try
        {
            _proxy.AddChangedEquipList(changeEquips);
        }
        catch (Exception ex)
        {
            _apiLog.Error("AddChangedEquip - Add【设备重置接口异常】" + ex);
        }

        await _apiLog.Audit(new AuditAction(_session.UserName)
        {
            ResourceName = "设备管理",
            EventType = "从产品添加设备",
            Result = new AuditResult<string, string>
            {
                Default = "添加成功",
                New = $"设备号列表：{string.Join(",", equips.Select(d => d.EquipNo))}"
            }
        });

        return OperateResult.Successed(equips
            .Select(x => new AddEquipFromModelRequest(x.StaN, x.EquipNo, x.EquipNm)).ToArray());
    }

    public async Task<OperateResult> SetEquipToModel(long equipNo)
    {
        if (await _context.IotEquip.AsNoTracking()
            .AnyAsync(x => x.EquipNo == equipNo))
        {
            return OperateResult.Failed("添加失败，模板已存在！");
        }

        var iotEquip = await _context.Equip.AsNoTracking()
            .FirstOrDefaultAsync(x => x.EquipNo == equipNo);

        if (iotEquip == null)
        {
            return OperateResult.Failed("设备不存在");
        }

        var templateEquipNo = (await _context.IotEquip.MaxAsync(x => (int?)x.EquipNo)) ?? 0;

        templateEquipNo++;

        var ycps = await _context.Ycp.AsNoTracking().Where(x => x.EquipNo == equipNo).ToListAsync();
        ycps.ForEach(d => d.EquipNo = 0);

        var yxps = await _context.Yxp.AsNoTracking().Where(x => x.EquipNo == equipNo).ToListAsync();
        yxps.ForEach(d => d.EquipNo = 0);

        var setps = await _context.SetParm.AsNoTracking().Where(x => x.EquipNo == equipNo).ToListAsync();
        setps.ForEach(d => d.EquipNo = 0);

        var equip = new IotEquip
        {
            StaN = iotEquip.StaN,
            EquipNo = templateEquipNo,
            EquipNm = iotEquip.EquipNm,
            EquipDetail = iotEquip.EquipDetail,
            AccCyc = iotEquip.AccCyc,
            RelatedPic = iotEquip.RelatedPic,
            ProcAdvice = iotEquip.ProcAdvice,
            OutOfContact = iotEquip.OutOfContact,
            Contacted = iotEquip.Contacted,
            EventWav = iotEquip.EventWav,
            CommunicationDrv = iotEquip.CommunicationDrv,
            LocalAddr = iotEquip.LocalAddr,
            EquipAddr = iotEquip.EquipAddr,
            CommunicationParam = iotEquip.CommunicationParam,
            CommunicationTimeParam = iotEquip.CommunicationTimeParam,
            RawEquipNo = iotEquip.RawEquipNo,
            Tabname = iotEquip.Tabname,
            AlarmScheme = iotEquip.AlarmScheme,
            Attrib = iotEquip.Attrib,
            StaIp = iotEquip.StaIp,
            AlarmRiseCycle = iotEquip.AlarmRiseCycle,
            Reserve1 = iotEquip.Reserve1,
            Reserve2 = iotEquip.Reserve2,
            Reserve3 = iotEquip.Reserve3,
            RelatedVideo = iotEquip.RelatedVideo,
            ZiChanId = iotEquip.ZiChanId,
            PlanNo = iotEquip.PlanNo,
            SafeTime = iotEquip.SafeTime,
            Backup = iotEquip.Backup,
        };

        equip.IotYcps.AddRange(ycps.MapToList<IotYcp>());

        equip.IotYxps.AddRange(yxps.MapToList<IotYxp>());

        equip.IotSetParms.AddRange(setps.MapToList<IotSetParm>());

        await _context.IotEquip.AddAsync(equip);

        if (await _context.SaveChangesAsync() < 0)
        {
            return OperateResult.Failed("将设备设置为模板失败");
        }

        await _apiLog.Audit(new AuditAction(_session.UserName)
        {
            ResourceName = "设备管理",
            EventType = "设备设为产品",
            Result = new AuditResult { Default = $"设备号“{equipNo}”设置产品成功", }
        });

        return OperateResult.Successed(equipNo);
    }

    private void ResetEquips(long equipNo)
    {
        try
        {
            _proxy.AddChangedEquip(new GrpcChangedEquip
            {
                iStaNo = 1,
                iEqpNo = Convert.ToInt32(equipNo),
                State = ChangedEquipState.Edit
            });
        }
        catch (Exception ex)
        {
            _apiLog.Error("AddChangedEquip - Edit【设备重置接口异常】" + ex);
        }
    }

    private void ResetDeleteEquip(long equipNo)
    {
        try
        {
            _proxy.AddChangedEquip(new GrpcChangedEquip
            {
                iStaNo = 1,
                iEqpNo = Convert.ToInt32(equipNo),
                State = ChangedEquipState.Delete
            });
        }
        catch (Exception ex)
        {
            _apiLog.Error("AddChangedEquip - Delete【设备重置接口异常】" + ex);
        }
    }

    public async Task<OperateResult> BatchModifyEquipParam(BatchOperateEquipModel model)
    {
        var equipModels = await _context.Equip.Where(d => model.Ids.Contains(d.EquipNo) && d.StaN == model.StaN)
            .ToListAsync();

        if (equipModels.Count <= 0)
        {
            return OperateResult.Failed("没有相关设备，请检查设备号是否存在");
        }

        for (int i = 0; i < equipModels.Count; i++)
        {
            var equip = equipModels[i];
            ConvertType(ref equip, model);
        }

        if (await _context.SaveChangesAsync() < 0)
        {
            OperateResult.Failed("批量修改设备参数失败！");
        }

        _proxy.AddChangedEquipList(equipModels.Select(d => new GrpcChangedEquip
        {
            iStaNo = 1,
            iEqpNo = Convert.ToInt32(d.EquipNo),
            State = ChangedEquipState.Edit
        }));

        _apiLog.Info(
            $"用户\"[{_session.UserName}({_session.IpAddress})]\"-批量编辑设备-设备号:{string.Join(',', model.Ids)}-成功");

        return OperateResult.Success;
    }

    public async Task<OperateResult> BatchModifyYcp(BatchOperateEquipModel model)
    {
        var ycpModels = await _context.Ycp.Where(d => model.Ids.Contains(d.EquipNo) && d.StaN == model.StaN)
            .ToListAsync();

        if (ycpModels.Count <= 0)
        {
            return OperateResult.Failed("没有相关设备遥测，请检查设备是否存在遥测");
        }

        for (int i = 0; i < ycpModels.Count; i++)
        {
            var yc = ycpModels[i];
            ConvertType(ref yc, model);

        }
        var saveRes = await _context.SaveChangesAsync();
        if (saveRes < 0)
        {
            OperateResult.Failed("批量修改遥测失败！");
        }

        _proxy.AddChangedEquipList(ycpModels.Select(d => new GrpcChangedEquip
        {
            iStaNo = 1,
            iEqpNo = Convert.ToInt32(d.EquipNo),
            State = ChangedEquipState.Edit
        }));

        _apiLog.Info(
            $"用户\"[{_session.UserName}({_session.IpAddress})]\"-批量修改设备号:{string.Join(',', model.Ids)}下所有遥测--成功");

        return OperateResult.Success;
    }

    public async Task<OperateResult> BatchModifyYxp(BatchOperateEquipModel model)
    {
        var yxpModels = await _context.Yxp.Where(d => model.Ids.Contains(d.EquipNo) && d.StaN == model.StaN)
            .ToListAsync();

        if (yxpModels.Count <= 0)
        {
            return OperateResult.Failed("没有相关设备遥信，请检查设备是否存在遥信");
        }

        for (int i = 0; i < yxpModels.Count; i++)
        {
            var yx = yxpModels[i];
            ConvertType(ref yx, model);
        }

        if (await _context.SaveChangesAsync() < 0)
        {
            OperateResult.Failed("批量修改遥信失败！");
        }

        _proxy.AddChangedEquipList(yxpModels.Select(d => new GrpcChangedEquip
        {
            iStaNo = 1,
            iEqpNo = Convert.ToInt32(d.EquipNo),
            State = ChangedEquipState.Edit
        }));

        _apiLog.Info(
            $"用户\"[{_session.UserName} ( {_session.IpAddress})]\"-批量修改设备号:{string.Join(',', model.Ids)}下所有遥信--成功");

        return OperateResult.Success;
    }

    public async Task<OperateResult> BatchModifyEquipSetting(BatchOperateEquipModel model)
    {
        var setModels = await _context.SetParm.Where(d => model.Ids.Contains(d.EquipNo) && d.StaN == model.StaN)
            .ToListAsync();

        if (setModels.Count <= 0)
        {
            return OperateResult.Failed("没有相关设置，请检查设备是否存在设置项");
        }

        for (int i = 0; i < setModels.Count; i++)
        {
            var set = setModels[i];
            ConvertType(ref set, model);
        }

        if (await _context.SaveChangesAsync() < 0)
        {
            OperateResult.Failed("批量修改设置失败！");
        }

        _proxy.AddChangedEquipList(setModels.Select(d => new GrpcChangedEquip
        {
            iStaNo = 1,
            iEqpNo = Convert.ToInt32(d.EquipNo),
            State = ChangedEquipState.Edit
        }));

        _apiLog.Info(
            $"用户\"[{_session.UserName} ({_session.IpAddress})]\"-批量修改设备号:{string.Join(',', model.Ids)}下所有设置项--成功");
        return OperateResult.Success;
    }

    private void ConvertType<T>(ref T targetInstant, BatchOperateEquipModel model)
    {
        foreach (var name in model.Dicts.Keys)
        {
            var propertyInfo = targetInstant.GetType().GetProperty(name,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (propertyInfo == null || model.Dicts[name] == null)
            {
                _apiLog.Warn($"SystemConfigServiceImpl-ConvertType:{typeof(T)}中，字段:\"{name}\"不存在或为值为空");
                continue;
            }

            var propertyType = propertyInfo.PropertyType;
            var converter = TypeDescriptor.GetConverter(propertyType);

            try
            {
                var value = converter.ConvertFromString(model.Dicts[name].ToString());
                propertyInfo.SetValue(targetInstant, value, null);
            }
            catch (Exception e)
            {
                _apiLog.Error($"SystemConfigServiceImpl-ConvertType:{e}");
                _apiLog.Warn($"参数：{name} 类型错误，更新字段类型应为：{propertyType}");
            }
        }
    }

    public async Task<OperateResult> BatchDeleteEquip(BaseBatchOperateEquipModel model)
    {
        var actucalEquips = _context.Equip
             .Where(d => model.Ids.Contains(d.EquipNo) && d.StaN == model.StaN)
             .ToList();

        var actucalEquipNos = actucalEquips.Select(d => d.EquipNo).ToList();

        if (!actucalEquipNos.Any())
        {
            return OperateResult.Failed("设备相关数据已删除或不存在");
        }

        _context.EGroupList.RemoveRange(_context.EGroupList.Where(d => actucalEquipNos.Contains(d.EquipNo)));

        _context.Yxp.RemoveRange(_context.Yxp.Where(d => actucalEquipNos.Contains(d.EquipNo)));

        _context.Ycp.RemoveRange(_context.Ycp.Where(d => actucalEquipNos.Contains(d.EquipNo)));

        _context.SetParm.RemoveRange(_context.SetParm.Where(d => actucalEquipNos.Contains(d.EquipNo)));

        _context.Equip.RemoveRange(actucalEquips);

        try
        {
            await _context.SaveChangesAsync();

            _proxy.AddChangedEquipList(actucalEquipNos.Select(n =>
                new GrpcChangedEquip
                {
                    iStaNo = 1,
                    iEqpNo = n,
                    State = ChangedEquipState.Delete
                }));

            _apiLog.Info(
                $"用户\"[{_session.UserName}({_session.IpAddress})]\"-删除设备-设备号:【{string.Join(",", actucalEquipNos)}】-成功");
        }
        catch (Exception ex)
        {
            _apiLog.Error("BatchDeleteEquip【批量删除设备】:" + ex);
            return OperateResult.Failed("批量删除设备失败");
        }

        return OperateResult.Success;
    }

    public OperateResult BatchImportEquipOrTemplate(List<BatchEquipModel> equips,
        List<BatchYcpModel> ycps, List<BatchYxpModel> yxps,
        List<BatchSetModel> sets, string[] tableNames, int? groupId = null)
    {
        var equipNos = new List<int>();
        try
        {
            var equipMapperList = equips.Select(equip => new EquipDataModel
            {
                StaNo = equip.StaN <= 0 ? 1 : equip.StaN,
                EquipNm = equip.EquipName,
                EquipDetail = equip.EquipDetail,
                AccCyc = equip.AccCyc.Value,
                RelatedPic = equip.RelatedPic,
                ProcAdvice = equip.ProcAdvice,
                OutOfContact = equip.OutOfContact,
                Contacted = equip.Contacted,
                EventWav = equip.EventWav,
                CommunicationDrv = equip.CommunicationDrv,
                LocalAddr = equip.LocalAddr,
                EquipAddr = equip.EquipAddr,
                CommunicationParam = equip.CommunicationParam,
                CommunicationTimeParam = equip.CommunicationTimeParam,
                RawEquipNo = equip.RawEquipNo,
                Tabname = equip.TabName,
                StaIp = equip.StaIp,
                AlarmRiseCycle = equip.AlarmRiseCycle,
                Reserve1 = equip.Reserve1,
                Reserve2 = equip.Reserve2,
                Reserve3 = equip.Reserve3,
                RelatedVideo = equip.RelatedVideo,
                ZiChanId = equip.ZiChanID,
                PlanNo = equip.PlanNo,
                SafeTime = equip.SafeTime,
                Backup = equip.Backup,
                AlarmScheme =
                    Convert.ToInt32($"{equip.ShowAlarm}{equip.RecordAlarm}{equip.Smslarm}{equip.EmailAlarm}", 2),
            }).ToList();

            if (tableNames[0].Equals("IotEquip", StringComparison.OrdinalIgnoreCase))
            {
                var maxEquoNo = _context.IotEquip.Any() ? _context.IotEquip.Max(d => d.EquipNo) : 0;
                var IotEquipModels = _mapper.Map<List<EquipDataModel>, List<IotEquip>>(equipMapperList);

                int i = 0;
                foreach (var equipModel in IotEquipModels)
                {
                    equipModel.StaN = equipMapperList[i].StaNo;

                    equipModel.EquipNo = ++maxEquoNo;
                    var ycpModel = _mapper.Map<List<BatchYcpModel>, List<IotYcp>>(ycps);
                    var yxpModel = _mapper.Map<List<BatchYxpModel>, List<IotYxp>>(yxps);
                    var setModel = _mapper.Map<List<BatchSetModel>, List<IotSetParm>>(sets);

                    ycpModel.ForEach(x => x.StaN = equipModel.StaN);
                    yxpModel.ForEach(x => x.StaN = equipModel.StaN);
                    setModel.ForEach(x => x.StaN = equipModel.StaN);

                    equipModel.IotYcps.AddRange(ycpModel);
                    equipModel.IotYxps.AddRange(yxpModel);
                    equipModel.IotSetParms.AddRange(setModel);

                    i++;
                }

                _context.IotEquip.AddRange(IotEquipModels);

                if (_context.SaveChanges() < 0)
                {
                    return OperateResult.Failed("导入设备数据失败");
                }

                equipNos = IotEquipModels.Select(d => d.EquipNo).Where(d => d != 0).ToList();
            }
            else
            {
                var equipModels = _mapper.Map<List<EquipDataModel>, List<Equip>>(equipMapperList);

                int i = 0;
                foreach (var equipModel in equipModels)
                {
                    equipModel.StaN = equipMapperList[i].StaNo;

                    var ycpModel = _mapper.Map<List<BatchYcpModel>, List<Ycp>>(ycps);
                    var yxpModel = _mapper.Map<List<BatchYxpModel>, List<Yxp>>(yxps);
                    var setModel = _mapper.Map<List<BatchSetModel>, List<SetParm>>(sets);

                    ycpModel.ForEach(x => x.StaN = equipModel.StaN);
                    yxpModel.ForEach(x => x.StaN = equipModel.StaN);
                    setModel.ForEach(x => x.StaN = equipModel.StaN);

                    equipModel.Ycps.AddRange(ycpModel);
                    equipModel.Yxps.AddRange(yxpModel);
                    equipModel.SetParms.AddRange(setModel);

                    i++;
                }

                _context.Equip.AddRange(equipModels);

                if (_context.SaveChanges() < 0)
                {
                    return OperateResult.Failed("导入设备数据失败");
                }

                equipNos = equipModels.Select(d => d.EquipNo).Where(d => d != 0).ToList();
            }

            var changeEquips = new List<GrpcChangedEquip>();
            if (groupId.HasValue && equipNos.Any())
            {
                var models = equipNos.Select(d => new EGroupList()
                {
                    StaNo = 1,
                    EquipNo = d,
                    GroupId = groupId.Value
                });

                changeEquips = equipNos.Select(d => new GrpcChangedEquip()
                {
                    iStaNo = 1,
                    iEqpNo = d,
                    State = ChangedEquipState.Add
                }).ToList();

                _context.EGroupList.AddRange(models);

                _context.SaveChanges();
            }

            try
            {
                if (changeEquips.Count > 0)
                {
                    _proxy.AddChangedEquipList(changeEquips);
                }
            }
            catch (Exception ex)
            {
                _apiLog.Error("AddChangedEquip - Add【设备重置接口异常】" + ex);
            }

            _apiLog.Audit(new AuditAction()
            {
                ResourceName = "设备管理",
                EventType = "导入设备或产品",
                Result = new AuditResult { Default = $"设备号列表：“{string.Join(",", equipNos)}”导入成功" }
            });

            return OperateResult.Success;
        }
        catch (Exception ex)
        {
            _apiLog.Error("BatchImportEquip【批量导入设备】:" + ex);
            return OperateResult.Failed("批量导入设备失败,注意每个excel中测点号、遥信号、设置号不能相同");
        }
    }


    public OperateResult<byte[]> BatchExportEquip(List<int> ids, bool exportEquip = true)
    {
        var equipTableName = exportEquip ? "equip" : "iotequip";
        var ycpTableName = exportEquip ? "ycp" : "iotycp";
        var yxpTableName = exportEquip ? "yxp" : "iotyxp";
        var setTableName = exportEquip ? "setparm" : "iotsetparm";

        if (ids == null || ids.Count <= 0)
        {
            return OperateResult.Failed<byte[]>("数据为空");
        }

        var equips = new List<BatchEquipModel>();
        if (equipTableName == "equip")
        {
            var equip = _context.Equip.AsNoTracking()
                .Where(d => ids.Contains(d.EquipNo)).ToList();

            equips = equip.MapToList<BatchEquipModel>();
        }
        else
        {
            var equip = _context.IotEquip.AsNoTracking().Where(d => ids.Contains(d.EquipNo)).ToList();

            equips = equip.MapToList<BatchEquipModel>();
        }

        if (!equips.Any())
        {
            return OperateResult.Failed<byte[]>("数据为空");
        }

        _ = equips.All(equip =>
        {
            var scheme = Convert.ToString(equip.AlarmScheme, 2).PadRight(4, '0')
                .ToCharArray();

            equip.ShowAlarm = Convert.ToInt32(scheme[0].ToString());
            equip.RecordAlarm = Convert.ToInt32(scheme[1].ToString());
            equip.Smslarm = Convert.ToInt32(scheme[2].ToString());
            equip.EmailAlarm = Convert.ToInt32(scheme[3].ToString());

            return true;
        });

        equips = equips.OrderBy(d => d.EquipNo).ToList();

        var ycps = new List<BatchYcpModel>();
        if (ycpTableName == "ycp")
        {
            var ycp = _context.Ycp.AsNoTracking()
                .Where(d => ids.Contains(d.EquipNo)).ToList();

            ycps = ycp.MapToList<BatchYcpModel>();
        }
        else
        {
            var ycp = _context.IotYcp.AsNoTracking().Where(d => ids.Contains(d.EquipNo)).ToList();

            ycps = ycp.MapToList<BatchYcpModel>();
        }

        if (ycps.Any())
        {
            _ = ycps.All(ycp =>
            {
                var scheme = Convert.ToString(ycp.AlarmScheme, 2).PadLeft(4, '0').ToCharArray();

                ycp.ShowAlarm = Convert.ToInt32(scheme[0].ToString());
                ycp.RecordAlarm = Convert.ToInt32(scheme[1].ToString());
                ycp.Smslarm = Convert.ToInt32(scheme[2].ToString());
                ycp.EmailAlarm = Convert.ToInt32(scheme[3].ToString());

                return true;
            });

            ycps = ycps.OrderBy(d => d.EquipNo).ToList();
        }

        List<BatchYxpModel> yxps;

        if (yxpTableName == "yxp")
        {
            var yxp = _context.Yxp.AsNoTracking().Where(d => ids.Contains(d.EquipNo)).ToList();
            yxps = yxp.MapToList<BatchYxpModel>();
        }
        else
        {
            var yxp = _context.IotYxp.AsNoTracking().Where(d => ids.Contains(d.EquipNo)).ToList();
            yxps = yxp.MapToList<BatchYxpModel>();
        }

        if (yxps.Any())
        {
            _ = yxps.All(yxp =>
            {
                var scheme = Convert.ToString(yxp.AlarmScheme, 2).PadLeft(4, '0').ToCharArray();

                yxp.ShowAlarm = Convert.ToInt32(scheme[0].ToString());
                yxp.RecordAlarm = Convert.ToInt32(scheme[1].ToString());
                yxp.Smslarm = Convert.ToInt32(scheme[2].ToString());
                yxp.EmailAlarm = Convert.ToInt32(scheme[3].ToString());

                return true;
            });

            yxps = yxps.OrderBy(d => d.EquipNo).ToList();
        }

        var sets = new List<BatchSetModel>();
        if (setTableName == "setparm")
        {
            var setParms = _context.SetParm.AsNoTracking()
                .Where(d => ids.Contains(d.EquipNo)).ToList();

            sets = setParms.MapToList<BatchSetModel>();
        }
        else
        {
            var setParms = _context.IotSetParm.AsNoTracking().Where(d => ids.Contains(d.EquipNo)).ToList();

            sets = setParms.MapToList<BatchSetModel>();
        }

        if (sets.Any())
        {
            sets = sets.OrderBy(d => d.EquipNo).ToList();
        }

        var equipNames = new List<PropertyByName<BatchEquipModel>>
        {
            new PropertyByName<BatchEquipModel>("设备号", d => d.EquipNo),
            new PropertyByName<BatchEquipModel>("设备名称", d => d.EquipName),
            new PropertyByName<BatchEquipModel>("设备属性", d => d.EquipDetail),
            new PropertyByName<BatchEquipModel>("设备地址", d => d.EquipAddr),
            new PropertyByName<BatchEquipModel>("模板设备号", d => d.RawEquipNo),
            new PropertyByName<BatchEquipModel>("关联页面", d => d.RelatedPic),
            new PropertyByName<BatchEquipModel>("关联视频", d => d.RelatedVideo),
            new PropertyByName<BatchEquipModel>("显示报警", d => d.ShowAlarm),
            new PropertyByName<BatchEquipModel>("记录报警", d => d.RecordAlarm),
            new PropertyByName<BatchEquipModel>("短信报警", d => d.Smslarm),
            new PropertyByName<BatchEquipModel>("Email报警", d => d.EmailAlarm),
            new PropertyByName<BatchEquipModel>("通讯故障处理意见", d => d.ProcAdvice),
            new PropertyByName<BatchEquipModel>("故障信息", d => d.OutOfContact),
            new PropertyByName<BatchEquipModel>("故障恢复提示", d => d.Contacted),
            new PropertyByName<BatchEquipModel>("报警声音文件", d => d.EventWav),
            new PropertyByName<BatchEquipModel>("报警方式", d => d.AlarmScheme),
            new PropertyByName<BatchEquipModel>("附表名称", d => d.TabName),
            new PropertyByName<BatchEquipModel>("站点IP", d => d.StaIp),
            new PropertyByName<BatchEquipModel>("驱动文件", d => d.CommunicationDrv),
            new PropertyByName<BatchEquipModel>("通讯端口", d => d.LocalAddr),
            new PropertyByName<BatchEquipModel>("通讯刷新周期", d => d.AccCyc),
            new PropertyByName<BatchEquipModel>("通讯参数", d => d.CommunicationParam),
            new PropertyByName<BatchEquipModel>("通讯时间参数", d => d.CommunicationTimeParam),
            new PropertyByName<BatchEquipModel>("报警升级周期（分钟）", d => d.AlarmRiseCycle)
        };


        var ziChan = new PropertyByName<BatchEquipModel>("资产编号",
            d => d.ZiChanID);
        equipNames.Add(ziChan);

        var planNo = new PropertyByName<BatchEquipModel>("预案号",
            d => d.PlanNo);
        equipNames.Add(planNo);

        var safeTime = new PropertyByName<BatchEquipModel>("安全时段",
            d => d.SafeTime);
        equipNames.Add(safeTime);

        var backup = new PropertyByName<BatchEquipModel>("双机热备",
            d => d.Backup);
        equipNames.Add(backup);

        var deviceType = new PropertyByName<BatchEquipModel>("设备类型",
            d => d.DeviceType);
        equipNames.Add(deviceType);

        equipNames.Add(new PropertyByName<BatchEquipModel>("预留字段1", d => d.Reserve1));
        equipNames.Add(new PropertyByName<BatchEquipModel>("预留字段2", d => d.Reserve2));
        equipNames.Add(new PropertyByName<BatchEquipModel>("预留字段3", d => d.Reserve3));

        IWorkbook workbook = new XSSFWorkbook();

        _exportManager.ExportToXlsx(workbook, equipNames.ToArray(), equips, "设备");

        var ycpNames = new PropertyByName<BatchYcpModel>[]
        {
            new PropertyByName<BatchYcpModel>("设备号", d => d.EquipNo),
            new PropertyByName<BatchYcpModel>("模拟量号", d => d.YcNo),
            new PropertyByName<BatchYcpModel>("模拟量名称", d => d.YcNm),
            new PropertyByName<BatchYcpModel>("下限值", d => d.ValMin),
            new PropertyByName<BatchYcpModel>("回复下限值", d => d.RestoreMin),
            new PropertyByName<BatchYcpModel>("回复上限值", d => d.RestoreMax),
            new PropertyByName<BatchYcpModel>("上限值", d => d.ValMax),
            new PropertyByName<BatchYcpModel>("单位", d => d.Unit),
            new PropertyByName<BatchYcpModel>("关联页面", d => d.RelatedPic),
            new PropertyByName<BatchYcpModel>("关联视频", d => d.RelatedVideo),
            new PropertyByName<BatchYcpModel>("资产编号", d => d.ZiChanId),
            new PropertyByName<BatchYcpModel>("预案号", d => d.PlanNo),
            new PropertyByName<BatchYcpModel>("曲线记录", d => d.CurveRcd),
            new PropertyByName<BatchYcpModel>("曲线记录阈值", d => d.CurveLimit),
            new PropertyByName<BatchYcpModel>("越上限事件", d => d.OutmaxEvt),
            new PropertyByName<BatchYcpModel>("越下限事件", d => d.OutminEvt),
            new PropertyByName<BatchYcpModel>("显示报警", d => d.ShowAlarm),
            new PropertyByName<BatchYcpModel>("记录报警", d => d.RecordAlarm),
            new PropertyByName<BatchYcpModel>("短信报警", d => d.Smslarm),
            new PropertyByName<BatchYcpModel>("Email报警", d => d.EmailAlarm),
            new PropertyByName<BatchYcpModel>("报警级别", d => d.LvlLevel),
            new PropertyByName<BatchYcpModel>("属性值", d => d.ValTrait),
            new PropertyByName<BatchYcpModel>("比例变换", d => d.Mapping),
            new PropertyByName<BatchYcpModel>("实测最小值", d => d.PhysicMin),
            new PropertyByName<BatchYcpModel>("实测最大值", d => d.PhysicMax),
            new PropertyByName<BatchYcpModel>("最小值", d => d.YcMin),
            new PropertyByName<BatchYcpModel>("最大值", d => d.YcMax),
            new PropertyByName<BatchYcpModel>("操作命令", d => d.MainInstruction),
            new PropertyByName<BatchYcpModel>("操作参数", d => d.MinorInstruction),
            new PropertyByName<BatchYcpModel>("越线滞纳时间（秒）", d => d.AlarmAcceptableTime),
            new PropertyByName<BatchYcpModel>("恢复滞纳时间（秒）", d => d.RestoreAcceptableTime),
            new PropertyByName<BatchYcpModel>("重复报警时间（分钟）", d => d.AlarmRepeatTime),
            new PropertyByName<BatchYcpModel>("报警升级周期", d => d.AlarmRiseCycle),
            new PropertyByName<BatchYcpModel>("声音文件", d => d.WaveFile),
            new PropertyByName<BatchYcpModel>("报警屏蔽 ", d => d.AlarmShield),
            new PropertyByName<BatchYcpModel>("安全时段", d => d.SafeTime),
            new PropertyByName<BatchYcpModel>("预留字段1", d => d.Reserve1),
            new PropertyByName<BatchYcpModel>("预留字段2", d => d.Reserve2),
            new PropertyByName<BatchYcpModel>("预留字段3", d => d.Reserve3)
        };

        _exportManager.ExportToXlsx(workbook, ycpNames, ycps, "遥测");

        var yxpNames = new PropertyByName<BatchYxpModel>[]
        {
            new PropertyByName<BatchYxpModel>("设备号", d => d.EquipNo),
            new PropertyByName<BatchYxpModel>("状态量号", d => d.YxNo),
            new PropertyByName<BatchYxpModel>("状态量名称", d => d.YxNm),
            new PropertyByName<BatchYxpModel>("事件（0-1）", d => d.Evt01),
            new PropertyByName<BatchYxpModel>("事件（1-0）", d => d.Evt10),
            new PropertyByName<BatchYxpModel>("关联页面", d => d.RelatedPic),
            new PropertyByName<BatchYxpModel>("关联视频", d => d.RelatedVideo),
            new PropertyByName<BatchYxpModel>("资产编号", d => d.ZiChanId),
            new PropertyByName<BatchYxpModel>("预案号", d => d.PlanNo),
            new PropertyByName<BatchYxpModel>("曲线记录", d => d.CurveRcd),
            new PropertyByName<BatchYxpModel>("显示报警", d => d.ShowAlarm),
            new PropertyByName<BatchYxpModel>("记录报警", d => d.RecordAlarm),
            new PropertyByName<BatchYxpModel>("短信报警", d => d.Smslarm),
            new PropertyByName<BatchYxpModel>("Email报警", d => d.EmailAlarm),
            new PropertyByName<BatchYxpModel>("属性值", d => d.ValTrait),
            new PropertyByName<BatchYxpModel>("是否取反", d => d.Inversion),
            new PropertyByName<BatchYxpModel>("处理意见（0-1）", d => d.ProcAdviceR),
            new PropertyByName<BatchYxpModel>("处理意见（1-0）", d => d.ProcAdviceD),
            new PropertyByName<BatchYxpModel>("级别（0-1）", d => d.LevelR),
            new PropertyByName<BatchYxpModel>("级别（1-0）", d => d.LevelD),
            new PropertyByName<BatchYxpModel>("初始状态", d => d.Initval),
            new PropertyByName<BatchYxpModel>("操作命令", d => d.MainInstruction),
            new PropertyByName<BatchYxpModel>("操作参数", d => d.MinorInstruction),
            new PropertyByName<BatchYxpModel>("越线滞纳时间（秒）", d => d.AlarmAcceptableTime),
            new PropertyByName<BatchYxpModel>("恢复滞纳时间（秒）", d => d.RestoreAcceptableTime),
            new PropertyByName<BatchYxpModel>("重复报警时间（分钟）", d => d.AlarmRepeatTime),
            new PropertyByName<BatchYxpModel>("报警升级周期", d => d.AlarmRiseCycle),
            new PropertyByName<BatchYxpModel>("声音文件", d => d.WaveFile),
            new PropertyByName<BatchYxpModel>("报警屏蔽 ", d => d.AlarmShield),
            new PropertyByName<BatchYxpModel>("安全时段", d => d.SafeTime),
            new PropertyByName<BatchYxpModel>("预留字段1", d => d.Reserve1),
            new PropertyByName<BatchYxpModel>("预留字段2", d => d.Reserve2),
            new PropertyByName<BatchYxpModel>("预留字段3", d => d.Reserve3)
        };

        _exportManager.ExportToXlsx(workbook, yxpNames, yxps, "遥信");

        var setNames = new PropertyByName<BatchSetModel>[]
        {
            new PropertyByName<BatchSetModel>("设备号", d => d.EquipNo),
            new PropertyByName<BatchSetModel>("设置号", d => d.SetNo),
            new PropertyByName<BatchSetModel>("设置名称", d => d.SetNm),
            new PropertyByName<BatchSetModel>("值", d => d.Value),
            new PropertyByName<BatchSetModel>("设置类型", d => d.SetType),
            new PropertyByName<BatchSetModel>("动作", d => d.Action),
            new PropertyByName<BatchSetModel>("操作命令", d => d.MainInstruction),
            new PropertyByName<BatchSetModel>("操作参数", d => d.MinorInstruction),
            new PropertyByName<BatchSetModel>("记录", d => d.Record),
            new PropertyByName<BatchSetModel>("语音控制字符", d => d.VoiceKeys),
            new PropertyByName<BatchSetModel>("是否语音控制", d => d.EnableVoice),
            new PropertyByName<BatchSetModel>("预留字段1", d => d.Reserve1),
            new PropertyByName<BatchSetModel>("预留字段2", d => d.Reserve2),
            new PropertyByName<BatchSetModel>("预留字段3", d => d.Reserve3)
        };

        _exportManager.ExportToXlsx(workbook, setNames, sets, "设置");

        using var stream = new MemoryStream();

        workbook.Write(stream);

        _apiLog.Audit(new AuditAction()
        {
            ResourceName = "设备管理",
            EventType = "导入设备",
            Result = new AuditResult
            {
                Default = $"设备号列表：“{string.Join(",", equips.Select(d => d.EquipNo))}”导入成功"
            }
        });

        return OperateResult.Successed(stream.ToArray());
    }

    public OperateResult<IWorkbook> BatchXmlEquip(List<int> ids, bool exportEquip = true)
    {
        var equipTableName = exportEquip ? "equip" : "iotequip";
        var ycpTableName = exportEquip ? "ycp" : "iotycp";
        var yxpTableName = exportEquip ? "yxp" : "iotyxp";
        var setTableName = exportEquip ? "setparm" : "iotsetparm";

        if (ids == null || !ids.Any())
        {
            return OperateResult.Failed<IWorkbook>("数据为空");
        }

        var equips = new List<BatchEquipModel>();
        if (equipTableName == "equip")
        {
            var equip = _context.Equip.AsNoTracking()
                .Where(d => ids.Contains(d.EquipNo)).ToList();

            equips = equip.MapToList<BatchEquipModel>();
        }
        else
        {
            var equip = _context.IotEquip.AsNoTracking().Where(d => ids.Contains(d.EquipNo)).ToList();

            equips = equip.MapToList<BatchEquipModel>();
        }

        if (!equips.Any())
        {
            return OperateResult.Failed<IWorkbook>("数据为空");
        }

        _ = equips.All(equip =>
        {
            var scheme = Convert.ToString(equip.AlarmScheme, 2).PadRight(4, '0')
                .ToCharArray();

            equip.ShowAlarm = Convert.ToInt32(scheme[0].ToString());
            equip.RecordAlarm = Convert.ToInt32(scheme[1].ToString());
            equip.Smslarm = Convert.ToInt32(scheme[2].ToString());
            equip.EmailAlarm = Convert.ToInt32(scheme[3].ToString());

            return true;
        });

        equips = equips.OrderBy(d => d.EquipNo).ToList();

        var ycps = new List<BatchYcpModel>();
        if (ycpTableName == "ycp")
        {
            var equip = _context.Ycp.AsNoTracking()
                .Where(d => ids.Contains(d.EquipNo)).ToList();

            ycps = equip.MapToList<BatchYcpModel>();
        }
        else
        {
            var equip = _context.IotYcp.AsNoTracking().Where(d => ids.Contains(d.EquipNo)).ToList();

            ycps = equip.MapToList<BatchYcpModel>();
        }

        if (ycps.Any())
        {
            _ = ycps.All(ycp =>
            {
                var scheme = Convert.ToString(ycp.AlarmScheme, 2).PadLeft(4, '0').ToCharArray();

                ycp.ShowAlarm = Convert.ToInt32(scheme[0].ToString());
                ycp.RecordAlarm = Convert.ToInt32(scheme[1].ToString());
                ycp.Smslarm = Convert.ToInt32(scheme[2].ToString());
                ycp.EmailAlarm = Convert.ToInt32(scheme[3].ToString());

                return true;
            });

            ycps = ycps.OrderBy(d => d.EquipNo).ToList();
        }

        List<BatchYxpModel> yxps;

        if (yxpTableName == "yxp")
        {
            var yxp = _context.Yxp.AsNoTracking().Where(d => ids.Contains(d.EquipNo)).ToList();
            yxps = yxp.MapToList<BatchYxpModel>();
        }
        else
        {
            var yxp = _context.IotYxp.AsNoTracking().Where(d => ids.Contains(d.EquipNo)).ToList();
            yxps = yxp.MapToList<BatchYxpModel>();
        }

        if (yxps.Any())
        {
            _ = yxps.All(yxp =>
            {
                var scheme = Convert.ToString(yxp.AlarmScheme, 2).PadLeft(4, '0').ToCharArray();

                yxp.ShowAlarm = Convert.ToInt32(scheme[0].ToString());
                yxp.RecordAlarm = Convert.ToInt32(scheme[1].ToString());
                yxp.Smslarm = Convert.ToInt32(scheme[2].ToString());
                yxp.EmailAlarm = Convert.ToInt32(scheme[3].ToString());

                return true;
            });

            yxps = yxps.OrderBy(d => d.EquipNo).ToList();
        }

        var sets = new List<BatchSetModel>();
        if (setTableName == "setparm")
        {
            var setParms = _context.SetParm.AsNoTracking()
                .Where(d => ids.Contains(d.EquipNo)).ToList();

            sets = setParms.MapToList<BatchSetModel>();
        }
        else
        {
            var setParms = _context.IotSetParm.AsNoTracking().Where(d => ids.Contains(d.EquipNo)).ToList();

            sets = setParms.MapToList<BatchSetModel>();
        }

        if (sets.Any())
        {
            sets = sets.OrderBy(d => d.EquipNo).ToList();
        }

        var equipNames = new List<PropertyByName<BatchEquipModel>>
        {
            new PropertyByName<BatchEquipModel>("设备号", d => d.EquipNo),
            new PropertyByName<BatchEquipModel>("设备名称", d => d.EquipName),
            new PropertyByName<BatchEquipModel>("设备属性", d => d.EquipDetail),
            new PropertyByName<BatchEquipModel>("设备地址", d => d.EquipAddr),
            new PropertyByName<BatchEquipModel>("模板设备号", d => d.RawEquipNo),
            new PropertyByName<BatchEquipModel>("关联页面", d => d.RelatedPic),
            new PropertyByName<BatchEquipModel>("关联视频", d => d.RelatedVideo),
            new PropertyByName<BatchEquipModel>("显示报警", d => d.ShowAlarm),
            new PropertyByName<BatchEquipModel>("记录报警", d => d.RecordAlarm),
            new PropertyByName<BatchEquipModel>("短信报警", d => d.Smslarm),
            new PropertyByName<BatchEquipModel>("Email报警", d => d.EmailAlarm),
            new PropertyByName<BatchEquipModel>("通讯故障处理意见", d => d.ProcAdvice),
            new PropertyByName<BatchEquipModel>("故障信息", d => d.OutOfContact),
            new PropertyByName<BatchEquipModel>("故障恢复提示", d => d.Contacted),
            new PropertyByName<BatchEquipModel>("报警声音文件", d => d.EventWav),
            new PropertyByName<BatchEquipModel>("报警方式", d => d.AlarmScheme),
            new PropertyByName<BatchEquipModel>("附表名称", d => d.TabName),
            new PropertyByName<BatchEquipModel>("站点IP", d => d.StaIp),
            new PropertyByName<BatchEquipModel>("驱动文件", d => d.CommunicationDrv),
            new PropertyByName<BatchEquipModel>("通讯端口", d => d.LocalAddr),
            new PropertyByName<BatchEquipModel>("通讯刷新周期", d => d.AccCyc),
            new PropertyByName<BatchEquipModel>("通讯参数", d => d.CommunicationParam),
            new PropertyByName<BatchEquipModel>("通讯时间参数", d => d.CommunicationTimeParam),
            new PropertyByName<BatchEquipModel>("报警升级周期（分钟）", d => d.AlarmRiseCycle)
        };


        var ziChan = new PropertyByName<BatchEquipModel>("资产编号",
            d => d.ZiChanID);
        equipNames.Add(ziChan);

        var planNo = new PropertyByName<BatchEquipModel>("预案号",
            d => d.PlanNo);
        equipNames.Add(planNo);

        var safeTime = new PropertyByName<BatchEquipModel>("安全时段",
            d => d.SafeTime);
        equipNames.Add(safeTime);

        var backup = new PropertyByName<BatchEquipModel>("双机热备",
            d => d.Backup);
        equipNames.Add(backup);

        equipNames.Add(new PropertyByName<BatchEquipModel>("预留字段1", d => d.Reserve1));
        equipNames.Add(new PropertyByName<BatchEquipModel>("预留字段2", d => d.Reserve2));
        equipNames.Add(new PropertyByName<BatchEquipModel>("预留字段3", d => d.Reserve3));

        IWorkbook workbook = new XSSFWorkbook();

        _exportManager.ExportToXlsx(workbook, equipNames.ToArray(), equips, "设备");

        var ycpNames = new PropertyByName<BatchYcpModel>[]
        {
            new PropertyByName<BatchYcpModel>("设备号", d => d.EquipNo),
            new PropertyByName<BatchYcpModel>("模拟量号", d => d.YcNo),
            new PropertyByName<BatchYcpModel>("模拟量名称", d => d.YcNm),
            new PropertyByName<BatchYcpModel>("下限值", d => d.ValMin),
            new PropertyByName<BatchYcpModel>("回复下限值", d => d.RestoreMin),
            new PropertyByName<BatchYcpModel>("回复上限值", d => d.RestoreMax),
            new PropertyByName<BatchYcpModel>("上限值", d => d.ValMax),
            new PropertyByName<BatchYcpModel>("单位", d => d.Unit),
            new PropertyByName<BatchYcpModel>("关联页面", d => d.RelatedPic),
            new PropertyByName<BatchYcpModel>("关联视频", d => d.RelatedVideo),
            new PropertyByName<BatchYcpModel>("资产编号", d => d.ZiChanId),
            new PropertyByName<BatchYcpModel>("预案号", d => d.PlanNo),
            new PropertyByName<BatchYcpModel>("曲线记录", d => d.CurveRcd),
            new PropertyByName<BatchYcpModel>("曲线记录阈值", d => d.CurveLimit),
            new PropertyByName<BatchYcpModel>("越上限事件", d => d.OutmaxEvt),
            new PropertyByName<BatchYcpModel>("越下限事件", d => d.OutminEvt),
            new PropertyByName<BatchYcpModel>("显示报警", d => d.ShowAlarm),
            new PropertyByName<BatchYcpModel>("记录报警", d => d.RecordAlarm),
            new PropertyByName<BatchYcpModel>("短信报警", d => d.Smslarm),
            new PropertyByName<BatchYcpModel>("Email报警", d => d.EmailAlarm),
            new PropertyByName<BatchYcpModel>("报警级别", d => d.LvlLevel),
            new PropertyByName<BatchYcpModel>("属性值", d => d.ValTrait),
            new PropertyByName<BatchYcpModel>("比例变换", d => d.Mapping),
            new PropertyByName<BatchYcpModel>("实测最小值", d => d.PhysicMin),
            new PropertyByName<BatchYcpModel>("实测最大值", d => d.PhysicMax),
            new PropertyByName<BatchYcpModel>("最小值", d => d.YcMin),
            new PropertyByName<BatchYcpModel>("最大值", d => d.YcMax),
            new PropertyByName<BatchYcpModel>("操作命令", d => d.MainInstruction),
            new PropertyByName<BatchYcpModel>("操作参数", d => d.MinorInstruction),
            new PropertyByName<BatchYcpModel>("越线滞纳时间（秒）", d => d.AlarmAcceptableTime),
            new PropertyByName<BatchYcpModel>("恢复滞纳时间（秒）", d => d.RestoreAcceptableTime),
            new PropertyByName<BatchYcpModel>("重复报警时间（分钟）", d => d.AlarmRepeatTime),
            new PropertyByName<BatchYcpModel>("报警升级周期", d => d.AlarmRiseCycle),
            new PropertyByName<BatchYcpModel>("声音文件", d => d.WaveFile),
            new PropertyByName<BatchYcpModel>("报警屏蔽 ", d => d.AlarmShield),
            new PropertyByName<BatchYcpModel>("安全时段", d => d.SafeTime),
            new PropertyByName<BatchYcpModel>("预留字段1", d => d.Reserve1),
            new PropertyByName<BatchYcpModel>("预留字段2", d => d.Reserve2),
            new PropertyByName<BatchYcpModel>("预留字段3", d => d.Reserve3)
        };

        _exportManager.ExportToXlsx(workbook, ycpNames, ycps, "遥测");

        var yxpNames = new PropertyByName<BatchYxpModel>[]
        {
            new PropertyByName<BatchYxpModel>("设备号", d => d.EquipNo),
            new PropertyByName<BatchYxpModel>("状态量号", d => d.YxNo),
            new PropertyByName<BatchYxpModel>("状态量名称", d => d.YxNm),
            new PropertyByName<BatchYxpModel>("事件（0-1）", d => d.Evt01),
            new PropertyByName<BatchYxpModel>("事件（1-0）", d => d.Evt10),
            new PropertyByName<BatchYxpModel>("关联页面", d => d.RelatedPic),
            new PropertyByName<BatchYxpModel>("关联视频", d => d.RelatedVideo),
            new PropertyByName<BatchYxpModel>("资产编号", d => d.ZiChanId),
            new PropertyByName<BatchYxpModel>("预案号", d => d.PlanNo),
            new PropertyByName<BatchYxpModel>("曲线记录", d => d.CurveRcd),
            new PropertyByName<BatchYxpModel>("显示报警", d => d.ShowAlarm),
            new PropertyByName<BatchYxpModel>("记录报警", d => d.RecordAlarm),
            new PropertyByName<BatchYxpModel>("短信报警", d => d.Smslarm),
            new PropertyByName<BatchYxpModel>("Email报警", d => d.EmailAlarm),
            new PropertyByName<BatchYxpModel>("属性值", d => d.ValTrait),
            new PropertyByName<BatchYxpModel>("是否取反", d => d.Inversion),
            new PropertyByName<BatchYxpModel>("处理意见（0-1）", d => d.ProcAdviceR),
            new PropertyByName<BatchYxpModel>("处理意见（1-0）", d => d.ProcAdviceD),
            new PropertyByName<BatchYxpModel>("级别（0-1）", d => d.LevelR),
            new PropertyByName<BatchYxpModel>("级别（1-0）", d => d.LevelD),
            new PropertyByName<BatchYxpModel>("初始状态", d => d.Initval),
            new PropertyByName<BatchYxpModel>("操作命令", d => d.MainInstruction),
            new PropertyByName<BatchYxpModel>("操作参数", d => d.MinorInstruction),
            new PropertyByName<BatchYxpModel>("越线滞纳时间（秒）", d => d.AlarmAcceptableTime),
            new PropertyByName<BatchYxpModel>("恢复滞纳时间（秒）", d => d.RestoreAcceptableTime),
            new PropertyByName<BatchYxpModel>("重复报警时间（分钟）", d => d.AlarmRepeatTime),
            new PropertyByName<BatchYxpModel>("报警升级周期", d => d.AlarmRiseCycle),
            new PropertyByName<BatchYxpModel>("声音文件", d => d.WaveFile),
            new PropertyByName<BatchYxpModel>("报警屏蔽 ", d => d.AlarmShield),
            new PropertyByName<BatchYxpModel>("安全时段", d => d.SafeTime),
            new PropertyByName<BatchYxpModel>("预留字段1", d => d.Reserve1),
            new PropertyByName<BatchYxpModel>("预留字段2", d => d.Reserve2),
            new PropertyByName<BatchYxpModel>("预留字段3", d => d.Reserve3)
        };

        _exportManager.ExportToXlsx(workbook, yxpNames, yxps, "遥信");

        var setNames = new PropertyByName<BatchSetModel>[]
        {
            new PropertyByName<BatchSetModel>("设备号", d => d.EquipNo),
            new PropertyByName<BatchSetModel>("设置号", d => d.SetNo),
            new PropertyByName<BatchSetModel>("设置名称", d => d.SetNm),
            new PropertyByName<BatchSetModel>("值", d => d.Value),
            new PropertyByName<BatchSetModel>("设置类型", d => d.SetType),
            new PropertyByName<BatchSetModel>("动作", d => d.Action),
            new PropertyByName<BatchSetModel>("操作命令", d => d.MainInstruction),
            new PropertyByName<BatchSetModel>("操作参数", d => d.MinorInstruction),
            new PropertyByName<BatchSetModel>("记录", d => d.Record),
            new PropertyByName<BatchSetModel>("语音控制字符", d => d.VoiceKeys),
            new PropertyByName<BatchSetModel>("是否语音控制", d => d.EnableVoice),
            new PropertyByName<BatchSetModel>("预留字段1", d => d.Reserve1),
            new PropertyByName<BatchSetModel>("预留字段2", d => d.Reserve2),
            new PropertyByName<BatchSetModel>("预留字段3", d => d.Reserve3)
        };

        _exportManager.ExportToXlsx(workbook, setNames, sets, "设置");

        using var stream = new MemoryStream();

        workbook.Write(stream);

        _apiLog.Audit(new AuditAction()
        {
            ResourceName = "设备管理",
            EventType = "导入设备",
            Result = new AuditResult
            {
                Default = $"设备号列表：“{string.Join(",", equips.Select(d => d.EquipNo))}”导入成功"
            }
        });

        return OperateResult.Successed(workbook);
    }
}