//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel;

namespace IoTCenterHost.Core.Abstraction
{
    public enum MsgEventType
    {
        [Description("{0}")]
        EquipAddEvent = 1,
        [Description("{0}")]
        EquipChangeEvent = 2,
        [Description("{0}")]
        EquipDeleteEvent = 3,
        [Description("{0}/{1}")]
        EquipStateEvent = 4,
        [Description("{0}/{1}")]
        YcAddEvent = 5,
        [Description("{0}/{1}")]
        YcChangeEvent = 6,
        [Description("{0}/{1}")]
        YcDeleteEvent = 7,
        [Description("{0}/{1}")]
        YxAddEvent = 8,
        [Description("{0}/{1}")]
        YxChangeEvent = 9,
        [Description("{0}/{1}")]
        YxDeleteEvent = 10,
        [Description("{0}/{1}")]
        SetAddEvent = 11,
        [Description("{0}/{1}")]
        SetChangeEvent = 12,
        [Description("{0}/{1}")]
        SetDeleteEvent = 13,
        [Description("{0}/{1}")]
        SendControl = 14,
        [Description("{0}")]
        SendVoice = 15,
        [Description("{0}")]
        AddRealTimeSnapshot = 16,
        [Description("{0}")]
        DeleteRealTimeSnapshot = 17,
        OpenPage4InterScreen = 18,
        ShowOrClosePage = 19,
        [Description("{0}/{1}")]
        KickClient = 21,
        ShowMsg = 22,
        NotifyOffLine = 23,
        NotifyRoleOffLine = 24,
        ShowLockSetParmMsg = 25,
        [Description("{0}/")]
        VOpenPage = 26,
        ShowInfo = 27,
        SetparmCallback = 28,

        FireSetParm = 29,
    }
}
