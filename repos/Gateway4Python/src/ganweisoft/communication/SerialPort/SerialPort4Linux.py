import re
import time
from typing import List, Optional, Union

import serial
from serial import SerialException

from ganweisoft.EquipItem import EquipItem


class SerialPort4Linux:
    def __init__(self):
        self.equipno = 0
        self.Equip = None
        self.commFaultReTryTime = 3
        self.commWaitTime = 100
        self.MyserialPort: serial.Serial = serial.Serial()
        self.ss = self.get_port_names()
        self.portName = ""
        self.param = ""
        self.timeparam = ""
        self.bSerialError = False

    def get_port_names(self):
        return [f"COM{i}" for i in range(1, 257)]

    @property
    def EquipNo(self):
        return self.equipno

    @EquipNo.setter
    def EquipNo(self, value):
        self.equipno = value

    @property
    def CommFaultReTryTime(self):
        return self.commFaultReTryTime

    @CommFaultReTryTime.setter
    def CommFaultReTryTime(self, value):
        self.commFaultReTryTime = value

    @property
    def CommWaitTime(self):
        return self.commWaitTime

    @CommWaitTime.setter
    def CommWaitTime(self, value):
        self.commWaitTime = value

    def __del__(self):
        self.Dispose()

    def Dispose(self):
        if self.MyserialPort is not None:
            if self.MyserialPort.is_open:
                self.MyserialPort.close()
            self.MyserialPort = None

    def GetSerialPort(self):
        return self.MyserialPort

    def VerifyPortNm(self, portName: str) -> bool:
        if re.match(r"[cC][oO][mM][1-9]\d*", portName):
            return True
        return False

    def VerifyParam(self, param: str) -> bool:
        if re.match(r"[\d]+/[\d]/[\d]/[\w]", param):
            return True
        return False

    def VerifyTimeParam(self, param: str) -> bool:
        if re.match(r"[\d]+/[\d]+/[\d]+/[\d]+", param):
            return True
        return False

    def Initialize(self, item: EquipItem) -> bool:
        portName = item.local_addr
        param = item.communication_param
        timeparam = item.communication_time_param
        equipno = item.iequipno
        self.Equip = item

        if self.VerifyPortNm(portName):
            portName = portName.strip().upper()
            if portName not in self.ss:
                if not self.bSerialError:
                    self.bSerialError = True
                return False
            else:
                if self.IsOpen:
                    self.Close()
                self.MyserialPort = serial.Serial()
                self.MyserialPort.port = portName
        else:
            return False

        if self.VerifyParam(param):
            ParamArray = param.split('/')
            self.MyserialPort.baudrate = int(ParamArray[0])
            self.MyserialPort.bytesize = int(ParamArray[1])
            ParamArray[2] = ParamArray[2].strip().upper()
            if ParamArray[2] == "1":
                self.MyserialPort.stopbits = serial.STOPBITS_ONE
            elif ParamArray[2] == "1.5":
                self.MyserialPort.stopbits = serial.STOPBITS_ONE_POINT_FIVE
            elif ParamArray[2] == "2":
                self.MyserialPort.stopbits = serial.STOPBITS_TWO
            else:
                return False
            ParamArray[3] = ParamArray[3].strip().upper()
            if ParamArray[3] == "NO":
                self.MyserialPort.parity = serial.PARITY_NONE
            elif ParamArray[3] == "EVEN":
                self.MyserialPort.parity = serial.PARITY_EVEN
            elif ParamArray[3] == "ODD":
                self.MyserialPort.parity = serial.PARITY_ODD
            elif ParamArray[3] == "MARK":
                self.MyserialPort.parity = serial.PARITY_MARK
            elif ParamArray[3] == "SPACE":
                self.MyserialPort.parity = serial.PARITY_SPACE
            else:
                return False
        else:
            return False

        if self.VerifyTimeParam(timeparam):
            TimeParamArray = timeparam.split('/')
            self.MyserialPort.write_timeout = int(TimeParamArray[0])
            self.MyserialPort.timeout = int(TimeParamArray[0])
            self.commFaultReTryTime = int(TimeParamArray[2])
            self.commWaitTime = int(TimeParamArray[3])
        else:
            self.MyserialPort.write_timeout = 1000
            self.MyserialPort.timeout = 1000
            self.commFaultReTryTime = 3
            self.commWaitTime = 500

        self.MyserialPort.rts = True
        self.MyserialPort.dtr = True

        if self.IsOpen:
            self.MyserialPort.reset_input_buffer()
            self.MyserialPort.reset_output_buffer()

        return True

    def Open(self) -> bool:
        try:
            if self.MyserialPort.is_open:
                self.MyserialPort.close()
            time.sleep(0.02)
            self.MyserialPort.open()
            time.sleep(0.02)
            self.MyserialPort.reset_input_buffer()
            self.MyserialPort.reset_output_buffer()
        except Exception as e:
            return False
        return True

    def Close(self):
        if self.MyserialPort.is_open:
            self.MyserialPort.close()

    def Write(self, buffer: bytearray, offset: int, count: int):
        if not self.VerifyPortNm(self.portName):
            return

        msg = ""

        try:
            if not self.IsOpen:
                self.Open()
                time.sleep(0.02)
            if self.IsOpen:
                self.MyserialPort.write(buffer[offset:offset+count])
            self.MyserialPort.reset_output_buffer()
        except Exception as ex:
            pass

        if self.Equip is not None and self.Equip.IsDebug:
            msg = f"{self.portName}>>Write>>"
            msg += buffer[offset:offset+count].decode('latin-1')
            msg += "("
            for k in range(count):
                msg += f"{buffer[offset + k]}/"
            msg += ")"
            print(msg)

    def ReadList(self, list_buffer: list):
        return 0

    def Read(self, buffer: bytearray, offset: int, count: int) -> int:
        if not self.VerifyPortNm(self.portName):
            return 0

        msg = ""
        iRet = 0

        try:
            if not self.IsOpen:
                self.Open()
                time.sleep(0.02)
            data = self.MyserialPort.read(count)
            if data:
                buffer[offset:offset+len(data)] = data
                iRet = len(data)
                self.MyserialPort.reset_input_buffer()
        except serial.SerialTimeoutException:
            msg += "Timeout  "
        except Exception as ex:
            pass

        if self.Equip is not None and self.Equip.IsDebug:
            msg = f"{self.portName}>>Read[{iRet}/{count}]>>"
            msg += buffer[offset:offset+iRet].decode('latin-1')
            msg += "("
            for k in range(iRet):
                msg += f"{buffer[offset + k]}/"
            msg += ")"
            print(msg)

        return iRet

    @property
    def IsOpen(self):
        return self.MyserialPort.is_open if self.MyserialPort else False