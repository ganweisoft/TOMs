// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using GWOpcUAStandard.STD.Model;
using Opc.Ua;
using Opc.Ua.Client;

public class OpcUaSession
{
    public Session Session { get; private set; }
    public HashSet<string> MonitoredItems { get; private set; }
    public Dictionary<string, DataValue> NodeIdValues { get; private set; }
    public ConnectionConfig ConnectionConfig { get; private set; }

    public OpcUASessionStatus StatusInfo { get; private set; }

    public Subscription Subscription { get; private set; }
    public OpcUaSession(ConnectionConfig connectionConfig, Session session)
    {
        ConnectionConfig = connectionConfig;
        Session = session ?? throw new ArgumentNullException(nameof(session));
        MonitoredItems = new HashSet<string>();
        NodeIdValues = new Dictionary<string, DataValue>();
        StatusInfo = new();
        Session.KeepAlive += OnKeepAlive;
        //创建完连接后，创建一个订阅。
        Subscription = CreateSubscription();
    }
    private Subscription CreateSubscription()
    {
        var subscription = Session.Subscriptions.FirstOrDefault(m => m.DisplayName == ConnectionConfig.ServerUrl);
        if (subscription != null)
        {
            subscription.Delete(true);
            subscription = null;
        }

        try
        {
            subscription = new Subscription(Session.DefaultSubscription);
            subscription.PublishingEnabled = true;
            subscription.PublishingInterval = 500;
            subscription.KeepAliveCount = uint.MaxValue;
            subscription.LifetimeCount = uint.MaxValue;
            subscription.MaxNotificationsPerPublish = 1000;
            subscription.Priority = 100;
            subscription.DisplayName = ConnectionConfig.ServerUrl;
            subscription.TimestampsToReturn = TimestampsToReturn.Both;
            Session.AddSubscription(subscription);
            subscription.Create();
        }
        catch (ServiceResultException ex) when (ex.StatusCode == StatusCodes.BadTooManySubscriptions)
        {
            DataCenter.WriteLogFile($"Error: Too many subscriptions. Cannot create new subscription.count:{Session.Subscriptions.Count()}");
            return null;
        }
        catch (Exception ex)
        {
            DataCenter.WriteLogFile("CreateSubscription 出现错误" + ex.Message);
        }
        return subscription;
    }

    public void ReSetOpcUaSession(Session resession)
    {
        // 清空现有订阅并重新订阅
        MonitoredItems.Clear();
        NodeIdValues.Clear();
        Session = resession;
        Subscription = CreateSubscription();
        StatusInfo = new();
        // 这里可以重新订阅之前的节点
        // 例如: AddSubscription(之前的节点列表, 之前的设备号);
    }

    /// <summary>
    /// 添加当前连接的所有订阅
    /// </summary>
    /// <param name="serverUrl">opc服务地址</param>
    /// <param name="nodeIds">NodeId集合</param>
    /// <param name="equipNo">设备号</param>
    public void AddAllSubscription(string[] nodeIds)
    {
        try
        {
            // AddSubscriptionToMonitoredItems(opcUaSession.MonitoredItems, nodeIds);
            foreach (var nodeId in nodeIds)
            {
                // 使用 equipNo 作为 DisplayName
                if (!MonitoredItems.Contains(nodeId))
                {
                    try
                    {
                        var monitoredItem = new MonitoredItem
                        {
                            StartNodeId = new NodeId(nodeId),
                            AttributeId = Attributes.Value,
                            DisplayName = nodeId, // 设置 DisplayName 为设备号和 NodeId 的组合
                            SamplingInterval = 1000
                        };
                        monitoredItem.Notification += OnNotification;
                        Subscription?.AddItem(monitoredItem);
                        MonitoredItems.Add(nodeId);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            Subscription?.ApplyChanges();
        }
        catch (Exception ex)
        {
            DataCenter.WriteLogFile($"AddSubscription ex：{ex}");
        }
    }

    /// <summary>
    /// 添加订阅
    /// </summary>
    /// <param name="serverUrl">opc服务地址</param>
    /// <param name="nodeIds">NodeId集合</param>
    /// <param name="equipNo">设备号</param>
    public void AddSubscription(string[] nodeIds)
    {
        try
        {
            // AddSubscriptionToMonitoredItems(opcUaSession.MonitoredItems, nodeIds);
            bool hasAddMonitoredItems = false;
            // 检查是否已有有效的订阅
            // 使用一个新的订阅对象
            foreach (var nodeId in nodeIds)
            {
                // 使用 equipNo 作为 DisplayName
                if (!MonitoredItems.Contains(nodeId))
                {
                    try
                    {
                        var monitoredItem = new MonitoredItem
                        {
                            StartNodeId = new NodeId(nodeId),
                            AttributeId = Attributes.Value,
                            DisplayName = nodeId, // 设置 DisplayName 为设备号和 NodeId 的组合
                            SamplingInterval = 1000
                        };
                        monitoredItem.Notification += OnNotification;
                        Subscription?.AddItem(monitoredItem);
                        MonitoredItems.Add(nodeId);
                        hasAddMonitoredItems = true;
                        //DataCenter.WriteLogFile($"Added MonitoredItem for NodeId: {nodeId}.{serverUrl}", LogType.Debug);
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                }
            }
            if (hasAddMonitoredItems)
            {
                Subscription?.ApplyChanges();
                // 主动获取一次数据
                var nodeIdKeysSet = NodeIdValues.Keys; // 一次性生成 HashSet
                var itemsNotInNodeIdValues = MonitoredItems
                    .Where(item => !nodeIdKeysSet.Contains(item))
                    .ToHashSet();
                foreach (var nodeId in nodeIds)
                {
                    itemsNotInNodeIdValues.Add(nodeId);
                }
                ReadNodeValueByNodes(itemsNotInNodeIdValues.ToArray());
            }
        }
        catch (Exception ex)
        {
            DataCenter.WriteLogFile($"AddSubscription ex：{ex}");
        }
    }
    /// <summary>
    /// 连接保持监听
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnKeepAlive(object sender, KeepAliveEventArgs e)
    {
        try
        {
            if (!ServiceResult.IsBad(e.Status))
            {
                //如果连接正常，设置为在线
                StatusInfo.SetOnceKeepAliveTime(); ;
            }
            //else
            //{
            //    DataCenter.WriteLogFile($"心跳状态异常[{string.Join(",", ConnectionConfig.ServerUrl)}]", LogType.Debug);
            //}
        }
        catch (Exception ex)
        {
            DataCenter.WriteLogFile("OnKeepAlive 出现异常" + ex.ToString());
        }
    }

    /// <summary>
    /// 订阅回调方法
    /// </summary>
    /// <param name="item"></param>
    /// <param name="e"></param>
    /// <param name="opcUaSession"></param>
    private void OnNotification(MonitoredItem item, MonitoredItemNotificationEventArgs e)
    {
        var nodeId = item.DisplayName;
        if (e.NotificationValue is MonitoredItemNotification monitoredItemNotification)
        {
            var value = monitoredItemNotification.Value;
            NodeIdValues[nodeId] = value;
            StatusInfo.SetOnceKeepAliveTime();
            // DataCenter.WriteLogFile($"收到节点订阅消息 [{nodeId} : {value}]", LogType.Debug);
        }
    }

    private int _isPolling = 0; // 使用整数标志代替布尔值

    public async Task PullData()
    {
        if (Interlocked.CompareExchange(ref _isPolling, 1, 0) != 0) return;
        try
        {
            ReadAllNodes();
        }
        catch (Exception ex)
        {
            DataCenter.WriteLogFile($"[{ConnectionConfig.ServerUrl}]主动拉取时出现异常:{ex.ToString()}");
        }
        finally
        {
            await Task.Delay(ConnectionConfig.PollingSleepTime);
            Interlocked.Exchange(ref _isPolling, 0); // 重置标志
        }
    }

    public void ReadAllNodes()
    {
        if (MonitoredItems == null || !MonitoredItems.Any())
        {
            return;
        }
        // Create a list of nodes to read
        var nodesToRead = new ReadValueIdCollection();
        foreach (var nodeId in MonitoredItems)
        {
            nodesToRead.Add(new ReadValueId
            {
                NodeId = new NodeId(nodeId),
                AttributeId = Attributes.Value
            });
        }
        // 读取当前的值
        Session.Read(
            null,
            0,
            TimestampsToReturn.Neither,
            nodesToRead,
            out DataValueCollection results,
            out Opc.Ua.DiagnosticInfoCollection diagnosticInfos);

        ClientBase.ValidateResponse(results, nodesToRead);
        ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);

        if (results != null && results.Count > 0)
        {
            StatusInfo.SetOnceKeepAliveTime();
        }

        for (int i = 0; i < nodesToRead.Count; i++)
        {
            var nodeId = nodesToRead[i].NodeId.ToString();
            var statusCode = results[i].StatusCode;
            if (FilterNodeQualityType(ConnectionConfig.NodeQualityType, statusCode))
            {
                var value = results[i].Value;
                NodeIdValues[nodeId] = results[i];
                // DataCenter.WriteLogFile($"[{ConnectionConfig.ServerUrl}]主动拉取节点 [{nodeId} : {value}]", LogType.Debug);
            }
            else
            {
                DataCenter.WriteLogFile($"[{ConnectionConfig.ServerUrl}]主动拉取节点状态码不正确 [{nodeId} : {statusCode}]", LogType.Debug);
            }
        }
    }

    public void ReadNodeValueByNodes(string[] nodes)
    {
        if (nodes == null || !nodes.Any())
        {
            return;
        }
        var nodesToRead = new ReadValueIdCollection();
        foreach (var nodeId in nodes)
        {
            try
            {
                nodesToRead.Add(new ReadValueId
                {
                    NodeId = new NodeId(nodeId),
                    AttributeId = Attributes.Value
                });
            }
            catch (Exception)
            {
                continue;
            }
        }
        if (nodesToRead.Count <= 0)
        {
            return;
        }
        // 读取当前的值
        Session.Read(
            null,
            0,
            TimestampsToReturn.Neither,
            nodesToRead,
            out DataValueCollection results,
            out Opc.Ua.DiagnosticInfoCollection diagnosticInfos);

        ClientBase.ValidateResponse(results, nodesToRead);
        ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);

        for (int i = 0; i < nodesToRead.Count; i++)
        {
            var nodeId = nodesToRead[i].NodeId.ToString();
            var statusCode = results[i].StatusCode;
            if (FilterNodeQualityType(ConnectionConfig.NodeQualityType, statusCode))
            {
                var value = results[i].Value;
                NodeIdValues[nodeId] = results[i];
                // DataCenter.WriteLogFile($"[{ConnectionConfig.ServerUrl}]主动拉取节点 [{nodeId} : {value}]", LogType.Debug);
            }
            else
            {
                DataCenter.WriteLogFile($"[{ConnectionConfig.ServerUrl}]主动拉取节点状态码不正确 [{nodeId} : {statusCode}]", LogType.Debug);
            }
        }
    }
    public bool FilterNodeQualityType(List<NodeQualityType> types, StatusCode code)
    {
        foreach (NodeQualityType type in types)
        {
            if (type == NodeQualityType.Good && StatusCode.IsGood(code))
            {
                return true;
            }
            else if (type == NodeQualityType.Uncertain && StatusCode.IsUncertain(code))
            {
                return true;
            }
            else if (type == NodeQualityType.Bad && StatusCode.IsBad(code))
            {
                return true;
            }
        }
        return false;
    }

    public void DisposeSession()
    {
        try
        {
            // 取消订阅并清空监控项
            if (Session != null && Session.Subscriptions != null)
            {
                foreach (var subscription in Session.Subscriptions)
                {
                    subscription.Delete(true); // 删除订阅
                }
            }
            MonitoredItems.Clear();
            NodeIdValues.Clear();
            StatusInfo = null;
            // 关闭会话
            if (Session != null)
            {
                Session.KeepAlive -= OnKeepAlive;
                Session?.Close();
                Session?.Dispose();
                DataCenter.WriteLogFile($"Session for {ConnectionConfig.ServerUrl} has been disposed.", LogType.Debug);
            }
            // 设置状态为离线
            // SetOffline();
        }
        catch (Exception ex)
        {
            DataCenter.WriteLogFile($"Failed to dispose session for {ConnectionConfig.ServerUrl}: {ex.Message}");
        }
    }
}
