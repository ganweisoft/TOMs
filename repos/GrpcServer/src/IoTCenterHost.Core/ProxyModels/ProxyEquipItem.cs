//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using IoTCenterHost.Core.Extension;
using OpenGWDataCenter.Model;
using System;

namespace IoTCenterHost.Core.ProxyModels
{

    public class ProxyEquipItem : EventArgs
    {
        public ProxyEquipItem() { }
        public ProxyEquipItem(EquipItem o)
        {
            m_iEquipNo = o.iEquipno;
            m_EquipNm = o.Equip_nm;
            m_State = o.State;
            m_Bufang = o.Bufang;
            m_related_video = o.related_video;
            m_ZiChanID = o.ZiChanID;
            m_PlanNo = o.PlanNo;
            m_related_pic = o.related_pic;
            TimeStamp = DateTime.Now.GetUnixTimeStampMilliseconds();
        }
        object iEquipNo;
        string EquipNm;
        EquipState State;
        bool? Bufang;// = true;
        string related_video;
        string ZiChanID;
        string PlanNo;
        string related_pic;


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

        public bool? m_Bufang
        {
            get
            {
                return Bufang;
            }
            set
            {
                Bufang = value;
            }
        }

        public string m_EquipNm
        {
            get
            {
                return EquipNm;
            }
            set
            {
                EquipNm = value;
            }
        }

        public string m_related_video
        {
            get
            {
                return related_video;
            }
            set
            {
                related_video = value;
            }
        }

        public string m_related_pic
        {
            get
            {
                return related_pic;
            }
            set
            {
                related_pic = value;
            }
        }

        public string m_ZiChanID
        {
            get
            {
                return ZiChanID;
            }
            set
            {
                ZiChanID = value;
            }
        }

        public string m_PlanNo
        {
            get
            {
                return PlanNo;
            }
            set
            {
                PlanNo = value;
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
