//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter.Database;
using IoTCenterHost.AppServices.Domain.PO;
using IoTCenterHost.Domain.Domain.DO.DbConnections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IoTCenterHost.AppServices
{
    public static class InjectDBContextExtension
    {
        public static IServiceCollection InjectDBContext(this IServiceCollection services, IConfiguration configuration)
        {
            var dbConnectionType = MySqlDbConnection.IsSelected()
                ? MySqlDbConnection.MySqlDbName
                : SqliteConnection.SqliteDbName;
            switch (dbConnectionType)
            {
                case MySqlDbConnection.MySqlDbName:
                    var mysqlConn = new MySqlDbConnection().Create()?.ToString();
                    var connStr = !string.IsNullOrWhiteSpace(mysqlConn)
                        ? mysqlConn
                        : configuration.GetConnectionString("MySQL");

                    if (string.IsNullOrWhiteSpace(connStr))
                    {
                        throw new InvalidOperationException("MySQL 连接字符串无效。");
                    }

                    InjectMySQLDbContext(services, connStr);
                    break;

                case SqliteConnection.SqliteDbName:
                    InjectSqliteDbContext(services);
                    break;
            }

            return services;
        }

        private static void InjectSqliteDbContext(IServiceCollection services)
        {
            var conn = new SqliteConnection().Create();
            var filePath = conn.DataFilePath;

            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException("SQLite 数据库文件不存在。", filePath);
            }

            string sqliteConnStr = $"filename={filePath}";

            services.AddDbContext<GWDataContext>(options =>
            {
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
                options.UseSqlite(sqliteConnStr);
            }, ServiceLifetime.Scoped);

            services.AddDbContext<GanweiDbContext>(options =>
            {
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
                options.UseSqlite(sqliteConnStr);
            }, ServiceLifetime.Scoped);
        }

        private static void InjectMySQLDbContext(IServiceCollection services, string connStr)
        {
            services.AddDbContext<GanweiDbContext>(options =>
            {
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                options.UseMySql(connStr, ServerVersion.AutoDetect(connStr));
            }, ServiceLifetime.Scoped);
        }
    }
}
