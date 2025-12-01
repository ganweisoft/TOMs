import socket
import threading
from typing import List, Optional


class GWTCPServer:
    def __init__(self, port):
        self._port = port
        self._allDone = threading.Event()
        self._clientList = []
        self.disposed = False
        self.StartListen()

    @property
    def Port(self):
        return self._port

    @Port.setter
    def Port(self, value):
        self._port = value

    def GetClientSocket(self, ip, port):
        for socket in self._clientList:
            if port != -1 and socket.getpeername() == (ip.address, port):
                return socket
            if socket.getpeername()[0] == ip.address:
                return socket
        return None

    def GetClientSocketByPort(self, port):
        if port == -1:
            if self._clientList:
                return self._clientList[0]
            return None

        for socket in self._clientList:
            if socket.getpeername()[1] == port:
                return socket
        return None

    def GetAllClientSocket(self):
        return self._clientList.copy()

    def StartListen(self):
        th_listen = threading.Thread(target=self.Listen)
        th_listen.start()

    def Listen(self):
        try:
            server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            server.bind(('0.0.0.0', self.Port))
            server.listen(20)

            while not self.disposed:
                self._allDone.clear()
                client_socket, _ = server.accept()
                self.Accept(client_socket, server)
                self._allDone.wait()
        except Exception as ex:
            return

    def Accept(self, client_socket, server):
        self._allDone.set()
        for sock in self._clientList[:]:
            try:
                peer = sock.getpeername()
            except:
                if sock in self._clientList:
                    self._clientList.remove(sock)
                continue
            try:
                client_peer = client_socket.getpeername()
            except:
                continue
            if str(peer) == str(client_peer):
                if sock in self._clientList:
                    self._clientList.remove(sock)
        self._clientList.append(client_socket)

    def RemoveSocket(self, socket):
        if socket in self._clientList:
            self._clientList.remove(socket)
