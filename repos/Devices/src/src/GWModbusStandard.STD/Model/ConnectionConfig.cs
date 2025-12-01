// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTClient.Enums;
using System.IO.Ports;
using System.Net;

namespace GWModbusStandard.STD;

/// <summary>
/// 连接参数类
/// 不同驱动连接参数不一样，可以根据实际情况去定义与设备或者系统的连接属性。
/// </summary>
public class ConnectionConfig
{
    /// <summary>
    /// IP地址:端口
    /// 或者COM端口号
    /// </summary>
    public string ServerUrl { get; set; }

    /// <summary>
    /// 采集频次
    /// </summary>
    public int SleepInterval { get; set; } = 5000;


    /// <summary>
    /// Modbus类型
    /// </summary>
    public ModbusType ModbusType { get; set; } = ModbusType.Tcp;

    /// <summary>
    /// 发送和接收的超时时间
    /// 毫秒
    /// </summary>
    public int TimeOut { get; set; } = 3000;

    /// <summary>
    /// ABCD
    /// BADC
    /// CDAB
    /// DCBA
    /// </summary>
    public EndianFormat EndianFormat { get; set; } = EndianFormat.ABCD;

    /// <summary>
    /// 波特率
    /// </summary>
    public int BaudRate { get; set; } = 9600;

    /// <summary>
    /// 数据位
    /// </summary>
    public int DataBits { get; set; } = 8;

    /// <summary>
    /// 停止位
    /// </summary>
    public StopBits StopBits { get; set; } = StopBits.None;

    /// <summary>
    /// 奇偶校验
    /// </summary>
    public Parity Parity { get; set; } = Parity.None;

    public IPEndPoint ServerIpAndPoint
    {
        get
        {
            // Split the ServerUrl by ':'
            var parts = ServerUrl.Split(':');

            // Check if parts array contains at least 2 elements (IP and port)
            if (parts.Length != 2)
            {
                throw new FormatException("ServerUrl must be in the format 'IP:Port'.");
            }

            // Extract IP address and port
            var ipAddress = parts[0];
            var port = int.Parse(parts[1]);

            // Create and return an IPEndPoint object
            return new IPEndPoint(IPAddress.Parse(ipAddress), port);
        }
    }
}
