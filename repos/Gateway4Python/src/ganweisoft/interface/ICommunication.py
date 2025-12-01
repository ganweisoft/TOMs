from typing import List

from ganweisoft import EquipItem


class ICommunication:
    @property
    def CommFaultReTryTime(self) -> int:
        pass

    @CommFaultReTryTime.setter
    def CommFaultReTryTime(self, value: int):
        pass

    @property
    def CommWaitTime(self) -> int:
        pass

    @CommWaitTime.setter
    def CommWaitTime(self, value: int):
        pass

    def Initialize(self, item: 'EquipItem') -> bool:
        pass

    def Read(self, buffer: bytearray, offset: int, count: int) -> int:
        pass

    def ReadList(self, list_buffer: List[bytearray]) -> int:
        pass

    def Write(self, buffer: bytearray, offset: int, count: int) -> None:
        pass

    def Dispose(self) -> None:
        pass