// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace IoTClient.Models
{
    public class ModbusOutput
    {
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 站号
        /// </summary>
        public byte StationNumber { get; set; }
        /// <summary>
        /// 功能码
        /// </summary>
        public byte FunctionCode { get; set; }
        public object Value { get; set; }

        public string DisplayNameKey
        {
            get { return $"{StationNumber}_{FunctionCode}_{Address}"; }
        }
    }
}
