from datetime import datetime
from enum import Enum
from typing import Dict, List, Optional, Union, Tuple

from ganweisoft import MessageLevel
from ganweisoft import EquipItem


class CommunicationState(Enum):
    fail = 0
    ok = 1
    setreturn = 2
    retry = 3


class EquipEvent:
    def __init__(self, msg: str, level: MessageLevel, dt: datetime, msg4Linkage: Optional[str] = None,
                 iEquipNo: Optional[int] = None):
        self.msg = msg
        self.msg4Linkage = msg4Linkage if msg4Linkage is not None else msg
        self.level = level
        self.dt = dt
        self.iEquipNo = iEquipNo


class IEquip:
    @property
    def m_sta_no(self) -> int:
        pass

    @m_sta_no.setter
    def m_sta_no(self, value: int):
        pass

    @property
    def m_equip_no(self) -> int:
        pass

    @m_equip_no.setter
    def m_equip_no(self, value: int):
        pass

    @property
    def m_retrytime(self) -> int:
        pass

    @m_retrytime.setter
    def m_retrytime(self, value: int):
        pass

    @property
    def YCResults(self) -> Dict[int, object]:
        pass

    @property
    def YXResults(self) -> Dict[int, object]:
        pass

    @property
    def EquipEventList(self) -> List[EquipEvent]:
        pass

    @property
    def RunSetParmFlag(self) -> bool:
        pass

    @RunSetParmFlag.setter
    def RunSetParmFlag(self, value: bool):
        pass

    @property
    def ResetFlag(self) -> bool:
        pass

    @ResetFlag.setter
    def ResetFlag(self, value: bool):
        pass

    @property
    def equipitem(self) -> EquipItem:
        pass

    @equipitem.setter
    def equipitem(self, value: EquipItem):
        pass

    @property
    def bCanConfirm2NormalState(self) -> bool:
        pass

    @bCanConfirm2NormalState.setter
    def bCanConfirm2NormalState(self, value: bool):
        pass

    @property
    def SetParmExecutor(self) -> str:
        pass

    @SetParmExecutor.setter
    def SetParmExecutor(self, value: str):
        pass

    def init(self, item: EquipItem) -> bool:
        pass

    def GetData(self, pEquip) -> CommunicationState:
        pass

    def SetParm(self, cmd1: str, cmd2: str, value: str) -> bool:
        pass

    def Confirm2NormalState(self, sYcYxType: str, iYcYxNo: int) -> bool:
        pass

    def CloseCommunication(self) -> bool:
        pass
