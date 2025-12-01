//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.Core.Abstraction.BaseModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTCenterHost.Core.Abstraction.AppServices
{
    public interface ICurveAppService
    {
        byte[] GetCurveData(DateTime d, int eqpno, int ycno);
        byte[] GetCurveData1(DateTime d, int eqpno, int ycyxno, string type);

        bool HaveHistoryCurve(int EquipNo, int YCPNo);

    }

    public interface ICurveClientAppService : ICurveAppService
    {
        Task<List<GrpcMyCurveData>> GetDataFromCurve(List<DateTime> DTList, int stano, int eqpno, int ycyxno, string type);

        Task<List<GrpcMyCurveData>> GetChangedDataFromCurveAsync(DateTime bgn, DateTime end, int stano, int eqpno, int ycyxno, string type);

        void SetHistoryStorePeriod(int period);

        int GetHistoryStorePeriod();

        byte[] GetCurveData(DateTime d, int eqpno, int ycno);

        byte[] GetCurveData1(DateTime d, int eqpno, int ycyxno, string type);

        bool HaveHistoryCurve(int EquipNo, int YCPNo);

        Task<List<GrpcMyCurveData>> GetCurveDataAsync(DateTime bgn, DateTime end, int stano, int eqpno, int ycyxno, string type);
    }

}
