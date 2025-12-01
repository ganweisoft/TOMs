//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Dapr;
using Dapr.Client;
using GWDataCenter;
using GWDataCenter.Database;
using IoTCenterHost.AppServices.Domain.PO;
using IoTCenterHost.Dapr.Models;
using IoTCenterHost.Dapr.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IoTCenterHost.Dapr.Controllers
{
    [ApiController]
    public class DataReportController : ControllerBase
    {
        private const string DriverName = "GW.DaprServer.Subscribe.STD.dll";
        private const string AppName = "open_datacenter";
        private const string PubSubName = "open_datacenter_pubsub";
        private static readonly List<string> EquipCategaryRegex = new () { @"^CSHARP\|", @"^JAVA\|", @"^PYTHON\|", @"^CPLUSPLUS\|" };

        private readonly ILogger<DataReportController> _logger;

        public DataReportController(ILogger<DataReportController> logger)
        {
            this._logger = logger;
        }

        /// <summary>
        /// Method for depositing to account as specified in transaction.
        /// </summary>
        /// <param name="data">Transaction info.</param>
        /// <param name="daprClient">State client to interact with Dapr runtime.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        ///  "open_datacenter", the first parameter into the Topic attribute, is name of the default pub/sub configured by the Dapr CLI.
        [Topic(PubSubName, "report")]
        [HttpPost("report")]
        public async Task<ActionResult> Report(DeviceDataExReport data, [FromServices] DaprClient daprClient)
        {
            var headerEntries = Request.Headers.Aggregate("",
                (current, header) =>
                    current + ($"------- Header: {header.Key} : {header.Value}" + Environment.NewLine));

            _logger.LogInformation(headerEntries);
            if (data == null || data.DataItems == null || data.DataItems.Count <= 0)
            {
                return NoContent();
            }

            foreach (var item in data.DataItems)
            {
                var equipNo = item.DeviceId;
                var equipItem = DataCenter.GetEquipItem(equipNo);

                if (equipItem == null)
                {
                    _logger.LogWarning($"根据deviceId转换成equipNo，使用equipNo获取设备对象时为空,DeviceId:{item.DeviceId}",
                        LogLevel.Error);
                    continue;
                }

                switch (data.DataType)
                {
                    case 1:
                    {
                        var ycDictionary = equipItem.YCItemDict.ToDictionary(x => x.Value.Yc_no);

                        foreach (var kvp in item.Attribute)
                        {
                            if (!ycDictionary.TryGetValue(kvp.Key, out var ycp))
                                continue;

                            ycp.Value.YCValue = decimal.TryParse(kvp.Value.ToString(), out var val)
                                ? val
                                : kvp.Value;
                        }

                        break;
                    }
                    case 2:
                    {
                        var yxDictionary = equipItem.YXItemDict.ToDictionary(x => x.Value.Yx_no);

                        foreach (var kvp in item.Attribute)
                        {
                            if (!yxDictionary.TryGetValue(kvp.Key, out var yxp))
                                continue;

                            if (kvp.Value.ToString() == yxp.Value.Evt_01 ||
                                kvp.Value?.ToString()?.ToLower() == "true")
                            {
                                yxp.Value.YXValue = true;
                            }
                            else
                            {
                                yxp.Value.YXValue = false;
                            }
                        }

                        break;
                    }
                }
            }

            return new OkResult();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        //[Topic(PubSubName, "equip")]
        [HttpPost("equip")]
        public async Task<ActionResult> Equip([FromBody] EquipCategary categary)
        {
            var equipWrappers = StationItem.db_Eqp
                .Where(v => !string.IsNullOrEmpty(v.equip_detail) &&
                            v.equip_detail.Contains(categary.Categary, StringComparison.OrdinalIgnoreCase) &&
                            EquipCategaryRegex.Any(c => Regex.IsMatch(v.equip_detail, c)) &&
                            v.communication_drv == DriverName)
                .Select(v => new EquipWrapper
                {
                    EquipNo = v.equip_no,
                    StaN = v.sta_n,
                    EquipNm = v.equip_nm,
                    EquipDetail = v.equip_detail,
                    AccCyc = v.acc_cyc,
                    RelatedPic = v.related_pic,
                    ProcAdvice = v.proc_advice,
                    OutOfContact = v.out_of_contact,
                    Contacted = v.contacted,
                    EventWav = v.event_wav,
                    CommunicationDrv = v.equip_detail.Split('|')[1],
                    LocalAddr = v.local_addr,
                    EquipAddr = v.equip_addr,
                    CommunicationParam = v.communication_param,
                    CommunicationTimeParam = v.communication_time_param,
                    RawEquipNo = v.raw_equip_no,
                    Tabname = v.tabname,
                    AlarmScheme = v.alarm_scheme,
                    Attrib = v.attrib,
                    StaIp = v.sta_IP,
                    AlarmRiseCycle = v.AlarmRiseCycle,
                    Reserve1 = v.Reserve1,
                    Reserve2 = v.Reserve2,
                    Reserve3 = v.Reserve3,
                    RelatedVideo = v.related_video,
                    ZiChanId = v.ZiChanID,
                    PlanNo = v.PlanNo,
                    SafeTime = v.SafeTime,
                    Backup = v.backup,
                    Ycps = StationItem.db_Ycp.Where(e => e.equip_no == v.equip_no)
                        .Select(MapHelper.ConvertToWrapper).ToList(),
                    Yxps = StationItem.db_Yxp.Where(e => e.equip_no == v.equip_no)
                        .Select(MapHelper.ConvertToWrapper).ToList(),
                    SetParms = StationItem.db_Setparm.Where(e => e.equip_no == v.equip_no)
                        .Select(MapHelper.ConvertToWrapper).ToList()
                }).ToList();

            // 区分Java，CSharp, Python
            var sendRuntimeEvts = new List<RuntimeEquipSyncEvent>();
            foreach (var eGroup in equipWrappers.GroupBy(e => e.EquipDetail.Split('|')[0]))
            {
                var addOne = new RuntimeEquipSyncEvent
                {
                    Flow = Guid.NewGuid().ToString("N"),
                    FlowType = EquipCategaryRegex.FindIndex(c => c.Contains(eGroup.Key)),
                    AppInstanceId = AppName,
                    Equips = eGroup.ToList()
                };

                sendRuntimeEvts.Add(addOne);
            }

            return Ok(sendRuntimeEvts);
        }

        // TODO: 1、通讯故障，检测原理：dapr的连接状态，设备自我状态topic发布
        // TODO: 2、测试
        // TODO: 3、文档
    }
}