class MqttTopic:
    TopicIotsysEquipDown = "$sys/iotcenter/+/python/down"
    TopicIotsysCommandDown = "$sys/iotcenter/+/command/down"
    TopicIotsysEquipAddDown = "$sys/iotcenter/+/eqpadd/down"
    TopicIotsysEquipDeleteDown = "$sys/iotcenter/+/eqpdel/down"
    TOPIC_IOTSYS_MINIDCYC_DEVICE_DATA_REPORT = "$sys/iotcenter/{0}/minidc/yc/up"
    TOPIC_IOTSYS_MINIDCYX_DEVICE_DATA_REPORT = "$sys/iotcenter/{0}/minidc/yx/up"
    TOPIC_IOTSYS_MINIDCYX_DEVICE_EVT_REPORT = "$sys/iotcenter/{0}/minidc/evt/up"
    TOPIC_IOTSYS_DEVICE_STATE_REPORT = "$sys/iotcenter/{0}/statereport/up"