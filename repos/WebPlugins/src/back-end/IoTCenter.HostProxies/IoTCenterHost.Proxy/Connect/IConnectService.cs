// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.Core.Abstraction;
using IoTCenterHost.Core.Abstraction.EnumDefine;
using System.Threading.Tasks;

namespace IoTCenterHost.Proxy
{
    public interface IConnectService
    {
        string Login(string userName, string password, GwClientType clientType);

        Task<string> LoginAsync(string userName, string password, GwClientType clientType);
       
        ConnectionStatus ConnectionStatus { get; set; }

        void CloseSession();
    }
}
