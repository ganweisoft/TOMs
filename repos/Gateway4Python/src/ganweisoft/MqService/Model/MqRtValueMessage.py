from typing import List, Dict, Any


class DataItem:
    def __init__(self):
        self.DeviceId = 0
        self.Attribute: Dict[int, Any] = {}

    def __init__(self, device_id: int, attribute: Dict[int, Any]):
        self.DeviceId = device_id
        self.Attribute = attribute


class MqRtValueMessage:
    def __init__(self):
        self.DataType = 0
        self.Time = None
        self.Flow = None
        self.DataItems: List[DataItem] = []

    def __init__(self, data_type: int, time: str, flow: str, data_items: List[DataItem]):
        self.DataType = data_type
        self.Time = time
        self.Flow = flow
        self.DataItems = data_items
