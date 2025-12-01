//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;

namespace IoTCenterHost.Core.Abstraction
{


    public class WcfRealTimeEventItem : EventArgs
    {
        int m_level;
        string m_EventMsg;
        string m_Proc_advice_Msg;
        int m_equipno;
        string m_type;
        string m_related_pic;
        int m_ycyxno;
        DateTime DT;
        bool m_bConfirmed;
        string m_User_Confirmed;
        DateTime _DT_Confirmed;

        string related_video;
        string m_ZiChanID;
        string m_PlanNo;

        string wavefile;

        public bool bConfirmed
        {
            get
            {
                return m_bConfirmed;
            }
            set
            {
                m_bConfirmed = value;
            }
        }

        public string User_Confirmed
        {
            get
            {
                return m_User_Confirmed;
            }
            set
            {
                m_User_Confirmed = value;
            }
        }

        public DateTime DT_Confirmed
        {
            get
            {
                return _DT_Confirmed;
            }
            set
            {
                _DT_Confirmed = value;
            }
        }

        public int Level
        {
            get
            {
                return m_level;
            }
            set
            {
                m_level = value;
            }
        }
        public string EventMsg
        {
            get
            {
                return m_EventMsg;
            }
            set
            {
                m_EventMsg = value;
            }
        }
        public string Proc_advice_Msg
        {
            get
            {
                return m_Proc_advice_Msg;
            }
            set
            {
                m_Proc_advice_Msg = value;
            }
        }
        public string Wavefile
        {
            get
            {
                return wavefile;
            }
            set
            {
                wavefile = value;
            }
        }
        public string Related_pic
        {
            get
            {
                return m_related_pic;
            }
            set
            {
                m_related_pic = value;
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

        public string ZiChanID
        {
            get
            {
                return m_ZiChanID;
            }
            set
            {
                m_ZiChanID = value;
            }
        }

        public string PlanNo
        {
            get
            {
                return m_PlanNo;
            }
            set
            {
                m_PlanNo = value;
            }
        }

        public int Equipno
        {
            get
            {
                return m_equipno;
            }
            set
            {
                m_equipno = value;
            }
        }

        public string Type
        {
            get
            {
                return m_type;
            }
            set
            {
                m_type = value;
            }
        }

        public int Ycyxno
        {
            get
            {
                return m_ycyxno;
            }
            set
            {
                m_ycyxno = value;
            }
        }

        public DateTime Time
        {
            get
            {
                return DT;
            }
            set
            {
                DT = value;
            }
        }
        public string GUID
        {
            get; set;
        }

        public long TimeID
        {
            get; set;
        }
    }
}
