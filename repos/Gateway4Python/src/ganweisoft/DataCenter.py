import os
import json
import base64
import threading
from datetime import datetime
from enum import Enum
from typing import Dict

from ganweisoft import StationItem
from ganweisoft import EquipItem
from ganweisoft.PropertyService import PropertyService, Properties
from ganweisoft.Logging import Logging


class DataCenter:
    _equip_item_dict: Dict[int, EquipItem] = {}
    _brunning = False
    _bsibok = False
    _s_station_only_mark = ""
    _b_encrypt = False
    _myo = True

    @classmethod
    def get_equip_item_dict(cls):
        with threading.Lock():
            return cls._equip_item_dict

    @classmethod
    def get_equip_item(cls, i_equip_no):
        if i_equip_no in cls._equip_item_dict:
            return cls._equip_item_dict[i_equip_no]
        return None

    @staticmethod
    def encode_base64(source: str, encoding: str = "utf-8") -> str:
        try:
            return base64.b64encode(source.encode(encoding)).decode(encoding)
        except:
            return source

    @staticmethod
    def decode_base64(encoded: str, encoding: str = "utf-8") -> str:
        try:
            return base64.b64decode(encoded).decode(encoding)
        except:
            return encoded

    @classmethod
    def start(cls):
        with threading.Lock():
            if not cls._brunning:
                try:
                    if not StationItem.StationItem.init():
                        input("Press Enter to continue...")
                        return
                    for entry in StationItem.StationItem.EquipCategoryDict.items():
                        equip_list = entry[1]
                        equip_list.old_equip = None
                        equip_list.start_refresh_thread()
                        equip_list.start_set_parm_thread()
                    cls._brunning = True
                    print(f"{datetime.now().strftime('%Y-%m-%d %H:%M:%S')} 程序运行中...")
                except Exception as e:
                    Logging.write_log_file(str(e))

    @classmethod
    def get_property_from_property_service(cls, property_name, node_name, default_value):
        str_value = ""
        try:
            if not node_name:
                str_value = PropertyService.get(property_name, default_value)
                return str_value

            properties = PropertyService.get(property_name, Properties())
            if properties.contains(node_name):
                str_value = properties.get(node_name, default_value)
            else:
                str_value = default_value
                properties.set(node_name, default_value)
                PropertyService.save()
            return str_value
        except Exception as e:
            Logging.write_log_file(str(e))
        return str_value

    @classmethod
    def set_property_to_property_service(cls, property_name, node_name, value):
        try:
            if not node_name:
                PropertyService.set(property_name, value)
                return

            properties = PropertyService.get(property_name, Properties())
            if properties.contains(node_name):
                properties.set(node_name, value)
                PropertyService.save()
        except Exception as e:
            Logging.write_log_file(str(e))

    @classmethod
    def get_property_from_reserve(cls, table_name, reserve_name, i_equip_no, i_chedian_no, node_name):
        str_value = ""
        try:
            table_name = table_name.lower()
            if table_name == "equip":
                r = next((item for item in StationItem.StationItem.db_eqp if item.equip_no == i_equip_no), None)
                if not r:
                    Logging.write_log_file(f"GetPropertyFromReserve 找不到设备号为{i_equip_no}的EquipTableRow！")
                    return False
                if reserve_name.lower() == "reserve1":
                    str_reserve = r.Reserve1
                elif reserve_name.lower() == "reserve2":
                    str_reserve = r.Reserve2
                elif reserve_name.lower() == "reserve3":
                    str_reserve = r.Reserve3
                else:
                    return False
            elif table_name == "ycp":
                r = next(
                    (item for item in StationItem.StationItem.db_ycp if item.equip_no == i_equip_no and item.yc_no == i_chedian_no),
                    None)
                if not r:
                    Logging.write_log_file(
                        f"GetPropertyFromReserve 找不到设备号为{i_equip_no}测点号为{i_chedian_no}的YcpTableRow！")
                    return False
                if reserve_name.lower() == "reserve1":
                    str_reserve = r.Reserve1
                elif reserve_name.lower() == "reserve2":
                    str_reserve = r.Reserve2
                elif reserve_name.lower() == "reserve3":
                    str_reserve = r.Reserve3
                else:
                    return False
            elif table_name == "yxp":
                r = next(
                    (item for item in StationItem.StationItem.db_yxp if item.equip_no == i_equip_no and item.yx_no == i_chedian_no),
                    None)
                if not r:
                    Logging.write_log_file(
                        f"GetPropertyFromReserve 找不到设备号为{i_equip_no}测点号为{i_chedian_no}的YxpTableRow！")
                    return False
                if reserve_name.lower() == "reserve1":
                    str_reserve = r.Reserve1
                elif reserve_name.lower() == "reserve2":
                    str_reserve = r.Reserve2
                elif reserve_name.lower() == "reserve3":
                    str_reserve = r.Reserve3
                else:
                    return False
            elif table_name == "setparm":
                r = next((item for item in StationItem.StationItem.db_setparm if
                          item.equip_no == i_equip_no and item.set_no == i_chedian_no), None)
                if not r:
                    Logging.write_log_file(
                        f"GetPropertyFromReserve 找不到设备号为{i_equip_no}设置号为{i_chedian_no}的SetParmTableRow！")
                    return False
                if reserve_name.lower() == "reserve1":
                    str_reserve = r.Reserve1
                elif reserve_name.lower() == "reserve2":
                    str_reserve = r.Reserve2
                elif reserve_name.lower() == "reserve3":
                    str_reserve = r.Reserve3
                else:
                    return False
            else:
                Logging.write_log_file(f"GetPropertyFromReserve 的参数{table_name}不符合预期！")
                return False

            if not str_reserve or not str_reserve.strip():
                return False

            doc = json.loads(str_reserve)
            current_element = doc
            for segment in node_name.split("#"):
                if isinstance(current_element, dict) and segment in current_element:
                    current_element = current_element[segment]
                else:
                    Logging.write_log_file(f"GetPropertyFromReserve 的参数{node_name}中的节点{segment}没有找到！")
                    return False
            str_value = str(current_element)
            return True
        except Exception as e:
            Logging.write_log_file(
                f"GetPropertyFromReserve 方法出错,TableName：{table_name},ReserveName：{reserve_name},iEquipNo：{i_equip_no},iChedianNo：{i_chedian_no},NodeName：{node_name},异常内容：{str(e)}")
        return False

    @classmethod
    def get_property_from_reserve_with_equip_table_row(cls, reserve_name, node_name, r):
        str_value = ""
        try:
            if not r:
                return False
            if reserve_name.lower() == "reserve1":
                str_reserve = r.Reserve1
            elif reserve_name.lower() == "reserve2":
                str_reserve = r.Reserve2
            elif reserve_name.lower() == "reserve3":
                str_reserve = r.Reserve3
            else:
                return False

            if not str_reserve or not str_reserve.strip():
                return False

            doc = json.loads(str_reserve)
            current_element = doc
            for segment in node_name.split("#"):
                if isinstance(current_element, dict) and segment in current_element:
                    current_element = current_element[segment]
                else:
                    Logging.write_log_file(f"GetPropertyFromReserve 的参数{node_name}中的节点{segment}没有找到！")
                    return False
            str_value = str(current_element)
            return True
        except Exception as e:
            Logging.write_log_file(
                f"GetPropertyFromReserveWithEquipTableRow 方法出错,ReserveName：{reserve_name},iEquipNo：{r.equip_no},NodeName：{node_name},异常内容：{str(e)}")
        return False

    @classmethod
    def check_and_create_node(cls, node, keys, value, index=0):
        if index >= len(keys):
            return
        if keys[index] not in node:
            if index == len(keys) - 1:
                node[keys[index]] = value
            else:
                child = {}
                node[keys[index]] = child
                cls.check_and_create_node(child, keys, value, index + 1)
        else:
            if index == len(keys) - 1:
                node[keys[index]] = value
            else:
                cls.check_and_create_node(node[keys[index]], keys, value, index + 1)
