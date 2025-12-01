// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Google.Protobuf.WellKnownTypes;
using IoTCenter.Utilities;
using IoTCenterCore.AutoMapper;
using IoTCenterHost.Core.Abstraction.AppServices;
using IoTCenterHost.Core.Abstraction.BaseModels;
using IoTCenterHost.Core.Abstraction.EnumDefine;
using IoTCenterHost.Proto;
using IoTCenterHost.Proxy.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTCenterHost.Proxy.ServiceImpl
{
    public class IoTCenterAppServiceImpl : IoTCenterAppService
    {
        private readonly IotService.IotServiceClient _alarmCenterClient;
        private readonly IotQueryService.IotQueryServiceClient _alarmCenterCallbackClient;
        private readonly IConnectStatusProvider _connectionStatus;
        private readonly Empty empty;
        public IoTCenterAppServiceImpl(IConnectStatusProvider connectionStatus,
            IotService.IotServiceClient alarmCenterClient,
            IotQueryService.IotQueryServiceClient alarmCenterCallbackClient)
        {
            _connectionStatus = connectionStatus;
            _alarmCenterClient = alarmCenterClient;
            empty = new Google.Protobuf.WellKnownTypes.Empty();
            _alarmCenterCallbackClient = alarmCenterCallbackClient;
        }


        public byte[] GetImageFromSvr1(string path, string ImageNm)
        {
            byte[] result = _alarmCenterClient.GetImageFromSvr1(new GetImageFromSvr1Request { Path = path, ImageNm = ImageNm }).Result.ConvertByteStrToByte();
            return result;
        }

      
        public string GetPropertyFromPropertyService(string PropertyName, string NodeName, string DefaultValue)
        {
            return _alarmCenterClient.GetPropertyFromPropertyService(new GetPropertyFromPropertyServiceRequest { DefaultValue = DefaultValue, PropertyName = PropertyName, NodeName = NodeName }).Result;
        }

    
        public string GetVersionInfo()
        {
            return _alarmCenterClient.GetVersionInfo(empty).Result;
        }

    
        public void MResetYcYxNo(int EquipNo, string sType, int YcYxNo)
        {
            _alarmCenterClient.MResetYcYxNo(new MResetYcYxNoRequest { EquipNo = EquipNo, SType = sType, YcYxNo = YcYxNo });
        }

    
        public void ResetDelayActionPlan()
        {
            _alarmCenterClient.ResetDelayActionPlan(empty);
        }

        public void ResetGWDataRecordItems()
        {
            _alarmCenterClient.ResetGWDataRecordItems(empty);
        }

        public void ResetProcTimeManage()
        {
            _alarmCenterClient.ResetProcTimeManage(empty);
        }

        public void SetGUID(string strGUID)
        {
            _connectionStatus.ConnectionStatus.Instance_GUID = strGUID;
        }

        public void SetInterScreenID(int id)
        {
            _connectionStatus.ConnectionStatus.ID_InterScreen = id;
        }

        public void SetPropertyToPropertyService(string PropertyName, string NodeName, string Value)
        {
            _alarmCenterClient.SetPropertyToPropertyService(new GetPropertyFromPropertyServiceRequest { DefaultValue = Value, PropertyName = PropertyName, NodeName = NodeName });
        }

        public void SetUserNm(string strNm)
        {
            _connectionStatus.ConnectionStatus.UserName = strNm;
        }
    }
}
