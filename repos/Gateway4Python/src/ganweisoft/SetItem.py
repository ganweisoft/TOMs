import asyncio
import threading
from datetime import time, datetime, timedelta
from typing import Optional

from ganweisoft import DataCenter
from ganweisoft import StationItem


class SetItem:
    def __init__(self, equipno: int, main_instruct: str, minor_instruct: str, value: str, executor: str = "", can_repeat: bool = False):
        self.equipno = equipno
        self.main_instruct = main_instruct
        self.minor_instruct = minor_instruct
        self.value = value
        self.executor = executor
        self.can_repeat = can_repeat
        self.waiting_time = 0
        self.start_tick_count = 0
        self.b_record = False
        self.client_instance_guid = None
        self.description = None
        self.is_cj = False
        self.is_wait_set_parm = False
        self.wait_set_parm_is_finish = None
        self.b_stop_set_parm = False
        self.m_set_no = -1
        self.cj_eqp_no = -1
        self.cj_set_no = -1
        self.cs_response = None
        self.is_synchronization = False
        self.user_ip_and_port = ""
        self.cs_reserve1 = ""
        self.cs_reserve2 = ""
        self.cs_reserve3 = ""
        self.b_only_delay_type = False
        self.i_delay_time = 0
        self.enable = True
        self.set_code = None
        self.oo = threading.Lock()
        self._lock = threading.Lock()

    @property
    def equipno(self) -> int:
        return self._equipno

    @equipno.setter
    def equipno(self, value: int):
        self._equipno = value

    @property
    def main_instruct(self) -> str:
        return self._main_instruct

    @main_instruct.setter
    def main_instruct(self, value: str):
        self._main_instruct = value

    @property
    def minor_instruct(self) -> str:
        return self._minor_instruct

    @minor_instruct.setter
    def minor_instruct(self, value: str):
        self._minor_instruct = value

    @property
    def value(self) -> str:
        return self._value

    @value.setter
    def value(self, value: str):
        self._value = value

    @property
    def executor(self) -> str:
        return self._executor

    @executor.setter
    def executor(self, value: str):
        self._executor = value

    @property
    def can_repeat(self) -> bool:
        return self._can_repeat

    @can_repeat.setter
    def can_repeat(self, value: bool):
        self._can_repeat = value

    @property
    def waiting_time(self) -> int:
        return self._waiting_time

    @waiting_time.setter
    def waiting_time(self, value: int):
        self._waiting_time = value

    @property
    def start_tick_count(self) -> int:
        return self._start_tick_count

    @start_tick_count.setter
    def start_tick_count(self, value: int):
        self._start_tick_count = value

    @property
    def b_record(self) -> bool:
        return self._b_record

    @b_record.setter
    def b_record(self, value: bool):
        self._b_record = value

    @property
    def client_instance_guid(self) -> Optional[str]:
        return self._client_instance_guid

    @client_instance_guid.setter
    def client_instance_guid(self, value: Optional[str]):
        self._client_instance_guid = value

    @property
    def description(self) -> Optional[str]:
        return self._description

    @description.setter
    def description(self, value: Optional[str]):
        self._description = value

    @property
    def is_cj(self) -> bool:
        return self._is_cj

    @is_cj.setter
    def is_cj(self, value: bool):
        self._is_cj = value

    @property
    def is_wait_set_parm(self) -> bool:
        return self._is_wait_set_parm

    @is_wait_set_parm.setter
    def is_wait_set_parm(self, value: bool):
        self._is_wait_set_parm = value

    @property
    def wait_set_parm_is_finish(self) -> Optional[bool]:
        with self.oo:
            return self._wait_set_parm_is_finish

    @wait_set_parm_is_finish.setter
    def wait_set_parm_is_finish(self, value: Optional[bool]):
        with self.oo:
            self._wait_set_parm_is_finish = value

    @property
    def b_stop_set_parm(self) -> bool:
        with self.oo:
            return self._b_stop_set_parm

    @b_stop_set_parm.setter
    def b_stop_set_parm(self, value: bool):
        with self.oo:
            self._b_stop_set_parm = value

    @property
    def m_set_no(self) -> int:
        return self._m_set_no

    @m_set_no.setter
    def m_set_no(self, value: int):
        self._m_set_no = value

    @property
    def cj_eqp_no(self) -> int:
        return self._cj_eqp_no

    @cj_eqp_no.setter
    def cj_eqp_no(self, value: int):
        self._cj_eqp_no = value

    @property
    def cj_set_no(self) -> int:
        return self._cj_set_no

    @cj_set_no.setter
    def cj_set_no(self, value: int):
        self._cj_set_no = value

    @property
    def cs_response(self) -> Optional[str]:
        return self._cs_response

    @cs_response.setter
    def cs_response(self, value: Optional[str]):
        self._cs_response = value

    @property
    def is_synchronization(self) -> bool:
        return self._is_synchronization

    @is_synchronization.setter
    def is_synchronization(self, value: bool):
        self._is_synchronization = value

    @property
    def user_ip_and_port(self) -> str:
        return self._user_ip_and_port

    @user_ip_and_port.setter
    def user_ip_and_port(self, value: str):
        self._user_ip_and_port = value

    @property
    def cs_reserve1(self) -> str:
        return self._cs_reserve1

    @cs_reserve1.setter
    def cs_reserve1(self, value: str):
        self._cs_reserve1 = value

    @property
    def cs_reserve2(self) -> str:
        return self._cs_reserve2

    @cs_reserve2.setter
    def cs_reserve2(self, value: str):
        self._cs_reserve2 = value

    @property
    def cs_reserve3(self) -> str:
        return self._cs_reserve3

    @cs_reserve3.setter
    def cs_reserve3(self, value: str):
        self._cs_reserve3 = value

    @property
    def b_only_delay_type(self) -> bool:
        return self._b_only_delay_type

    @b_only_delay_type.setter
    def b_only_delay_type(self, value: bool):
        self._b_only_delay_type = value

    @property
    def i_delay_time(self) -> int:
        return self._i_delay_time

    @i_delay_time.setter
    def i_delay_time(self, value: int):
        self._i_delay_time = value

    @property
    def enable(self) -> bool:
        with self._lock:
            return self._enable

    @enable.setter
    def enable(self, value: bool):
        with self._lock:
            self._enable = value

    @property
    def set_code(self) -> Optional[str]:
        return self._set_code

    @set_code.setter
    def set_code(self, value: Optional[str]):
        self._set_code = value

    def get_enable_state(self):
        self.get_set_no()
        str_enable = "True"
        if self.m_set_no != -1:
            # 假设DataCenter和StationItem是已经定义好的类
            if DataCenter.get_property_from_reserve("setparm", "Reserve1", self.equipno, self.m_set_no, "GWDataCenter.dll#EnableSetParm", str_enable):
                if str_enable.lower() == "true":
                    self.enable = True
                elif str_enable.lower() == "false":
                    self.enable = False
        self.get_set_code()

    def do_delay(self):
        if self.b_only_delay_type:
            start_time = datetime.now()
            while (datetime.now() - start_time) * 1000 < timedelta(0, 0, self.i_delay_time):
                if self.b_stop_set_parm:
                    return
                asyncio.sleep(0.001)

    def get_set_item_desc(self):
        str_set_nm = ""
        s_str_main_instruct = self.main_instruct if self.main_instruct else ""
        s_str_minor_instruct = self.minor_instruct if self.minor_instruct else ""
        s_str_value = self.value if self.value else ""

        query = [item for item in StationItem.db_setparm if
                 item.equip_no == self.equipno and
                 item.main_instruction == s_str_main_instruct and
                 item.minor_instruction == s_str_minor_instruct and
                 item.value == s_str_value]

        if query:
            str_set_nm = query[0].set_nm
        else:
            query1 = [item for item in StationItem.db_setparm if
                      item.equip_no == self.equipno and item.set_no == self.m_set_no]

            if query1:
                str_set_nm = query1[0].set_nm

        if str_set_nm == "":
            return None

        eqp_nm = ""
        try:
            eqp_item = StationItem.get_equip_item_from_equip_no(self.equipno)
            if eqp_item:
                eqp_nm = eqp_item.equip_nm
        except Exception as e:
            pass

        return f"{eqp_nm}-{str_set_nm}" if eqp_nm else str_set_nm

    def get_set_no(self):
        if self.m_set_no != -1:
            return self.m_set_no

        s_str_main_instruct = self.main_instruct if self.main_instruct else ""
        s_str_minor_instruct = self.minor_instruct if self.minor_instruct else ""
        s_str_value = self.value if self.value else ""

        query = [item for item in StationItem.db_setparm if
                 item.equip_no == self.equipno and
                 item.main_instruction == s_str_main_instruct and
                 item.minor_instruction == s_str_minor_instruct and
                 item.value == s_str_value]

        if not query:
            return -1
        else:
            return query[0].set_no

    def get_set_code(self):
        query = [item for item in StationItem.db_setparm if
                 item.equip_no == self.equipno and item.set_no == self.m_set_no]

        if not query:
            self.set_code = ""
        else:
            self.set_code = query[0].set_code

    def get_record(self):
        if self.m_set_no != -1:
            result = [item for item in StationItem.db_setparm if
                      item.equip_no == self.equipno and item.set_no == self.m_set_no]
        else:
            s_str_main_instruct = self.main_instruct if self.main_instruct else ""
            s_str_minor_instruct = self.minor_instruct if self.minor_instruct else ""
            s_str_value = self.value if self.value else ""

            result = [item for item in StationItem.db_setparm if
                      item.equip_no == self.equipno and
                      item.main_instruction == s_str_main_instruct and
                      item.minor_instruction == s_str_minor_instruct and
                      item.value == s_str_value]

        if not result:
            self.b_record = True
        else:
            self.b_record = result[0].record

    def get_set_type(self):
        try:
            if self.m_set_no == -1:
                s_str_main_instruct = self.main_instruct if self.main_instruct else ""
                s_str_minor_instruct = self.minor_instruct if self.minor_instruct else ""
                s_str_value = self.value if self.value else ""

                query = [item for item in StationItem.db_setparm if
                         item.equip_no == self.equipno and
                         item.main_instruction == s_str_main_instruct and
                         item.minor_instruction == s_str_minor_instruct and
                         item.value == s_str_value]

                if not query:
                    # 假设DataCenter是已经定义好的类
                    Logging.write_log_file("set_type of SetParm is null")
                    return None
                else:
                    return query[0].set_type
            else:
                query = [item for item in StationItem.db_setparm if
                         item.equip_no == self.equipno and item.set_no == self.m_set_no]

                if not query:
                    Logging.write_log_file(f"SetParm 不存在equip_no={self.equipno} set_no={self.m_set_no}的对应项")
                    return None
                else:
                    self.cs_reserve1 = query[0].Reserve1
                    self.cs_reserve2 = query[0].Reserve2
                    self.cs_reserve3 = query[0].Reserve3
                    return query[0].set_type
        except Exception as e:
            return None

    def __str__(self):
        return super().__str__()