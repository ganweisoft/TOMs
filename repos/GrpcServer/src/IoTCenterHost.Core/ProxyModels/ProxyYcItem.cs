//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.Core.Abstraction;
using IoTCenterHost.Core.Extension;
using System;

namespace IoTCenterHost.Core.ProxyModels
{
    public class ProxyYcItem : EventArgs
    {
        public ProxyYcItem()
        {

        }
        public ProxyYcItem(GWDataCenter.YCItem o)
        {
            m_iEquipNo = o.Equip_no;
            m_iYCNo = o.Yc_no;
            m_YCNm = o.Yc_nm;
            m_Unit = o.strUnit;
            if (o.YCValue != null && o.YCValue.GetType() == typeof(double))
            {
                m_YCValue.f = Convert.ToDouble(o.YCValue);
            }
            else if (o.YCValue != null)
            {
                m_YCValue.s = o.YCValue.ToString();
            }
            else if (o.YCValue.GetType() == typeof(double))
            {
                m_YCValue.s = "***";
            }
            m_IsAlarm = o.IsAlarm;
            m_AdviceMsg = o.Comments;
            m_bHasHistoryCcurve = o.Curve_rcd;
            m_Bufang = o.Bufang;

            m_related_pic = o.Related_pic;
            m_related_video = o.related_video;
            m_PlanNo = o.PlanNo;
            m_ZiChanID = o.ZiChanID;
            TimeStamp = DateTime.Now.GetUnixTimeStampMilliseconds();
        }



        object iEquipNo = 0;
        object iYCNo = 0;
        string YCNm = "";
        szYCValue YCValue = new szYCValue();
        object IsAlarm = false;//是否报警
        string AdviceMsg = "";//处理意见
        string strUnit = "";
        bool bHasHistoryCcurve;
        bool Bufang;// = true;
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

        public int m_iYCNo
        {
            get
            {
                return (int)iYCNo;
            }
            set
            {
                iYCNo = value;
            }
        }
        public string m_YCNm
        {
            get
            {
                return YCNm;
            }
            set
            {
                YCNm = value;
            }
        }

        public szYCValue m_YCValue
        {
            get
            {
                return YCValue;
            }
            set
            {
                YCValue = value;
            }
        }

        public string m_Unit
        {
            get
            {
                return strUnit;
            }
            set
            {
                strUnit = value;
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

        public bool m_IsAlarm
        {
            get
            {
                return (bool)IsAlarm;
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
        public bool m_bHasHistoryCcurve
        {
            get
            {
                return bHasHistoryCcurve;
            }
            set
            {
                bHasHistoryCcurve = value;
            }
        }

        public GWDataCenter.EquipState equipState { get; set; }

        public long TimeStamp { get; private set; }
    }
}
