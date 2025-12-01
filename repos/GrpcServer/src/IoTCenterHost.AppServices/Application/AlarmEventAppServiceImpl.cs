//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using IoTCenterHost.AppServices.Interfaces;
using IoTCenterHost.Core.Abstraction;
using IoTCenterHost.Core.Abstraction.BaseModels;
using IoTCenterHost.Core.Abstraction.IotModels;
using IoTCenterHost.Core.Extension;
using IoTCenterHost.Core.ServerInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace IoTCenterHost.AppServices.AppServices
{
    public class AlarmEventAppServiceImpl : BaseAppServiceImpl, IAlarmEventServerAppService
    {
        private readonly IotCenterService iotCenterService;
        private readonly IotRealTimeDataService iotRealTimeDataService;
        protected readonly IMemoryCacheService _memoryCacheService;
        private readonly ILogger<AccountAppServiceImpl> _logger;

        public AlarmEventAppServiceImpl(IotCenterService iotCenter, IotRealTimeDataService iotRealTimeData, IHttpContextAccessor contextAccessor, IMemoryCacheService memoryCache, ILogger<AccountAppServiceImpl> logger) : base(contextAccessor, logger)
        {
            iotCenterService = iotCenter;
            iotRealTimeDataService = iotRealTimeData;
            _memoryCacheService = memoryCache;
        }

        public void ConfirmedRealTimeEventItem(WcfRealTimeEventItem item)
        {
            RealTimeEventItem ConfirmedRealTimeEventItem = item.Map<RealTimeEventItem>();
            ConfirmedRealTimeEventItem.bConfirmed = item.bConfirmed;
            ConfirmedRealTimeEventItem.DT_Confirm = item.DT_Confirmed;
            ConfirmedRealTimeEventItem.Equipno = item.Equipno;
            ConfirmedRealTimeEventItem.EventMsg = item.EventMsg;
            ConfirmedRealTimeEventItem.Level = item.Level;
            ConfirmedRealTimeEventItem.Proc_advice_Msg = item.Proc_advice_Msg;
            ConfirmedRealTimeEventItem.Time = item.Time;
            ConfirmedRealTimeEventItem.Type = item.Type;
            ConfirmedRealTimeEventItem.User_confirm = item.User_Confirmed;
            ConfirmedRealTimeEventItem.Wavefile = item.Wavefile;
            ConfirmedRealTimeEventItem.Related_pic = item.Related_pic;
            ConfirmedRealTimeEventItem.Ycyxno = item.Ycyxno;
            ConfirmedRealTimeEventItem.Related_video = item.m_related_video;
            ConfirmedRealTimeEventItem.PlanNo = item.PlanNo;
            ConfirmedRealTimeEventItem.ZiChanID = item.ZiChanID;
            ConfirmedRealTimeEventItem.GUID = item.GUID;
            iotCenterService.AddConfirmedMessage(ConfirmedRealTimeEventItem);
        }



        public bool Contains(string guid)
        {
            return iotRealTimeDataService.GetIotRealTimeData(base.ConnectId, base.GetLoginUser()).Contains(guid); ;
        }

        [Obsolete]
        public void FirstGetRealEventItem(Action<ObservableCollection<RealTimeEventItem>> action)
        {

        }

        public List<WcfRealTimeEventItem> FirstGetRealEventItem(bool isFirst = false)
        {
            return iotRealTimeDataService.GetIotRealTimeData(base.ConnectId).FirstGetRealEventItem(isFirst);
        }

        public void GetAddRealEventItem1()
        {
            iotRealTimeDataService.GetIotRealTimeData(base.ConnectId, base.GetLoginUser()).GetAddRealEventItem();
        }

        public void GetDelRealEventItem1()
        {
            iotRealTimeDataService.GetIotRealTimeData(base.ConnectId, base.GetLoginUser()).GetDelRealEventItem();
        }

        public Task<List<GrpcGwEventInfo>> GetGWEventInfoAsync(GrpcGetEventInfo grpcGetEventInfo)
        {
            return null;
        }

        public List<WcfRealTimeEventItem> GetLastRealEventItem(int lastCount)
        {
            return iotRealTimeDataService.GetIotRealTimeData(base.ConnectId, base.GetLoginUser()).FirstGetRealEventItem(false, lastCount);
        }

        public PaginationData GetRealEventItems(Pagination pagination)
        {
            return iotRealTimeDataService.GetIotRealTimeData(base.ConnectId, base.GetLoginUser()).CreatePaginationRealEventItems(pagination);
        }

        public WcfRealTimeEventItem GetRealTimeEventItem(string guid)
        {
            return iotRealTimeDataService.GetIotRealTimeData(base.ConnectId, base.GetLoginUser()).GetRealTimeEventItem(guid);
        }

        public IEnumerable<RealTimeGroupCount> GetRealTimeGroupCount()
        {
            return iotRealTimeDataService.GetIotRealTimeData(base.ConnectId, base.GetLoginUser()).GetRealTimeGroupCount();
        }
    }
}
