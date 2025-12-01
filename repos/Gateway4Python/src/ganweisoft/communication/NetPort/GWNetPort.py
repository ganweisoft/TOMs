import time
from queue import Queue
from socket import socket, error
from typing import List

from ganweisoft.communication.NetPort.GWTCPManager import GWTCPManager
from ganweisoft.interface.ICommunication import ICommunication


class GWNetPort(ICommunication):
    def __init__(self):
        self._queue = Queue()
        self.comm_fault_retry_time = 3
        self.out_time = 1500
        self.wait_time = 100
        self.sockets: List[socket] = []

    @property
    def BufferQueue(self):
        return self._queue

    @BufferQueue.setter
    def BufferQueue(self, value):
        self._queue = value

    @property
    def OutTime(self):
        return self.out_time

    @OutTime.setter
    def OutTime(self, value):
        self.out_time = value

    @property
    def CommWaitTime(self):
        return self.wait_time

    @CommWaitTime.setter
    def CommWaitTime(self, value):
        self.wait_time = value

    @property
    def CommFaultReTryTime(self):
        return self.comm_fault_retry_time

    @CommFaultReTryTime.setter
    def CommFaultReTryTime(self, value):
        self.comm_fault_retry_time = value

    def Initialize(self, item):
        try:
            if item is None:
                return False

            self.InitParam(item.communication_time_param)

            self.BufferQueue.queue.clear()

            local_addr = f"TS/{self.OutTime}-{item.local_addr}"
            self.sockets = GWTCPManager.GetSocket(local_addr)

            if self.sockets is None or len(self.sockets) <= 0:
                time.sleep(self.OutTime / 1000)
                return False

            for sock in self.sockets:
                sock.settimeout(self.out_time / 1000)
            return True
        except Exception as e:
            time.sleep(self.OutTime / 1000)
            return False

    def InitParam(self, communication_param):
        if communication_param is None or len(communication_param.strip()) == 0:
            return
        param = communication_param.split('/')
        if len(param) == 3:
            self.CommWaitTime = int(param[0])
            self.OutTime = int(param[1])
            self.CommFaultReTryTime = int(param[2])

    def Write(self, buffer, offset, size):
        self.Write(buffer, offset, size)

    def Read(self, buffer, offset, size):
        return self.Read(buffer, offset, size)

    def ReadList(self, list_buffer):
        if list_buffer is not None:
            while not self.BufferQueue.empty():
                list_buffer.append(self.BufferQueue.get())
            return len(list_buffer)
        return 0

    def Write(self, buffer, offset, size, socket_flags):
        for sock in self.sockets:
            try:
                if sock is not None:
                    if sock.gettimeout() > 0:
                        # 读取缓存
                        temp_buffer = bytearray(size)
                        self.Read(temp_buffer, 0, len(temp_buffer), socket_flags)
                        time.sleep(0.01)
                    sock.send(buffer[offset:offset+size])
            except error as e:
                if e.errno == 35:
                    # 仍然处于连接状态,但是发送可能被阻塞
                    pass
                else:
                    GWTCPManager.RemoveTCPServerSocket(sock)
                    sock.shutdown(2)
                    sock.close()

    def Read(self, buffer, offset, size, socket_flags = None):
        for sock in self.sockets:
            try:
                bytes_received = sock.recv(size)
                if bytes_received:
                    buffer[offset:offset+len(bytes_received)] = bytes_received
                    return len(bytes_received)
            except:
                pass
        return 0

    def Dispose(self):
        for sock in self.sockets:
            GWTCPManager.RemoveTCPServerSocket(sock)
            sock.shutdown(2)
            sock.close()