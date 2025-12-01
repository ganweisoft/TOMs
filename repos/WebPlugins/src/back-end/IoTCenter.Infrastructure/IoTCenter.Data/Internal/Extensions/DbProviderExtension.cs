// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Data.Database;
using IoTCenter.Data.Model;
using IoTCenter.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace IoTCenter.Data.Internal.Extensions
{
    public static class DbProviderExtension
    {
        private static object lockObj = new object();
        private static bool Initialized = false;
        public static IServiceCollection AddIoTDbContext<T>(this IServiceCollection services, IConfiguration configuration)
            where T : DbContext
        {
            using var provider = services.BuildServiceProvider();

            var xpath = CommonUtil.GetPropertiesPath();

            if (string.IsNullOrEmpty(xpath))
            {
                throw new ArgumentNullException(nameof(configuration), "未配置读取数据库路径");
            }

            var root = XElement.Load(xpath);

            var elements = from el in root.Elements("Properties")
                           where (string)el.Attribute("name") == "AlarmCenter.Gui.OptionPanels.DatabaseOptions"
                           select el;

            var isEncrypt = GetXMLBool(GetXmlVal(elements, "JiaMi"));

            var mysqlElement = GetXmlVal(elements, "MySql.Select");

            var mysqlSelected = GetXMLBool(mysqlElement);

            if (mysqlSelected)
            {
                var ip = GetXmlVal(elements, "MySql.IP");

                if (string.IsNullOrEmpty(ip))
                {
                    throw new Exception("使用MySql数据库，但节点“MySql.IP”值为空");
                }

                var pwd = GetXmlVal(elements, "MySql.PWD");
                if (string.IsNullOrEmpty(pwd))
                {
                    throw new Exception("使用MySql数据库，但节点“MySql.PWD”值为空");
                }

                var port = GetXmlVal(elements, "MySql.PORT");

                var uid = GetXmlVal(elements, "MySql.UID");

                if (string.IsNullOrEmpty(uid))
                {
                    throw new Exception("使用MySql数据库，但节点“MySql.UID”值为空");
                }

                var database = GetXmlVal(elements, "MySql.Database");

                if (string.IsNullOrEmpty(database))
                {
                    throw new Exception("使用MySql数据库，但节点“MySql.Database”值为空");
                }


                var connectionString = $"server={ip};";

                if (!string.IsNullOrEmpty(port))
                {
                    connectionString = $"{connectionString}port={port};";
                }

                connectionString = $"{connectionString}database={database};uid={uid};pwd={pwd};charset=utf8mb4;SslMode=preferred;allowPublicKeyRetrieval=true";

                services.AddDbContext<T>(d => d.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)), ServiceLifetime.Scoped);

                services.AddScoped<IDatabaseRepository, MySqlRepository>();
            }
            else
            {
                var sqlitePath = GetXmlVal(elements, "SQLite.DefaultPath");

                if (string.IsNullOrEmpty(sqlitePath))
                {
                    throw new Exception("使用Sqlite数据库，但节点“SQLite.DefaultPath”值为空");
                }

                if (!File.Exists(sqlitePath))
                {
                    throw new Exception($"使用Sqlite数据库，但根据配置路径：({sqlitePath})未能找到数据库文件");
                }

                services.AddDbContextPool<T>(d => d.UseSqlite($"Filename={sqlitePath}"));

                services.AddScoped<IDatabaseRepository, SqliteRepository>();
            }


            return services;
        }

        public static void AddDefaultSeedData(GWDbContext context)
        {
            var defaultUser = context.Gwuser.FirstOrDefault(d => d.Name == "admin");

            if (defaultUser == null)
            {
                defaultUser = new Gwuser()
                {
                    Name = "admin",
                    Password = "ganwei.123",
                    Roles = "ADMIN",
                    ControlLevel = "1",
                    LockoutEnabled = false,
                    AccessFailedCount = 0,
                    PwdUpdateTime = DateTime.Now
                };

                var securityStamp = Guid.NewGuid().ToString("N");

                defaultUser.SetSecurityStamp(securityStamp);

                context.Gwuser.Add(defaultUser);

                context.SaveChanges();
            }

            var gz = context.GwsnapshotConfig.FirstOrDefault(g => g.SnapshotName == "故障");
            if (gz == null)
            {
                context.GwsnapshotConfig.Add(new GwsnapshotConfig()
                {
                    SnapshotName = "故障",
                    SnapshotLevelMin = 10003,
                    SnapshotLevelMax = 10005,
                    MaxCount = -1,
                    IsShow = 1,
                    IconRes = "Errors.png"
                });
            }

            var jg = context.GwsnapshotConfig.FirstOrDefault(g => g.SnapshotName == "警告");
            if (jg == null)
            {
                context.GwsnapshotConfig.Add(new GwsnapshotConfig()
                {
                    SnapshotName = "警告",
                    SnapshotLevelMin = 2,
                    SnapshotLevelMax = 9,
                    MaxCount = -1,
                    IsShow = 1,
                    IconRes = "Warnings.png"
                });
            }

            var xx = context.GwsnapshotConfig.FirstOrDefault(g => g.SnapshotName == "信息");
            if (xx == null)
            {
                context.GwsnapshotConfig.Add(new GwsnapshotConfig()
                {
                    SnapshotName = "信息",
                    SnapshotLevelMin = 0,
                    SnapshotLevelMax = 1,
                    MaxCount = -1,
                    IsShow = 1,
                    IconRes = "Informations.png"
                });
            }

            var zc = context.GwsnapshotConfig.FirstOrDefault(g => g.SnapshotName == "资产");
            if (zc == null)
            {
                context.GwsnapshotConfig.Add(new GwsnapshotConfig()
                {
                    SnapshotName = "资产",
                    SnapshotLevelMin = 10002,
                    SnapshotLevelMax = 10002,
                    MaxCount = -1,
                    IsShow = 1,
                    IconRes = "Assets.png"
                });
            }

            var sz = context.GwsnapshotConfig.FirstOrDefault(g => g.SnapshotName == "设置");
            if (sz == null)
            {
                context.GwsnapshotConfig.Add(new GwsnapshotConfig()
                {
                    SnapshotName = "设置",
                    SnapshotLevelMin = 10001,
                    SnapshotLevelMax = 10001,
                    MaxCount = -1,
                    IsShow = 1,
                    IconRes = "Settings.png"
                });
            }

            var group = context.EGroup.FirstOrDefault(d => d.GroupName == "综合管理平台");
            if (group == null)
            {
                context.EGroup.Add(new EGroup()
                {
                    GroupName = "综合管理平台",
                    ParentGroupId = 0
                });
            }

            context.SaveChanges();

            var iotequip = context.IotEquip.AsNoTracking()
                .FirstOrDefault(e => e.EquipNm == "温湿度");
            
            if (iotequip == null)
            {
                iotequip = new IotEquip()
                {
                    StaN = 1,
                    EquipNm = "温湿度",
                    EquipDetail = "TCP",
                    AccCyc = 1,
                    ProcAdvice = "设备故障",
                    CommunicationDrv = "BCDataSimu.STD.dll",
                    LocalAddr = "4ec839503b7f4ec79ef6383f808e3c24",
                    EquipAddr = "1fd57489523f4e6193017261675d5451",
                    CommunicationTimeParam = "1000/6/16/400",
                    RawEquipNo = 4,
                    AlarmScheme = 0,
                    Attrib = 0,
                    AlarmRiseCycle = 0,
                    EquipConnType = 0
                };

                context.IotEquip.Add(iotequip);

                context.SaveChanges();

                context.IotYcp.AddRange(new List<IotYcp>()
                {
                    new IotYcp()
                    {
                        StaN = 1,
                        EquipNo = iotequip.EquipNo,
                        YcNm = "温度",
                        Mapping = false,
                        YcMin = 31,
                        YcMax = 32,
                        PhysicMin = 15,
                        PhysicMax = 20,
                        ValMin = 15,
                        RestoreMin = 16,
                        RestoreMax = 31,
                        ValMax = 32,
                        ValTrait = 0,
                        MainInstruction = "0",
                        MinorInstruction = "1",
                        AlarmAcceptableTime = 0,
                        RestoreAcceptableTime = 0,
                        AlarmRepeatTime = 0,
                        LvlLevel = 3,
                        OutminEvt = "",
                        OutmaxEvt = "",
                        WaveFile = "",
                        RelatedPic = "",
                        AlarmScheme = 0,
                        CurveRcd = false
                    },
                    new IotYcp()
                    {
                        StaN = 1,
                        EquipNo = iotequip.EquipNo,
                        YcNm = "湿度",
                        Mapping = false,
                        YcMin = 80,
                        YcMax = 90,
                        PhysicMin = 20,
                        PhysicMax = 30,
                        ValMin = 15,
                        RestoreMin = 16,
                        RestoreMax = 80,
                        ValMax = 90,
                        ValTrait = 0,
                        MainInstruction = "0",
                        MinorInstruction = "1",
                        AlarmAcceptableTime = 0,
                        RestoreAcceptableTime = 0,
                        AlarmRepeatTime = 0,
                        LvlLevel = 3,
                        OutminEvt = "",
                        OutmaxEvt = "",
                        WaveFile = "",
                        RelatedPic = "",
                        AlarmScheme = 0,
                    }
                });

                context.SaveChanges();

                context.IotSetParm.AddRange(new List<IotSetParm>()
                {
                    new IotSetParm()
                    {
                        StaN = 1,
                        EquipNo = iotequip.EquipNo,
                        SetNm = "遥测设置1",
                        SetType = "V",
                        MainInstruction = "SetYCYXValue",
                        MinorInstruction = "C_1",
                        Record = true,
                        Action = "设置",
                        Value = "1",
                        Canexecution = false,
                        EnableVoice = false,
                        QrEquipNo = 0
                    },
                    new IotSetParm()
                    {
                        StaN = 1,
                        EquipNo = iotequip.EquipNo,
                        SetNm = "遥测设置2",
                        SetType = "V",
                        MainInstruction = "SetYCYXValue",
                        MinorInstruction = "C_2",
                        Record = true,
                        Action = "设置",
                        Value = "1",
                        Canexecution = false,
                        EnableVoice = false,
                        QrEquipNo = 0
                    }
                });

                context.SaveChanges();
            }
        }

        public static void ApplyMigrations(IServiceProvider serviceProvider)
        {
            if (Initialized)
            {
                return;
            }

            lock (lockObj)
            {
                if (!Initialized)
                {
                    try
                    {
                        var dbContext = serviceProvider.GetRequiredService<GWDbContext>();

                        dbContext.Database.Migrate();

                        Initialized = true;
                    }
                    catch
                    {
                        Initialized = true;
                    }
                    finally
                    {
                        var dbContext = serviceProvider.GetRequiredService<GWDbContext>();
                        AddDefaultSeedData(dbContext);
                    }
                }
            }
        }

        private static bool GetXMLBool(string val)
        {
            return val?.ToLower() == "true";
        }

        private static string GetXmlVal(IEnumerable<XElement> elements,
            string parameter)
        {
            var element = elements.Elements(parameter).FirstOrDefault();
            return element?.Attribute("value")?.Value;
        }
    }
}
