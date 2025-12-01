//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GWDataCenter;
using IoTCenterHost.Core.Extension;
using IoTCenterHost.Proto;
using Microsoft.AspNetCore.Authorization;

namespace IoTCenterHost.AppServices.GrpcConstracts.IotHostService
{
    [AllowAnonymous]
    public partial class IotHostService : IotService.IotServiceBase
    {
        public override async Task<StringResult> GetChangedDataFromCurveAsync(GetChangedDataFromCurveAsyncRequest request, ServerCallContext context)
        {
            var result = await this.CurveAppService.GetChangedDataFromCurveAsync(DateTime.ParseExact(request.Bgn, "yyyy-MM-dd HH:mm:ss fff", null), DateTime.ParseExact(request.End, "yyyy-MM-dd HH:mm:ss fff", null), request.Stano, request.Eqpno, request.Ycyxno, request.Type);
            return new StringResult { Result = result.ToJson() };
        }


        public override Task<ByteArrDefine> GetCurveData(GetCurveDataRequest request, ServerCallContext context)
        {
            var result = this.CurveAppService.GetCurveData(Convert.ToDateTime(request.D), request.Eqpno, request.Ycno);
            ByteArrDefine byteArrDefine = new ByteArrDefine();
            byteArrDefine.Result.Add(result.ConvertByteToByteString());
            return Task.FromResult(byteArrDefine);
        }
        public override Task<ByteArrDefine> GetCurveData1(GetCurveData1Request request, ServerCallContext context)
        {
            var result = this.CurveAppService.GetCurveData1(DateTime.ParseExact(request.D, "yyyy-MM-dd HH:mm:ss fff", null), request.Eqpno, request.Ycyxno, request.Type);
            ByteArrDefine byteArrDefine = new ByteArrDefine();
            if (result != null)
                byteArrDefine.Result.Add(result.ConvertByteToByteString());
            return Task.FromResult(byteArrDefine);
        }
        public override Task<StringResult> GetDataFromCurve(GetDataFromCurveRequest request, ServerCallContext context)
        {
            var result = this.CurveAppService.GetDataFromCurve(request.DTList.FromJson<List<DateTime>>(), request.Stano, request.Eqpno, request.Ycyxno, request.Type).Result;
            return Task.FromResult(new StringResult { Result = result.ToJson() });
        }
        public override Task<BoolDefine> HaveHistoryCurve(HaveHistoryCurveRequest request, ServerCallContext context)
        {
            var result = this.CurveAppService.HaveHistoryCurve(request.EquipNo, request.YCPNo);
            return Task.FromResult(new BoolDefine { Result = result });
        }

        public override Task<Empty> SetHistoryStorePeriod(IntegerDefine request, ServerCallContext context)
        {
            DataCenter.SetPropertyToPropertyService("AlarmCenter.Gui.OptionPanels.CurveOptions", "History_CurveStoreTime", request.Result.ToString());
            return Task.FromResult(new Empty());
        }

        public override Task<BaseResult> GetHistoryStorePeriod(Empty request, ServerCallContext context)
        {
            var result = DataCenter.GetPropertyFromPropertyService("AlarmCenter.Gui.OptionPanels.CurveOptions", "History_CurveStoreTime", "");
            return Task.FromResult(new BaseResult { Result = result });
        }
    }
}
