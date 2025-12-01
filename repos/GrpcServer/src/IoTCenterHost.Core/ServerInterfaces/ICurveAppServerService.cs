//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.Core.Abstraction.AppServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static GWDataCenter.DataCenter;

namespace IoTCenterHost.Core.ServerInterfaces
{
    public interface ICurveServerAppService : ICurveAppService
    {
        Task<List<myCurveData>> GetDataFromCurve(List<DateTime> DTList, int stano, int eqpno, int ycyxno, string type);
        Task<List<myCurveData>> GetChangedDataFromCurveAsync(DateTime bgn, DateTime end, int stano, int eqpno, int ycyxno, string type);
    }
}
