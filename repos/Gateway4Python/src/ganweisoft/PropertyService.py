import os
import xml.etree.ElementTree as ET
from typing import Dict, List, Optional, Any, Callable, LiteralString
import threading


class CallbackOnDispose:
    def __init__(self, callback: Callable):
        if callback is None:
            raise ValueError("callback cannot be None")
        self.callback = callback

    def __del__(self):
        action = self.callback
        if action is not None:
            action()


class PropertyChangedEventArgs:
    def __init__(self, properties: 'Properties', key: str, old_value: Any, new_value: Any):
        self.properties = properties
        self.key = key
        self.old_value = old_value
        self.new_value = new_value


class Properties:
    def __init__(self):
        self.properties = {}
        self.lock = threading.Lock()

    def __getitem__(self, key: str) -> str:
        return str(self.get(key))

    def __setitem__(self, key: str, value: str):
        self.set(key, value)

    @property
    def elements(self) -> List[str]:
        with self.lock:
            return list(self.properties.keys())

    def get(self, key: str) -> Any:
        with self.lock:
            return self.properties.get(key)

    def set(self, key: str, value: Any):
        if key is None:
            raise ValueError("key cannot be None")
        if value is None:
            raise ValueError("value cannot be None")
        old_value = None
        with self.lock:
            if key not in self.properties:
                self.properties[key] = value
            else:
                old_value = self.properties[key]
                self.properties[key] = value
        self.on_property_changed(PropertyChangedEventArgs(self, key, old_value, value))

    def contains(self, key: str) -> bool:
        with self.lock:
            return key in self.properties

    @property
    def count(self) -> int:
        with self.lock:
            return len(self.properties)

    def remove(self, key: str) -> bool:
        with self.lock:
            if key in self.properties:
                del self.properties[key]
                return True
            return False

    def __str__(self) -> str:
        with self.lock:
            sb = ["[Properties:{"]
            for key, value in self.properties.items():
                sb.append(f"{key}={value},")
            sb.append("}]")
            return ''.join(sb)

    @staticmethod
    def read_from_attributes(reader: ET.Element) -> 'Properties':
        properties = Properties()
        if reader.attrib:
            for key, value in reader.attrib.items():
                properties[key] = value
        return properties

    def read_properties(self, root: ET.Element):
        if root is None or root.tag != "Properties":
            return
        for child in root:
            if child.tag == "Properties":
                key = child.attrib.get("name")
                p = Properties()
                p.read_properties(child)
                self.properties[key] = p
            elif child.tag == "Array":
                key = child.attrib.get("name")
                self.properties[key] = self.read_array(child)
            elif child.tag == "SerializedValue":
                key = child.attrib.get("name")
                self.properties[key] = child.text
            else:
                key = child.tag
                self.properties[key] = child.attrib.get("value")

    def read_array(self, element: ET.Element) -> List[Any]:
        arr = []
        for child in element:
            if child.tag == "Element":
                arr.append(child.attrib.get("value"))
        return arr

    def write_properties(self, root: ET.Element):
        with self.lock:
            sorted_properties = sorted(self.properties.items(), key=lambda x: x[0].lower())
            for key, value in sorted_properties:
                if isinstance(value, Properties):
                    prop_element = ET.SubElement(root, "Properties")
                    prop_element.set("name", key)
                    value.write_properties(prop_element)
                elif isinstance(value, list):
                    array_element = ET.SubElement(root, "Array")
                    array_element.set("name", key)
                    for item in value:
                        element = ET.SubElement(array_element, "Element")
                        element.set("value", str(item))
                else:
                    element = ET.SubElement(root, key)
                    element.set("value", str(value))

    def save(self, file_name: str):
        root = ET.Element("Properties")
        self.write_properties(root)
        tree = ET.ElementTree(root)
        tree.write(file_name, encoding="utf-8", xml_declaration=True)

    @staticmethod
    def load(file_name: str) -> Optional['Properties']:
        if not os.path.exists(file_name):
            return None
        tree = ET.parse(file_name)
        root = tree.getroot()
        if root is None or root.tag != "Properties":
            return None
        properties = Properties()
        properties.read_properties(root)
        return properties

    def get(self, property_name: str, default_value: Any) -> Any:
        with self.lock:
            value = self.properties.get(property_name)
            if value is None:
                self.properties[property_name] = default_value
                return default_value
            return value

    def on_property_changed(self, e: PropertyChangedEventArgs):
        if hasattr(self, "property_changed") and self.property_changed is not None:
            self.property_changed(self, e)

    property_changed = None


class PropertyService:
    property_file_name = None
    property_xml_root_node_name = None
    config_directory = None
    data_directory = None
    properties = None

    @classmethod
    def get(cls, property_name: str) -> str:
        return cls.properties[property_name]

    @classmethod
    def get(cls, property_name: str, default_value: Any) -> Any:
        return cls.properties.get(property_name, default_value)

    @classmethod
    def set(cls, property_name: str, value: Any):
        cls.properties.set(property_name, value)

    @classmethod
    def load(cls):
        if cls.properties is None:
            raise Exception("Service is not initialized")
        file_path = os.path.join(cls.config_directory, cls.property_file_name)
        if not cls.load_properties_from_stream(file_path):
            options_path = os.path.join(cls.data_directory, "options", cls.property_file_name)
            cls.load_properties_from_stream(options_path)

    @classmethod
    def load_properties_from_stream(cls, file_path: str | LiteralString | bytes) -> bool:
        if not os.path.exists(file_path):
            return False
        try:
            with cls.lock_property_file():
                tree = ET.parse(file_path)
                root = tree.getroot()
                if root is not None and root.tag == cls.property_xml_root_node_name:
                    cls.properties.read_properties(root)
                    return True
        except ET.ParseError:
            pass
        return False

    @classmethod
    def save(cls):
        if cls.config_directory is None or cls.property_xml_root_node_name is None:
            raise Exception("No file name was specified on service creation")
        file_path = os.path.join(cls.config_directory, cls.property_file_name)
        with cls.lock_property_file():
            cls.properties.save(file_path)

    @classmethod
    def lock_property_file(cls) -> threading.Lock:
        cls.lock = threading.Lock()
        cls.lock.acquire()
        return cls.lock

    @classmethod
    def unlock_property_file(cls):
        if hasattr(cls, "lock") and cls.lock is not None:
            cls.lock.release()

    @classmethod
    def properties_property_changed(cls, sender: Any, e: PropertyChangedEventArgs):
        if hasattr(cls, "property_changed") and cls.property_changed is not None:
            cls.property_changed(None, e)

    property_changed = None

    @classmethod
    def initialize_service(cls, config_directory: str, data_directory: str, properties_name: str):
        if cls.properties is not None:
            raise Exception("Service is already initialized")
        cls.properties = Properties()
        cls.config_directory = config_directory
        cls.data_directory = data_directory
        cls.property_xml_root_node_name = properties_name
        cls.property_file_name = f"{properties_name}.xml"
        cls.properties.property_changed = cls.properties_property_changed

    @classmethod
    def initialize_service_for_unit_tests(cls):
        cls.properties = None
        cls.initialize_service(None, None, None)
