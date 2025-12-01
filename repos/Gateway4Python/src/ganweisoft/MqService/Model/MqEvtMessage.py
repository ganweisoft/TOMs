from datetime import datetime
from typing import List

from ganweisoft import MessageLevel


class EquipEventItem:
    def __init__(self):
        self.Msg = None
        self.Msg4Linkage = None
        self.Level = None
        self.OccurDateTime = None
        self.EquipNo = 0

    def __init__(self, msg: str, msg4_linkage: str, level: MessageLevel, occur_date_time: datetime, equip_no: int):
        self.Msg = msg
        self.Msg4Linkage = msg4_linkage
        self.Level = level
        self.OccurDateTime = occur_date_time
        self.EquipNo = equip_no


class EquipEvent:
    def __init__(self):
        self.DeviceId = 0
        self.EquipEvents: List[EquipEventItem] = []

    def __init__(self, device_id: int, equip_events: List[EquipEventItem]):
        self.DeviceId = device_id
        self.EquipEvents = equip_events


class MqEvtMessage:
    def __init__(self):
        self.Time = None
        self.Flow = None
        self.EventItems: List[EquipEvent] = []

    def __init__(self, time: str, flow: str, event_items: List[EquipEvent]):
        self.Time = time
        self.Flow = flow
        self.EventItems = event_items
