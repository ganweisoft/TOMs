// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using IoTClient.Clients.Modbus;
using IoTClient.Models;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace GWModbusStandard.STD;

public class ConnClientManager
{
    private static readonly Lazy<ConnClientManager> instance = new Lazy<ConnClientManager>(() => new ConnClientManager());

    /// <summary>
    /// Modbus连接对象池
    /// </summary>
    private ConcurrentDictionary<string, Lazy<ClientSession>> clientSessions = new ConcurrentDictionary<string, Lazy<ClientSession>>();

    /// <summary>
    /// 失败的服务及时间
    /// </summary>
    private ConcurrentDictionary<string, DateTime> connectFailServers = new();

    /// <summary>
    /// 重试时间间隔30秒
    /// </summary>
    private int reTryTime = 30;

    private ConnClientManager() { }

    public static ConnClientManager Instance
    {
        get { return instance.Value; }
    }

    /// <summary>
    /// 添加连接对接
    /// 需要确保当前方法可以重复执行，应避免多次创建连接对象。
    /// </summary>
    /// <param name="connectionConfig"></param>
    public void CreateClientSession(ConnectionConfig connectionConfig)
    {
        //判断当前连接是否出现连接失败的问题。
        if (connectFailServers.ContainsKey(connectionConfig.ServerUrl))
        {
            //判断上次失败的时间是否超过了30秒，如果未超过30秒就不在重新建立连接，避免其他正常的连接阻塞
            var lastErrorDate = connectFailServers[connectionConfig.ServerUrl];
            if (lastErrorDate > DateTime.Now) return;
        }

        var opcUaSession = clientSessions.GetOrAdd(connectionConfig.ServerUrl, key => new Lazy<ClientSession>(() => CreateNewSession(connectionConfig)));

        //如果连接对象为空，移除
        if (opcUaSession == null || opcUaSession.Value == null)
        {
            clientSessions.TryRemove(connectionConfig.ServerUrl, out _);
            return;
        }

        //更新配置
        opcUaSession.Value.ReSetPollingInterval(connectionConfig);
    }

    public ClientSession CreateNewSession(ConnectionConfig connectionConfig)
    {
        try
        {
            IModbusClient modbusClient;
            switch (connectionConfig.ModbusType)
            {
                case ModbusType.Tcp:
                    modbusClient = new ModbusTcpClient(connectionConfig.ServerIpAndPoint, connectionConfig.TimeOut, connectionConfig.EndianFormat);
                    break;
                case ModbusType.Rtu:
                    modbusClient = new ModbusRtuClient(connectionConfig.ServerUrl, connectionConfig.BaudRate, connectionConfig.DataBits, connectionConfig.StopBits, connectionConfig.Parity, connectionConfig.TimeOut, connectionConfig.EndianFormat);
                    break;
                case ModbusType.Ascii:
                    modbusClient = new ModbusAsciiClient(connectionConfig.ServerUrl, connectionConfig.BaudRate, connectionConfig.DataBits, connectionConfig.StopBits, connectionConfig.Parity, connectionConfig.TimeOut, connectionConfig.EndianFormat);
                    break;
                case ModbusType.RtuOverTcp:
                    modbusClient = new ModbusRtuOverTcpClient(connectionConfig.ServerIpAndPoint, connectionConfig.TimeOut, connectionConfig.EndianFormat);
                    break;
                default:
                    throw new Exception("不支持的ModbusType类型");
            }
            modbusClient.EnsureConnected();
            var clientSession = new ClientSession(connectionConfig, modbusClient);

            //当前连接订阅所有地址
            AddAllAddress(connectionConfig.ServerUrl, clientSession);

            //开始所有地址数据的获取
            clientSession.StartPolling();

            return clientSession;
        }
        catch (Exception ex)
        {
            connectFailServers[connectionConfig.ServerUrl] = DateTime.Now.AddSeconds(reTryTime);
            DataCenter.WriteLogFile($"Failed to connect to server {connectionConfig.ServerUrl}: {ex.Message}", LogType.Error);
            return null;
        }
    }


    /// <summary>
    /// 获取当前连接实例的状态
    /// </summary>
    /// <param name="serverUrl"></param>
    /// <returns></returns>
    public bool GetClientSessionStatus(string serverUrl)
    {
        try
        {
            if (clientSessions.TryGetValue(serverUrl, out var clientSession))
            {
                clientSession.Value.StartPolling();
                return clientSession.Value.Status;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            DataCenter.WriteLogFile($"Exception occurred while writing value: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 一次性获取当前连接的所有点位数据
    /// </summary>
    /// <param name="serverUrl">连接地址</param>
    /// <param name="equipNo">设备号</param>
    public void AddAllAddress(string serverUrl, ClientSession clientSession)
    {
        try
        {
            var allStationEquipNo = StationItem.db_Eqp
                 .Where(m => m.communication_param == serverUrl)
                 .Select(m => new { m.equip_addr, m.equip_no })
                 .GroupBy(m => m.equip_addr);

            foreach (var item in allStationEquipNo)
            {
                var allEquipNo = item.Select(m => m.equip_no).ToArray();
                var ycNodeIds = StationItem.db_Ycp.Where(m => allEquipNo.Contains(m.equip_no)).Select(m => m.main_instruction).ToList();
                var yxnodeIds = StationItem.db_Yxp.Where(m => allEquipNo.Contains(m.equip_no)).Select(m => m.main_instruction).ToList();
                var combinedNodeIds = ycNodeIds.Concat(yxnodeIds).Distinct().ToArray();

                if (!byte.TryParse(item.Key, out var _stationNumber))
                {
                    DataCenter.WriteLogFile($"AddAllSubscription 连接站号填写不对,请检查，当前填写值为：{item.Key}，所在设备号为：{allEquipNo.FirstOrDefault()}");
                }

                //将字符串的点位值转成模型
                var subAddressList = clientSession.NodeStrTranHelper.GetModbusInputList(_stationNumber, combinedNodeIds);
                //订阅所有地址的数据
                clientSession.AddAddressInputs(subAddressList);
            }
        }
        catch (Exception ex)
        {
            DataCenter.WriteLogFile($"AddAllAddress ex：{ex}");
        }
    }


    /// <summary>
    /// 添加Modbus的地址到当前连接的列表中，。
    /// </summary>
    /// <param name="serverUrl">连接地址</param>
    /// <param name="equipNo">设备号</param>
    public void AddSubscription(string serverUrl, byte station, List<ModbusInput> modbusInputs)
    {
        try
        {
            if (!clientSessions.TryGetValue(serverUrl, out Lazy<ClientSession> clientSession)) return;
            clientSession.Value.AddAddressInputs(modbusInputs);
        }
        catch (Exception ex)
        {
            DataCenter.WriteLogFile($"AddSubscription ex：{ex}");
        }
    }

    /// <summary>
    /// 向设备下发控制命令
    /// 注意：该方法的具体实现根据协议的来处理，开发人员可以修改入参等内容。
    /// </summary>
    /// <param name="serverUrl">连接地址</param>
    /// <param name="controlMethod">控制方法</param>
    /// <param name="value">传入值</param>
    /// <returns></returns>
    public async Task<EquipSetResponseModel> WriteValue(string serverUrl, EquipSetRequestModel equipSetRequest)
    {
        var response = new EquipSetResponseModel();
        try
        {
            if (!clientSessions.TryGetValue(serverUrl, out Lazy<ClientSession> clientSession))
            {
                response.Fail("设备连接对象不存在");
                return response;
            }

            if (equipSetRequest.WriteList != null && equipSetRequest.WriteList.Count > 0)
            {
                var writeResultList = new List<EquipSetResponseModel>();
                foreach (var inputModel in equipSetRequest.WriteList)
                {
                    var writeRes = new EquipSetResponseModel();
                    var res = await clientSession.Value.WriteAddressValues(inputModel);

                    DataCenter.WriteLogFile($"写入点位响应结果: {JsonConvert.SerializeObject(res)}", LogType.Debug);
                    if (res.IsSucceed)
                    {
                        writeRes.Ok(inputModel);
                    }
                    else
                    {
                        writeRes.Fail(res.Err, inputModel);
                    }
                    writeResultList.Add(writeRes);
                }
                response.Ok(writeResultList);
            }

            //如果有读取数据
            if (equipSetRequest.ReadList != null && equipSetRequest.ReadList.Count > 0)
            {
                var writeResultList = new List<EquipSetResponseModel>();
                foreach (var inputModel in equipSetRequest.ReadList)
                {
                    var writeRes = new EquipSetResponseModel();
                    var res = await clientSession.Value.ReadAddressValues(inputModel);

                    DataCenter.WriteLogFile($"读取点位响应结果: {JsonConvert.SerializeObject(res)}", LogType.Debug);
                    if (res.IsSucceed)
                    {
                        writeRes.Ok(inputModel);
                    }
                    else
                    {
                        writeRes.Fail(res.Err, inputModel);
                    }
                    writeResultList.Add(writeRes);
                }
                response.Ok(writeResultList);
            }
        }
        catch (Exception ex)
        {
            DataCenter.WriteLogFile($"Exception occurred while writing value: {ex}");
            response.Fail("命令执行出现异常：" + ex.Message);
        }
        return response;
    }


    /// <summary>
    /// 获取当前连接地址，当前设备采集的实时数据
    /// </summary>
    /// <param name="serverUrl"></param>
    /// <param name="nodeId"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public Dictionary<string, object> GetCurrentValues(string serverUrl, List<ModbusInput> modbusInputs)
    {
        try
        {
            if (!clientSessions.TryGetValue(serverUrl, out Lazy<ClientSession> clientSession)) return null;
            // 获取并返回符合条件的 CurrentValues
            return clientSession.Value.GetCurrentValues(modbusInputs);
        }
        catch (Exception ex)
        {
            DataCenter.WriteLogFile($"Exception occurred while getting values: {ex.Message}");
            return null;
        }
    }


    /// <summary>
    /// 获取当前连接地址，当前设备产生的事件记录
    /// </summary>
    /// <param name="serverUrl"></param>
    /// <param name="nodeId"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public List<EquipEventModel> GetCurrentEvents(string serverUrl, int equipNo)
    {
        try
        {
            if (clientSessions.TryGetValue(serverUrl, out Lazy<ClientSession> clientSession))
            {
                //事件记录都是递增方式添加事件，相对于属性值而言，正常业务不希望事件记录丢失，开发者可以对事件记录数据做一份落库更保险。
                //本次获取后将数据移除，防止内存单个设备事件记录一直增加。
                return clientSession.Value.GetCurrentEvents(equipNo);
            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            DataCenter.WriteLogFile($"Exception occurred while writing value: {ex.Message}");
            return null;
        }
        return null;
    }


    public List<ModbusInput> GetModbusInputList(string serverUrl, byte stationNumber, string[] nodeStrs)
    {
        if (!clientSessions.TryGetValue(serverUrl, out Lazy<ClientSession> clientSession)) return null;

        var modbusInputs = clientSession.Value.NodeStrTranHelper.GetModbusInputList(stationNumber, nodeStrs);

        return modbusInputs;
    }
}