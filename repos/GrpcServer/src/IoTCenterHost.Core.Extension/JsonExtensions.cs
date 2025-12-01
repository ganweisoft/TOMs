//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;

namespace IoTCenterHost.Core.Extension
{
    public static class JsonExtensions
    {
        public static string ToJson<T>(this T obj) where T : class
        {
            var jSetting = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, jSetting);
        }
        public static string ToJsonEnumerable<T>(this T obj) where T : class
        {
            var arr = new List<T>();
            arr.Add(obj);
            var jSetting = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(arr, jSetting);

        }

        public static T FromJson<T>(this string obj) where T : class
        {
            var jSetting = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.All,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            if (!string.IsNullOrEmpty(obj))
            {
                try
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(obj);
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;

        }


        public static string ConvertJsonString(this string str)
        {
            JsonSerializer serializer = new JsonSerializer();
            TextReader tr = new StringReader(str);
            JsonTextReader jtr = new JsonTextReader(tr);
            object obj = serializer.Deserialize(jtr);
            if (obj != null)
            {
                StringWriter textWriter = new StringWriter();
                JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
                {
                    Formatting = Formatting.Indented,
                    Indentation = 4,
                    IndentChar = ' '
                };
                serializer.Serialize(jsonWriter, obj);
                return textWriter.ToString();
            }
            else
            {
                return str;
            }
        }

    }
}
