// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using IoTClient.Clients.Modbus;
using IoTClient.Models;
using System.Collections.Concurrent;

namespace GWModbusStandard.STD;

public partial class ClientSession
{
    /// <summary>
    /// 此处更换具体的连接对象，如OPC,MQTT,Http的连接对象。
    /// </summary>
    private IModbusClient Session { get; set; }

    /// <summary>
    /// 连接配置信息，主要用于重连
    /// </summary>
    private ConnectionConfig Config { get; set; }

    /// <summary>
    /// 当前连接下，设备及对应的属性内容，此次的object可以根据设备遥测遥信来设计模型
    /// </summary>
    private ConcurrentDictionary<string, ModbusInput> AddressInputs { get; set; }

    /// <summary>
    /// 当前连接下，设备及对应的属性内容，此次的object可以根据设备遥测遥信来设计模型
    /// </summary>
    private Dictionary<string, object> CurrentValues { get; set; }

    /// <summary>
    /// 当前连接下，设备的事件信息。
    /// </summary>
    private ConcurrentDictionary<int, List<EquipEventModel>> CurrentEvents { get; set; }

    public NodeStrTranHelper NodeStrTranHelper { get; set; }

    /// <summary>
    /// 当前设备连接参数
    /// </summary>
    public bool Status = false;

    public ClientSession(ConnectionConfig connectionConfig, IModbusClient session)
    {
        Config = connectionConfig;
        Session = session ?? throw new ArgumentNullException(nameof(session));
        Status = true;
        CurrentValues = new Dictionary<string, object>();
        CurrentEvents = new ConcurrentDictionary<int, List<EquipEventModel>>();
        AddressInputs = new ConcurrentDictionary<string, ModbusInput>();
        NodeStrTranHelper = new NodeStrTranHelper();
    }


    private int _isPolling = 0; // 使用整数标志代替布尔值

    public async Task StartPolling()
    {
        if (Interlocked.CompareExchange(ref _isPolling, 1, 0) != 0) return;
        try
        {
            await ReadAllNodesValues();
        }
        finally
        {
            await Task.Delay(Config.SleepInterval);
            Interlocked.Exchange(ref _isPolling, 0); // 重置标志
        }
    }


    public async Task ReSetClientSession(IModbusClient resession)
    {
        Session = resession;
        await Task.CompletedTask;
    }

    /// <summary>
    /// 更新拉取数据的频率
    /// </summary>
    /// <param name="newConfig"></param>
    public void ReSetPollingInterval(ConnectionConfig newConfig)
    {
        Config.SleepInterval = newConfig.SleepInterval;
    }

    public void AddCurrentValue(string equipNo, object value)
    {
        CurrentValues[equipNo] = value;
    }

    public void AddCurrentEvent(int equipNo, List<EquipEventModel> values)
    {
        if (CurrentEvents.ContainsKey(equipNo))
        {
            CurrentEvents[equipNo].AddRange(values);
        }
        else
        {
            CurrentEvents[equipNo] = values;
        }
    }

    public void AddAddressInputs(List<ModbusInput> modbusInputs)
    {
        foreach (var item in modbusInputs)
        {
            AddressInputs[item.DisplayNameKey] = item;
        }
    }

    public Dictionary<string, object> GetCurrentValues(List<ModbusInput> modbusInputs)
    {
        try
        {
            var modbusInputsKey = modbusInputs.Select(m => m.DisplayNameKey).Distinct().ToArray();
            return CurrentValues.Where(m => modbusInputsKey.Contains(m.Key))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
        catch (Exception ex)
        {
            DataCenter.WriteLogFile($"Exception occurred while getting values: {ex.Message}");
            return null;
        }
    }

    public List<EquipEventModel> GetCurrentEvents(int equipNo)
    {
        try
        {
            if (CurrentEvents.Remove(equipNo, out var equipEventModels))
                return equipEventModels;
        }
        catch (Exception ex)
        {
            DataCenter.WriteLogFile($"Exception occurred while writing value: {ex.Message}");
            return null;
        }
        return null;
    }

    public void SetOnline()
    {
        Status = true;
    }
    public void SetOffline()
    {
        Status = false;
    }

    public void DisposeSession()
    {
        try
        {
            CurrentValues.Clear();
            AddressInputs.Clear();
            CurrentEvents.Clear();

            // 关闭会话
            if (Session != null)
            {
                //Session?.Close();
                //Session?.Dispose();
            }
            // 设置状态为离线
            SetOffline();
        }
        catch (Exception ex)
        {
            DataCenter.WriteLogFile($"Failed to dispose session for {Config.ServerUrl}: {ex.Message}");
        }
    }
}
