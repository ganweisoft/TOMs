//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace IoTCenterHost.Core.Abstraction
{
    public class ConnectionStatus
    {
        public ConnectionStatus()
        {
            AccessTokenInfo = new LoginResult();
        }
        public LoginResult AccessTokenInfo { get; set; }
        public string UserName { get; set; }
        public string Instance_GUID { get; set; }
        public int ID_InterScreen { get; set; }
    }
}
