//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using IoTCenterHost.AppServices.Domain.DO.RemoteStatus;
using IoTCenterHost.AppServices.Interfaces;
using IoTCenterHost.Core.Abstraction;
using IoTCenterHost.Core.Abstraction.IotModels;
using IoTCenterHost.Core.Extension;
using IoTCenterHost.Core.ServerInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenGWDataCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IoTCenterHost.AppServices.AppServices
{
    public class YXAppServiceImpl : BaseAppServiceImpl, IYXServerAppService
    {
        private readonly IRemoteStatusRepository _remoteStatusRepository;
        private readonly IotRealTimeDataService _iotRealTimeDataService;
        private readonly ILogger<YXAppServiceImpl> _logger;

        public YXAppServiceImpl(IRemoteStatusRepository remoteStatusRepository, IotRealTimeDataService iotRealTimeDataService, IHttpContextAccessor contextAccessor, ILogger<YXAppServiceImpl> logger) : base(contextAccessor, logger)
        {
            _remoteStatusRepository = remoteStatusRepository;
            _iotRealTimeDataService = iotRealTimeDataService;
            _logger = logger;
        }

        public void GetChangedRTYXItemData()
        {
            _iotRealTimeDataService.GetIotRealTimeData(base.ConnectId, base.GetLoginUser()).GetChangedRTYXItemData();
        }

        public void GetChangedRTYXItemData1()
        {
            _iotRealTimeDataService.GetIotRealTimeData(base.ConnectId, base.GetLoginUser()).GetChangedRTYXItemData();
        }

        public void GetTotalRTYXItemData()
        {
        }

        public void GetTotalRTYXItemData1()
        {

        }

        public void GetTotalYXData(bool isDynamic, Action<List<YXItem>> action)
        {
        }
        public List<YXItem> GetTotalYXDataEx(bool isDynamic)
        {
            return _iotRealTimeDataService.GetIotRealTimeData(base.ConnectId).GetTotalRTYXItemDataEx();
        }
        public string GetYXAlarmComments(int iEqpNo, int iYXPNo)
        {
            return _remoteStatusRepository.GetYXAlarmComments(iEqpNo, iYXPNo);

        }

        public bool GetYXAlarmState(int iEquipNo, int iYxpNo)
        {
            return _remoteStatusRepository.GetYXAlarmState(iEquipNo, iYxpNo);
        }

        public Dictionary<int, bool> GetYXAlarmStateDictFromEquip(int iEquipNo)
        {
            return _remoteStatusRepository.GetYXAlarmStateDictFromEquip(iEquipNo);
        }

        public string GetYXEvt01(int iEquipNo, int iYxpNo)
        {
            return _remoteStatusRepository.GetYXEvt01(iEquipNo, iYxpNo);
        }

        public string GetYXEvt10(int iEquipNo, int iYxpNo)
        {
            return _remoteStatusRepository.GetYXEvt10(iEquipNo, iYxpNo);
        }

        public PaginationData GetYXPDict(Pagination pagination)
        {
            var result = _iotRealTimeDataService.GetIotRealTimeData(base.ConnectId).CreateProxyYXItems();

            return new PaginationData { Data = result.ToJson() };
        }

        public PaginationData GetYXPListByEquipNo(Pagination pagination)
        {
            int equipNo = int.Parse(pagination.WhereCause);
            var ycpTableRows = _remoteStatusRepository.GetYxpTableRows(equipNo, out int total).Skip((pagination.PageIndex - 1) * pagination.PageSize).Take(pagination.PageSize);
            return new PaginationData
            {
                Data = ycpTableRows.Select(u => new YxpList
                {
                    YxNm = u.yx_nm,
                    YxNo = u.yx_no,
                    EquipNo = u.equip_no,
                    PlanNo = u.PlanNo,
                    ProcAdvice = u.proc_advice_d,
                    RelatedPic = u.related_pic,
                    RelatedVideo = u.related_video,
                    StaN = u.sta_n,
                    State = GetYXAlarmState(equipNo, u.yx_no),
                    Value = GetYXValue(equipNo, u.yx_no).ToString(),
                    ZiChanID = u.ZiChanID,
                }).ToJson(),
                Total = total,
            };
        }

        public string GetYXPListStr(int iEquipNo)
        {
            return _remoteStatusRepository.GetYXPListStr(iEquipNo);
        }

        public object GetYXValue(int iEquipNo, int iYxpNo)
        {
            return _remoteStatusRepository.GetYXValue(iEquipNo, iYxpNo);
        }

        public Dictionary<int, string> GetYXValueDictFromEquip(int iEquipNo)
        {
            return _remoteStatusRepository.GetYXValueDictFromEquip(iEquipNo);
        }

        public bool HaveYXP(int EquipNo)
        {
            return _remoteStatusRepository.HaveYXP(EquipNo);
        }
        public void SetYxpNm(int EquipNo, int YxpNo, string Nm)
        {
            _remoteStatusRepository.SetYxpNmAsync(EquipNo, YxpNo, Nm);
        }

    }
}
