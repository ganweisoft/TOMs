import threading
import time
from typing import Dict, List, Optional, Tuple, Union

from ganweisoft.Database.YcpTable import YcpTableRow
from ganweisoft.Database.YxpTable import YxpTableRow
from ganweisoft import StationItem
from ganweisoft import EquipItem
from ganweisoft.General import General
from ganweisoft.communication.Communication import SerialPort
from ganweisoft.interface.IEquip import IEquip, CommunicationState, EquipEvent
from ganweisoft.Logging import Logging


class CEquipBase(IEquip):
    def __init__(self):
        self.iSta_no = 0
        self.iEquip_no = 0
        self.iRetrytime = 3
        self.b = True
        self.myequipitem = None
        self.serialport = SerialPort()
        self.ycprows = []
        self.yxprows = []
        self.equiprow = None
        self.EquipNm = ""
        self.ycresults = {}
        self.yxresults = {}
        self.equipEventlist = []
        self.runSetParmFlag = False
        self.resetFlag = False
        self.ycpdataflag = {}
        self.yxpdataflag = {}
        self.setparmexecutor = None
        self.bSetCacheData = False
        self.bFirstGetData = True

    @property
    def RunSetParmFlag(self) -> bool:
        return self.runSetParmFlag

    @RunSetParmFlag.setter
    def RunSetParmFlag(self, value: bool):
        self.runSetParmFlag = value

    @property
    def ResetFlag(self) -> bool:
        return self.resetFlag

    @ResetFlag.setter
    def ResetFlag(self, value: bool):
        self.resetFlag = value

    @property
    def equipitem(self) -> EquipItem:
        return self.myequipitem

    @equipitem.setter
    def equipitem(self, value: EquipItem):
        self.myequipitem = value
        self.serialport = value.serial_port

    @property
    def YCResults(self) -> Dict[int, object]:
        return self.ycresults

    @property
    def YXResults(self) -> Dict[int, object]:
        return self.yxresults

    @property
    def EquipEventList(self) -> List[EquipEvent]:
        return self.equipEventlist

    @property
    def m_sta_no(self) -> int:
        return self.iSta_no

    @m_sta_no.setter
    def m_sta_no(self, value: int):
        self.iSta_no = value

    @property
    def m_equip_no(self) -> int:
        return self.iEquip_no

    @m_equip_no.setter
    def m_equip_no(self, value: int):
        self.iEquip_no = value

    @property
    def m_retrytime(self) -> int:
        return self.iRetrytime

    @m_retrytime.setter
    def m_retrytime(self, value: int):
        self.iRetrytime = value

    @property
    def bCanConfirm2NormalState(self) -> bool:
        return self.myequipitem.bCanConfirm2NormalState if self.myequipitem else False

    @bCanConfirm2NormalState.setter
    def bCanConfirm2NormalState(self, value: bool):
        if self.myequipitem:
            self.myequipitem.bCanConfirm2NormalState = value

    @property
    def SetParmExecutor(self) -> str:
        return self.setparmexecutor

    @SetParmExecutor.setter
    def SetParmExecutor(self, value: str):
        self.setparmexecutor = value

    def init(self, item: EquipItem) -> bool:
        if item is None:
            Logging.write_log_file("CEquipBase调用init(EquipItem item)时，item==null)")
            return False
        self.iSta_no = item.istano
        self.iEquip_no = item.iequipno
        self.m_retrytime = self.serialport.CommFaultReTryTime
        self.bSetCacheData = item.b_set_cache_data
        if self.b or self.ResetFlag:
            self.myequipitem = item
            self.equiprow = next((row for row in StationItem.StationItem.db_eqp if row.equip_no == self.iEquip_no), None)
            if self.equiprow is None:
                return False
            self.ycprows = [row for row in StationItem.StationItem.db_ycp if row.equip_no == self.iEquip_no]
            self.yxprows = [row for row in StationItem.StationItem.db_yxp if row.equip_no == self.iEquip_no]
            self.EquipNm = self.equiprow.equip_nm
            self.b = False
            self.OnLoaded()
        return True

    def Sleep(self, t: int, bBreak: bool = True):
        if not self.myequipitem.IsRageMode:
            time.sleep(t / 1000)
        else:
            if not bBreak:
                time.sleep(t / 1000)
            else:
                count = t // 10
                for _ in range(count):
                    if self.RunSetParmFlag:
                        break
                    time.sleep(0.01)

    def GetData(self, pEquip) -> CommunicationState:
        try:
            if not isinstance(pEquip, CEquipBase):
                return CommunicationState.fail

            if self.RunSetParmFlag:
                return CommunicationState.setreturn
            if self.ycprows:
                for r in self.ycprows:
                    if self.RunSetParmFlag:
                        return CommunicationState.setreturn
                    if not self.GetYC(r):
                        return CommunicationState.fail
            if self.yxprows:
                for r in self.yxprows:
                    if self.RunSetParmFlag:
                        return CommunicationState.setreturn
                    if not self.GetYX(r):
                        return CommunicationState.fail
            if not self.GetEvent():
                return CommunicationState.fail
            if self.bFirstGetData:
                self.bFirstGetData = False
        except Exception as e:
            Logging.write_log_file(General.GetExceptionInfo(e))
            return CommunicationState.fail
        return CommunicationState.ok

    def OnLoaded(self) -> bool:
        return True

    def GetYC(self, r: YcpTableRow) -> bool:
        return False

    def GetYX(self, r: YxpTableRow) -> bool:
        return False

    def GetEvent(self) -> bool:
        return True

    def SetParm(self, MainInstruct: str, MinorInstruct: str, Value: str) -> bool:
        return False

    def Confirm2NormalState(self, sYcYxType: str, iYcYxNo: int) -> bool:
        return False

    def CloseCommunication(self) -> bool:
        return True

    def YCToPhysic(self, r: YcpTableRow):
        pass

    def YXToPhysic(self, r: YxpTableRow):
        pass

    def SetYCDataNoRead(self, rows: List[YcpTableRow]):
        if not rows:
            return
        for r in rows:
            iycno = r.yc_no
            if iycno not in self.ycpdataflag:
                self.ycpdataflag[iycno] = False
            else:
                self.ycpdataflag[iycno] = False

    def SetYXDataNoRead(self, rows: List[YxpTableRow]):
        if not rows:
            return
        for r in rows:
            iyxno = r.yx_no
            if iyxno not in self.yxpdataflag:
                self.yxpdataflag[iyxno] = False
            else:
                self.yxpdataflag[iyxno] = False

    def SetYCData(self, r: YcpTableRow, o: object):
        iycno = r.yc_no
        with threading.Lock():
            if iycno not in self.ycresults:
                self.ycresults[iycno] = o
            else:
                self.ycresults[iycno] = o

    def GetYCData(self, r: YcpTableRow) -> object:
        with threading.Lock():
            iycno = r.yc_no
            return self.ycresults.get(iycno)

    def SetYXData(self, r: YxpTableRow, o: object):
        iyxno = r.yx_no
        with threading.Lock():
            if iyxno not in self.yxresults:
                self.yxresults[iyxno] = o
            else:
                self.yxresults[iyxno] = o

    def GetYXData(self, r: YxpTableRow) -> object:
        with threading.Lock():
            iyxno = r.yx_no
            return self.yxresults.get(iyxno)
