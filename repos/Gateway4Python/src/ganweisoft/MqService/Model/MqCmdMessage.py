class MqCmdMessage:
    def __init__(self):
        self.RequestId = None
        self.LoginName = None
        self.Flow = None
        self.MainInstruct = None
        self.MinorInstruct = None
        self.Value = None
        self.GatewayId = 0
        self.EquipNo = 0
        self.SetNo = 0
        self.TerminalIdentityId = 0