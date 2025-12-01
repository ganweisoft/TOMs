// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTClient.Common.Helpers;
using IoTClient.Enums;
using IoTClient.Models;
using System.Net;
using System.Net.Sockets;

namespace IoTClient.Clients.Modbus
{
    /// <summary>
    /// Tcp的方式发送ModbusRtu协议报文 - 客户端
    /// </summary>
    public class ModbusRtuOverTcpClient : SocketBase, IModbusClient
    {
        private IPEndPoint ipEndPoint;
        private int timeout = -1;
        private EndianFormat format;
        private bool plcAddresses;

        /// <summary>
        /// 是否是连接的
        /// </summary>
        public bool Connected => socket?.Connected ?? false;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <param name="port">端口</param>
        /// <param name="timeout">超时时间（毫秒）</param>
        /// <param name="format">大小端设置</param>
        /// <param name="plcAddresses">PLC地址</param>
        public ModbusRtuOverTcpClient(string ip, int port, int timeout = 1500, EndianFormat format = EndianFormat.ABCD, bool plcAddresses = false)
        {
            this.timeout = timeout;
            if (!IPAddress.TryParse(ip, out IPAddress address))
                address = Dns.GetHostEntry(ip).AddressList?.FirstOrDefault();
            ipEndPoint = new IPEndPoint(address, port);
            this.format = format;
            this.plcAddresses = plcAddresses;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ipEndPoint">ip地址和端口</param>
        /// <param name="timeout">超时时间（毫秒）</param>
        /// <param name="format">大小端设置</param>
        public ModbusRtuOverTcpClient(IPEndPoint ipEndPoint, int timeout = 1500, EndianFormat format = EndianFormat.ABCD, bool plcAddresses = false)
        {
            this.timeout = timeout;
            this.ipEndPoint = ipEndPoint;
            this.format = format;
            this.plcAddresses = plcAddresses;
        }

        protected override Result Connect()
        {
            var result = new Result();

            // 如果已有的Socket对象存在且连接是有效的，直接返回成功结果
            if (socket != null && socket.Connected)
            {
                return result.EndTime();
            }

            try
            {
                // 如果Socket已经存在但连接断开，先释放旧的Socket
                if (socket != null)
                {
                    socket.SafeClose();
                    socket = null;
                }

                // 创建新的Socket对象
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                {
                    ReceiveTimeout = timeout,
                    SendTimeout = timeout
                };

                // 异步连接
                var connectResult = socket.BeginConnect(ipEndPoint, null, null);
                if (!connectResult.AsyncWaitHandle.WaitOne(timeout))
                {
                    throw new TimeoutException("连接超时");
                }
                socket.EndConnect(connectResult);
            }
            catch (Exception ex)
            {
                // 发生异常时关闭Socket并释放资源
                socket?.SafeClose();
                result.IsSucceed = false;
                result.Err = ex.Message;
                result.ErrCode = 408;
                result.Exception = ex;
            }

            return result.EndTime();
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
        /// <param name="lenght"></param>
        /// <returns></returns>
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        public async Task<Result<byte[]>> SendPackage(byte[] command, int lenght)
        {
            async Task<Result<byte[]>> _SendPackageAsync()
            {
                await _semaphoreSlim.WaitAsync();
                try
                {
                    //从发送命令到读取响应为最小单元，避免多线程执行串数据（可线程安全执行）
                    Result<byte[]> result = new Result<byte[]>();
                    //发送命令
                    await socket.SendAsync(command, SocketFlags.None);
                    //获取响应报文    
                    var socketReadResul = await SocketRead(socket, lenght);
                    if (!socketReadResul.IsSucceed)
                        return socketReadResul;
                    result.Value = socketReadResul.Value;
                    return result.EndTime();
                }
                finally
                {
                    _semaphoreSlim.Release();
                }
            }

            try
            {
                var result = await _SendPackageAsync();
                if (!result.IsSucceed)
                {
                    WarningLog?.Invoke(result.Err, result.Exception);
                    //如果出现异常，则进行一次重试         
                    var conentResult = Connect();
                    if (!conentResult.IsSucceed)
                        return new Result<byte[]>(conentResult);

                    return await _SendPackageAsync();
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

                return await _SendPackageAsync();
            }
        }

        public override async Task<Result<byte[]>> SendPackageSingle(byte[] command)
        {
            throw new NotImplementedException();
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
        /// <param name="byteFormatting"></param>
        /// <returns></returns>
        public async Task<Result<byte[]>> Read(string address, byte stationNumber = 1, byte functionCode = 3, ushort readLength = 1, bool byteFormatting = true)
        {
            if (isAutoOpen) Connect();

            var result = new Result<byte[]>();
            try
            {
                //获取命令（组装报文）
                byte[] command = GetReadCommand(address, stationNumber, functionCode, readLength);
                var commandCRC16 = CRC16.GetCRC16(command);
                result.Requst = string.Join(" ", commandCRC16.Select(t => t.ToString("X2")));

                //发送命令并获取响应报文
                int readLenght;
                if (functionCode == 1 || functionCode == 2)
                    readLenght = 5 + (int)Math.Ceiling((float)readLength / 8);
                else
                    readLenght = 5 + readLength * 2;
                var sendResult = await SendPackage(commandCRC16, readLenght);
                if (!sendResult.IsSucceed)
                    return sendResult;
                var responsePackage = sendResult.Value;
                //var responsePackage = SendPackage(commandCRC16, readLenght);
                if (!responsePackage.Any())
                {
                    result.IsSucceed = false;
                    result.Err = "响应结果为空";
                    return result.EndTime();
                }
                else if (!CRC16.CheckCRC16(responsePackage))
                {
                    result.IsSucceed = false;
                    result.Err = "响应结果CRC16验证失败";
                    //return result.EndTime();
                }

                byte[] resultData = new byte[responsePackage.Length - 2 - 3];
                Array.Copy(responsePackage, 3, resultData, 0, resultData.Length);
                result.Response = string.Join(" ", responsePackage.Select(t => t.ToString("X2")));
                //4 获取响应报文数据（字节数组形式）       
                if (byteFormatting)
                    result.Value = resultData.Reverse().ToArray().ByteFormatting(format);
                else
                    result.Value = resultData.Reverse().ToArray();
            }
            catch (Exception ex)
            {
                result.IsSucceed = false;
                result.Err = ex.Message;
            }
            finally
            {
                if (isAutoOpen) Dispose();
            }
            return result.EndTime();
        }

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
                        result.IsSucceed = tempResult.IsSucceed;
                        result.Err = tempResult.Err;
                        result.Exception = tempResult.Exception;
                        result.ErrCode = tempResult.ErrCode;
                    }
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
        public async Task<Result> Write(string address, bool value, byte stationNumber = 1, byte functionCode = 5)
        {
            if (isAutoOpen) Connect();
            var result = new Result();
            try
            {
                var command = GetWriteCoilCommand(address, value, stationNumber, functionCode);
                var commandCRC16 = CRC16.GetCRC16(command);
                result.Requst = string.Join(" ", commandCRC16.Select(t => t.ToString("X2")));
                //发送命令并获取响应报文
                //var responsePackage = SendPackage(commandCRC16, 8);
                var sendResult = await SendPackage(commandCRC16, 8);
                if (!sendResult.IsSucceed)
                    return sendResult;
                var responsePackage = sendResult.Value;

                if (!responsePackage.Any())
                {
                    result.IsSucceed = false;
                    result.Err = "响应结果为空";
                    return result.EndTime();
                }
                else if (!CRC16.CheckCRC16(responsePackage))
                {
                    result.IsSucceed = false;
                    result.Err = "响应结果CRC16验证失败";
                    //return result.EndTime();
                }
                byte[] resultBuffer = new byte[responsePackage.Length - 2];
                Buffer.BlockCopy(responsePackage, 0, resultBuffer, 0, resultBuffer.Length);
                result.Response = string.Join(" ", responsePackage.Select(t => t.ToString("X2")));
            }
            catch (Exception ex)
            {
                result.IsSucceed = false;
                result.Err = ex.Message;
            }
            finally
            {
                if (isAutoOpen) Dispose();
            }
            return result.EndTime();
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="address"></param>
        /// <param name="values"></param>
        /// <param name="stationNumber"></param>
        /// <param name="functionCode"></param>
        /// <returns></returns>
        public async Task<Result> Write(string address, byte[] values, byte stationNumber = 1, byte functionCode = 16, bool byteFormatting = true)
        {
            if (isAutoOpen) Connect();

            var result = new Result();
            try
            {
                values = values.ByteFormatting(format);
                var command = GetWriteCommand(address, values, stationNumber, functionCode);

                var commandCRC16 = CRC16.GetCRC16(command);
                result.Requst = string.Join(" ", commandCRC16.Select(t => t.ToString("X2")));
                //var responsePackage = SendPackage(commandCRC16, 8);
                var sendResult = await SendPackage(commandCRC16, 8);
                if (!sendResult.IsSucceed)
                    return sendResult;
                var responsePackage = sendResult.Value;

                if (!responsePackage.Any())
                {
                    result.IsSucceed = false;
                    result.Err = "响应结果为空";
                    return result.EndTime();
                }
                else if (!CRC16.CheckCRC16(responsePackage))
                {
                    result.IsSucceed = false;
                    result.Err = "响应结果CRC16验证失败";
                    //return result.EndTime();
                }
                byte[] resultBuffer = new byte[responsePackage.Length - 2];
                Array.Copy(responsePackage, 0, resultBuffer, 0, resultBuffer.Length);
                result.Response = string.Join(" ", responsePackage.Select(t => t.ToString("X2")));
            }
            catch (Exception ex)
            {
                result.IsSucceed = false;
                result.Err = ex.Message;
            }
            finally
            {
                if (isAutoOpen) Dispose();
            }
            return result.EndTime();
        }

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
                // 写单个寄存器，要求 values 必须是2字节
                if (values.Length != 2)
                    throw new ArgumentException("功能码0x06要求 values 长度为2字节（一个寄存器）");

                byte[] buffer = new byte[6];
                buffer[0] = stationNumber;                               // 站号
                buffer[1] = functionCode;                                // 功能码（0x06）
                buffer[2] = BitConverter.GetBytes(writeAddress)[1];      // 地址高位
                buffer[3] = BitConverter.GetBytes(writeAddress)[0];      // 地址低位
                buffer[4] = values[0];                                   // 数据高位
                buffer[5] = values[1];                                   // 数据低位
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
