//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using IoTCenterHost.Core.Abstraction.AppServices;
using System.Collections.Generic;

namespace IoTCenterHost.Core.ServerInterfaces
{
    public interface IYCServerAppService : IYCAppService
    {
        List<YCItem> GetTotalYCDataEx(bool isDynamic);

    }
}
