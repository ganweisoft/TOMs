//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IoTCenterHost.AppServices.Interfaces;
using IoTCenterHost.Core.Extension;
using IoTCenterHost.Proto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
namespace IoTCenterHost.GrpcConstract.GrpcConstract
{
    [Authorize]
    public class IotSubgatewayService : iotsubgatewayContract.iotsubgatewayContractBase
    {
        private readonly IotRealTimeDataService _iotRealTimeDataService;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        private IHttpContextAccessor _httpContextAccessor
        {
            get
            {
                return _serviceScopeFactory.CreateScope().ServiceProvider.GetService<IHttpContextAccessor>();
            }
        }


        public IotSubgatewayService(IotRealTimeDataService iotRealTimeDataService, IServiceScopeFactory serviceScopeFactory)
        {
            _iotRealTimeDataService = iotRealTimeDataService;
            _serviceScopeFactory = serviceScopeFactory;
        }
        private static Task AwaitCancellation(CancellationToken token)
        {
            var completion = new TaskCompletionSource<object>();
            token.Register(() => completion.SetResult(null));
            return completion.Task;
        }

        [AllowAnonymous]
        public override async Task AddRealTimeSnapshot(Empty request, IServerStreamWriter<StringResult> responseStream, ServerCallContext context)
        {
            _iotRealTimeDataService.GetRealTimeDataItem().AddRealTimeSnapshot += (sender, e) =>
            {
                responseStream.WriteAsync(new StringResult { Result = e.ToJson() });
            };
            await AwaitCancellation(context.CancellationToken);
        }


        [AllowAnonymous]
        public override async Task DeleteRealTimeSnapshot(Empty request, IServerStreamWriter<StringResult> responseStream, ServerCallContext context)
        {
            _iotRealTimeDataService.GetRealTimeDataItem().DeleteRealTimeSnapshot += (sender, e) =>
            {
                responseStream.WriteAsync(new StringResult { Result = e.ToJson() });
            };
            await AwaitCancellation(context.CancellationToken);
        }

        [AllowAnonymous]
        public override async Task YcChangeEvent(Empty request, IServerStreamWriter<YcItemResponseList> responseStream, ServerCallContext context)
        {
            try
            {
                string ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                string port = _httpContextAccessor.HttpContext.Connection.RemotePort.ToString();
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex);
            }

            _iotRealTimeDataService.GetRealTimeDataItem().OnYCChanged += (sender, e) =>
            {
                if (e == null || e.Count <= 0)
                {
                    return;
                }

                var ycItemResponseList = new YcItemResponseList();

                ycItemResponseList.YcItemList.AddRange(e.Select(i => new YcItemResponse
                {
                    EquipState = i.equipState.Map<EquipState>(),
                    MYCNm = i.m_YCNm,
                    MIYCNo = i.m_iYCNo,
                    MAdviceMsg = i.m_AdviceMsg,
                    MBHasHistoryCcurve = i.m_bHasHistoryCcurve,
                    MBufang = i.m_Bufang,
                    MIEquipNo = i.m_iEquipNo,
                    MIsAlarm = i.m_IsAlarm,
                    MPlanNo = i.m_PlanNo,
                    MRelatedPic = i.m_related_pic,
                    MRelatedVideo = i.m_related_video,
                    MUnit = i.m_Unit,
                    MYCValue = new szYCValueResponse
                    {
                        S = string.IsNullOrWhiteSpace(i.m_YCValue.s) ? "" : i.m_YCValue.s,
                        F = i.m_YCValue.f,
                    },
                    MZiChanID = i.m_ZiChanID,
                    Timestamp = i.TimeStamp
                }));
                responseStream.WriteAsync(ycItemResponseList);
            };
            await AwaitCancellation(context.CancellationToken);
        }

        [AllowAnonymous]
        public override async Task YxChangeEvent(Empty request, IServerStreamWriter<StringResult> responseStream, ServerCallContext context)
        {
            _iotRealTimeDataService.GetRealTimeDataItem().OnYXChanged += (sender, e) => { responseStream.WriteAsync(new StringResult { Result = e.ToJson() }); };
            await AwaitCancellation(context.CancellationToken);
        }

        [AllowAnonymous]
        public override async Task EquipAddEvent(Empty request, IServerStreamWriter<StringResult> responseStream, ServerCallContext context)
        {
            _iotRealTimeDataService.GetRealTimeDataItem().OnEquipAdd += (sender, e) =>
            {
                responseStream.WriteAsync(new StringResult { Result = e.ToJson() });
            };
            await AwaitCancellation(context.CancellationToken);
        }

        [AllowAnonymous]
        public override async Task EquipChangeEvent(Empty request, IServerStreamWriter<StringResult> responseStream, ServerCallContext context)
        {
            _iotRealTimeDataService.GetRealTimeDataItem().OnEquipChanged += (sender, e) =>
            {
                responseStream.WriteAsync(new StringResult { Result = e.ToJson() });
            };
            await AwaitCancellation(context.CancellationToken);
        }

        [AllowAnonymous]
        public override async Task EquipDeleteEvent(Empty request, IServerStreamWriter<IntegerDefine> responseStream, ServerCallContext context)
        {
            _iotRealTimeDataService.GetRealTimeDataItem().OnEquipDeleted += (sender, e) =>
            {
                responseStream.WriteAsync(new IntegerDefine { Result = e });
            };
            await AwaitCancellation(context.CancellationToken);
        }

        [AllowAnonymous]
        public override async Task EquipStateEvent(Empty request, IServerStreamWriter<StringResult> responseStream, ServerCallContext context)
        {
            _iotRealTimeDataService.GetRealTimeDataItem().OnEquipStatusChanged += (sender, e) =>
            {
                responseStream.WriteAsync(new StringResult { Result = e.ToJson() });
            };
            await AwaitCancellation(context.CancellationToken);
        }
    }
}
