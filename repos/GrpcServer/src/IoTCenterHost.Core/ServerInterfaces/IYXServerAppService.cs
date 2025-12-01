//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using IoTCenterHost.Core.Abstraction.AppServices;
using OpenGWDataCenter.Model;
using System.Collections.Generic;

namespace IoTCenterHost.Core.ServerInterfaces
{
    public interface IYXServerAppService : IYXAppService
    {
        List<YXItem> GetTotalYXDataEx(bool isDynamic);
    }
}
