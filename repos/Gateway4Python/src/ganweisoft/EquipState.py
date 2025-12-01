from enum import Enum


class EquipState(Enum):
    NoCommunication = 0
    CommunicationOK = 1
    HaveAlarm = 2
    HaveSetParm = 3
    Initial = 4
    CheFang = 5
    BackUp = 6
