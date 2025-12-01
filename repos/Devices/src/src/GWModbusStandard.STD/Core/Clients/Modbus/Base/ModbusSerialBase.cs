// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTClient.Common.Helpers;
using IoTClient.Enums;
using IoTClient.Models;
using System.IO.Ports;
using System.Text;

namespace IoTClient.Clients.Modbus
{
    public abstract class ModbusSerialBase : SerialPortBase, IModbusClient
    {
        protected EndianFormat format;
        private bool plcAddresses;

        /// <summary>
        /// 是否是连接的
        /// </summary>
        public bool Connected => serialPort?.IsOpen ?? false;

        /// <summary>
        /// 警告日志委托        
        /// </summary>
        public LoggerDelegate WarningLog { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="portName">COM端口名称</param>
        /// <param name="baudRate">波特率</param>
        /// <param name="dataBits">数据位</param>
        /// <param name="stopBits">停止位</param>
        /// <param name="parity">奇偶校验</param>
        /// <param name="timeout">超时时间（毫秒）</param>
        /// <param name="format">大小端设置</param>
        /// <param name="plcAddresses">PLC地址</param>
        public ModbusSerialBase(string portName, int baudRate, int dataBits, StopBits stopBits, Parity parity, int timeout = 1500, EndianFormat format = EndianFormat.ABCD, bool plcAddresses = false)
        {
            if (serialPort == null) serialPort = new SerialPort();
            serialPort.PortName = portName;
            serialPort.BaudRate = baudRate;
            serialPort.DataBits = dataBits;
            serialPort.StopBits = stopBits;
            serialPort.Encoding = Encoding.ASCII;
            serialPort.Parity = parity;

            serialPort.ReadTimeout = timeout;
            serialPort.WriteTimeout = timeout;

            this.format = format;
            this.plcAddresses = plcAddresses;
        }

        /// <summary>
        /// 检查连接状态
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void EnsureConnected()
        {
            if (!Connected)
            {
                var connectResult = Connect();
                if (!connectResult.IsSucceed)
                {
                    throw new InvalidOperationException("无法建立连接：" + connectResult.Err);
                }
            }
        }

        #region 发送报文，并获取响应报文
        /// <summary>
        /// 发送报文，并获取响应报文
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        public async Task<Result<byte[]>> SendPackageReliable(byte[] command)
        {
            async Task<Result<byte[]>> _sendPackage()
            {
                //从发送命令到读取响应为最小单元，避免多线程执行串数据（可线程安全执行）
                //lock (this)
                await _semaphoreSlim.WaitAsync();
                try
                {
                    //发送命令
                    serialPort.Write(command, 0, command.Length);
                    //获取响应报文
                    return await SerialPortRead(serialPort);
                }
                finally
                {
                    _semaphoreSlim.Release();
                }
            }

            try
            {
                var result = await _sendPackage();
                if (!result.IsSucceed)
                {
                    WarningLog?.Invoke(result.Err, result.Exception);
                    //如果出现异常，则进行一次重试         
                    var conentResult = Connect();
                    if (!conentResult.IsSucceed)
                        return new Result<byte[]>(conentResult);

                    return await _sendPackage();
                }
                else
                    return result;
            }
            catch (Exception ex)
            {
                WarningLog?.Invoke(ex.Message, ex);
                //如果出现异常，则进行一次重试
                //重新打开连接
                var conentResult = Connect();
                if (!conentResult.IsSucceed)
                    return new Result<byte[]>(conentResult);

                return await _sendPackage();
            }
        }
        #endregion

        #region  Read 读取
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <param name="readLength">读取长度</param>
        /// <returns></returns>
        public abstract Task<Result<byte[]>> Read(string address, byte stationNumber = 1, byte functionCode = 3, ushort readLength = 1, bool byteFormatting = true);

        /// <summary>
        /// 读取Int16
        /// </summary>
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <returns></returns>
        public async Task<Result<short>> ReadInt16(string address, byte stationNumber = 1, byte functionCode = 3)
        {
            var readResut = await Read(address, stationNumber, functionCode);
            var result = new Result<short>(readResut);
            if (result.IsSucceed)
                result.Value = BitConverter.ToInt16(readResut.Value, 0);
            return result.EndTime();
        }
        /// <summary>
        /// 按位的方式读取
        /// </summary>
        /// <param name="address">寄存器地址:如1:00 ... 1:14、1:15或者1:00-1:12</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <param name="left">按位取值从左边开始取</param>
        /// <returns></returns>
        public async Task<Result<short>> ReadInt16Bit(string address, byte stationNumber = 1, byte functionCode = 3, bool left = true)
        {
            string[] adds = address.Split(':', '-');
            var readResut = await Read(adds[0].Trim(), stationNumber, functionCode);
            var result = new Result<short>(readResut);
            if (result.IsSucceed)
            {
                result.Value = BitConverter.ToInt16(readResut.Value, 0);
                if (adds.Length >= 2)
                {
                    var startBit = int.Parse(adds[1].Trim());
                    int endBit = startBit; // 默认为单个位读取

                    //1.00-1.12 拆分成数组后为[1,00,1,12]
                    if (adds.Length == 4) endBit = int.Parse(adds[3].Trim());

                    var binaryArray = DataConvert.IntToBinaryArray(result.Value, 16).Select(c => c - '0').ToArray();
                    if (left)
                    {
                        var length = binaryArray.Length - 16;

                        var selectedBits = binaryArray.Skip(length + startBit).Take(endBit - startBit + 1).Reverse().ToArray();
                        result.Value = (short)selectedBits.Select((bit, index) => bit << index).Sum();
                    }
                    else
                    {
                        var selectedBits = binaryArray.Skip(binaryArray.Length - 1 - endBit).Take(endBit - startBit + 1).ToArray();
                        result.Value = (short)selectedBits.Select((bit, index) => bit << index).Sum();
                    }
                }
            }
            return result.EndTime();
        }

        /// <summary>
        /// 读取UInt16
        /// </summary>
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <returns></returns>
        public async Task<Result<ushort>> ReadUInt16(string address, byte stationNumber = 1, byte functionCode = 3)
        {
            var readResut = await Read(address, stationNumber, functionCode);
            var result = new Result<ushort>(readResut);
            if (result.IsSucceed)
                result.Value = BitConverter.ToUInt16(readResut.Value, 0);
            return result.EndTime();
        }

        /// <summary>
        /// 按位的方式读取
        /// </summary>
        /// <param name="address">寄存器地址:如1:00 ... 1:14、1:15或者1:00-1:12</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <param name="left">按位取值从左边开始取</param>
        /// <returns></returns>
        public async Task<Result<ushort>> ReadUInt16Bit(string address, byte stationNumber = 1, byte functionCode = 3, bool left = true)
        {
            string[] adds = address.Split(':', '-');
            var readResut = await Read(adds[0].Trim(), stationNumber, functionCode);
            var result = new Result<ushort>(readResut);
            if (result.IsSucceed)
            {
                result.Value = BitConverter.ToUInt16(readResut.Value, 0);
                if (adds.Length >= 2)
                {
                    var startBit = int.Parse(adds[1].Trim());
                    int endBit = startBit; // 默认为单个位读取

                    //1.00-1.12 拆分成数组后为[1,00,1,12]
                    if (adds.Length == 4) endBit = int.Parse(adds[3].Trim());

                    var binaryArray = DataConvert.IntToBinaryArray(result.Value, 16).Select(c => c - '0').ToArray();
                    if (left)
                    {
                        var length = binaryArray.Length - 16;
                        var selectedBits = binaryArray.Skip(length + startBit).Take(endBit - startBit + 1).Reverse().ToArray();
                        result.Value = (ushort)selectedBits.Select((bit, index) => bit << index).Sum();
                    }
                    else
                    {
                        var selectedBits = binaryArray.Skip(binaryArray.Length - 1 - endBit).Take(endBit - startBit + 1).ToArray();
                        result.Value = (ushort)selectedBits.Select((bit, index) => bit << index).Sum();
                    }
                }
            }
            return result.EndTime();
        }
        /// <summary>
        /// 读取Int32
        /// </summary>
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <returns></returns>
        public async Task<Result<int>> ReadInt32(string address, byte stationNumber = 1, byte functionCode = 3)
        {
            var readResut = await Read(address, stationNumber, functionCode, readLength: 2);
            var result = new Result<int>(readResut);
            if (result.IsSucceed)
                result.Value = BitConverter.ToInt32(readResut.Value, 0);
            return result.EndTime();
        }

        /// <summary>
        /// 读取UInt32
        /// </summary>
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <returns></returns>
        public async Task<Result<uint>> ReadUInt32(string address, byte stationNumber = 1, byte functionCode = 3)
        {
            var readResut = await Read(address, stationNumber, functionCode, readLength: 2);
            var result = new Result<uint>(readResut);
            if (result.IsSucceed)
                result.Value = BitConverter.ToUInt32(readResut.Value, 0);
            return result.EndTime();
        }

        /// <summary>
        /// 读取Int64
        /// </summary>
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <returns></returns>
        public async Task<Result<long>> ReadInt64(string address, byte stationNumber = 1, byte functionCode = 3)
        {
            var readResut = await Read(address, stationNumber, functionCode, readLength: 4);
            var result = new Result<long>(readResut);
            if (result.IsSucceed)
                result.Value = BitConverter.ToInt64(readResut.Value, 0);
            return result.EndTime();
        }

        /// <summary>
        /// 读取UInt64
        /// </summary>
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <returns></returns>
        public async Task<Result<ulong>> ReadUInt64(string address, byte stationNumber = 1, byte functionCode = 3)
        {
            var readResut = await Read(address, stationNumber, functionCode, readLength: 4);
            var result = new Result<ulong>(readResut);
            if (result.IsSucceed)
                result.Value = BitConverter.ToUInt64(readResut.Value, 0);
            return result.EndTime();
        }

        /// <summary>
        /// 读取Float
        /// </summary>
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <returns></returns>
        public async Task<Result<float>> ReadFloat(string address, byte stationNumber = 1, byte functionCode = 3)
        {
            var readResut = await Read(address, stationNumber, functionCode, readLength: 2);
            var result = new Result<float>(readResut);
            if (result.IsSucceed)
                result.Value = BitConverter.ToSingle(readResut.Value, 0);
            return result.EndTime();
        }

        /// <summary>
        /// 读取Double
        /// </summary>
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <returns></returns>
        public async Task<Result<double>> ReadDouble(string address, byte stationNumber = 1, byte functionCode = 3)
        {
            var readResut = await Read(address, stationNumber, functionCode, readLength: 4);
            var result = new Result<double>(readResut);
            if (result.IsSucceed)
                result.Value = BitConverter.ToDouble(readResut.Value, 0);
            return result.EndTime();
        }

        /// <summary>
        /// 读取线圈
        /// </summary>
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <returns></returns>
        public async Task<Result<bool>> ReadCoil(string address, byte stationNumber = 1, byte functionCode = 1)
        {
            var readResut = await Read(address, stationNumber, functionCode);
            var result = new Result<bool>(readResut);
            if (result.IsSucceed)
                result.Value = BitConverter.ToBoolean(readResut.Value, 0);
            return result.EndTime();
        }

        /// <summary>
        /// 读取离散
        /// </summary>
        /// <param name="address"></param>
        /// <param name="stationNumber"></param>
        /// <param name="functionCode"></param>
        /// <returns></returns>
        public async Task<Result<bool>> ReadDiscrete(string address, byte stationNumber = 1, byte functionCode = 2)
        {
            var readResut = await Read(address, stationNumber, functionCode);
            var result = new Result<bool>(readResut);
            if (result.IsSucceed)
                result.Value = BitConverter.ToBoolean(readResut.Value, 0);
            return result.EndTime();
        }

        /// <summary>
        /// 从批量读取的数据字节提取对应的地址数据
        /// </summary>
        /// <param name="beginAddress">批量读取的起始地址</param>
        /// <param name="address">读取地址</param>
        /// <param name="values">批量读取的值</param>
        /// <returns></returns>
        public Result<short> ReadInt16(string beginAddress, string address, byte[] values)
        {
            if (!int.TryParse(address?.Trim(), out int addressInt) || !int.TryParse(beginAddress?.Trim(), out int beginAddressInt))
                throw new Exception($"只能是数字，参数address：{address}  beginAddress：{beginAddress}");
            try
            {
                var interval = addressInt - beginAddressInt;
                var byteArry = values.Skip(interval * 2).Take(2).Reverse().ToArray();
                return new Result<short>
                {
                    Value = BitConverter.ToInt16(byteArry, 0)
                };
            }
            catch (Exception ex)
            {
                return new Result<short>
                {
                    IsSucceed = false,
                    Err = ex.Message
                };
            }
        }

        public Result<short> ReadInt16(int beginAddress, int address, byte[] values)
        {
            return ReadInt16(beginAddress.ToString(), address.ToString(), values);
        }

        /// <summary>
        /// 从批量读取的数据字节提取对应的地址数据
        /// </summary>
        /// <param name="beginAddress">批量读取的起始地址</param>
        /// <param name="address">读取地址</param>
        /// <param name="values">批量读取的值</param>
        /// <returns></returns>
        public Result<ushort> ReadUInt16(string beginAddress, string address, byte[] values)
        {
            if (!int.TryParse(address?.Trim(), out int addressInt) || !int.TryParse(beginAddress?.Trim(), out int beginAddressInt))
                throw new Exception($"只能是数字，参数address：{address}  beginAddress：{beginAddress}");
            try
            {
                var interval = addressInt - beginAddressInt;
                var byteArry = values.Skip(interval * 2).Take(2).Reverse().ToArray();
                return new Result<ushort>
                {
                    Value = BitConverter.ToUInt16(byteArry, 0)
                };
            }
            catch (Exception ex)
            {
                return new Result<ushort>
                {
                    IsSucceed = false,
                    Err = ex.Message
                };
            }
        }

        public Result<ushort> ReadUInt16(int beginAddress, int address, byte[] values)
        {
            return ReadUInt16(beginAddress.ToString(), address.ToString(), values);
        }

        /// <summary>
        /// 从批量读取的数据字节提取对应的地址数据
        /// </summary>
        /// <param name="beginAddress">批量读取的起始地址</param>
        /// <param name="address">读取地址</param>
        /// <param name="values">批量读取的值</param>
        /// <returns></returns>
        public Result<int> ReadInt32(string beginAddress, string address, byte[] values)
        {
            if (!int.TryParse(address?.Trim(), out int addressInt) || !int.TryParse(beginAddress?.Trim(), out int beginAddressInt))
                throw new Exception($"只能是数字，参数address：{address}  beginAddress：{beginAddress}");
            try
            {
                var interval = (addressInt - beginAddressInt) / 2;
                var offset = (addressInt - beginAddressInt) % 2 * 2;//取余 乘以2（每个地址16位，占两个字节）
                var byteArry = values.Skip(interval * 2 * 2 + offset).Take(2 * 2).Reverse().ToArray().ByteFormatting(format);
                return new Result<int>
                {
                    Value = BitConverter.ToInt32(byteArry, 0)
                };
            }
            catch (Exception ex)
            {
                return new Result<int>
                {
                    IsSucceed = false,
                    Err = ex.Message
                };
            }
        }

        public Result<int> ReadInt32(int beginAddress, int address, byte[] values)
        {
            return ReadInt32(beginAddress.ToString(), address.ToString(), values);
        }

        /// <summary>
        /// 从批量读取的数据字节提取对应的地址数据
        /// </summary>
        /// <param name="beginAddress">批量读取的起始地址</param>
        /// <param name="address">读取地址</param>
        /// <param name="values">批量读取的值</param>
        /// <returns></returns>
        public Result<uint> ReadUInt32(string beginAddress, string address, byte[] values)
        {
            if (!int.TryParse(address?.Trim(), out int addressInt) || !int.TryParse(beginAddress?.Trim(), out int beginAddressInt))
                throw new Exception($"只能是数字，参数address：{address}  beginAddress：{beginAddress}");
            try
            {
                var interval = (addressInt - beginAddressInt) / 2;
                var offset = (addressInt - beginAddressInt) % 2 * 2;//取余 乘以2（每个地址16位，占两个字节）
                var byteArry = values.Skip(interval * 2 * 2 + offset).Take(2 * 2).Reverse().ToArray().ByteFormatting(format);
                return new Result<uint>
                {
                    Value = BitConverter.ToUInt32(byteArry, 0)
                };
            }
            catch (Exception ex)
            {
                return new Result<uint>
                {
                    IsSucceed = false,
                    Err = ex.Message
                };
            }
        }

        public Result<uint> ReadUInt32(int beginAddress, int address, byte[] values)
        {
            return ReadUInt32(beginAddress.ToString(), address.ToString(), values);
        }

        /// <summary>
        /// 从批量读取的数据字节提取对应的地址数据
        /// </summary>
        /// <param name="beginAddress">批量读取的起始地址</param>
        /// <param name="address">读取地址</param>
        /// <param name="values">批量读取的值</param>
        /// <returns></returns>
        public Result<long> ReadInt64(string beginAddress, string address, byte[] values)
        {
            if (!int.TryParse(address?.Trim(), out int addressInt) || !int.TryParse(beginAddress?.Trim(), out int beginAddressInt))
                throw new Exception($"只能是数字，参数address：{address}  beginAddress：{beginAddress}");
            try
            {
                var interval = (addressInt - beginAddressInt) / 4;
                var offset = (addressInt - beginAddressInt) % 4 * 2;//取余 乘以2（每个地址16位，占两个字节）
                var byteArry = values.Skip(interval * 2 * 4 + offset).Take(2 * 4).Reverse().ToArray().ByteFormatting(format);
                return new Result<long>
                {
                    Value = BitConverter.ToInt64(byteArry, 0)
                };
            }
            catch (Exception ex)
            {
                return new Result<long>
                {
                    IsSucceed = false,
                    Err = ex.Message
                };
            }
        }

        public Result<long> ReadInt64(int beginAddress, int address, byte[] values)
        {
            return ReadInt64(beginAddress.ToString(), address.ToString(), values);
        }

        /// <summary>
        /// 从批量读取的数据字节提取对应的地址数据
        /// </summary>
        /// <param name="beginAddress">批量读取的起始地址</param>
        /// <param name="address">读取地址</param>
        /// <param name="values">批量读取的值</param>
        /// <returns></returns>
        public Result<ulong> ReadUInt64(string beginAddress, string address, byte[] values)
        {
            if (!int.TryParse(address?.Trim(), out int addressInt) || !int.TryParse(beginAddress?.Trim(), out int beginAddressInt))
                throw new Exception($"只能是数字，参数address：{address}  beginAddress：{beginAddress}");
            try
            {
                var interval = (addressInt - beginAddressInt) / 4;
                var offset = (addressInt - beginAddressInt) % 4 * 2;//取余 乘以2（每个地址16位，占两个字节）
                var byteArry = values.Skip(interval * 2 * 4 + offset).Take(2 * 4).Reverse().ToArray().ByteFormatting(format);
                return new Result<ulong>
                {
                    Value = BitConverter.ToUInt64(byteArry, 0)
                };
            }
            catch (Exception ex)
            {
                return new Result<ulong>
                {
                    IsSucceed = false,
                    Err = ex.Message
                };
            }
        }

        public Result<ulong> ReadUInt64(int beginAddress, int address, byte[] values)
        {
            return ReadUInt64(beginAddress.ToString(), address.ToString(), values);
        }

        /// <summary>
        /// 从批量读取的数据字节提取对应的地址数据
        /// </summary>
        /// <param name="beginAddress">批量读取的起始地址</param>
        /// <param name="address">读取地址</param>
        /// <param name="values">批量读取的值</param>
        /// <returns></returns>
        public Result<float> ReadFloat(string beginAddress, string address, byte[] values)
        {
            if (!int.TryParse(address?.Trim(), out int addressInt) || !int.TryParse(beginAddress?.Trim(), out int beginAddressInt))
                throw new Exception($"只能是数字，参数address：{address}  beginAddress：{beginAddress}");
            try
            {
                var interval = (addressInt - beginAddressInt) / 2;
                var offset = (addressInt - beginAddressInt) % 2 * 2;//取余 乘以2（每个地址16位，占两个字节）
                var byteArry = values.Skip(interval * 2 * 2 + offset).Take(2 * 2).Reverse().ToArray().ByteFormatting(format);
                return new Result<float>
                {
                    Value = BitConverter.ToSingle(byteArry, 0)
                };
            }
            catch (Exception ex)
            {
                return new Result<float>
                {
                    IsSucceed = false,
                    Err = ex.Message
                };
            }
        }

        public Result<float> ReadFloat(int beginAddress, int address, byte[] values)
        {
            return ReadFloat(beginAddress.ToString(), address.ToString(), values);
        }

        /// <summary>
        /// 从批量读取的数据字节提取对应的地址数据
        /// </summary>
        /// <param name="beginAddress">批量读取的起始地址</param>
        /// <param name="address">读取地址</param>
        /// <param name="values">批量读取的值</param>
        /// <returns></returns>
        public Result<double> ReadDouble(string beginAddress, string address, byte[] values)
        {
            if (!int.TryParse(address?.Trim(), out int addressInt) || !int.TryParse(beginAddress?.Trim(), out int beginAddressInt))
                throw new Exception($"只能是数字，参数address：{address}  beginAddress：{beginAddress}");
            try
            {
                var interval = (addressInt - beginAddressInt) / 4;
                var offset = (addressInt - beginAddressInt) % 4 * 2;//取余 乘以2（每个地址16位，占两个字节）
                var byteArry = values.Skip(interval * 2 * 4 + offset).Take(2 * 4).Reverse().ToArray().ByteFormatting(format);
                return new Result<double>
                {
                    Value = BitConverter.ToDouble(byteArry, 0)
                };
            }
            catch (Exception ex)
            {
                return new Result<double>
                {
                    IsSucceed = false,
                    Err = ex.Message
                };
            }
        }

        public Result<double> ReadDouble(int beginAddress, int address, byte[] values)
        {
            return ReadDouble(beginAddress.ToString(), address.ToString(), values);
        }

        /// <summary>
        /// 从批量读取的数据字节提取对应的地址数据
        /// </summary>
        /// <param name="beginAddress">批量读取的起始地址</param>
        /// <param name="address">读取地址</param>
        /// <param name="values">批量读取的值</param>
        /// <returns></returns>
        public Result<bool> ReadCoil(string beginAddress, string address, byte[] values)
        {
            if (!int.TryParse(address?.Trim(), out int addressInt) || !int.TryParse(beginAddress?.Trim(), out int beginAddressInt))
                throw new Exception($"只能是数字，参数address：{address}  beginAddress：{beginAddress}");
            try
            {
                var interval = addressInt - beginAddressInt;
                var index = (interval + 1) % 8 == 0 ? (interval + 1) / 8 : (interval + 1) / 8 + 1;
                var binaryArray = Convert.ToInt32(values[index - 1]).IntToBinaryArray().ToArray().Reverse().ToArray();
                var isBit = false;
                if ((index - 1) * 8 + binaryArray.Length > interval)
                    isBit = binaryArray[interval - (index - 1) * 8].ToString() == 1.ToString();
                return new Result<bool>()
                {
                    Value = isBit
                };
            }
            catch (Exception ex)
            {
                return new Result<bool>
                {
                    IsSucceed = false,
                    Err = ex.Message
                };
            }
        }

        public Result<bool> ReadCoil(int beginAddress, int address, byte[] values)
        {
            return ReadCoil(beginAddress.ToString(), address.ToString(), values);
        }


        public Result<short> ReadInt16Bit(int beginAddress, string address, byte[] values, bool left = true)
        {
            var result = ReadInt16(beginAddress.ToString(), address.Split(':')[0], values);
            string[] adds = address.Split(':', '-');
            if (adds.Length >= 2)
            {
                var startBit = int.Parse(adds[1].Trim());
                int endBit = startBit; // 默认为单个位读取

                //1.00-1.12 拆分成数组后为[1,00,1,12]
                if (adds.Length == 4) endBit = int.Parse(adds[3].Trim());

                var binaryArray = DataConvert.IntToBinaryArray(result.Value, 16).Select(c => c - '0').ToArray();
                if (left)
                {
                    var length = binaryArray.Length - 16;

                    var selectedBits = binaryArray.Skip(length + startBit).Take(endBit - startBit + 1).Reverse().ToArray();
                    result.Value = (short)selectedBits.Select((bit, index) => bit << index).Sum();
                }
                else
                {
                    var selectedBits = binaryArray.Skip(binaryArray.Length - 1 - endBit).Take(endBit - startBit + 1).ToArray();
                    result.Value = (short)selectedBits.Select((bit, index) => bit << index).Sum();
                }
            }
            return result.EndTime();
        }
        public Result<ushort> ReadUInt16Bit(int beginAddress, string address, byte[] values, bool left = true)
        {
            var result = ReadUInt16(beginAddress.ToString(), address.Split(':')[0], values);
            string[] adds = address.Split(':', '-');
            if (adds.Length >= 2)
            {
                var startBit = int.Parse(adds[1].Trim());
                int endBit = startBit; // 默认为单个位读取

                //1.00-1.12 拆分成数组后为[1,00,1,12]
                if (adds.Length == 4) endBit = int.Parse(adds[3].Trim());

                var binaryArray = DataConvert.IntToBinaryArray(result.Value, 16).Select(c => c - '0').ToArray();
                if (left)
                {
                    var length = binaryArray.Length - 16;

                    var selectedBits = binaryArray.Skip(length + startBit).Take(endBit - startBit + 1).Reverse().ToArray();
                    result.Value = (ushort)selectedBits.Select((bit, index) => bit << index).Sum();
                }
                else
                {
                    var selectedBits = binaryArray.Skip(binaryArray.Length - 1 - endBit).Take(endBit - startBit + 1).ToArray();
                    result.Value = (ushort)selectedBits.Select((bit, index) => bit << index).Sum();
                }
            }
            return result.EndTime();
        }

        /// <summary>
        /// 从批量读取的数据字节提取对应的地址数据
        /// </summary>
        /// <param name="beginAddress">批量读取的起始地址</param>
        /// <param name="address">读取地址</param>
        /// <param name="values">批量读取的值</param>
        /// <returns></returns>
        public Result<bool> ReadDiscrete(string beginAddress, string address, byte[] values)
        {
            if (!int.TryParse(address?.Trim(), out int addressInt) || !int.TryParse(beginAddress?.Trim(), out int beginAddressInt))
                throw new Exception($"只能是数字，参数address：{address}  beginAddress：{beginAddress}");
            try
            {
                var interval = addressInt - beginAddressInt;
                var index = (interval + 1) % 8 == 0 ? (interval + 1) / 8 : (interval + 1) / 8 + 1;
                var binaryArray = Convert.ToInt32(values[index - 1]).IntToBinaryArray().ToArray().Reverse().ToArray();
                var isBit = false;
                if ((index - 1) * 8 + binaryArray.Length > interval)
                    isBit = binaryArray[interval - (index - 1) * 8].ToString() == 1.ToString();
                return new Result<bool>()
                {
                    Value = isBit
                };
            }
            catch (Exception ex)
            {
                return new Result<bool>
                {
                    IsSucceed = false,
                    Err = ex.Message
                };
            }
        }

        public Result<bool> ReadDiscrete(int beginAddress, int address, byte[] values)
        {
            return ReadDiscrete(beginAddress.ToString(), address.ToString(), values);
        }

        /// <summary>
        /// 分批读取（批量读取，内部进行批量计算读取）
        /// </summary>
        /// <param name="addresses"></param>
        /// <returns></returns>
        private async Task<Result<List<ModbusOutput>>> BatchRead(List<ModbusInput> addresses)
        {
            var result = new Result<List<ModbusOutput>>();
            result.Value = new List<ModbusOutput>();
            var functionCodes = addresses.Select(t => t.FunctionCode).Distinct();
            foreach (var functionCode in functionCodes)
            {
                var stationNumbers = addresses.Where(t => t.FunctionCode == functionCode).Select(t => t.StationNumber).Distinct();
                foreach (var stationNumber in stationNumbers)
                {
                    var addressList = addresses.Where(t => t.FunctionCode == functionCode && t.StationNumber == stationNumber)
                        .DistinctCustomBy(t => t.Address)
                        .ToDictionary(t => t.Address, t => t.DataType);
                    var tempResult = await BatchRead(addressList, stationNumber, functionCode);
                    if (tempResult.IsSucceed)
                    {
                        foreach (var item in tempResult.Value)
                        {
                            result.Value.Add(new ModbusOutput()
                            {
                                Address = item.Key,
                                FunctionCode = functionCode,
                                StationNumber = stationNumber,
                                Value = item.Value
                            });
                        }
                    }
                    else
                    {
                        result.SetErrInfo(tempResult);
                    }
                    result.Requst = tempResult.Requst;
                    result.Response = tempResult.Response;
                }
            }
            return result.EndTime();
        }

        private async Task<Result<Dictionary<string, object>>> BatchRead(Dictionary<string, DataTypeEnum> addressList, byte stationNumber, byte functionCode)
        {
            var result = new Result<Dictionary<string, object>>();
            result.Value = new Dictionary<string, object>();

            var addresses = addressList.Select(t => new KeyValuePair<string, DataTypeEnum>(t.Key, t.Value)).ToList();

            var minAddress = addresses.Select(t => int.Parse(t.Key.Split(':')[0])).Min();
            var maxAddress = addresses.Select(t => int.Parse(t.Key.Split(':')[0])).Max();
            while (maxAddress >= minAddress)
            {
                int readLength = 121;//125 - 4 = 121

                var tempAddress = addresses.Where(t => int.Parse(t.Key.Split(':')[0]) >= minAddress && int.Parse(t.Key.Split(':')[0]) <= minAddress + readLength).ToList();
                //如果范围内没有数据。按正确逻辑不存在这种情况。
                if (!tempAddress.Any())
                {
                    minAddress = minAddress + readLength;
                    continue;
                }

                var tempMax = tempAddress.OrderByDescending(t => int.Parse(t.Key.Split(':')[0])).FirstOrDefault();
                switch (tempMax.Value)
                {
                    case DataTypeEnum.Bool:
                    case DataTypeEnum.Byte:
                    case DataTypeEnum.Int16:
                    case DataTypeEnum.UInt16:
                    case DataTypeEnum.Int16Bit:
                    case DataTypeEnum.UInt16Bit:
                        readLength = int.Parse(tempMax.Key.Split(':')[0]) + 1 - minAddress;
                        break;
                    case DataTypeEnum.Int32:
                    case DataTypeEnum.UInt32:
                    case DataTypeEnum.Float:
                        readLength = int.Parse(tempMax.Key.Split(':')[0]) + 2 - minAddress;
                        break;
                    case DataTypeEnum.Int64:
                    case DataTypeEnum.UInt64:
                    case DataTypeEnum.Double:
                        readLength = int.Parse(tempMax.Key.Split(':')[0]) + 4 - minAddress;
                        break;
                    default:
                        throw new Exception($"Err BatchRead 未定义类型：{tempMax.Value.GetHashCode()}");
                }

                var tempResult = await Read(minAddress.ToString(), stationNumber, functionCode, Convert.ToUInt16(readLength), false);

                result.Requst = tempResult.Requst;
                result.Response = tempResult.Response;
                if (!tempResult.IsSucceed)
                {
                    result.IsSucceed = tempResult.IsSucceed;
                    result.Exception = tempResult.Exception;
                    result.ErrCode = tempResult.ErrCode;
                    result.Err = $"读取 地址:{minAddress} 站号:{stationNumber} 功能码:{functionCode} 失败。{tempResult.Err}";
                    result.AddErr2List();
                    return result.EndTime();
                }

                var rValue = tempResult.Value.Reverse().ToArray();
                foreach (var item in tempAddress)
                {
                    object tempVaue = null;

                    switch (item.Value)
                    {
                        case DataTypeEnum.Bool:
                            tempVaue = ReadCoil(minAddress.ToString(), item.Key.Split(':')[0], rValue).Value;
                            break;
                        case DataTypeEnum.Byte:
                            throw new Exception("Err BatchRead 未定义类型：2");
                        case DataTypeEnum.Int16:
                            tempVaue = ReadInt16(minAddress.ToString(), item.Key.Split(':')[0], rValue).Value;
                            break;
                        case DataTypeEnum.Int16Bit:
                            tempVaue = ReadInt16Bit(minAddress, item.Key, rValue).Value;
                            break;
                        case DataTypeEnum.UInt16:
                            tempVaue = ReadUInt16(minAddress.ToString(), item.Key.Split(':')[0], rValue).Value;
                            break;
                        case DataTypeEnum.UInt16Bit:
                            tempVaue = ReadInt16Bit(minAddress, item.Key, rValue).Value;
                            break;
                        case DataTypeEnum.Int32:
                            tempVaue = ReadInt32(minAddress.ToString(), item.Key.Split(':')[0], rValue).Value;
                            break;
                        case DataTypeEnum.UInt32:
                            tempVaue = ReadUInt32(minAddress.ToString(), item.Key.Split(':')[0], rValue).Value;
                            break;
                        case DataTypeEnum.Int64:
                            tempVaue = ReadInt64(minAddress.ToString(), item.Key.Split(':')[0], rValue).Value;
                            break;
                        case DataTypeEnum.UInt64:
                            tempVaue = ReadUInt64(minAddress.ToString(), item.Key.Split(':')[0], rValue).Value;
                            break;
                        case DataTypeEnum.Float:
                            tempVaue = ReadFloat(minAddress.ToString(), item.Key.Split(':')[0], rValue).Value;
                            break;
                        case DataTypeEnum.Double:
                            tempVaue = ReadDouble(minAddress.ToString(), item.Key.Split(':')[0], rValue).Value;
                            break;
                        default:
                            throw new Exception($"Err BatchRead 未定义类型：{item.Value.GetHashCode()}");
                    }

                    result.Value.Add(item.Key, tempVaue);
                }
                minAddress = minAddress + readLength;

                if (addresses.Any(t => int.Parse(t.Key.Split(':')[0]) >= minAddress))
                    minAddress = int.Parse(addresses.Where(t => int.Parse(t.Key.Split(':')[0]) >= minAddress).OrderBy(t => t.Key).FirstOrDefault().Key.Split(':')[0]);
                else
                    return result.EndTime();
            }
            return result.EndTime();
        }

        /// <summary>
        /// 分批读取
        /// </summary>
        /// <param name="addresses"></param>
        /// <param name="retryCount">如果读取异常，重试次数</param>
        /// <returns></returns>
        public async Task<Result<List<ModbusOutput>>> BatchRead(List<ModbusInput> addresses, uint retryCount = 1)
        {
            var result = await BatchRead(addresses);
            for (int i = 0; i < retryCount; i++)
            {
                if (!result.IsSucceed)
                {
                    WarningLog?.Invoke(result.Err, result.Exception);
                    result = await BatchRead(addresses);
                }
                else
                    break;
            }
            return result;
        }
        #endregion

        #region Write 写入
        /// <summary>
        /// 线圈写入
        /// </summary>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <param name="stationNumber"></param>
        /// <param name="functionCode"></param>
        public abstract Task<Result> Write(string address, bool value, byte stationNumber = 1, byte functionCode = 5);

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="address"></param>
        /// <param name="values"></param>
        /// <param name="stationNumber"></param>
        /// <param name="functionCode"></param>
        /// <returns></returns>
        public abstract Task<Result> Write(string address, byte[] values, byte stationNumber = 1, byte functionCode = 16, bool byteFormatting = true);

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">写入的值</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        public async Task<Result> Write(string address, short value, byte stationNumber = 1, byte functionCode = 16)
        {
            var values = BitConverter.GetBytes(value).Reverse().ToArray();
            return await Write(address, values, stationNumber, functionCode);
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">写入的值</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        public async Task<Result> Write(string address, ushort value, byte stationNumber = 1, byte functionCode = 16)
        {
            var values = BitConverter.GetBytes(value).Reverse().ToArray();
            return await Write(address, values, stationNumber, functionCode);
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">写入的值</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        public async Task<Result> Write(string address, int value, byte stationNumber = 1, byte functionCode = 16)
        {
            var values = BitConverter.GetBytes(value).Reverse().ToArray();
            return await Write(address, values, stationNumber, functionCode);
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">写入的值</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        public async Task<Result> Write(string address, uint value, byte stationNumber = 1, byte functionCode = 16)
        {
            var values = BitConverter.GetBytes(value).Reverse().ToArray();
            return await Write(address, values, stationNumber, functionCode);
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">写入的值</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        public async Task<Result> Write(string address, long value, byte stationNumber = 1, byte functionCode = 16)
        {
            var values = BitConverter.GetBytes(value).Reverse().ToArray();
            return await Write(address, values, stationNumber, functionCode);
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">写入的值</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        public async Task<Result> Write(string address, ulong value, byte stationNumber = 1, byte functionCode = 16)
        {
            var values = BitConverter.GetBytes(value).Reverse().ToArray();
            return await Write(address, values, stationNumber, functionCode);
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">写入的值</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        public async Task<Result> Write(string address, float value, byte stationNumber = 1, byte functionCode = 16)
        {
            var values = BitConverter.GetBytes(value).Reverse().ToArray();
            return await Write(address, values, stationNumber, functionCode);
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">写入的值</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        public async Task<Result> Write(string address, double value, byte stationNumber = 1, byte functionCode = 16)
        {
            var values = BitConverter.GetBytes(value).Reverse().ToArray();
            return await Write(address, values, stationNumber, functionCode);
        }
        #endregion

        #region 获取命令

        /// <summary>
        /// 获取读取命令
        /// </summary>
        /// <param name="address">寄存器起始地址</param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <param name="length">读取长度</param>
        /// <returns></returns>
        public byte[] GetReadCommand(string address, byte stationNumber, byte functionCode, ushort length)
        {
            var readAddress = ushort.Parse(address?.Trim());
            if (plcAddresses) readAddress = (ushort)(Convert.ToUInt16(address?.Trim().Substring(1)) - 1);

            byte[] buffer = new byte[6];
            buffer[0] = stationNumber;  //站号
            buffer[1] = functionCode;   //功能码
            buffer[2] = BitConverter.GetBytes(readAddress)[1];
            buffer[3] = BitConverter.GetBytes(readAddress)[0];//寄存器地址
            buffer[4] = BitConverter.GetBytes(length)[1];
            buffer[5] = BitConverter.GetBytes(length)[0];//表示request 寄存器的长度(寄存器个数)
            return buffer;
        }

        /// <summary>
        /// 获取写入命令
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="values"></param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <returns></returns>
        public byte[] GetWriteCommand(string address, byte[] values, byte stationNumber, byte functionCode)
        {
            var writeAddress = ushort.Parse(address?.Trim());
            if (plcAddresses) writeAddress = (ushort)(Convert.ToUInt16(address?.Trim().Substring(1)) - 1);
            if (functionCode == 0x06)
            {
                // 写单个寄存器（固定为 6 字节的 Modbus RTU 请求）
                if (values.Length != 2)
                    throw new ArgumentException("功能码06只允许写入2个字节（一个寄存器）");

                byte[] buffer = new byte[6];
                buffer[0] = stationNumber;                               // 站号
                buffer[1] = functionCode;                                // 功能码06
                buffer[2] = BitConverter.GetBytes(writeAddress)[1];      // 地址高字节
                buffer[3] = BitConverter.GetBytes(writeAddress)[0];      // 地址低字节
                buffer[4] = values[0];                                   // 写入值高字节
                buffer[5] = values[1];                                   // 写入值低字节
                return buffer;
            }
            else
            {
                byte[] buffer = new byte[7 + values.Length];
                buffer[0] = stationNumber; //站号
                buffer[1] = functionCode;  //功能码
                buffer[2] = BitConverter.GetBytes(writeAddress)[1];
                buffer[3] = BitConverter.GetBytes(writeAddress)[0];//寄存器地址
                buffer[4] = (byte)(values.Length / 2 / 256);
                buffer[5] = (byte)(values.Length / 2 % 256);//写寄存器数量(除2是两个字节一个寄存器，寄存器16位。除以256是byte最大存储255。)              
                buffer[6] = (byte)(values.Length);          //写字节的个数
                values.CopyTo(buffer, 7);                   //把目标值附加到数组后面
                return buffer;
            }
        }

        /// <summary>
        /// 获取线圈写入命令
        /// </summary>
        /// <param name="address">寄存器地址</param>
        /// <param name="value"></param>
        /// <param name="stationNumber">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <returns></returns>
        public byte[] GetWriteCoilCommand(string address, bool value, byte stationNumber, byte functionCode)
        {
            var writeAddress = ushort.Parse(address?.Trim());
            if (plcAddresses) writeAddress = (ushort)(Convert.ToUInt16(address?.Trim().Substring(1)) - 1);

            byte[] buffer = new byte[6];
            buffer[0] = stationNumber;//站号
            buffer[1] = functionCode; //功能码
            buffer[2] = BitConverter.GetBytes(writeAddress)[1];
            buffer[3] = BitConverter.GetBytes(writeAddress)[0];//寄存器地址
            buffer[4] = (byte)(value ? 0xFF : 0x00);     //此处只可以是FF表示闭合00表示断开，其他数值非法
            buffer[5] = 0x00;
            return buffer;
        }
        #endregion
    }
}
