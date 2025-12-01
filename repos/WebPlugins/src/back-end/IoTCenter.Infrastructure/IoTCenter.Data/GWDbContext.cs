// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Data.Internal;
using IoTCenter.Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Threading.Tasks;

namespace IoTCenter.Data
{
    public class GWDbContext : DbContext
    {
        public GWDbContext(DbContextOptions<GWDbContext> options) : base(options) { }

        protected GWDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Administrator> Administrator { get; set; }
        public DbSet<AlarmProc> AlarmProc { get; set; }
        public DbSet<AlarmRec> AlarmRec { get; set; }
        public DbSet<AlmReport> AlmReport { get; set; }
        public DbSet<AutoProc> AutoProc { get; set; }
        public DbSet<GwdataRecordItems> GwdataRecordItems { get; set; }
        public DbSet<GwdelayAction> GwdelayAction { get; set; }
        public DbSet<GwexProc> GwexProc { get; set; }
        public DbSet<GwexProcCmd> GwexProcCmd { get; set; }
        public DbSet<GwsnapshotConfig> GwsnapshotConfig { get; set; }
        public DbSet<SpeAlmReport> SpeAlmReport { get; set; }

        public DbSet<GwprocCycleProcessTime> GwprocCycleProcessTime { get; set; }
        public DbSet<GwprocCycleTable> GwProcCycleTable { get; set; }
        public DbSet<GwprocCycleTlist> GwProcCycleTlist { get; set; }
        public DbSet<GwprocProcess> GwProcProcess { get; set; }
        public DbSet<GwprocSpecTable> GwProcSpecTable { get; set; }
        public DbSet<GwprocTaskProcessTime> GwProcTaskProcessTime { get; set; }
        public DbSet<GwprocTimeEqpTable> GwProcTimeEqpTable { get; set; }
        public DbSet<GwprocTimeSysTable> GwProcTimeSysTable { get; set; }
        public DbSet<GwprocTimeTlist> GwProcTimeTlist { get; set; }
        public DbSet<GwprocWeekTable> GwProcWeekTable { get; set; }

        public DbSet<Equip> Equip { get; set; }
        public DbSet<EquipGroup> EquipGroup { get; set; }

        public DbSet<Gwrole> Gwrole { get; set; }

        public DbSet<Gwuser> Gwuser { get; set; }
        public DbSet<IotEquip> IotEquip { get; set; }
        public DbSet<IotSetParm> IotSetParm { get; set; }
        public DbSet<IotYcp> IotYcp { get; set; }
        public DbSet<IotYxp> IotYxp { get; set; }
        public DbSet<IoTdevice> IoTDevice { get; set; }
        public DbSet<SetEvt> SetEvt { get; set; }
        public DbSet<SetParm> SetParm { get; set; }
        public DbSet<SysEvt> SysEvt { get; set; }
        public DbSet<YcYxEvt> YcYxEvt { get; set; }
        public DbSet<Ycp> Ycp { get; set; }
        public DbSet<Yxp> Yxp { get; set; }
        public DbSet<IoTAccountPasswordRule> PasswordRules { get; set; }

        public DbSet<GwziChanTable> GwziChanTable { get; set; }
        public DbSet<GwziChanRecord> GwziChanRecord { get; set; }

        #region 设备分组

        public DbSet<EGroup> EGroup { get; set; }
        public DbSet<EGroupList> EGroupList { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                return;
            }

            var entityTypes = modelBuilder.Model.GetEntityTypes();

            foreach (var entityType in entityTypes)
            {
                entityType.SetTableName(entityType.GetTableName().ToLower());

                foreach (var property in entityType.GetProperties())
                {
                    var storeObjectId = StoreObjectIdentifier.Table(entityType.GetTableName().ToLower());
                    property.SetColumnName(property.GetColumnName(storeObjectId).ToLower());
                }
            }

            modelBuilder.Entity<Equip>(entity =>
            {
                entity.HasKey(e => e.EquipNo);

                entity.Property(e => e.EquipNo)
                    .HasColumnName("equip_no")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.AccCyc).HasColumnName("acc_cyc");

                entity.Property(e => e.AlarmScheme).HasColumnName("alarm_scheme");

                entity.Property(e => e.Attrib).HasColumnName("attrib");

                entity.Property(e => e.Backup)
                    .HasColumnName("backup");

                entity.Property(e => e.CommunicationDrv).HasColumnName("communication_drv");

                entity.Property(e => e.CommunicationParam).HasColumnName("communication_param");

                entity.Property(e => e.CommunicationTimeParam).HasColumnName("communication_time_param");

                entity.Property(e => e.Contacted).HasColumnName("contacted");

                entity.Property(e => e.EquipAddr).HasColumnName("equip_addr");

                entity.Property(e => e.EquipDetail).HasColumnName("equip_detail");

                entity.Property(e => e.EquipNm)
                    .IsRequired()
                    .HasColumnName("equip_nm");

                entity.Property(e => e.EventWav).HasColumnName("event_wav");

                entity.Property(e => e.LocalAddr).HasColumnName("local_addr");

                entity.Property(e => e.OutOfContact).HasColumnName("out_of_contact");

                entity.Property(e => e.ProcAdvice).HasColumnName("proc_advice");

                entity.Property(e => e.RawEquipNo).HasColumnName("raw_equip_no");

                entity.Property(e => e.RelatedPic).HasColumnName("related_pic");

                entity.Property(e => e.RelatedVideo).HasColumnName("related_video");

                entity.Property(e => e.StaIp).HasColumnName("sta_ip");

                entity.Property(e => e.StaN).HasColumnName("sta_n");

                entity.Property(e => e.Tabname).HasColumnName("tabname");

                entity.Property(e => e.ZiChanId);

                entity.HasMany(m => m.Ycps).WithOne(o => o.Equip).HasForeignKey(k => k.EquipNo);
                entity.HasMany(m => m.Yxps).WithOne(o => o.Equip).HasForeignKey(k => k.EquipNo);
                entity.HasMany(m => m.SetParms).WithOne(o => o.Equip).HasForeignKey(k => k.EquipNo);
            });

            modelBuilder.Entity<EquipGroup>(entity =>
            {
                entity.HasKey(e => e.GroupNo);

                entity.Property(e => e.GroupNo)
                    .HasColumnName("group_no")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Equipcomb).HasColumnName("equipcomb");

                entity.Property(e => e.GroupName)
                    .HasColumnName("group_name")
                    .IsRequired();

                entity.Property(e => e.StaN)
                    .HasColumnName("sta_n");

            });

            modelBuilder.Entity<Gwrole>(entity =>
            {
                entity.HasKey(e => e.Name);

                entity.Property(e => e.ControlEquipsUnit).HasColumnName("controlequips_unit");

                entity.Property(e => e.Remark).HasColumnName("remark");
            });

            modelBuilder.Entity<GwsnapshotConfig>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Gwuser>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<IotEquip>(entity =>
            {
                entity.HasKey(e => e.EquipNo);

                entity.Property(e => e.StaN).HasColumnName("sta_n");

                entity.Property(e => e.EquipNo).HasColumnName("equip_no").ValueGeneratedOnAdd();

                entity.Property(e => e.AccCyc).HasColumnName("acc_cyc");

                entity.Property(e => e.AlarmScheme).HasColumnName("alarm_scheme");

                entity.Property(e => e.Attrib).HasColumnName("attrib");

                entity.Property(e => e.Backup).HasColumnName("backup");

                entity.Property(e => e.CommunicationDrv).HasColumnName("communication_drv");

                entity.Property(e => e.CommunicationParam).HasColumnName("communication_param");

                entity.Property(e => e.CommunicationTimeParam).HasColumnName("communication_time_param");

                entity.Property(e => e.Contacted).HasColumnName("contacted");

                entity.Property(e => e.EquipAddr).HasColumnName("equip_addr");

                entity.Property(e => e.EquipDetail).HasColumnName("equip_detail");

                entity.Property(e => e.EquipNm)
                    .IsRequired()
                    .HasColumnName("equip_nm");

                entity.Property(e => e.EventWav).HasColumnName("event_wav");

                entity.Property(e => e.LocalAddr).HasColumnName("local_addr");

                entity.Property(e => e.OutOfContact).HasColumnName("out_of_contact");

                entity.Property(e => e.ProcAdvice).HasColumnName("proc_advice");

                entity.Property(e => e.RawEquipNo).HasColumnName("raw_equip_no");

                entity.Property(e => e.RelatedPic).HasColumnName("related_pic");

                entity.Property(e => e.RelatedVideo).HasColumnName("related_video");

                entity.Property(e => e.StaIp).HasColumnName("sta_ip");

                entity.Property(e => e.Tabname).HasColumnName("tabname");

                entity.Property(e => e.ZiChanId);

                entity.HasMany(m => m.IotYcps).WithOne(o => o.IotEquip).HasForeignKey(k => k.EquipNo);
                entity.HasMany(m => m.IotYxps).WithOne(o => o.IotEquip).HasForeignKey(k => k.EquipNo);
                entity.HasMany(m => m.IotSetParms).WithOne(o => o.IotEquip).HasForeignKey(k => k.EquipNo);
            });

            modelBuilder.Entity<IotSetParm>(entity =>
            {
                entity.HasKey(e => new { e.EquipNo, e.SetNo });

                entity.Property(e => e.EquipNo).HasColumnName("equip_no");

                entity.Property(e => e.SetNo).HasColumnName("set_no");

                entity.Property(e => e.Action).HasColumnName("action");

                entity.Property(e => e.Canexecution).HasColumnName("canexecution");

                entity.Property(e => e.MainInstruction).HasColumnName("main_instruction");

                entity.Property(e => e.MinorInstruction).HasColumnName("minor_instruction");

                entity.Property(e => e.QrEquipNo).HasColumnName("qr_equip_no");

                entity.Property(e => e.Record).HasColumnName("record");

                entity.Property(e => e.SetNm).HasColumnName("set_nm");

                entity.Property(e => e.SetType)
                    .IsRequired()
                    .HasColumnName("set_type");

                entity.Property(e => e.StaN).HasColumnName("sta_n");

                entity.Property(e => e.Value).HasColumnName("value");
            });

            modelBuilder.Entity<IotYcp>(entity =>
            {
                entity.HasKey(e => new { e.EquipNo, e.YcNo });

                entity.Property(e => e.EquipNo).HasColumnName("equip_no");

                entity.Property(e => e.YcNo).HasColumnName("yc_no");

                entity.Property(e => e.AlarmAcceptableTime).HasColumnName("alarm_acceptable_time");

                entity.Property(e => e.AlarmRepeatTime).HasColumnName("alarm_repeat_time");

                entity.Property(e => e.AlarmScheme).HasColumnName("alarm_scheme");

                entity.Property(e => e.AlarmShield).HasColumnName("alarm_shield");

                entity.Property(e => e.CurveLimit).HasColumnName("curve_limit");

                entity.Property(e => e.CurveRcd).HasColumnName("curve_rcd");

                entity.Property(e => e.LvlLevel).HasColumnName("lvl_level");

                entity.Property(e => e.MainInstruction).HasColumnName("main_instruction");

                entity.Property(e => e.Mapping).HasColumnName("mapping");

                entity.Property(e => e.MinorInstruction).HasColumnName("minor_instruction");

                entity.Property(e => e.OutmaxEvt).HasColumnName("outmax_evt");

                entity.Property(e => e.OutminEvt).HasColumnName("outmin_evt");

                entity.Property(e => e.PhysicMax).HasColumnName("physic_max");

                entity.Property(e => e.PhysicMin).HasColumnName("physic_min");

                entity.Property(e => e.ProcAdvice).HasColumnName("proc_advice");

                entity.Property(e => e.RelatedPic).HasColumnName("related_pic");

                entity.Property(e => e.RelatedVideo).HasColumnName("related_video");

                entity.Property(e => e.RestoreAcceptableTime).HasColumnName("restore_acceptable_time");

                entity.Property(e => e.RestoreMax).HasColumnName("restore_max");

                entity.Property(e => e.RestoreMin).HasColumnName("restore_min");

                entity.Property(e => e.SafeBgn)
                    .HasColumnName("safe_bgn");

                entity.Property(e => e.SafeEnd)
                    .HasColumnName("safe_end");

                entity.Property(e => e.StaN).HasColumnName("sta_n");

                entity.Property(e => e.Unit).HasColumnName("unit");

                entity.Property(e => e.ValMax).HasColumnName("val_max");

                entity.Property(e => e.ValMin).HasColumnName("val_min");

                entity.Property(e => e.ValTrait).HasColumnName("val_trait");

                entity.Property(e => e.WaveFile).HasColumnName("wave_file");

                entity.Property(e => e.YcMax).HasColumnName("yc_max");

                entity.Property(e => e.YcMin).HasColumnName("yc_min");

                entity.Property(e => e.YcNm)
                    .IsRequired()
                    .HasColumnName("yc_nm");

                entity.Property(e => e.ZiChanId);
            });

            modelBuilder.Entity<IotYxp>(entity =>
            {
                entity.HasKey(e => new { e.EquipNo, e.YxNo });

                entity.Property(e => e.EquipNo).HasColumnName("equip_no");

                entity.Property(e => e.YxNo).HasColumnName("yx_no");

                entity.Property(e => e.AlarmAcceptableTime).HasColumnName("alarm_acceptable_time");

                entity.Property(e => e.AlarmRepeatTime).HasColumnName("alarm_repeat_time");

                entity.Property(e => e.AlarmScheme).HasColumnName("alarm_scheme");

                entity.Property(e => e.AlarmShield).HasColumnName("alarm_shield");

                entity.Property(e => e.CurveRcd).HasColumnName("curve_rcd");

                entity.Property(e => e.Evt01).HasColumnName("evt_01");

                entity.Property(e => e.Evt10).HasColumnName("evt_10");

                entity.Property(e => e.Initval).HasColumnName("initval");

                entity.Property(e => e.Inversion).HasColumnName("inversion");

                entity.Property(e => e.LevelD).HasColumnName("level_d");

                entity.Property(e => e.LevelR).HasColumnName("level_r");

                entity.Property(e => e.MainInstruction).HasColumnName("main_instruction");

                entity.Property(e => e.MinorInstruction).HasColumnName("minor_instruction");

                entity.Property(e => e.ProcAdviceD).HasColumnName("proc_advice_d");

                entity.Property(e => e.ProcAdviceR).HasColumnName("proc_advice_r");

                entity.Property(e => e.RelatedPic).HasColumnName("related_pic");

                entity.Property(e => e.RelatedVideo).HasColumnName("related_video");

                entity.Property(e => e.RestoreAcceptableTime).HasColumnName("restore_acceptable_time");

                entity.Property(e => e.SafeBgn)
                    .HasColumnName("safe_bgn");

                entity.Property(e => e.SafeEnd)
                    .HasColumnName("safe_end");

                entity.Property(e => e.StaN).HasColumnName("sta_n");

                entity.Property(e => e.ValTrait).HasColumnName("val_trait");

                entity.Property(e => e.WaveFile).HasColumnName("wave_file");

                entity.Property(e => e.YxNm)
                    .IsRequired()
                    .HasColumnName("yx_nm");

                entity.Property(e => e.ZiChanId);
            });

            modelBuilder.Entity<SetEvt>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.EquipNo).HasColumnName("equip_no");

                entity.Property(e => e.Gwevent).HasColumnName("gwevent");

                entity.Property(e => e.Gwoperator).HasColumnName("gwoperator");

                entity.Property(e => e.Gwsource).HasColumnName("gwsource");

                entity.Property(e => e.Gwtime)
                    .IsRequired()
                    .HasColumnName("gwtime");

                entity.Property(e => e.SetNo).HasColumnName("set_no");

                entity.Property(e => e.StaN).HasColumnName("sta_n");
            });

            modelBuilder.Entity<SetParm>(entity =>
            {
                entity.HasKey(e => new { e.EquipNo, e.SetNo });

                entity.Property(e => e.EquipNo).HasColumnName("equip_no");

                entity.Property(e => e.SetNo).HasColumnName("set_no");

                entity.Property(e => e.Action)
                    .HasColumnName("action");

                entity.Property(e => e.Canexecution)
                    .HasColumnName("canexecution");

                entity.Property(e => e.EnableVoice);

                entity.Property(e => e.MainInstruction)
                    .HasColumnName("main_instruction");

                entity.Property(e => e.MinorInstruction)
                    .HasColumnName("minor_instruction");

                entity.Property(e => e.QrEquipNo).HasColumnName("qr_equip_no");

                entity.Property(e => e.Record)
                    .HasColumnName("record");

                entity.Property(e => e.Reserve1);

                entity.Property(e => e.Reserve2);

                entity.Property(e => e.Reserve3);

                entity.Property(e => e.SetNm)
                    .HasColumnName("set_nm");

                entity.Property(e => e.SetType)
                    .HasColumnName("set_type");

                entity.Property(e => e.StaN).HasColumnName("sta_n");

                entity.Property(e => e.Value)
                    .HasColumnName("value");

                entity.Property(e => e.VoiceKeys);
            });

            modelBuilder.Entity<SysEvt>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Confirmname)
                    .HasColumnName("confirmname");

                entity.Property(e => e.Confirmremark)
                    .HasColumnName("confirmremark");

                entity.Property(e => e.Confirmtime)
                    .IsRequired()
                    .HasColumnName("confirmtime");

                entity.Property(e => e.Event)
                    .IsRequired()
                    .HasColumnName("event");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid");

                entity.Property(e => e.StaN).HasColumnName("sta_n");

                entity.Property(e => e.Time)
                    .IsRequired()
                    .HasColumnName("time");
            });

            modelBuilder.Entity<YcYxEvt>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Alarmlevel).HasColumnName("alarmlevel");

                entity.Property(e => e.Confirmname).HasColumnName("confirmname");

                entity.Property(e => e.Confirmremark).HasColumnName("confirmremark");

                entity.Property(e => e.Confirmtime)
                    .IsRequired()
                    .HasColumnName("confirmtime");

                entity.Property(e => e.EquipNo).HasColumnName("equip_no");

                entity.Property(e => e.Event)
                    .IsRequired()
                    .HasColumnName("event");

                entity.Property(e => e.Guid).HasColumnName("guid");

                entity.Property(e => e.ProcRec).HasColumnName("proc_rec");

                entity.Property(e => e.Snapshotlevel).HasColumnName("snapshotlevel");

                entity.Property(e => e.StaN).HasColumnName("sta_n");

                entity.Property(e => e.Time)
                    .IsRequired()
                    .HasColumnName("time");

                entity.Property(e => e.YcyxNo).HasColumnName("ycyx_no");

                entity.Property(e => e.YcyxType)
                    .IsRequired()
                    .HasColumnName("ycyx_type");
            });

            modelBuilder.Entity<Ycp>(entity =>
            {
                entity.HasKey(e => new { e.EquipNo, e.YcNo });

                entity.Property(e => e.EquipNo).HasColumnName("equip_no");

                entity.Property(e => e.YcNo).HasColumnName("yc_no");

                entity.Property(e => e.AlarmAcceptableTime).HasColumnName("alarm_acceptable_time");

                entity.Property(e => e.AlarmRepeatTime).HasColumnName("alarm_repeat_time");

                entity.Property(e => e.AlarmScheme).HasColumnName("alarm_scheme");

                entity.Property(e => e.AlarmShield).HasColumnName("alarm_shield");

                entity.Property(e => e.CurveLimit).HasColumnName("curve_limit");

                entity.Property(e => e.CurveRcd).HasColumnName("curve_rcd");

                entity.Property(e => e.LvlLevel).HasColumnName("lvl_level");

                entity.Property(e => e.MainInstruction).HasColumnName("main_instruction");

                entity.Property(e => e.Mapping).HasColumnName("mapping");

                entity.Property(e => e.MinorInstruction).HasColumnName("minor_instruction");

                entity.Property(e => e.OutmaxEvt).HasColumnName("outmax_evt");

                entity.Property(e => e.OutminEvt).HasColumnName("outmin_evt");

                entity.Property(e => e.PhysicMax)
                    .HasColumnName("physic_max");

                entity.Property(e => e.PhysicMin)
                    .HasColumnName("physic_min");

                entity.Property(e => e.ProcAdvice).HasColumnName("proc_advice");

                entity.Property(e => e.RelatedPic).HasColumnName("related_pic");

                entity.Property(e => e.RelatedVideo).HasColumnName("related_video");

                entity.Property(e => e.RestoreAcceptableTime).HasColumnName("restore_acceptable_time");

                entity.Property(e => e.RestoreMax)
                    .HasColumnName("restore_max");

                entity.Property(e => e.RestoreMin)
                    .HasColumnName("restore_min");

                entity.Property(e => e.StaN).HasColumnName("sta_n");

                entity.Property(e => e.Unit).HasColumnName("unit");

                entity.Property(e => e.ValMax)
                    .HasColumnName("val_max");

                entity.Property(e => e.ValMin)
                    .HasColumnName("val_min");

                entity.Property(e => e.ValTrait).HasColumnName("val_trait");

                entity.Property(e => e.WaveFile).HasColumnName("wave_file");

                entity.Property(e => e.YcMax)
                    .HasColumnName("yc_max");

                entity.Property(e => e.YcMin)
                    .HasColumnName("yc_min");

                entity.Property(e => e.YcNm)
                    .IsRequired()
                    .HasColumnName("yc_nm");

                entity.Property(e => e.ZiChanId);

                entity.Property(e => e.SafeBgn)
                .HasColumnName("safe_bgn");

                entity.Property(e => e.SafeEnd)
                    .HasColumnName("safe_end");
            });

            modelBuilder.Entity<Yxp>(entity =>
            {
                entity.HasKey(e => new { e.EquipNo, e.YxNo });

                entity.Property(e => e.EquipNo).HasColumnName("equip_no");

                entity.Property(e => e.YxNo).HasColumnName("yx_no");

                entity.Property(e => e.AlarmAcceptableTime).HasColumnName("alarm_acceptable_time");

                entity.Property(e => e.AlarmRepeatTime).HasColumnName("alarm_repeat_time");

                entity.Property(e => e.AlarmScheme).HasColumnName("alarm_scheme");

                entity.Property(e => e.AlarmShield).HasColumnName("alarm_shield");

                entity.Property(e => e.CurveRcd).HasColumnName("curve_rcd");

                entity.Property(e => e.Evt01).HasColumnName("evt_01");

                entity.Property(e => e.Evt10).HasColumnName("evt_10");

                entity.Property(e => e.Initval).HasColumnName("initval");

                entity.Property(e => e.Inversion).HasColumnName("inversion");

                entity.Property(e => e.LevelD).HasColumnName("level_d");

                entity.Property(e => e.LevelR).HasColumnName("level_r");

                entity.Property(e => e.MainInstruction).HasColumnName("main_instruction");

                entity.Property(e => e.MinorInstruction).HasColumnName("minor_instruction");

                entity.Property(e => e.ProcAdviceD).HasColumnName("proc_advice_d");

                entity.Property(e => e.ProcAdviceR).HasColumnName("proc_advice_r");

                entity.Property(e => e.RelatedPic).HasColumnName("related_pic");

                entity.Property(e => e.RelatedVideo).HasColumnName("related_video");

                entity.Property(e => e.RestoreAcceptableTime).HasColumnName("restore_acceptable_time");

                entity.Property(e => e.StaN).HasColumnName("sta_n");

                entity.Property(e => e.ValTrait).HasColumnName("val_trait");

                entity.Property(e => e.WaveFile).HasColumnName("wave_file");

                entity.Property(e => e.YxNm)
                    .IsRequired()
                    .HasColumnName("yx_nm");

                entity.Property(e => e.ZiChanId);

                entity.Property(e => e.SafeBgn)
                .HasColumnName("safe_bgn");

                entity.Property(e => e.SafeEnd)
                    .HasColumnName("safe_end");
            });

            #region 设备分组

            modelBuilder.Entity<EGroup>(entity =>
            {
                entity.HasKey(e => e.GroupId);
                entity.Property(e => e.GroupId).ValueGeneratedOnAdd();
                entity.Property(e => e.GroupName).IsRequired();
                entity.Property(e => e.ParentGroupId).IsRequired();
                entity.Ignore(p => p.Lists);
            });

            modelBuilder.Entity<EGroupList>(entity =>
            {
                entity.HasKey(e => e.EGroupListId);
                entity.Property(e => e.EGroupListId).ValueGeneratedOnAdd();
                entity.Ignore(x => x.EquipName);
            });

            modelBuilder.Entity<GwdataRecordItems>(entity =>
            {
                entity.HasKey(e => new { e.EquipNo, e.DataType, e.YcyxNo });
            });

            modelBuilder.Entity<GwexProcCmd>(entity =>
            {
                entity.HasKey(e => new { e.ProcCode, e.CmdNm });
            });

            modelBuilder.Entity<GwprocCycleTable>(entity =>
            {
                entity.HasKey(e => new { e.TableId, e.DoOrder });
            });

            modelBuilder.RemoveForeignKeys();

            #endregion
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }
    }
}
