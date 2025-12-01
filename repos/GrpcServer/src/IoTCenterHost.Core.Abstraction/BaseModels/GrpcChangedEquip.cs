//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.Core.Abstraction.EnumDefine;

namespace IoTCenterHost.Core.Abstraction.BaseModels
{
    public class GrpcChangedEquip
    {
        public int iStaNo;
        public int iEqpNo;
        public ChangedEquipState State;
    }
}
