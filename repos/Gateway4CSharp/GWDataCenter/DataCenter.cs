using GWDataCenter.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace GWDataCenter
{
    public enum MessageLevel
    {
        Wubao = -1, Info = 0, SpecalInfo = 9999, Debug = 10000, SetParm, ZiChan, Warn, Error, Fatal
    }
    public static partial class DataCenter
    {
        static  Dictionary<int, EquipItem> equipItemDict = new Dictionary<int, EquipItem>();
        static public object brunning = false;

        static object bsibok = false;

        static object s_StationOnlyMark = "";
        static bool bEncrypt = false;

        public static object myo = true;
        static public Dictionary<int, EquipItem> EquipItemDict
        {
            get
            {
                lock (myo)
                {
                    return equipItemDict;
                }
            }
        }

        static public EquipItem GetEquipItem(int iEquipNo)
        {
            if (EquipItemDict.ContainsKey(iEquipNo))
                return EquipItemDict[iEquipNo];
            return null;
        }

        /// <summary>
        /// 写入错误日志文件
        /// </summary>
        /// <param name="input">写入内容</param>
        public static void WriteLogFile(string input)
        {
            Console.WriteLine(input);
            try
            {
                byte[] bs;
                FileStream filestream;
                string RootPathName = Path.Combine(General.GetApplicationRootPath(), "log");
                System.IO.Directory.CreateDirectory(RootPathName);
                string FileName = null;
                //如果没有发现指定文件，则生成新文件

                FileName = "XLog.txt";

                if (!File.Exists(Path.Combine(RootPathName, FileName)))
                {
                    filestream = File.Create(Path.Combine(RootPathName, FileName));
                    filestream.Close();
                }
                else//文件存在
                {
                    FileInfo fileInfo = new FileInfo(Path.Combine(RootPathName, FileName));
                    if (fileInfo.Length > 1024 * 1024 * 5)//文件大于10M则删除
                    {
                        File.Delete(Path.Combine(RootPathName, FileName));
                        filestream = File.Create(Path.Combine(RootPathName, FileName));
                        filestream.Close();
                    }
                }
                //StreamWriter sw = new StreamWriter(Path.Combine(RootPathName, FileName), true, System.Text.Encoding.Default);
                filestream = File.Open(Path.Combine(RootPathName, FileName), FileMode.Append, FileAccess.Write);
                /////////
                //写入时间
                string sss = "服务端";

                string strNow = string.Format("*******************************************{0}-{1}({2})*******************************************{3}", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString(), sss, Environment.NewLine);
                bs = System.Text.Encoding.Default.GetBytes(strNow);
                filestream.Write(bs, 0, bs.Length);

                //sw.Write(strNow);

                bs = System.Text.Encoding.Default.GetBytes(input);
                filestream.Write(bs, 0, bs.Length);
                //sw.Write(input);

                bs = System.Text.Encoding.Default.GetBytes(Environment.NewLine);
                filestream.Write(bs, 0, bs.Length);
                //sw.WriteLine(Environment.NewLine);

                string fg = "**********************************************************************************************";
                bs = System.Text.Encoding.Default.GetBytes(fg);
                filestream.Write(bs, 0, bs.Length);
                //sw.WriteLine(fg);

                //末尾换行
                bs = System.Text.Encoding.Default.GetBytes(Environment.NewLine);
                filestream.Write(bs, 0, bs.Length);
                //sw.WriteLine(Environment.NewLine);
                //sw.Close();
                filestream.Close();
                filestream.Dispose();
            }
            catch (Exception e)
            {
            }
        }


        /// <summary> 
        /// Base64加密 
        /// </summary> 
        /// <param name="codeName">加密采用的编码方式</param> 
        /// <param name="source">待加密的明文</param> 
        /// <returns></returns> 
        public static string EncodeBase64(Encoding encode, string source)
        {
            byte[] bytes = encode.GetBytes(source);
            string result;
            try
            {
                result = Convert.ToBase64String(bytes);
            }
            catch
            {
                result = source;
            }
            return result;
        }

        /// <summary> 
        /// Base64加密，采用utf8编码方式加密 
        /// </summary> 
        /// <param name="source">待加密的明文</param> 
        /// <returns>加密后的字符串</returns> 
        public static string EncodeBase64(string source)
        {
            return EncodeBase64(Encoding.UTF8, source);
        }

        /// <summary> 
        /// Base64解密 
        /// </summary> 
        /// <param name="codeName">解密采用的编码方式，注意和加密时采用的方式一致</param> 
        /// <param name="result">待解密的密文</param> 
        /// <returns>解密后的字符串</returns> 
        public static string DecodeBase64(Encoding encode, string result)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(result);
            try
            {
                decode = encode.GetString(bytes);
            }
            catch
            {
                decode = result;
            }
            return decode;
        }

        /// <summary> 
        /// Base64解密，采用utf8编码方式解密 
        /// </summary> 
        /// <param name="result">待解密的密文</param> 
        /// <returns>解密后的字符串</returns> 
        public static string DecodeBase64(string result)
        {
            return DecodeBase64(Encoding.UTF8, result);
        }

        static public void Start()
        {
            lock (brunning)
            {
                if (!(bool)brunning)
                {
                    try
                    {
                        if (!StationItem.init())
                        {
                            Console.ReadLine();
                            return;
                        }
                        foreach (KeyValuePair<string, object> entry in StationItem.EquipCategoryDict)
                        {
                            //启动数据获取线程和设置扫描线程 
                            SubEquipList EquipList = (SubEquipList)entry.Value;
                            EquipList.OldEquip = null;

                            EquipList.StartRefreshThread();
                            EquipList.StartSetParmThread();
                        }

                        brunning = true;
                        Console.WriteLine(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ResourceService.GetString("AlarmCenter.DataCenter.Msg22", "程序运行中…"));

                    }

                    catch (Exception e)
                    {
                        DataCenter.WriteLogFile(e.ToString());
                    }
                }
            }
        }

        #region 读写系统的配置文件
        /// <summary>
        /// 读取系统的配置文件
        /// </summary>
        /// <param name="PropertyName">配置文件中的属性名称</param>
        /// <param name="NodeName">属性下面的节点名称</param>
        /// <param name="DefaultValue">如果没有配置该属性，则返回默认值</param>
        /// <returns></returns>
        public static string GetPropertyFromPropertyService(string PropertyName, string NodeName, string DefaultValue)
        {
            string strValue = "";
            try
            {
                if (string.IsNullOrEmpty(NodeName))
                {
                    strValue = PropertyService.Get(PropertyName, DefaultValue);
                    return strValue;
                }

                Properties properties = PropertyService.Get(PropertyName, new Properties());
                if (properties.Contains(NodeName))
                {
                    strValue = properties.Get(NodeName, DefaultValue);
                }
                else
                {
                    strValue = DefaultValue;
                    properties.Set(NodeName, DefaultValue);
                    PropertyService.Save();
                }
                return strValue;
            }
            catch (Exception e)
            {
                DataCenter.WriteLogFile(e.ToString());
            }
            return strValue;
        }
        /// <summary>
        /// 修改系统的配置文件
        /// </summary>
        /// <param name="PropertyName">配置文件中的属性名称</param>
        /// <param name="NodeName">属性下面的节点名称</param>
        /// <param name="DefaultValue">属性或者节点的值</param>
        /// <returns></returns>
        public static void SetPropertyToPropertyService(string PropertyName, string NodeName, string Value)
        {
            try
            {
                if (string.IsNullOrEmpty(NodeName))
                {
                    PropertyService.Set(PropertyName, Value);
                    return;
                }

                Properties properties = PropertyService.Get(PropertyName, new Properties());
                if (properties.Contains(NodeName))
                {
                    properties.Set(NodeName, Value);
                    PropertyService.Save();
                }
            }
            catch (Exception e)
            {
                DataCenter.WriteLogFile(e.ToString());
            }
        }
        #endregion

        #region 读写数据表Reserve字段的配置文件
        /// <summary>
        /// 读取扩展字段里面的配置参数
        /// </summary>
        /// <param name="strValue">读到的参数值</param>
        /// <param name="TableName">表名称：目前支持equip、ycp、yxp、setparm这四个表</param>
        /// <param name="ReserveName">字段名称，仅支持Reserve1、Reserve2、Reserve3这三个字段名称</param>
        /// <param name="iEquipNo">设备号</param>
        /// <param name="iChedianNo">测点号，无需填入时就填0，比如读取equip表时</param>
        /// <param name="NodeName">json的键名，支持多层深度，每个深度之间用#隔开，比如"GWDataCenter.dll#EnableEquip"</param>
        /// <returns>读取成功返回true，否则为false</returns>
        public static bool GetPropertyFromReserve(out string strValue, string TableName, string ReserveName, int iEquipNo, int iChedianNo, string NodeName)
        {
            string strReserve = "";
            strValue = "";
            try
            {
                switch (TableName.ToLower())
                {
                    case "equip":
                        {
                            EquipTableRow R = (from r in StationItem.db_Eqp where r.equip_no == iEquipNo select r).FirstOrDefault();
                            if (R == null)
                            {
                                DataCenter.WriteLogFile($"GetPropertyFromReserve 找不到设备号为{iEquipNo}的EquipTableRow！");
                                return false;
                            }
                            if (ReserveName.ToLower() == "reserve1")
                                strReserve = R.Reserve1;
                            else if (ReserveName.ToLower() == "reserve2")
                                strReserve = R.Reserve2;
                            else if (ReserveName.ToLower() == "reserve3")
                                strReserve = R.Reserve3;
                            break;
                        }
                    case "ycp":
                        {
                            YcpTableRow R = (from r in StationItem.db_Ycp where r.equip_no == iEquipNo && r.yc_no == iChedianNo select r).FirstOrDefault();
                            if (R == null)
                            {
                                DataCenter.WriteLogFile($"GetPropertyFromReserve 找不到设备号为{iEquipNo}测点号为{iChedianNo}的YcpTableRow！");
                                return false;
                            }
                            if (ReserveName.ToLower() == "reserve1")
                                strReserve = R.Reserve1;
                            else if (ReserveName.ToLower() == "reserve2")
                                strReserve = R.Reserve2;
                            else if (ReserveName.ToLower() == "reserve3")
                                strReserve = R.Reserve3;
                            break;
                        }
                    case "yxp":
                        {
                            YxpTableRow R = (from r in StationItem.db_Yxp where r.equip_no == iEquipNo && r.yx_no == iChedianNo select r).FirstOrDefault();
                            if (R == null)
                            {
                                DataCenter.WriteLogFile($"GetPropertyFromReserve 找不到设备号为{iEquipNo}测点号为{iChedianNo}的YxpTableRow！");
                                return false;
                            }
                            if (ReserveName.ToLower() == "reserve1")
                                strReserve = R.Reserve1;
                            else if (ReserveName.ToLower() == "reserve2")
                                strReserve = R.Reserve2;
                            else if (ReserveName.ToLower() == "reserve3")
                                strReserve = R.Reserve3;
                            break;
                        }
                    case "setparm":
                        {
                            SetParmTableRow R = (from r in StationItem.db_Setparm where r.equip_no == iEquipNo && r.set_no == iChedianNo select r).FirstOrDefault();
                            if (R == null)
                            {
                                DataCenter.WriteLogFile($"GetPropertyFromReserve 找不到设备号为{iEquipNo}设置号为{iChedianNo}的SetParmTableRow！");
                                return false;
                            }
                            if (ReserveName.ToLower() == "reserve1")
                                strReserve = R.Reserve1;
                            else if (ReserveName.ToLower() == "reserve2")
                                strReserve = R.Reserve2;
                            else if (ReserveName.ToLower() == "reserve3")
                                strReserve = R.Reserve3;
                            break;
                        }
                    default:
                        {
                            DataCenter.WriteLogFile($"GetPropertyFromReserve 的参数{TableName}不符合预期！");
                            return false;
                        }
                }
                //空字符串直接返回
                if (string.IsNullOrEmpty(strReserve) || string.IsNullOrWhiteSpace(strReserve)) { return false; }
                // 解析 JSON 字符串
                JsonDocument doc = JsonDocument.Parse(strReserve);
                JsonElement currentElement = doc.RootElement;

                foreach (var segment in NodeName.Split("#"))
                {
                    if (currentElement.ValueKind == JsonValueKind.Object &&
                        currentElement.TryGetProperty(segment, out JsonElement nextElement))
                    {
                        currentElement = nextElement;
                    }
                    else
                    {
                        DataCenter.WriteLogFile($"GetPropertyFromReserve 的参数{NodeName}中的节点{segment}没有找到！");
                        return false;
                    }
                }
                strValue = currentElement.ToString();
                return true;
            }
            catch (Exception e)
            {
                DataCenter.WriteLogFile($"GetPropertyFromReserve 方法出错,TableName：{TableName},ReserveName：{ReserveName},iEquipNo：{iEquipNo},iChedianNo：{iChedianNo},NodeName：{NodeName},异常内容：{e.ToString()}");
            }
            return false;
        }

        public static bool GetPropertyFromReserveWithEquipTableRow(out string strValue, string ReserveName, string NodeName, EquipTableRow R)
        {
            string strReserve = "";
            strValue = "";
            try
            {
                if (R == null)
                {
                    return false;
                }
                if (ReserveName.ToLower() == "reserve1")
                    strReserve = R.Reserve1;
                else if (ReserveName.ToLower() == "reserve2")
                    strReserve = R.Reserve2;
                else if (ReserveName.ToLower() == "reserve3")
                    strReserve = R.Reserve3;


                //空字符串直接返回
                if (string.IsNullOrEmpty(strReserve) || string.IsNullOrWhiteSpace(strReserve)) { return false; }
                // 解析 JSON 字符串
                JsonDocument doc = JsonDocument.Parse(strReserve);
                JsonElement currentElement = doc.RootElement;

                foreach (var segment in NodeName.Split("#"))
                {
                    if (currentElement.ValueKind == JsonValueKind.Object &&
                        currentElement.TryGetProperty(segment, out JsonElement nextElement))
                    {
                        currentElement = nextElement;
                    }
                    else
                    {
                        DataCenter.WriteLogFile($"GetPropertyFromReserve 的参数{NodeName}中的节点{segment}没有找到！");
                        return false;
                    }
                }
                strValue = currentElement.ToString();
                return true;
            }
            catch (Exception e)
            {
                DataCenter.WriteLogFile($"GetPropertyFromReserveWithEquipTableRow 方法出错,ReserveName：{ReserveName},iEquipNo：{R.equip_no},NodeName：{NodeName},异常内容：{e.ToString()}");
            }
            return false;
        }

        static void CheckAndCreateNode(JsonObject node, string[] keys, string value, int index = 0)
        {
            // 如果索引超过键的长度，结束递归
            if (index >= keys.Length) return;

            // 检查当前节点是否包含键
            if (!node.ContainsKey(keys[index]))
            {
                // 如果是最后一个键，添加键值对并打印消息
                if (index == keys.Length - 1)
                {
                    node.Add(keys[index], value);
                }
                else
                {
                    // 如果不是最后一个键，创建子节点并递归
                    JsonObject child = new JsonObject();
                    node.Add(keys[index], child);
                    CheckAndCreateNode(child, keys, value, index + 1);
                }
            }
            else
            {
                // 如果是最后一个键，更新值
                if (index == keys.Length - 1)
                {
                    node[keys[index]] = value;
                }
                else
                {
                    // 如果不是最后一个键，递归到下一个键
                    CheckAndCreateNode(node[keys[index]].AsObject(), keys, value, index + 1);
                }
            }
        }
        #endregion

    }

}
