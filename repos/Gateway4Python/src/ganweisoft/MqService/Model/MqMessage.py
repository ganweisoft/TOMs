from typing import List, Dict, Optional


class MqMessage:
    def __init__(self):
        self.Flow = None
        self.FlowType = 0
        self.Equips: List[Equip] = []


class Equip:
    def __init__(self):
        self.EquipNo = 0
        self.StaN = 0
        self.EquipNm = None
        self.EquipDetail = None
        self.AccCyc = 0
        self.RelatedPic = None
        self.ProcAdvice = None
        self.OutOfContact = None
        self.Contacted = None
        self.EventWav = None
        self.CommunicationDrv = None
        self.LocalAddr = None
        self.EquipAddr = None
        self.CommunicationParam = None
        self.CommunicationTimeParam = None
        self.RawEquipNo = 0
        self.Tabname = None
        self.AlarmScheme = 0
        self.Attrib = 0
        self.StaIp = None
        self.AlarmRiseCycle = None
        self.Reserve1 = None
        self.Reserve2 = None
        self.Reserve3 = None
        self.RelatedVideo = None
        self.ZiChanId = None
        self.PlanNo = None
        self.SafeTime = None
        self.Backup = None
        self.Ycps: List[Ycp] = []
        self.Yxps: List[Yxp] = []
        self.SetParms: List[SetParm] = []

    def __init__(self, d=None):
        if d is not None:
            for key, value in d.items():
                if key[0].islower():
                    setattr(self, (key[0].upper() + key[1:]), value)
                else:
                    setattr(self, key, value)


class Ycp:
    def __init__(self):
        self.YcNo = 0
        self.StaN = 0
        self.EquipNo = 0
        self.YcNm = None
        self.Mapping = None
        self.YcMin = 0.0
        self.YcMax = 0.0
        self.PhysicMin = 0.0
        self.PhysicMax = 0.0
        self.ValMin = 0.0
        self.RestoreMin = 0.0
        self.RestoreMax = 0.0
        self.ValMax = 0.0
        self.ValTrait = 0
        self.MainInstruction = None
        self.MinorInstruction = None
        self.AlarmAcceptableTime = 0
        self.RestoreAcceptableTime = 0
        self.AlarmRepeatTime = 0
        self.ProcAdvice = None
        self.LvlLevel = 0
        self.OutminEvt = None
        self.OutmaxEvt = None
        self.WaveFile = None
        self.RelatedPic = None
        self.AlarmScheme = 0
        self.CurveRcd = False
        self.CurveLimit = None
        self.AlarmShield = None
        self.Unit = None
        self.AlarmRiseCycle = None
        self.Reserve1 = None
        self.Reserve2 = None
        self.Reserve3 = None
        self.RelatedVideo = None
        self.ZiChanId = None
        self.PlanNo = None
        self.SafeTime = None
        self.SafeBgn = None
        self.SafeEnd = None
        self.GWValue = None
        self.GWTime = None
        self.DataType = None
        self.YcCode = None

    def __init__(self, d=None):
        if d is not None:
            for key, value in d.items():
                if key[0].islower():
                    setattr(self, (key[0].upper() + key[1:]), value)
                else:
                    setattr(self, key, value)


class Yxp:
    def __init__(self):
        self.YxNo = 0
        self.StaN = 0
        self.EquipNo = 0
        self.YxNm = None
        self.ProcAdviceR = None
        self.ProcAdviceD = None
        self.LevelR = 0
        self.LevelD = 0
        self.Evt01 = None
        self.Evt10 = None
        self.MainInstruction = None
        self.MinorInstruction = None
        self.AlarmAcceptableTime = 0
        self.RestoreAcceptableTime = 0
        self.AlarmRepeatTime = 0
        self.WaveFile = None
        self.RelatedPic = None
        self.AlarmScheme = 0
        self.Inversion = False
        self.Initval = 0
        self.ValTrait = 0
        self.AlarmShield = None
        self.AlarmRiseCycle = None
        self.RelatedVideo = None
        self.ZiChanId = None
        self.PlanNo = None
        self.SafeTime = None
        self.CurveRcd = False
        self.Reserve1 = None
        self.Reserve2 = None
        self.Reserve3 = None
        self.SafeBgn = None
        self.SafeEnd = None
        self.GWValue = None
        self.GWTime = None
        self.DataType = None
        self.YxCode = None
        self.Equip = None

    def __init__(self, d=None):
        if d is not None:
            for key, value in d.items():
                if key[0].islower():
                    setattr(self, (key[0].upper() + key[1:]), value)
                else:
                    setattr(self, key, value)


class SetParm:
    def __init__(self):
        self.SetNo = 0
        self.StaN = 0
        self.EquipNo = 0
        self.SetNm = None
        self.SetType = None
        self.MainInstruction = None
        self.MinorInstruction = None
        self.Record = False
        self.Action = None
        self.Value = None
        self.Canexecution = False
        self.VoiceKeys = None
        self.EnableVoice = False
        self.QrEquipNo = 0
        self.Reserve1 = None
        self.Reserve2 = None
        self.Reserve3 = None
        self.SetCode = None

    def __init__(self, d=None):
        if d is not None:
            for key, value in d.items():
                if key[0].islower():
                    setattr(self, (key[0].upper() + key[1:]), value)
                else:
                    setattr(self, key, value)
