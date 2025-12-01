//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.Core.Abstraction.BaseModels;
using IoTCenterHost.Core.Abstraction.IotModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTCenterHost.Core.Abstraction.AppServices
{
    public interface IAlarmEventAppService
    {
        void GetAddRealEventItem1();
        void GetDelRealEventItem1();
        void ConfirmedRealTimeEventItem(WcfRealTimeEventItem item);

        List<WcfRealTimeEventItem> GetLastRealEventItem(int lastCount);

        PaginationData GetRealEventItems(Pagination pagination);
        IEnumerable<RealTimeGroupCount> GetRealTimeGroupCount();

        WcfRealTimeEventItem GetRealTimeEventItem(string guid);
        bool Contains(string guid);


        Task<List<GrpcGwEventInfo>> GetGWEventInfoAsync(GrpcGetEventInfo grpcGetEventInfo);


    }

    public interface IAlarmEventClientAppService : IAlarmEventAppService
    {

        Task<List<WcfRealTimeEventItem>> FirstGetRealEventItemExAsync();

        List<WcfRealTimeEventItem> GetRealEventItem(bool isFirst = false);


    }
}
