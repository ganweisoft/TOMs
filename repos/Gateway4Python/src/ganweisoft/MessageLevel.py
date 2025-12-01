from enum import Enum


class MessageLevel(Enum):
    Wubao = -1
    Info = 0
    SpecalInfo = 9999
    Debug = 10000
    SetParm = 10001
    ZiChan = 10002
    Warn = 10003
    Error = 10004
    Fatal = 10005
