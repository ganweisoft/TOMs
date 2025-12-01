import socket


class GWTCPClient:
    def __init__(self, ip: str, port: int):
        self.ip = ip
        self.port = port
        self.socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.socket.connect((ip, port))

    @property
    def IP(self):
        return self.ip

    @property
    def Port(self):
        return self.port

    @property
    def Socket(self):
        return self.socket