// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using IoTCenterCore.AutoMapper;
using IoTCenterHost.Core.Abstraction;
using IoTCenterHost.Core.Abstraction.EnumDefine;
using IoTCenterHost.Proto;
using IoTCenterHost.Proxy.Core;
using System.Threading.Tasks;

namespace IoTCenterHost.Proxy.Connect
{
    public class ConnectServiceImpl : IConnectService
    {
        private readonly IoTCenterHost.Proto.IotService.IotServiceClient _alarmCenterClient;

        private readonly IConnectPoolManager _connectPoolManager;

        private readonly IConnectStatusProvider _connectStatusProvider;

        public ConnectServiceImpl(
            IConnectPoolManager connectPoolManager,
            IConnectStatusProvider connectStatusProvider,
            IotService.IotServiceClient alarmCenterClient)
        {
            _connectPoolManager = connectPoolManager;
            _alarmCenterClient = alarmCenterClient;
            _connectStatusProvider = connectStatusProvider;
        }
        public ConnectionStatus ConnectionStatus
        {
            get
            {
                return _connectStatusProvider.ConnectionStatus;
            }
            set
            {
                _connectStatusProvider.ConnectionStatus = value;
            }
        }
        public string Login(string userName, string password, GwClientType clientType)
        {
            BaseResult baseResult = this.Login(userName, password, clientType.MapTo<IoTCenterHost.Proto.ClientType>());
            return baseResult.ToJson();
        }

        public async Task<string> LoginAsync(string userName, string password, GwClientType clientType)
        {
            BaseResult baseResult = await this.LoginAsync(userName, password, clientType.MapTo<IoTCenterHost.Proto.ClientType>());
            return baseResult.ToJson();
        }

        private BaseResult Login(string user, string pwd, IoTCenterHost.Proto.ClientType client)
        {
            BaseResult baseResult = _alarmCenterClient.Login(new LoginModel() { User = user, Pwd = pwd, CT = client });
            return baseResult;
        }

        private async Task<BaseResult> LoginAsync(string user, string pwd, IoTCenterHost.Proto.ClientType client)
        {
            BaseResult baseResult = await _alarmCenterClient.LoginAsync(new LoginModel() { User = user, Pwd = pwd, CT = client });
            return baseResult;
        }

        public void CloseSession()
        {
            _alarmCenterClient.CloseSession(new Google.Protobuf.WellKnownTypes.Empty()).ToJson();
        }
    }
}
