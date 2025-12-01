//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.Core.Extension;
using System.Reflection;

namespace IoTCenterHost.AppServices.Domain
{

    public class DbConnection
    {
        public DbConnection(DbConnection dbConnection)
        {
            this.DbName = dbConnection.DbName;
            Source = dbConnection.Source;
            IP = dbConnection.IP;
            Pwd = dbConnection.Pwd;
            Uid = dbConnection.Uid;
            Select = dbConnection.Select;
            Port = dbConnection.Port;
            DataFilePath = dbConnection.DataFilePath;
        }

        public DbConnection()
        {

        }

        public virtual string DbName { get; set; }

        public virtual string Source { get; set; }

        public virtual string IP { get; set; }

        public virtual string Pwd { get; set; }

        public virtual string Uid { get; set; }

        public virtual string Select { get; set; }

        public virtual string Port { get; set; }

        public virtual string DataFilePath { get; set; }

        public virtual string Ssl { get; set; }

        public virtual bool IsSelect
        {
            get { return string.Equals(Select, "true", StringComparison.InvariantCultureIgnoreCase); }
        }

        public static string ConfigurationXml
        {
            get
            {
                var directoryInfo = new System.IO.DirectoryInfo(Assembly.GetEntryAssembly().Location).Parent.Parent
                    .FullName;
                return Path.Combine(directoryInfo, "data", "AlarmCenter", "AlarmCenterProperties.xml");
            }
        }

        public virtual bool HasDbKey()
        {
            return !string.IsNullOrEmpty(FileExtension.ReadXml(ConfigurationXml,
                "AlarmCenter.Gui.OptionPanels.DatabaseOptions", $"{DbName}.Database"));
        }

        public virtual bool IsSelected()
        {
            return string.Equals(
                FileExtension.ReadXml(ConfigurationXml, "AlarmCenter.Gui.OptionPanels.DatabaseOptions",
                    $"{DbName}.Select").ToUpper(), "true", StringComparison.InvariantCultureIgnoreCase);
        }

        public virtual DbConnection Create()
        {
            string database = FileExtension.ReadXml(DbConnection.ConfigurationXml,
                "AlarmCenter.Gui.OptionPanels.DatabaseOptions",
                $"{DbName}.Database"); //GWDataCenter.DataCenter.GetPropertyFromPropertyService("AlarmCenter.Gui.OptionPanels.DatabaseOptions", "MySql.Database", "");
            string ip = FileExtension.ReadXml(DbConnection.ConfigurationXml,
                "AlarmCenter.Gui.OptionPanels.DatabaseOptions",
                $"{DbName}.IP"); //GWDataCenter.DataCenter.GetPropertyFromPropertyService("AlarmCenter.Gui.OptionPanels.DatabaseOptions", "MySql.IP", "");
            string isEncryptStr = FileExtension.ReadXml(DbConnection.ConfigurationXml,
                "AlarmCenter.Gui.OptionPanels.DatabaseOptions",
                "JiaMi"); //GWDataCenter.DataCenter.GetPropertyFromPropertyService("AlarmCenter.Gui.OptionPanels.DatabaseOptions", "JiaMi", "");
            string port = FileExtension.ReadXml(DbConnection.ConfigurationXml,
                "AlarmCenter.Gui.OptionPanels.DatabaseOptions",
                $"{DbName}.PORT"); //GWDataCenter.DataCenter.GetPropertyFromPropertyService("AlarmCenter.Gui.OptionPanels.DatabaseOptions", "MySql.PORT", "");
            string select = FileExtension.ReadXml(DbConnection.ConfigurationXml,
                "AlarmCenter.Gui.OptionPanels.DatabaseOptions", $"{DbName}.Select");
            string pwd = FileExtension.ReadXml(DbConnection.ConfigurationXml,
                    "AlarmCenter.Gui.OptionPanels.DatabaseOptions", $"{DbName}.PWD"); //GWDataCenter.DataCenter.GetPropertyFromPropertyService("AlarmCenter.Gui.OptionPanels.DatabaseOptions", "MySql.PWD", "").DecryptStr(); 
            string uid = FileExtension.ReadXml(DbConnection.ConfigurationXml,
                "AlarmCenter.Gui.OptionPanels.DatabaseOptions",
                $"{DbName}.UID"); // GWDataCenter.DataCenter.GetPropertyFromPropertyService("AlarmCenter.Gui.OptionPanels.DatabaseOptions", "MySql.UID", "");
            string ssl = FileExtension.ReadXml(DbConnection.ConfigurationXml,
                "AlarmCenter.Gui.OptionPanels.DatabaseOptions", $"{DbName}.SSL");
            this.DbName = DbName;
            this.Source = database;
            this.IP = ip;
            this.Port = port;
            this.Select = select;
            this.Uid = uid;
            this.DataFilePath = "";
            this.Pwd = pwd;
            this.Ssl = ssl;
            return this;
        }

        public virtual void Save()
        {
            if (!string.IsNullOrEmpty(Uid))
                GWDataCenter.DataCenter.SetPropertyToPropertyService("AlarmCenter.Gui.OptionPanels.DatabaseOptions",
                    $"{DbName}.Database", Source);
            if (!string.IsNullOrEmpty(Uid))
                GWDataCenter.DataCenter.SetPropertyToPropertyService("AlarmCenter.Gui.OptionPanels.DatabaseOptions",
                    $"{DbName}.UID", Uid);
            if (!string.IsNullOrEmpty(IP))
                GWDataCenter.DataCenter.SetPropertyToPropertyService("AlarmCenter.Gui.OptionPanels.DatabaseOptions",
                    $"{DbName}.IP", IP);
            if (!string.IsNullOrEmpty(Port))
                GWDataCenter.DataCenter.SetPropertyToPropertyService("AlarmCenter.Gui.OptionPanels.DatabaseOptions",
                    $"{DbName}.PORT", Port);
            if (!string.IsNullOrEmpty(Pwd))
                GWDataCenter.DataCenter.SetPropertyToPropertyService("AlarmCenter.Gui.OptionPanels.DatabaseOptions",
                    $"{DbName}.PWD", Pwd);
            if (IsSelect)
            {
                GWDataCenter.DataCenter.SetPropertyToPropertyService("AlarmCenter.Gui.OptionPanels.DatabaseOptions",
                    $"{DbName}.Select",
                    System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(Select));
            }
            else
            {
                GWDataCenter.DataCenter.SetPropertyToPropertyService("AlarmCenter.Gui.OptionPanels.DatabaseOptions",
                    $"{DbName}.Select",
                    System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(Select));
            }
        }
    }
}
