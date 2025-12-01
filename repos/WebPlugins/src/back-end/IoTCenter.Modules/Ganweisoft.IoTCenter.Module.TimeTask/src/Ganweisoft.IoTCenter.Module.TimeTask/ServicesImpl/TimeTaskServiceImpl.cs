// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Ganweisoft.IoTCenter.Module.TimeTask.Controllers;
using Ganweisoft.IoTCenter.Module.TimeTask.Models;
using IoTCenter.Data.Model;
using IoTCenter.Utilities;
using IoTCenter.Utilities.Extensions;
using IoTCenterHost.Core.Abstraction.Services;
using IoTCenterWebApi.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.TimeTask
{
    public class TimeTaskServiceImpl : ITimeTaskService
    {
        private readonly ILoggingService _apiLog;
        private readonly TimeTaskDbContext _context;
        private readonly IStringLocalizer<TimeTaskController> _stringLocalizer;

        private readonly IotCenterHostService _proxy;

        public TimeTaskServiceImpl(
            ILoggingService apiLog,
            IotCenterHostService proxy,
            TimeTaskDbContext context,
            IStringLocalizer<TimeTaskController> stringLocalizer)
        {
            _apiLog = apiLog;
            _context = context;
            _proxy = proxy;
            _stringLocalizer = stringLocalizer;
        }

        #region new api

        public async Task<OperateResult<PagedResult<AllTimeTaskListDto>>> GetAllTaskDataList(CommonSearchPageModel commonSearchPageModel)
        {
            if (commonSearchPageModel == null)
            {
                return OperateResult.Failed<PagedResult<AllTimeTaskListDto>>(_stringLocalizer["请求参数为空，请检查"]);
            }

            var searchName = commonSearchPageModel.SearchName;

            var commonTaskQuery = _context.GwProcTimeTlist.AsNoTracking();

            var cycleTaskQuery = _context.GwProcCycleTlist.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(commonSearchPageModel.SearchName))
            {
                commonTaskQuery = commonTaskQuery.Where(d =>
                    d.TableName.Contains(searchName));

                cycleTaskQuery = cycleTaskQuery.Where(d =>
                    d.TableName.Contains(searchName));
            }

            var commonTasks = commonTaskQuery.Select(d =>
                new AllTimeTaskListDto()
                {
                    TableName = d.TableName,
                    TableId = d.TableId,
                    TableType = TaskType.T.ToString(),
                    Remark = d.Comment
                });

            var cycleTasks = cycleTaskQuery.Select(d =>
                new AllTimeTaskListDto()
                {
                    TableName = d.TableName,
                    TableId = d.TableId,
                    TableType = TaskType.C.ToString(),
                    Remark = d.Reserve1
                });

            var taskList = await commonTasks.Union(cycleTasks).ToListAsync();

            taskList = taskList.OrderBy(d => d.TableType, new CompareTaskList()).ThenByDescending(d => d.TableId).ToList();

            var total = taskList.Count;

            var pageResult = PagedResult<AllTimeTaskListDto>.Create(total, taskList);

            return OperateResult.Successed(pageResult);
        }

        public async Task<OperateResult<CommonTaskData>> GetCommonTaskData(int? tableId)
        {
            var gwprocTimeTlistQuery = _context.GwProcTimeTlist.AsNoTracking()
                .Where(d => d.TableId == tableId);

            var gwprocTimeEqpTableList = await (from equipTask in _context.GwProcTimeEqpTable.AsNoTracking()
                                                from equip in _context.Equip.AsNoTracking()
                                                    .Where(e => equipTask.EquipNo == e.EquipNo)
                                                    .DefaultIfEmpty()
                                                from set in _context.SetParm.AsNoTracking()
                                                    .Where(s => equipTask.SetNo == s.SetNo && equipTask.EquipNo == s.EquipNo)
                                                    .DefaultIfEmpty()
                                                where equipTask.TableId == tableId
                                                select new ProcTaskEqpConfig()
                                                {
                                                    Time = equipTask.Time.ToString("yyyy-MM-dd HH:mm:ss"),
                                                    TimeDur = equipTask.TimeDur.ToString("yyyy-MM-dd HH:mm:ss"),
                                                    EquipNo = equipTask.EquipNo,
                                                    SetNo = equipTask.SetNo,
                                                    Value = equipTask.Value,
                                                    EquipSetNm = $"{equip.EquipNm}-{set.SetNm}",
                                                    ProcessOrder = equipTask.ProcessOrder,
                                                    Id = equipTask.Id,
                                                    NoEdit = set.SetType == "X"
                                                }).ToListAsync();


            var gwprocTimeSysTableList = await _context.GwProcTimeSysTable
                .AsNoTracking().Where(d => d.TableId == tableId)
                .Select(d => new ProcTaskSysConfig()
                {
                    Id = d.Id,
                    Time = d.Time.ToString("yyyy-MM-dd HH:mm:ss"),
                    TimeDur = d.TimeDur.ToString("yyyy-MM-dd HH:mm:ss"),
                    ProcCode = d.ProcCode,
                    CmdNm = d.CmdNm,
                }).ToListAsync();

            var responseDto = await gwprocTimeTlistQuery.Select(d =>
                new CommonTaskData()
                {
                    TableId = d.TableId,
                    TableName = d.TableName,
                    Comment = d.Comment,
                    procTaskEqp = gwprocTimeEqpTableList,
                    procTaskSys = gwprocTimeSysTableList
                }).FirstOrDefaultAsync();

            return OperateResult.Successed(responseDto);
        }


        public async Task<OperateResult<CycleTaskData>> GetCycleTaskData(int? tableId)
        {
            var gwProcCycleTableList = await _context.GwProcCycleTable.AsNoTracking()
                .Where(d => d.TableId == tableId)
                .Select(d =>
                    new CycleTaskContentList()
                    {
                        DoOrder = d.DoOrder,
                        Type = d.Type,
                        EquipNo = d.EquipNo,
                        SetNo = d.SetNo,
                        Value = d.Value,
                        EquipSetNm = d.EquipSetNm,
                        ProcCode = d.ProcCode,
                        CmdNm = d.CmdNm,
                        SleepTime = d.SleepTime,
                        SleepUnit = d.SleepUnit,
                    }).ToListAsync();

            var responseDto = await _context.GwProcCycleTlist
                .AsNoTracking()
                .Where(d => d.TableId == tableId)
                .Select(d =>
                new CycleTaskData()
                {
                    TableId = d.TableId,
                    TableName = d.TableName,
                    Comment = d.Reserve1,
                    cycleTask = new CycleTaskConfig()
                    {
                        BeginTime = d.BeginTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        EndTime = d.EndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        ZhenDianDo = d.ZhenDianDo,
                        ZhidingDo = d.ZhidingDo,
                        ZhidingTime = d.ZhidingTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        CycleMustFinish = d.CycleMustFinish,
                        MaxCycleNum = d.MaxCycleNum,
                    },
                    cycleTaskContent = gwProcCycleTableList
                }).FirstOrDefaultAsync();

            return OperateResult.Successed(responseDto);
        }

        public async Task<(bool, string)> CheckTaskSys(IEnumerable<ProcTaskSysConfig> commonTaskSysList)
        {

            var gwProcGroups = commonTaskSysList.GroupBy(x => x.ProcCode)
                .Select(x => new
                {
                    procCode = x.Key,
                    cmdNms = x.Select(j => j.CmdNm).Distinct()
                })
                .ToArray();
            var procCodes = gwProcGroups.Select(x => x.procCode);

            if (procCodes.HasValues())
            {
                var queryCount = await _context.GwexProc.AsNoTracking().CountAsync(x => procCodes.Contains(x.ProcCode));
                if (queryCount != procCodes.Count())
                {
                    return (false, _stringLocalizer["请配置正确的系统控制项"]);
                }
            }

            foreach (var gwProc in gwProcGroups)
            {
                if (!await _context.GwexProcCmd.AsNoTracking().AnyAsync(x => x.ProcCode == gwProc.procCode && gwProc.cmdNms.Contains(x.CmdNm)))
                {
                    return (false, _stringLocalizer["请配置正确的系统控制命令项"]);
                }
            }

            return (true, string.Empty);
        }

        public async Task<(bool, string)> CheckTaskSys(IEnumerable<CycleTaskContentList> cycleTaskContentLists)
        {

            var gwProcGroups = cycleTaskContentLists.GroupBy(x => x.ProcCode)
                .Select(x => new
                {
                    procCode = x.Key,
                    cmdNms = x.Select(j => j.CmdNm).Distinct()
                })
                .ToArray();
            var procCodes = gwProcGroups.Select(x => x.procCode);

            if (procCodes.HasValues())
            {
                var queryCount = await _context.GwexProc.AsNoTracking().CountAsync(x => procCodes.Contains(x.ProcCode));
                if (queryCount != procCodes.Count())
                {
                    return (false, _stringLocalizer["请配置正确的系统控制项"]);
                }
            }

            foreach (var gwProc in gwProcGroups)
            {
                if (!await _context.GwexProcCmd.AsNoTracking().AnyAsync(x => x.ProcCode == gwProc.procCode && gwProc.cmdNms.Contains(x.CmdNm)))
                {
                    return (false, _stringLocalizer["请配置正确的系统控制命令项"]);
                }
            }

            return (true, string.Empty);
        }

        public async Task<OperateResult> CreateCommonTask(AddCommonTaskModel addCommonTaskModel)
        {
            var commonTaskSysList = addCommonTaskModel.ProcTaskSys;
            var commonTaskEqpList = addCommonTaskModel.ProcTaskEqp;

            var checkResult = await CheckTaskSys(commonTaskSysList);
            if (!checkResult.Item1)
            {
                return OperateResult.Failed(checkResult.Item2);
            }
            if (addCommonTaskModel.Comment.HasValue() && addCommonTaskModel.Comment.Length > 255)
            {
                return OperateResult.Failed(string.Format(_stringLocalizer["描述的最长字符限制为{0}"], 255));
            }

            var model = new GwprocTimeTlist()
            {
                TableName = addCommonTaskModel.TableName,
                Comment = addCommonTaskModel.Comment
            };

            await _context.GwProcTimeTlist.AddAsync(model);
            try
            {
                if (await _context.SaveChangesAsync() <= 0)
                {
                    return OperateResult.Failed(_stringLocalizer["添加普通任务失败"]);
                }
                var insertId = model.TableId;

                bool isAdditem = false;

                List<(GwprocTimeEqpTable data, bool isCanRun)> eqEntityModelList = commonTaskEqpList.Select(data => (new GwprocTimeEqpTable
                {
                    TableId = insertId,
                    Time = Convert.ToDateTime(data.Time),
                    TimeDur = Convert.ToDateTime(data.TimeDur),
                    EquipNo = data.EquipNo,
                    SetNo = data.SetNo,
                    Value = data.Value,
                    EquipSetNm = data.EquipSetNm,
                    ProcessOrder = data.ProcessOrder,
                }, data.IsCanRun)).ToList();
                if (eqEntityModelList.HasValues())
                {
                    isAdditem = true;
                    await _context.GwProcTimeEqpTable.AddRangeAsync(eqEntityModelList.Select(x => x.data));
                }

                List<(GwprocTimeSysTable data, bool isCanRun)> sysEntityModelList = commonTaskSysList.Select(data => (new GwprocTimeSysTable()
                {
                    TableId = insertId,
                    Time = Convert.ToDateTime(data.Time),
                    TimeDur = Convert.ToDateTime(data.TimeDur),
                    CmdNm = data.CmdNm,
                    ProcCode = data.ProcCode,
                    ProcessOrder = data.ProcessOrder,
                }, data.IsCanRun)).ToList();
                if (sysEntityModelList.HasValues())
                {
                    isAdditem = true;
                    await _context.GwProcTimeSysTable.AddRangeAsync(sysEntityModelList.Select(x => x.data));
                }

                if (isAdditem && await _context.SaveChangesAsync() <= 0)
                {
                    return OperateResult.Failed(_stringLocalizer["添加普通任务失败"]);
                }

                ResetTimeTaskManage();

                return OperateResult.Success;
            }
            catch (Exception e)
            {
                _apiLog.Error($"[添加普通任务] 出现异常：{e}");
                return OperateResult.Failed(_stringLocalizer["添加普通任务失败"] + $":{e.Message}");
            }
        }

        public async Task<OperateResult> CreateCycleTask(AddCycleTaskModel addCycleTaskModel)
        {
            if (addCycleTaskModel == null)
            {
                return OperateResult.Failed(_stringLocalizer["请求参数为空，请检查"]);
            }

            if (addCycleTaskModel == null || addCycleTaskModel.CycleTask == null)
            {
                return OperateResult.Failed(_stringLocalizer["请求参数不完整，请检查"]);
            }

            var checkResult = await CheckTaskSys(addCycleTaskModel.CycleTaskContent.Where(x => x.Type == "S"));
            if (!checkResult.Item1)
            {
                return OperateResult.Failed(checkResult.Item2);
            }

            var model = new GwprocCycleTlist()
            {
                TableName = addCycleTaskModel.TableName,
                BeginTime = Convert.ToDateTime(addCycleTaskModel.CycleTask.BeginTime),
                EndTime = Convert.ToDateTime(addCycleTaskModel.CycleTask.EndTime),
                ZhenDianDo = addCycleTaskModel.CycleTask.ZhenDianDo,
                ZhidingDo = addCycleTaskModel.CycleTask.ZhidingDo,
                ZhidingTime = Convert.ToDateTime(addCycleTaskModel.CycleTask.ZhidingTime),
                CycleMustFinish = addCycleTaskModel.CycleTask.CycleMustFinish,
                MaxCycleNum = addCycleTaskModel.CycleTask.MaxCycleNum,
                Reserve1 = addCycleTaskModel.Comment
            };

            if (!await _context.GwProcCycleTlist.AnyAsync())
            {
                model.TableId = 10000;
            }

            await _context.GwProcCycleTlist.AddAsync(model);
            try
            {
                if (await _context.SaveChangesAsync() <= 0)
                {
                    return OperateResult.Failed(_stringLocalizer["添加循环任务失败"]);
                }

                var insertId = model.TableId;

                if (addCycleTaskModel.CycleTaskContent.HasValues())
                {
                    int doOrderNum = 0;
                    var cycleEntityModelList = addCycleTaskModel.CycleTaskContent.Select(d =>
                        new GwprocCycleTable()
                        {
                            TableId = insertId,
                            DoOrder = ++doOrderNum,
                            Type = d.Type,
                            EquipNo = d.EquipNo,
                            SetNo = d.SetNo,
                            Value = d.Value,
                            EquipSetNm = d.EquipSetNm,
                            ProcCode = d.ProcCode,
                            CmdNm = d.CmdNm,
                            SleepTime = d.SleepTime,
                            SleepUnit = d.SleepUnit,
                            ProcessOrder = d.ProcessOrder,
                        });

                    await _context.GwProcCycleTable.AddRangeAsync(cycleEntityModelList);


                    if (await _context.SaveChangesAsync() <= 0)
                    {
                        return OperateResult.Failed(_stringLocalizer["添加循环任务失败"]);
                    }

                    ResetTimeTaskManage();
                }


                return OperateResult.Success;
            }
            catch (Exception e)
            {
                _apiLog.Error($"[添加循环任务失败] 出现异常：{e}");
                return OperateResult.Failed(_stringLocalizer["添加循环任务失败"] + $":{e.Message}");
            }
        }


        public async Task<OperateResult> EditCommonTaskData(EditCommonTaskModel editCommonTaskModel)
        {

            var tableId = editCommonTaskModel.TableId;

            var tableName = editCommonTaskModel.TableName;

            var comment = editCommonTaskModel.Comment;
            if (editCommonTaskModel.Comment.HasValue() && editCommonTaskModel.Comment.Length > 255)
            {
                return OperateResult.Failed(string.Format(_stringLocalizer["描述的最长字符限制为{0}"], 255));
            }


            var commonTaskSysList = editCommonTaskModel.ProcTaskSys;
            var commonTaskEqpList = editCommonTaskModel.ProcTaskEqp;

            if (commonTaskSysList.Where(x => x.Id.HasValue).GroupBy(x => x.Id).Where(x => x.Count() > 1).HasValues())
            {
                return OperateResult.Failed(_stringLocalizer["系统控制项参数不正确，存在重复数据"]);
            }
            if (commonTaskEqpList.Where(x => x.Id.HasValue).GroupBy(x => x.Id).Where(x => x.Count() > 1).HasValues())
            {
                return OperateResult.Failed(_stringLocalizer["设备控制项参数不正确，存在重复数据"]);
            }
            var checkResult = await CheckTaskSys(commonTaskSysList);
            if (!checkResult.Item1)
            {
                return OperateResult.Failed(checkResult.Item2);
            }
            var gwProcTimeList = await _context.GwProcTimeTlist.FirstOrDefaultAsync(x => x.TableId == tableId);
            if (gwProcTimeList == default)
            {
                return OperateResult.Failed(_stringLocalizer["普通任务不存在"]);
            }
            bool procTimeIsChange = false;
            if (gwProcTimeList.TableName != tableName)
            {
                gwProcTimeList.TableName = tableName;
                procTimeIsChange = true;
            }
            if (gwProcTimeList.Comment != comment)
            {
                gwProcTimeList.Comment = comment;
                procTimeIsChange = true;
            }
            if (procTimeIsChange)
            {
                _context.GwProcTimeTlist.Update(gwProcTimeList);
            }

            List<(GwprocTimeEqpTable data, bool isCanRun)> addedEqpDatas = commonTaskEqpList
                .Where(x => !x.Id.HasValue)
                .Select((data) => (new GwprocTimeEqpTable
                {
                    TableId = tableId,
                    Time = Convert.ToDateTime(data.Time),
                    TimeDur = Convert.ToDateTime(data.TimeDur),
                    EquipNo = data.EquipNo,
                    SetNo = data.SetNo,
                    Value = data.Value,
                    EquipSetNm = data.EquipSetNm,
                    ProcessOrder = data.ProcessOrder,
                }, data.IsCanRun))
                .ToList();

            List<(int id, bool isCanRun)> eqpChangedIsCanRrunList = new List<(int, bool)>();

            if (addedEqpDatas.HasValues())
            {
                await _context.GwProcTimeEqpTable.AddRangeAsync(addedEqpDatas.Select(x => x.data));
            }

            var eqpDatas = commonTaskEqpList.Where(x => x.Id.HasValue).ToDictionary(x => x.Id.Value);

            var eqpEntitys = await _context.GwProcTimeEqpTable
                .Where(d => d.TableId == tableId).ToListAsync();

            foreach (var item in eqpEntitys)
            {
                if (!eqpDatas.TryGetValue(item.Id, out var data))
                {
                    _context.GwProcTimeEqpTable.Remove(item);
                    continue;
                }
                bool isChange = false;
                var time = Convert.ToDateTime(data.Time);
                if (item.Time != time)
                {
                    item.Time = time;
                    isChange = true;
                }

                var timeDur = Convert.ToDateTime(data.TimeDur);
                if (item.TimeDur != timeDur)
                {
                    item.TimeDur = timeDur;
                    isChange = true;
                }

                if (item.EquipNo != data.EquipNo)
                {
                    item.EquipNo = data.EquipNo;
                    isChange = true;
                }

                if (item.SetNo != data.SetNo)
                {
                    item.SetNo = data.SetNo;
                    isChange = true;
                }

                if (item.Value != data.Value)
                {
                    item.Value = data.Value;
                    isChange = true;
                }

                if (item.EquipSetNm != data.EquipSetNm)
                {
                    item.EquipSetNm = data.EquipSetNm;
                    isChange = true;
                }

                if (item.ProcessOrder != data.ProcessOrder)
                {
                    item.ProcessOrder = data.ProcessOrder;
                    isChange = true;
                }

                if (isChange)
                {
                    _context.GwProcTimeEqpTable.Update(item);
                }
            }
            List<(GwprocTimeSysTable data, bool isCanRun)> addedSysDatas = commonTaskSysList
                .Where(x => !x.Id.HasValue)
                .Select((data) => (new GwprocTimeSysTable
                {
                    TableId = tableId,
                    Time = Convert.ToDateTime(data.Time),
                    TimeDur = Convert.ToDateTime(data.TimeDur),
                    CmdNm = data.CmdNm,
                    ProcCode = data.ProcCode,
                    ProcessOrder = data.ProcessOrder,
                }, data.IsCanRun))
                .ToList();
            List<(int id, bool isCanRun)> sysChangedIsCanRrunList = new List<(int, bool)>();
            if (addedSysDatas.HasValues())
            {
                await _context.GwProcTimeSysTable.AddRangeAsync(addedSysDatas.Select(x => x.data));
            }

            var sysDatas = commonTaskSysList.Where(x => x.Id.HasValue).ToDictionary(x => x.Id.Value);
            var sysEntitys = await _context.GwProcTimeSysTable
                .Where(d => d.TableId == tableId).ToListAsync();
            foreach (var item in sysEntitys)
            {
                if (!sysDatas.TryGetValue(item.Id, out var data))
                {
                    _context.GwProcTimeSysTable.Remove(item);
                    continue;
                }

                bool isChange = false;
                var time = Convert.ToDateTime(data.Time);
                if (item.Time != time)
                {
                    item.Time = time;
                    isChange = true;
                }
                var timeDur = Convert.ToDateTime(data.TimeDur);
                if (item.TimeDur != timeDur)
                {
                    item.TimeDur = timeDur;
                    isChange = true;
                }
                if (item.CmdNm != data.CmdNm)
                {
                    item.CmdNm = data.CmdNm;
                    isChange = true;
                }

                if (item.ProcCode != data.ProcCode)
                {
                    item.ProcCode = data.ProcCode;
                    isChange = true;
                }

                if (item.ProcessOrder != data.ProcessOrder)
                {
                    item.ProcessOrder = data.ProcessOrder;
                    isChange = true;
                }

                if (isChange)
                {
                    _context.GwProcTimeSysTable.Update(item);
                }
            }

            try
            {
                await _context.SaveChangesAsync();

                ResetTimeTaskManage();

                return OperateResult.Success;
            }
            catch (Exception e)
            {
                _apiLog.Error($"[修改普通任务失败] 出现异常：{e}");
                return OperateResult.Failed(_stringLocalizer["修改普通任务失败"] + $":{e.Message}");
            }
        }

        public async Task<OperateResult> EditCycleyTaskData(EditCycleTaskModel editCycleTaskModel)
        {
            var tableId = editCycleTaskModel.TableId;

            var tableName = editCycleTaskModel.TableName;

            var comment = editCycleTaskModel.Comment;

            var cycleTaskContentList = editCycleTaskModel.CycleTaskContent;
            var checkResult = await CheckTaskSys(cycleTaskContentList.Where(x => x.Type == "S"));
            if (!checkResult.Item1)
            {
                return OperateResult.Failed(checkResult.Item2);
            }

            var cycleTaskModel = new GwprocCycleTlist()
            {
                TableId = tableId,
                TableName = tableName,
                BeginTime = Convert.ToDateTime(editCycleTaskModel.CycleTask.BeginTime),
                EndTime = Convert.ToDateTime(editCycleTaskModel.CycleTask.EndTime),
                ZhenDianDo = editCycleTaskModel.CycleTask.ZhenDianDo,
                ZhidingDo = editCycleTaskModel.CycleTask.ZhidingDo,
                ZhidingTime = Convert.ToDateTime(editCycleTaskModel.CycleTask.ZhidingTime),
                CycleMustFinish = editCycleTaskModel.CycleTask.CycleMustFinish,
                MaxCycleNum = editCycleTaskModel.CycleTask.MaxCycleNum,
                Reserve1 = comment
            };

            _context.GwProcCycleTlist.Update(cycleTaskModel);

            var cEntitys = await _context.GwProcCycleTable
                .Where(d => d.TableId == tableId).ToListAsync();

            if (cEntitys != null)
            {
                _context.GwProcCycleTable.RemoveRange(cEntitys);
            }

            if (cycleTaskContentList.Any())
            {

                var orderNum = 0;
                await _context.GwProcCycleTable.AddRangeAsync(cycleTaskContentList.Select(d =>
                    new GwprocCycleTable()
                    {
                        TableId = tableId,
                        DoOrder = ++orderNum,
                        Type = d.Type,
                        EquipNo = d.EquipNo,
                        SetNo = d.SetNo,
                        Value = d.Value,
                        EquipSetNm = d.EquipSetNm,
                        ProcCode = d.ProcCode,
                        CmdNm = d.CmdNm,
                        SleepTime = d.SleepTime,
                        SleepUnit = d.SleepUnit,
                        ProcessOrder = d.ProcessOrder
                    }));
            }

            try
            {
                if (await _context.SaveChangesAsync() <= 0)
                {
                    return OperateResult.Failed(_stringLocalizer["修改循环任务失败"]);
                }

                ResetTimeTaskManage();
                return OperateResult.Success;
            }
            catch (Exception e)
            {
                _apiLog.Error($"[修改循环任务] 出现异常：{e}");
                return OperateResult.Failed(_stringLocalizer["修改循环任务失败"] + $":{e.Message}");
            }
        }

        public async Task<OperateResult> DelCommonTaskData(int? tableId)
        {
            if (!tableId.HasValue || tableId.Value <= 0)
            {
                return OperateResult.Failed(_stringLocalizer["请求参数为空，请检查"]);
            }

            int code = await IsHasRenwu(TaskType.T, tableId.Value);

            switch (code)
            {
                case 1:
                    return OperateResult.Failed(_stringLocalizer["请先删除每周任务"]);
                case 2:
                    return OperateResult.Failed(_stringLocalizer["请先删除特殊任务安排"]);
                default:
                    break;
            }

            var commonTaskEntity = await _context.GwProcTimeTlist
                .SingleOrDefaultAsync(d => d.TableId == tableId);

            if (commonTaskEntity == null)
            {
                return OperateResult.Failed(_stringLocalizer["普通任务不存在"]);
            }

            _context.GwProcTimeTlist.Remove(commonTaskEntity);

            var eqpEntitys = await _context.GwProcTimeEqpTable
                .Where(d => d.TableId == tableId)
                .ToListAsync();

            if (eqpEntitys.Any())
            {
                _context.GwProcTimeEqpTable.RemoveRange(eqpEntitys);
            }

            var sysEntitys = await _context.GwProcTimeSysTable
                .Where(d => d.TableId == tableId).ToListAsync();

            if (sysEntitys.Any())
            {
                _context.GwProcTimeSysTable.RemoveRange(sysEntitys);
            }

            await DeleteSpecAndWeekTable(TaskType.T, tableId.Value);

            try
            {
                if (await _context.SaveChangesAsync() <= 0)
                {
                    return OperateResult.Failed(_stringLocalizer["删除普通任务失败"]);
                }

                ResetTimeTaskManage();

                return OperateResult.Success;
            }
            catch (Exception e)
            {
                _apiLog.Error($"[删除普通任务] 出现异常：{e}");
                return OperateResult.Failed(_stringLocalizer["删除普通任务失败"] + $":{e.Message}");
            }
        }

        public async Task<OperateResult> DelCycleTaskData(int? tableId)
        {
            if (!tableId.HasValue || tableId.Value <= 0)
            {
                return OperateResult.Failed(_stringLocalizer["请求参数为空，请检查"]);
            }

            int code = await IsHasRenwu(TaskType.C, tableId.Value);
            switch (code)
            {
                case 1:
                    return OperateResult.Failed(_stringLocalizer["请先删除每周任务"]);
                case 2:
                    return OperateResult.Failed(_stringLocalizer["请先删除特殊任务安排"]);
                default:
                    break;
            }

            var cycleTaskEntity = await _context.GwProcCycleTlist
                 .SingleOrDefaultAsync(d => d.TableId == tableId);

            if (cycleTaskEntity == null)
            {
                return OperateResult.Failed(_stringLocalizer["循环任务不存在"]);
            }

            _context.GwProcCycleTlist.Remove(cycleTaskEntity);

            var cEntitys = await _context.GwProcCycleTable
                .Where(d => d.TableId == tableId).ToListAsync();

            if (cEntitys.Any())
            {
                _context.GwProcCycleTable.RemoveRange(cEntitys);
            }

            var timeEntitys = await _context.GwprocCycleProcessTime
                .Where(d => d.TableId == tableId).ToListAsync();
            if (timeEntitys.Any())
            {
                _context.GwprocCycleProcessTime.RemoveRange(timeEntitys);
            }

            await DeleteSpecAndWeekTable(TaskType.C, tableId.Value);

            try
            {
                if (await _context.SaveChangesAsync() <= 0)
                {
                    return OperateResult.Failed(_stringLocalizer["删除循环任务失败"]);
                }

                ResetTimeTaskManage();

                return OperateResult.Success;
            }
            catch (Exception e)
            {
                _apiLog.Error($"[删除循环任务] 出现异常：{e}");
                return OperateResult.Failed(_stringLocalizer["删除循环任务失败"] + $":{e.Message}");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] // 内联函数
        private async Task DeleteSpecAndWeekTable(TaskType taskType, int tableId)
        {
            var type = taskType.ToString();
            var specEntitys = await _context.GwProcSpecTable
                .Where(d => d.TableId == $"{type}{tableId}").ToListAsync();
            if (specEntitys.Any())
            {
                _context.GwProcSpecTable.RemoveRange(specEntitys);
            }
            var commonTaskSpecList = await _context.GwProcSpecTable.AsNoTracking()
                .Where(d => EF.Functions.Like(d.TableId, $"%{type}%"))
                .ToListAsync();
            foreach (var commonTaskSpec in commonTaskSpecList)
            {
                commonTaskSpec.TableId = commonTaskSpec.TableId
                    .Replace($"{type}{tableId}+", "", StringComparison.CurrentCultureIgnoreCase)
                    .Replace($"+{type}{tableId}", "", StringComparison.CurrentCultureIgnoreCase);
            }
            await DeleteWeekTask(type + tableId);

        }


        private async Task<int> IsHasRenwu(TaskType type, int id)
        {
            var con = $"{type}{id}";

            if (await _context.GwProcWeekTable
                .AnyAsync(x => x.Fri.Contains(con) || x.Mon.Contains(con) ||
                x.Sat.Contains(con) || x.Sun.Contains(con) ||
                x.Thurs.Contains(con) || x.Tues.Contains(con) || x.Wed.Contains(con)))
            {
                return 1;
            }

            if (await _context.GwProcSpecTable.AnyAsync(x => x.TableId.Contains(con)))
            {
                return 2;
            }

            return 0;
        }

        #endregion

        private async Task DeleteWeekTask(string tableId)
        {
            var commonTaskSpecEntity = await _context.GwProcWeekTable.SingleOrDefaultAsync();

            if (commonTaskSpecEntity != null)
            {
                commonTaskSpecEntity.Mon = RemoveTargetTaskForWeekTask(commonTaskSpecEntity.Mon, tableId);
                commonTaskSpecEntity.Tues = RemoveTargetTaskForWeekTask(commonTaskSpecEntity.Tues, tableId);
                commonTaskSpecEntity.Wed = RemoveTargetTaskForWeekTask(commonTaskSpecEntity.Wed, tableId);
                commonTaskSpecEntity.Thurs = RemoveTargetTaskForWeekTask(commonTaskSpecEntity.Thurs, tableId);
                commonTaskSpecEntity.Fri = RemoveTargetTaskForWeekTask(commonTaskSpecEntity.Fri, tableId);
                commonTaskSpecEntity.Sat = RemoveTargetTaskForWeekTask(commonTaskSpecEntity.Sat, tableId);
                commonTaskSpecEntity.Sun = RemoveTargetTaskForWeekTask(commonTaskSpecEntity.Sun, tableId);
            }
        }

        public static string RemoveTargetTaskForWeekTask(string str, string tableId)
        {
            var bld = new StringBuilder();

            var resultData = string.Empty;

            if (!string.IsNullOrWhiteSpace(str))
            {
                var list = str.Split("+").ToList();

                foreach (var item in list)
                {
                    if (item != tableId)
                    {
                        bld.Append($"{item}+");
                    }
                }

                resultData = bld.ToString();
            }

            if (resultData.Length > 0)
            {
                resultData = resultData.Substring(0, resultData.Length - 1);
            }

            return resultData;
        }

        public async Task<OperateResult<EditProcTaskWeekModel>> GetProcTaskWeekDataList(string searchName)
        {
            var taskWeekData = await _context.GwProcWeekTable.Select(d =>
                new GwprocWeekTable()
                {
                    Mon = d.Mon,
                    Tues = d.Tues,
                    Wed = d.Wed,
                    Thurs = d.Thurs,
                    Fri = d.Fri,
                    Sat = d.Sat,
                    Sun = d.Sun
                }).FirstOrDefaultAsync();

            if (taskWeekData != null)
            {
                var commonTaskQuery = _context.GwProcTimeTlist.AsNoTracking();

                var cycleTaskQuery = _context.GwProcCycleTlist.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(searchName))
                {
                    commonTaskQuery = commonTaskQuery.Where(d =>
                        d.TableName.Contains(searchName));

                    cycleTaskQuery = cycleTaskQuery.Where(d =>
                         d.TableName.Contains(searchName));
                }

                var commonTasks = await commonTaskQuery.Select(d =>
                    new GetProcTaskWeekModel()
                    {
                        TableId = d.TableId,
                        TableName = d.TableName,
                        TableType = TaskType.T.ToString(),
                        Remark = d.Comment,
                        BeginTime = "",
                        EndTime = ""
                    }).ToListAsync();

                var cycleTasks = await cycleTaskQuery.Select(d =>
                   new GetProcTaskWeekModel
                   {
                       TableId = d.TableId,
                       TableName = d.TableName,
                       TableType = TaskType.C.ToString(),
                       Remark = d.Reserve1,
                       BeginTime = Convert.ToDateTime(d.BeginTime).ToString("yyyy-MM-dd HH:mm:ss"),
                       EndTime = Convert.ToDateTime(d.EndTime).ToString("yyyy-MM-dd HH:mm:ss")
                   }).ToListAsync();

                var weekTaskList = commonTasks.Union(cycleTasks).ToList();

                await GetCommonTaskWeekModel(weekTaskList);

                var editProcTaskWeekModel = new EditProcTaskWeekModel
                {
                    Mon = ProcTaskWeekTypeList(weekTaskList, taskWeekData.Mon),
                    Tues = ProcTaskWeekTypeList(weekTaskList, taskWeekData.Tues),
                    Wed = ProcTaskWeekTypeList(weekTaskList, taskWeekData.Wed),
                    Thurs = ProcTaskWeekTypeList(weekTaskList, taskWeekData.Thurs),
                    Fri = ProcTaskWeekTypeList(weekTaskList, taskWeekData.Fri),
                    Sat = ProcTaskWeekTypeList(weekTaskList, taskWeekData.Sat),
                    Sun = ProcTaskWeekTypeList(weekTaskList, taskWeekData.Sun),
                };

                return OperateResult.Successed(editProcTaskWeekModel);
            }

            var weekDataInfo = new WeekDataInfo();

            return OperateResult.Successed(new EditProcTaskWeekModel
            {
                Mon = weekDataInfo,
                Tues = weekDataInfo,
                Wed = weekDataInfo,
                Thurs = weekDataInfo,
                Fri = weekDataInfo,
                Sat = weekDataInfo,
                Sun = weekDataInfo
            });
        }

        private async Task GetCommonTaskWeekModel(IEnumerable<GetProcTaskWeekModel> taskWeekModels)
        {
            var commonTaskEqpQuery = _context.GwProcTimeEqpTable.AsNoTracking();
            var commonTaskSysQuery = _context.GwProcTimeSysTable.AsNoTracking();

            var commonEqpTasks = await commonTaskEqpQuery.Select(d =>
                new TaskDetailData()
                {
                    TableId = d.TableId,
                    Time = Convert.ToDateTime(d.Time).ToString("yyyy-MM-dd HH:mm:ss"),
                    Type = "E"
                }).ToListAsync();

            var commonSysTasks = await commonTaskSysQuery.Select(d =>
                new TaskDetailData()
                {
                    TableId = d.TableId,
                    Time = Convert.ToDateTime(d.Time).ToString("yyyy-MM-dd HH:mm:ss"),
                    Type = "S"
                }).ToListAsync();

            var commonTaskDetailDatas = commonEqpTasks
                .Union(commonSysTasks).ToList();

            foreach (var taskWeekModel in taskWeekModels)
            {
                var taskDetailDatasOfTable = commonTaskDetailDatas
                    .Where(d => d.TableId == taskWeekModel.TableId).OrderBy(d => d.Time).ToList();

                if (taskDetailDatasOfTable.Count > 0)
                {
                    taskWeekModel.BeginTime = taskDetailDatasOfTable[0].Time;
                    taskWeekModel.EndTime = taskDetailDatasOfTable[^1].Time;
                }
            }
        }

        public async Task<OperateResult> EditProcTaskWeekData(SaveProcTaskWeekModel saveProcTaskWeekModel)
        {
            /*任务格式
             * 循环任务（C作为开头前缀+任务id）
             * 普通任务（T作为开头前缀+任务id）
             */

            /* 每周选择多个任务，则通过“+”进行拼接
             * 比如：每周一执行普通任务1和循环任务1（T1+C1）
             */

            if (saveProcTaskWeekModel == null)
            {
                return OperateResult.Failed(_stringLocalizer["请求参数为空，请检查"]);
            }


            var monArr = string.IsNullOrWhiteSpace(saveProcTaskWeekModel.Mon) ? Array.Empty<string>() : saveProcTaskWeekModel.Mon.Split("+");
            var tuesArr = string.IsNullOrWhiteSpace(saveProcTaskWeekModel.Tues) ? Array.Empty<string>() : saveProcTaskWeekModel.Tues.Split("+");
            var wedArr = string.IsNullOrWhiteSpace(saveProcTaskWeekModel.Wed) ? Array.Empty<string>() : saveProcTaskWeekModel.Wed.Split("+");
            var thursArr = string.IsNullOrWhiteSpace(saveProcTaskWeekModel.Thurs) ? Array.Empty<string>() : saveProcTaskWeekModel.Thurs.Split("+");
            var friArr = string.IsNullOrWhiteSpace(saveProcTaskWeekModel.Fri) ? Array.Empty<string>() : saveProcTaskWeekModel.Fri.Split("+");
            var satArr = string.IsNullOrWhiteSpace(saveProcTaskWeekModel.Sat) ? Array.Empty<string>() : saveProcTaskWeekModel.Sat.Split("+");
            var sunArr = string.IsNullOrWhiteSpace(saveProcTaskWeekModel.Sun) ? Array.Empty<string>() : saveProcTaskWeekModel.Sun.Split("+");

            if (HasRepeat(monArr) || HasRepeat(tuesArr) || HasRepeat(wedArr) ||
               HasRepeat(thursArr) || HasRepeat(friArr) || HasRepeat(satArr) || HasRepeat(sunArr))
            {
                return OperateResult.Failed(_stringLocalizer["任务存在重复，请检查"]);
            }

            if (IsInvalidFormat(monArr) || IsInvalidFormat(tuesArr) || IsInvalidFormat(wedArr) ||
                IsInvalidFormat(thursArr) || IsInvalidFormat(friArr) || IsInvalidFormat(satArr) || IsInvalidFormat(sunArr))
            {
                return OperateResult.Failed(_stringLocalizer["任务格式不正确，请检查"]);
            }

            var totalArr = monArr.Union(tuesArr).Union(wedArr).Union(thursArr).Union(friArr).Union(satArr).Union(sunArr).Distinct().ToArray();
            var cycleCheckResult = await CheckCycleTask(totalArr);
            if (cycleCheckResult.isError)
            {
                return cycleCheckResult.opResult;
            }
            var commCheckResult = await CheckCommTask(totalArr);
            if (commCheckResult.isError)
            {
                return commCheckResult.opResult;
            }

            var newWeekTableEntity = new GwprocWeekTable()
            {
                Mon = saveProcTaskWeekModel.Mon,
                Tues = saveProcTaskWeekModel.Tues,
                Wed = saveProcTaskWeekModel.Wed,
                Thurs = saveProcTaskWeekModel.Thurs,
                Fri = saveProcTaskWeekModel.Fri,
                Sat = saveProcTaskWeekModel.Sat,
                Sun = saveProcTaskWeekModel.Sun
            };

            var weekTableEntity = await _context.GwProcWeekTable.FirstOrDefaultAsync();

            if (weekTableEntity != null)
            {
                _context.GwProcWeekTable.Remove(weekTableEntity);
            }

            await _context.GwProcWeekTable.AddAsync(newWeekTableEntity);

            try
            {
                if (await _context.SaveChangesAsync() <= 0)
                {
                    return OperateResult.Failed(_stringLocalizer["修改每周任务失败"]);
                }

                ResetTimeTaskManage();

                return OperateResult.Success;
            }
            catch (Exception e)
            {
                _apiLog.Error($"[修改每周任务] 出现异常：{e}");
                return OperateResult.Failed(_stringLocalizer["修改每周任务失败"] + $":{e.Message}");
            }
        }

        public async Task<OperateResult<PagedResult<SpecTimeListDto>>> GetProcTaskSpecDataList(GetProcTaskSpecDataModel model)
        {
            if (model == null)
            {
                return OperateResult.Failed<PagedResult<SpecTimeListDto>>(_stringLocalizer["请求参数为空，请检查"]);
            }

            if (!model.PageNo.HasValue || !model.PageSize.HasValue)
            {
                return OperateResult.Failed<PagedResult<SpecTimeListDto>>("请求参数不完整，请检查");
            }
            if (model.PageNo.Value <= 0 || model.PageSize.Value <= 0)
            {
                return OperateResult.Failed<PagedResult<SpecTimeListDto>>("请求参数不完整，请检查");
            }

            var searchName = model.SearchName;

            var specTableQuery = _context.GwProcSpecTable.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(model.BeginTime) && !string.IsNullOrWhiteSpace(model.EndTime))
            {
                if (!DateTime.TryParse(model.BeginTime, out var beginDate))
                {
                    return OperateResult.Failed<PagedResult<SpecTimeListDto>>(_stringLocalizer["开始日期格式不正确"]);
                }

                if (!DateTime.TryParse(model.EndTime, out var endDate))
                {
                    return OperateResult.Failed<PagedResult<SpecTimeListDto>>(_stringLocalizer["结束日期格式不正确"]);
                }

                if (beginDate > endDate)
                {
                    return OperateResult.Failed<PagedResult<SpecTimeListDto>>(_stringLocalizer["开始日期应小于结束日期"]);
                }

                specTableQuery = specTableQuery.Where(d => (d.BeginDate >= beginDate && d.BeginDate <= endDate) // 开始时间在外
                        || (d.BeginDate <= beginDate && d.EndDate >= beginDate) // 开始时间在内
                        || (d.EndDate >= beginDate && d.EndDate <= endDate) // 结束时间在内
                        || (d.BeginDate <= endDate && d.EndDate >= endDate) // 结束之间在外
                );
            }

            if (!string.IsNullOrEmpty(searchName))
            {
                specTableQuery = specTableQuery.Where(d => d.DateName.Contains(searchName));
            }

            var specTableEntityList = await specTableQuery.ToListAsync();

            var specTableDtoList = specTableEntityList.Select(d =>
                new SpecTimeListDto
                {
                    TableID = d.TableId,
                    EndDate = d.EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    BeginDate = d.BeginDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    DateName = d.DateName,
                    Color = d.Color,
                    Id = d.Id
                }).ToList();

            var total = specTableEntityList.Count;

            var skipRow = (model.PageNo - 1) * model.PageSize;

            var rows = specTableDtoList.Skip(skipRow.Value).Take(model.PageSize.Value);

            var result = PagedResult<SpecTimeListDto>.Create(total, rows);

            return OperateResult.Successed(result);
        }

        public async Task<OperateResult> AddProcTaskSpecData(AddProcTaskSpecModel addProcTaskSpecModel)
        {
            if (addProcTaskSpecModel == null)
            {
                return OperateResult.Failed(_stringLocalizer["请求参数为空，请检查"]);
            }

            if (string.IsNullOrEmpty(addProcTaskSpecModel.DateName) || string.IsNullOrEmpty(addProcTaskSpecModel.TableId))
            {
                return OperateResult.Failed(_stringLocalizer["请求参数不完整，请检查"]);
            }

            if (!DateTime.TryParse(addProcTaskSpecModel.BeginDate, out var beginDate))
            {
                return OperateResult.Failed(_stringLocalizer["开始日期格式不正确"]);
            }

            if (!DateTime.TryParse(addProcTaskSpecModel.EndDate, out var endDate))
            {
                return OperateResult.Failed(_stringLocalizer["结束日期格式不正确"]);
            }

            if (beginDate >= endDate)
            {
                return OperateResult.Failed(_stringLocalizer["开始日期应小于结束日期"]);
            }


            var tableId = addProcTaskSpecModel.TableId;

            var taskIds = tableId.Split('+');
            if (IsInvalidFormat(taskIds))
            {
                return OperateResult.Failed(_stringLocalizer["任务格式不正确，请检查"]);
            }

            var cycleCheckResult = await CheckCycleTask(taskIds);
            if (cycleCheckResult.isError)
            {
                return cycleCheckResult.opResult;
            }
            var commCheckResult = await CheckCommTask(taskIds);
            if (commCheckResult.isError)
            {
                return commCheckResult.opResult;
            }

            if (await _context.GwProcSpecTable.AnyAsync(d => d.DateName == addProcTaskSpecModel.DateName))
            {
                return OperateResult.Failed(_stringLocalizer["特殊日期安排任务名称重复"]);
            }

            var specAddEntityModel = new GwprocSpecTable()
            {
                DateName = addProcTaskSpecModel.DateName,
                BeginDate = beginDate,
                EndDate = endDate,
                TableId = addProcTaskSpecModel.TableId,
                Color = addProcTaskSpecModel.Color
            };

            await _context.GwProcSpecTable.AddAsync(specAddEntityModel);

            try
            {
                if (await _context.SaveChangesAsync() <= 0)
                {
                    return OperateResult.Failed(_stringLocalizer["添加特殊日期安排任务失败"]);
                }

                ResetTimeTaskManage();

                return OperateResult.Success;
            }
            catch (Exception e)
            {
                _apiLog.Error($"[添加特殊日期安排任务] 出现异常：{e}");
                return OperateResult.Failed(_stringLocalizer["添加特殊日期安排任务失败"] + $":{e.Message}");
            }
        }

        public async Task<OperateResult> EditProcTaskSpecData(EditProcTaskSpecModel editProcTaskSpecModel)
        {
            if (editProcTaskSpecModel == null)
            {
                return OperateResult.Failed(_stringLocalizer["请求参数为空，请检查"]);
            }

            if (string.IsNullOrEmpty(editProcTaskSpecModel.DateName) || string.IsNullOrEmpty(editProcTaskSpecModel.TableId))
            {
                return OperateResult.Failed(_stringLocalizer["请求参数不完整，请检查"]);
            }

            if (!DateTime.TryParse(editProcTaskSpecModel.BeginDate, out var beginDate))
            {
                return OperateResult.Failed(_stringLocalizer["开始日期格式不正确"]);
            }

            if (!DateTime.TryParse(editProcTaskSpecModel.EndDate, out var endDate))
            {
                return OperateResult.Failed(_stringLocalizer["结束日期格式不正确"]);
            }

            if (beginDate > endDate)
            {
                return OperateResult.Failed(_stringLocalizer["开始日期应小于结束日期"]);
            }

            var tableId = editProcTaskSpecModel.TableId;

            var taskIds = tableId.Split('+');
            if (IsInvalidFormat(taskIds))
            {
                return OperateResult.Failed(_stringLocalizer["任务格式不正确，请检查"]);
            }
            var cycleCheckResult = await CheckCycleTask(taskIds);
            if (cycleCheckResult.isError)
            {
                return cycleCheckResult.opResult;
            }
            var commCheckResult = await CheckCommTask(taskIds);
            if (commCheckResult.isError)
            {
                return commCheckResult.opResult;
            }

            if (await _context.GwProcSpecTable.AnyAsync(d => d.Id != editProcTaskSpecModel.Id && d.DateName == editProcTaskSpecModel.DateName))
            {
                return OperateResult.Failed(_stringLocalizer["特殊日期安排任务名称重复"]);
            }

            var specUpdateEntityModel = await _context.GwProcSpecTable
                .FirstOrDefaultAsync(d => d.Id == editProcTaskSpecModel.Id);

            if (specUpdateEntityModel == null)
            {
                return OperateResult.Failed(_stringLocalizer["特殊日期安排任务不存在"]);
            }

            specUpdateEntityModel.DateName = editProcTaskSpecModel.DateName;
            specUpdateEntityModel.BeginDate = beginDate;
            specUpdateEntityModel.EndDate = endDate;
            specUpdateEntityModel.TableId = editProcTaskSpecModel.TableId;
            _context.GwProcSpecTable.Update(specUpdateEntityModel);
            try
            {
                if (await _context.SaveChangesAsync() <= 0)
                {
                    return OperateResult.Failed(_stringLocalizer["修改特殊日期安排任务失败"]);
                }

                ResetTimeTaskManage();

                return OperateResult.Success;
            }
            catch (Exception e)
            {
                _apiLog.Error($"[修改特殊日期安排任务] 出现异常：{e}");
                return OperateResult.Failed(_stringLocalizer["修改特殊日期安排任务失败"] + $":{e.Message}");
            }
        }

        public async Task<OperateResult> DelProcTaskSpecData(long Id)
        {
            var specTbaleEntity = await _context.GwProcSpecTable.FirstOrDefaultAsync(d => d.Id == Id);

            if (specTbaleEntity == null)
            {
                return OperateResult.Failed(_stringLocalizer["特殊日期安排任务不存在"]);
            }

            _context.Remove(specTbaleEntity);

            try
            {
                if (await _context.SaveChangesAsync() <= 0)
                {
                    return OperateResult.Failed(_stringLocalizer["删除特殊日期安排任务失败"]);
                }

                ResetTimeTaskManage();

                return OperateResult.Success;
            }
            catch (Exception e)
            {
                _apiLog.Error($"[删除特殊日期安排任务] 出现异常：{e}");
                return OperateResult.Failed(_stringLocalizer["删除特殊日期安排任务失败"] + $":{e.Message}");
            }
        }

        public async Task<OperateResult<IEnumerable<SpecMonthDataDto>>> GetProcTaskSpecMonthData(string beginTime, string endTime)
        {
            var specTableQuery = _context.GwProcSpecTable.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(beginTime))
            {
                if (!DateTime.TryParse(beginTime, out var realBeginTime))
                {
                    return OperateResult.Failed<IEnumerable<SpecMonthDataDto>>(_stringLocalizer["开始日期格式不正确"]);
                }
                specTableQuery = specTableQuery.Where(d => d.BeginDate >= realBeginTime);
            }

            if (!string.IsNullOrWhiteSpace(endTime))
            {
                if (DateTime.TryParse(endTime, out var realEndTime))
                {
                    return OperateResult.Failed<IEnumerable<SpecMonthDataDto>>(_stringLocalizer["结束日期格式不正确"]);
                }
                specTableQuery = specTableQuery.Where(d => d.EndDate <= realEndTime);
            }

            var specTableList = await specTableQuery.ToListAsync();

            var result = specTableList.GroupBy(d => d.BeginDate.ToString("yyyy-MM"))
                .Select(d => new SpecMonthDataDto
                {
                    MonthDate = d.Key,
                    CountNum = d.Count()
                });

            return OperateResult.Successed(result);

        }

        public WeekDataInfo ProcTaskWeekTypeList(IEnumerable<GetProcTaskWeekModel> procTaskWeeks, string strList)
        {
            var weekDataInfo = new WeekDataInfo();

            if (string.IsNullOrEmpty(strList))
            {
                return weekDataInfo;
            }

            string[] list = strList.Split("+");

            var cycleTasks = list.Where(d => d.StartsWith('C')).Select(d => d.TrimStart('C')).ToList();
            var commonTasks = list.Where(d => d.StartsWith('T')).Select(d => d.TrimStart('T')).ToList();

            var tProcTaskLists = procTaskWeeks.Where(item => commonTasks.Any(tListValue => tListValue == item.TableId.ToString() && item.TableType == TaskType.T.ToString())).ToList();

            var cProcTaskLists = procTaskWeeks.Where(item => cycleTasks.Any(cListValue => cListValue == item.TableId.ToString() && item.TableType == TaskType.C.ToString())).ToList();

            weekDataInfo = new WeekDataInfo()
            {
                tProcTaskLists = tProcTaskLists,
                cProcTaskLists = cProcTaskLists
            };

            return weekDataInfo;
        }


        private void ResetTimeTaskManage()
        {
            try
            {
                _proxy.ResetProcTimeManage();
            }
            catch (Exception ex)
            {
                _apiLog.Error("ResetProcTimeManage【定时任务重置接口异常】" + ex);
            }
        }


        private static bool HasRepeat(string[] strs)
        {
            if (strs == null || strs.Length == 0)
            {
                return false;
            }

            var distinct = strs.Distinct().ToList();

            return distinct.Count != strs.Length;
        }

        private static bool IsInvalidFormat(string[] strs)
        {
            if (strs == null || strs.Length == 0)
            {
                return false;
            }

            var filters = strs.Where(d => d.HasValue()).ToList();

            if (filters.Count <= 0)
            {
                return false;
            }

            if (filters.Any(d => !d.StartsWith("C") && !d.StartsWith("T")))
            {
                return true;
            }

            var trimNumbers = filters.Select(d => d.TrimStart('C'))
                   .Select(d => d.TrimStart('T')).ToList();
            return trimNumbers.Any(d => !int.TryParse(d, out var number) || number <= 0);
        }

        private IEnumerable<int> GetTaskIds(IEnumerable<string> reqTaskIdString, string taskPrefix)
        {
            var cycleArr = reqTaskIdString
                .Where(x => x.StartsWith(taskPrefix))
                .Select(x => int.TryParse(x.Replace(taskPrefix, ""), out var intVal) ? intVal : 0)
                .Where(x => x > 0)
                .ToArray();
            return cycleArr;
        }
        private async Task<(bool isError, OperateResult opResult)> CheckCycleTask(IEnumerable<string> reqTaskIdString)
        {
            var cycleArr = GetTaskIds(reqTaskIdString, CycleTaskPrefix);
            if (cycleArr.HasValues() &&
                !await _context.GwProcCycleTable.AsNoTracking().AnyAsync(x => cycleArr.Contains(x.TableId)))
            {
                return (true, OperateResult.Failed(_stringLocalizer["循环任务不存在"]));
            }
            return (false, null);
        }
        private async Task<(bool isError, OperateResult opResult)> CheckCommTask(IEnumerable<string> reqTaskIdString)
        {
            var commArr = GetTaskIds(reqTaskIdString, CommTaskPrefix);
            if (commArr.HasValues() &&
                !await _context.GwProcTimeSysTable.AsNoTracking().AnyAsync(x => commArr.Contains(x.TableId))
                &&
                !await _context.GwProcTimeEqpTable.AsNoTracking().AnyAsync(x => commArr.Contains(x.TableId)))
            {
                return (true, OperateResult.Failed(_stringLocalizer["普通任务不存在"]));
            }
            return (false, null);
        }
        const string CycleTaskPrefix = "C";
        const string CommTaskPrefix = "T";

        public async Task<OperateResult<IEnumerable<GetExProcCmdDataResponse>>> GetExProcCmdData()
        {
            var result = from proc in _context.GwexProc.AsNoTracking()
                         join procCmd in _context.GwexProcCmd.AsNoTracking() on proc.ProcCode equals procCmd.ProcCode
                         select new GetExProcCmdDataResponse
                         {
                             ProcCode = procCmd.ProcCode,
                             CmdNm = procCmd.CmdNm,
                             MainInstruction = procCmd.MainInstruction,
                             MinorInstruction = procCmd.MinorInstruction,
                             Value = procCmd.Value,
                             Record = procCmd.Record,
                             ProcName = proc.ProcName,
                         };
            return OperateResult.Successed<IEnumerable<GetExProcCmdDataResponse>>(await result.ToListAsync());
        }
    }
}
