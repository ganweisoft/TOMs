import random
import threading
import os
import sys

sys.path.append(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

from ganweisoft.Database.YcpTable import YcpTableRow
from ganweisoft.Database.YxpTable import YxpTableRow
from ganweisoft.Logging import Logging
from ganweisoft.interface.CEquipBase import CEquipBase
from ganweisoft.interface import CommunicationState


class CEquip(CEquipBase):
    def __init__(self):
        super().__init__()
        self.data_fetch_counter = 0
        self.lock = threading.Lock()

    def GetData(self, pEquip) -> CommunicationState:
        super().Sleep(1000)
        if super().RunSetParmFlag:
            return CommunicationState.setreturn

        comm_state = super().GetData(pEquip)
        if comm_state != CommunicationState.ok:
            return comm_state

        if not pEquip.GetEvent():
            return CommunicationState.fail
        return CommunicationState.ok

    def GetYC(self, row: YcpTableRow) -> bool:
        min_val = max(row.val_min, 0.0)
        max_val = min(row.val_max, 100.0)
        super().SetYCData(row, int(random.uniform(min_val, max_val)))
        return True

    def GetYX(self, row: YxpTableRow) -> bool:
        yx_value = random.choice([True, False])
        super().SetYXData(row, yx_value)
        return True

    def SetParm(self, main_instruct: str, minor_instruct: str, value: str) -> bool:
        try:
            if main_instruct.lower() == "setycyxvalue":
                if minor_instruct is None or len(minor_instruct) < 3:
                    Logging.write_log_file(f"Invalid MinorInstruct format: {minor_instruct}")
                    return False

                prefix = minor_instruct[0].lower()
                index_str = minor_instruct[2:]
                if not index_str.isdigit():
                    Logging.write_log_file(f"Invalid YC/YX index: {index_str}")
                    return False

                ycyx_no = int(index_str)
                if ycyx_no <= 0:
                    Logging.write_log_file(f"YC/YX index must be > 0: {ycyx_no}")
                    return False

                if prefix == 'c':  # YC
                    if value is None or len(value) < 0:
                        Logging.write_log_file("Missing YC value")
                        return False
                    yc_value = float(value)
                    with self.lock:
                        yc_results = super().YCResults
                        yc_results[ycyx_no] = yc_value
                    return True
                elif prefix == 'x':  # YX
                    yx_value = int(value) > 0
                    with self.lock:
                        yx_results = super().YXResults
                        yx_results[ycyx_no] = yx_value
                    return True
            return False
        except ValueError as e:
            Logging.write_log_file(f"Number format error: {str(e)}")
            return False
        except Exception as e:
            Logging.write_log_file(f"SetParm error: {str(e)}")
            return False


if __name__ == '__main__':
    from ganweisoft.DataCenter import DataCenter

    DataCenter.start()
