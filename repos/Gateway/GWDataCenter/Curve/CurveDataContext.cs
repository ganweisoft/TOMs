﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Reflection;
namespace GWDataCenter.Database
{
    public class CurveDataTableRow
    {
        [Key]
        [Required]
        [Display(Name = "key")]
        public Int64 key { get; set; }
        [Display(Name = "历史数据")]
        public byte[] curvedata { get; set; }
        [Display(Name = "保留字段1")]
        public string Reserve1 { get; set; }
        [Display(Name = "保留字段2")]
        public string Reserve2 { get; set; }
        [Display(Name = "保留字段3")]
        public string Reserve3 { get; set; }
    }
    public class DynamicModelCacheKeyFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context, bool designTime)
        {
            return new { Type = context.GetType(), designTime };
        }
    }
    public static class EntityFrameworkCoreExtensions
    {
        private static DbCommand CreateCommand(DatabaseFacade facade, string sql, out DbConnection connection, params object[] parameters)
        {
            var conn = facade.GetDbConnection();
            connection = conn;
            conn.Open();
            if (facade.IsMySql())
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.Parameters.AddRange(parameters);
                return cmd;
            }
            return null;
        }
        public static DataTable SqlQuery(this DatabaseFacade facade, string sql, params object[] parameters)
        {
            var command = CreateCommand(facade, sql, out DbConnection conn, parameters);
            var reader = command.ExecuteReader();
            var dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            conn.Close();
            return dt;
        }
        public static List<T> SqlQuery<T>(this DatabaseFacade facade, string sql, params object[] parameters) where T : class, new()
        {
            var dt = SqlQuery(facade, sql, parameters);
            return dt.ToList<T>();
        }
        public static List<T> ToList<T>(this DataTable dt) where T : class, new()
        {
            var propertyInfos = typeof(T).GetProperties();
            var list = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                var t = new T();
                foreach (PropertyInfo p in propertyInfos)
                {
                    if (dt.Columns.IndexOf(p.Name) != -1 && row[p.Name] != DBNull.Value)
                        p.SetValue(t, row[p.Name], null);
                }
                list.Add(t);
            }
            return list;
        }
    }
    public class GWCurveContext : DbContext
    {
        public static object lockstate = true;
        public string strCurveDate { get; set; }
        public DbSet<CurveDataTableRow> CurveDataTable { get; set; }
        string DatabaseProperty = "AlarmCenter.Gui.OptionPanels.DatabaseOptions";
        string ConnectStr;
        Properties properties;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (properties.Get("SQLite.Select", false))
            {
                string RootPathName = Path.Combine(General.GetApplicationRootPath(), "database");
                System.IO.Directory.CreateDirectory(RootPathName);
                string pathname = Path.Combine(RootPathName, "Database.db");
                ConnectStr = @"Filename=" + pathname;
                optionsBuilder.UseSqlite(ConnectStr);
            }
            if (properties.Get("MySql.Select", false))
            {
                ConnectStr = @"Server="
                + properties.Get("MySql.IP", "") + ";" + "Database="
                + properties.Get("MySql.Database", "") + ";" + "Uid="
                + properties.Get("MySql.UID", "") + ";" + "Pwd="
                + GetPWD(properties, "MySql.Select");
                optionsBuilder.UseMySql(ConnectStr, ServerVersion.Create(8, 0, 0, Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MySql), option => option.EnableRetryOnFailure(1024));
            }
            if (properties.Get("Access.Select", false))
            {
                throw new Exception("Access数据库 未实现");
            }
            if (properties.Get("SQLServer.Select", false))
            {
                throw new Exception("SQL Server 数据库 未实现");
            }
            optionsBuilder.ReplaceService<IModelCacheKeyFactory, DynamicModelCacheKeyFactory>();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CurveDataTableRow>().ToTable(strCurveDate);
        }
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
        public GWCurveContext()
        {
            properties = PropertyService.Get(DatabaseProperty, new Properties());
        }
    }
}
