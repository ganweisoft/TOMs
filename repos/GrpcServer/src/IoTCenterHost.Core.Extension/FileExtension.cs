//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace IoTCenterHost.Core.Extension
{
    public static class FileExtension
    {
        public static string ReadString(this string filePath)
        {
            if (!System.IO.File.Exists(filePath))
                return string.Empty;
            using (StreamReader streamReader = new StreamReader(filePath))
                return streamReader.ReadToEnd();
        }

        public static void WriteStream(string filePath, string input)
        {
            using (StreamWriter streamWriter = new StreamWriter(filePath, false))
                streamWriter.Write(input);
        }
        public static string ReadXml(this string filePath, string name, string key, string property = "Properties")
        {
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = GetConfigXmlPath();
            }

            var root = XElement.Load(filePath);

            if (property == "Properties")
            {
                var elements = from el in root.Elements("Properties")
                               where (string)el.Attribute("name") == name
                               select el;

                var element = elements.Elements(key).FirstOrDefault();
                return element?.Attribute("value")?.Value;
            }
            else
            {
                return root.Elements(property).FirstOrDefault()?.Attribute("value")?.Value;
            }
            return string.Empty;
        }


        public static void WriteXml(this string filePath, string elementName, string key, string value, string group = "Properties")
        {
            if (string.IsNullOrEmpty(filePath))
                filePath = FileExtension.GetConfigXmlPath();

            if (!File.Exists(filePath))
                throw new FileNotFoundException("配置文件不存在", filePath);

            var root = XElement.Load(filePath);

            var groupElements = root.Elements(group);
            var targetElement = groupElements
                .FirstOrDefault(el => (string)el.Attribute("name") == elementName);

            if (targetElement == null)
            {
                targetElement = new XElement(group,
                    new XAttribute("name", elementName));
                root.Add(targetElement);
            }

            var keyElement = targetElement.Element(key);
            if (keyElement == null)
            {
                keyElement = new XElement(key);
                targetElement.Add(keyElement);
            }

            keyElement.SetAttributeValue("value", value);

            root.Save(filePath);
        }


        public static string GetConfigXmlPath()
        {
            var directoryInfo = new System.IO.DirectoryInfo(Assembly.GetEntryAssembly().Location).Parent.Parent.FullName;
            return Path.Combine(directoryInfo, "data", "AlarmCenter", "AlarmCenterProperties.xml");
        }

        public static void SetAppSettingValue(string section, string key, string value, string appSettingsJsonFilePath = null)
        {
            if (string.IsNullOrEmpty(appSettingsJsonFilePath))
            {
                appSettingsJsonFilePath = System.IO.Path.Combine(System.AppContext.BaseDirectory, "appsettings.json");
            }

            var json = System.IO.File.ReadAllText(appSettingsJsonFilePath);
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(json);

            if (!string.IsNullOrEmpty(key))
                jsonObj[section][key] = value;
            else
                jsonObj[section] = value;
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            using var streamWriter = new StreamWriter(appSettingsJsonFilePath, false);
            streamWriter.Write(output);
        }

        public static byte[] ReadStream(string filePath)
        {
            byte[] bytes = null;
            using (FileStream streamReader = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                bytes = new byte[streamReader.Length]; ;
                streamReader.Read(bytes, 0, (int)streamReader.Length);
            }
            return bytes;
        }
    }
}
