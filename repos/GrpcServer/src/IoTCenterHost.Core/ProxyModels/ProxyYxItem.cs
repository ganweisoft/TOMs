//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.Core.Abstraction;
using IoTCenterHost.Core.Extension;
using OpenGWDataCenter.Model;
using ProtoBuf;
using System;

namespace IoTCenterHost.Core.ProxyModels
{
    [ProtoContract]
    public class ProxyYxItem : EventArgs
    {
        public ProxyYxItem()
        {

        }
        public ProxyYxItem(YXItem o)
        {
            m_iEquipNo = o.Equip_no;
            m_iYXNo = o.Yx_no;
            m_YXNm = o.Yx_nm;
            if (o.YXValue.GetType() == typeof(string))
            {
                m_YXValue.s = o.YXValue.ToString();
            }
            else
            {
                m_YXValue.temp = Convert.ToBoolean(o.YXValue) == true ? 1 : -1;
                m_YXValue.b = Convert.ToBoolean(o.YXValue);
            }
            try
            {
                if (!string.IsNullOrWhiteSpace(o.YXState))
                    m_YXState = o.YXState;
            }
            catch
            {

            }

            m_IsAlarm.temp = Convert.ToBoolean(o.IsAlarm) == true ? 1 : -1;
            m_IsAlarm.b = o.IsAlarm;
            m_AdviceMsg = o.Comments;
            m_Bufang = o.Bufang;
            m_bHasHistoryXcurve = o.Curve_rcd;
            m_related_pic = o.Related_pic;
            m_related_video = o.related_video;
            m_PlanNo = o.PlanNo;
            m_ZiChanID = o.ZiChanID;
            TimeStamp = DateTime.Now.GetUnixTimeStampMilliseconds();
        }
        object iEquipNo = 0;
        object iYXNo = 0;
        string YXNm = "";
        szYXValue YXValue = new szYXValue();
        string YXState = "";
        szYXAlarm IsAlarm = new szYXAlarm();//是否报警
        string AdviceMsg = "";//处理意见
        bool bHasHistoryXcurve;
        bool Bufang;// = true;
        string related_video;
        string ZiChanID;
        string PlanNo;
        string related_pic;

        public bool m_Bufang
        {
            get
            {
                return (bool)Bufang;
            }
            set
            {
                Bufang = value;
            }
        }

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
        public int m_iYXNo
        {
            get
            {
                return (int)iYXNo;
            }
            set
            {
                iYXNo = value;
            }
        }
        public string m_YXNm
        {
            get
            {
                return YXNm;
            }
            set
            {
                YXNm = value;
            }
        }
        public szYXValue m_YXValue
        {
            get
            {
                return YXValue;
            }
            set
            {
                YXValue = value;
            }
        }
        public string m_YXState
        {
            get
            {
                return YXState;
            }
            set
            {
                YXState = value;
            }
        }
        public szYXAlarm m_IsAlarm
        {
            get
            {
                return IsAlarm;
            }
            set
            {
                IsAlarm = value;
            }
        }
        public string m_AdviceMsg
        {
            get
            {
                return AdviceMsg;
            }
            set
            {
                AdviceMsg = value;
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
        public bool m_bHasHistoryXcurve
        {
            get
            {
                return bHasHistoryXcurve;
            }
            set
            {
                bHasHistoryXcurve = value;
            }
        }
        public GWDataCenter.EquipState equipState { get; set; }


        public long TimeStamp { get; private set; }

    }
}
