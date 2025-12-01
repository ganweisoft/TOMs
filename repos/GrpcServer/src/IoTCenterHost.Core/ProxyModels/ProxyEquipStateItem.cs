//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using IoTCenterHost.Core.Extension;
using OpenGWDataCenter.Model;
using System;

namespace IoTCenterHost.Core.ProxyModels
{
    public class ProxyEquipStateItem
    {
        public ProxyEquipStateItem() { }
        public ProxyEquipStateItem(EquipItem o)
        {
            m_iEquipNo = o.iEquipno;
            m_State = o.State;
            TimeStamp = DateTime.Now.GetUnixTimeStampMilliseconds();
        }
        object iEquipNo;
        EquipState State;


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

        public EquipState m_State
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
        public long TimeStamp { get; private set; }
    }
}
