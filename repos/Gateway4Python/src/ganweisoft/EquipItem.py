import threading
import time
from datetime import datetime, timedelta
from enum import Enum
from typing import Any, Callable, Dict, List, Optional

from ganweisoft.Database.EquipTable import EquipTableRow
from ganweisoft.Logging import Logging
from ganweisoft import StationItem, DataCenter
from ganweisoft.SetItem import SetItem
from ganweisoft.interface import IEquip
from ganweisoft.EquipState import EquipState


class SafeTimeSpan:
    def __init__(self, t_start: timedelta, t_end: timedelta):
        self.t_start = t_start
        self.t_end = t_end


class NoSetItemPermissionEventArgs:
    def __init__(self):
        self.str_guid = None


class EquipItem:
    def __init__(self, sta: int, eqp: int, serial_port, equip_table_row):
        self.istano = sta
        self.iequipno = eqp
        self.iacc_cyc = 0
        self.iacc_num = 0
        self.alarm_scheme = 0
        self.equip_nm = ""
        self.local_addr = ""
        self.equip_addr = ""
        self.communication_drv = ""
        self.communication_param = ""
        self.communication_time_param = ""
        self.alarm_msg = ""
        self.restore_alarm_msg = ""
        self.advice_msg = ""
        self.wave_file = ""
        self.restore_wave_file = ""
        self.related_pic = ""
        self.equip_detail = ""
        self.attrib = 0
        self.alarm_rise_cycle = None
        self.reserve1 = ""
        self.reserve2 = ""
        self.reserve3 = ""
        self.related_video = ""
        self.zichan_id = ""
        self.plan_no = ""
        self.safe_time_span_list = []
        self.dll = None
        self.icommunication: IEquip = None
        self.equip_base = None
        self.cur_set_item = None
        self.data_fresh = False
        self.set_item_queue = []
        self.offline_set_item_queue = []
        self.i_comm_fault_retry_count = 0
        self.b_init_ok = False
        self.b_communication_ok = False
        self.serial_port = serial_port
        self.dr = equip_table_row
        self.b_can_monitor = False
        self.do_set_parm = False
        self.equip_rw_state = False
        self.equip_reset_lock = False
        self.b_set_cache_data = False
        self.reset = False
        self.debug = False
        self.can_confirm = False
        self.is_backup = False
        self.state = EquipState.Initial
        self.event_guid = None
        self.enable = True

        self.ResetWhenDBChanged(sta, eqp, equip_table_row)

    @property
    def DataFrash(self):
        return self.data_fresh

    @DataFrash.setter
    def DataFrash(self, value):
        self.data_fresh = value

    @property
    def State(self):
        return self.state

    @State.setter
    def State(self, value):
        if value != self.state:
            self.state = value

    @property
    def Enable(self):
        return self.enable

    @Enable.setter
    def Enable(self, value):
        self.enable = value

    @property
    def IsDebug(self):
        return self.debug

    @IsDebug.setter
    def IsDebug(self, value):
        self.debug = value

    @property
    def IsBackupState(self):
        return self.is_backup

    @IsBackupState.setter
    def IsBackupState(self, value):
        self.is_backup = value

    def ResetWhenDBChanged(self, sta, eqp, dr: EquipTableRow | None):
        if dr is None:
            dr = next((item for item in StationItem.StationItem.db_eqp if item.equip_no == eqp), None)

        self.equip_nm = dr.equip_nm
        self.equip_detail = dr.equip_detail
        self.iacc_cyc = dr.acc_cyc
        self.alarm_scheme = dr.alarm_scheme
        self.local_addr = dr.local_addr
        self.equip_addr = dr.equip_addr
        self.communication_drv = dr.communication_drv
        self.alarm_msg = f"{dr.equip_nm}:{dr.out_of_contact}"
        self.advice_msg = dr.proc_advice
        self.attrib = dr.attrib
        self.b_set_cache_data = True if self.attrib == 1 else False
        self.restore_alarm_msg = f"{dr.equip_nm}:{dr.contacted}"
        self.alarm_rise_cycle = dr.AlarmRiseCycle
        self.reserve1 = dr.Reserve1
        self.reserve2 = dr.Reserve2
        self.reserve3 = dr.Reserve3
        self.related_video = dr.related_video
        self.zichan_id = dr.ZiChanID
        self.plan_no = dr.PlanNo
        self.GetSafeTimeSpanList(dr.SafeTime)
        self.communication_param = dr.communication_param
        self.communication_time_param = dr.communication_time_param
        self.GetInterfaceOfEquip()

    def GetSafeTimeSpanList(self, s):
        self.safe_time_span_list.clear()
        if not s.strip():
            return
        segments = s.split('+')
        for segment in segments:
            parts = segment.split('-')
            if len(parts) == 2:
                start_parts = parts[0].split(':')
                end_parts = parts[1].split(':')
                start_time = timedelta(
                    hours=int(start_parts[0]),
                    minutes=int(start_parts[1]),
                    seconds=int(start_parts[2])
                )
                end_time = timedelta(
                    hours=int(end_parts[0]),
                    minutes=int(end_parts[1]),
                    seconds=int(end_parts[2])
                )
                self.safe_time_span_list.append(SafeTimeSpan(start_time, end_time))

    def GetInterfaceOfEquip(self):
        self.icommunication = None
        try:
            self.dll = __import__("__main__")
            for name in dir(self.dll):
                obj = getattr(self.dll, name)
                if hasattr(obj, "__name__") and obj.__name__ == "CEquip":
                    self.icommunication = obj()
                    self.icommunication.equipitem = self
                    self.equip_base = obj()
                    break
            if self.icommunication is None:
                raise Exception("icommunication as IEquip is null")
        except Exception as e:
            Logging.write_log_file(str(e))
            self.b_communication_ok = False
            self.State = EquipState.NoCommunication

    def AddSetItem(self, item: SetItem):
        if item is None:
            return
        if self.State != EquipState.NoCommunication and self.State != EquipState.Initial:
            self.set_item_queue.append(item)

    def __str__(self):
        return f"EquipItem({self.iequipno})"

    def IsRageMode(self):
        strResult = DataCenter.DataCenter.get_property_from_property_service("RunRageMode", "", "")
        if strResult.strip().upper() == "ALL":
            return True
        if not strResult.strip():
            return False
        ss = strResult.split('%')
        for s in ss:
            if s.upper().strip() == self.local_addr.upper().strip():
                return True
        return False


class DelayEventFire:
    def __init__(self, event_handler: Callable, msec: int, sender, args):
        self.event_handler = event_handler
        self.msec = msec
        self.sender = sender
        self.args = args
        self.timer = None
        self.lock = threading.Lock()

    def AddEvent(self):
        with self.lock:
            if self.timer is None:
                self.timer = threading.Timer(self.msec / 1000, self.theout)
                self.timer.start()

    def theout(self):
        with self.lock:
            if self.event_handler:
                self.event_handler(self.sender, self.args)
