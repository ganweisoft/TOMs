// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using GWDataCenter.Database;
using IoTClient.Enums;
using IoTClient.Models;
using Newtonsoft.Json;
using OpenGWDataCenter.Model;
using System.IO.Ports;

namespace GWModbusStandard.STD;

/// <summary>
/// 在设备管理中，每个设备在程序中都会生成一个设备对象
/// </summary>
public class CEquip : CEquipBase
{
    /// <summary>
    /// 设备通信间隔时间
    /// </summary>
    private int _defaultSleepInterval = 5000;

    /// <summary>
    /// 设备连接参数信息，如果连接参数很简单，可以不用定义对象。
    /// </summary>
    private ConnectionConfig _config;

    /// <summary>
    /// 当前设备的实时数据
    /// </summary>
    private Dictionary<string, object> _currentValue;

    /// <summary>
    /// 当前设备的实时事件，开发者需要处理相同事件内容重复产生的问题。
    /// </summary>
    private List<EquipEventModel> _currentEvents;

    /// <summary>
    /// 自定义当前设备的事件名称级别
    /// 如果有多个事件名称需要定义，可以在自定义参数中进行定义
    /// </summary>
    private int _defaultEventMessageLevel = 0;

    /// <summary>
    /// 站点号
    /// 一个TCP地址就是一个从站
    /// 如果是Modbus RTU/ASCII (串行通信)，则一个串口可能有多个从站
    /// </summary>
    private byte _stationNumber = 1;

    private List<ModbusInput> modbusInputs = new List<ModbusInput>();

    /// <summary>
    /// 初始化设备相关参数
    /// 在界面添加完成后，会进入到该方法进行初始化
    /// 之后再界面修改连接参数后，会再一次进入该方法。
    /// </summary>
    /// <param name="item">equip表对象属性</param>
    /// <returns></returns>
    public override bool init(EquipItem item)
    {
        /*
        item.Equip_addr 设备地址
        解释：通常存放设备的唯一标识或者设备的连接地址。这里需要根据具体的协议来区分，如果一对一的直连设备

        item.communication_param 设备连接参数
        解释：通常存放设备的连接信息，具体由当前协议插件来约定，在配置文档中写明即可。

        item.Local_addr 通讯端口（也叫通讯线程），任意字符，不宜过长。
        解释：在Equip表，你可能会发现不少设备的Local_addr字段可能都是空的，也可能都是一个具体的字符串。
        我们按照该字段的值进行Group By归类后，就得到了同一个值的设备数量有多少个，这个就代表一个线程管控了多少个设备。

        item.communication_time_param
        解释：在设备线程组里面，一个设备多久通信一次，即多久采集一次数据，单位毫秒。 
        如果communication_time_param职能比较多，也可以将多个参数的拼接，此时需要自行处理拆分后再转换。
        配置举例：假设1个线程管控10个设备，要求每个设备每秒采集一次数据，那么这个字段的值应不大于100毫秒。其他场景同理计算即可。

        item.Reserve2 设备自定义参数
        解释：一般一些连接参数较多，需要规范化存储时，可以将属性放到自定义参数中，直观一些。当然也可以使用其他字段去拼接起来，但不建议这样做。
        在6.1版本中，该字段在数据库中存储的值为一个JSON格式的数据。
        在低版本中可以按照JSON格式来存储这个数据。
         */

        //获取设备连接通讯的间隔时间。
        _ = int.TryParse(item.communication_time_param, out int sleepInterval);

        //获取从站号/设备号
        _ = byte.TryParse(item.Equip_addr, out _stationNumber);

        /*
        在构造连接参数数，根据实际情况，以下展示一个连接参数模型的赋值。
        如果连接参数简单，也可以使用自定义连接参数，直接使用communication_param更好，减少配置项，这里需要开发人员自己确定好。
         */
        _config = new ConnectionConfig()
        {
            ServerUrl = item.communication_param,
            SleepInterval = sleepInterval == 0 ? _defaultSleepInterval : sleepInterval
        };
        if (!string.IsNullOrWhiteSpace(item.Reserve2))
        {
            var dictParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(item.Reserve2);
            _config.ModbusType = Enum.TryParse<ModbusType>(dictParams.TryGetValue("ModbusType", out var modbusType) ? modbusType : ModbusType.Tcp.ToString(), out var parsedModbusType) ? parsedModbusType : ModbusType.Tcp;
            _config.EndianFormat = Enum.TryParse<EndianFormat>(dictParams.TryGetValue("EndianFormat", out var endianFormat) ? endianFormat : EndianFormat.ABCD.ToString(), out var parsedEndianFormat) ? parsedEndianFormat : EndianFormat.ABCD;
            _config.BaudRate = dictParams.TryGetValue("BaudRate", out var baudRateStr) && int.TryParse(baudRateStr, out var baudRate) ? baudRate : 9600;
            _config.DataBits = dictParams.TryGetValue("DataBits", out var dataBitsStr) && int.TryParse(dataBitsStr, out var dataBits) ? dataBits : 8;
            _config.TimeOut = dictParams.TryGetValue("TimeOut", out var timeOutStr) && int.TryParse(timeOutStr, out var timeOut) ? timeOut : 3000;
            _config.Parity = Enum.TryParse<Parity>(dictParams.TryGetValue("Parity", out var parity) ? parity : Parity.None.ToString(), out var parsedParity) ? parsedParity : Parity.None;
            _config.StopBits = Enum.TryParse<StopBits>(dictParams.TryGetValue("StopBits", out var stopBits) ? stopBits : StopBits.None.ToString(), out var parsedStopBits) ? parsedStopBits : StopBits.None;

            //我们可以定义多个事件名称的级别，命名方式如DefaultEventMessageLevel，如果未取到，默认值给0，但最好要区分好，因为使用0的事件级别很多场景都使用。
            _ = int.TryParse(dictParams.TryGetValue("DefaultEventMessageLevel", out var defaultEventMessageLevelStr) ? defaultEventMessageLevelStr : "0", out _defaultEventMessageLevel);

            OnLoaded();
        }
        return base.init(item);
    }

    /// <summary>
    /// 设备连接初始化
    /// 对于设备的连接地址，连接账号密码发生更改后，可以进行重连。
    /// </summary>
    /// <returns></returns>
    public void OnLoaded()
    {
        //TODO 这里可以写于设备连接的具体代码了。根据_connectionConfig连接参数，去创建自己的连接对象。
        ConnClientManager.Instance.CreateClientSession(_config);

        // main_instruction 功能码，地址，类型
        var ycNodeIds = this.ycprows?.Where(m => m != null).Select(m => m.main_instruction) ?? Enumerable.Empty<string>();
        var yxnodeIds = this.yxprows?.Where(m => m != null).Select(m => m.main_instruction) ?? Enumerable.Empty<string>();

        //获取当前设备下所以的点位地址
        var combinedNodeIds = ycNodeIds.Concat(yxnodeIds).Distinct().ToArray();
        //将字符串的点位值转成模型
        modbusInputs = ConnClientManager.Instance.GetModbusInputList(_config.ServerUrl, _stationNumber, combinedNodeIds);

        //将点位地址添加到订阅里面
        ConnClientManager.Instance.AddSubscription(_config.ServerUrl, _stationNumber, modbusInputs);
        //返回默认值
        //return base.OnLoaded();
    }

    /// <summary>
    /// 获取设备状态及实时数据
    /// 注意要控制好该方法不要出异常，否则会出现设备一直处于初始化状态中
    /// </summary>
    /// <param name="pEquip">设备基类对象</param>
    /// <returns></returns>
    public override CommunicationState GetData(CEquipBase pEquip)
    {
        //获取当前连接地址的状态
        var equipStatus = ConnClientManager.Instance.GetClientSessionStatus(_config.ServerUrl);

        //如果连接状态正常，设置为在线
        if (equipStatus)
        {
            //只有在线是才采集数据
            _currentValue = ConnClientManager.Instance.GetCurrentValues(_config.ServerUrl, modbusInputs);

            //如果设备在线，默认执行基类的方法，base.GetData(pEquip)返回值是ok。
            return base.GetData(pEquip);
        }
        else
        {
            this.Sleep(10);
            //否则设置离线
            return CommunicationState.fail;
        }
    }

    /// <summary>
    /// 遥测点设置
    /// </summary>
    /// <param name="r">ycp表对象属性(不是全部)</param>
    /// <returns></returns>
    public override bool GetYC(YcpTableRow r)
    {
        /*
        注意：在此处最好不用打印日志，因为这里会产生大量的日志，如果需要调试某个点位时，可以在自定义参数里面加参数，针对固定的遥测进行日志调试。
        r.main_instruction 操作命令，如EquipCurrentInfo
        r.minor_instruction 操作参数,如Temperature，Humidness等
        r.Reserve2 自定义参数，以json结构存储，同设备的自定义参数一样。

        在给遥测赋值时提供了诸多方法，支持单个类型，多元组类型，可以根据实际需要使用。
        SetYCData(YcpTableRow r, object o);
        SetYCDataNoRead(IQueryable<YcpTableRow> Rows);
        SetYcpTableRowData(YcpTableRow r, float o);
        SetYcpTableRowData(YcpTableRow r, (double, double, double, double, double, double) o);
        SetYcpTableRowData(YcpTableRow r, string o);
        SetYcpTableRowData(YcpTableRow r, int o);
        SetYcpTableRowData(YcpTableRow r, double o);
        SetYcpTableRowData(YcpTableRow r, (double, double, double, double, double, double, double) o);
        SetYcpTableRowData(YcpTableRow r, (double, double) o);
        SetYcpTableRowData(YcpTableRow r, (DateTime, double) o);
        SetYcpTableRowData(YcpTableRow r, (double, double, double, double) o);
        SetYcpTableRowData(YcpTableRow r, (double, double, double, double, double) o);
        SetYcpTableRowData(YcpTableRow r, (double, double, double) o);
        */

        /* 实时数据示例代码，可以根据自己的业务进行处理*/
        if (_currentValue == null || _currentValue.Count == 0) return true;
        try
        {
            //此处的Key值需要根据实际情况去处理。如果构造实时数据缓存字典是需要由开发去定义。
            //总的来说，按照设备+遥测遥信的方式构造缓存数据是比较合理的。

            var inputAddress = modbusInputs.Find(m => m.NodeStr == r.main_instruction && m.StationNumber == _stationNumber);
            if (inputAddress == null) return true;

            if (_currentValue.ContainsKey(inputAddress.DisplayNameKey))
            {
                var objValue = _currentValue[inputAddress.DisplayNameKey];
                if (objValue == null) SetYCData(r, "");//此处不可以设置为null。
                else SetYCData(r, objValue);
            }
            else
            {
                SetYCData(r, "***");
            }
        }
        catch (Exception ex)
        {
            SetYCData(r, "***");
            var logMsg = string.Format("设备号：{0}，遥测名称：{1}，异常：{2}", r.equip_no, r.yc_nm, ex.ToString());
            DataCenter.WriteLogFile(logMsg);
        }

        //此处默认都返回true，否则设备会处于离线。
        return true;
    }

    /// <summary>
    /// 遥信点设置
    /// </summary>
    /// <param name="r">yxp表对象属性(不是全部)</param>
    /// <returns></returns>
    public override bool GetYX(YxpTableRow r)
    {
        /*
        注意：在此处最好不用打印日志，因为这里会产生大量的日志，如果需要调试某个点位时，可以在自定义参数里面加参数，针对固定的遥测进行日志调试。
        r.main_instruction 操作命令，如EquipCurrentInfo
        r.minor_instruction 操作参数,如Temperature，Humidness等
        r.Reserve2 自定义参数，以json结构存储，同设备的自定义参数一样。

        在给遥测赋值时提供了诸多方法，支持bool、string类型，正常使用bool就够了，特殊情况可自行处理。
        SetYXData(YxpTableRow r, object o);
        SetYxpTableRowData(YxpTableRow r, string o);
        SetYxpTableRowData(YxpTableRow r, bool o);
        */

        /* 实时数据示例代码，可以根据自己的业务进行处理*/
        if (_currentValue == null || _currentValue.Count == 0) return true;
        try
        {
            //使用账号_地址组合
            var inputAddress = modbusInputs.Find(m => m.NodeStr == r.main_instruction);
            if (inputAddress == null) return true;

            if (_currentValue.ContainsKey(inputAddress.DisplayNameKey))
            {
                var nodeIdObj = _currentValue[inputAddress.DisplayNameKey];
                if (nodeIdObj == null) SetYXData(r, "***");
                else SetYXData(r, nodeIdObj);
            }
            else
            {
                SetYXData(r, "***");
            }
        }
        catch (Exception ex)
        {
            SetYXData(r, "***");
            //记录执行传参及响应结果到日志中，便于追溯。
            var logMsg = string.Format("设备号：{0}，遥信名称：{1}，异常：{2}", r.equip_no, r.yx_nm, ex.ToString());
            DataCenter.WriteLogFile(logMsg);
        }
        return true;
    }

    /// <summary>
    /// 事件发布
    /// 如门禁设备的一些通行记录数据。
    /// 如果对事件记录实时性有非常高的要求，可以接收到事件后直接转。
    /// </summary>
    /// <returns></returns>
    public override bool GetEvent()
    {
        //从当前设备连接中获取事件列表
        _currentEvents = ConnClientManager.Instance.GetCurrentEvents(_config.ServerUrl, this.m_equip_no);
        if (_currentEvents == null || _currentEvents.Count == 0) return true;

        //假设_currentEvents对象每次都是新的数据，不存在旧数据,需开发者自行处理好.
        foreach (var eventItem in _currentEvents)
        {
            //EquipEvent中的事件级别根据当前事件名称定义好的级别。便于北向上报数据时的甄别。
            var evt = new EquipEvent(JsonConvert.SerializeObject(eventItem), (MessageLevel)_defaultEventMessageLevel, DateTime.Now);
            EquipEventList.Add(evt);
        }
        _currentEvents = null; //循环完成后，将事件记录置空，避免下次重复产生相同的事件.
        return true;
    }

    /// <summary>
    /// 设备命令下发
    /// </summary>
    /// <param name="mainInstruct">操作命令</param>
    /// <param name="minorInstruct">操作参数</param>
    /// <param name="value">传入的值</param>
    /// <returns></returns>
    public override bool SetParm(string mainInstruct, string minorInstruct, string value)
    {
        /*
        注意：建议在此处打印日志，便于记录由平台执行命令的情况，用于追溯命令下发情况。
        mainInstruct 操作命令，如：Write,Read,WriteRead
        minorInstruct 操作参数,功能码，地址，类型
        value 命令下发的参数值，如：22
        */

        var _nodeStrTran = new NodeStrTranHelper();
        var responseModel = new EquipSetResponseModel();

        //将执行结果对象转换成json字符串
        string logMsg = string.Empty;

        if (!Enum.TryParse<ControlType>(mainInstruct, out var controlType))
        {
            responseModel.Fail("操作命令错误，请按使用文档填写");
        }
        if (string.IsNullOrWhiteSpace(minorInstruct))
        {
            responseModel.Fail("操作参数不能为空，请按使用文档填写");
        }
        if (responseModel.Code != 200)
        {
            //给当前设置点赋值响应内容，用于北向转发时告知设备实际执行结果
            logMsg = string.Format("命令下发参数,设备号：{0}，mainInstruct：{1}，minorInstruct：{2}，value：{3},下发执行结果：{4}",
            this.equipitem.iEquipno, mainInstruct, minorInstruct, value, responseModel.Message);
            DataCenter.WriteLogFile(logMsg, LogType.Error);
            return false;
        }

        var setRequestModel = new EquipSetRequestModel(controlType);
        switch (controlType)
        {
            case ControlType.Write:
                /*
                类型为write
                对某个地址进行值写入。
                minor_instruction格式：功能码,寄存器地址,数据类型。如：16,0,1。
                */

                var writeAddress = _nodeStrTran.GetModbusInput(_stationNumber, minorInstruct);
                if (writeAddress == null)
                {
                    responseModel.Fail("操作命令错误，请按使用文档填写");
                    break;
                }

                if (!TryParseWriteValue(writeAddress.FunctionCode, value, out var parsedValue, out var error))
                {
                    responseModel.Fail(error);
                }
                else
                {
                    writeAddress.Value = parsedValue;
                }
                setRequestModel.WriteList.Add(writeAddress);
                break;
            case ControlType.Read:
                /*
                类型为read
                对多个地址进行进行读取，使用“|”进行分割，如：4,0,1|4,0,3。
                minor_instruction格式：功能码,寄存器地址,数据类型|功能码,寄存器地址,数据类型。
                */
                var readAddressList = _nodeStrTran.GetModbusInputList(_stationNumber, minorInstruct.Split('|'));
                if (readAddressList == null)
                {
                    responseModel.Fail("操作命令错误，请按使用文档填写");
                    break;
                }
                setRequestModel.ReadList.AddRange(readAddressList);
                break;
            case ControlType.WriteRead:
                /*
                类型为writeread
                对地址进行写入后，再读取地址。如：16,0,1|4,0,1。其中：16,0,1为写入的值，4,0,1为读取的值。
                minor_instruction格式：功能码,寄存器地址,数据类型|功能码,寄存器地址,数据类型。
                 */
                if (minorInstruct.Split('|').Length != 2)
                {
                    responseModel.Fail("操作命令错误，请按使用文档填写");
                    break;
                }
                var writeStr = minorInstruct.Split('|')[0];
                var readStr = minorInstruct.Split('|')[1];
                var writeAddr = _nodeStrTran.GetModbusInput(_stationNumber, writeStr);
                if (writeAddr == null)
                {
                    responseModel.Fail("操作命令错误，请按使用文档填写");
                    break;
                }

                if (!TryParseWriteValue(writeAddr.FunctionCode, value, out var parsedValue2, out var error2))
                {
                    responseModel.Fail(error2);
                }
                else
                {
                    writeAddr.Value = parsedValue2;
                }

                var readAddr = _nodeStrTran.GetModbusInput(_stationNumber, readStr);
                if (readAddr == null)
                {
                    responseModel.Fail("操作命令错误，请按使用文档填写");
                    break;
                }

                writeAddr.Value = value;
                setRequestModel.WriteList.Add(writeAddr);
                setRequestModel.ReadList.Add(readAddr);
                break;
        }

        if (responseModel.Code != 200)
        {
            //给当前设置点赋值响应内容，用于北向转发时告知设备实际执行结果
            logMsg = string.Format("命令下发参数日志1,设备号：{0}，mainInstruct：{1}，minorInstruct：{2}，value：{3},下发执行结果：{4}",
            this.equipitem.iEquipno, mainInstruct, minorInstruct, value, responseModel.Message);
            DataCenter.WriteLogFile(logMsg, LogType.Error);
            return false;
        }

        responseModel = ConnClientManager.Instance.WriteValue(_config.ServerUrl, setRequestModel).Result;
        var csResponse = JsonConvert.SerializeObject(responseModel);

        //记录执行传参及响应结果到日志中，便于追溯。
        logMsg = string.Format("命令下发参数日志2,设备号：{0}，mainInstruct：{1}，minorInstruct：{2}，value：{3},下发参数：{4}，下发执行结果：{5}",
            this.equipitem.iEquipno, mainInstruct, minorInstruct, value, JsonConvert.SerializeObject(setRequestModel), csResponse);
        DataCenter.WriteLogFile(logMsg, LogType.Debug);

        _nodeStrTran.ClearCache();

        //根据设备执行状态，返回状态，对于发布订阅模式可直接返回true，在相关地方做好日志记录即可。
        if (responseModel.Code == 200) return true;
        else return false;
    }

    private bool TryParseWriteValue(byte functionCode, string value, out object parsedValue, out string error)
    {
        parsedValue = null;
        error = null;

        if (functionCode == 5)
        {
            if (value is "1" or "true" or "True" or "65280")
            {
                parsedValue = true;
                return true;
            }
            else if (value is "0" or "false" or "False" or "0x0000")
            {
                parsedValue = false;
                return true;
            }
            else
            {
                error = "功能码为5时，写入值必须是 [1, 0, true, false, 65280, 0x0000] 中的一个";
                return false;
            }
        }
        else if (functionCode == 6)
        {
            if (ushort.TryParse(value, out var ushortValue))
            {
                parsedValue = ushortValue;
                return true;
            }
            else
            {
                error = "功能码为6时，写入值必须是 0~65535 的整数";
                return false;
            }
        }
        else
        {
            parsedValue = value;
            return true;
        }
    }

}
