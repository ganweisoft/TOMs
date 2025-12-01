//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using IoTCenterHost.AppServices.Domain.DO.RemoteValue;
using IoTCenterHost.AppServices.Interfaces;
using IoTCenterHost.Core.Abstraction;
using IoTCenterHost.Core.Abstraction.IotModels;
using IoTCenterHost.Core.Extension;
using IoTCenterHost.Core.ServerInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace IoTCenterHost.AppServices.AppServices
{
    public class YCAppServiceImpl : BaseAppServiceImpl, IYCServerAppService
    {
        private readonly IRemoteValueRepository _remoteValueRepository;
        private readonly IotCenterService _iotCenterService;
        private readonly IotRealTimeDataService _iotRealTimeDataService;
        private readonly ILogger<YCAppServiceImpl> _logger;

        public YCAppServiceImpl(IRemoteValueRepository remoteValueRepository, IotRealTimeDataService iotRealTimeDataService, IotCenterService iotCenterService, IHttpContextAccessor contextAccessor, ILogger<YCAppServiceImpl> logger) : base(contextAccessor, logger)
        {
            _remoteValueRepository = remoteValueRepository;
            _iotCenterService = iotCenterService;
            _iotRealTimeDataService = iotRealTimeDataService;
            _logger = logger;
        }
        [Obsolete]
        public void GetChangedRTYCItemData()
        {
            _iotRealTimeDataService.GetIotRealTimeData(base.ConnectId, base.GetLoginUser()).GetChangedRTYCItemData();
        }
        [Obsolete]
        public void GetChangedRTYCItemData1()
        {
            _iotRealTimeDataService.GetIotRealTimeData(base.ConnectId, base.GetLoginUser()).GetChangedRTYCItemData();
        }
        [Obsolete]
        public void GetTotalRTYCItemData()
        {
        }
        [Obsolete]
        public void GetTotalRTYCItemData1()
        {
        }
        [Obsolete]
        public void GetTotalYCData(bool isDynamic, Action<List<YCItem>> action)
        {
        }
        public List<YCItem> GetTotalYCDataEx(bool isDynamic)
        {
            return _iotRealTimeDataService.GetIotRealTimeData(base.ConnectId).GetTotalRTYCItemDataEx();
        }
        public string GetYCAlarmComments(int iEqpNo, int iYCPNo)
        {
            return _remoteValueRepository.GetYCAlarmComments(iEqpNo, iYCPNo);
        }

        public bool GetYCAlarmState(int iEquipNo, int iYcpNo)
        {
            return _remoteValueRepository.GetYCAlarmState(iEquipNo, iYcpNo);
        }

        public Dictionary<int, bool> GetYCAlarmStateDictFromEquip(int iEquipNo)
        {
            return _remoteValueRepository.GetYCAlarmStateDictFromEquip(iEquipNo);
        }

        public PaginationData GetYCPDict(Pagination pagination)
        {
            var result = _iotRealTimeDataService.GetIotRealTimeData(base.ConnectId, base.GetLoginUser()).CreateProxyYCItems();
            return new PaginationData
            {
                Data = result.ToJson()
            };
        }

        public PaginationData GetYCPListByEquipNo(Pagination pagination)
        {
            int equipNo = int.Parse(pagination.WhereCause);
            var ycpTableRows = _remoteValueRepository.GetYcpTableRows(equipNo, out int total).Skip((pagination.PageIndex - 1) * pagination.PageSize).Take(pagination.PageSize).ToList();
            var result = ycpTableRows.Select(u => new YcpList
            {
                YcNm = u.yc_nm,
                YcNo = u.yc_no.Value,
                EquipNo = u.equip_no.Value,
                PlanNo = u.PlanNo,
                ProcAdvice = u.proc_advice,
                RelatedPic = u.related_pic,
                Unit = u.unit,
                RelatedVideo = u.related_video,
                StaN = u.sta_n.Value,
                ZiChanID = u.ZiChanID,
            }).ToList();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            result.ForEach((item) =>
            {
                var ycItem = _remoteValueRepository.GetYCItem(equipNo, item.YcNo);
                item.State = ycItem != null ? ycItem.IsAlarm : false;
                item.Value = ycItem != null ? ycItem.YCValue.ToString() : "0";
            });
            stopwatch.Stop();
            return new PaginationData
            {
                Data = result.ToJson(),
                Total = total,
            };
        }

        public string GetYCPListStr(int iEquipNo)
        {
            return _remoteValueRepository.GetYCPListStr(iEquipNo);
        }

        public object GetYCValue(int iEquipNo, int iYcpNo)
        {
            return _remoteValueRepository.GetYCValue(iEquipNo, iYcpNo);
        }

        public Dictionary<int, object> GetYCValueDictFromEquip(int iEquipNo)
        {
            return _remoteValueRepository.GetYCValueDictFromEquip(iEquipNo);
        }

        public bool HaveYCP(int EquipNo)
        {
            return _remoteValueRepository.HaveYCP(EquipNo);
        }

        public void SetYcpNm(int EquipNo, int YcpNo, string Nm)
        {
            _remoteValueRepository.SetYcpNm(EquipNo, YcpNo, Nm);
        }

    }
}
