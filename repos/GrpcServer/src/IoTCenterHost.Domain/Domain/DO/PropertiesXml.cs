//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.AppServices.Domain.CurveOptions;
using IoTCenterHost.AppServices.Domain.WebOptions;
using IoTCenterHost.Core.Extension;
using IoTCenterHost.Domain.Domain.DO.DbConnections;
using System.Reflection;

namespace IoTCenterHost.AppServices.Domain
{
    public class PropertiesXml
    {
        private static readonly object Locker = new();
        public static readonly PropertiesXml Default = Create();

        public string HostIpAddr { get; set; }

        public string HttpPort { get; set; }

        public string HttpsPort { get; set; }

        public string HttpsPassword { get; set; }

        public string HttpsCertFile { get; set; }

        public string SingleAppStart { get; set; }


        public string UILanguage { get; set; }
        public IEnumerable<DbConnection> DbConnections
        {
            get; set;
        }

        public CurveOption CurveOption { get; set; }


        public string PluginsPath { get; set; }

        public string StorageFile { get; set; }

        public List<int> DebugEquipNos { get; set; } = new();

        public WebApiOption WebApiOption { get; set; }

        public IEnumerable<string> AllowOrigins { get; set; } = Enumerable.Empty<string>();

        public RedisOption RedisOption { get; set; }


        private static PropertiesXml Create()
        {
            var isLocking = Monitor.TryEnter(Locker);
            if (!isLocking)
            {
                return new PropertiesXml();
            }

            try
            {
                var directoryInfo = new DirectoryInfo(Assembly.GetEntryAssembly().Location).Parent.Parent.FullName;

                string configurationXml = Path.Combine(directoryInfo, "data", "AlarmCenter", "AlarmCenterProperties.xml");

                var path = FileExtension.ReadXml(configurationXml, "HostServer", "HostIpAddr");

                string port = FileExtension.ReadXml(configurationXml, "HostServer", "MQPort");

                string portTwo = FileExtension.ReadXml(configurationXml, "HostServer", "MQPort2");

                string httpsPort = FileExtension.ReadXml(configurationXml, "HostServer", "HttpsPort");

                string httpsCertFile = FileExtension.ReadXml(configurationXml, "HostServer", "HttpsCertFile");

                string httpPort = FileExtension.ReadXml(configurationXml, "HostServer", "HttpPort");

                string language = GWDataCenter.DataCenter.GetPropertyFromPropertyService("CoreProperties.UILanguage", "", "");

                string password = FileExtension.ReadXml(configurationXml, "HostServer", "HttpsPassword");


                var dbConnections = new List<DbConnection>();

                if (MySqlDbConnection.HasDbKey())
                    dbConnections.Add(new MySqlDbConnection().Create());

                if (SqliteConnection.HasDbKey())
                    dbConnections.Add(new SqliteConnection().Create());


                var debugEquipNosStr =
                    GWDataCenter.DataCenter.GetPropertyFromPropertyService("AlarmCenter.Debug", "", "");
                var debugEquipNos = new List<int>();
                if (!string.IsNullOrWhiteSpace(debugEquipNosStr) && debugEquipNosStr != "0")
                    debugEquipNos = debugEquipNosStr.Split(",").Select(s => int.TryParse(s, out int t) ? t : 0).Where(s => s > 0).ToList();


                var pluginsPathStr = FileExtension.ReadXml(configurationXml, "HostServer", "PluginsPath");

                var storageFileStr = FileExtension.ReadXml(configurationXml, "HostServer", "StorageFile");

                var allowOriginsStr = GWDataCenter.DataCenter.GetPropertyFromPropertyService("AllowOrigins", "", "");

                var singleAppStartStr = FileExtension.ReadXml(configurationXml, "HostServer", "SingleAppStart");


                return new PropertiesXml
                {
                    HostIpAddr = path,
                    DbConnections = dbConnections,
                    HttpPort = httpPort,
                    HttpsPort = httpsPort,
                    HttpsCertFile = httpsCertFile,
                    HttpsPassword = password,
                    SingleAppStart = singleAppStartStr,
                    UILanguage = language,
                    DebugEquipNos = debugEquipNos,
                    CurveOption = CurveOption.Create(),
                    PluginsPath = pluginsPathStr,
                    StorageFile = storageFileStr,
                    WebApiOption = WebApiOption.Create(),
                    RedisOption = RedisOption.Create(),
                    AllowOrigins = allowOriginsStr.Split(","),
                };
            }
            catch (Exception)
            {
                return new PropertiesXml();
            }
            finally
            {
                Monitor.Exit(Locker);
            }
        }

        public static void Save(PropertiesXml inputProp)
        {
            var isLocking = Monitor.TryEnter(Locker);
            if (!isLocking)
            {
                return;
            }

            try
            {

                if (!string.IsNullOrEmpty(inputProp.HttpsPort) && inputProp.HttpsPort != Default.HttpsPort)
                {
                    GWDataCenter.DataCenter.SetPropertyToPropertyService("HostServer", "HttpsPort", inputProp.HttpsPort);
                    Default.HttpsPort = inputProp.HttpsPort;
                }

                if (!string.IsNullOrEmpty(inputProp.HttpsPort) && !string.IsNullOrEmpty(inputProp.HttpsPassword))
                {

                    GWDataCenter.DataCenter.SetPropertyToPropertyService("HostServer", "HttpsPassword", inputProp.HttpsPassword);
                    Default.HttpsPassword = inputProp.HttpsPassword;
                }

                if (!string.IsNullOrEmpty(inputProp.HttpsPort) && !string.IsNullOrEmpty(inputProp.HttpsCertFile)
                                                               && inputProp.HttpsCertFile != Default.HttpsCertFile)
                {
                    GWDataCenter.DataCenter.SetPropertyToPropertyService("HostServer", "HttpsCertFile", inputProp.HttpsCertFile);
                    Default.HttpsCertFile = inputProp.HttpsCertFile;
                }


                if (!string.IsNullOrEmpty(inputProp.HttpPort) && inputProp.HttpPort != Default.HttpPort)
                {
                    GWDataCenter.DataCenter.SetPropertyToPropertyService("HostServer", "HttpPort", inputProp.HttpPort);
                    Default.HttpPort = inputProp.HttpPort;
                }

                if (!string.IsNullOrEmpty(inputProp.HostIpAddr) && inputProp.HostIpAddr != Default.HostIpAddr)
                {
                    GWDataCenter.DataCenter.SetPropertyToPropertyService("HostServer", "HostIpAddr", inputProp.HostIpAddr);
                    Default.HostIpAddr = inputProp.HostIpAddr;
                }



                if (!string.IsNullOrEmpty(inputProp.SingleAppStart) && inputProp.SingleAppStart != Default.SingleAppStart)
                {
                    GWDataCenter.DataCenter.SetPropertyToPropertyService("HostServer", "SingleAppStart", inputProp.SingleAppStart);
                    Default.SingleAppStart = inputProp.SingleAppStart;
                }


                if (!string.IsNullOrEmpty(inputProp.UILanguage) && inputProp.UILanguage != Default.UILanguage)
                {
                    GWDataCenter.DataCenter.SetPropertyToPropertyService("CoreProperties.UILanguage", "",
                        inputProp.UILanguage);
                    Default.UILanguage = inputProp.UILanguage;
                }

                Default.DbConnections = inputProp.DbConnections;
                foreach (var dbConnect in Default.DbConnections)
                {
                    switch (dbConnect.DbName)
                    {
                        case MySqlDbConnection.MySqlDbName:
                            new MySqlDbConnection(dbConnect).Save();
                            break;
                        case SqliteConnection.SqliteDbName:
                            new SqliteConnection(dbConnect).Save();
                            break;
                    }
                }

                if (!Default.DebugEquipNos.SequenceEqual(inputProp.DebugEquipNos))
                {
                    GWDataCenter.DataCenter.SetPropertyToPropertyService("AlarmCenter.Debug", "",
                        string.Join(",", inputProp.DebugEquipNos));
                    Default.DebugEquipNos = inputProp.DebugEquipNos;
                }

                var curveComparer = new CurveOptionCompare();
                if (!curveComparer.Equals(inputProp.CurveOption, Default.CurveOption))
                {
                    inputProp.CurveOption.Save();
                    Default.CurveOption = inputProp.CurveOption;
                }

                if (!string.IsNullOrEmpty(inputProp.PluginsPath) && inputProp.PluginsPath != Default.PluginsPath)
                {
                    GWDataCenter.DataCenter.SetPropertyToPropertyService("HostServer", "PluginsPath",
                        inputProp.PluginsPath);
                    Default.PluginsPath = inputProp.PluginsPath;
                }

                if (!string.IsNullOrEmpty(inputProp.StorageFile) && inputProp.StorageFile != Default.StorageFile)
                {
                    GWDataCenter.DataCenter.SetPropertyToPropertyService("HostServer", "StorageFile",
                        inputProp.StorageFile);
                    Default.StorageFile = inputProp.StorageFile;
                }

                if (!Default.AllowOrigins.SequenceEqual(inputProp.AllowOrigins))
                {
                    GWDataCenter.DataCenter.SetPropertyToPropertyService("AllowOrigins", "", string.Join(",", inputProp.AllowOrigins));
                    Default.AllowOrigins = inputProp.AllowOrigins;
                }

                var webComparer = new WebApiOptionComparer();
                if (!webComparer.Equals(inputProp.WebApiOption, Default.WebApiOption))
                {
                    inputProp.WebApiOption.Save();
                    Default.WebApiOption = inputProp.WebApiOption;
                }
            }
            finally
            {
                Monitor.Exit(Locker);
            }
        }
    }

}
