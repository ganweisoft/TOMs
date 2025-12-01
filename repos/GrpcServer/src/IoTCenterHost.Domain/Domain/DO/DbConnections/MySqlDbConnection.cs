//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.AppServices.Domain;
using IoTCenterHost.Core.Extension;

namespace IoTCenterHost.Domain.Domain.DO.DbConnections
{
    public class MySqlDbConnection : DbConnection
    {
        public MySqlDbConnection()
        {
        }
        public MySqlDbConnection(DbConnection dbConnection) : base(dbConnection)
        {

        }

        public const string MySqlDbName = "MySql";

        public override string DbName { get; set; } = MySqlDbName;

        public static new bool HasDbKey()
        {
            return !string.IsNullOrEmpty(FileExtension.ReadXml(DbConnection.ConfigurationXml, "AlarmCenter.Gui.OptionPanels.DatabaseOptions", $"{MySqlDbName}.Database"));

        }

        public static new bool IsSelected()
        {
            return HasDbKey() && string.Equals(FileExtension.ReadXml(DbConnection.ConfigurationXml, "AlarmCenter.Gui.OptionPanels.DatabaseOptions", $"{MySqlDbName}.Select").ToUpper(), "true", StringComparison.InvariantCultureIgnoreCase);
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(DbName))
                return $"Data Source={IP};port={Port};Initial Catalog={Source};user id={Uid};password={Pwd};default command timeout=120;charset=utf8mb4;{Ssl}";
            return string.Empty;
        }
    }
}
