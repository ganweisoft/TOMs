//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.ComponentModel;

namespace IoTCenterHost.Core.Abstraction.BaseModels
{
    public class GrpcYxItem : INotifyPropertyChanged
    {
        public GrpcYxItem()
        {

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
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

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
                if (PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("m_YXNm"));
                }
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
                if (PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("m_YXValue"));
                }
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
                if (PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("m_YXState"));
                }
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
                if (PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("m_IsAlarm"));
                }
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
                if (PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("m_AdviceMsg"));
                }
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
                if (PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("m_bHasHistoryXcurve"));
                }
            }
        }
        public GrpcEquipState equipState { get; set; }
        public long TimeStamp { get; set; }

    }
}
