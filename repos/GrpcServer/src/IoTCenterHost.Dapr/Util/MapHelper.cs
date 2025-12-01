//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter.Database;
using IoTCenterHost.Dapr.Models;

namespace IoTCenterHost.Dapr.Util;

public static class MapHelper
{
    public static YcpWrapper ConvertToWrapper(YcpTableRow ycp)
    {
        return new YcpWrapper
        {
            YcNo = ycp.yc_no,
            StaN = ycp.sta_n,
            EquipNo = ycp.equip_no,
            YcNm = ycp.yc_nm,
            Mapping = ycp.mapping,
            YcMin = ycp.yc_min,
            YcMax = ycp.yc_max,
            PhysicMin = ycp.physic_min,
            PhysicMax = ycp.physic_max,
            ValMin = ycp.val_min,
            RestoreMin = ycp.restore_min,
            RestoreMax = ycp.restore_max,
            ValMax = ycp.val_max,
            ValTrait = ycp.val_trait,
            MainInstruction = ycp.main_instruction,
            MinorInstruction = ycp.minor_instruction,
            AlarmAcceptableTime = ycp.alarm_acceptable_time ?? 1,
            RestoreAcceptableTime = ycp.restore_acceptable_time ?? 1,
            AlarmRepeatTime = ycp.alarm_repeat_time,
            ProcAdvice = ycp.proc_advice,
            LvlLevel = ycp.lvl_level,
            OutminEvt = ycp.outmin_evt,
            OutmaxEvt = ycp.outmax_evt,
            WaveFile = ycp.wave_file,
            RelatedPic = ycp.related_pic,
            AlarmScheme = ycp.alarm_scheme,
            CurveRcd = ycp.curve_rcd,
            CurveLimit = ycp.curve_limit,
            AlarmShield = ycp.alarm_shield,
            Unit = ycp.unit,
            AlarmRiseCycle = ycp.AlarmRiseCycle,
            Reserve1 = ycp.Reserve1,
            Reserve2 = ycp.Reserve2,
            Reserve3 = ycp.Reserve3,
            RelatedVideo = ycp.related_video,
            ZiChanId = ycp.ZiChanID,
            PlanNo = ycp.PlanNo,
            SafeTime = ycp.SafeTime
        };
    }

    public static YxpWrapper ConvertToWrapper(YxpTableRow yxp)
    {
        return new YxpWrapper
        {
            YxNo = yxp.yx_no,
            StaN = yxp.sta_n,
            EquipNo = yxp.equip_no,
            YxNm = yxp.yx_nm,
            ProcAdviceR = yxp.proc_advice_r,
            ProcAdviceD = yxp.proc_advice_d,
            LevelR = yxp.level_r,
            LevelD = yxp.level_d,
            Evt01 = yxp.evt_01,
            Evt10 = yxp.evt_10,
            MainInstruction = yxp.main_instruction,
            MinorInstruction = yxp.minor_instruction,
            AlarmAcceptableTime = yxp.alarm_acceptable_time ?? 1,
            RestoreAcceptableTime = yxp.restore_acceptable_time ?? 1,
            AlarmRepeatTime = yxp.alarm_repeat_time,
            WaveFile = yxp.wave_file,
            RelatedPic = yxp.related_pic,
            AlarmScheme = yxp.alarm_scheme,
            Inversion = yxp.inversion,
            Initval = yxp.initval,
            ValTrait = yxp.val_trait,
            AlarmShield = yxp.alarm_shield,
            AlarmRiseCycle = yxp.AlarmRiseCycle,
            RelatedVideo = yxp.related_video,
            ZiChanId = yxp.ZiChanID,
            PlanNo = yxp.PlanNo,
            SafeTime = yxp.SafeTime,
            CurveRcd = yxp.curve_rcd,
            Reserve1 = yxp.Reserve1,
            Reserve2 = yxp.Reserve2,
            Reserve3 = yxp.Reserve3
        };
    }

    public static SetWrapper ConvertToWrapper(SetParmTableRow set)
    {
        return new SetWrapper
        {
            SetNo = set.set_no,
            StaN = set.sta_n,
            EquipNo = set.equip_no,
            SetNm = set.set_nm,
            SetType = set.set_type,
            MainInstruction = set.main_instruction,
            MinorInstruction = set.minor_instruction,
            Record = set.record,
            Action = set.action,
            Value = set.value,
            Canexecution = set.canexecution,
            VoiceKeys = set.VoiceKeys,
            EnableVoice = set.EnableVoice,
            QrEquipNo = set.qr_equip_no,
            Reserve1 = set.Reserve1,
            Reserve2 = set.Reserve2,
            Reserve3 = set.Reserve3
        };
    }
}