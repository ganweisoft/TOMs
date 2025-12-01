from typing import List


class MqEquipDelMessage:
    def __init__(self):
        self.Flow = None
        self.FlowType = 0
        self.EquipNos: List[int] = []
