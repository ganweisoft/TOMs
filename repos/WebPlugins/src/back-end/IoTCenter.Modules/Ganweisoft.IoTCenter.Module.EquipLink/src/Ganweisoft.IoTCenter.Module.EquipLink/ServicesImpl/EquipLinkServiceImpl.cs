// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
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
using System.Text;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.EquipLink
{
    public class EquipLinkServiceImpl : IEquipLinkService
    {
        private const string EquipLinkDllName = "GWChangJing.STD.dll";

        private readonly EquipBaseImpl _equipBaseImpl;
        private readonly Session _session;
        private readonly EquipLinkDbContext _context;
        private readonly ILoggingService _apiLog;
        private readonly IotCenterHostService proxy;
        private readonly PermissionCacheService _permissionCacheService;
        private readonly IStringLocalizer<EquipLinkServiceImpl> _stringLocalizer;

        public EquipLinkServiceImpl(
            Session session,
            EquipBaseImpl equipBaseImpl,
            EquipLinkDbContext context,
            ILoggingService apiLog,
            IotCenterHostService alarmCenterService,
            PermissionCacheService permissionCacheService,
            IStringLocalizer<EquipLinkServiceImpl> stringLocalizer)
        {
            _equipBaseImpl = equipBaseImpl;
            _apiLog = apiLog;
            _context = context;
            _session = session;
            proxy = alarmCenterService;
            _stringLocalizer = stringLocalizer;
            _permissionCacheService = permissionCacheService;
        }

        public async Task<OperateResult<PageResult<EquipLinkListResponse>>> GetEquipLinkListByPage(GetEquipLinkModel equipLinkModel)
        {
            if (equipLinkModel == null)
            {
                return OperateResult.Failed<PageResult<EquipLinkListResponse>>(_stringLocalizer["请求参数为空，请检查"]);
            }

            string equipName = equipLinkModel.EquipName;
            int[] iequipNos = equipLinkModel.IequipNos;
            string iycyxTypes = equipLinkModel.IycyxTypes;
            string oequipNos = equipLinkModel.OequipNos;
            string osetNos = equipLinkModel.OsetNos;


            var query = from a in _context.AutoProc.AsNoTracking()
                        join equip in _context.Equip.AsNoTracking() on a.IequipNo equals equip.EquipNo
                        join oequip in _context.Equip.AsNoTracking() on a.OequipNo equals oequip.EquipNo
                        join setparm in _context.SetParm.AsNoTracking() on new { eq = a.OequipNo, set = a.OsetNo } equals new { eq = setparm.EquipNo, set = setparm.SetNo } into setparms
                        from setparm in setparms.DefaultIfEmpty()
                        join ycp in _context.Ycp.AsNoTracking() on new { eq = a.IequipNo, yc = a.IycyxNo } equals new { eq = ycp.EquipNo, yc = ycp.YcNo } into ycps
                        from ycp in ycps.DefaultIfEmpty()
                        join yxp in _context.Yxp.AsNoTracking() on new { eq = a.IequipNo, yx = a.IycyxNo } equals new { eq = yxp.EquipNo, yx = yxp.YxNo } into yxps
                        from yxp in yxps.DefaultIfEmpty()
                        select new EquipLinkListResponse
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
                            Enable = a.Enable
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
                if (noSetNos.HasValues())
                {
                    query = query.Where(x => noSetNos.Any(j => j == x.OequipNo));
                }
                var hasSetNos = equipLinkModel.EquipSetLists.Where(x => x.SetNos.Any());

                var orConditions = new List<Expression<Func<EquipLinkListResponse, bool>>>();
                foreach (var item in hasSetNos)
                {
                    Expression<Func<EquipLinkListResponse, bool>> left = (x) => (x.OequipNo == item.EquipNo);
                    Expression<Func<EquipLinkListResponse, bool>> right = (x) => (item.SetNos.Contains(x.OsetNo));
                    orConditions.Add(EfExt.And<EquipLinkListResponse>(left, right));
                }
                if (orConditions.HasValues())
                {
                    var totalExpression = orConditions.Aggregate((l, r) => EfExt.Or(l, r));
                    query = query.Where(totalExpression);
                }
            }

            var sql2 = query.ToQueryString();

            var total2 = await query.CountAsync();
            var skipRow = (equipLinkModel.PageNo - 1) * equipLinkModel.PageSize;
            var rows2 = await query.Skip(skipRow.Value).Take(equipLinkModel.PageSize.Value).ToListAsync();

            var pageResult = PageResult<EquipLinkListResponse>.Create(total2, rows2);

            return OperateResult.Successed(pageResult);

        }

        public async Task<OperateResult> AddEquipLinkData(AddEquipLinkModel addEquipLinkModel)
        {
            if (addEquipLinkModel == null)
            {
                return OperateResult.Failed(_stringLocalizer["请求参数为空，请检查"]);
            }

            if (addEquipLinkModel.IycyxType.IsEmpty())
            {
                return OperateResult.Failed(_stringLocalizer["触发类型为空"]);
            }

            var equipExist = await _context.Equip
                                      .AsNoTracking()
                                      .AnyAsync(x => x.EquipNo == addEquipLinkModel.IequipNo);
            if (!equipExist)
            {
                return OperateResult.Failed(_stringLocalizer["设备不存在"]);
            }
            if (addEquipLinkModel.IycyxType.Equals("c", StringComparison.OrdinalIgnoreCase))
            {
                if (!addEquipLinkModel.IycyxNo.HasValue)
                {
                    return OperateResult.Failed(_stringLocalizer["遥测模拟量不存在"]);
                }
                var ycExist = await _context.Ycp
                                            .AsNoTracking()
                                            .AnyAsync(x => x.EquipNo == addEquipLinkModel.IequipNo && x.YcNo == addEquipLinkModel.IycyxNo);
                if (!ycExist)
                {
                    return OperateResult.Failed(_stringLocalizer["遥测模拟量不存在"]);
                }
            }
            else if (addEquipLinkModel.IycyxType.Equals("x", StringComparison.OrdinalIgnoreCase))
            {
                if (!addEquipLinkModel.IycyxNo.HasValue)
                {
                    return OperateResult.Failed(_stringLocalizer["遥信模拟量不存在"]);
                }
                var yxExist = await _context.Yxp
                                            .AsNoTracking()
                                            .AnyAsync(x => x.EquipNo == addEquipLinkModel.IequipNo && x.YxNo == addEquipLinkModel.IycyxNo);
                if (!yxExist)
                {
                    return OperateResult.Failed(_stringLocalizer["遥信模拟量不存在"]);
                }
            }

            var ycyxTypes = new List<string>()
            {
                /* E 设备通讯故障
                 * e 设备通讯恢复
                 * S 设备状态故障
                 * s 设备状态恢复
                 * C 模拟量越限（遥测）
                 * c 模拟量恢复（遥测）
                 * X 状态量报警（遥信）
                 * x 状态量恢复（遥信）
                 */
                "E", "e", "S", "s", "C", "c", "X", "x","evt"
            };

            if (!ycyxTypes.Any(d => d == addEquipLinkModel.IycyxType || addEquipLinkModel.IycyxType.Contains(d)))
            {
                return OperateResult.Failed(_stringLocalizer["触发类型参数非法"]);
            }


            var model = new AutoProc()
            {
                IequipNo = addEquipLinkModel.IequipNo,
                IycyxNo = addEquipLinkModel.IycyxNo ?? 0,
                IycyxType = addEquipLinkModel.IycyxType,
                Delay = addEquipLinkModel.Delay,
                OequipNo = addEquipLinkModel.OequipNo,
                OsetNo = addEquipLinkModel.OsetNo,
                Value = addEquipLinkModel.Value,
                ProcDesc = addEquipLinkModel.ProcDesc,
                Enable = addEquipLinkModel.Enable
            };

            var modelEntity = await _context.AutoProc.AddAsync(model);
            try
            {
                if (await _context.SaveChangesAsync() <= 0)
                {
                    return OperateResult.Failed(_stringLocalizer["增加单条设备联动信息失败"]);
                }
            }
            catch (Exception e)
            {
                _apiLog.Error($"[增加单条设备联动信息] 出现异常：{e}");
                return OperateResult.Failed(_stringLocalizer["增加单条设备联动信息失败"] + $":{e.Message}");
            }

            proxy.ResetEquipmentLinkage();

            return OperateResult.Successed(new EquipAddResponse
            {
                PointNo = modelEntity.Entity.Id
            });
        }

        public async Task<OperateResult> EditEquipLinkData(EditEquipLinkModel editEquipLinkModel)
        {
            if (editEquipLinkModel == null)
            {
                return OperateResult.Failed(_stringLocalizer["请求参数为空，请检查"]);
            }

            if (editEquipLinkModel.IycyxType.IsEmpty())
            {
                return OperateResult.Failed(_stringLocalizer["触发类型为空"]);
            }

            var equipExist = await _context.Equip
                                     .AsNoTracking()
                                     .AnyAsync(x => x.EquipNo == editEquipLinkModel.IequipNo);
            if (!equipExist)
            {
                return OperateResult.Failed(_stringLocalizer["设备不存在"]);
            }
            if (editEquipLinkModel.IycyxType.Equals("c", StringComparison.OrdinalIgnoreCase))
            {

                if (!editEquipLinkModel.IycyxNo.HasValue)
                {
                    return OperateResult.Failed(_stringLocalizer["遥测模拟量不存在"]);
                }

                var ycExist = await _context.Ycp
                                            .AsNoTracking()
                                            .AnyAsync(x => x.EquipNo == editEquipLinkModel.IequipNo && x.YcNo == editEquipLinkModel.IycyxNo);
                if (!ycExist)
                {
                    return OperateResult.Failed(_stringLocalizer["遥测模拟量不存在"]);
                }
            }
            else if (editEquipLinkModel.IycyxType.Equals("x", StringComparison.OrdinalIgnoreCase))
            {

                if (!editEquipLinkModel.IycyxNo.HasValue)
                {
                    return OperateResult.Failed(_stringLocalizer["遥信模拟量不存在"]);
                }

                var yxExist = await _context.Yxp
                                            .AsNoTracking()
                                            .AnyAsync(x => x.EquipNo == editEquipLinkModel.IequipNo && x.YxNo == editEquipLinkModel.IycyxNo);
                if (!yxExist)
                {
                    return OperateResult.Failed(_stringLocalizer["遥信模拟量不存在"]);
                }
            }

            var entity = await _context.AutoProc.FirstOrDefaultAsync(d => d.Id == editEquipLinkModel.Id);

            if (entity == null)
            {
                return OperateResult.Failed(_stringLocalizer["设备联动信息不存在"]);

            }

            entity.IequipNo = editEquipLinkModel.IequipNo;
            entity.IycyxNo = editEquipLinkModel.IycyxNo ?? 0;
            entity.IycyxType = editEquipLinkModel.IycyxType;
            entity.Delay = editEquipLinkModel.Delay;
            entity.OequipNo = editEquipLinkModel.OequipNo;
            entity.OsetNo = editEquipLinkModel.OsetNo;
            entity.Value = editEquipLinkModel.Value;
            entity.ProcDesc = editEquipLinkModel.ProcDesc;
            entity.Enable = editEquipLinkModel.Enable;

            try
            {
                _context.AutoProc.Update(entity);
                if (await _context.SaveChangesAsync() <= 0)
                {
                    return OperateResult.Failed(_stringLocalizer["修改单条设备联动信息失败"]);
                }
            }
            catch (Exception e)
            {
                _apiLog.Error($"[修改单条设备联动信息] 出现异常：{e}");
                return OperateResult.Failed(_stringLocalizer["修改单条设备联动信息失败"] + $":{e.Message}");
            }

            proxy.ResetEquipmentLinkage();

            return OperateResult.Success;
        }

        public async Task<OperateResult> DelEquipLinkData(int id)
        {
            var entity = await _context.AutoProc.FirstOrDefaultAsync(d => d.Id == id);
            if (entity == null)
            {
                return OperateResult.Failed(_stringLocalizer["设备联动信息不存在"]);
            }

            _context.AutoProc.Remove(entity);

            try
            {
                if (await _context.SaveChangesAsync() <= 0)
                {
                    return OperateResult.Failed(_stringLocalizer["删除单条设备联动信息失败"]);
                }
            }
            catch (Exception e)
            {
                _apiLog.Error($"[删除单条设备联动信息] 出现异常：{e}");
                return OperateResult.Failed(_stringLocalizer["删除单条设备联动信息失败"] + $":{e.Message}");
            }

            proxy.ResetEquipmentLinkage();

            return OperateResult.Success;
        }

        public async Task<OperateResult<TEquipAndOEquiepListResponse>> GetIEquipAndOEquiepList()
        {
            var iequips =
                from a in _context.AutoProc.AsNoTracking()
                from b in _context.Equip.AsNoTracking().Where(b => a.IequipNo == b.EquipNo)
                select new TOEquip()
                {
                    EquipNo = b.EquipNo,
                    EquipNm = b.EquipNm,
                    EquipType = "I"
                };

            var oequips =
                from a in _context.AutoProc.AsNoTracking()
                from b in _context.Equip.AsNoTracking().Where(b => a.OequipNo == b.EquipNo)
                select new TOEquip()
                {
                    EquipNo = b.EquipNo,
                    EquipNm = b.EquipNm,
                    EquipType = "O"
                };

            var response = new TEquipAndOEquiepListResponse()
            {
                IList = await iequips.ToListAsync(),
                OList = await oequips.ToListAsync()
            };

            return OperateResult.Successed(response);
        }

        public async Task<OperateResult<PagedResult<EqulpLinkSence>>> GetSceneListByPage(CommonSearchPageModel commonSearchPageModel)
        {
            if (commonSearchPageModel == null)
            {
                return OperateResult.Failed<PagedResult<EqulpLinkSence>>(_stringLocalizer["请求参数为空，请检查"]);
            }

            if (!await _context.Equip.AnyAsync(d => d.CommunicationDrv == EquipLinkDllName))
            {
                return OperateResult.Successed(PagedResult<EqulpLinkSence>.Create(0, Enumerable.Empty<EqulpLinkSence>()));
            }

            var scenceQuery =
                from b in _context.Equip.AsNoTracking().Where(b => b.CommunicationDrv == EquipLinkDllName).DefaultIfEmpty()
                from a in _context.SetParm.AsNoTracking().Where(s => s.EquipNo == b.EquipNo)
                select new EqulpLinkSence
                {
                    StaNo = a.StaN,
                    EquipNo = a.EquipNo,
                    EquipNm = b.EquipNm,
                    SetNo = a.SetNo,
                    SetNm = a.SetNm,
                    SetType = a.SetType,
                    Value = a.Value,
                };

            if (!string.IsNullOrEmpty(commonSearchPageModel.SearchName))
            {
                scenceQuery = scenceQuery.Where(d => d.SetNm.Contains(commonSearchPageModel.SearchName));
            }

            var scenceList = await scenceQuery.ToListAsync();
            scenceList.Reverse();

            await TransformValueDisplay(scenceList);

            var total = scenceList.Count;

            if (commonSearchPageModel.PageNo.HasValue && commonSearchPageModel.PageSize.HasValue)
            {
                var skipRows = (commonSearchPageModel.PageNo - 1) * commonSearchPageModel.PageSize;

                scenceList = scenceList.Skip(skipRows.Value).Take(commonSearchPageModel.PageSize.Value).ToList();
            }

            var pageResult = PagedResult<EqulpLinkSence>.Create(total, scenceList);

            return OperateResult.Successed(pageResult);
        }

        private async Task TransformValueDisplay(List<EqulpLinkSence> equipLists)
        {
            foreach (EqulpLinkSence equipItem in equipLists)
            {
                List<ListValue> listValues = new List<ListValue>();
                string value = equipItem.Value;

                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                var valueList = value.Split("+").Select(x => x.Split(",")).ToList();


                foreach (var i in valueList)
                {
                    if (i.Length > 1)
                    {
                        listValues.Add(new ListValue()
                        {
                            SceneType = "E",
                            EquipNo = i[0],
                            SetNo = i[1],
                            EquipNm = await _equipBaseImpl.GetSetName(i[0], i[1], "EquipNm"),
                            SetNm = await _equipBaseImpl.GetSetName(i[0], i[1], "SetNm"),
                            SetType = await _equipBaseImpl.GetSetName(i[0], i[1], "SetType"),
                            Value = i.Length > 2 ? i[2] : string.Empty
                        });
                    }
                    else
                    {
                        listValues.Add(new ListValue()
                        {
                            SceneType = "T",
                            TimeValue = i.FirstOrDefault()  // 此时数组里面只有一个值
                        });
                    }
                }

                equipItem.List = listValues;
            }

        }

        public async Task<OperateResult> AddSceneLinkData(AddSceneModel addSceneModel)
        {
            if (addSceneModel == null)
            {
                return OperateResult.Failed(_stringLocalizer["请求参数为空，请检查"]);
            }

            var setNm = addSceneModel.SetNm;
            List<ListValue> values = addSceneModel.list;
            if (setNm.IsEmpty() || values.IsEmpty())
            {
                return OperateResult.Failed(_stringLocalizer["请求参数不完整，请检查"]);
            }

            if (values.All(v => v.SceneType == "T"))
            {
                return OperateResult.Failed(_stringLocalizer["场景至少需选择一个控制项"]);
            }

            if (values.All(v => v.SceneType == "E"))
            {
                return OperateResult.Failed(_stringLocalizer["场景至少需要一个时间延迟"]);
            }

            var equipNos = new List<int>();
            foreach (var item in values.Where(x => x.SceneType == "E"))
            {
                if (!int.TryParse(item.EquipNo, out var no))
                {
                    return OperateResult.Failed(_stringLocalizer["输入的设备号不正确"]);
                }

                if (!equipNos.Contains(no))
                {
                    equipNos.Add(no);
                }
            }

            equipNos = equipNos.Distinct().ToList();
            var dbEquipNos = await _context.Equip.AsNoTracking().CountAsync(x => equipNos.Contains(x.EquipNo));
            if (dbEquipNos != equipNos.Count())
            {
                return OperateResult.Failed(_stringLocalizer["未找到匹配的设备数"]);
            }

            if (!await _context.Equip
                .AnyAsync(d => d.CommunicationDrv == EquipLinkDllName))
            {
                var addEquip = await this.AddEquipLinkDriverDevice();

                if (!addEquip.Succeeded)
                {
                    return OperateResult.Failed<PagedResult<EqulpLinkSence>>(_stringLocalizer["场景设备添加失败"] + $":{addEquip.Message}");
                }
            }

            var changjingEquipNo = await _context.Equip
                .Where(d => d.CommunicationDrv == EquipLinkDllName)
                .Select(d => d.EquipNo)
                .FirstOrDefaultAsync();

            if (await _context.SetParm.Where(s => s.EquipNo == changjingEquipNo)
                .AnyAsync(v => v.SetNm == setNm))
            {
                return OperateResult.Failed(_stringLocalizer["场景名不能重复"]);
            }

            var realValue = SceneLinkData(values);

            if (realValue.Length > 0)
            {
                realValue = realValue[0..^1];
            }

            var setNo = await _context.SetParm.Where(s => s.EquipNo == changjingEquipNo)
                .MaxAsync(v => (int?)v.SetNo) ?? 0;

            var model = new SetParm()
            {
                StaN = 1,
                EquipNo = changjingEquipNo,
                SetNo = ++setNo,
                MainInstruction = "ChangJing",
                MinorInstruction = "-",
                SetNm = setNm,
                SetType = "J",
                Value = realValue,
                Record = true,
                Action = "设置",
                Canexecution = true,
                VoiceKeys = "1",
                EnableVoice = false,
                QrEquipNo = 0
            };

            await _context.SetParm.AddRangeAsync(model);

            try
            {
                if (await _context.SaveChangesAsync() <= 0)
                {
                    return OperateResult.Failed(_stringLocalizer["增加单条场景编辑信息失败"]);
                }
            }
            catch (Exception e)
            {
                _apiLog.Error($"[增加单条场景编辑信息] 出现异常：{e}");
                return OperateResult.Failed(_stringLocalizer["增加单条场景编辑信息失败"] + $":{e.Message}");
            }


            proxy.AddChangedEquip(new GrpcChangedEquip
            {
                iStaNo = 1,
                iEqpNo = Convert.ToInt32(changjingEquipNo),
                State = ChangedEquipState.Add
            });

            proxy.ResetEquipmentLinkage();

            return OperateResult.Success;
        }

        private async Task<OperateResult> AddEquipLinkDriverDevice()
        {
            var equipDataModel = new EquipDataModel();
            equipDataModel.CommunicationDrv = EquipLinkDllName;
            equipDataModel.EquipNm = "场景控制";
            equipDataModel.LocalAddr = "CJ";
            equipDataModel.AccCyc = 1;
            equipDataModel.OutOfContact = "处理意见";
            equipDataModel.ProcAdvice = "通讯故障";
            equipDataModel.Contacted = "通讯恢复正常";
            equipDataModel.EquipAddr = "ChangJing";
            equipDataModel.CommunicationParam = "9600/8/1/no";
            equipDataModel.CommunicationTimeParam = "200/8/16/400";
            equipDataModel.RawEquipNo = 1;

            return await AddEquipData(equipDataModel, 1);
        }

        public async Task<OperateResult> AddEquipData(EquipDataModel equipDataModel, int groudId = 0)
        {
            if (await _context.Equip.AnyAsync(
                          x => x.StaN == equipDataModel.StaNo && x.EquipNm == equipDataModel.EquipNm))
            {
                return OperateResult.Failed(_stringLocalizer["设备名称已存在"]);
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
                    return OperateResult.Failed(_stringLocalizer["初始化场景控制设备失败"]);
                }
            }
            catch (Exception e)
            {
                _apiLog.Error($"[初始化场景控制设备] 出现异常：{e}");
                return OperateResult.Failed(_stringLocalizer["初始化场景控制设备失败"] + $":{e.Message}");
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
                proxy.AddChangedEquip(new GrpcChangedEquip
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

        private static string SceneLinkData(List<ListValue> values)
        {
            var sb = new StringBuilder();

            foreach (ListValue item in values)
            {
                if (item.SceneType == "E")
                {
                    if (item.Value.HasValue())
                    {
                        sb.Append($"{item.EquipNo},{item.SetNo},{item.Value}+");
                    }
                    else
                    {
                        sb.Append($"{item.EquipNo},{item.SetNo}+");
                    }
                }
                else if (item.SceneType == "T")
                {
                    sb.Append($"{item.TimeValue}+");
                }
            }

            return sb.ToString();
        }

        public async Task<OperateResult> EditSceneLinkData(UpdateSceneModel updateSceneModel)
        {
            if (updateSceneModel == null)
            {
                return OperateResult.Failed(_stringLocalizer["请求参数为空，请检查"]);
            }

            long equipNo = updateSceneModel.EquipNo;
            long setNo = updateSceneModel.SetNo;
            string setNm = updateSceneModel.SetNm;
            List<ListValue> values = updateSceneModel.list;
            var old = values;
            if (setNm.IsEmpty() || values.IsEmpty())
            {
                return OperateResult.Failed(_stringLocalizer["请求参数不完整，请检查"]);
            }

            if (values.All(v => v.SceneType == "T"))
            {
                return OperateResult.Failed(_stringLocalizer["场景至少需选择一个控制项"]);
            }

            if (values.All(v => v.SceneType == "E"))
            {
                return OperateResult.Failed(_stringLocalizer["场景至少需要一个时间延迟"]);
            }

            var equipNos = new List<int>();
            foreach (var item in values.Where(x => x.SceneType == "E"))
            {
                if (!int.TryParse(item.EquipNo, out var no))
                {
                    return OperateResult.Failed(_stringLocalizer["输入的设备号不正确"]);
                }

                if (!equipNos.Contains(no))
                {
                    equipNos.Add(no);
                }
            }

            equipNos = equipNos.Distinct().ToList();
            var dbEquipNos = await _context.Equip.AsNoTracking().CountAsync(x => equipNos.Contains(x.EquipNo));
            if (dbEquipNos != equipNos.Count())
            {
                return OperateResult.Failed(_stringLocalizer["未找到匹配的设备数"]);
            }

            var realValue = SceneLinkData(values);

            if (realValue.Length > 0)
            {
                realValue = realValue[0..^1];
            }

            var entity = _context.SetParm.FirstOrDefault(d => d.EquipNo == equipNo && d.SetNo == setNo);
            if (entity == null) return OperateResult.Failed(_stringLocalizer["场景信息不存在"]);

            entity.SetNm = setNm;
            entity.Value = realValue;

            _context.SetParm.Update(entity);

            try
            {
                if (await _context.SaveChangesAsync() <= 0)
                {
                    return OperateResult.Failed(_stringLocalizer["修改单条场景编辑信息失败"]);
                }
            }
            catch (Exception e)
            {
                _apiLog.Error($"[修改单条场景编辑信息] 出现异常：{e}");
                return OperateResult.Failed(_stringLocalizer["修改单条场景编辑信息失败"] + $":{e.Message}");
            }

            proxy.ResetEquips();
            proxy.ResetEquipmentLinkage();

            return OperateResult.Success;
        }

        public async Task<OperateResult> DelSceneLinkData(DelSceneModel delSceneModel)
        {
            if (delSceneModel == null)
            {
                return OperateResult.Failed(_stringLocalizer["请求参数为空，请检查"]);
            }

            long equipNo = delSceneModel.EquipNo;
            long setNo = delSceneModel.SetNo;

            var sceneEntity = await _context.SetParm.FirstOrDefaultAsync(d => d.EquipNo == equipNo && d.SetNo == setNo);

            if (sceneEntity == null)
            {
                return OperateResult.Failed(_stringLocalizer["场景信息不存在"]);
            }

            _context.SetParm.Remove(sceneEntity);

            try
            {
                if (await _context.SaveChangesAsync() <= 0)
                {
                    return OperateResult.Failed(_stringLocalizer["删除单条场景编辑信息失败"]);
                }
            }
            catch (Exception e)
            {
                _apiLog.Error($"[删除单条场景编辑信息] 出现异常：{e}");
                return OperateResult.Failed(_stringLocalizer["删除单条场景编辑信息失败"] + $":{e.Message}");
            }

            proxy.AddChangedEquip(new GrpcChangedEquip
            {
                iStaNo = 1,
                iEqpNo = Convert.ToInt32(equipNo),
                State = ChangedEquipState.Delete
            });

            proxy.ResetEquipmentLinkage();

            return OperateResult.Success;
        }
    }

    public static class EfExt
    {
        public static Expression<Func<T, bool>> And<T>(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            ParameterExpression s = Expression.Parameter(typeof(T), "s");
            MyExpressionVisitor visitor = new MyExpressionVisitor(s);
            Expression body1 = visitor.Visit(left.Body);
            Expression body2 = visitor.Visit(right.Body);
            Expression<Func<T, bool>> finalEx = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(body1, body2), s);
            return finalEx;
        }
        public static Expression<Func<T, bool>> Or<T>(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            ParameterExpression s = Expression.Parameter(typeof(T), "s");
            MyExpressionVisitor visitor = new MyExpressionVisitor(s);
            Expression body1 = visitor.Visit(left.Body);
            Expression body2 = visitor.Visit(right.Body);
            Expression<Func<T, bool>> finalEx = Expression.Lambda<Func<T, bool>>(Expression.OrElse(body1, body2), s);
            return finalEx;
        }
    }
    public class MyExpressionVisitor : ExpressionVisitor
    {
        public ParameterExpression _Parameter { get; set; }

        public MyExpressionVisitor(ParameterExpression Parameter)
        {
            _Parameter = Parameter;
        }
        protected override Expression VisitParameter(ParameterExpression p)
        {
            return _Parameter;
        }

        public override Expression Visit(Expression node)
        {
            return base.Visit(node);
        }
    }
}
