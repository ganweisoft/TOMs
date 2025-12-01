// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Data.Model;
using IoTCenter.Utilities;
using IoTCenterCore.ExcelHelper;
using IoTCenterWebApi.BaseCore;
using IoTCenterWebApi.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.RealTime.Controllers
{
    [ApiController]
    [Route("/IoT/api/v3/[controller]/[action]")]
    public class RealTimeController : DefaultController
    {
        private readonly IExportManager _exportManager;
        private readonly IRealTimeService _realTimeService;

        private readonly IRealTimeCacheService _realTimeCacheService;
        private readonly ILoggingService _apiLog;

        public RealTimeController(IRealTimeService realTimeService,
            ILoggingService apiLog,
            IRealTimeCacheService realTimeCacheService,
            IExportManager exportManager)
        {
            _realTimeService = realTimeService;

            _realTimeCacheService = realTimeCacheService;
            _apiLog = apiLog;
            _exportManager = exportManager;
        }

        [HttpGet]
        public async Task<OperateResult<IEnumerable<GwsnapshotConfig>>> GetRealTimeEventTypeConfig()
        {
            return await _realTimeService.GetRealTimeEventTypeConfig();
        }

        [HttpGet]
        [DoNotRefreshSlidingExpiration]
        public async Task<OperateResult<IEnumerable<RealTimeEventCount>>> GetRealTimeEventCount()
        {
            return await _realTimeService.GetRealTimeEventCount();
        }

        [HttpPost]
        [DoNotRefreshSlidingExpiration]
        public OperateResult<RealTimeEventByType> GetRealTimeEvent([FromBody] RealTimePageModel realTimePageModel)
        {
            return _realTimeService.GetRealTimeEventByType(realTimePageModel);
        }

        [HttpPost]
        public IActionResult BatchExportRealTime([FromQuery] List<string> types)
        {
            if (types == null || types.Count <= 0)
            {
                return File(Array.Empty<byte>(), MimeTypes.TextXlsx);
            }

            List<int> ttypes;

            try
            {
                ttypes = JsonConvert.DeserializeObject<List<int>>(types[0]);
            }
            catch (Exception)
            {
                return BadRequest(OperateResult.Failed("请求参数不正确"));
            }

            if (ttypes.Count <= 0)
            {
                return BadRequest(OperateResult.Failed("请求参数不正确"));
            }

            var result = _realTimeService.GetBatchImportRealTimes(ttypes);
            var realTimes = result.Data;

            var workbook = new XSSFWorkbook();

            var fileName = $"{DateTime.Now:yyyy-MM-dd}-实时快照.xlsx";
            _exportManager.ExportToXlsx(workbook, GetAssetPropertyByNames(), realTimes, fileName);

            using var stream = new MemoryStream();
            workbook.Write(stream);

            var userName = HttpContext.User?.FindFirst(m => m.Type == ClaimTypes.Name)?.Value;
            
            _apiLog.Info(
                $"用户\"[{userName}]\"-导出实时快照文件名:{fileName}-成功");

            return File(stream.ToArray(), MimeTypes.TextXlsx);
        }


        private static PropertyByName<BatchImportRealTimeModel>[] GetAssetPropertyByNames()
        {
            var propertyByNames = new PropertyByName<BatchImportRealTimeModel>[]
            {
                new PropertyByName<BatchImportRealTimeModel>("事件详情", d => d.EventMsg),
                new PropertyByName<BatchImportRealTimeModel>("事件时间", d => d.Time),
                new PropertyByName<BatchImportRealTimeModel>("设备号", d => d.Equipno),
                new PropertyByName<BatchImportRealTimeModel>("设备名称", d => d.EquipName),
                new PropertyByName<BatchImportRealTimeModel>("遥测遥信号", d => d.Ycyxno),
                new PropertyByName<BatchImportRealTimeModel>("遥测遥信类型", d => d.Type),
                new PropertyByName<BatchImportRealTimeModel>("是否已确认", d => d.bConfirmed),
                new PropertyByName<BatchImportRealTimeModel>("确认人", d => d.UserConfirm),
                new PropertyByName<BatchImportRealTimeModel>("确认时间", d => d.DTConfirm),
                new PropertyByName<BatchImportRealTimeModel>("处理意见", d => d.ProcAdviceMsg),
                new PropertyByName<BatchImportRealTimeModel>("资产编号", d => d.ZiChanID),
                new PropertyByName<BatchImportRealTimeModel>("资产名称", d => d.ZiChanName)
            };

            return propertyByNames;
        }

        [HttpPost]
        public OperateResult<PagedResult<RealTimeEventList>> GetConfirmedRealTimeEvent(RealTimePageModel realTimePageModel)
        {
            return _realTimeService.GetConfirmedRealTimeEventByType(realTimePageModel);
        }

        [HttpPost]
        [SkipCustomFilter]
        public OperateResult ConfirmedRealTimeEvent([FromBody] ConfirmRealTimeModel confirmRealTimeModel)
        {
            return _realTimeService.ConfirmRealTimeEvent(confirmRealTimeModel);
        }

        [HttpGet]
        [DoNotRefreshSlidingExpiration]
        public string GetRealTimeData()
        {
            var roleName = HttpContext.User?.FindFirst(m => m.Type == ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(roleName))
            {
                return JsonConvert.SerializeObject(OperateResult.Failed("请求参数为空"));
            }

            var data = _realTimeCacheService.HandleRealTimeData(roleName);
            try
            {
                data.Wait();
            }
            catch (System.Exception ex)
            {
                _apiLog.Error($"GetRealTimeData:{ex}");
            }

            if (data.Status == TaskStatus.Faulted)
            {
                return JsonConvert.SerializeObject(new
                {
                    code = 400,
                    message = "请求失败",
                    succeeded = false,
                }, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            }
            else
            {
                return JsonConvert.SerializeObject(new
                {
                    data,
                    code = 200,
                    message = "Success",
                    succeeded = true,
                }, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            }
        }

        [HttpPost]
        public OperateResult<RealTimeEventByType> GetRealTimeEventFitter(RealTimeFilterPageModel realTimePageModel)
        {
            return _realTimeService.GetRealTimeEventFitter(realTimePageModel);
        }
    }
}
