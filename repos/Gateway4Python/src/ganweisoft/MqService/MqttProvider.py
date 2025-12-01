import os
import json
from typing import List, Dict, Callable

from dapr.common.pubsub.subscription import SubscriptionMessage
from dapr.conf import settings

from ganweisoft.Logging import Logging

from ganweisoft.MqService.Model.MqRtValueMessage import MqRtValueMessage
from ganweisoft.SetItem import SetItem
from ganweisoft import StationItem
from ganweisoft.MqService.Model.MqMessage import Equip, Ycp

from dapr.clients import DaprClient
from dapr.clients.grpc._response import TopicEventResponse
from dapr.clients.exceptions import DaprGrpcError


class MqttProvider:
    _instance = None
    EquipTableRows: Dict[int, Equip] = {}
    _dapr_client = None
    _dapr_grpc_client = None
    _gateway_id: str = None
    _mq_server: str = None
    _gateway_id: str = None
    _config = {}
    STATE_TOPIC_NAME = "state"
    EVT_TOPIC_NAME = "event"
    TOPIC_NAME = "report"
    PUBSUB_NAME = "open_datacenter_pubsub"
    CMD_TOPIC_NAME = "set"

    def __new__(cls):
        if cls._instance is None:
            cls._instance = super().__new__(cls)
            cls._instance.__init__()
        return cls._instance

    def __init__(self):
        pass

    def init(self):
        with open('config/config.properties', 'r') as file:
            for line in file:
                line = line.strip()
                if line and not line.startswith('#'):  # 跳过空行和注释行
                    key, value = line.split(':', 1)  # 从第一个冒号处分割
                    self._config[key.strip()] = value.strip()

        username = os.environ.get("IoTCenterMqttName")
        if not username:
            username = self._config.get("MqUsername")

        password = os.environ.get("IoTCenterMqttKey")
        if not password:
            password = self._config.get("MqPassword")

        self._gateway_id = os.environ.get("IoTCenterGatewayInstanceId")
        if not self._gateway_id:
            self._gateway_id = self._config.get("InstanceId")

        mq_server = os.environ.get("IoTCenterMqttServerIp")
        if not mq_server:
            mq_server = self._config.get("MqServer")

        mq_grpc_server = os.environ.get("IoTCenterMqttGrpcServerIp")
        if not mq_grpc_server:
            mq_grpc_server = self._config.get("MqGRPCServer")

        def trace_injector() -> Dict[str, str]:
            headers: Dict[str, str] = {"dapr-api-token": username + password}
            return headers

        if mq_server:
            settings.DAPR_HTTP_ENDPOINT = mq_server

        if username or password:
            settings.DAPR_API_TOKEN = username + password

        self._dapr_client = DaprClient()
        self._dapr_grpc_client = DaprClient(mq_grpc_server)
        self.get_equip()

        # self._dapr_client.subscribe_with_handler(
        #     pubsub_name=self.PUBSUB_NAME,
        #     topic=self.CMD_TOPIC_NAME,
        #     handler_fn=self.process_cmd_message,
        #     metadata=tuple((k, v) for k, v in trace_injector().items()),
        #     dead_letter_topic=self.CMD_TOPIC_NAME + '_DEAD',
        # )

    def get_equip(self):
        req_data = {'Categary': "PYTHON"}

        try:
            # Create a typed message with content type and body
            resp = self._dapr_client.invoke_method(
                self._gateway_id,
                'equip',
                http_verb='POST',
                data=json.dumps(req_data),
            )

            Logging.write_log_file("get_equip response data: " + resp.text())

            des_msg_object = json.loads(resp.text())[0]
            if des_msg_object.get("flowType") == 2 and des_msg_object.get("equips"):
                for equip in des_msg_object.get("equips"):
                    self.EquipTableRows[equip["equipNo"]] = Equip(equip)
                self.equip_inited()

        except DaprGrpcError as e:
            print(e._err_message, flush=True)
            print(e.error_code, flush=True)
        except Exception as ex:
            print(ex.__str__(), flush=True)

    @staticmethod
    def process_cmd_message(message: SubscriptionMessage):
        des_msg_object = json.loads(message.data())
        equip_item = StationItem.StationItem.get_equip_item_from_equip_no(des_msg_object.get("EquipNo"))
        if equip_item:
            equip_item.AddSetItem(SetItem(
                equipno=des_msg_object.get("EquipNo"),
                main_instruct=des_msg_object.get("MainInstruct"),
                minor_instruct=des_msg_object.get("MinorInstruct"),
                value=des_msg_object.get("Value")
            ))
            return TopicEventResponse('success')
        return TopicEventResponse('drop')

    def publish_yc_rt_value_async(self, msg: MqRtValueMessage):
        payload = json.dumps(msg, default=lambda o: o.__dict__)
        #self._dapr_client.set_metadata("dapr-api-token", "jvQYmCmRfwCRpO0r8/8KFQ==")
        self._dapr_grpc_client.publish_event(
            pubsub_name=self.PUBSUB_NAME,
            topic_name=self.TOPIC_NAME,
            data=payload,
            data_content_type='application/json',
            publish_metadata={'ttlInSeconds': '100', 'rawPayload': 'false'},
        )

    def publish_yx_rt_value_async(self, msg: MqRtValueMessage):
        payload = json.dumps(msg, default=lambda o: o.__dict__)
        self._dapr_grpc_client.publish_event(
            pubsub_name=self.PUBSUB_NAME,
            topic_name=self.TOPIC_NAME,
            data=payload,
            data_content_type='application/json',
            publish_metadata={'ttlInSeconds': '100', 'rawPayload': 'false'},
        )

    def publish_rt_state_async(self, msg):
        payload = json.dumps(msg, default=lambda o: o.__dict__)
        self._dapr_grpc_client.publish_event(
            pubsub_name=self.PUBSUB_NAME,
            topic_name=self.STATE_TOPIC_NAME,
            data=payload,
            data_content_type='application/json',
            publish_metadata={'ttlInSeconds': '100', 'rawPayload': 'false'},
        )

    def publish_evt_value_async(self, msg):
        payload = json.dumps(msg, default=lambda o: o.__dict__)
        self._dapr_grpc_client.publish_event(
            pubsub_name=self.PUBSUB_NAME,
            topic_name=self.EVT_TOPIC_NAME,
            data=payload,
            data_content_type='application/json',
            publish_metadata={'ttlInSeconds': '100', 'rawPayload': 'false'},
        )

    def equip_inited(self):
        # Implement equip initialized logic
        from ganweisoft.Database.GWDbProvider import GWDbProvider
        GWDbProvider().InitCompleted = True
