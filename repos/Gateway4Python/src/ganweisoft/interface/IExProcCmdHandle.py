from ganweisoft.Database.GWExProcTable import GWExProcTableRow


class IExProcCmdHandle:
    def init(self, row: GWExProcTableRow) -> bool:
        pass

    def SetParm(self, main_instruction: str, minor_instruction: str, value: str) -> None:
        pass