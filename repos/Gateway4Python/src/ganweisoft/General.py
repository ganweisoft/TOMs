import os
import re
import datetime
import uuid


class General:
    CfgValue1 = "MicrosoftWPF4.0"
    CfgValue2 = "MicrosoftWCF4.0"

    @staticmethod
    def Convert2DT(dt):
        return datetime.datetime(dt.year, dt.month, dt.day, dt.hour, dt.minute, dt.second)

    @staticmethod
    def VerifyStringFormat(s, s1):
        regex = re.compile(s)
        return regex.match(s1) is not None

    @staticmethod
    def GetExceptionInfo(e):
        return f"{e.__class__.__name__}: {str(e)}"

    @staticmethod
    def GetApplicationRootPath():
        try:
            exe = os.path.abspath(__file__)
            return os.path.join(os.path.dirname(exe), "..")
        except:
            return os.getcwd()

    @staticmethod
    def GetExecutingAssemblyFileName():
        return os.path.basename(__file__)

    @staticmethod
    def GetDayOfWeek(t):
        return {
            0: 2,  # Monday
            1: 3,  # Tuesday
            2: 4,  # Wednesday
            3: 5,  # Thursday
            4: 6,  # Friday
            5: 7,  # Saturday
            6: 1  # Sunday
        }.get(t.weekday(), 1)

    @staticmethod
    def GetString1(mac_addr):
        str_id = "AlarmCenter1"
        str_id += mac_addr
        str_id = str_id.replace(':', '8')
        str_id = str_id.replace(' ', 'F')
        char_array = list(str_id)
        char_array.reverse()
        str_id = ''.join(char_array)[:8]
        return str_id
