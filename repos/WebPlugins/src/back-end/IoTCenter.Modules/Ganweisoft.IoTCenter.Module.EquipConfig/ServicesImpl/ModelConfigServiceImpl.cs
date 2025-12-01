// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Data;
using IoTCenter.Data.Model;
using IoTCenter.Utilities;
using IoTCenterCore.AutoMapper;
using IoTCenterWebApi.BaseCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.EquipConfig
{
    public class ModelConfigServiceImpl : IModelConfigService
    {
        private readonly string[] COMMANDTYPE = { "V", "X", "C", "S", "J" };
        private readonly ILoggingService _apiLog;
        private readonly GWDbContext _context;
        private readonly string equipName = "虚拟计算设备";

        private readonly Session _session;
        public ModelConfigServiceImpl(ILoggingService apiLog,
            Session session,
            GWDbContext context)
        {
            _session = session;
            _apiLog = apiLog;
            _context = context;
        }

        public OperateResult<PagedResult<AllEquipDataList>> GetAllEquipDataList(
            GetEquipDataListModel getAllEquipDataListModel)
        {
            if (getAllEquipDataListModel == null)
            {
                return OperateResult.Failed<PagedResult<AllEquipDataList>>("请求参数为空");
            }

            var equipName = getAllEquipDataListModel.EquipName;
            var equipNos = getAllEquipDataListModel.EquipNos;

            var query = (from a in _context.IotEquip.AsNoTracking()
                         where a.EquipNo > 0
                         select new AllEquipDataList
                         {
                             StaN = a.StaN,
                             EquipNo = a.EquipNo,
                             EquipNm = a.EquipNm,
                             EquipDetail = a.EquipDetail,
                             AccCyc = a.AccCyc,
                             ProcAdvice = a.ProcAdvice,
                             OutOfContact = a.OutOfContact,
                             Contacted = a.Contacted,
                             EventWav = a.EventWav,
                             CommunicationDrv = a.CommunicationDrv,
                             LocalAddr = a.LocalAddr,
                             EquipAddr = a.EquipAddr,
                             CommunicationParam = a.CommunicationParam,
                             CommunicationTimeParam = a.CommunicationTimeParam,
                             RawEquipNo = a.RawEquipNo,
                             Tabname = a.Tabname,
                             AlarmScheme = a.AlarmScheme,
                             Attrib = a.Attrib,
                             AlarmRiseCycle = a.AlarmRiseCycle,
                             StaIp = a.StaIp,
                             Reserve1 = a.Reserve1,
                             Reserve2 = a.Reserve2,
                             Reserve3 = a.Reserve3,
                             RelatedPic = a.RelatedPic,
                             RelatedVideo = a.RelatedVideo,
                             ZiChanId = a.ZiChanId,
                             PlanNo = a.PlanNo,
                             SafeTime = a.SafeTime,
                             Backup = a.Backup,
                             equipconntype = a.EquipConnType,
                             YxNum = a.IotYxps.Count(),
                             YcNum = a.IotYcps.Count(),
                             SetNum = a.IotSetParms.Count()
                         }).ToList();
            
            if (!string.IsNullOrEmpty(equipName))
            {
                query = query.Where(x => x.EquipNm.Contains(equipName)).ToList();
            }

            if (equipNos.Any())
            {
                query = query.Where(x => equipNos.Contains(x.EquipNo)).ToList();
            }

            var count = query.Count;

            if (getAllEquipDataListModel.PageNo != null && getAllEquipDataListModel.PageSize != null)
            {
                query = query.Skip((getAllEquipDataListModel.PageNo - 1).Value *
                                     getAllEquipDataListModel.PageSize.Value)
                    .Take(getAllEquipDataListModel.PageSize.Value).ToList();
            }

            return OperateResult.Successed(PagedResult<AllEquipDataList>.Create(count, query));
        }


        public async Task<OperateResult<IotEquip>> GetEquipDataByEquipNo(GetEquipDataByNoModel getEquipDataByNoModel)
        {
            if (getEquipDataByNoModel == null)
            {
                return OperateResult.Failed<IotEquip>("请求参数为空");
            }

            var staNo = getEquipDataByNoModel.StaNo;
            var equipNo = getEquipDataByNoModel.EquipNo;

            if (equipNo <= 0)
            {
                return OperateResult.Failed<IotEquip>("设备号不能小于0");
            }

            return OperateResult.Successed(
                await _context.IotEquip.FirstOrDefaultAsync(x => x.StaN == staNo && x.EquipNo == equipNo));
        }

        public async Task<OperateResult> AddEquipData(EquipDataModel equipDataModel)
        {
            if (equipDataModel == null)
            {
                return OperateResult.Failed("请求参数为空");
            }

            if (string.IsNullOrEmpty(equipDataModel.EquipNm))
            {
                return OperateResult.Failed("产品名称不能为空");
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

            if (equipDataModel.EquipConnType > 2 || equipDataModel.EquipConnType < 0)
            {
                return OperateResult.Failed("设备类型参数不正确");
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

            var filter = new CustomFilterAttribute(_apiLog);
            if (!filter.CheckStringForSql(new string[] { equipDataModel.LocalAddr }))
            {
                return OperateResult.Failed("请求参数存在非法字符!");
            }

            var staNo = equipDataModel.StaNo;
            var equipNm = equipDataModel.EquipNm;
            var outOfContact = equipDataModel.OutOfContact;

            if (await _context.IotEquip.AnyAsync(x => x.EquipNm == equipNm))
            {
                return OperateResult.Failed("产品名称已经存在");
            }

            var maxNo = (await _context.IotEquip.MaxAsync(x => (int?)x.EquipNo) ?? 0) + 1;

            var model = equipDataModel.MapTo<IotEquip>();
            model.EquipNo = maxNo;
            model.StaN = equipDataModel.StaNo <= 0 ? 1 : equipDataModel.StaNo;

            await _context.IotEquip.AddAsync(model);

            if (await _context.SaveChangesAsync() <= 0)
            {
                return OperateResult.Failed("操作失败，请联系系统管理员！");
            }

            await _apiLog.Audit(new AuditAction()
            {
                ResourceName = "产品管理",
                EventType = "新增产品",
                Result = new AuditResult
                {
                    Default = $"新增产品名称“{model.EquipNm}”成功"
                }
            });

            return OperateResult.Successed(maxNo);
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
                return OperateResult.Failed("产品名称不能为空");
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

            if (equipDataModel.EquipConnType > 2 || equipDataModel.EquipConnType < 0)
            {
                return OperateResult.Failed("设备类型参数不正确");
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

            CustomFilterAttribute filter = new CustomFilterAttribute(_apiLog);
            if (!filter.CheckStringForSql(new string[] { equipDataModel.Tabname }) || filter.CheckKeyWord(equipDataModel.Tabname))
            {
                return OperateResult.Failed("请求参数存在非法字符!");
            }

            var result = await _context.IotEquip.AsNoTracking().FirstOrDefaultAsync(x =>
                x.StaN == equipDataModel.StaNo && x.EquipNo == equipDataModel.EquipNo);
            if (result == null)
            {
                return OperateResult.Failed("产品不存在");
            }

            if (await _context.IotEquip.Where(x => x.EquipNo != equipDataModel.EquipNo).AnyAsync(x =>
                         x.StaN == equipDataModel.StaNo && x.EquipNm == equipDataModel.EquipNm))
            {
                return OperateResult.Failed("产品名称已存在");
            }

            var model = equipDataModel.MapTo<IotEquip>();
            model.StaN = equipDataModel.StaNo <= 0 ? 1 : equipDataModel.StaNo;

            _context.IotEquip.Update(model);

            await _context.SaveChangesAsync();

            await _apiLog.Audit(new AuditAction()
            {
                ResourceName = "产品管理",
                EventType = "编辑产品",
                Result = new AuditResult<object, object>()
                {
                    Default = "编辑成功",
                    Old = equipDataModel,
                    New = result
                }
            });

            return OperateResult.Success;
        }

        public async Task<OperateResult> DelEquipData(DelEquipDataModel delEquipDataModel)
        {
            var equip = await _context.IotEquip.FirstOrDefaultAsync(x =>
                x.EquipNo == delEquipDataModel.EquipNo && x.StaN == delEquipDataModel.StaNo);
            if (equip != null)
            {
                _context.IotEquip.Remove(equip);
            }

            var ycp = await _context.IotYcp.Where(x => x.EquipNo == delEquipDataModel.EquipNo
                                                       && x.StaN == delEquipDataModel.StaNo).ToArrayAsync();
            if (ycp != null)
            {
                _context.IotYcp.RemoveRange(ycp);
            }

            var yxp = await _context.IotYxp
                .Where(x => x.EquipNo == delEquipDataModel.EquipNo && x.StaN == delEquipDataModel.StaNo).ToArrayAsync();
            if (yxp != null)
            {
                _context.IotYxp.RemoveRange(yxp);
            }

            var set = await _context.IotSetParm
                .Where(x => x.EquipNo == delEquipDataModel.EquipNo && x.StaN == delEquipDataModel.StaNo).ToArrayAsync();
            if (set != null)
            {
                _context.IotSetParm.RemoveRange(set);
            }

            await _context.SaveChangesAsync();

            await _apiLog.Audit(new AuditAction()
            {
                ResourceName = "产品管理",
                EventType = "删除产品",
                Result = new AuditResult()
                {
                    Default = $"删除产品名称“{equip.EquipNm}”成功"
                }
            });

            return OperateResult.Success;
        }

        public async Task<OperateResult<PagedResult<IotYcp>>> GetYcpDataList(
            GetYcYxSetDataListModel getYcYxSetDataListModel)
        {
            if (getYcYxSetDataListModel == null)
                return OperateResult.Failed<PagedResult<IotYcp>>("请求参数为空");

            int equipNo = getYcYxSetDataListModel.EquipNo;
            string ycName = getYcYxSetDataListModel.SearchName;

            var query = _context.IotYcp.AsQueryable().Where(d => d.EquipNo == equipNo);

            if (!string.IsNullOrEmpty(ycName))
            {
                query = query.Where(x => x.YcNm.Contains(ycName));
            }

            var list = await query.ToListAsync();
            var count = list.Count;

            var result = list.Skip((getYcYxSetDataListModel.PageNo - 1) * getYcYxSetDataListModel.PageSize)
                .Take(getYcYxSetDataListModel.PageSize);

            return OperateResult.Successed(PagedResult<IotYcp>.Create(count, result));
        }

        public async Task<OperateResult<IotYcp>> GetYcpDataByEquipYcNo(EquipYcYxSetNoModel equipYcYxSetNoModel)
        {
            return OperateResult.Successed(await _context.IotYcp.FirstOrDefaultAsync(x =>
                x.StaN == equipYcYxSetNoModel.StaNo &&
                x.EquipNo == equipYcYxSetNoModel.EquipNo &&
                x.YcNo == equipYcYxSetNoModel.YcyxSetpNo));
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

            if (await _context.IotYcp.AnyAsync(x =>
                x.StaN == ycpDataModel.StaNo &&
                x.EquipNo == ycpDataModel.EquipNo &&
                x.YcNo == ycpDataModel.YcNo))
            {
                return OperateResult.Failed("模拟量已经存在");
            }

            if (ExistsCode(null, ycpDataModel.YcCode, "yc", ycpDataModel.EquipNo))
            {
                return OperateResult.Failed("遥测编码已经存在");
            }

            var ycs = await _context.IotYcp.Where(d => d.EquipNo == ycpDataModel.EquipNo).ToListAsync();
            var ycNo = ycs.Any() ? ycs.Max(d => d.YcNo) + 1 : 1;

            var iotYcp = ycpDataModel.MapTo<IotYcp>();
            iotYcp.StaN = 1;

            await _context.IotYcp.AddAsync(iotYcp);
            if (await _context.SaveChangesAsync() <= 0)
            {
                return OperateResult.Failed("操作失败，请联系系统管理员！");
            }

            await _apiLog.Audit(new AuditAction()
            {
                ResourceName = "产品管理",
                EventType = "新增模拟量",
                Result = new AuditResult<object, object>()
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

            string[] ycDataType = new string[] { "S", "SEQ1", "SEQ2", "SEQ3", "SEQ4", "SEQ5", "SEQ6", "SEQ7" };

            if (ycpDataModel.DataType != null)
            {
                if (!ycDataType.Contains(ycpDataModel.DataType))
                {
                    return OperateResult.Failed("数据类型参数错误");
                }
            }

            if (!await _context.IotYcp.AnyAsync(x => x.StaN == ycpDataModel.StaNo &&
                                                     x.EquipNo == ycpDataModel.EquipNo && x.YcNo == ycpDataModel.YcNo))
            {
                return OperateResult.Failed("模拟量不存在");
            }

            if (ExistsCode(ycpDataModel.YcNo, ycpDataModel.YcCode, "yc", ycpDataModel.EquipNo))
            {
                return OperateResult.Failed("遥测编码已经存在");
            }

            List<string> YcNm = _context.IotYcp.Where(x => x.StaN == ycpDataModel.StaNo &&
                                                   x.EquipNo == ycpDataModel.EquipNo && x.YcNo == ycpDataModel.YcNo).Select(x => x.YcNm).ToList();
            var old = YcNm;

            var iotYcp = ycpDataModel.MapTo<IotYcp>();
            iotYcp.StaN = 1;

            _context.IotYcp.Update(iotYcp);
            await _context.SaveChangesAsync();

            await _apiLog.Audit(new AuditAction()
            {
                ResourceName = "产品管理",
                EventType = "编辑模拟量",
                Result = new AuditResult<object, object>() { Default = "编辑成功", Old = old, New = YcNm }
            });

            return OperateResult.Success;
        }

        public async Task<OperateResult> DelYcpData(DelYcYxSetDataModel delYcYxSetDataModel)
        {
            if (delYcYxSetDataModel == null)
            {
                return OperateResult.Failed("请求参数为空");
            }

            var tmp = await _context.IotYcp.FirstOrDefaultAsync(
                x => x.StaN == delYcYxSetDataModel.StaNo && x.EquipNo == delYcYxSetDataModel.EquipNo &&
                     x.YcNo == delYcYxSetDataModel.YcYxSetNo
            );

            if (tmp != null)
            {
                _context.IotYcp.Remove(tmp);

                await _context.SaveChangesAsync();
            }

            await _apiLog.Audit(new AuditAction()
            {
                ResourceName = "产品管理",
                EventType = "删除模拟量",
                Result = new AuditResult<object, object>()
                {
                    Default = $"删除模拟量：({delYcYxSetDataModel.YcYxSetNo})成功，产品号：({delYcYxSetDataModel.EquipNo})"
                }
            });

            return OperateResult.Success;
        }

        public async Task<OperateResult<PagedResult<IotYxp>>> GetYxpDataList(
            GetYcYxSetDataListModel getYcYxSetDataListModel)
        {
            if (getYcYxSetDataListModel == null)
                return OperateResult.Failed<PagedResult<IotYxp>>("请求参数为空");

            int pageNo = getYcYxSetDataListModel.PageNo;
            int pageSize = getYcYxSetDataListModel.PageSize;
            int equipNo = getYcYxSetDataListModel.EquipNo;
            string yxName = getYcYxSetDataListModel.SearchName;

            var query = _context.IotYxp.Where(d => d.EquipNo == equipNo);
            if (!string.IsNullOrEmpty(yxName))
            {
                query = query.Where(x => x.YxNm.Contains(yxName));
            }

            var list = await query.ToListAsync();
            var count = query.Count();
            var result = list.Skip((pageNo - 1) * pageSize).Take(pageSize);

            return OperateResult.Successed(PagedResult<IotYxp>.Create(count, result));
        }

        public async Task<OperateResult<IotYxp>> GetYxpDataByEquipYxNo(EquipYcYxSetNoModel equipYcYxSetNoModel)
        {
            var iotYxp = await _context.IotYxp.FirstOrDefaultAsync(x => x.StaN == equipYcYxSetNoModel.StaNo &&
                                                                        x.EquipNo == equipYcYxSetNoModel.EquipNo &&
                                                                        x.YxNo == equipYcYxSetNoModel.YcyxSetpNo);

            return OperateResult.Successed(iotYxp);
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

            if (await _context.IotYxp.AsNoTracking().AnyAsync(x => x.StaN == yxpDataModel.StaNo &&
                                                    x.EquipNo == yxpDataModel.EquipNo && x.YxNo == yxpDataModel.YxNo))
            {
                return OperateResult.Failed("模拟量已经存在");
            }

            if (ExistsCode(null, yxpDataModel.YxCode, "yx", yxpDataModel.EquipNo))
            {
                return OperateResult.Failed("遥信编码已经存在");
            }

            var yxs = await _context.IotYxp.AsNoTracking().Where(d => d.EquipNo == yxpDataModel.EquipNo).ToListAsync();

            var yxNo = yxs.Any() ? yxs.Max(d => d.YxNo) + 1 : 1;

            var iotYxp = yxpDataModel.MapTo<IotYxp>();
            iotYxp.StaN = 1;

            await _context.IotYxp.AddAsync(iotYxp);

            if (await _context.SaveChangesAsync() <= 0)
            {
                return OperateResult.Failed("操作失败，请联系系统管理员！");
            }

            await _apiLog.Audit(new AuditAction()
            {
                ResourceName = "产品管理",
                EventType = "新增模拟量",
                Result = new AuditResult<object, object>()
                {
                    Default = "新增成功",
                    New = yxpDataModel
                }
            });

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

            var dbYxp = await _context.IotYxp.AsNoTracking().FirstOrDefaultAsync(x =>
                x.StaN == yxpDataModel.StaNo && x.EquipNo == yxpDataModel.EquipNo && x.YxNo == yxpDataModel.YxNo);

            if (dbYxp == null)
            {
                return OperateResult.Failed("模拟量不存在");
            }

            if (ExistsCode(yxpDataModel.YxNo, yxpDataModel.YxCode, "yx", yxpDataModel.EquipNo))
            {
                return OperateResult.Failed("遥信编码已经存在");
            }

            var yxpModel = yxpDataModel.MapTo<IotYxp>();
            yxpModel.StaN = 1;

            _context.IotYxp.Update(yxpModel);

            await _context.SaveChangesAsync();

            await _apiLog.Audit(new AuditAction()
            {
                ResourceName = "产品管理",
                EventType = "编辑模拟量",
                Result = new AuditResult<object, object>()
                {
                    Default = "编辑成功",
                    Old = dbYxp,
                    New = yxpDataModel
                }
            });

            return OperateResult.Success;
        }

        public async Task<OperateResult> DelYxpData(DelYcYxSetDataModel delYcYxSetDataModel)
        {
            if (delYcYxSetDataModel == null)
            {
                return OperateResult.Failed("请求参数为空");
            }

            var tmp = await _context.IotYxp.FirstOrDefaultAsync(x =>
                x.StaN == delYcYxSetDataModel.StaNo &&
                x.EquipNo == delYcYxSetDataModel.EquipNo &&
                x.YxNo == delYcYxSetDataModel.YcYxSetNo);

            if (tmp != null)
            {
                _context.IotYxp.Remove(tmp);

                await _context.SaveChangesAsync();
            }

            var equipNo = delYcYxSetDataModel.EquipNo;
            var yxNo = delYcYxSetDataModel.YcYxSetNo;

            await _apiLog.Audit(new AuditAction()
            {
                ResourceName = "产品管理",
                EventType = "删除模拟量",
                Result = new AuditResult() { Default = $"删除模拟量：({yxNo})成功，产品号：({equipNo})" }
            });

            return OperateResult.Success;
        }

        public async Task<OperateResult<PagedResult<IotSetParm>>> GetSetParmDataList(
            GetYcYxSetDataListModel getYcYxSetDataListModel)
        {
            if (getYcYxSetDataListModel == null)
            {
                return OperateResult.Failed<PagedResult<IotSetParm>>("请求参数为空");
            }

            int pageNo = getYcYxSetDataListModel.PageNo;
            int pageSize = getYcYxSetDataListModel.PageSize;
            int equipNo = getYcYxSetDataListModel.EquipNo;
            string setName = getYcYxSetDataListModel.SearchName;

            var query = _context.IotSetParm.AsQueryable().Where(d => d.EquipNo == equipNo);

            if (!string.IsNullOrEmpty(setName))
            {
                query = query.Where(x => x.SetNm.Contains(setName));
            }

            var count = await query.CountAsync();
            var result = await query.Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

            return OperateResult.Successed(PagedResult<IotSetParm>.Create(count, result));
        }

        public async Task<OperateResult<IotSetParm>> GetSetParmDataByEquipSetNo(EquipYcYxSetNoModel equipYcYxSetNoModel)
        {
            var iotsetparm = await _context.IotSetParm.FirstOrDefaultAsync(
                x => x.StaN == equipYcYxSetNoModel.StaNo && x.EquipNo == equipYcYxSetNoModel.EquipNo &&
                     x.SetNo == equipYcYxSetNoModel.YcyxSetpNo
            );

            return OperateResult.Successed(iotsetparm);
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

            if (!this.COMMANDTYPE.Contains(setDataModel.SetType))
            {
                return OperateResult.Failed("输入的设置类型不合法.");
            }

            if (await _context.IotSetParm.AnyAsync(x =>
                x.StaN == setDataModel.StaNo &&
                x.EquipNo == setDataModel.EquipNo &&
                x.SetNo == setDataModel.SetNo))
            {
                return OperateResult.Failed("模拟量已经存在");
            }
            if (ExistsCode(null, setDataModel.SetCode, "set", setDataModel.EquipNo))
            {
                return OperateResult.Failed("模拟量Code已经存在");
            }

            var sets = await _context.IotSetParm.Where(d => d.EquipNo == setDataModel.EquipNo).ToListAsync();
            var setNo = sets.Any() ? sets.Max(d => d.SetNo) + 1 : 1;

            var setParm = setDataModel.MapTo<IotSetParm>();
            setParm.StaN = 1;

            await _context.IotSetParm.AddAsync(setParm);
            if (await _context.SaveChangesAsync() <= 0)
            {
                return OperateResult.Failed("操作失败，请联系系统管理员！");
            }

            await _apiLog.Audit(new AuditAction()
            {
                ResourceName = "产品管理",
                EventType = "新增设置",
                Result = new AuditResult<object, object>() { Default = "新增成功", New = setDataModel }
            });

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

            if (!this.COMMANDTYPE.Contains(setDataModel.SetType))
            {
                return OperateResult.Failed("输入的设置类型不合法.");
            }

            var dbSet = await _context.IotSetParm.AsNoTracking()
                .FirstOrDefaultAsync(x =>
                x.StaN == setDataModel.StaNo &&
                x.EquipNo == setDataModel.EquipNo &&
                x.SetNo == setDataModel.SetNo);

            if (dbSet == null)
            {
                return OperateResult.Failed("配置不存在");
            }

            if (ExistsCode(setDataModel.SetNo, setDataModel.SetCode, "set", setDataModel.EquipNo))
            {
                return OperateResult.Failed("模拟量Code已经存在");
            }

            var iotSet = setDataModel.MapTo<IotSetParm>();
            iotSet.StaN = 1;

            _context.IotSetParm.Update(iotSet);

            List<string> model = _context.IotSetParm.Where(x =>
                x.StaN == setDataModel.StaNo &&
                x.EquipNo == setDataModel.EquipNo &&
                x.SetNo == setDataModel.SetNo).Select(x => x.SetNm).ToList();

            await _context.SaveChangesAsync();

            await _apiLog.Audit(new AuditAction()
            {
                ResourceName = "产品管理",
                EventType = "编辑设置",
                Result = new AuditResult<object, object>() { Default = "编辑成功", Old = dbSet, New = setDataModel }
            });

            return OperateResult.Success;
        }

        public async Task<OperateResult> DelSetData(DelYcYxSetDataModel delYcYxSetDataModel)
        {
            if (delYcYxSetDataModel == null)
            {
                return OperateResult.Failed("请求参数为空");
            }

            long equipNo = delYcYxSetDataModel.EquipNo;
            long setNo = delYcYxSetDataModel.YcYxSetNo;

            var tmp = await _context.IotSetParm.FirstOrDefaultAsync(x =>
                x.StaN == delYcYxSetDataModel.StaNo &&
                x.EquipNo == delYcYxSetDataModel.EquipNo &&
                x.SetNo == delYcYxSetDataModel.YcYxSetNo);

            if (tmp != null)
            {
                _context.IotSetParm.Remove(tmp);

                await _context.SaveChangesAsync();

                await _apiLog.Audit(new AuditAction()
                {
                    ResourceName = "产品管理",
                    EventType = "删除设置",
                    Result = new AuditResult() { Default = $"删除设置号：({setNo})成功，产品号：({equipNo})" }
                });
            }

            return OperateResult.Success;
        }

        public async Task<OperateResult> GetYcYxSetNumByEquipNo(int equipNo)
        {
            var result = await _context.IotEquip.AsNoTracking()
                .Include(d => d.IotYcps)
                .Include(d => d.IotYxps)
                .Include(d => d.IotSetParms)
                .Where(d => d.EquipNo == equipNo)
                .Select(d => new
                {
                    YcNum = d.IotYcps.Count,
                    YxNum = d.IotYxps.Count,
                    SetNum = d.IotSetParms.Count
                })
                .FirstOrDefaultAsync();

            if (result == null)
            {
                return OperateResult.Failed("产品不存在");
            }

            return OperateResult.Successed(result);
        }

        public async Task<OperateResult<PageResult<SubsystemTypeEquip>>> GetSubsystemTypeEquipsAsync(QueryRequest request)
        {
            var query = from e in _context.Equip.AsNoTracking()
                        join i in _context.IotEquip.AsNoTracking().Where(ie => ie.EquipConnType == 1)
                            on e.RawEquipNo equals i.EquipNo
                        select new SubsystemTypeEquip
                        {
                            EquipNo = e.EquipNo,
                            EquipNm = e.EquipNm
                        };

            var count = await query.CountAsync();
            if (request.PageNo.HasValue && request.PageSize.HasValue)
            {
                query = query.Skip((request.PageNo.Value - 1) * request.PageSize.Value).Take(request.PageSize.Value);
            }

            var result = await query.ToListAsync();
            return OperateResult.Successed(PageResult<SubsystemTypeEquip>.Create(count, result));
        }

        private bool ExistsCode(long? ycYxSetNo, string code, string type, long equipNo)
        {
            bool exists = false;
            if (string.IsNullOrEmpty(code)) return exists;
            switch (type)
            {
                case "yc":
                    exists = _context.IotYcp.Any(o => o.YcCode == code && o.EquipNo == equipNo && (ycYxSetNo.HasValue ? o.YcNo != ycYxSetNo.Value : true));
                    break;
                case "yx":
                    exists = _context.IotYxp.Any(o => o.YxCode == code && o.EquipNo == equipNo && (ycYxSetNo.HasValue ? o.YxNo != ycYxSetNo.Value : true));
                    break;
                case "set":
                    exists = _context.IotSetParm.Any(o => o.SetCode == code && o.EquipNo == equipNo && (ycYxSetNo.HasValue ? o.SetNo != ycYxSetNo.Value : true));
                    break;
            }
            return exists;
        }
    }
}