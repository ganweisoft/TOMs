using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GWDataCenter.MqService;

namespace GWDataCenter.Database
{
    public class GWDbProvider
    {
        public GWDbProvider() { }
        public static GWDbProvider Instance = new GWDbProvider();
        public void Init()
        {
            //TODO: 从MQTT服务器获取json数据并做处理
            MqttProvider.Instance.EquipInited += (sender, args) => this.InitCompleted = true;
            MqttProvider.Instance.Init();
            while (!this.InitCompleted)
            {
                Thread.Sleep(500);
            }
        }

        public bool InitCompleted { get; set; }

        public List<EquipTableRow> GetEquipTableList()
        {
            List<EquipTableRow> temp  = new List<EquipTableRow>();
            // 从MQTT服务器获取到的json数据进行转换

            foreach (var equip in MqttProvider.Instance.EquipTableRows.Values)
            {
                temp.Add(new EquipTableRow
                {
                    sta_n = equip.StaN,
                    equip_no = equip.EquipNo,
                    equip_nm = equip.EquipNm,
                    equip_detail = equip.EquipDetail,
                    acc_cyc = equip.AccCyc,
                    related_pic = equip.RelatedPic,
                    proc_advice = equip.ProcAdvice,
                    out_of_contact = equip.OutOfContact,
                    contacted = equip.Contacted,
                    event_wav = equip.EventWav,
                    communication_drv = equip.CommunicationDrv,
                    local_addr = equip.LocalAddr,
                    equip_addr = equip.EquipAddr,
                    communication_param = equip.CommunicationParam,
                    communication_time_param = equip.CommunicationTimeParam,
                    raw_equip_no = equip.RawEquipNo,
                    tabname = equip.Tabname,
                    alarm_scheme = equip.AlarmScheme,
                    attrib = equip.Attrib,
                    sta_IP = equip.StaIp,
                    AlarmRiseCycle = equip.AlarmRiseCycle,
                    Reserve1 = equip.Reserve1,
                    Reserve2 = equip.Reserve2,
                    Reserve3 = equip.Reserve3,
                    related_video = equip.RelatedVideo,
                    ZiChanID = equip.ZiChanId,
                    PlanNo = equip.PlanNo,
                    SafeTime = equip.SafeTime,
                    backup = equip.Backup
                });
            }
            
            return temp;
        }
        public List<YcpTableRow> GetYcpTableList()
        {
            List<YcpTableRow> temp = new List<YcpTableRow>();
            // 从MQTT服务器获取到的json数据进行转换
            foreach (var equip in MqttProvider.Instance.EquipTableRows.Values)
            {
                temp.AddRange(equip.Ycps.Select(ycp => new YcpTableRow
                {
                    sta_n = ycp.StaN,
                    equip_no = ycp.EquipNo,
                    yc_no = ycp.YcNo,
                    yc_nm = ycp.YcNm,
                    mapping = ycp.Mapping ?? false,
                    yc_min = ycp.YcMin,
                    yc_max = ycp.YcMax,
                    physic_min = ycp.PhysicMin,
                    physic_max = ycp.PhysicMax,
                    val_min = ycp.ValMin,
                    restore_min = ycp.RestoreMin,
                    restore_max = ycp.RestoreMax,
                    val_max = ycp.ValMax,
                    val_trait = ycp.ValTrait,
                    main_instruction = ycp.MainInstruction,
                    minor_instruction = ycp.MinorInstruction,
                    alarm_acceptable_time = ycp.AlarmAcceptableTime,
                    restore_acceptable_time = ycp.RestoreAcceptableTime,
                    alarm_repeat_time = ycp.AlarmRepeatTime,
                    proc_advice = ycp.ProcAdvice,
                    lvl_level = ycp.LvlLevel,
                    outmin_evt = ycp.OutminEvt,
                    outmax_evt = ycp.OutmaxEvt,
                    wave_file = ycp.WaveFile,
                    related_pic = ycp.RelatedPic,
                    alarm_scheme = ycp.AlarmScheme,
                    curve_rcd = ycp.CurveRcd,
                    curve_limit = ycp.CurveLimit,
                    alarm_shield = ycp.AlarmShield,
                    unit = ycp.Unit,
                    AlarmRiseCycle = ycp.AlarmRiseCycle,
                    Reserve1 = ycp.Reserve1,
                    Reserve2 = ycp.Reserve2,
                    Reserve3 = ycp.Reserve3,
                    related_video = ycp.RelatedVideo,
                    ZiChanID = ycp.ZiChanId,
                    PlanNo = ycp.PlanNo,
                    SafeTime = ycp.SafeTime,
                    GWValue = ycp.GWValue,
                    GWTime = ycp.GWTime,
                    datatype = ycp.DataType,
                    yc_code = ycp.YcCode
                }));
            }
            
            return temp;
        }
        public List<YxpTableRow> GetYxpTableList()
        {
            List<YxpTableRow> temp = new List<YxpTableRow>();
            // 从MQTT服务器获取到的json数据进行转换
            foreach (var equip in MqttProvider.Instance.EquipTableRows.Values)
            {
                temp.AddRange(equip.Yxps.Select(yxp => new YxpTableRow
                {
                    yx_no = yxp.YxNo,
                    yx_nm = yxp.YxNm,
                    proc_advice_r = yxp.ProcAdviceR,
                    proc_advice_d = yxp.ProcAdviceD,
                    level_r = yxp.LevelR,
                    level_d = yxp.LevelD,
                    evt_01 = yxp.Evt01,
                    evt_10 = yxp.Evt10,
                    sta_n = yxp.StaN,
                    equip_no = yxp.EquipNo,
                    val_trait = yxp.ValTrait,
                    main_instruction = yxp.MainInstruction,
                    minor_instruction = yxp.MinorInstruction,
                    alarm_acceptable_time = yxp.AlarmAcceptableTime,
                    restore_acceptable_time = yxp.RestoreAcceptableTime,
                    alarm_repeat_time = yxp.AlarmRepeatTime,
                    wave_file = yxp.WaveFile,
                    related_pic = yxp.RelatedPic,
                    alarm_scheme = yxp.AlarmScheme,
                    curve_rcd = yxp.CurveRcd,
                    alarm_shield = yxp.AlarmShield,
                    AlarmRiseCycle = yxp.AlarmRiseCycle,
                    Reserve1 = yxp.Reserve1,
                    Reserve2 = yxp.Reserve2,
                    Reserve3 = yxp.Reserve3,
                    related_video = yxp.RelatedVideo,
                    ZiChanID = yxp.ZiChanId,
                    PlanNo = yxp.PlanNo,
                    SafeTime = yxp.SafeTime,
                    GWValue = yxp.GWValue,
                    GWTime = yxp.GWTime,
                    datatype = yxp.DataType,
                    inversion = yxp.Inversion,
                    initval = yxp.Initval,
                    yx_code = yxp.YxCode,
                }));
            }
            return temp;
        }
        public List<SetParmTableRow> GetSetParmTableList()
        {
            List<SetParmTableRow> temp = new List<SetParmTableRow>();
            //TODO: 从MQTT服务器获取到的json数据进行转换
            foreach (var equip in MqttProvider.Instance.EquipTableRows.Values)
            {
                temp.AddRange(equip.SetParms.Select(set => new SetParmTableRow
                {
                    set_no = set.SetNo,
                    set_nm = set.SetNm,
                    set_type = set.SetType,
                    record = set.Record,
                    action = set.Action,
                    value = set.Value,
                    canexecution = set.Canexecution,
                    VoiceKeys = set.VoiceKeys,
                    EnableVoice = set.EnableVoice,
                    qr_equip_no = set.QrEquipNo,
                    set_code = set.SetCode,
                    sta_n = set.StaN,
                    equip_no = set.EquipNo,
                    main_instruction = set.MainInstruction,
                    minor_instruction = set.MinorInstruction,
                    Reserve1 = set.Reserve1,
                    Reserve2 = set.Reserve2,
                    Reserve3 = set.Reserve3,
                }));
            }
            return temp;
        }

    }

}
