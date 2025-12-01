﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
namespace GWDataCenter.Database
{
    public class GWDbProvider
    {
        public static object lockstate = true;
        static readonly string DatabaseProperty = "AlarmCenter.Gui.OptionPanels.DatabaseOptions";
        IDatabase Instance = null;
        public ServiceProvider serviceProvider = null;
        public string GetPWD(Properties properties, string DBSelect)
        {
            bool isJiaMi = false;
            if (properties.Contains("JiaMi"))
            {
                if (properties.Get("JiaMi", "False") == "True")
                    isJiaMi = true;
            }
            switch (DBSelect)
            {
                case "Access.Select":
                    if (isJiaMi)
                        return DataCenter.AESDecrypt(properties.Get("Access.Password", ""), DataCenter.GeneratAESKey());
                    else
                        return properties.Get("Access.Password", "");
                case "SQLServer.Select":
                    if (isJiaMi)
                        return DataCenter.AESDecrypt(properties.Get("SQLServer.PWD", ""), DataCenter.GeneratAESKey());
                    else
                        return properties.Get("SQLServer.PWD", "");
                case "MySql.Select":
                    if (isJiaMi)
                        return DataCenter.AESDecrypt(properties.Get("MySql.PWD", ""), DataCenter.GeneratAESKey());
                    else
                        return properties.Get("MySql.PWD", "");
            }
            return "";
        }
        public ServiceProvider Initialize<T>() where T : DbContext
        {
            Properties properties = PropertyService.Get(DatabaseProperty, new Properties());
            if (properties.Get("SQLite.Select", false))
            {
                if (Instance == null)
                {
                    Instance = new GWDbProvider4SQLite();
                    serviceProvider = Instance.Initialize<T>(GetPWD(properties, "SQLite.Select"));
                }
            }
            if (properties.Get("MySql.Select", false))
            {
                if (Instance == null)
                {
                    Instance = new GWDbProvider4MySql();
                    serviceProvider = Instance.Initialize<T>(GetPWD(properties, "MySql.Select"));
                }
            }
            if (properties.Get("Access.Select", false))
            {
                throw new Exception("Access数据库 未实现");
            }
            if (properties.Get("SQLServer.Select", false))
            {
                throw new Exception("SQL Server 数据库 未实现");
            }
            if (serviceProvider == null)
                throw new Exception("数据库未能初始化!!!");
            return serviceProvider;
        }
    }
    class GWDbProvider4SQLite : IDatabase
    {
        readonly string DatabaseProperty = "AlarmCenter.Gui.OptionPanels.DatabaseOptions";
        public object lockstate = true;
        public IServiceCollection serviceCollection = new ServiceCollection();
        public ServiceProvider serviceProvider = null;
        public GWDataContext GetDB()
        {
            lock (lockstate)
            {
                return serviceProvider.GetService<GWDataContext>();
            }
        }
        public GWDbProvider4SQLite()
        {
            serviceCollection.AddLogging();
            serviceCollection.AddMemoryCache();
        }
        public ServiceProvider Initialize<T>(string csPWD) where T : DbContext
        {
            try
            {
                serviceCollection.AddDbContextPool<T>(c =>
                {
                    string RootPathName = Path.Combine(General.GetApplicationRootPath(), "database");
                    System.IO.Directory.CreateDirectory(RootPathName);
                    string pathname = Path.Combine(RootPathName, "Database.db");
                    Properties properties = PropertyService.Get(DatabaseProperty, new Properties());
                    if (properties.Get("SQLite.Select", false))
                    {
                        c.UseSqlite(@"Filename=" + pathname);
                    }
                });
                serviceProvider = serviceCollection.BuildServiceProvider();
                if (typeof(T).Name.Equals("GWDataContext"))
                {
                    using (var db = serviceProvider.GetService<GWDataContext>())
                    {
                        if (!db.GWSnapshotConfigTable.Any())
                        {
                            var SnapshotConfigs = new GWSnapshotConfigTableRow[]
                            {
                new GWSnapshotConfigTableRow { SnapshotName = "故障",   SnapshotLevelMin = 10003,SnapshotLevelMax=10004,
                    MaxCount=-1,IsShow=1,IconRes="Errors.png" },
                new GWSnapshotConfigTableRow { SnapshotName = "警告",   SnapshotLevelMin = 2,SnapshotLevelMax=9,
                    MaxCount=-1,IsShow=1,IconRes="Warnings.png" },
                new GWSnapshotConfigTableRow { SnapshotName = "信息",   SnapshotLevelMin = 0,SnapshotLevelMax=1,
                    MaxCount=-1,IsShow=1,IconRes="Informations.png" },
                new GWSnapshotConfigTableRow { SnapshotName = "设置",   SnapshotLevelMin = 10001,SnapshotLevelMax=10001,
                    MaxCount=-1,IsShow=1,IconRes="Settings.png" },
                new GWSnapshotConfigTableRow { SnapshotName = "资产",   SnapshotLevelMin = 10002,SnapshotLevelMax=10002,
                    MaxCount=-1,IsShow=1,IconRes="Assets.png" }
                            };
                            foreach (GWSnapshotConfigTableRow s in SnapshotConfigs)
                            {
                                db.GWSnapshotConfigTable.Add(s);
                            }
                        }
                        db.SaveChanges();
                    }
                }
                return serviceProvider;
            }
            catch (Exception e)
            {
                GWDataCenter.DataCenter.WriteLogFile(e.ToString());
                return null;
            }
        }
    }
    class GWDbProvider4MySql : IDatabase
    {
        readonly string DatabaseProperty = "AlarmCenter.Gui.OptionPanels.DatabaseOptions";
        public object lockstate = true;
        public IServiceCollection serviceCollection = new ServiceCollection();
        public ServiceProvider serviceProvider = null;
        public GWDataContext GetDB()
        {
            lock (lockstate)
            {
                return serviceProvider.GetService<GWDataContext>();
            }
        }
        public ServiceProvider Initialize<T>(string csPWD) where T : DbContext
        {
            serviceCollection.AddDbContextPool<T>(c =>
            {
                Properties properties = PropertyService.Get(DatabaseProperty, new Properties());
                string cnnstr = @"Server="
                + properties.Get("MySql.IP", "") + ";" + "Database="
                + properties.Get("MySql.Database", "") + ";" + "Uid="
                + properties.Get("MySql.UID", "") + ";" + "Port="
                + properties.Get("MySql.PORT", "3306") + ";" + "Pwd="
                + csPWD;
                if (properties.Get("MySql.Select", false))
                {
                    c.UseMySql(cnnstr, ServerVersion.Create(8, 0, 0, Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MySql), option => option.EnableRetryOnFailure(1024));
                }
            });
            serviceProvider = serviceCollection.BuildServiceProvider();
            if (typeof(T).Name.Equals("GWDataContext"))
            {
                using (var db = serviceProvider.GetService<GWDataContext>())
                {
                    if (!db.GWSnapshotConfigTable.Any())
                    {
                        var SnapshotConfigs = new GWSnapshotConfigTableRow[]
                        {
                new GWSnapshotConfigTableRow { SnapshotName = "故障",   SnapshotLevelMin = 10003,SnapshotLevelMax=10004,
                    MaxCount=-1,IsShow=1,IconRes="Errors.png" },
                new GWSnapshotConfigTableRow { SnapshotName = "警告",   SnapshotLevelMin = 2,SnapshotLevelMax=9,
                    MaxCount=-1,IsShow=1,IconRes="Warnings.png" },
                new GWSnapshotConfigTableRow { SnapshotName = "信息",   SnapshotLevelMin = 0,SnapshotLevelMax=1,
                    MaxCount=-1,IsShow=1,IconRes="Informations.png" },
                new GWSnapshotConfigTableRow { SnapshotName = "设置",   SnapshotLevelMin = 10001,SnapshotLevelMax=10001,
                    MaxCount=-1,IsShow=1,IconRes="Settings.png" },
                new GWSnapshotConfigTableRow { SnapshotName = "资产",   SnapshotLevelMin = 10002,SnapshotLevelMax=10002,
                    MaxCount=-1,IsShow=1,IconRes="Assets.png" }
                        };
                        foreach (GWSnapshotConfigTableRow s in SnapshotConfigs)
                        {
                            db.GWSnapshotConfigTable.Add(s);
                        }
                    }
                    db.SaveChanges();
                }
            }
            return serviceProvider;
        }
    }
}
