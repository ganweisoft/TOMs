import os
import threading
import time
from datetime import datetime
from enum import Enum
from typing import List

from ganweisoft import DataCenter
from ganweisoft.Database.EquipTable import EquipTableRow
from ganweisoft.EquipItem import EquipItem, DelayEventFire
from ganweisoft.EquipState import EquipState
from ganweisoft.Logging import Logging
from ganweisoft.MqService.Model.MqEvtMessage import MqEvtMessage, EquipEvent, EquipEventItem
from ganweisoft.MqService.Model.MqRtValueMessage import MqRtValueMessage, DataItem
from ganweisoft.MqService import MqttProvider
from ganweisoft.SetItem import SetItem
from ganweisoft import StationItem
from ganweisoft.communication.Communication import SerialPort
from ganweisoft.interface import CEquipBase
from ganweisoft.interface.IEquip import CommunicationState


class SafetyLevel(Enum):
    Unsafety = 1
    Safety = 2


class ChangedEquipState(Enum):
    Add = 1
    Delete = 2
    Edit = 3


class ChangedEquip:
    def __init__(self, state: ChangedEquipState, i_sta_no: int, i_eqp_no: int):
        self.state = state
        self.i_sta_no = i_sta_no
        self.i_eqp_no = i_eqp_no


class SubEquipList:
    def __init__(self, local_addr: str, data_row_list: List[EquipTableRow]):
        self.refresh_thread: threading.Thread = threading.Thread()
        self.reset_equips: bool = False
        self.set_parm_thread: threading.Thread = threading.Thread()
        self.exist_set_parm = True
        self.b_start_thread = True
        self.b_start_refresh_thread = False
        self.b_start_set_parm_thread = False
        self.thread_interval = 10
        self.delay_del_eqp_second = 2
        self.local_addr = local_addr
        self.equip_list: List[EquipItem] = []
        self.data_refresh_break = False
        self.set_parm_equip_list: List[int] = []
        self.b_can_execute = True
        self.b_edit_local_addr = False
        self.changed_equip_list: List[ChangedEquip] = []
        self.changed_equip_list1: List[ChangedEquip] = []

        self.bChangingState = threading.Lock()
        self.EquipAdd = None
        self.EquipDel = None
        self.EquipEdit = None

        self.spt = SerialPort()
        r: EquipTableRow
        for r in data_row_list:
            stano = r.sta_n
            eqpno = r.equip_no
            dr = next((item for item in StationItem.StationItem.db_eqp if item.equip_no == eqpno), None)
            eqpitm = EquipItem(stano, eqpno, self.spt, dr)
            self.equip_list.append(eqpitm)
            if eqpno in DataCenter.DataCenter.get_equip_item_dict().keys():
                del DataCenter.DataCenter.get_equip_item_dict()[eqpno]
            DataCenter.DataCenter.get_equip_item_dict()[eqpno] = eqpitm

        self.ChangedEquipListChangedDelayEvent = None

        if self.IsRageMode():
            self.ThreadInterval = 0
            for item in self.equip_list:
                item.IsRageMode = True

    def refresh(self):
        while self.b_start_thread:
            while self.exist_set_parm and self.data_refresh_break and self.b_start_thread:
                time.sleep(self.thread_interval)

            while not self.data_refresh_break and self.b_start_thread:
                if self.reset_equips:
                    try:
                        with threading.Lock():
                            for equip in self.equip_list:
                                equip.ResetWhenDBChanged(equip.istano, equip.iequipno)
                                if equip.icommunication is not None:
                                    equip.icommunication.ResetFlag = True
                    except Exception as e:
                        Logging.write_log_file(str(e))
                    self.reset_equips = False

                try:
                    self.edit_equip_list()
                    self.equip_refresh()
                except Exception as e:
                    Logging.write_log_file(str(e))
                time.sleep(self.thread_interval)

    def equip_refresh(self):
        state_items = []
        for equip in self.equip_list:
            if self.changed_equip_list and len(self.changed_equip_list) > 0:
                break

            if equip.icommunication is None:
                continue

            if equip.IsBackupState:
                if equip.State != EquipState.BackUp:
                    equip.State = EquipState.BackUp
                continue

            if not equip.Enable:
                equip.State = EquipState.NoCommunication
                continue

            equip.iacc_num += 1
            if equip.iacc_num % equip.iacc_cyc != 0:
                continue
            equip.iacc_num = 0
            equip.reset = False

            if not equip.b_init_ok or equip.icommunication.ResetFlag:
                if not equip.icommunication.init(equip):
                    equip.i_comm_fault_retry_count += 1
                    equip.icommunication.ResetFlag = True
                    if equip.icommunication.m_retrytime <= equip.i_comm_fault_retry_count:
                        equip.b_communication_ok = False
                        equip.State = EquipState.NoCommunication
                        equip.i_comm_fault_retry_count = 0
                        equip.b_init_ok = False
                    continue
                else:
                    equip.b_init_ok = True
                    equip.icommunication.ResetFlag = False
                    if equip.State != EquipState.NoCommunication:
                        equip.State = EquipState.Initial

            communication_state = equip.icommunication.GetData(equip.icommunication)
            if communication_state == CommunicationState.ok:
                equip.b_communication_ok = True
                equip.i_comm_fault_retry_count = 0

                if equip.icommunication.YCResults:
                    MqttProvider().publish_yc_rt_value_async(MqRtValueMessage(
                        data_type=1,
                        time=datetime.now().strftime("%Y-%m-%d %H:%M:%S"),
                        flow=os.urandom(16).hex(),
                        data_items=[DataItem(device_id=equip.icommunication.m_equip_no,
                                             attribute=equip.icommunication.YCResults)]
                    ))

                if equip.icommunication.YXResults:
                    MqttProvider().publish_yx_rt_value_async(MqRtValueMessage(
                        data_type=2,
                        time=datetime.now().strftime("%Y-%m-%d %H:%M:%S"),
                        flow=os.urandom(16).hex(),
                        data_items=[DataItem(device_id=equip.icommunication.m_equip_no,
                                             attribute=equip.icommunication.YXResults)]
                    ))

                if equip.icommunication.EquipEventList:
                    MqttProvider().publish_evt_value_async(MqEvtMessage(
                        time=datetime.now().strftime("%Y-%m-%d %H:%M:%S"),
                        flow=os.urandom(16).hex(),
                        event_items=[EquipEvent(
                            device_id=equip.icommunication.m_equip_no,
                            equip_events=[EquipEventItem(
                                msg=event.msg,
                                msg4_linkage=event.msg4Linkage,
                                level=event.level,
                                occur_date_time=event.dt,
                                equip_no=event.iEquipNo
                            ) for event in equip.icommunication.EquipEventList]
                        )]
                    ))
                    equip.icommunication.EquipEventList.clear()

                equip.data_fresh = True

            if communication_state == CommunicationState.setreturn:
                if equip.do_set_parm and equip.State == EquipState.CommunicationOK:
                    equip.State = EquipState.HaveSetParm
                self.data_refresh_break = True
                return

            if communication_state == CommunicationState.fail:
                equip.i_comm_fault_retry_count += 1
                equip.icommunication.reset_flag = True
                if equip.icommunication.m_retrytime <= equip.i_comm_fault_retry_count:
                    equip.b_communication_ok = False
                    equip.State = EquipState.NoCommunication
                    equip.i_comm_fault_retry_count = 0

    def start_refresh_thread(self):
        if not self.b_start_refresh_thread:
            self.refresh_thread = threading.Thread(target=self.refresh)
            self.refresh_thread.start()
            self.b_start_refresh_thread = True

    def set_parm_scan(self):
        while self.b_start_thread:
            try:
                with threading.Lock():
                    for equip in self.equip_list:
                        if equip.set_item_queue and not equip.set_item_queue.empty():
                            self.exist_set_parm = True
                            self.send_set_parm_flag(True)
                            equip.do_set_parm = True
                            set_item = equip.set_item_queue.get()
                            if set_item.b_stop_set_parm:
                                continue

                            if equip.reserve3.upper().strip() != "FASTSET" and set_item.type.upper() != "S":
                                while not self.data_refresh_break:
                                    time.sleep(self.thread_interval)

                            try:
                                self.set_parm(equip, set_item)
                                if equip.iequipno not in self.set_parm_equip_list:
                                    self.set_parm_equip_list.append(equip.iequipno)
                            except Exception as e:
                                Logging.write_log_file(str(e))

                            equip.do_set_parm = False
            except Exception as e:
                Logging.write_log_file(str(e))
            time.sleep(self.thread_interval)

        self.exist_set_parm = False
        self.data_refresh_break = False
        self.send_set_parm_flag(False)

    def send_set_parm_flag(self, flag: bool):
        with threading.Lock():
            for equip in self.equip_list:
                if equip.icommunication is not None:
                    equip.icommunication.RunSetParmFlag = flag

    def start_set_parm_thread(self):
        if not self.b_start_set_parm_thread:
            self.set_parm_thread = threading.Thread(target=self.set_parm_scan)
            self.set_parm_thread.start()
            self.b_start_set_parm_thread = True

    def set_parm(self, equip: EquipItem, set_item: SetItem):
        threading.Thread(target=self._set_parm, args=(equip, set_item)).start()

    def _set_parm(self, equip: EquipItem, set_item: SetItem):
        with threading.Lock():
            if equip.icommunication is None:
                return
            if equip.equip_base is None:
                return

            equip.state = EquipState.HaveSetParm
            equip.icommunication.init(equip)

            if set_item.b_stop_set_parm:
                return

            equip.icommunication.set_parm_executor = set_item.executor
            cs_executor = set_item.executor if set_item.executor else ""

            if equip.icommunication.SetParm(set_item.main_instruct, set_item.minor_instruct, set_item.value):
                set_item.wait_set_parm_is_finish = True
            else:
                set_item.wait_set_parm_is_finish = False

            if not set_item.is_synchronization:
                StationItem.StationItem.FireSetParmResultEvent(set_item)

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

    def AddChangedEquipList(self, e: ChangedEquip):
        self.bChangingState.acquire()
        self.changed_equip_list.append(ChangedEquip(e.state, e.i_sta_no, e.i_eqp_no))
        StationItem.StationItem.DoHaveEquipChanged()
        time.sleep(1)
        self.bChangingState.release()

    def AddChangedEquipList1(self, e: ChangedEquip):
        self.bChangingState.acquire()
        self.changed_equip_list1.append(ChangedEquip(e.state, e.i_sta_no, e.i_eqp_no))
        self.bChangingState.release()
        if self.ChangedEquipListChangedDelayEvent is None:
            self.ChangedEquipListChangedDelayEvent = DelayEventFire(self.SubEquipList_ChangedEquipListChanged, 500, self.changed_equip_list1, None)
        self.ChangedEquipListChangedDelayEvent.AddEvent()

    def SubEquipList_ChangedEquipListChanged(self, sender, e):
        self.bChangingState.acquire()
        if len(self.changed_equip_list1) == 0:
            self.bChangingState.release()
            return
        for item in self.changed_equip_list1:
            self.changed_equip_list.append(ChangedEquip(item.state, item.i_sta_no, item.i_eqp_no))
        self.changed_equip_list.clear()
        StationItem.StationItem.DoHaveEquipChanged()
        time.sleep(1)
        self.bChangingState.release()

    def edit_equip_list(self):
        self.bChangingState.acquire()
        if len(self.changed_equip_list) == 0:
            self.bChangingState.release()
            return
        for k in range(len(self.changed_equip_list)):
            if self.changed_equip_list[k].state == ChangedEquipState.Add:
                spt = SerialPort()
                stano = self.changed_equip_list[k].i_sta_no
                eqpno = self.changed_equip_list[k].i_eqp_no
                dr = next((item for item in StationItem.StationItem.db_eqp if item.equip_no == eqpno), None)
                eqpitm = EquipItem(stano, eqpno, spt, dr)
                eqpitm.icommunication.ResetFlag = True
                if eqpno not in DataCenter.DataCenter.get_equip_item_dict():
                    DataCenter.DataCenter.get_equip_item_dict()[eqpno] = eqpitm
                if self.EquipAdd is not None:
                    self.EquipAdd(eqpno, None)
            elif self.changed_equip_list[k].state == ChangedEquipState.Delete:
                for e in self.equip_list:
                    if e.iequipno == self.changed_equip_list[k].i_eqp_no:
                        if not self.b_edit_local_addr:
                            e.State = EquipState.NoCommunication
                            if self.EquipDel is not None:
                                self.EquipDel(e.iequipno, None)
                        self.equip_list.remove(e)
                        if not self.b_edit_local_addr:
                            if self.changed_equip_list[k].i_eqp_no in DataCenter.DataCenter.get_equip_item_dict().keys():
                                del DataCenter.DataCenter.get_equip_item_dict()[self.changed_equip_list[k].i_eqp_no]
                        if len(self.equip_list) == 0:
                            if self.local_addr in StationItem.StationItem.EquipCategoryDict:
                                del StationItem.StationItem.EquipCategoryDict[self.local_addr]
                            self.b_start_thread = False
                        e.icommunication.CloseCommunication()
                        if self.b_edit_local_addr:
                            self.b_edit_local_addr = False
                        break
            elif self.changed_equip_list[k].state == ChangedEquipState.Edit:
                for e in self.equip_list:
                    if e.iequipno == self.changed_equip_list[k].i_eqp_no:
                        e.ResetWhenDBChanged(e.istano, e.iequipno)
                        if e.icommunication is not None:
                            e.icommunication.ResetFlag = True
                        break
                if self.EquipEdit is not None:
                    self.EquipEdit(self.changed_equip_list[k].i_eqp_no, None)
        self.changed_equip_list.clear()
        self.bChangingState.release()

