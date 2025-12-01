// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Ganweisoft.IoTCenter.Module.RealTime;
using IoTCenter.Data.Model;
using IoTCenter.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTCenterWebApi.Service
{
    public interface IRealTimeService
    {
        Task<OperateResult<IEnumerable<GwsnapshotConfig>>> GetRealTimeEventTypeConfig();

        Task<OperateResult<IEnumerable<RealTimeEventCount>>> GetRealTimeEventCount();

        OperateResult<RealTimeEventByType> GetRealTimeEventByType(RealTimePageModel realTimePageModel);

        OperateResult<RealTimeEventByType> GetRealTimeEventFitter(RealTimeFilterPageModel realTimePageModel);
        OperateResult<PagedResult<RealTimeEventList>> GetConfirmedRealTimeEventByType(RealTimePageModel realTimePageModel);

        OperateResult ConfirmRealTimeEvent(ConfirmRealTimeModel confirmRealTimeModel);

        OperateResult<List<BatchImportRealTimeModel>> GetBatchImportRealTimes(List<int> types);
    }
}
