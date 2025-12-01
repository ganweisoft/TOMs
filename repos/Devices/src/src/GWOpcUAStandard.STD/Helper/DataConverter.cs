// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWOpcUAStandard.STD;

public class DataConverter
{
    public static bool Parse(DataTypeEnum dataTypeEnum, string value, out object sendValue, out string msg)
    {
        sendValue = null;
        msg = null;
        try
        {
            switch (dataTypeEnum)
            {
                case DataTypeEnum.Bool:
                    sendValue = Convert.ToBoolean(value);
                    break;
                case DataTypeEnum.Byte:
                    sendValue = Convert.ToByte(value);
                    break;
                case DataTypeEnum.Int16:
                    sendValue = Convert.ToInt16(value);
                    break;
                case DataTypeEnum.UInt16:
                    sendValue = Convert.ToUInt16(value);
                    break;
                case DataTypeEnum.Int32:
                    sendValue = Convert.ToInt32(value);
                    break;
                case DataTypeEnum.UInt32:
                    sendValue = Convert.ToUInt32(value);
                    break;
                case DataTypeEnum.Int64:
                    sendValue = Convert.ToInt64(value);
                    break;
                case DataTypeEnum.UInt64:
                    sendValue = Convert.ToUInt64(value);
                    break;
                case DataTypeEnum.Float:
                    sendValue = Convert.ToSingle(value);  // 或者 (float)value
                    break;
                case DataTypeEnum.Double:
                    sendValue = Convert.ToDouble(value);
                    break;
                case DataTypeEnum.DateTime:
                    sendValue = Convert.ToDateTime(value);
                    break;
                case DataTypeEnum.SByte:
                    sendValue = Convert.ToSByte(value);
                    break;
                case DataTypeEnum.String:
                    sendValue = value;
                    break;
                case DataTypeEnum.None:
                default:
                    sendValue = value;
                    break;
            }

            return true;
        }
        catch (Exception ex)
        {
            msg = $"值 '{value}' 无法转换为 {dataTypeEnum} 类型。异常：" + ex.Message;
            return false;
        }
    }
}

/*
 private bool ConvertToType(string minorInstruct, string value, out object sendValue,out string msg)
        {
            sendValue = null;
            msg = null;
            try
            {
                switch (minorInstruct.ToLower())
                {
                    case "int32":
                    case "int":
                        sendValue = Convert.ToInt32(value);
                        break;
                    case "float":
                        sendValue = Convert.ToSingle(value);
                        break;
                    case "short":
                    case "int16":
                        sendValue = Convert.ToInt16(value);
                        break;
                    case "byte":
                        sendValue = Convert.ToByte(value);
                        break;
                    case "ushort":
                    case "uint16":
                        sendValue = Convert.ToUInt16(value);
                        break;
                    case "int64":
                    case "long":
                        sendValue = Convert.ToInt64(value);
                        break;
                    case "uint":
                    case "uint32":
                        sendValue = Convert.ToUInt32(value);
                        break;
                    case "sbyte":
                        sendValue = Convert.ToSByte(value);
                        break;
                    case "uint64":
                    case "ulong":
                        sendValue = Convert.ToUInt64(value);
                        break;
                    case "double":
                        sendValue = Convert.ToDouble(value);
                        break;
                    case "datetime":
                        sendValue = Convert.ToDateTime(value);
                        break;
                    case "string":
                        sendValue = value;
                        break;
                    case "boolean":
                    case "bool":
                        if (string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "false", StringComparison.OrdinalIgnoreCase))
                        {
                            sendValue = bool.Parse(value);
                        }
                        else if (string.Equals(value, "0", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "1", StringComparison.OrdinalIgnoreCase))
                        {
                            sendValue = Convert.ToBoolean(int.Parse(value));
                        }
                        else
                        {
                            msg = "SetParm执行结果:下发BOOL类型只支持true/false或者1/0";
                            return false;
                        }
                        break;
                    default:
                        sendValue = value;
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {
                msg = $"SetParm执行结果:转换失败 - {ex.Message}";
                return false;
            }
        }
 */