//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel;

namespace IoTCenterHost.Core.Abstraction.BaseModels
{
    public class GrpcYcItem : INotifyPropertyChanged
    {
        public GrpcYcItem()
        {
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

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

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
                if (PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("m_YCNm"));
                }
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
                if (PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("m_YCValue"));
                }
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
                if (PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("m_Unit"));
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

        public bool m_IsAlarm
        {
            get
            {
                return (bool)IsAlarm;
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
        public bool m_bHasHistoryCcurve
        {
            get
            {
                return bHasHistoryCcurve;
            }
            set
            {
                bHasHistoryCcurve = value;
                if (PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("m_bHasHistoryCcurve"));
                }
            }
        }

        public GrpcEquipState equipState { get; set; }
        public long TimeStamp { get; set; }
    }
}
