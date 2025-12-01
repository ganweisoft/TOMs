// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using GWOpcUAStandard.STD;
using GWOpcUAStandard.STD.Model;
using GWOpcUAStandard.STD.Service;
using Newtonsoft.Json;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

public class OpcUaManager
{
    private static readonly Lazy<OpcUaManager> instance = new Lazy<OpcUaManager>(() => new OpcUaManager());
    /// <summary>
    /// OPC连接对象池
    /// </summary>
    private ConcurrentDictionary<string, Lazy<OpcUaSession>> opcUaSessions = new();
    private ConcurrentDictionary<string, bool> opcUaSessionsState = new();

    private Dictionary<string, DateTime> connectFailServers = new();

    /// <summary>
    /// 重试时间间隔30秒
    /// </summary>
    private int reTryTime = 30;

    private OpcUaManager() { }

    public static OpcUaManager Instance
    {
        get { return instance.Value; }
    }

    public void CreateClientSession(ConnectionConfig connectionConfig)
    {
        // 判断当前连接是否出现连接失败的问题。
        if (connectFailServers.ContainsKey(connectionConfig.ServerUrl))
        {
            //判断上次失败的时间是否超过了30秒，如果未超过30秒就不在重新建立连接，避免其他正常的连接阻塞
            var lastErrorDate = connectFailServers[connectionConfig.ServerUrl];
            if (lastErrorDate > DateTime.Now) return;
        }
        var opcUaSessionLazy = opcUaSessions.GetOrAdd(connectionConfig.ServerUrl, (key) => new Lazy<OpcUaSession>(() => CreateNewSession(connectionConfig)));
        var opcUaSession = opcUaSessionLazy.Value;
        if (opcUaSession == null)
        {
            opcUaSessions.Remove(connectionConfig.ServerUrl, out _);
            opcUaSessionsState[connectionConfig.ServerUrl] = false;
            WriteState();
            DataCenter.WriteLogFile($"因为opcUaSession为空，移除了连接[{connectionConfig.ServerUrl}]");
            return;
        }
        // 不为空则更新配置信息
        else
        {
            if (!HotReloadCompare(opcUaSession.ConnectionConfig, connectionConfig))
            {
                DataCenter.WriteLogFile($"连接配置变化了，正在进行热重载[{connectionConfig.ServerUrl}],旧配置：{JsonConvert.SerializeObject(opcUaSession.ConnectionConfig)}；新配置：{JsonConvert.SerializeObject(connectionConfig)}");
                opcUaSession.DisposeSession();
                opcUaSessionsState[connectionConfig.ServerUrl] = false;
                WriteState();
                opcUaSessions.Remove(connectionConfig.ServerUrl, out _);
                return;
            }
        }
        if (!opcUaSession.StatusInfo.GetTimeOutStatus())
        {
            DataCenter.WriteLogFile($"因为状态异常，释放并移除了连接[{connectionConfig.ServerUrl}],[连接创建时间：{opcUaSession.StatusInfo.SessionCreateTime}],[最后一次KeepAlive：{opcUaSession.StatusInfo.LastKeepAliveTime}]");
            opcUaSession.DisposeSession();
            opcUaSessionsState[connectionConfig.ServerUrl] = false;
            WriteState();
            opcUaSessions.Remove(connectionConfig.ServerUrl, out _);
        }
        try
        {
            if (opcUaSession.Session == null)
            {
                DataCenter.WriteLogFile($"[{connectionConfig.ServerUrl}] opcUaSession.Session 为空，进入了异常分支，需排查");
                opcUaSession.DisposeSession();
                opcUaSessionsState[connectionConfig.ServerUrl] = false;
                WriteState();
                opcUaSessions.Remove(connectionConfig.ServerUrl, out _);
            }
        }
        catch (Exception ex)
        {
            DataCenter.WriteLogFile($"GWOpcUAStandard.STD CreateClientSession Eror: {connectionConfig.ServerUrl}: {ex.ToString()}");
        }
    }
    /// <summary>
    /// 添加连接对接
    /// </summary>
    /// <param name="connectionConfig"></param>
    public OpcUaSession CreateNewSession(ConnectionConfig connectionConfig)
    {
        try
        {
            // 使用 Result 获取异步结果，但最好使用 async/await 处理异步操作
            var session = ConnectToServerAsync(connectionConfig).Result;
            if (session == null)
            {
                connectFailServers[connectionConfig.ServerUrl] = DateTime.Now.AddSeconds(reTryTime);
                return null;
            }
            DataCenter.WriteLogFile($"{connectionConfig.ServerUrl}创建了连接", LogType.Debug);
            var opcUaSession = new OpcUaSession(connectionConfig, session);
            var allEquipNo = StationItem.db_Eqp.Where(m => m.equip_addr == connectionConfig.ServerUrl).Select(m => m.equip_no).ToList();
            var ycNodeIds = StationItem.db_Ycp.Where(m => allEquipNo.Contains(m.equip_no)).Select(m => m.main_instruction).ToList();
            var yxnodeIds = StationItem.db_Yxp.Where(m => allEquipNo.Contains(m.equip_no)).Select(m => m.main_instruction).ToList();

            var combinedNodeIds = ycNodeIds.Concat(yxnodeIds).Distinct().ToArray();

            //获取一次所有节点的数据
            opcUaSession.ReadNodeValueByNodes(combinedNodeIds);

            //订阅所有节点数据
            opcUaSession.AddAllSubscription(combinedNodeIds);

            DataCenter.WriteLogFile($"Server {connectionConfig.ServerUrl} Connect Successfully.", LogType.Debug);

            return opcUaSession;
        }
        catch (Exception ex)
        {
            DataCenter.WriteLogFile($"GWOpcUAStandard.STD Failed to connect to server {connectionConfig.ServerUrl}: {ex.Message}");
            connectFailServers[connectionConfig.ServerUrl] = DateTime.Now.AddSeconds(reTryTime);
            return null;
        }
    }
    //private bool PortTest(string serviceUrl, int timeout = 500)
    //{
    //    // 去掉协议部分
    //    string withoutProtocol = serviceUrl.Substring("opc.tcp://".Length);

    //    // 分离主机名/IP、端口和路径
    //    var parts = withoutProtocol.Split(new[] { '/', ':' }, 3, StringSplitOptions.RemoveEmptyEntries);

    //    string host = parts[0]; // 主机名或 IP
    //    int port = parts.Length > 1 && int.TryParse(parts[1], out var parsedPort) ? parsedPort : 0;
    //    try
    //    {
    //        var ipAddresses = Dns.GetHostAddresses(host);
    //        using (var client = new TcpClient())
    //        {
    //            var result = client.BeginConnect(ipAddresses[0].ToString(), port, null, null);
    //            var success = result.AsyncWaitHandle.WaitOne(timeout);

    //            if (!success)
    //                return false;

    //            client.EndConnect(result); // 连接成功
    //            return true;
    //        }
    //    }
    //    catch
    //    {
    //        return false; // 捕获异常表示端口未监听
    //    }
    //}

    public void AddSubscriptionToMonitoredItems(HashSet<string> set, string[] nodeIds)
    {
        foreach (var nodeId in nodeIds)
        {
            if (!set.Contains(nodeId))
            {
                set.Add(nodeId);
            }
        }
    }
    /// <summary>
    /// 命令下发
    /// </summary>
    /// <param name="serverUrl"></param>
    /// <param name="nodeId"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public EquipSetResponseModel WriteValueAsync(string serverUrl, string nodeId, object value)
    {
        EquipSetResponseModel equipSet = new EquipSetResponseModel();
        try
        {
            if (opcUaSessions.TryGetValue(serverUrl, out Lazy<OpcUaSession> opcUaSessionLazy) && opcUaSessionLazy.Value != null)
            {
                var opcUaSession = opcUaSessionLazy.Value;
                var writeValue = new WriteValue
                {
                    NodeId = NodeId.Parse(nodeId),
                    AttributeId = Attributes.Value,
                    Value = new DataValue
                    {
                        Value = value,
                        StatusCode = StatusCodes.Good,
                        ServerTimestamp = DateTime.UtcNow,
                        SourceTimestamp = DateTime.UtcNow
                    }
                };
                WriteValueCollection valuesToWrite = new WriteValueCollection { writeValue };

                opcUaSession.Session.Write(null, valuesToWrite, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos);

                ClientBase.ValidateResponse(results, valuesToWrite);
                ClientBase.ValidateDiagnosticInfos(diagnosticInfos, valuesToWrite);

                if (StatusCode.IsGood(results[0]))
                {
                    equipSet.Fail($"执行成功");
                    return equipSet;
                }
                else
                {
                    equipSet.Fail($"执行失败，返回Code为: {results[0].Code}");
                    return equipSet;
                }
            }
            else
            {

                equipSet.Fail("执行失败，连接未找到");
                return equipSet;
            }
        }
        catch (Exception ex)
        {
            equipSet.Fail($"执行失败，异常信息：{ex.Message}");
            return equipSet;
        }
    }

    public EquipSetResponseModel ReadAllNodes(string serverUrl)
    {
        EquipSetResponseModel equipSet = new EquipSetResponseModel();
        if (opcUaSessions.TryGetValue(serverUrl, out Lazy<OpcUaSession> opcUaSessionLazy) && opcUaSessionLazy.Value != null)
        {
            var opcUaSession = opcUaSessionLazy.Value;
            opcUaSession.ReadAllNodes();
            equipSet.Ok();
        }
        else
        {
            equipSet.Fail("执行失败");
        }
        return equipSet;
    }

    /// <summary>
    /// 获取链接状态
    /// </summary>
    /// <param name="serverUrl"></param>
    /// <param name="nodeId"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool GetOpcSessionStatus(string serverUrl)
    {
        try
        {
            if (opcUaSessions.TryGetValue(serverUrl, out Lazy<OpcUaSession> opcUaSessionLazy) && opcUaSessionLazy.Value != null)
            {
                var opcUaSession = opcUaSessionLazy.Value;
                var state = !opcUaSession.Session.KeepAliveStopped && (opcUaSession.StatusInfo?.GetTimeOutStatus() ?? false);
                if (opcUaSession.ConnectionConfig.Polling && state)
                {
                    opcUaSession.PullData();
                }
                return state;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            DataCenter.WriteLogFile($"Exception occurred while get session state: {ex.Message}");
            return false;
        }
    }
    public void AddSubscription(string serverUrl, string[] nodeIds)
    {
        try
        {
            if (opcUaSessions.TryGetValue(serverUrl, out Lazy<OpcUaSession> opcUaSessionLazy) && opcUaSessionLazy.Value != null)
            {
                var opcUaSession = opcUaSessionLazy.Value;
                opcUaSession.AddSubscription(nodeIds);
            }
        }
        catch (Exception)
        {

            throw;
        }
    }
    /// <summary>
    /// 命令下发
    /// </summary>
    /// <param name="serverUrl"></param>
    /// <param name="nodeId"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public Dictionary<string, DataValue> GetNodeIdValues(string serverUrl, string[] nodeIds)
    {
        try
        {
            if (opcUaSessions.TryGetValue(serverUrl, out Lazy<OpcUaSession> opcUaSessionLazy) && opcUaSessionLazy.Value != null)
            {
                var opcUaSession = opcUaSessionLazy.Value;
                // 过滤并转换为 Dictionary<string, DataValue>
                var result = new Dictionary<string, DataValue>();
                foreach (var nodeId in nodeIds)
                {
                    if (opcUaSession.NodeIdValues.ContainsKey(nodeId))
                    {
                        var val = opcUaSession.NodeIdValues[nodeId];
                        if (val != null)
                        {
                            result.Add(nodeId, val);
                        }
                    }
                }
                return result;
            }
            else
            {
                DataCenter.WriteLogFile($"Session for server {serverUrl} not found.", LogType.Debug);
                return new Dictionary<string, DataValue>();
            }
        }
        catch (Exception ex)
        {
            DataCenter.WriteLogFile($"Exception occurred while getting OPC session node ID values: {ex.Message}");
            return new Dictionary<string, DataValue>();
        }
    }

    /// <summary>
    /// 创建新的OPC连接对象
    /// </summary>
    /// <param name="connectionConfig"></param>
    /// <returns></returns>
    private async Task<Session> ConnectToServerAsync(ConnectionConfig connectionConfig)
    {
        try
        {
            string dllPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            if (string.IsNullOrWhiteSpace(dllPath))
            {
                throw new Exception("无法获取dll路径");
            }
            string projectName = Assembly.GetExecutingAssembly().GetName().Name ?? string.Empty;
            if (string.IsNullOrWhiteSpace(projectName))
            {
                throw new Exception("无法获取dll项目名称");
            }
            string pkiPath = Path.Combine(dllPath, projectName, "pki");
            CreatePath(pkiPath);
            //构建证书目录
            string storePath_Identifier = Path.Combine(pkiPath, "own");
            CreatePath(storePath_Identifier);
            string storePath_Trust = Path.Combine(pkiPath, "trusted");
            CreatePath(storePath_Trust);
            string storePath_Reject = Path.Combine(pkiPath, "rejected");
            CreatePath(storePath_Reject);
            string OpcUaName = connectionConfig.CertificateName;
            var certificateValidator = new CertificateValidator();
            certificateValidator.CertificateValidation += (sender, eventArgs) =>
            {
                if (ServiceResult.IsGood(eventArgs.Error))
                    eventArgs.Accept = true;
                else if (eventArgs.Error.StatusCode.Code == Opc.Ua.StatusCodes.BadCertificateUntrusted)
                    eventArgs.Accept = true;
                else
                    throw new Exception(string.Format("Failed to validate certificate with error code {0}: {1}", eventArgs.Error.Code, eventArgs.Error.AdditionalInfo));
            };
            var securityConfiguration = new SecurityConfiguration
            {
                AutoAcceptUntrustedCertificates = true,
                RejectSHA1SignedCertificates = false,
                MinimumCertificateKeySize = 1024,
                ApplicationCertificate = new CertificateIdentifier
                {
                    StoreType = @"Directory",
                    StorePath = storePath_Identifier,
                    SubjectName = $"CN={OpcUaName}, OU=GW, O=GW, L=CHANGSHA, S=HUNAN, C=CN"
                },
                TrustedPeerCertificates = new CertificateTrustList
                {
                    StoreType = @"Directory",
                    StorePath = storePath_Trust
                },
                //拒绝证书配置
                RejectedCertificateStore = new CertificateTrustList
                {
                    StoreType = @"Directory",
                    StorePath = storePath_Reject
                }
            };
            await certificateValidator.Update(securityConfiguration);
            var application = new ApplicationInstance
            {
                ApplicationName = OpcUaName,
                ApplicationType = ApplicationType.Client,
                // ConfigSectionName = "Opc.Ua.Client",
                ApplicationConfiguration = new ApplicationConfiguration
                {
                    ApplicationName = OpcUaName,
                    ApplicationUri = Utils.Format($"urn:{System.Net.Dns.GetHostName()}:{OpcUaName}"),
                    ApplicationType = ApplicationType.Client,
                    CertificateValidator = certificateValidator,
                    ServerConfiguration = new ServerConfiguration
                    {
                        MaxSubscriptionCount = 100000,
                        MaxMessageQueueSize = 1000000,
                        MaxNotificationQueueSize = 1000000,
                        MaxPublishRequestCount = 10000000,
                    },

                    SecurityConfiguration = securityConfiguration,

                    TransportQuotas = new TransportQuotas
                    {
                        OperationTimeout = 6000000,
                        MaxStringLength = int.MaxValue,
                        MaxByteStringLength = int.MaxValue,
                        MaxArrayLength = 65535,
                        MaxMessageSize = 419430400,
                        MaxBufferSize = 65535,
                    },
                    ClientConfiguration = new ClientConfiguration
                    {
                        DefaultSessionTimeout = 60000,
                    },
                    DisableHiResClock = true
                }
            };
            await application.ApplicationConfiguration.Validate(ApplicationType.Client);
            var appConfig = application.ApplicationConfiguration;

            CheckOrCreate(application);

            var endpointConfiguration = EndpointConfiguration.Create(appConfig);
            var selectedEndpoint = CoreClientUtils.SelectEndpoint(connectionConfig.ServerUrl, useSecurity: false);
            var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);

            UserIdentity userIdentity = connectionConfig.ConnectionType switch
            {
                ConnectionType.Anonymous => new UserIdentity(),
                ConnectionType.UserName => new UserIdentity(connectionConfig.UserName, connectionConfig.Password),
                ConnectionType.Certificate => new UserIdentity(LoadCertificate(connectionConfig.CertificatePath, connectionConfig.CertificatePwd)),
                _ => throw new ArgumentException($"Unsupported connection type: {connectionConfig.ConnectionType}")
            };
            DataCenter.WriteLogFile($"GWOpcUAStandard.STD : 正在通过{connectionConfig.ConnectionType}连接到{connectionConfig.ServerUrl} ...", LogType.Debug);
            var session = await Session.Create(
                appConfig,
                endpoint,
                false,
                "OPCUAClient",
                60000,
                userIdentity,
                null
            );
            if (session != null && session.Connected)
            {
                session.KeepAliveInterval = 5000;
                opcUaSessionsState[connectionConfig.ServerUrl] = true;
                WriteState();
                DataCenter.WriteLogFile($"GWOpcUAStandard.STD : 连接{connectionConfig.ServerUrl} 成功！", LogType.Debug);
            }
            DataCenter.WriteLogFile($"Connected to OPC UA server: {selectedEndpoint}", LogType.Debug);
            return session;
        }
        catch (Exception ex)
        {
            DataCenter.WriteLogFile($"Failed to connect to OPC UA server: {connectionConfig.ServerUrl}. Exception: {ex.Message}");
            return null; // 返回 null 或者抛出异常，具体看你的需求
        }
    }

    private void CreatePath(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
    /// <summary>
    /// 加载证书
    /// </summary>
    /// <param name="certificatePath"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private X509Certificate2 LoadCertificate(string certificatePath, string password = null)
    {
        if (string.IsNullOrEmpty(certificatePath))
        {
            throw new ArgumentException("Certificate path is required for certificate authentication.");
        }
        return new X509Certificate2(certificatePath, password);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    private void CheckOrCreate(ApplicationInstance application)
    {
        // 检查应用实例对象的证书
        bool check = application.CheckApplicationInstanceCertificate(false, 2048, 60).GetAwaiter().GetResult();
        if (!check)
        {
            DataCenter.WriteLogFile($"GWOpcUAStandard.STD : 证书生成异常！");
        }
    }
    private bool HotReloadCompare(ConnectionConfig param1, ConnectionConfig param2)
    {
        if (param1.ServerUrl != param2.ServerUrl)
        {
            return false;
        }
        if (param1.Polling != param2.Polling)
        {
            return false;
        }
        if (param1.PollingSleepTime != param2.PollingSleepTime)
        {
            return false;
        }
        if (param1.ConnectionType != param2.ConnectionType)
        {
            return false;
        }
        if (param1.UserName != param2.UserName)
        {
            return false;
        }
        if (param1.Password != param2.Password)
        {
            return false;
        }
        if (param1.CertificatePath != param2.CertificatePath)
        {
            return false;
        }
        if (param1.CertificatePwd != param2.CertificatePwd)
        {
            return false;
        }
        if (param1.CertificateName != param2.CertificateName)
        {
            return false;
        }
        return true;
    }

    private int _isWriteState = 0; // 使用整数标志代替布尔值
    public async Task WriteState()
    {
        if (Interlocked.CompareExchange(ref _isWriteState, 1, 0) != 0) return;
        try
        {
            var service = new LinkStateFileService();
            var stateList = opcUaSessionsState.Select(x => new LinkStateFileModel()
            {
                Code = $"OPCUA-Conn-{x.Key}",
                State = x.Value,
                StateName = x.Value == true ? "online" : "offline",
                Name = "OPCUA-ConnectionState",
                UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            }).ToList();
            await service.UpdateLinkStateFile(stateList);
        }
        catch (Exception ex)
        {
            DataCenter.WriteLogFile($"写入连接状态信息时出现异常:{ex.ToString()}");
        }
        finally
        {
            await Task.Delay(1000);
            Interlocked.Exchange(ref _isWriteState, 0); // 重置标志
        }
    }
}
