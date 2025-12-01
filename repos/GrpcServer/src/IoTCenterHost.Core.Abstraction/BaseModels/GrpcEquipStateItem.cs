//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.Core.Abstraction.BaseModels;

namespace IoTCenterHost.Core.Abstraction.ProxyModels
{
    public class GrpcEquipStateItem
    {

        object iEquipNo;
        GrpcEquipState State;


        public int m_iEquipNo
        {
            get
            {
                return (int)iEquipNo;
            }
            set
            {
                iEquipNo = value;
            }
        }

        public GrpcEquipState m_State
        {
            get
            {
                return State;
            }
            set
            {
                State = value;
            }
        }

        public long TimeStamp { get; set; }
    }
}
