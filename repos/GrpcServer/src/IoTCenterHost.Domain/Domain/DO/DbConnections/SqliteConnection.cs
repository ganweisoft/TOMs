//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.AppServices.Domain;
using IoTCenterHost.Core.Extension;

namespace IoTCenterHost.Domain.Domain.DO.DbConnections
{
    public class SqliteConnection : DbConnection
    {
        public SqliteConnection(DbConnection dbConnection) : base(dbConnection)
        {

        }

        public SqliteConnection()
        {
        }


        public string configurationXml = "";

        public const string SqliteDbName = "SQLite";
        public override string DbName { get; set; } = SqliteDbName;

        public new SqliteConnection Create()
        {
            string fileName = FileExtension.ReadXml(DbConnection.ConfigurationXml, "AlarmCenter.Gui.OptionPanels.DatabaseOptions", $"{DbName}.DefaultPath");

            if (Path.GetFullPath(fileName) != fileName)
            {
                FileExtension.WriteXml(DbConnection.ConfigurationXml, "AlarmCenter.Gui.OptionPanels.DatabaseOptions", $"{DbName}.DefaultPath", Path.GetFullPath(fileName));
            }

            string select = FileExtension.ReadXml(DbConnection.ConfigurationXml, "AlarmCenter.Gui.OptionPanels.DatabaseOptions", $"{DbName}.Select");
            DbConnection sqliteConnection = new SqliteConnection
            {
                Select = select,
                DataFilePath = Path.GetFullPath(fileName)
            };
            return (SqliteConnection)sqliteConnection;
        }

        public static new bool HasDbKey()
        {
            return !string.IsNullOrEmpty(FileExtension.ReadXml(DbConnection.ConfigurationXml, "AlarmCenter.Gui.OptionPanels.DatabaseOptions", $"{SqliteDbName}.DefaultPath"));
        }

        public static new bool IsSelected()
        {
            return HasDbKey() && string.Equals(FileExtension.ReadXml(DbConnection.ConfigurationXml, "AlarmCenter.Gui.OptionPanels.DatabaseOptions", $"{SqliteDbName}.Select").ToUpper(), "true", StringComparison.InvariantCultureIgnoreCase);
        }


        public override string ToString()
        {
            return FileExtension.ReadXml(DbConnection.ConfigurationXml, "AlarmCenter.Gui.OptionPanels.DatabaseOptions", $"{DbName}.DefaultPath");
        }

        public void Save()
        {
            if (this.IsSelect)
                GWDataCenter.DataCenter.SetPropertyToPropertyService("AlarmCenter.Gui.OptionPanels.DatabaseOptions", "SQLite.Select", System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(this.Select));
            else
            {
                GWDataCenter.DataCenter.SetPropertyToPropertyService("AlarmCenter.Gui.OptionPanels.DatabaseOptions", "SQLite.Select", System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(this.Select));
            }
        }
    }
}
