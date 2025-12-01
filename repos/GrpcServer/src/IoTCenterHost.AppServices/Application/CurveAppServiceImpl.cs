//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using IoTCenterHost.AppServices.Domain.VO.MemoryData.Curve;
using IoTCenterHost.Core.ServerInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static GWDataCenter.DataCenter;

namespace IoTCenterHost.AppServices.AppServices
{
    public class CurveAppServiceImpl : BaseAppServiceImpl, ICurveServerAppService
    {
        private readonly ICurveDomainService _curveDomainService;
        private readonly ILogger<CurveAppServiceImpl> _logger;

        public CurveAppServiceImpl(ICurveDomainService curveDomainService, IHttpContextAccessor contextAccessor, ILogger<CurveAppServiceImpl> logger) : base(contextAccessor, logger)
        {
            _curveDomainService = curveDomainService;
            _logger = logger;
        }

        public Task<List<myCurveData>> GetChangedDataFromCurveAsync(DateTime bgn, DateTime end, int stano, int eqpno, int ycyxno, string type)
        {
            return _curveDomainService.GetChangedDataFromCurveAsync(bgn, end, stano, eqpno, ycyxno, type);
        }

        public byte[] GetCurveData(DateTime d, int eqpno, int ycno)
        {
            return _curveDomainService.GetCurveData(d, eqpno, ycno);
        }

        public byte[] GetCurveData1(DateTime d, int eqpno, int ycyxno, string type)
        {
            return _curveDomainService.GetCurveData1(d, eqpno, ycyxno, type);

        }

        public Task<List<myCurveData>> GetDataFromCurve(List<DateTime> DTList, int stano, int eqpno, int ycyxno, string type)
        {
            return DataCenter.GetDataFromCurveAsync(DTList, stano, eqpno, ycyxno, type);
        }
        public bool HaveHistoryCurve(int EquipNo, int YCPNo)
        {
            return _curveDomainService.HaveHistoryCurve(EquipNo, YCPNo);
        }
    }
}
