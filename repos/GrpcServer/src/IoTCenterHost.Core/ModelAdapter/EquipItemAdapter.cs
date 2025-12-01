//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using OpenGWDataCenter.Model;

namespace IoTCenterHost.Core.ModelAdapter
{
    public class EquipItemAdapter
    {
        public EquipItemAdapter(EquipItem equipItem)
        {
            DataFrash = equipItem.DataFrash;
            iAcc_cyc = equipItem.iAcc_cyc;
            Alarm_scheme = equipItem.Alarm_scheme;
            iEquipno = equipItem.iEquipno;
            iStano = equipItem.iStano;
            ICommunication = equipItem.ICommunication;
            Equip_addr = equipItem.Equip_addr;
            State = equipItem.State;
            Related_pic = equipItem.Related_pic;
            bCanConfirm2NormalState = equipItem.bCanConfirm2NormalState;
            IsDebug = equipItem.IsDebug;
            Reset = equipItem.Reset;
            iAcc_num = equipItem.iAcc_num;
            Local_addr = equipItem.Local_addr;
            Equip_nm = equipItem.Equip_nm;
            Bufang = equipItem.Bufang;
        }
        public bool DataFrash { get; set; }
        public int? iAcc_cyc { get; }
        public int? Alarm_scheme { get; }
        public int iEquipno { get; }
        public int iStano { get; }
        public IEquip ICommunication { get; }
        public string Equip_addr { get; }
        public EquipState State { get; set; }
        public string Related_pic { get; }
        public bool bCanConfirm2NormalState { get; set; }
        public bool IsDebug { get; set; }
        public object Reset { get; set; }
        public int? iAcc_num { get; set; }
        public string Local_addr { get; }
        public string Equip_nm { get; set; }
        public bool? Bufang { get; set; }

    }
}
