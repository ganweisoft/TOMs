// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTClient.Enums;

namespace IoTClient.Models
{
    public class ModbusInput
    {
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public DataTypeEnum DataType { get; set; }
        /// <summary>
        /// 站号
        /// </summary>
        public byte StationNumber { get; set; }
        /// <summary>
        /// 功能码
        /// </summary>
        public byte FunctionCode { get; set; }

        /// <summary>
        /// 写入的值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 获取当前唯一Key，不包含数据类型
        /// </summary>
        public string DisplayNameKey
        {
            get { return $"{StationNumber}_{FunctionCode}_{Address}"; }
        }

        /// <summary>
        /// 遥测遥信的main_instruction字段
        /// </summary>
        public string NodeStr { get; private set; }

        /// <summary>
        /// 对原始值进行保留
        /// </summary>
        /// <param name="nodeStr"></param>
        public void SetNodeStr(string nodeStr)
        {
            NodeStr = nodeStr;
        }
    }

}
