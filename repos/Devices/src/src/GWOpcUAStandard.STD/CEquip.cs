// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using GWDataCenter.Database;
using GWOpcUAStandard.STD.Model;
using Newtonsoft.Json;
using Opc.Ua;
using OpenGWDataCenter.Model;

namespace GWOpcUAStandard.STD
{
    public class CEquip : CEquipBase
    {
        /// <summary>
        /// 设备通信间隔时间
        /// </summary>
        private int _sleepInterval = 1000;
        private ConnectionConfig _config;
        private Dictionary<string, DataValue> _nodeIdValues;
        public override bool init(EquipItem item)
        {
            try
            {
                _config = new ConnectionConfig
                {
                    ServerUrl = item.Equip_addr,
                    ConnectionType = ConnectionType.Anonymous,
                    UserName = string.Empty,
                    Password = string.Empty,
                    CertificatePath = string.Empty,
                    CertificatePwd = string.Empty,
                    CertificateName = "opc",
                    Polling = false,
                    PollingSleepTime = _sleepInterval
                };
                if (!string.IsNullOrWhiteSpace(item.Reserve2))
                {
                    var dictParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(item.Reserve2);
                    _config.ConnectionType = Enum.TryParse<ConnectionType>(dictParams.TryGetValue("ConnectionType", out var connType) ? connType : ConnectionType.Anonymous.ToString(), out var parsedConnType) ? parsedConnType : ConnectionType.Anonymous;
                    _config.UserName = dictParams.TryGetValue("UserName", out var userName) ? userName : string.Empty;
                    _config.Password = dictParams.TryGetValue("Password", out var password) ? password : string.Empty;
                    _config.CertificatePath = dictParams.TryGetValue("CertificatePath", out var certPath) ? certPath : string.Empty;
                    _config.CertificatePwd = dictParams.TryGetValue("CertificatePwd", out var certPwd) ? certPwd : string.Empty;
                    _config.CertificateName = dictParams.TryGetValue("CertificateName", out var certificateName) ? certificateName : "opc";
                    var _nodeQualityType = dictParams.TryGetValue("NodeQualityType", out var nodeQualityType) ? nodeQualityType : "good";
                    _config.NodeQualityType = _nodeQualityType.Split(",").Select(x => Enum.TryParse<NodeQualityType>(x, out var _type) ? _type : NodeQualityType.Good).Distinct().ToList();
                    if (dictParams.TryGetValue("Polling", out var polling))
                    {
                        _config.Polling = polling.ToLower() == "true" ? true : false;
                    }
                    _ = int.TryParse(item.communication_time_param, out var _time);
                    _config.PollingSleepTime = _time == 0 ? _sleepInterval : _time;

                    OnLoaded();
                }
            }
            catch (Exception ex)
            {
                DataCenter.WriteLogFile("OPC UA Init 初始化出现错误" + ex.ToString());
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
            try
            {
                //TODO 这里可以写于设备连接的具体代码了。根据_connectionConfig连接参数，去创建自己的连接对象。
                OpcUaManager.Instance.CreateClientSession(_config);
                //返回默认值
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public override CommunicationState GetData(CEquipBase pEquip)
        {
            try
            {
                var opcSessionStatus = OpcUaManager.Instance.GetOpcSessionStatus(_config.ServerUrl);
                if (opcSessionStatus)
                {
                    var ycNodeIds = this.ycprows?.Where(m => m != null).Select(m => m.main_instruction) ?? Enumerable.Empty<string>();
                    var yxnodeIds = this.yxprows?.Where(m => m != null).Select(m => m.main_instruction) ?? Enumerable.Empty<string>();

                    var combinedNodeIds = ycNodeIds.Concat(yxnodeIds).Distinct().ToArray();
                    OpcUaManager.Instance.AddSubscription(_config.ServerUrl, combinedNodeIds);
                    _nodeIdValues = OpcUaManager.Instance.GetNodeIdValues(_config.ServerUrl, combinedNodeIds);
                    return base.GetData(pEquip);
                }
                else
                {
                    this.Sleep(10);
                    return CommunicationState.fail;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return CommunicationState.fail;
            }
        }

        public override bool GetYC(YcpTableRow r)
        {
            if (_nodeIdValues == null) return true;
            string main = r.main_instruction;

            string minor = r.minor_instruction;
            try
            {
                if (_nodeIdValues.ContainsKey(main))
                {
                    var nodeIdObj = _nodeIdValues[main];
                    if (nodeIdObj.Value == null)
                    {
                        SetYCData(r, "***");
                        return true;
                    }
                    if (!string.IsNullOrEmpty(minor))
                    {
                        var prop = nodeIdObj.GetType().GetProperty(minor);
                        if (prop != null)
                        {
                            SetYCData(r, prop.GetValue(nodeIdObj) ?? "");
                            return true;
                        }
                    }
                    SetYCData(r, nodeIdObj.Value);
                }
                else
                {
                    SetYCData(r, "");
                }
            }
            catch (Exception ex)
            {
                DataCenter.WriteLogFile($"GWOpcUAStandard.STD Error[{_nodeIdValues}]:{ex.Message}", LogType.Error);
                SetYCData(r, "***");
            }
            return true;
        }

        public override bool GetYX(YxpTableRow r)
        {
            if (_nodeIdValues == null) return false;
            string main = r.main_instruction;
            string minor = r.minor_instruction;
            try
            {
                if (_nodeIdValues.ContainsKey(main))
                {
                    var nodeIdObj = _nodeIdValues[main];
                    if (nodeIdObj.Value == null)
                    {
                        SetYXData(r, "***");
                        return true;
                    }
                    if (!string.IsNullOrEmpty(minor))
                    {
                        var prop = nodeIdObj.GetType().GetProperty(minor);
                        if (prop != null)
                        {
                            SetYXData(r, prop.GetValue(nodeIdObj) ?? "");
                            return true;
                        }
                    }
                    SetYXData(r, nodeIdObj.Value);
                }
                else
                {
                    SetYXData(r, "***");
                }
            }
            catch (Exception ex)
            {
                DataCenter.WriteLogFile($"GWOpcUAStandard.STD Error[{_nodeIdValues}]:{ex.Message}", LogType.Error);
                SetYXData(r, "***");
            }
            return true;
        }

        public override bool SetParm(string mainInstruct, string minorInstruct, string value)
        {
            var responseModel = new EquipSetResponseModel();
            string logMsg = string.Empty;

            if (mainInstruct.ToLower().Equals("readall"))
            {
                responseModel = OpcUaManager.Instance.ReadAllNodes(_config.ServerUrl);
            }
            else
            {

                DataTypeEnum dataTypeEnum = DataTypeEnum.None;
                if (string.IsNullOrWhiteSpace(minorInstruct))
                {
                    responseModel.Fail("操作参数不能为空，请按使用文档填写");
                }
                else if (!Enum.TryParse(minorInstruct, out dataTypeEnum))
                {
                    responseModel.Fail("操作参数填写不对，请按使用文档填写");
                }

                if (responseModel.Code != 200)
                {
                    //给当前设置点赋值响应内容，用于北向转发时告知设备实际执行结果
                    logMsg = string.Format("命令下发参数,设备号：{0}，mainInstruct：{1}，minorInstruct：{2}，value：{3},下发执行结果：{4}",
                    this.equipitem.iEquipno, mainInstruct, minorInstruct, value, responseModel.Message);
                    DataCenter.WriteLogFile(logMsg, LogType.Error);
                    return false;
                }

                if (DataConverter.Parse(dataTypeEnum, value, out object sendValue, out string msg))
                {
                    responseModel = OpcUaManager.Instance.WriteValueAsync(_config.ServerUrl, mainInstruct, sendValue);
                }
                else
                {
                    responseModel.Fail(msg);
                }
            }

            //给当前设置点赋值响应内容，用于北向转发时告知设备实际执行结果
            logMsg = string.Format("命令下发参数,设备号：{0}，mainInstruct：{1}，minorInstruct：{2}，value：{3},下发执行结果：{4}",
            this.equipitem.iEquipno, mainInstruct, minorInstruct, value, responseModel.Message);
            DataCenter.WriteLogFile(logMsg, LogType.Error);

            //根据设备执行状态，返回状态，对于发布订阅模式可直接返回true，在相关地方做好日志记录即可。
            if (responseModel.Code == 200) return true;
            else return false;
        }
    }
}
