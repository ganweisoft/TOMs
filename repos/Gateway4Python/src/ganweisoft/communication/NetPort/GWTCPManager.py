import socket
import threading
from typing import List, Optional

from ganweisoft.communication.NetPort.GWTCPClient import GWTCPClient
from ganweisoft.communication.NetPort.GWTCPServer import GWTCPServer


class GWTcpType:
    TS = 0
    TC = 1
    Other = 2

class GWTCPManager:
    TSList = []
    TCList = []

    @classmethod
    def GetSocket(cls, local_addr_params):
        comparams = local_addr_params.split('-')
        commparam = comparams[0]
        gw_type = cls.GetNetTypeByParam(commparam)

        if gw_type == GWTcpType.TS:
            port = int(commparam.split('/')[1])
            ip = "127.0.0.1"
            re_port = -1

            if len(comparams) == 2:
                s_ip_end_port = comparams[1]
                s_ip_param = s_ip_end_port.split(':')
                if len(s_ip_param) == 1:
                    ip = s_ip_param[0]
                elif len(s_ip_param) == 2:
                    ip = s_ip_param[0]
                    re_port = int(s_ip_param[1])

            tcp_server = cls.GetTCPServerByPort(port)
            if tcp_server is None:
                tcp_server = GWTCPServer(port)
                with threading.Lock():
                    cls.TSList.append(tcp_server)

            if ip == "127.0.0.1" and re_port == -1:
                return tcp_server.GetAllClientSocket()
            if ip == "127.0.0.1":
                return [tcp_server.GetClientSocketByPort(re_port)]
            return [tcp_server.GetClientSocket(ip, re_port)]
        elif gw_type == GWTcpType.TC:
            s_ip_param = commparam.split('/')[1]
            s_ip_params = s_ip_param.split(':')
            if len(s_ip_params) == 2:
                ip = s_ip_params[0]
                port = int(s_ip_params[1])

                client = cls.GetTCPClient(ip, port)
                if client is None:
                    try:
                        client = GWTCPClient(ip, port)
                        with threading.Lock():
                            cls.TCList.append(client)
                    except Exception as ex:
                        return None
                return [client.Socket]
        return None

    @classmethod
    def GetNetTypeByParam(cls, s_parm):
        commparam = s_parm.split('/')
        if len(commparam) > 0 and commparam[0].lower() == "ts":
            return GWTcpType.TS
        elif len(commparam) > 0 and commparam[0].lower() == "tc":
            return GWTcpType.TC
        else:
            return GWTcpType.Other

    @classmethod
    def GetTCPServerByPort(cls, port):
        for server in cls.TSList:
            if server.Port == port:
                return server
        return None

    @classmethod
    def GetTCPClient(cls, ip: str, port: int):
        for client in cls.TCList:
            if client.IP == ip and client.Port == port:
                return client
        return None

    @classmethod
    def RemoveTCPServerSocket(cls, socket):
        with threading.Lock():
            for server in cls.TSList:
                if server.Port == socket.getsockname()[1]:
                    server.RemoveSocket(socket)
            for client in cls.TCList[:]:
                if client.Socket == socket:
                    cls.TCList.remove(client)