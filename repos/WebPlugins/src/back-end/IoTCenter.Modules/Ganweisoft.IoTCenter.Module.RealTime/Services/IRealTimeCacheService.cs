// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.Core.Abstraction;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.RealTime;

public interface IRealTimeCacheService
{
    Task<List<WcfRealTimeEventItem>> HandleRealTimeData(string role);
}
