//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.Core.Abstraction.IotModels;
using System;

namespace IoTCenterHost.AppServices.Interfaces.Token
{
    public class LoginUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string Thumbnail { get; set; }
        public DateTime LoginTime { get; set; }
        public string LoginMark { get; set; }
        public GWUserItem UserItem { get; set; }
    }
}
