from typing import List


class StateItem:
    def __init__(self):
        self.DeviceId: int = 0
        self.State: str = None


class MqRtStateMessage:
    def __init__(self):
        self.Time: str = None
        self.Flow: str = None
        self.StateItems: List[StateItem] = []
