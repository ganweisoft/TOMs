import threading
from queue import Queue
from typing import Dict, List, Optional

from ganweisoft.Database.EquipTable import EquipTableRow
from ganweisoft.Database.GWDbProvider import GWDbProvider
from ganweisoft.Database.SetParmTable import SetParmTableRow
from ganweisoft.Database.YcpTable import YcpTableRow
from ganweisoft.Database.YxpTable import YxpTableRow
from ganweisoft.EquipCategory import ChangedEquip, SafetyLevel, SubEquipList, ChangedEquipState
from ganweisoft.EquipItem import EquipItem, DelayEventFire
from ganweisoft.Logging import Logging
from ganweisoft import DataCenter


class StationItem:
    ChangedEquipListChanged = None
    ChangedEquipListChangedDelayEvent: DelayEventFire = None
    ChangedEquipList: List[ChangedEquip] = []
    m_SafetyLevel: SafetyLevel = SafetyLevel.Safety
    EquipCategoryDict: Dict[str, SubEquipList] = {}
    SetItemQueue: Queue = []
    SubEquipListDict: Dict[str, List[EquipTableRow]] = None
    EqpDt = None

    AddNewSubEquipList = None
    SetParmResultEvent = None
    SetParmResponseEvent = None
    AppClose = None
    RightHandWave = None
    LeftHandWave = None
    StationCommError = None
    StationCommOk = None
    StationHaveAlarm = None
    StationNoAlarm = None
    EquipBackUpChanged = None
    HaveEquipChanged = None
    HaveEquipReset = None

    resetequip = False
    db_eqp: List[EquipTableRow] = []
    db_ycp: List[YcpTableRow] = []
    db_yxp: List[YxpTableRow] = []
    db_setparm: List[SetParmTableRow] = []

    db_eqp_lock = threading.Lock()
    db_ycp_lock = threading.Lock()
    db_yxp_lock = threading.Lock()
    db_setparm_lock = threading.Lock()

    @classmethod
    def get_equip_item_from_equip_no(cls, i_equip_no: int) -> Optional[EquipItem]:
        with threading.Lock():
            for pair in cls.EquipCategoryDict.items():
                equip_list = pair[1]
                for equip in equip_list.equip_list:
                    if equip.iequipno == i_equip_no:
                        return equip
        return None

    @classmethod
    def add_changed_equip(cls, equip: ChangedEquip):
        with threading.Lock():
            cls.ChangedEquipList.append(equip)
        if cls.ChangedEquipListChangedDelayEvent is None:
            cls.ChangedEquipListChanged = cls.station_item_changed_equip_list_changed
            cls.ChangedEquipListChangedDelayEvent = DelayEventFire(cls.ChangedEquipListChanged, 500,
                                                                   cls.ChangedEquipList, None)
        cls.ChangedEquipListChangedDelayEvent.AddEvent()

    @classmethod
    def station_item_changed_equip_list_changed(cls, sender, e):
        with threading.Lock():
            cls.update_main_data_table()
            for equip in cls.ChangedEquipList:
                sub_equip_list = cls.get_sub_equip_list(equip.i_sta_no, equip.i_eqp_no, equip.state)
                if sub_equip_list is not None:
                    sub_equip_list.start_refresh_thread()
                    sub_equip_list.start_set_parm_thread()
                    sub_equip_list.AddChangedEquipList1(equip)
            cls.ChangedEquipList.clear()

    @classmethod
    def get_sub_equip_list(cls, i_sta_no: int, i_eqp_no: int, state: ChangedEquipState) -> Optional[SubEquipList]:
        sub_equip_list = None
        this_sub_equip_list = None

        with threading.Lock():
            if state == ChangedEquipState.Delete:
                for pair in cls.EquipCategoryDict.items():
                    equip_list = pair[1]
                    for equip in equip_list.equip_list:
                        if equip.iequipno == i_eqp_no:
                            return equip_list

            r = next((item for item in cls.db_eqp if item.equip_no == i_eqp_no), None)
            if r is not None:
                str_local_addr = r.local_addr.upper().strip()
            else:
                equip_item = cls.get_equip_item_from_equip_no(i_eqp_no)
                if equip_item is None:
                    return None
                str_local_addr = equip_item.local_addr.upper().strip()

            if str_local_addr is None:
                return None

            for pair in cls.EquipCategoryDict.items():
                equip_list = pair[1]
                if equip_list.local_addr == str_local_addr:
                    sub_equip_list = equip_list
                    this_sub_equip_list = equip_list
                    break

            if sub_equip_list is None:
                cls.SubEquipListDict = cls.get_sub_equip_list_data_row(cls.db_eqp)
                new_equip_list = SubEquipList(str_local_addr, cls.SubEquipListDict[str_local_addr])
                this_sub_equip_list = new_equip_list
                if new_equip_list.b_can_execute:
                    cls.EquipCategoryDict[str_local_addr] = new_equip_list

                if cls.AddNewSubEquipList is not None:
                    cls.AddNewSubEquipList(new_equip_list, None)

            if state == ChangedEquipState.Edit:
                for pair in cls.EquipCategoryDict.items():
                    equip_list = pair[1]
                    if equip_list.local_addr != this_sub_equip_list.local_addr:
                        for equip in equip_list.equip_list:
                            if equip.iequipno == i_eqp_no:
                                equip_list.equip_list.remove(equip)
                                break

            return this_sub_equip_list

    @classmethod
    def update_main_data_table(cls):
        try:
            cls.db_eqp = GWDbProvider().GetEquipTableList()
            cls.db_ycp = GWDbProvider().GetYcpTableList()
            cls.db_yxp = GWDbProvider().GetYxpTableList()
            cls.db_setparm = GWDbProvider().GetSetParmTableList()
        except Exception as e:
            Logging.write_log_file(str(e))

    @classmethod
    def get_sub_equip_list_data_row(cls, rows: List[EquipTableRow]) -> Dict[str, List[EquipTableRow]]:
        ds = {}
        try:
            for row in rows:
                key = row.local_addr.upper().strip()
                if key in ds:
                    ds[key].append(row)
                else:
                    ds[key] = [row]
        except Exception as e:
            Logging.write_log_file(f"{str(e)} 检查local_addr字段不能为空")
        return ds

    @classmethod
    def init(cls) -> bool:
        DataCenter.brunning = True
        GWDbProvider().init()
        Logging.write_log_file("初始化数据库")

        cls.update_main_data_table()

        cls.SubEquipListDict = cls.get_sub_equip_list_data_row(cls.db_eqp)

        for pair in cls.SubEquipListDict.items():
            sub_equip_list = SubEquipList(pair[0], pair[1])
            if sub_equip_list.b_can_execute:
                with threading.Lock():
                    cls.EquipCategoryDict[pair[0]] = sub_equip_list

        Logging.write_log_file("初始化其它服务")
        return True

    @classmethod
    def DoHaveEquipChanged(cls):
        if cls.HaveEquipChanged:
            cls.HaveEquipChanged()

    @classmethod
    def FireSetParmResultEvent(cls, set_item):
        if cls.SetParmResultEvent:
            cls.SetParmResultEvent(set_item)
