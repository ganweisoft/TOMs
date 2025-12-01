package gwdatacenter.mqservice;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.core.JsonProcessingException;
import gwdatacenter.*;
import io.dapr.client.*;
import io.dapr.client.domain.CloudEvent;
import io.dapr.client.domain.HttpExtension;
import io.dapr.client.domain.InvokeMethodRequest;
import io.dapr.exceptions.DaprErrorDetails;
import io.dapr.exceptions.DaprException;
import io.dapr.utils.TypeRef;
import reactor.core.publisher.Mono;
import sharpSystem.*;

import java.io.BufferedInputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.util.Arrays;
import java.util.HashMap;
import java.util.Properties;
import java.util.concurrent.CopyOnWriteArrayList;

public class MqttProvider
{
	private String EVT_TOPIC_NAME = "event";
	private String TOPIC_NAME = "report";
	private String PUBSUB_NAME = "open_datacenter_pubsub";
	private String CMD_TOPIC_NAME = "set";

	private MqttProvider()
	{
		try {
			// 通过Spring中的PropertiesLoaderUtils工具类进行获取
			_properties = new Properties();
			if(new File("config/config.properties").exists()){
				var inputStream = new BufferedInputStream(new FileInputStream("config/config.properties"));
				_properties.load(inputStream);
			}
		} catch (IOException e) {
			e.printStackTrace();
		}
	}

	private Thread _subThread;
	private String _gateWayId = null;
	private java.util.Properties _properties;
	public static final MqttProvider Instance = new MqttProvider();
	private DaprClientBuilder _daprClientBuilder;
	private DaprClient _grpcDaprClient;
	public final CopyOnWriteArrayList<Equip> EquipTableRows = new CopyOnWriteArrayList<>();

	/**
	 * Json serializer/deserializer.
	 */
	private static final ObjectMapper OBJECT_MAPPER = new ObjectMapper();

	public final void Init()
	{
        try {
            System.out.println(new ObjectMapper().writeValueAsString(_properties));
        } catch (JsonProcessingException e) {
            throw new RuntimeException(e);
        }

		_gateWayId = System.getenv("IoTCenterGatewayInstanceId");
		if(StringHelper.isNullOrEmpty(_gateWayId)){
			_gateWayId = _properties.getProperty("InstanceId");
		}

		var mqServer = System.getenv("IoTCenterMqttServerIp");
		if(StringHelper.isNullOrEmpty(mqServer)){
			mqServer = _properties.getProperty("MqServer");
		}

		var daprBuilder = new DaprClientBuilder();
		if(!StringHelper.isNullOrEmpty(mqServer)){
			daprBuilder.withPropertyOverride(io.dapr.config.Properties.HTTP_ENDPOINT, mqServer);
		}

		var mqGrpcServer = System.getenv("IoTCenterMqttGrpcServerIp");
		if(StringHelper.isNullOrEmpty(mqGrpcServer)){
			mqGrpcServer = _properties.getProperty("MqGRPCServer");
		}

		if(!StringHelper.isNullOrEmpty(mqGrpcServer)){
			daprBuilder.withPropertyOverride(io.dapr.config.Properties.GRPC_ENDPOINT, mqGrpcServer);
		}

		var password = System.getenv("IoTCenterMqttKey");
		if(StringHelper.isNullOrEmpty(password)){
			password = _properties.getProperty("MqPassword");
		}
		if(!StringHelper.isNullOrEmpty(password)){
			daprBuilder.withPropertyOverride(io.dapr.config.Properties.API_TOKEN, password);
		}

		_daprClientBuilder = daprBuilder;
		_grpcDaprClient = _daprClientBuilder.build();
		this.GetEquipsAsync();

		// 使用流式订阅设备命令消息
		_subThread = new Thread(new Runnable() {
			@Override
			public void run() {
				// 是一个阻塞函数，需放入线程中执行
				try (var client = _daprClientBuilder.buildPreviewClient()) {
					var subscription = client.subscribeToEvents(
							PUBSUB_NAME,
							CMD_TOPIC_NAME,
							new SubscriptionListener<>() {

								@Override
								public Mono<Status> onEvent(CloudEvent<MqCmdMessage> event) {
									var desMsgObject = event.getData();
									var equipItem = StationItem.GetEquipItemFromEquipNo(desMsgObject.getEquipNo());
									if (equipItem != null) {
										equipItem.AddSetItem(new SetItem(desMsgObject.getEquipNo(),
												desMsgObject.getMainInstruct(),
												desMsgObject.getMinorInstruct(),
												desMsgObject.getValue()));
									}

									return Mono.just(Status.SUCCESS);
								}

								@Override
								public void onError(RuntimeException exception) {
									DataCenter.WriteLogFile("Subscriber got exception: " + exception.getMessage());
								}
							},
							TypeRef.get(MqCmdMessage.class));

					subscription.awaitTermination();
				} catch (Exception exception) {
					DataCenter.WriteLogFile("Subscriber Error message: " + exception.getMessage());
				}
			}
		});
		_subThread.start();
    }

	private final void GetEquipsAsync() {
        try (var daprClient = _daprClientBuilder.build()){

			var cateObj = new HashMap<String, String>();
			cateObj.put("Categary", "JAVA");
			InvokeMethodRequest request = new InvokeMethodRequest(_gateWayId, "equip")
					.setBody(cateObj)
					.setHttpExtension(HttpExtension.POST);
			var responses = daprClient.invokeMethod(request, TypeRef.get(MqMessage[].class)).block();
			if (responses == null)
			{
				throw new NullPointerException();
			}

			MqMessage desMsgObject = Arrays.stream(responses).findFirst().orElse(null);
			if (desMsgObject != null && desMsgObject.FlowType == 1 && !desMsgObject.Equips.isEmpty())
			{
				for (int i = 0; i < desMsgObject.getEquips().size(); i++)
				{
					EquipTableRows.add(i, desMsgObject.getEquips().get(i));
				}

				if (EquipInited != null)
				{
					for (EventHandler<EventArgs> listener : EquipInited.listeners())
					{
						listener.invoke(this, EventArgs.Empty);
					}
				}
			}
        } catch (DaprException exception) {
			DataCenter.WriteLogFile("Error code: " + exception.getErrorCode() +
					"Error message: " + exception.getMessage() +
					"Reason: " + exception.getErrorDetails().get(DaprErrorDetails.ErrorDetailType.ERROR_INFO, "reason", TypeRef.STRING) +
					"Error payload: " + new String(exception.getPayload()));
		} catch (Exception e) {
			throw new RuntimeException(e);
		}
    }

	public final void PublishYcRtValueAsync(MqRtValueMessage msg) {

		try {
			msg.setDataType(1);
			_grpcDaprClient.publishEvent(
					PUBSUB_NAME,
					TOPIC_NAME,
					msg).block();

		} catch (DaprException exception) {
			DataCenter.WriteLogFile("Error code: " + exception.getErrorCode() +
					"Error message: " + exception.getMessage() +
					"Reason: " + exception.getErrorDetails().get(DaprErrorDetails.ErrorDetailType.ERROR_INFO, "reason", TypeRef.STRING) +
					"Error payload: " + new String(exception.getPayload()));
		}
	}

	public final void PublishYxRtValueAsync(MqRtValueMessage msg)
	{
		try {
			msg.setDataType(2);
			_grpcDaprClient.publishEvent(
					PUBSUB_NAME,
					TOPIC_NAME,
					msg).block();
		}catch (DaprException exception) {
			DataCenter.WriteLogFile("Error code: " + exception.getErrorCode() +
					"Error message: " + exception.getMessage() +
					"Reason: " + exception.getErrorDetails().get(DaprErrorDetails.ErrorDetailType.ERROR_INFO, "reason", TypeRef.STRING) +
					"Error payload: " + new String(exception.getPayload()));
		}
	}


	public final void PublishRtStateAsync(MqRtStateMessage msg)
	{
		try {
			_grpcDaprClient.publishEvent(
					PUBSUB_NAME,
					TOPIC_NAME,
					msg).block();
		} catch (DaprException exception) {
			DataCenter.WriteLogFile("Error code: " + exception.getErrorCode() +
					"Error message: " + exception.getMessage() +
					"Reason: " + exception.getErrorDetails().get(DaprErrorDetails.ErrorDetailType.ERROR_INFO, "reason", TypeRef.STRING) +
					"Error payload: " + new String(exception.getPayload()));
		}
	}

	public final void PublishEvtValueAsync(MqEvtMessage msg)
	{
		try {
			_grpcDaprClient.publishEvent(
					PUBSUB_NAME,
					TOPIC_NAME,
					msg).block();
		} catch (DaprException exception) {
			DataCenter.WriteLogFile("Error code: " + exception.getErrorCode() +
					"Error message: " + exception.getMessage() +
					"Reason: " + exception.getErrorDetails().get(DaprErrorDetails.ErrorDetailType.ERROR_INFO, "reason", TypeRef.STRING) +
					"Error payload: " + new String(exception.getPayload()));
		}
	}

	public Event<EventHandler<EventArgs>> EquipInited = new Event<EventHandler<EventArgs>>();
}