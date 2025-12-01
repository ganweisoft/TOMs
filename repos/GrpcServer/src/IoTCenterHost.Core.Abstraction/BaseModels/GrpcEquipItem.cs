//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.ComponentModel;

namespace IoTCenterHost.Core.Abstraction.BaseModels
{

    public class GrpcEquipItem : INotifyPropertyChanged
    {
        public GrpcEquipItem() { }
        object iEquipNo;
        string EquipNm;
        GrpcEquipState State;
        bool? Bufang;// = true;
        string related_video;
        string ZiChanID;
        string PlanNo;
        string related_pic;

        [field: NonSerialized]
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
                if (PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("m_EquipNm"));
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

        public GrpcEquipState m_State
        {
            get
            {
                return State;
            }
            set
            {
                State = value;
                if (PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("m_State"));
                }
            }
        }
        public long TimeStamp { get; set; }
    }
}
