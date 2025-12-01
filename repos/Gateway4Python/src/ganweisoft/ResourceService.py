import os
import xml.etree.ElementTree as ET
import base64
from typing import Dict


class ResourceService:
    class ResourceItem:
        def __init__(self):
            self.StringID = None
            self.zh = None
            self.ft = None
            self.en = None

        def GetString(self, language):
            if language in ["zh-CN", "zh"]:
                return self.zh
            elif language in ["zh-HK", "ft"]:
                return self.ft
            elif language in ["en-US", "en"]:
                return self.en
            return None

    ResourceDict: Dict[str, ResourceItem] = {}

    @classmethod
    def GetApplicationRootPath(cls):
        try:
            return os.path.join(os.path.dirname(os.path.abspath(__file__)), "..")
        except:
            return os.getcwd()

    @classmethod
    def InitializeService(cls):
        xml_file = os.path.join(cls.GetApplicationRootPath(), "bin/GWRES1.dll")
        if not os.path.exists(xml_file):
            return
        tree = ET.parse(xml_file)
        root = tree.getroot()
        for node in root:
            item = cls.ResourceItem()
            item.StringID = node.find("StringID").text
            item.zh = node.find("zh-CN").text if node.find("zh-CN") is not None else node.find("zh").text
            item.ft = node.find("zh-HK").text if node.find("zh-HK") is not None else node.find("ft").text
            item.en = node.find("en-US").text if node.find("en-US") is not None else node.find("en").text
            cls.ResourceDict[base64.b64decode(item.StringID).decode().strip()] = item

    @classmethod
    def GetString(cls, name):
        try:
            if name.strip() in cls.ResourceDict:
                return base64.b64decode(cls.ResourceDict[name].GetString("zh-CN")).decode()
        except:
            pass
        return name

    @classmethod
    def GetString(cls, name, default_str):
        try:
            if name.strip() in cls.ResourceDict:
                return base64.b64decode(cls.ResourceDict[name].GetString("zh-CN")).decode()
            return default_str
        except:
            return default_str