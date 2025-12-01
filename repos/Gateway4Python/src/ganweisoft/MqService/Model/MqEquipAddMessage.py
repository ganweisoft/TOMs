from typing import List

from ganweisoft.MqService.Model.MqMessage import Equip


class MqEquipAddMessage:
    def __init__(self):
        self.Flow = None
        self.FlowType = 0
        self.Equips: List[Equip] = []
