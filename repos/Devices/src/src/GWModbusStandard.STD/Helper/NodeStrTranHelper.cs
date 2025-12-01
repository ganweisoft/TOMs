// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTClient.Enums;
using IoTClient.Models;
using System.Collections.Concurrent;

namespace GWModbusStandard.STD
{
    public class NodeStrTranHelper
    {
        private ConcurrentDictionary<string, ModbusInput> ModbusInputCache = new ConcurrentDictionary<string, ModbusInput>();

        public ModbusInput GetModbusInput(byte stationNumber, string nodeStr)
        {
            // Check the cache first
            var key = $"{stationNumber}_{nodeStr}";
            if (ModbusInputCache.TryGetValue(key, out var cachedInput))
            {
                // Return the cached object with updated station number
                // cachedInput.StationNumber = stationNumber;
                return cachedInput;
            }

            var strArray = nodeStr.Split(',');

            if (strArray.Length != 3 ||
                !byte.TryParse(strArray[0], out byte functionCode) ||
                !Enum.TryParse(strArray[2], out DataTypeEnum dataTypeEnum))
            {
                return null;
            }

            // Create a new ModbusInput object and add it to the cache
            var modbusInput = new ModbusInput
            {
                Address = strArray[1].PadLeft(4, '0'),
                DataType = dataTypeEnum,
                FunctionCode = functionCode,
                StationNumber = stationNumber
            };
            modbusInput.SetNodeStr(nodeStr);

            ModbusInputCache[key] = modbusInput;

            return modbusInput;
        }

        public List<ModbusInput> GetModbusInputList(byte stationNumber, string[] nodeStrs)
        {
            var result = new List<ModbusInput>(nodeStrs.Length);

            foreach (var nodeStr in nodeStrs)
            {
                var modbusInput = GetModbusInput(stationNumber, nodeStr);
                if (modbusInput != null)
                {
                    result.Add(modbusInput);
                }
            }

            return result;
        }

        public void ClearCache()
        {
            ModbusInputCache.Clear();
        }
    }
}
