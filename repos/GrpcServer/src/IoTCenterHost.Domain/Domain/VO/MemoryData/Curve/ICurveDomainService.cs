//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using static GWDataCenter.DataCenter;

namespace IoTCenterHost.AppServices.Domain.VO.MemoryData.Curve
{
    public interface ICurveDomainService
    {
        byte[] GetCurveData1(DateTime d, int eqpno, int ycyxno, string type = "C");
        byte[] GetCurveData(DateTime d, int eqpno, int ycno);
        Task<List<myCurveData>> GetDataFromCurve(List<DateTime> DTList, int stano, int eqpno, int ycyxno, string type);
        Task<List<myCurveData>> GetChangedDataFromCurveAsync(DateTime bgn, DateTime end, int stano, int eqpno, int ycyxno, string type);

        bool HaveHistoryCurve(int EquipNo, int YCPNo);
    }
}
