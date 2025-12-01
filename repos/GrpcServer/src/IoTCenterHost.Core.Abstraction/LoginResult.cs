//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;

namespace IoTCenterHost.Core.Abstraction
{
    public class LoginResult
    {
        public LoginResult()
        {

        }
        public string UserItem { get; set; }
        public string Token { get; set; }
        public string LoginMark { get; set; }
        public DateTime LoginTime { get; set; }

        public string MqIdentity { get; set; }

    }
}
