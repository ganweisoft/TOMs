// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Ganweisoft.IoTCenter.Module.EquipLink.Data;
using Ganweisoft.IoTCenter.Module.EquipLink.Models;
using Ganweisoft.IoTCenter.Module.EquipLink.Models.Condition;
using IoTCenter.Data.Model;
using IoTCenter.Utilities;
using IoTCenter.Utilities.Extensions;
using IoTCenterHost.Core.Abstraction.BaseModels;
using IoTCenterHost.Core.Abstraction.EnumDefine;
using IoTCenterHost.Core.Abstraction.Services;
using IoTCenterWebApi.BaseCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.EquipLink
{
    public class ConditionLinkageServiceImpl : IConditionLinkageService
    {
        private const string ConditionEquipLinkName = "条件联动";
        private const string ConditionEquipLinkDllName = "BCDataSimu.STD.dll";

        private readonly EquipLinkDbContext _context;
        private readonly ILoggingService _apiLog;
        private readonly IotCenterHostService _proxy;
        private readonly IStringLocalizer<ConditionLinkageServiceImpl> _stringLocalizer;
        private readonly PermissionCacheService _permissionCacheService;
        private readonly Session _session;
        private readonly IEquipLinkService _equipLinkService;

        public ConditionLinkageServiceImpl(EquipLinkDbContext context,
            IStringLocalizer<ConditionLinkageServiceImpl> stringLocalizer,
            PermissionCacheService permissionCacheService,
            Session session,
            ILoggingService apiLog,
            IotCenterHostService proxy,
            IEquipLinkService equipLinkService)
        {
            _context = context;
            _stringLocalizer = stringLocalizer;
            _permissionCacheService = permissionCacheService;
            _session = session;
            _apiLog = apiLog;
            _proxy = proxy;
            _equipLinkService = equipLinkService;
        }

        public async Task<OperateResult<PageResult<EquipLinkListResponseEx>>> GetConditionLinkListByPage(
            GetEquipLinkModel equipLinkModel)
        {
            if (equipLinkModel == null)
            {
                return OperateResult.Failed<PageResult<EquipLinkListResponseEx>>(_stringLocalizer["请求参数为空，请检查"]);
            }

            string equipName = equipLinkModel.EquipName;
            int[] iequipNos = equipLinkModel.IequipNos;
            string iycyxTypes = equipLinkModel.IycyxTypes;
            string oequipNos = equipLinkModel.OequipNos;
            string osetNos = equipLinkModel.OsetNos;


            var query = from a in _context.AutoProc.AsNoTracking()
                        join ac in _context.ConditionAutoProcs.AsNoTracking() on a.Id equals ac.RelateAutoProc into acs
                        from ac in acs.DefaultIfEmpty()
                        join equip in _context.Equip.AsNoTracking() on a.IequipNo equals equip.EquipNo
                        join oequip in _context.Equip.AsNoTracking() on a.OequipNo equals oequip.EquipNo
                        join setparm in _context.SetParm.AsNoTracking() on new { eq = a.OequipNo, set = a.OsetNo } equals new { eq = setparm.EquipNo, set = setparm.SetNo } into setparms
                        from setparm in setparms.DefaultIfEmpty()
                        join ycp in _context.Ycp.AsNoTracking() on new { eq = a.IequipNo, yc = a.IycyxNo } equals new { eq = ycp.EquipNo, yc = ycp.YcNo } into ycps
                        from ycp in ycps.DefaultIfEmpty()
                        join yxp in _context.Yxp.AsNoTracking() on new { eq = a.IequipNo, yx = a.IycyxNo } equals new { eq = yxp.EquipNo, yx = yxp.YxNo } into yxps
                        from yxp in yxps.DefaultIfEmpty()
                        select new EquipLinkListResponseEx
                        {
                            Id = a.Id,
                            IequipNo = a.IequipNo,
                            IequipNm = equip.EquipNm,
                            IycyxNo = a.IycyxNo,
                            YcYxName = a.IycyxType == "C" || a.IycyxType == "c" ? ycp.YcNm : (a.IycyxType == "X" || a.IycyxType == "x" ? yxp.YxNm : ""),
                            IycyxType = a.IycyxType,
                            Delay = a.Delay,
                            OequipNo = a.OequipNo,
                            OequipNm = oequip.EquipNm,
                            OsetNo = a.OsetNo,
                            SetNm = setparm.SetNm,
                            SetType = setparm.SetType,
                            Value = a.Value,
                            ProcDesc = a.ProcDesc,
                            Enable = a.Enable,
                            IsConditionLink = ac != null
                        };

            if (!string.IsNullOrEmpty(equipName))
            {
                query = query.Where(x => x.IequipNm.Contains(equipName));
            }

            if (iequipNos != null && iequipNos.Any())
            {
                query = query.Where(x => iequipNos.Contains(x.IequipNo));
            }

            if (equipLinkModel.MinDelay.HasValue)
            {
                query = query.Where(x => x.Delay >= equipLinkModel.MinDelay);
            }

            if (equipLinkModel.MaxDelay.HasValue)
            {
                query = query.Where(x => x.Delay <= equipLinkModel.MaxDelay);
            }

            if (!string.IsNullOrWhiteSpace(oequipNos))
            {
                var oeqs = oequipNos.Split(',').Select(x => Convert.ToInt32(x));
                query = query.Where(x => oeqs.Contains(x.IequipNo));
            }

            if (!string.IsNullOrWhiteSpace(osetNos))
            {
                var osets = osetNos.Split(',').Select(x => Convert.ToInt32(x));
                query = query.Where(x => osets.Contains(x.OsetNo));
            }


            var ycyxTypeArr = Array.Empty<string>();
            if (!string.IsNullOrWhiteSpace(iycyxTypes))
            {
                ycyxTypeArr = iycyxTypes.Split(",", StringSplitOptions.RemoveEmptyEntries);
                query = query.Where(x => ycyxTypeArr.Contains(x.IycyxType));
            }

            if (equipLinkModel.EquipSetLists.Any())
            {
                var noSetNos = equipLinkModel.EquipSetLists.Where(x => !x.SetNos.Any()).Select(x => x.EquipNo);
                if (noSetNos.Any())
                {
                    query = query.Where(x => noSetNos.Any(j => j == x.OequipNo));
                }
                var hasSetNos = equipLinkModel.EquipSetLists.Where(x => x.SetNos.Any());

                var orConditions = new List<Expression<Func<EquipLinkListResponseEx, bool>>>();
                foreach (var item in hasSetNos)
                {
                    Expression<Func<EquipLinkListResponseEx, bool>> left = (x) => (x.OequipNo == item.EquipNo);
                    Expression<Func<EquipLinkListResponseEx, bool>> right = (x) => (item.SetNos.Contains(x.OsetNo));
                    orConditions.Add(EfExt.And<EquipLinkListResponseEx>(left, right));
                }
                if (orConditions.HasValues())
                {
                    var totalExpression = orConditions.Aggregate((l, r) => EfExt.Or(l, r));
                    query = query.Where(totalExpression);
                }
            }

            var total2 = await query.CountAsync();

            if (!equipLinkModel.PageNo.HasValue)
            {
                equipLinkModel.PageNo = 1;
            }

            if (!equipLinkModel.PageSize.HasValue)
            {
                equipLinkModel.PageSize = 10;
            }

            var skipRow = (equipLinkModel.PageNo - 1) * equipLinkModel.PageSize;
            var rows2 = await query.Skip(skipRow.Value).Take(equipLinkModel.PageSize.Value).ToListAsync();

            var pageResult = PageResult<EquipLinkListResponseEx>.Create(total2, rows2.OrderByDescending(r => r.Id));

            return OperateResult.Successed(pageResult);
        }

        public async Task<OperateResult> AddConditionLinkData(AddConditionModel addSceneModel)
        {
            if (addSceneModel == null)
            {
                return OperateResult.Failed(_stringLocalizer["请求参数为空，请检查"]);
            }

            if (!addSceneModel.IConditionItems.Any())
            {
                return OperateResult.Failed(_stringLocalizer["条件联动至少需选择一个触发项"]);
            }

            var equipExist = await _context.Equip
                .AsNoTracking()
                .AnyAsync(x => x.EquipNo == addSceneModel.OequipNo);
            if (!equipExist)
            {
                return OperateResult.Failed(_stringLocalizer["设备不存在"] + ":" + addSceneModel.OequipNo);
            }

            if (!await _context.SetParm
                    .AsNoTracking()
                    .AnyAsync(x => x.EquipNo == addSceneModel.OequipNo && x.SetNo == addSceneModel.OsetNo))
            {
                return OperateResult.Failed(_stringLocalizer["设置点不存在"] + ":" + addSceneModel.OequipNo);
            }

            foreach (var conditionItem in addSceneModel.IConditionItems)
            {
                var conditionEquip = await _context.Equip
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.EquipNo == conditionItem.IequipNo);
                if (conditionEquip == null)
                {
                    return OperateResult.Failed(_stringLocalizer["设备不存在"] + ":" + conditionItem.IequipNo);
                }

                conditionItem.IequipNm = conditionEquip.EquipNm;
            }

            int rootEquipNo;
            var equipRoot = await _context.Equip
                .FirstOrDefaultAsync(d => d.CommunicationDrv == ConditionEquipLinkDllName && d.EquipNm == ConditionEquipLinkName);
            if (equipRoot == null)
            {
                var addEquip = await this.AddEquipLinkDriverDevice();
                if (!addEquip.Succeeded)
                {
                    return OperateResult.Failed<PagedResult<EqulpLinkSence>>(_stringLocalizer["条件联动设备添加失败"] +
                                                                             $":{addEquip.Message}");
                }

                rootEquipNo = addEquip.Data.EquipNo;
            }
            else
            {
                rootEquipNo = equipRoot.EquipNo;
            }

            var exprStr = addSceneModel.GetExpression();
            _apiLog.Error("ConditionLinkage GetExpression: " + exprStr);

            var yxDataModel = new YxpDataModel(rootEquipNo, exprStr);
            var addEquipYxpData = await this.AddEquipYxpData(yxDataModel);
            if (!addEquipYxpData.Succeeded)
            {
                return OperateResult.Failed(_stringLocalizer["条件联动设备添加测点失败"] +
                                            $":{addEquipYxpData.Message}");
            }

            var condiAutoProc = new ConditionAutoProc
            {
                ProcName = addSceneModel.ProcName,
                RelateYxNo = addEquipYxpData.Data.PointNo,
                Delay = addSceneModel.Delay,
                OequipNo = addSceneModel.OequipNo,
                OsetNo = addSceneModel.OsetNo,
                Value = addSceneModel.Value,
                ProcDesc = addSceneModel.ProcDesc,
                Enable = addSceneModel.Enable,
                Modifier = _session.UserName,
                ModifyDate = DateTime.Now,
                Deleted = false,
            };

            condiAutoProc.ConditionExpressions.AddRange(addSceneModel.IConditionItems.SelectMany(i =>
                i.IYcYxItems.Select(ii => new ConditionEquipExpr
                {
                    IequipNo = i.IequipNo,
                    IequipNm = i.IequipNm,
                    IycyxNo = ii.IycyxNo,
                    IycyxType = ii.IycyxType,
                    IycyxValue = ii.IycyxValue,
                    Condition = ii.Condition
                })));

            var condiAutoProcEntity = await _context.ConditionAutoProcs.AddAsync(condiAutoProc);
            await _context.SaveChangesAsync();

            var autoProcResp = await _equipLinkService.AddEquipLinkData(new AddEquipLinkModel
            {
                IequipNo = addEquipYxpData.Data.EquipNo,
                IycyxNo = addEquipYxpData.Data.PointNo,
                IycyxType = "x",
                Delay = addSceneModel.Delay,
                OequipNo = addSceneModel.OequipNo,
                OsetNo = addSceneModel.OsetNo,
                Value = addSceneModel.Value,
                ProcDesc = addSceneModel.ProcDesc,
                Enable = addSceneModel.Enable
            });

            if (!autoProcResp.Succeeded)
            {
                return OperateResult.Failed(_stringLocalizer["条件联动设备添加记录失败"] +
                                            $":{autoProcResp.Message}");
            }

            var convertResp = autoProcResp as OperateResult<EquipAddResponse>;
            condiAutoProcEntity.Entity.RelateAutoProc = convertResp?.Data.PointNo ?? 0;
            _context.ConditionAutoProcs.Update(condiAutoProc);
            await _context.SaveChangesAsync();
            return OperateResult.Success;
        }

        public async Task<OperateResult> EditConditionLinkData(EditConditionModel updateModel)
        {
            if (updateModel == null)
            {
                return OperateResult.Failed(_stringLocalizer["请求参数为空，请检查"]);
            }

            var autoProc = await _context.AutoProc.AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == updateModel.Id);
            if (autoProc == null)
            {
                return OperateResult.Failed(_stringLocalizer["设备联动信息不存在"]);
            }

            var condiAutoProc = await _context.ConditionAutoProcs.AsNoTracking()
                .Include(p => p.ConditionExpressions)
                .FirstOrDefaultAsync(x => x.RelateAutoProc == autoProc.Id && !x.Deleted);
            if (condiAutoProc == null)
            {
                return OperateResult.Failed(_stringLocalizer["设备联动信息不存在"]);
            }

            var equipExist = await _context.Equip
                .AsNoTracking()
                .AnyAsync(x => x.EquipNo == updateModel.OequipNo);
            if (!equipExist)
            {
                return OperateResult.Failed(_stringLocalizer["设备不存在"]);
            }

            if (!await _context.SetParm
                    .AsNoTracking()
                    .AnyAsync(x => x.EquipNo == updateModel.OequipNo && x.SetNo == updateModel.OsetNo))
            {
                return OperateResult.Failed(_stringLocalizer["设置点不存在"]);
            }

            foreach (var conditionItem in updateModel.IConditionItems)
            {
                var conditionEquip = await _context.Equip
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.EquipNo == conditionItem.IequipNo);
                if (conditionEquip == null)
                {
                    return OperateResult.Failed(_stringLocalizer["设备不存在"]);
                }

                conditionItem.IequipNm = conditionEquip.EquipNm;
            }

            var equipRoot = await _context.Equip
                .FirstOrDefaultAsync(d => d.CommunicationDrv == ConditionEquipLinkDllName && d.EquipNm == ConditionEquipLinkName);
            if (equipRoot == null)
            {
                return OperateResult.Failed<PagedResult<EqulpLinkSence>>(_stringLocalizer["条件联动设备添加失败"]);
            }

            autoProc.Delay = updateModel.Delay;
            autoProc.OequipNo = updateModel.OequipNo;
            autoProc.OsetNo = updateModel.OsetNo;
            autoProc.Value = updateModel.Value;
            autoProc.ProcDesc = updateModel.ProcDesc;
            autoProc.Enable = updateModel.Enable;
            _context.AutoProc.Update(autoProc);

            condiAutoProc.Delay = updateModel.Delay;
            condiAutoProc.OequipNo = updateModel.OequipNo;
            condiAutoProc.OsetNo = updateModel.OsetNo;
            condiAutoProc.Value = updateModel.Value;
            condiAutoProc.ProcDesc = updateModel.ProcDesc;
            condiAutoProc.Enable = updateModel.Enable;
            condiAutoProc.Modifier = _session.UserName;
            condiAutoProc.ModifyDate = DateTime.Now;
            _context.ConditionAutoProcs.Update(condiAutoProc);

            _context.ConditionEquipExprs.RemoveRange(condiAutoProc.ConditionExpressions);

            await _context.ConditionEquipExprs.AddRangeAsync(updateModel.IConditionItems.SelectMany(i =>
                i.IYcYxItems.Select(ii => new ConditionEquipExpr
                {
                    ConditionId = condiAutoProc.Id,
                    IequipNo = i.IequipNo,
                    IequipNm = i.IequipNm,
                    IycyxNo = ii.IycyxNo,
                    IycyxType = ii.IycyxType,
                    IycyxValue = ii.IycyxValue,
                    Condition = ii.Condition
                })));

            await _context.SaveChangesAsync();

            var exprStr = updateModel.GetExpression();
            _apiLog.Error("ConditionLinkage GetExpression: " + exprStr);
            if (await _context.Yxp.AsNoTracking()
                    .AnyAsync(x => x.EquipNo == updateModel.OequipNo && condiAutoProc.RelateYxNo == x.YxNo))
            {
            }
            else
            {
                var yxDataModel = new YxpDataModel(equipRoot.EquipNo, exprStr);
                var addEquipYxpData = await this.AddEquipYxpData(yxDataModel);
                if (!addEquipYxpData.Succeeded)
                {
                    return OperateResult.Failed(_stringLocalizer["条件联动设备添加测点失败"] +
                                                $":{addEquipYxpData.Message}");
                }
            }

            try
            {
                _proxy.ResetEquipmentLinkage();
                _proxy.AddChangedEquip(new GrpcChangedEquip
                {
                    iStaNo = 1,
                    iEqpNo = Convert.ToInt32(autoProc.IequipNo),
                    State = ChangedEquipState.Edit
                });
            }
            catch (Exception ex)
            {
                _apiLog.Error("AddChangedEquip - Edit【设备重置接口异常】" + ex);
            }

            return OperateResult.Success;
        }

        public async Task<OperateResult> DelConditionLinkData(DelConditionModel delSceneModel)
        {
            var autoProc = await _context.AutoProc.AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == delSceneModel.Id);
            if (autoProc == null)
            {
                return OperateResult.Failed(_stringLocalizer["设备联动信息不存在"]);
            }

            var condiAutoProc = await _context.ConditionAutoProcs.AsNoTracking()
                .FirstOrDefaultAsync(x => x.RelateAutoProc == autoProc.Id && !x.Deleted);
            if (condiAutoProc == null)
            {
                return OperateResult.Failed(_stringLocalizer["设备联动信息不存在"]);
            }

            condiAutoProc.Deleted = true;
            condiAutoProc.Modifier = _session.UserName;
            condiAutoProc.ModifyDate = DateTime.Now;
            _context.ConditionAutoProcs.Update(condiAutoProc);

            _context.AutoProc.Remove(autoProc);

            var yxp = await _context.Yxp.AsNoTracking()
                .FirstOrDefaultAsync(x => x.EquipNo == autoProc.IequipNo && x.YxNo == autoProc.IycyxNo);
            _context.Yxp.Remove(yxp);
            await _context.SaveChangesAsync();

            try
            {
                _proxy.ResetEquipmentLinkage();

                _proxy.AddChangedEquip(new GrpcChangedEquip
                {
                    iStaNo = 1,
                    iEqpNo = autoProc.IequipNo,
                    State = ChangedEquipState.Edit
                });
            }
            catch (Exception ex)
            {
                _apiLog.Error("AddChangedEquip - Edit【设备重置接口异常】" + ex);
            }

            return OperateResult.Success;
        }

        public async Task<OperateResult<GetConditionResponse>> GetConditionLinkByAutoProcId(GetConditionModel getConditionModel)
        {
            if (getConditionModel == null)
            {
                return OperateResult.Failed<GetConditionResponse>(_stringLocalizer["请求参数为空，请检查"]);
            }

            var condiAutoProc = await _context.ConditionAutoProcs
                .AsNoTracking()
                .Include(x => x.ConditionExpressions)
                .FirstOrDefaultAsync(x => x.RelateAutoProc == getConditionModel.AutoProcId && !x.Deleted);
            if (condiAutoProc == null)
            {
                return OperateResult.Failed<GetConditionResponse>(_stringLocalizer["条件联动记录不存在"]);
            }

            var result = new GetConditionResponse
            {
                Id = condiAutoProc.Id,
                ProcName = condiAutoProc.ProcName,
                Delay = condiAutoProc.Delay,
                IConditionItems = condiAutoProc.ConditionExpressions.GroupBy(i => new { i.IequipNo, i.IequipNm })
                    .Select(i =>
                        new AddIConditionItem
                        {
                            IequipNo = i.Key.IequipNo,
                            IequipNm = i.Key.IequipNm,
                            IYcYxItems = i.Select(ii => new AddIConditionYcYxItem
                            {
                                IycyxNo = ii.IycyxNo,
                                IycyxType = ii.IycyxType,
                                IycyxValue = ii.IycyxValue,
                                Condition = ii.Condition
                            })
                        }),
                OequipNo = condiAutoProc.OequipNo,
                OsetNo = condiAutoProc.OsetNo,
                Value = condiAutoProc.Value,
                ProcDesc = condiAutoProc.ProcDesc,
                Enable = condiAutoProc.Enable
            };

            return OperateResult.Successed(result);
        }

        public async Task<OperateResult<GetEquipYcYxpResponse>> GetEquipYcYxps(GetEquipYcYxpModel model)
        {
            if (model == null || model.EquipNos == null)
            {
                return OperateResult.Failed<GetEquipYcYxpResponse>(_stringLocalizer["请求参数为空，请检查"]);
            }

            var roleName = _session.RoleName;

            var query = _context.Equip.AsNoTracking();
            if (!roleName.Equals("admin", StringComparison.OrdinalIgnoreCase))
            {
                var browerEquips = _permissionCacheService.GetPermissionObj(roleName)?.BrowseEquips ?? new List<int>();
                query = query.Where(x => browerEquips.Contains(x.EquipNo));
            }

            query = query.Where(x => model.EquipNos.Contains(x.EquipNo));

            var ycRespones = await (from e in query
                                    join yc in _context.Ycp.AsNoTracking()
                                    on e.EquipNo equals yc.EquipNo
                                    select new GetEquipYcYxpResponseItem
                                    {
                                        StaNo = e.StaN,
                                        EquipNo = e.EquipNo,
                                        YcyxNo = yc.YcNo,
                                        YcyxName = yc.YcNm,
                                        YcyxType = "C"
                                    }).ToArrayAsync();


            var yxRespones = await (from e in query
                                    join yx in _context.Yxp.AsNoTracking()
                                    on e.EquipNo equals yx.EquipNo
                                    select new GetEquipYcYxpResponseItem
                                    {
                                        StaNo = e.StaN,
                                        EquipNo = e.EquipNo,
                                        YcyxNo = yx.YxNo,
                                        YcyxType = "X",
                                        YcyxName = yx.YxNm
                                    }).ToArrayAsync();



            return OperateResult.Successed(new GetEquipYcYxpResponse
            {
                Items = ycRespones.Concat(yxRespones).Concat(model.EquipNos.Select(e => new GetEquipYcYxpResponseItem
                {
                    StaNo = 1,
                    EquipNo = e,
                    YcyxNo = 0,
                    YcyxName = _stringLocalizer["设备状态"],
                    YcyxType = "E"
                }))
            });
        }

        private async Task<OperateResult<EquipAddResponse>> AddEquipLinkDriverDevice()
        {
            var equipDataModel = new EquipDataModel
            {
                CommunicationDrv = ConditionEquipLinkDllName,
                EquipNm = ConditionEquipLinkName,
                LocalAddr = Guid.NewGuid().ToString("N"),
                AccCyc = 1,
                OutOfContact = "处理意见",
                ProcAdvice = "通讯故障",
                Contacted = "通讯恢复正常",
                EquipAddr = "1fd57489523f4e6193017261675d5451",
                CommunicationParam = "",
                CommunicationTimeParam = "1000/6/16/400",
                RawEquipNo = 1
            };

            return await AddEquipData(equipDataModel, 1);
        }

        private async Task<OperateResult<EquipAddResponse>> AddEquipData(EquipDataModel equipDataModel, int groudId = 0)
        {
            if (await _context.Equip.AnyAsync(
                          x => x.StaN == equipDataModel.StaNo && x.EquipNm == equipDataModel.EquipNm))
            {
                return OperateResult.Failed<EquipAddResponse>(_stringLocalizer["设备名称已存在"]);
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
                RawEquipNo = (int)equipDataModel.RawEquipNo,
                Tabname = equipDataModel.Tabname,
                AlarmScheme = (int)equipDataModel.AlarmScheme,
                Attrib = (int)equipDataModel.Attrib,
                StaIp = equipDataModel.StaIp,
                AlarmRiseCycle = (int)equipDataModel.AlarmRiseCycle,
                Reserve1 = equipDataModel.Reserve1,
                Reserve2 = equipDataModel.Reserve2,
                Reserve3 = equipDataModel.Reserve3,
                RelatedVideo = equipDataModel.RelatedVideo,
                ZiChanId = equipDataModel.ZiChanId,
                PlanNo = equipDataModel.PlanNo,
                SafeTime = equipDataModel.SafeTime,
                Backup = equipDataModel.Backup,
            });

            try
            {
                if (await _context.SaveChangesAsync() <= 0)
                {
                    return OperateResult.Failed<EquipAddResponse>(_stringLocalizer["初始化场景控制设备失败"]);
                }
            }
            catch (Exception e)
            {
                _apiLog.Error($"[初始化场景控制设备] 出现异常：{e}");
                return OperateResult.Failed<EquipAddResponse>(_stringLocalizer["初始化场景控制设备失败"] + $":{e.Message}");
            }

            var insertEquipNo = equip.Entity.EquipNo;

            if (groudId != 0)
            {
                _context.EGroupList.Add(new EGroupList
                {
                    GroupId = groudId,
                    StaNo = equipDataModel.StaNo,
                    EquipNo = insertEquipNo
                });
                await _context.SaveChangesAsync();
            }

            _permissionCacheService.AddEquipPermission(_session.RoleName, insertEquipNo);

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

            return OperateResult.Successed(new EquipAddResponse
            {
                StaN = equip.Entity.StaN,
                EquipNo = equip.Entity.EquipNo,
                PointNo = 0
            });
        }

        private async Task<OperateResult<EquipAddResponse>> AddEquipYxpData(YxpDataModel yxpDataModel)
        {
            var yxs = await _context.Yxp.AsNoTracking().Where(d => d.EquipNo == yxpDataModel.EquipNo).ToListAsync();
            var yxNo = yxs.Any() ? yxs.Max(d => d.YxNo) + 1 : 1;

            var yxp = new Yxp
            {
                StaN = (int)yxpDataModel.StaNo,
                EquipNo = (int)yxpDataModel.EquipNo,
                YxNo = yxNo,
                YxNm = "条件联动测点" + yxNo,
                ProcAdviceR = yxpDataModel.ProcAdviceR,
                ProcAdviceD = yxpDataModel.ProcAdviceD,
                LevelR = yxpDataModel.LevelR,
                LevelD = yxpDataModel.LevelD,
                Evt01 = yxpDataModel.Evt01,
                Evt10 = yxpDataModel.Evt10,
                MainInstruction = yxpDataModel.MainInstruction,
                MinorInstruction = yxpDataModel.MinorInstruction,
                AlarmAcceptableTime = (int)yxpDataModel.AlarmAcceptableTime,
                RestoreAcceptableTime = (int)yxpDataModel.RestoreAcceptableTime,
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
                YxCode = "ConditionLinkage" + yxNo,
                DataType = yxpDataModel.DataType
            };

            await _context.Yxp.AddAsync(yxp);

            try
            {
                if (await _context.SaveChangesAsync() < 0)
                {
                    return OperateResult.Failed<EquipAddResponse>(_stringLocalizer["新增遥信模拟量配置信息失败"]);
                }
            }
            catch (Exception ex)
            {
                _apiLog.Error($"[新增遥信模拟量配置信息] 出现异常：{ex}");
                return OperateResult.Failed<EquipAddResponse>(_stringLocalizer["新增遥信模拟量配置信息失败"] + $":{ex.Message}");
            }

            try
            {
                _proxy.AddChangedEquip(new GrpcChangedEquip
                {
                    iStaNo = 1,
                    iEqpNo = Convert.ToInt32(yxpDataModel.EquipNo),
                    State = ChangedEquipState.Edit
                });
            }
            catch (Exception ex)
            {
                _apiLog.Error("AddChangedEquip - Edit【设备重置接口异常】" + ex);
            }

            if (!string.IsNullOrEmpty(yxpDataModel.Expression))
            {
                await Task.Delay(2000);
            }

            return OperateResult.Successed(new EquipAddResponse
            {
                StaN = yxp.StaN,
                EquipNo = yxp.EquipNo,
                PointNo = yxp.YxNo
            });
        }
    }
}