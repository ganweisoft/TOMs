// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using GWDataCenter.Database;
using OpenGWDataCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GWDataSimu.STD
{
    /// <summary>
    /// 设备模拟器类，用于模拟设备数据通信
    /// </summary>
    internal class CEquip : CEquipBase
    {
        #region 常量定义
        private const int DEFAULT_SLEEP_TIME = 500;
        private const int DEFAULT_COMM_CHECK_SLEEP = 100;
        private const int DEFAULT_FREQUENCY = 100000;
        private const string SETPARM_COUNT_INSTRUCTION = "SETPARMCOUNT";
        private const string SET_YC_YX_VALUE_INSTRUCTION = "SETYCYXVALUE";
        private const string ADD_EVENT_INSTRUCTION = "ADDEVENT";
        private const string SET_COMM_STATE_INSTRUCTION = "SETCOMMSTATE";
        private const string STRING_DATATYPE = "S";
        private const int DATETIME_PREFIX_LENGTH = 14;
        #endregion

        #region 私有字段
        private bool _firstEnter = true;
        private int _count;
        private readonly int _frequency = DEFAULT_FREQUENCY;
        private readonly Random _random = new Random(Guid.NewGuid().GetHashCode());
        private readonly object _commStateLock = new object();
        private bool _commState = true;
        private int _sleepTime = DEFAULT_SLEEP_TIME;
        private readonly Dictionary<int, int> _setParmCountDict = new Dictionary<int, int>();
        private int _icount;

        // 预编译的正则表达式，提高性能
        private static readonly Regex NumberRegex = new Regex(@"^-?[1-9]\d*\.\d+$|^-?0\.\d+$|^-?[1-9]\d*$|^0$", RegexOptions.Compiled);
        private static readonly Regex StringRegex = new Regex(@"""(?:[^""\\]|\\.)*""", RegexOptions.Compiled);
        private static readonly Regex SequenceRegex = new Regex(@"^-?\d+\.?(\d+)?(,\-?\d+\.?(\d+)?){1,6}$", RegexOptions.Compiled);
        #endregion

        #region 公共方法
        /// <summary>
        /// 初始化设备
        /// </summary>
        /// <param name="item">设备项</param>
        /// <returns>初始化是否成功</returns>
        public override bool init(EquipItem item)
        {
            if (!CheckCommunicationState())
            {
                return false;
            }

            if (!base.init(item))
            {
                return false;
            }

            if (base.ResetFlag || _firstEnter)
            {
                InitializeSleepTime();
                InitializeSetParmCountDict(item);
                _firstEnter = false;
            }

            return true;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="pEquip">设备对象</param>
        /// <returns>通信状态</returns>
        public override CommunicationState GetData(CEquipBase pEquip)
        {
            base.Sleep(_sleepTime, true);

            if (!CheckCommunicationState())
            {
                return CommunicationState.fail; // 假设0表示失败状态
            }

            return base.GetData(pEquip);
        }

        /// <summary>
        /// 获取YC（遥测）数据
        /// </summary>
        /// <param name="r">YC表行数据</param>
        /// <returns>处理是否成功</returns>
        public override bool GetYC(YcpTableRow r)
        {
            if (r == null) return false;

            // 处理特殊指令
            if (IsSetParmCountInstruction(r))
            {
                return HandleSetParmCount(r);
            }

            // 检查现有数据
            var existingData = base.GetYCData(r);
            if (existingData != null && IsValidExistingData(existingData, r))
            {
                base.SetYCData(r, existingData);
                return true;
            }

            // 生成新数据
            GenerateYCData(r);
            return true;
        }

        /// <summary>
        /// 获取YX（遥信）数据
        /// </summary>
        /// <param name="r">YX表行数据</param>
        /// <returns>处理是否成功</returns>
        public override bool GetYX(YxpTableRow r)
        {
            if (r == null) return false;

            var inversion = r.inversion;
            var yxNo = r.yx_no;
            var isHighLevel = r.level_r >= r.level_d;
            object value = 0;

            // 当datatype为null时的处理逻辑

            if (isHighLevel)
            {
                // 高电平状态
                if (inversion)
                {
                    // 反向逻辑：高电平时应该为true
                    if (base.YXResults.ContainsKey(yxNo) && !Convert.ToBoolean(base.YXResults[yxNo]))
                    {
                        return true; // 如果当前值为false，保持不变
                    }
                    value = true;
                }
                else
                {
                    // 正向逻辑：高电平时应该为false
                    if (base.YXResults.ContainsKey(yxNo) && Convert.ToBoolean(base.YXResults[yxNo]))
                    {
                        return true; // 如果当前值为true，保持不变
                    }
                    value = false;
                }
            }
            else
            {
                // 低电平状态
                if (inversion)
                {
                    // 反向逻辑：低电平时应该为false
                    if (base.YXResults.ContainsKey(yxNo) && Convert.ToBoolean(base.YXResults[yxNo]))
                    {
                        return true; // 如果当前值为true，保持不变
                    }
                    value = false;
                }
                else
                {
                    // 正向逻辑：低电平时应该为true
                    if (base.YXResults.ContainsKey(yxNo) && !Convert.ToBoolean(base.YXResults[yxNo]))
                    {
                        return true; // 如果当前值为false，保持不变
                    }
                    value = true;
                }
            }
            SetYXData(r, value);
            YXToPhysic(r);
            return true;
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="mainInstruct">主指令</param>
        /// <param name="minorInstruct">次指令</param>
        /// <param name="value">值</param>
        /// <returns>设置是否成功</returns>
        public override bool SetParm(string mainInstruct, string minorInstruct, string value)
        {
            if (string.IsNullOrEmpty(mainInstruct) || string.IsNullOrEmpty(minorInstruct))
            {
                return false;
            }

            try
            {
                return mainInstruct.ToUpper() switch
                {
                    SET_YC_YX_VALUE_INSTRUCTION => HandleSetYCYXValue(minorInstruct, value),
                    ADD_EVENT_INSTRUCTION => HandleAddEvent(minorInstruct),
                    SET_COMM_STATE_INSTRUCTION => HandleSetCommState(minorInstruct),
                    _ => false
                };
            }
            catch (Exception ex)
            {
                GWDataCenter.DataCenter.WriteLogFile($"SetParm error: {ex.Message}", LogType.Error);
                return false;
            }
        }

        /// <summary>
        /// 确认到正常状态
        /// </summary>
        /// <param name="sYcYxType">YC/YX类型</param>
        /// <param name="iYcYxNo">YC/YX编号</param>
        /// <returns>确认是否成功</returns>
        public override bool Confirm2NormalState(string sYcYxType, int iYcYxNo)
        {
            return true;
        }
        #endregion

        #region 私有辅助方法
        /// <summary>
        /// 检查通信状态
        /// </summary>
        /// <returns>通信是否正常</returns>
        private bool CheckCommunicationState()
        {
            lock (_commStateLock)
            {
                if (!_commState)
                {
                    base.Sleep(DEFAULT_COMM_CHECK_SLEEP, true);
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 初始化休眠时间
        /// </summary>
        private void InitializeSleepTime()
        {
            if (string.IsNullOrWhiteSpace(base.Equipitem.communication_param))
            {
                _sleepTime = DEFAULT_SLEEP_TIME;
            }
            else
            {
                int.TryParse(base.Equipitem.communication_param, out _sleepTime);
            }
        }

        /// <summary>
        /// 初始化设置参数计数字典
        /// </summary>
        /// <param name="item">设备项</param>
        private void InitializeSetParmCountDict(EquipItem item)
        {
            var setParmItems = StationItem.db_Setparm
                .Where(m => m.equip_no == item.iEquipno)
                .ToList();

            foreach (var setParmItem in setParmItems)
            {
                _setParmCountDict.TryAdd(setParmItem.set_no, 0);
            }
        }

        /// <summary>
        /// 检查是否为设置参数计数指令
        /// </summary>
        /// <param name="r">YC表行数据</param>
        /// <returns>是否为设置参数计数指令</returns>
        private bool IsSetParmCountInstruction(YcpTableRow r)
        {
            return r.main_instruction?.Trim().ToUpper() == SETPARM_COUNT_INSTRUCTION;
        }

        /// <summary>
        /// 处理设置参数计数
        /// </summary>
        /// <param name="r">YC表行数据</param>
        /// <returns>处理是否成功</returns>
        private bool HandleSetParmCount(YcpTableRow r)
        {
            try
            {
                if (int.TryParse(r.minor_instruction?.Trim(), out int key) &&
                    _setParmCountDict.ContainsKey(key))
                {
                    base.SetYCData(r, _setParmCountDict[key]);
                }
                return true;
            }
            catch (Exception ex)
            {
                GWDataCenter.DataCenter.WriteLogFile($"HandleSetParmCount error: {ex.Message}", LogType.Error);
                return true; // 即使出错也返回true，保持原有逻辑
            }
        }

        /// <summary>
        /// 检查现有数据是否有效
        /// </summary>
        /// <param name="existingData">现有数据</param>
        /// <param name="r">YC表行数据</param>
        /// <returns>数据是否有效</returns>
        private bool IsValidExistingData(object existingData, YcpTableRow r)
        {
            try
            {
                if (double.TryParse(existingData.ToString(), out double value))
                {
                    return (value <= r.val_min || value >= r.val_max);
                }

                // 对于字符串类型数据
                if (existingData is string)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 生成YC数据
        /// </summary>
        /// <param name="r">YC表行数据</param>
        private void GenerateYCData(YcpTableRow r)
        {
            var dataType = string.Empty;

            if (string.IsNullOrEmpty(dataType))
            {
                base.SetYCData(r, CreateYCValue(r));
            }
            else if (dataType == STRING_DATATYPE)
            {
                base.SetYCData(r, CreateYCValue(r).ToString());
            }
            else if (dataType.StartsWith("SEQ") && int.TryParse(dataType.Substring(3), out int seqCount))
            {
                GenerateSequenceData(r, seqCount);
            }
        }

        /// <summary>
        /// 生成序列数据
        /// </summary>
        /// <param name="r">YC表行数据</param>
        /// <param name="count">序列数量</param>
        private void GenerateSequenceData(YcpTableRow r, int count)
        {
            var values = new double[count];
            for (int i = 0; i < count; i++)
            {
                values[i] = CreateYCValue(r);
            }

            var tuple = count switch
            {
                2 => (object)(values[0], values[1]),
                3 => (values[0], values[1], values[2]),
                4 => (values[0], values[1], values[2], values[3]),
                5 => (values[0], values[1], values[2], values[3], values[4]),
                6 => (values[0], values[1], values[2], values[3], values[4], values[5]),
                7 => (values[0], values[1], values[2], values[3], values[4], values[5], values[6]),
                _ => (object)values[0]
            };

            base.SetYCData(r, tuple);
        }

        /// <summary>
        /// 创建YC值
        /// </summary>
        /// <param name="r">YC表行数据</param>
        /// <returns>生成的YC值</returns>
        private double CreateYCValue(YcpTableRow r)
        {
            var valMin = r.val_min;
            var valMax = r.val_max;

            // 如果设置了特殊标志位，生成超出范围的值
            if ((r.val_trait & 1) > 0)
            {
                return _random.Next((int)(valMax * 1.1), (int)(valMax * 1.2));
            }

            // 正常范围内生成值
            if (valMin + 1.0 <= valMax - 1.0)
            {
                return _random.Next((int)valMin + 1, (int)valMax - 1);
            }

            // 范围较小时的处理
            if (valMax - valMin <= 1.0)
            {
                return _random.Next((int)(valMin * 100.0 + 1.0), (int)(valMax * 100.0 - 1.0)) / 100.0;
            }

            return _random.Next((int)valMin, (int)valMax);
        }

        /// <summary>
        /// 处理设置YC/YX值
        /// </summary>
        /// <param name="minorInstruct">次指令</param>
        /// <param name="value">值</param>
        /// <returns>处理是否成功</returns>
        private bool HandleSetYCYXValue(string minorInstruct, string value)
        {
            if (minorInstruct.Length <= 2 || string.IsNullOrEmpty(value))
            {
                return false;
            }

            if (!int.TryParse(minorInstruct.Substring(2), out int number) || number <= 0)
            {
                return false;
            }

            var equipItem = GWDataCenter.DataCenter.GetEquipItem(base.m_equip_no);
            if (equipItem == null)
            {
                return false;
            }

            var firstChar = char.ToUpper(minorInstruct[0]);

            return firstChar switch
            {
                'C' => HandleSetYCValue(equipItem, number, value),
                'X' => HandleSetYXValue(number, value),
                _ => false
            };
        }

        /// <summary>
        /// 处理设置YC值
        /// </summary>
        /// <param name="equipItem">设备项</param>
        /// <param name="number">编号</param>
        /// <param name="value">值</param>
        /// <returns>处理是否成功</returns>
        private bool HandleSetYCValue(EquipItem equipItem, int number, string value)
        {
            lock (base.YCResults)
            {
                var dateTimeIndex = value.IndexOf(':');

                if (dateTimeIndex == DATETIME_PREFIX_LENGTH)
                {
                    var parsedValue = ParseDateTimeValue(value, dateTimeIndex);
                    equipItem.YCItemDict[number].YCValue = parsedValue;
                }
                else
                {
                    try
                    {
                        var doubleValue = Convert.ToDouble(value);
                        equipItem.YCItemDict[number].YCValue = doubleValue;
                        base.YCResults[number] = doubleValue;
                    }
                    catch
                    {
                        equipItem.YCItemDict[number].YCValue = value;
                        base.YCResults[number] = value;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 处理设置YX值
        /// </summary>
        /// <param name="number">编号</param>
        /// <param name="value">值</param>
        /// <returns>处理是否成功</returns>
        private bool HandleSetYXValue(int number, string value)
        {
            lock (base.YXResults)
            {
                base.YXResults[number] = Convert.ToInt32(value) > 0;
            }

            return true;
        }

        /// <summary>
        /// 解析日期时间值
        /// </summary>
        /// <param name="value">值字符串</param>
        /// <param name="dateTimeIndex">日期时间索引</param>
        /// <returns>解析后的元组</returns>
        private (DateTime, object) ParseDateTimeValue(string value, int dateTimeIndex)
        {
            var dateTimeStr = value.Substring(0, dateTimeIndex);
            var dataStr = value.Substring(dateTimeIndex + 1);

            var dateTime = DateTime.ParseExact(dateTimeStr, "yyyyMMddHHmmss", null);

            // 尝试解析不同类型的数据
            if (NumberRegex.IsMatch(dataStr))
            {
                return (dateTime, Convert.ToDouble(dataStr));
            }

            if (StringRegex.IsMatch(dataStr))
            {
                return (dateTime, dataStr);
            }

            if (SequenceRegex.IsMatch(dataStr))
            {
                var sequenceData = ParseSequenceData(dataStr);
                return (dateTime, sequenceData);
            }

            return (dateTime, dataStr);
        }

        /// <summary>
        /// 解析序列数据
        /// </summary>
        /// <param name="dataStr">数据字符串</param>
        /// <returns>解析后的序列对象</returns>
        private object ParseSequenceData(string dataStr)
        {
            var parts = dataStr.Split(',');
            var values = parts.Select(double.Parse).ToArray();

            return values.Length switch
            {
                2 => (values[0], values[1]),
                3 => (values[0], values[1], values[2]),
                4 => (values[0], values[1], values[2], values[3]),
                5 => (values[0], values[1], values[2], values[3], values[4]),
                6 => (values[0], values[1], values[2], values[3], values[4], values[5]),
                7 => (values[0], values[1], values[2], values[3], values[4], values[5], values[6]),
                _ => (object)values[0]
            };
        }

        /// <summary>
        /// 处理添加事件
        /// </summary>
        /// <param name="minorInstruct">次指令</param>
        /// <returns>处理是否成功</returns>
        private bool HandleAddEvent(string minorInstruct)
        {
            var equipEvent = new EquipEvent(minorInstruct, MessageLevel.Info, DateTime.Now);
            base.EquipEventList.Add(equipEvent);
            return true;
        }

        /// <summary>
        /// 处理设置通信状态
        /// </summary>
        /// <param name="minorInstruct">次指令</param>
        /// <returns>处理是否成功</returns>
        private bool HandleSetCommState(string minorInstruct)
        {
            if (minorInstruct.Trim() == "0")
            {
                lock (_commStateLock)
                {
                    _commState = false;
                }
            }

            return true;
        }

        /// <summary>
        /// 检查类型是否有效
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>类型是否有效</returns>
        private bool CheckType(Type type)
        {
            var genericArguments = type.GetGenericArguments();
            return genericArguments.Length == 2 && genericArguments[0] == typeof(DateTime);
        }
        #endregion
    }
}