﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Threading;
namespace GWDataCenter.Database
{
    public class GWDataContext : DbContext
    {
        public DbSet<YcpTableRow> YcpTable { get; set; }
        public DbSet<YxpTableRow> YxpTable { get; set; }
        public DbSet<SetParmTableRow> SetParmTable { get; set; }
        public DbSet<EquipTableRow> EquipTable { get; set; }
        public DbSet<GWSnapshotConfigTableRow> GWSnapshotConfigTable { get; set; }
        public DbSet<SysEvtTableRow> SysEvtTable { get; set; }
        public DbSet<YcYxEvtTableRow> YcYxEvtTable { get; set; }
        public DbSet<AlarmProcTableRow> AlarmProcTable { get; set; }
        public DbSet<AutoProcTableRow> AutoProcTable { get; set; }
        public DbSet<GWProcTimeTListTableRow> GWProcTimeTListTable { get; set; }
        public DbSet<GWProcTimeEqpTableRow> GWProcTimeEqpTable { get; set; }
        public DbSet<GWProcCycleTableRow> GWProcCycleTable { get; set; }
        public DbSet<GWProcTimeSysTableRow> GWProcTimeSysTable { get; set; }
        public DbSet<GWProcCycleTListTableRow> GWProcCycleTListTable { get; set; }
        public DbSet<GWProcSpecTableRow> GWProcSpecTable { get; set; }
        public DbSet<GWProcWeekTableRow> GWProcWeekTable { get; set; }
        public DbSet<GWDelayActionTableRow> GWDelayActionTable { get; set; }
        public DbSet<GWRoleTableRow> GWRoleTable { get; set; }
        public DbSet<GWUserTableRow> GWUserTable { get; set; }
        public DbSet<GWZiChanTableRow> GWZiChanTable { get; set; }
        public DbSet<AdministratorTableRow> AdministratorTable { get; set; }
        public DbSet<GWDataRecordItemsTableRow> GWDataRecordItemsTable { get; set; }
        public DbSet<SetEvtTableRow> SetEvtTable { get; set; }
        public DbSet<SpeAlmReportTableRow> SpeAlmReportTable { get; set; }
        public DbSet<WeekAlmReportTableRow> WeekAlmReportTable { get; set; }
        public DbSet<AlmReportTableRow> AlmReportTable { get; set; }
        public DbSet<EquipGroupTableRow> EquipGroupTable { get; set; }
        public DbSet<AlarmRecTableRow> AlarmRecTable { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EquipTableRow>()
                .HasKey(c => new { c.equip_no });
            modelBuilder.Entity<YcpTableRow>()
                .HasKey(c => new { c.equip_no, c.yc_no });
            modelBuilder.Entity<YxpTableRow>()
                .HasKey(c => new { c.equip_no, c.yx_no });
            modelBuilder.Entity<SetParmTableRow>()
                .HasKey(c => new { c.equip_no, c.set_no });
            modelBuilder.Entity<GWProcCycleTableRow>()
                .HasKey(c => new { c.TableID, c.DoOrder });
            modelBuilder.Entity<GWDataRecordItemsTableRow>()
                .HasKey(c => new { c.equip_no, c.data_type, c.ycyx_no });
        }
        public GWDataContext(DbContextOptions<GWDataContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        public override int SaveChanges()
        {
            Int64 startTime = Stopwatch.GetTimestamp();
            int iResult = base.SaveChanges();
            Thread.Sleep(1000);
            double fStoreSecs = (Stopwatch.GetTimestamp() - startTime) / (double)Stopwatch.Frequency;
            if (fStoreSecs > DataCenter.ExecSQLTipsSec)
            {
                string stackInfo = new StackTrace().ToString();
                string msg = $"{System.Environment.NewLine}执行数据库存储时:{System.Environment.NewLine}{stackInfo}耗时过长，执行时间{fStoreSecs}秒，可能需要优化整理数据表!{System.Environment.NewLine}";
                Console.WriteLine(msg);
                MessageService.AddMessage(MessageLevel.Warn, msg, -1, false);
            }
            return iResult;
        }
    }
}
