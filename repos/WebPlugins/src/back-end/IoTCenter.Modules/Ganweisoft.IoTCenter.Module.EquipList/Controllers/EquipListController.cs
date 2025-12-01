// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Data;
using IoTCenter.Utilities;
using IoTCenterCore.ExcelHelper;
using IoTCenterHost.Core.Abstraction.AppServices;
using IoTCenterHost.Core.Abstraction.BaseModels;
using IoTCenterHost.Core.Abstraction.Services;
using IoTCenterHost.Proxy;
using IoTCenterWebApi.BaseCore;
using IoTCenterWebApi.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.EquipList.Controllers
{
    [ApiController]
    [Route("/IoT/api/v3/[controller]/[action]")]
    public class EquipListController : DefaultController
    {
        private readonly IEquipListService _equipListService;
        private readonly GWDbContext _context;
        private readonly IotCenterHostService _proxy;
        private readonly ICurveClientAppService _curveAppService;

        private readonly ILoggingService _apiLog;
        private readonly IHubContext<DownFileNotifyHub> _notifyHub;

        private static readonly ManualResetEventSlim exportLocker = new ManualResetEventSlim(true);

        public EquipListController(IEquipListService equipListService,
            GWDbContext context,
            IotCenterHostService proxy,
            IHubContext<DownFileNotifyHub> notifyHub,
            ICurveClientAppService curveAppService,
            ILoggingService apiLog)
        {
            _equipListService = equipListService;
            _context = context;
            _proxy = proxy;
            _curveAppService = curveAppService;

            _notifyHub = notifyHub;
            _apiLog = apiLog;
        }

        [HttpGet]
        public async Task<OperateResult> GetYcYxSetNumByEquipNo([Required] int? equipNo)
        {
            var result = await _context.Equip.AsNoTracking()
                .Include(d => d.Ycps)
                .Include(d => d.Yxps)
                .Include(d => d.SetParms)
                .Where(d => d.EquipNo == equipNo)
                .Select(d => new
                {
                    YcNum = d.Ycps.Count,
                    YxNum = d.Yxps.Count,
                    SetNum = d.SetParms.Count
                })
                .FirstOrDefaultAsync();

            if (result == null)
            {
                return OperateResult.Failed("设备不存在");
            }

            return OperateResult.Successed(result);
        }

        [HttpPost]
        public async Task<OperateResult> GetEquipListByPageNoState(GetEquipListModel getEquipListModel)
        {
            return await _equipListService.GetEquipListByPageNoState(getEquipListModel);
        }

        [HttpPost]
        public OperateResult<Dictionary<int, GrpcEquipState>> GetEquipListStateByPage(
            GetEquipListStateModel getEquipListStateModel)
        {
            return _equipListService.GetEquipListStateByPage(getEquipListStateModel);
        }

        [HttpPost]
        [ResponseCache(Duration = 5)]
        public OperateResult<Page> GetEquipItemStateByPage(GetEquipItemStateModel getEquipItemStateModel)
        {
            return _equipListService.GetEquipItemStateByPage(getEquipItemStateModel);
        }

        [HttpPost]
        public OperateResult GetEquipItemStateByIds(GetEquipItemStateByIds getEquipItemStateByIds)
        {
            return _equipListService.GetEquipItemStateByIds(getEquipItemStateByIds);
        }

        [HttpPost]
        public OperateResult GetYcpItemValueAndState(GetEquipItemStateModel getEquipItemStateModel)
        {
            return _equipListService.GetYcpItemValueAndState(getEquipItemStateModel);
        }

        [HttpPost]
        public OperateResult<Page> GetYcpItemStateByPage(GetEquipItemStateModel getEquipItemStateModel)
        {
            return _equipListService.GetYcpItemStateByPage(getEquipItemStateModel);
        }

        [HttpPost]
        public OperateResult GetYxpItemValueAndState(GetEquipItemStateModel getEquipItemStateModel)
        {
            return _equipListService.GetYxpItemValueAndState(getEquipItemStateModel);
        }

        [HttpPost]
        public OperateResult<Page> GetYxpItemStateByPage(GetEquipItemStateModel getEquipItemStateModel)
        {
            return _equipListService.GetYxpItemStateByPage(getEquipItemStateModel);
        }

        [HttpPost]
        public OperateResult<Page> GetSetItemStateByPage(GetEquipItemStateModel getEquipItemStateModel)
        {
            return _equipListService.GetSetItemStateByPage(getEquipItemStateModel);
        }

        [HttpPost]
        public OperateResult<object> GetEquipYcpState(GetEquipYcYxModel getEquipYcYxModel)
        {
            return _equipListService.GetEquipYcpState(getEquipYcYxModel);
        }

        [HttpPost]
        [SkipCustomFilter]
        public async Task<OperateResult> SetCommandBySetNo(CommandBySetNoModel commandBySetNoModel)
        {
            return await _equipListService.SetCommandBySetNo(commandBySetNoModel);
        }

        [HttpPost]
        public OperateResult GetEquipYxpState(GetEquipYcYxModel getEquipYcYxModel)
        {
            return _equipListService.GetEquipYxpState(getEquipYcYxModel);
        }

        [HttpPost]
        [SkipCustomFilter]
        public OperateResult SetCommandByParameter(CommandByParameterModel commandByParameterModel)
        {
            return _equipListService.SetCommandByParameter(commandByParameterModel);
        }

        [HttpPost]
        [SkipCustomFilter]
        public OperateResult SetCommandWithRespone(CommandWithResponeModel commandByParameterModel)
        {
            return _equipListService.SetCommandWithRespone(commandByParameterModel);
        }

        [HttpPost]
        [SkipCustomFilter]
        public OperateResult SyncCommand(CommandWithResponeModel commandByParameterModel)
        {
            return _equipListService.SetCommandWithRespone(commandByParameterModel);
        }

        [HttpPost]
        public OperateResult<Page> GetYcpHistroyByTimeAsync(GetYcpHistroyModel getYcpHistroyModel)
        {
            return _equipListService.GetYcpHistroyByTimeAsync(getYcpHistroyModel);
        }

        [HttpPost]
        public OperateResult<List<MyCurveCommonData>> GetYcpHistroyChartByTimeAsync(
            GetYcpHistroyModel getYcpHistroyModel)
        {
            return _equipListService.GetYcpHistroyChartByTimeAsync(getYcpHistroyModel);
        }

        [HttpPost]
        public OperateResult<PagedResult<GetEquipNoAndNameResponse>> GetEquipNoAndName(GetEquipListStateModel getEquipListStateModel)
        {
            return _equipListService.GetEquipNoAndName(getEquipListStateModel);
        }

        [HttpPost]
        public OperateResult<PagedResult<string>> GetSetEquip(GetEquipListStateModel getEquipListStateModel)
        {
            return _equipListService.GetSetEquip(getEquipListStateModel);
        }

        [HttpPost]
        public OperateResult<PagedResult<string>> GetEquipAndSet(GetEquipListStateModel getEquipListStateModel)
        {
            return _equipListService.GetEquipAndSet(getEquipListStateModel);
        }

        [HttpPost]
        public async Task<OperateResult<PageResult<YcpList>>> GetYcpByEquipNo(
            GetEquipItemStateModel getEquipItemStateModel)
        {
            return await _equipListService.GetYcpByEquipNo(getEquipItemStateModel);
        }

        [HttpPost]
        public async Task<OperateResult<PageResult<YxpList>>> GetYxpByEquipNo(GetEquipItemStateModel getEquipItemStateModel)
        {
            return await _equipListService.GetYxpByEquipNo(getEquipItemStateModel);
        }

        [HttpPost]
        public async Task<OperateResult<PageResult<EquipSetParmQuery>>> GetSetParmByEquipNo(
            GetEquipItemStateModel getEquipItemStateModel)
        {
            return await _equipListService.GetSetParmByEquipNo(getEquipItemStateModel);
        }

        [HttpPost]
        public async Task<OperateResult<PageResult<GetSetParmNewByEquipNoModel>>> GetFullSetParmByEquipNo(
            int equipNo)
        {
            return await _equipListService.NewGetSetParmByEquipNo(new GetEquipItemStateModel { EquipNo = equipNo, IsGetAll = true });
        }

        [HttpGet]
        public async Task<OperateResult<IEnumerable<SetParmByEquipNosResponse>>> GetSetParmByEquipNos(
            [FromQuery] int[] equipList)
        {
            if (equipList == null || !equipList.Any())
            {
                return OperateResult.Failed<IEnumerable<SetParmByEquipNosResponse>>("请求参数为空");
            }

            return await _equipListService.GetSetParmByEquipNos(equipList.ToList());
        }

        [HttpPost]
        public OperateResult<PagedResult<string>> GetEquipSetParmList(CommonSearchPageModel commonSearchPageModel)
        {
            return _equipListService.GetEquipSetParmList(commonSearchPageModel);
        }


        [HttpPost]
        public OperateResult<PagedResult<EquipSetParmResonse>> GetEquipSetParmTreeList(
            CommonSearchPageModel commonSearchPageModel)
        {
            return _equipListService.GetEquipSetParmTreeList(commonSearchPageModel);
        }


        [HttpPost]
        public async Task<OperateResult<PagedResult<RealEquipSetParmListModel>>> GetRealEquipSetParmList(
            CommonSearchPageModel commonSearchPageModel)
        {
            return await _equipListService.GetRealEquipSetParmList(commonSearchPageModel);
        }

        #region 设备各状态数据导出

        [HttpGet]
        public async Task<IActionResult> ExportAbnormalRecord([FromQuery][Required(ErrorMessage = "请求参数不能为空")] int deviceStatus)
        {
            var dr = await _equipListService.ExportAbnormalRecord(deviceStatus);
            if (dr.Code != 200)
            {
                return BadRequest(OperateResult.Failed(dr.Message));
            }

            var bytes = dr.Data;
            if (bytes.Length <= 0)
            {
                return BadRequest(OperateResult.Failed("暂无相关状态数据"));
            }
            return File(bytes, MimeTypes.TextXlsx);
        }

        #endregion


        #region 设备历史曲线导出
        [HttpPost]
        public async Task<OperateResult> ExportEquipHistroyCurves([FromBody] ExportHistoryCuresModel model)
        {
            if (model == null || model.ExportEquips == null)
            {
                return OperateResult.Failed("输入的导出设备参数为空");
            }

            if (model.EndTime < model.BeginTime)
            {
                return OperateResult.Failed("开始时间小于结束时间");
            }

            var userName = HttpContext.User?.FindFirst(ClaimTypes.Name)?.Value;

            if (!exportLocker.IsSet)
            {
                return OperateResult.Failed("已有数据正在处理，请等待处理完毕");
            }

            var filePath = Path.Combine(AppContext.BaseDirectory, "wwwroot", "static",
                "DownloadFile", "HistoryCurves");

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            var modelGroup = model.ExportEquips.GroupBy(d => d.StaNo).SingleOrDefault();

            if (!modelGroup.Any())
                return OperateResult.Failed("输入的导出设备参数为空");


            var modelEquipArr = modelGroup.Select(p => p.EquipNo).ToArray();
            var equips = await _context.Equip.AsNoTracking()
                .Where(e => modelEquipArr.Contains(e.EquipNo))
                .ToArrayAsync();

            var ycUnion = modelGroup.Select(m => m.Ycps).Aggregate((c1, c2) => c1.Union(c2).ToArray());
            var ycps = await _context.Ycp.AsNoTracking()
                .Where(c => modelEquipArr.Contains(c.EquipNo) && ycUnion.Any(u => u == c.YcNo))
                .ToArrayAsync();

            var yxUnion = modelGroup.Select(m => m.Yxps).Aggregate((x1, x2) => x1.Union(x2).ToArray());
            var yxps = await _context.Yxp.AsNoTracking()
                .Where(x => modelEquipArr.Contains(x.EquipNo) && ycUnion.Any(u => u == x.YxNo))
                .ToArrayAsync();

            var equipYcCurves = (from e in equips
                                 join y in ycps
                                 on e.EquipNo equals y.EquipNo
                                 let tmp = modelGroup.SingleOrDefault(m => m.EquipNo == e.EquipNo)
                                 where tmp != null && tmp.Ycps.Contains(y.YcNo)
                                 select new YcHistoryCureEquipModel()
                                 {
                                     EquipNo = e.EquipNo,
                                     EquipName = e.EquipNm,
                                     YcpNo = y.YcNo,
                                     YcpName = y.YcNm
                                 }).ToList();


            var equipYxCurves = (from e in equips
                                 join y in yxps
                                 on e.EquipNo equals y.EquipNo
                                 let tmp = modelGroup.SingleOrDefault(m => m.EquipNo == e.EquipNo)
                                 where tmp != null && tmp.Yxps.Contains(y.YxNo)
                                 select new YxHistoryCureEquipModel()
                                 {
                                     EquipNo = e.EquipNo,
                                     EquipName = e.EquipNm,
                                     YxpNo = y.YxNo,
                                     YxpName = y.YxNm
                                 }).ToList();

            Thread exportThread = new Thread(async () =>
            {
                try
                {
                    exportLocker.Reset();

                    var fileName = $"HistroyCurves.zip";
                    var wholePath = Path.Combine(filePath, fileName);

                    using var stream = new FileStream(wholePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                    using var zipOutStream = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(stream);

                    if (model.IsMerge)
                    {
                        var inputModel = new ExportHistoryCuresModelInternal();
                        inputModel.StaNo = modelGroup.Key;
                        inputModel.BeginTime = model.BeginTime;
                        inputModel.EndTime = model.EndTime;
                        inputModel.YcHistory = equipYcCurves;
                        inputModel.YxHistory = equipYxCurves;

                        var dr = await _equipListService.ExportEquipHistroyCurves(inputModel);
                        if (dr.Code != 200)
                            return;

                        var zipName = $"合并历史曲线{model.BeginTime:MM-dd}-{model.EndTime:MM-dd}.zip";
                        var entry = new ICSharpCode.SharpZipLib.Zip.ZipEntry(zipName);
                        entry.IsUnicodeText = true;
                        entry.DateTime = DateTime.Now;
                        zipOutStream.PutNextEntry(entry);

                        var buffer = dr.Data;
                        zipOutStream.Write(buffer, 0, buffer.Length);

                        _apiLog.Info($" 【历史数据导出】已处理文件: {zipName}");
                        Console.Write($" 【历史数据导出】已处理文件: {zipName}");
                    }
                    else
                    {
                        int i = 0;
                        int dayLeng = (model.EndTime - model.BeginTime).Days;

                        do
                        {
                            var d = model.BeginTime.AddDays(i);
                            var inputModel = new ExportHistoryCuresModelInternal();
                            inputModel.StaNo = modelGroup.Key;
                            inputModel.BeginTime = d;
                            inputModel.EndTime = d.AddDays(1);
                            inputModel.YcHistory = equipYcCurves;
                            inputModel.YxHistory = equipYxCurves;

                            var dr = await _equipListService.ExportEquipHistroyCurves(inputModel);
                            if (dr.Code != 200)
                                continue;

                            var zipName = $"历史曲线{d:MM-dd}.zip";
                            var entry = new ICSharpCode.SharpZipLib.Zip.ZipEntry(zipName);
                            entry.IsUnicodeText = true;
                            entry.DateTime = DateTime.Now;
                            zipOutStream.PutNextEntry(entry);

                            var buffer = dr.Data;
                            zipOutStream.Write(buffer, 0, buffer.Length);

                            _apiLog.Info($" 【历史数据导出】已处理文件: {zipName}");
                            Console.Write($" 【历史数据导出】已处理文件: {zipName}");

                            i++;

                        } while (i <= dayLeng);
                    }

                    zipOutStream.Finish();

                    _apiLog.Info($" 【历史数据导出】Zip文件处理完毕");
                    Console.Write($" 【历史数据导出】Zip文件处理完毕");

                    _apiLog.Error($" 【历史数据导出】 download user sessions: {JsonConvert.SerializeObject(GlobalConst.DownloadFileSession)}");

                    if (GlobalConst.DownloadFileSession.TryGetValue(userName, out var session))
                        await _notifyHub.Clients.Client(session.SignalRConnectId).SendAsync("downloadUrl", $"/DownloadFile/{fileName}");
                }
                catch (Exception ex)
                {
                    _apiLog.Error($"【历史数据导出出错】：{ex}");

                    if (GlobalConst.DownloadFileSession.TryGetValue(userName, out var session))
                        await _notifyHub.Clients.Client(session.SignalRConnectId).SendAsync("downloadError", "系统内部异常，请联系管理员");
                }
                finally
                {
                    exportLocker.Set();
                }
            });

            exportThread.IsBackground = true;
            exportThread.Start();

            return OperateResult.Successed("数据正在整理中");
        }

        [HttpPost]
        public async Task<OperateResult<GetExportEquipYcYxpsRespones>> GetExportEquipYcYxps([FromBody] GetExportEquipYcYxpsRequest request)
        {
            if (request == null)
                return OperateResult.Failed<GetExportEquipYcYxpsRespones>("参数错误");

            return await _equipListService.GetExportEquipYcYxps(request);
        }

        #endregion

        [HttpGet]
        public async Task<IActionResult> ExportAllEquipList()
        {
            var dr = await _equipListService.ExportAllEquipList();
            if (dr.Code != 200)
            {
                return BadRequest(OperateResult.Failed(dr.Message));
            }

            var bytes = dr.Data;
            return File(bytes, MimeTypes.TextXlsx);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadEquipTemplateFile()
        {
            var dr = await _equipListService.DownloadEquipTemplateFile();
            if (dr.Code != 200)
            {
                return BadRequest(OperateResult.Failed(dr.Message));
            }

            var bytes = dr.Data;
            return File(bytes, MimeTypes.TextXlsx);
        }

        [HttpPost]
        public async Task<OperateResult> ImportEquipList(IFormFile excelFile)
        {
            return await _equipListService.ImportEquipList(excelFile);
        }
    }
}
