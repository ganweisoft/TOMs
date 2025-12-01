from abc import ABC, abstractmethod
from typing import List, Optional, Union

from ganweisoft import StationItem
from ganweisoft import EquipItem
from ganweisoft.communication.NetPort import GWNetPort
from ganweisoft.communication.SerialPort.SerialPort import SZ_SerialPort
from ganweisoft.interface.ICommunication import ICommunication


class SerialPort(ICommunication):
    def __init__(self):
        self.Instance: Optional[ICommunication] = None
        self.szLocal_addr = ""

    @property
    def CommFaultReTryTime(self) -> int:
        if self.Instance is not None:
            return self.Instance.CommFaultReTryTime
        return 3

    @CommFaultReTryTime.setter
    def CommFaultReTryTime(self, value: int) -> None:
        if self.Instance is not None:
            self.Instance.CommFaultReTryTime = value

    @property
    def CommWaitTime(self) -> int:
        if self.Instance is not None:
            return self.Instance.CommWaitTime
        return 500

    @CommWaitTime.setter
    def CommWaitTime(self, value: int) -> None:
        if self.Instance is not None:
            self.Instance.CommWaitTime = value

    def Initialize(self, item: EquipItem) -> bool:
        str_flag = item.local_addr
        self.szLocal_addr = item.local_addr
        str_flag1 = item.communication_param
        str_flag = str_flag.upper().strip()
        str_flag1 = str_flag1.upper().strip()
        flag = False

        if self.Instance is None:
            if str_flag.startswith("TS") or str_flag.startswith("TC"):
                self.Instance = GWNetPort()
            else:
                self.Instance = SZ_SerialPort()

        if self.Instance is None:
            return False

        try:
            flag = self.Instance.Initialize(item)
            if "DATASIMU.NET.DLL" in item.communication_drv.upper():
                flag = True
        except Exception as e:
            pass

        return flag

    def Read(self, buffer: bytearray, offset: int, count: int) -> int:
        try:
            if self.Instance is not None:
                return self.Instance.Read(buffer, offset, count)
        except Exception as e:
            pass
        return 0

    def ReadList(self, list_buffer: List[bytearray]) -> int:
        try:
            if self.Instance is not None:
                return self.Instance.ReadList(list_buffer)
        except Exception as e:
            pass
        return 0

    def Write(self, buffer: bytearray, offset: int, count: int) -> None:
        try:
            if self.Instance is not None:
                self.Instance.Write(buffer, offset, count)
        except Exception as e:
            pass

    def Dispose(self) -> None:
        if not StationItem.EquipCategoryDict.get(self.szLocal_addr):
            if self.Instance is not None:
                self.Instance.Dispose()