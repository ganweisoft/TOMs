using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Dapr.Client;

namespace GWDataCenter.MqService;

public class MqttProvider
{
    private const string EVT_TOPIC_NAME = "event";
    private const string TOPIC_NAME = "report";
    private const string PUBSUB_NAME = "open_datacenter_pubsub";
    private MqttProvider() { }
    public static readonly MqttProvider Instance = new MqttProvider();
    private DaprClient _daprClient;
    public readonly ConcurrentDictionary<int, Equip> EquipTableRows = new ();
    private string _gateWayId;

    public void Init()
    {
        // Service App ID
        _gateWayId = Environment.GetEnvironmentVariable("InstanceId");

        var aspOptionBuilder = new DaprClientBuilder();
        
        // Endpoint
        var mqServer = Environment.GetEnvironmentVariable("MqServer");
        if (!string.IsNullOrEmpty(mqServer))
        {
            aspOptionBuilder.UseHttpEndpoint(mqServer);
        }
        
        // Token
        var username = Environment.GetEnvironmentVariable("MqUsername");
        var password = Environment.GetEnvironmentVariable("MqPassword");
        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            aspOptionBuilder.UseDaprApiToken(
                Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("Basic " + username + ":" + password)));
        }
        
        _daprClient = aspOptionBuilder.Build();
        this.GetEquipsAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        EquipInited?.Invoke(this, EventArgs.Empty);
    }

    private async Task GetEquipsAsync()
    {
        var responses = await _daprClient.InvokeMethodAsync<object, List<MqMessage>>(HttpMethod.Post, _gateWayId, "/equip", new
        {
            categary = "CSHARP"
        });
        if (responses == null)
        {
            throw new InvalidCastException(nameof(MqMessage));
        }

        var response = responses.FirstOrDefault();
        if (response != null && response.FlowType == 0 && response.Equips.Count > 0)
        {
            foreach (var equipTableRow in response.Equips)
            {
                EquipTableRows.AddOrUpdate(equipTableRow.EquipNo, equipTableRow, (_, _) => equipTableRow);
            }
                
            EquipInited?.Invoke(this, EventArgs.Empty);
        }
    }

    public async Task PublishYcRtValueAsync(MqRtValueMessage msg)
    {
        try
        {
            if (msg == null)
            {
                throw new ArgumentNullException(nameof(msg));
            }
        
            msg.DataType = 1;
            await _daprClient.PublishEventAsync(PUBSUB_NAME, TOPIC_NAME, msg);
        }
        catch (Exception e)
        {
            DataCenter.WriteLogFile($"publish yc realtime msg exception: {e}");
        }
    }
    
    public async Task PublishYxRtValueAsync(MqRtValueMessage msg)
    {
        if (msg == null)
        {
            throw new ArgumentNullException(nameof(msg));
        }

        msg.DataType = 2;
        await _daprClient.PublishEventAsync(PUBSUB_NAME, TOPIC_NAME, msg);
    }
    
    public async Task PublishEvtValueAsync(MqEvtMessage msg)
    {
        await _daprClient.PublishEventAsync(PUBSUB_NAME, EVT_TOPIC_NAME, msg);
    }
    
    
    public async Task PublishRtStateAsync(MqRtStateMessage msg)
    {
        try
        {
            await _daprClient.PublishEventAsync(PUBSUB_NAME, TOPIC_NAME, msg);
        }
        catch (Exception e)
        {
            DataCenter.WriteLogFile($"publish state realtime msg exception: {e}");
        }
    }

    public event EventHandler EquipInited;
}