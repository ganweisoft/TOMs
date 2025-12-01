import threading
import time
from typing import List

from ganweisoft.Database.EquipTable import EquipTableRow
from ganweisoft.Database.SetParmTable import SetParmTableRow
from ganweisoft.Database.YcpTable import YcpTableRow
from ganweisoft.Database.YxpTable import YxpTableRow
from ganweisoft.MqService import Equip, Ycp, Yxp, SetParm
from ganweisoft.MqService.MqttProvider import MqttProvider


class GWDbProvider:
    _instance_lock = threading.Lock()
    _instance = None

    def __init__(self):
        self.InitCompleted = False

    def __new__(cls):
        if cls._instance is None:
            with GWDbProvider._instance_lock:
                if GWDbProvider._instance is None:
                    cls._instance = super(GWDbProvider, cls).__new__(cls)
                    cls._instance.__init__()
        return cls._instance

    def init(self):
        # 从MQTT服务器获取json数据并做处理
        MqttProvider().init()
        # MqttProvider().equip_inited = lambda sender, args: setattr(self, 'InitCompleted', True)
        while not self.InitCompleted:
            time.sleep(0.5)

    def GetEquipTableList(self) -> List[EquipTableRow]:
        temp = []
        # 从MQTT服务器获取到的json数据进行转换
        for key, equip in MqttProvider().EquipTableRows.items():
            row = EquipTableRow()
            row.sta_n = equip.StaN
            row.equip_no = equip.EquipNo
            row.equip_nm = equip.EquipNm
            row.equip_detail = equip.EquipDetail
            row.acc_cyc = equip.AccCyc
            row.related_pic = equip.RelatedPic
            row.proc_advice = equip.ProcAdvice
            row.out_of_contact = equip.OutOfContact
            row.contacted = equip.Contacted
            row.event_wav = equip.EventWav
            row.communication_drv = equip.CommunicationDrv
            row.local_addr = equip.LocalAddr
            row.equip_addr = equip.EquipAddr
            row.communication_param = equip.CommunicationParam
            row.communication_time_param = equip.CommunicationTimeParam
            row.raw_equip_no = equip.RawEquipNo
            row.tabname = equip.Tabname
            row.alarm_scheme = equip.AlarmScheme
            row.attrib = equip.Attrib
            row.sta_IP = equip.StaIp
            row.AlarmRiseCycle = equip.AlarmRiseCycle
            row.Reserve1 = equip.Reserve1
            row.Reserve2 = equip.Reserve2
            row.Reserve3 = equip.Reserve3
            row.related_video = equip.RelatedVideo
            row.ZiChanID = equip.ZiChanId
            row.PlanNo = equip.PlanNo
            row.SafeTime = equip.SafeTime
            row.backup = equip.Backup
            temp.append(row)
        return temp

    def GetYcpTableList(self) -> List[YcpTableRow]:
        temp = []
        # 从MQTT服务器获取到的json数据进行转换
        for equip in MqttProvider().EquipTableRows.values():
            for _ycp in equip.Ycps:
                row = YcpTableRow()
                ycp = Ycp(_ycp)
                row.sta_n = ycp.StaN
                row.equip_no = ycp.EquipNo
                row.yc_no = ycp.YcNo
                row.yc_nm = ycp.YcNm
                row.mapping = ycp.Mapping if ycp.Mapping is not None else False
                row.yc_min = ycp.YcMin
                row.yc_max = ycp.YcMax
                row.physic_min = ycp.PhysicMin
                row.physic_max = ycp.PhysicMax
                row.val_min = ycp.ValMin
                row.restore_min = ycp.RestoreMin
                row.restore_max = ycp.RestoreMax
                row.val_max = ycp.ValMax
                row.val_trait = ycp.ValTrait
                row.main_instruction = ycp.MainInstruction
                row.minor_instruction = ycp.MinorInstruction
                row.alarm_acceptable_time = ycp.AlarmAcceptableTime
                row.restore_acceptable_time = ycp.RestoreAcceptableTime
                row.alarm_repeat_time = ycp.AlarmRepeatTime
                row.proc_advice = ycp.ProcAdvice
                row.lvl_level = ycp.LvlLevel
                row.outmin_evt = ycp.OutminEvt
                row.outmax_evt = ycp.OutmaxEvt
                row.wave_file = ycp.WaveFile
                row.related_pic = ycp.RelatedPic
                row.alarm_scheme = ycp.AlarmScheme
                row.curve_rcd = ycp.CurveRcd
                row.curve_limit = ycp.CurveLimit
                row.alarm_shield = ycp.AlarmShield
                row.unit = ycp.Unit
                row.AlarmRiseCycle = ycp.AlarmRiseCycle
                row.Reserve1 = ycp.Reserve1
                row.Reserve2 = ycp.Reserve2
                row.Reserve3 = ycp.Reserve3
                row.related_video = ycp.RelatedVideo
                row.ZiChanID = ycp.ZiChanId
                row.PlanNo = ycp.PlanNo
                row.SafeTime = ycp.SafeTime
                temp.append(row)
        return temp

    def GetYxpTableList(self) -> List[YxpTableRow]:
        temp = []
        # 从MQTT服务器获取到的json数据进行转换
        for equip in MqttProvider().EquipTableRows.values():
            for _yxp in equip.Yxps:
                row = YxpTableRow()
                yxp = Yxp(_yxp)
                row.yx_no = yxp.YxNo
                row.yx_nm = yxp.YxNm
                row.proc_advice_r = yxp.ProcAdviceR
                row.proc_advice_d = yxp.ProcAdviceD
                row.level_r = yxp.LevelR
                row.level_d = yxp.LevelD
                row.evt_01 = yxp.Evt01
                row.evt_10 = yxp.Evt10
                row.sta_n = yxp.StaN
                row.equip_no = yxp.EquipNo
                row.val_trait = yxp.ValTrait
                row.main_instruction = yxp.MainInstruction
                row.minor_instruction = yxp.MinorInstruction
                row.alarm_acceptable_time = yxp.AlarmAcceptableTime
                row.restore_acceptable_time = yxp.RestoreAcceptableTime
                row.alarm_repeat_time = yxp.AlarmRepeatTime
                row.wave_file = yxp.WaveFile
                row.related_pic = yxp.RelatedPic
                row.alarm_scheme = yxp.AlarmScheme
                row.curve_rcd = yxp.CurveRcd
                row.alarm_shield = yxp.AlarmShield
                row.AlarmRiseCycle = yxp.AlarmRiseCycle
                row.Reserve1 = yxp.Reserve1
                row.Reserve2 = yxp.Reserve2
                row.Reserve3 = yxp.Reserve3
                row.related_video = yxp.RelatedVideo
                row.ZiChanID = yxp.ZiChanId
                row.PlanNo = yxp.PlanNo
                row.SafeTime = yxp.SafeTime
                row.datatype = yxp.DataType
                row.inversion = yxp.Inversion
                row.initval = yxp.Initval
                temp.append(row)
        return temp

    def GetSetParmTableList(self) -> List[SetParmTableRow]:
        temp = []
        # 从MQTT服务器获取到的json数据进行转换
        for equip in MqttProvider().EquipTableRows.values():
            for _set_parm in equip.SetParms:
                row = SetParmTableRow()
                set_parm = SetParm(_set_parm)
                row.set_no = set_parm.SetNo
                row.set_nm = set_parm.SetNm
                row.set_type = set_parm.SetType
                row.record = set_parm.Record
                row.action = set_parm.Action
                row.value = set_parm.Value
                row.canexecution = set_parm.Canexecution
                row.VoiceKeys = set_parm.VoiceKeys
                row.EnableVoice = set_parm.EnableVoice
                row.qr_equip_no = set_parm.QrEquipNo
                row.sta_n = set_parm.StaN
                row.equip_no = set_parm.EquipNo
                row.main_instruction = set_parm.MainInstruction
                row.minor_instruction = set_parm.MinorInstruction
                row.Reserve1 = set_parm.Reserve1
                row.Reserve2 = set_parm.Reserve2
                row.Reserve3 = set_parm.Reserve3
                temp.append(row)
        return temp