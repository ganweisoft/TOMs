//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;

namespace IoTCenterHost.AppServices.Domain.VO.MemoryData.Curve
{
    public class CurveDomainServiceImpl : ICurveDomainService
    {
        public byte[] GetCurveData1(DateTime d, int eqpno, int ycyxno, string type = "C")
        {
            return DataCenter.GetMyCurveData(d, eqpno, ycyxno, type);
        }
        public byte[] GetCurveData(DateTime d, int eqpno, int ycno)
        {
            return DataCenter.GetMyCurveData(d, eqpno, ycno);
        }

        public Task<List<DataCenter.myCurveData>> GetDataFromCurve(List<DateTime> DTList, int stano, int eqpno, int ycyxno, string type)
        {
            return DataCenter.GetDataFromCurveAsync(DTList, stano, eqpno, ycyxno, type);
        }

        public Task<List<DataCenter.myCurveData>> GetChangedDataFromCurveAsync(DateTime bgn, DateTime end, int stano, int eqpno, int ycyxno, string type)
        {
            return DataCenter.GetChangedDataFromCurveAsync(bgn, end, stano, eqpno, ycyxno, type);
        }
        public bool HaveHistoryCurve(int EquipNo, int YCPNo)
        {
            return StationItem.GetEquipItemFromEquipNo(EquipNo).GetYCItem(YCPNo).Curve_rcd;
        }
    }
}
