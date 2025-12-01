//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace IoTCenterHost.Core.Abstraction
{
    public class GrpcHostMsg
    {

        public string SubscribeTopic { get; set; }
        public string Data { get; set; }

        public MsgEventType MsgEventType { get; set; }
    }
    public enum ZMQResponseType
    {
        Null = 0,
        Equip = 1,
        YX = 2,
        YC = 3,
        Event = 4,
        Command = 5,
    }
}
