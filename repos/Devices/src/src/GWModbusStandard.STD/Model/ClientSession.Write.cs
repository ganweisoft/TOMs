// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTClient;
using IoTClient.Enums;
using IoTClient.Models;

namespace GWModbusStandard.STD;

public partial class ClientSession
{
    /// <summary>
    /// 写入值
    /// </summary>
    /// <param name="stationNumber"></param>
    /// <param name="func"></param>
    /// <param name="addr"></param>
    /// <param name="dataTypeEnum"></param>
    /// <param name="originValue"></param>
    /// <returns></returns>
    public async Task<Result> WriteAddressValues(ModbusInput modbusInput)
    {
        var stationNumber = modbusInput.StationNumber;
        var func = modbusInput.FunctionCode;
        var addr = modbusInput.Address;
        var originValue = modbusInput.Value.ToString();
        Result writeResult;
        switch (modbusInput.DataType)
        {
            case DataTypeEnum.Bool:
                writeResult = await Session.Write(addr, bool.Parse(originValue), stationNumber, func);
                break;
            case DataTypeEnum.Double:
                writeResult = await Session.Write(addr, double.Parse(originValue), stationNumber, func);
                break;
            case DataTypeEnum.Float:
                writeResult = await Session.Write(addr, float.Parse(originValue), stationNumber, func);
                break;
            case DataTypeEnum.Int16:
                writeResult = await Session.Write(addr, short.Parse(originValue), stationNumber, func);
                break;
            case DataTypeEnum.Int32:
                writeResult = await Session.Write(addr, int.Parse(originValue), stationNumber, func);
                break;
            case DataTypeEnum.Int64:
                writeResult = await Session.Write(addr, long.Parse(originValue), stationNumber, func);
                break;
            case DataTypeEnum.UInt16:
                writeResult = await Session.Write(addr, ushort.Parse(originValue), stationNumber, func);
                break;
            case DataTypeEnum.UInt32:
                writeResult = await Session.Write(addr, uint.Parse(originValue), stationNumber, func);
                break;
            case DataTypeEnum.UInt64:
                writeResult = await Session.Write(addr, ulong.Parse(originValue), stationNumber, func);
                break;
            default:
                var bytes = System.Text.Encoding.Default.GetBytes(originValue);
                writeResult = await Session.Write(addr, bytes, stationNumber, func);
                break;
        }
        return writeResult;
    }

    /// <summary>
    /// 写入值
    /// </summary>
    /// <param name="stationNumber"></param>
    /// <param name="func"></param>
    /// <param name="addr"></param>
    /// <param name="dataTypeEnum"></param>
    /// <param name="originValue"></param>
    /// <returns></returns>
    public async Task<Result> ReadAddressValues(ModbusInput modbusInput)
    {
        var stationNumber = modbusInput.StationNumber;
        var func = modbusInput.FunctionCode;
        var addr = modbusInput.Address;
        Result writeResult;
        switch (modbusInput.DataType)
        {
            case DataTypeEnum.Bool:
                writeResult = await Session.ReadCoil(addr, stationNumber, func);
                break;
            case DataTypeEnum.Double:
                writeResult = await Session.ReadDouble(addr, stationNumber, func);
                break;
            case DataTypeEnum.Float:
                writeResult = await Session.ReadFloat(addr, stationNumber, func);
                break;
            case DataTypeEnum.Int16:
                writeResult = await Session.ReadInt16(addr, stationNumber, func);
                break;
            case DataTypeEnum.Int32:
                writeResult = await Session.ReadInt32(addr, stationNumber, func);
                break;
            case DataTypeEnum.Int64:
                writeResult = await Session.ReadInt64(addr, stationNumber, func);
                break;
            case DataTypeEnum.UInt16:
                writeResult = await Session.ReadUInt16(addr, stationNumber, func);
                break;
            case DataTypeEnum.UInt32:
                writeResult = await Session.ReadUInt32(addr, stationNumber, func);
                break;
            case DataTypeEnum.UInt64:
                writeResult = await Session.ReadUInt64(addr, stationNumber, func);
                break;
            case DataTypeEnum.Int16Bit:
                writeResult = await Session.ReadInt16Bit(addr, stationNumber, func);
                break;
            case DataTypeEnum.UInt16Bit:
                writeResult = await Session.ReadUInt16Bit(addr, stationNumber, func);
                break;
            default:
                writeResult = await Session.ReadInt32(addr, stationNumber, func);
                break;
        }
        return writeResult;
    }
}
