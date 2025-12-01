// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace IoTCenterHost.Proxy.Models
{
    public class IotSetParmModel
    {
        public GrpcSetItem SetItemModel { get; set; }
        public GrpcSetItemResponse SetResponseEventArgResp { get; set; }

    }

    public class GrpcSetItem
    {
        public bool BRecord { get; set; }
        public bool BCanRepeat { get; set; }
        public bool BShowDlg { get; set; }
        public string Client_Instance_GUID { get; set; }
        public string Description { get; set; }
        public bool IsCj { get; set; }
        public bool IsWaitSetParm { get; set; }
        public bool IsQRTrigger { get; set; }
        public string RequestId { get; set; }
        public int M_SetNo { get; set; }
        public int CJ_EqpNo { get; set; }
        public int CJ_SetNo { get; set; }
        public string SysExecutor { get; set; }
        public bool BOnlyDelayType { get; set; }
        public int IDelayTime { get; set; }

        public string CsReserve1 { get; set; }
        public string UserIPandPort { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public string MinorInstruct { get; set; }
        public string MainInstruct { get; set; }
        public int StartTickCount { get; set; }
        public int WaitingTime { get; set; }
        public int EquipNo { get; set; }
        public string Executor { get; set; }
        public string CsReserve3 { get; set; }
        public bool? WaitSetParmIsFinish { get; set; }
        public string CsResponse { get; set; }
        public string CsReserve2 { get; set; }
        public bool BStopSetParm { get; set; }
    }

    public class GrpcSetItemResponse
    {
        public int M_iEquipNo { get; set; }
        public int M_iSetNo { get; set; }
        public string M_Value { get; set; }
        public string M_Response { get; set; }
        public bool M_bFinish { get; set; }
        public string M_GUID { get; set; }
    }
}
