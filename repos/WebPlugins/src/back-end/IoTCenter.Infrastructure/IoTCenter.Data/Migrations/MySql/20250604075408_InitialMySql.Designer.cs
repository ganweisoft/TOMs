// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using IoTCenter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace IoTCenter.Data.Migrations.MySql
{
    [DbContext(typeof(GWDbContext))]
    [Migration("20250604075408_InitialMySql")]
    partial class InitialMySql
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("IoTCenter.Data.Model.Administrator", b =>
                {
                    b.Property<string>("AdministratorName")
                        .HasColumnType("varchar(255)")
                        .HasColumnName("administrator");

                    b.Property<int>("AckLevel")
                        .HasColumnType("int")
                        .HasColumnName("acklevel");

                    b.Property<string>("Email")
                        .HasColumnType("longtext")
                        .HasColumnName("email");

                    b.Property<string>("MobileTel")
                        .HasColumnType("longtext")
                        .HasColumnName("mobiletel");

                    b.Property<string>("PhotoImage")
                        .HasColumnType("longtext")
                        .HasColumnName("photoimage");

                    b.Property<string>("Reserve1")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve3");

                    b.Property<string>("Telphone")
                        .HasColumnType("longtext")
                        .HasColumnName("telphone");

                    b.Property<string>("UserName")
                        .HasColumnType("longtext")
                        .HasColumnName("username");

                    b.HasKey("AdministratorName");

                    b.ToTable("administrator");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.AlarmProc", b =>
                {
                    b.Property<int>("ProcCode")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("proc_code");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("ProcCode"));

                    b.Property<string>("Comment")
                        .HasColumnType("longtext")
                        .HasColumnName("comment");

                    b.Property<string>("ProcModule")
                        .HasColumnType("longtext")
                        .HasColumnName("proc_module");

                    b.Property<string>("ProcName")
                        .HasColumnType("longtext")
                        .HasColumnName("proc_name");

                    b.Property<string>("ProcParm")
                        .HasColumnType("longtext")
                        .HasColumnName("proc_parm");

                    b.Property<string>("Reserve1")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve3");

                    b.HasKey("ProcCode");

                    b.ToTable("alarmproc");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.AlarmRec", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Administrator")
                        .HasColumnType("longtext")
                        .HasColumnName("administrator");

                    b.Property<string>("Comment")
                        .HasColumnType("longtext")
                        .HasColumnName("comment");

                    b.Property<string>("Event")
                        .HasColumnType("longtext")
                        .HasColumnName("event");

                    b.Property<string>("ProcName")
                        .HasColumnType("longtext")
                        .HasColumnName("proc_name");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("time");

                    b.HasKey("Id");

                    b.ToTable("alarmrec");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.AlmReport", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Administrator")
                        .HasColumnType("longtext")
                        .HasColumnName("administrator");

                    b.Property<int>("GroupNo")
                        .HasColumnType("int")
                        .HasColumnName("group_no");

                    b.Property<int?>("StaN")
                        .HasColumnType("int")
                        .HasColumnName("sta_n");

                    b.HasKey("Id");

                    b.ToTable("almreport");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.AutoProc", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Delay")
                        .HasColumnType("int")
                        .HasColumnName("delay");

                    b.Property<bool>("Enable")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("enable");

                    b.Property<int>("IequipNo")
                        .HasColumnType("int")
                        .HasColumnName("iequip_no");

                    b.Property<int>("IycyxNo")
                        .HasColumnType("int")
                        .HasColumnName("iycyx_no");

                    b.Property<string>("IycyxType")
                        .HasColumnType("longtext")
                        .HasColumnName("iycyx_type");

                    b.Property<int>("OequipNo")
                        .HasColumnType("int")
                        .HasColumnName("oequip_no");

                    b.Property<int>("OsetNo")
                        .HasColumnType("int")
                        .HasColumnName("oset_no");

                    b.Property<string>("ProcDesc")
                        .HasColumnType("longtext")
                        .HasColumnName("procdesc");

                    b.Property<string>("Reserve")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve");

                    b.Property<string>("Value")
                        .HasColumnType("longtext")
                        .HasColumnName("value");

                    b.HasKey("Id");

                    b.ToTable("autoproc");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.EGroup", b =>
                {
                    b.Property<int>("GroupId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("groupid");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("GroupId"));

                    b.Property<string>("GroupName")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("groupname");

                    b.Property<int>("ParentGroupId")
                        .HasColumnType("int")
                        .HasColumnName("parentgroupid");

                    b.HasKey("GroupId");

                    b.ToTable("egroup");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.EGroupList", b =>
                {
                    b.Property<int>("EGroupListId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("egrouplistid");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("EGroupListId"));

                    b.Property<int>("EquipNo")
                        .HasColumnType("int")
                        .HasColumnName("equipno");

                    b.Property<int>("GroupId")
                        .HasColumnType("int")
                        .HasColumnName("groupid");

                    b.Property<int>("StaNo")
                        .HasColumnType("int")
                        .HasColumnName("stano");

                    b.HasKey("EGroupListId");

                    b.ToTable("egrouplist");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.Equip", b =>
                {
                    b.Property<int>("EquipNo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("equip_no");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("EquipNo"));

                    b.Property<int>("AccCyc")
                        .HasColumnType("int")
                        .HasColumnName("acc_cyc");

                    b.Property<int?>("AlarmRiseCycle")
                        .HasColumnType("int")
                        .HasColumnName("alarmrisecycle");

                    b.Property<int>("AlarmScheme")
                        .HasColumnType("int")
                        .HasColumnName("alarm_scheme");

                    b.Property<int>("Attrib")
                        .HasColumnType("int")
                        .HasColumnName("attrib");

                    b.Property<string>("Backup")
                        .HasColumnType("longtext")
                        .HasColumnName("backup");

                    b.Property<string>("CommunicationDrv")
                        .HasColumnType("longtext")
                        .HasColumnName("communication_drv");

                    b.Property<string>("CommunicationParam")
                        .HasColumnType("longtext")
                        .HasColumnName("communication_param");

                    b.Property<string>("CommunicationTimeParam")
                        .HasColumnType("longtext")
                        .HasColumnName("communication_time_param");

                    b.Property<string>("Contacted")
                        .HasColumnType("longtext")
                        .HasColumnName("contacted");

                    b.Property<string>("EquipAddr")
                        .HasColumnType("longtext")
                        .HasColumnName("equip_addr");

                    b.Property<string>("EquipDetail")
                        .HasColumnType("longtext")
                        .HasColumnName("equip_detail");

                    b.Property<string>("EquipNm")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("equip_nm");

                    b.Property<string>("EventWav")
                        .HasColumnType("longtext")
                        .HasColumnName("event_wav");

                    b.Property<bool?>("IsDisable")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("isdisable");

                    b.Property<string>("LocalAddr")
                        .HasColumnType("longtext")
                        .HasColumnName("local_addr");

                    b.Property<string>("OutOfContact")
                        .HasColumnType("longtext")
                        .HasColumnName("out_of_contact");

                    b.Property<string>("PlanNo")
                        .HasColumnType("longtext")
                        .HasColumnName("planno");

                    b.Property<string>("ProcAdvice")
                        .HasColumnType("longtext")
                        .HasColumnName("proc_advice");

                    b.Property<int>("RawEquipNo")
                        .HasColumnType("int")
                        .HasColumnName("raw_equip_no");

                    b.Property<string>("RelatedPic")
                        .HasColumnType("longtext")
                        .HasColumnName("related_pic");

                    b.Property<string>("RelatedVideo")
                        .HasColumnType("longtext")
                        .HasColumnName("related_video");

                    b.Property<string>("Reserve1")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve3");

                    b.Property<string>("SafeTime")
                        .HasColumnType("longtext")
                        .HasColumnName("safetime");

                    b.Property<string>("StaIp")
                        .HasColumnType("longtext")
                        .HasColumnName("sta_ip");

                    b.Property<int>("StaN")
                        .HasColumnType("int")
                        .HasColumnName("sta_n");

                    b.Property<string>("Tabname")
                        .HasColumnType("longtext")
                        .HasColumnName("tabname");

                    b.Property<string>("ZiChanId")
                        .HasColumnType("longtext")
                        .HasColumnName("zichanid");

                    b.HasKey("EquipNo");

                    b.ToTable("equip");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.EquipGroup", b =>
                {
                    b.Property<int>("GroupNo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("group_no");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("GroupNo"));

                    b.Property<string>("Equipcomb")
                        .HasColumnType("longtext")
                        .HasColumnName("equipcomb");

                    b.Property<string>("GroupName")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("group_name");

                    b.Property<int>("StaN")
                        .HasColumnType("int")
                        .HasColumnName("sta_n");

                    b.HasKey("GroupNo");

                    b.ToTable("equipgroup");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwdataRecordItems", b =>
                {
                    b.Property<int>("EquipNo")
                        .HasColumnType("int")
                        .HasColumnName("equip_no");

                    b.Property<string>("DataType")
                        .HasColumnType("varchar(255)")
                        .HasColumnName("data_type");

                    b.Property<int>("YcyxNo")
                        .HasColumnType("int")
                        .HasColumnName("ycyx_no");

                    b.Property<string>("DataName")
                        .HasColumnType("longtext")
                        .HasColumnName("data_name");

                    b.Property<string>("RuleName")
                        .HasColumnType("longtext")
                        .HasColumnName("rulename");

                    b.HasKey("EquipNo", "DataType", "YcyxNo");

                    b.ToTable("gwdatarecorditems");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwdelayAction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("GwAddDateTime")
                        .HasColumnType("longtext")
                        .HasColumnName("gw_adddatetime");

                    b.Property<int>("GwDelayNum")
                        .HasColumnType("int")
                        .HasColumnName("gw_delaynum");

                    b.Property<int>("GwEquipNo")
                        .HasColumnType("int")
                        .HasColumnName("gw_equip_no");

                    b.Property<int>("GwSetNo")
                        .HasColumnType("int")
                        .HasColumnName("gw_set_no");

                    b.Property<int>("GwSource")
                        .HasColumnType("int")
                        .HasColumnName("gw_source");

                    b.Property<int>("GwStaN")
                        .HasColumnType("int")
                        .HasColumnName("gw_sta_n");

                    b.Property<int>("GwState")
                        .HasColumnType("int")
                        .HasColumnName("gw_state");

                    b.Property<string>("GwUserNm")
                        .HasColumnType("longtext")
                        .HasColumnName("gw_usernm");

                    b.Property<string>("GwValue")
                        .HasColumnType("longtext")
                        .HasColumnName("gw_value");

                    b.Property<string>("Reserve1")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve3");

                    b.HasKey("Id");

                    b.ToTable("gwdelayaction");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwexProc", b =>
                {
                    b.Property<int>("ProcCode")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("proc_code");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("ProcCode"));

                    b.Property<string>("Comment")
                        .HasColumnType("longtext")
                        .HasColumnName("comment");

                    b.Property<string>("ProcModule")
                        .HasColumnType("longtext")
                        .HasColumnName("proc_module");

                    b.Property<string>("ProcName")
                        .HasColumnType("longtext")
                        .HasColumnName("proc_name");

                    b.Property<string>("ProcParm")
                        .HasColumnType("longtext")
                        .HasColumnName("proc_parm");

                    b.Property<string>("Reserve1")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve3");

                    b.HasKey("ProcCode");

                    b.ToTable("gwexproc");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwexProcCmd", b =>
                {
                    b.Property<int>("ProcCode")
                        .HasColumnType("int")
                        .HasColumnName("proc_code");

                    b.Property<string>("CmdNm")
                        .HasColumnType("varchar(255)")
                        .HasColumnName("cmd_nm");

                    b.Property<string>("MainInstruction")
                        .HasColumnType("longtext")
                        .HasColumnName("main_instruction");

                    b.Property<string>("MinorInstruction")
                        .HasColumnType("longtext")
                        .HasColumnName("minor_instruction");

                    b.Property<bool>("Record")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("record");

                    b.Property<string>("Value")
                        .HasColumnType("longtext")
                        .HasColumnName("value");

                    b.HasKey("ProcCode", "CmdNm");

                    b.ToTable("gwexproccmd");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwprocCycleProcessTime", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ProcessOrder")
                        .HasColumnType("longtext")
                        .HasColumnName("processorder");

                    b.Property<int?>("TableId")
                        .HasColumnType("int")
                        .HasColumnName("tableid");

                    b.Property<int?>("Time")
                        .HasColumnType("int")
                        .HasColumnName("time");

                    b.HasKey("Id");

                    b.ToTable("gwproccycleprocesstime");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwprocCycleTable", b =>
                {
                    b.Property<int>("TableId")
                        .HasColumnType("int")
                        .HasColumnName("tableid");

                    b.Property<int>("DoOrder")
                        .HasColumnType("int")
                        .HasColumnName("doorder");

                    b.Property<string>("CmdNm")
                        .HasColumnType("longtext")
                        .HasColumnName("cmd_nm");

                    b.Property<int>("EquipNo")
                        .HasColumnType("int")
                        .HasColumnName("equip_no");

                    b.Property<string>("EquipSetNm")
                        .HasColumnType("longtext")
                        .HasColumnName("equipsetnm");

                    b.Property<int>("ProcCode")
                        .HasColumnType("int")
                        .HasColumnName("proc_code");

                    b.Property<string>("ProcessOrder")
                        .HasColumnType("longtext")
                        .HasColumnName("processorder");

                    b.Property<string>("Reserve1")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve3");

                    b.Property<int>("SetNo")
                        .HasColumnType("int")
                        .HasColumnName("set_no");

                    b.Property<int>("SleepTime")
                        .HasColumnType("int")
                        .HasColumnName("sleeptime");

                    b.Property<string>("SleepUnit")
                        .HasColumnType("longtext")
                        .HasColumnName("sleepunit");

                    b.Property<string>("Type")
                        .HasColumnType("longtext")
                        .HasColumnName("type");

                    b.Property<string>("Value")
                        .HasColumnType("longtext")
                        .HasColumnName("value");

                    b.HasKey("TableId", "DoOrder");

                    b.ToTable("gwproccycletable");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwprocCycleTlist", b =>
                {
                    b.Property<int>("TableId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("tableid");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("TableId"));

                    b.Property<DateTime>("BeginTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("begintime");

                    b.Property<bool>("CycleMustFinish")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("cyclemustfinish");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("endtime");

                    b.Property<int>("MaxCycleNum")
                        .HasColumnType("int")
                        .HasColumnName("maxcyclenum");

                    b.Property<string>("Reserve1")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve3");

                    b.Property<string>("TableName")
                        .HasColumnType("longtext")
                        .HasColumnName("tablename");

                    b.Property<bool>("ZhenDianDo")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("zhendiando");

                    b.Property<bool>("ZhidingDo")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("zhidingdo");

                    b.Property<DateTime>("ZhidingTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("zhidingtime");

                    b.HasKey("TableId");

                    b.ToTable("gwproccycletlist");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwprocProcess", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("From")
                        .HasColumnType("longtext")
                        .HasColumnName("from");

                    b.Property<int?>("TableId")
                        .HasColumnType("int")
                        .HasColumnName("tableid");

                    b.Property<string>("TaskType")
                        .HasColumnType("longtext")
                        .HasColumnName("tasktype");

                    b.Property<string>("To")
                        .HasColumnType("longtext")
                        .HasColumnName("to");

                    b.HasKey("Id");

                    b.ToTable("gwprocprocess");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwprocSpecTable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("BeginDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("begindate");

                    b.Property<string>("Color")
                        .HasColumnType("longtext")
                        .HasColumnName("color");

                    b.Property<string>("DateName")
                        .HasColumnType("longtext")
                        .HasColumnName("datename");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("enddate");

                    b.Property<string>("TableId")
                        .HasColumnType("longtext")
                        .HasColumnName("tableid");

                    b.HasKey("Id");

                    b.ToTable("gwprocspectable");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwprocTaskProcessTime", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ControlType")
                        .HasColumnType("longtext")
                        .HasColumnName("controltype");

                    b.Property<string>("ProcessOrder")
                        .HasColumnType("longtext")
                        .HasColumnName("processorder");

                    b.Property<int?>("TableId")
                        .HasColumnType("int")
                        .HasColumnName("tableid");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("time");

                    b.Property<string>("TimeType")
                        .HasColumnType("longtext")
                        .HasColumnName("timetype");

                    b.HasKey("Id");

                    b.ToTable("gwproctaskprocesstime");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwprocTimeEqpTable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("EquipNo")
                        .HasColumnType("int")
                        .HasColumnName("equip_no");

                    b.Property<string>("EquipSetNm")
                        .HasColumnType("longtext")
                        .HasColumnName("equipsetnm");

                    b.Property<string>("ProcessOrder")
                        .HasColumnType("longtext")
                        .HasColumnName("processorder");

                    b.Property<string>("Reserve1")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve3");

                    b.Property<int>("SetNo")
                        .HasColumnType("int")
                        .HasColumnName("set_no");

                    b.Property<int>("TableId")
                        .HasColumnType("int")
                        .HasColumnName("tableid");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("time");

                    b.Property<DateTime>("TimeDur")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("timedur");

                    b.Property<string>("Value")
                        .HasColumnType("longtext")
                        .HasColumnName("value");

                    b.HasKey("Id");

                    b.ToTable("gwproctimeeqptable");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwprocTimeSysTable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CmdNm")
                        .HasColumnType("longtext")
                        .HasColumnName("cmd_nm");

                    b.Property<int>("ProcCode")
                        .HasColumnType("int")
                        .HasColumnName("proc_code");

                    b.Property<string>("ProcessOrder")
                        .HasColumnType("longtext")
                        .HasColumnName("processorder");

                    b.Property<string>("Reserve1")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve3");

                    b.Property<int>("TableId")
                        .HasColumnType("int")
                        .HasColumnName("tableid");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("time");

                    b.Property<DateTime>("TimeDur")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("timedur");

                    b.HasKey("Id");

                    b.ToTable("gwproctimesystable");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwprocTimeTlist", b =>
                {
                    b.Property<int>("TableId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("tableid");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("TableId"));

                    b.Property<string>("Comment")
                        .HasColumnType("longtext")
                        .HasColumnName("comment");

                    b.Property<string>("TableName")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("tablename");

                    b.HasKey("TableId");

                    b.ToTable("gwproctimetlist");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwprocWeekTable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Fri")
                        .HasColumnType("longtext")
                        .HasColumnName("fri");

                    b.Property<string>("Mon")
                        .HasColumnType("longtext")
                        .HasColumnName("mon");

                    b.Property<string>("Sat")
                        .HasColumnType("longtext")
                        .HasColumnName("sat");

                    b.Property<string>("Sun")
                        .HasColumnType("longtext")
                        .HasColumnName("sun");

                    b.Property<string>("Thurs")
                        .HasColumnType("longtext")
                        .HasColumnName("thurs");

                    b.Property<string>("Tues")
                        .HasColumnType("longtext")
                        .HasColumnName("tues");

                    b.Property<string>("Wed")
                        .HasColumnType("longtext")
                        .HasColumnName("wed");

                    b.HasKey("Id");

                    b.ToTable("gwprocweektable");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.Gwrole", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("varchar(255)")
                        .HasColumnName("name");

                    b.Property<string>("BrowseEquips")
                        .HasColumnType("longtext")
                        .HasColumnName("browseequips");

                    b.Property<string>("BrowsePages")
                        .HasColumnType("longtext")
                        .HasColumnName("browsepages");

                    b.Property<string>("ControlEquips")
                        .HasColumnType("longtext")
                        .HasColumnName("controlequips");

                    b.Property<string>("ControlEquipsUnit")
                        .HasColumnType("longtext")
                        .HasColumnName("controlequips_unit");

                    b.Property<string>("Remark")
                        .HasColumnType("longtext")
                        .HasColumnName("remark");

                    b.Property<string>("SpecialBrowseEquip")
                        .HasColumnType("longtext")
                        .HasColumnName("specialbrowseequip");

                    b.Property<string>("SystemModule")
                        .HasColumnType("longtext")
                        .HasColumnName("systemmodule");

                    b.HasKey("Name");

                    b.ToTable("gwrole");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwsnapshotConfig", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("IconRes")
                        .HasColumnType("longtext")
                        .HasColumnName("iconres");

                    b.Property<int>("IsShow")
                        .HasColumnType("int")
                        .HasColumnName("isshow");

                    b.Property<int>("MaxCount")
                        .HasColumnType("int")
                        .HasColumnName("maxcount");

                    b.Property<string>("Reserve1")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve3");

                    b.Property<int>("SnapshotLevelMax")
                        .HasColumnType("int")
                        .HasColumnName("snapshotlevelmax");

                    b.Property<int>("SnapshotLevelMin")
                        .HasColumnType("int")
                        .HasColumnName("snapshotlevelmin");

                    b.Property<string>("SnapshotName")
                        .HasColumnType("longtext")
                        .HasColumnName("snapshotname");

                    b.HasKey("Id");

                    b.ToTable("gwsnapshotconfig");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.Gwuser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("AccessFailedCount")
                        .HasColumnType("int")
                        .HasColumnName("accessfailedcount");

                    b.Property<string>("AutoInspectionPages")
                        .HasColumnType("longtext")
                        .HasColumnName("autoinspectionpages");

                    b.Property<string>("ControlLevel")
                        .HasColumnType("longtext")
                        .HasColumnName("controllevel");

                    b.Property<bool?>("FirstLogin")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("firstlogin");

                    b.Property<string>("HistoryPasswords")
                        .HasColumnType("longtext")
                        .HasColumnName("historypasswords");

                    b.Property<string>("HomePages")
                        .HasColumnType("longtext")
                        .HasColumnName("homepages");

                    b.Property<bool?>("LockoutEnabled")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("lockoutenabled");

                    b.Property<DateTime?>("LockoutEnd")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("lockoutend");

                    b.Property<string>("Name")
                        .HasColumnType("longtext")
                        .HasColumnName("name");

                    b.Property<string>("Password")
                        .HasColumnType("longtext")
                        .HasColumnName("password");

                    b.Property<DateTime?>("PwdUpdateTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("pwdupdatetime");

                    b.Property<string>("Remark")
                        .HasColumnType("longtext")
                        .HasColumnName("remark");

                    b.Property<string>("Roles")
                        .HasColumnType("longtext")
                        .HasColumnName("roles");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("longtext")
                        .HasColumnName("securitystamp");

                    b.Property<DateTime?>("UseExpiredTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("useexpiredtime");

                    b.HasKey("Id");

                    b.ToTable("gwuser");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwziChanRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("ItemAddDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("itemadddate");

                    b.Property<string>("ItemAddMan")
                        .HasColumnType("longtext")
                        .HasColumnName("itemaddman");

                    b.Property<string>("Pictures")
                        .HasColumnType("longtext")
                        .HasColumnName("pictures");

                    b.Property<string>("Reserve1")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve3");

                    b.Property<DateTime>("WeiHuDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("weihudate");

                    b.Property<string>("WeiHuName")
                        .HasColumnType("longtext")
                        .HasColumnName("weihuname");

                    b.Property<string>("WeiHuRecord")
                        .HasColumnType("longtext")
                        .HasColumnName("weihurecord");

                    b.Property<string>("ZiChanId")
                        .HasColumnType("longtext")
                        .HasColumnName("zichanid");

                    b.HasKey("Id");

                    b.ToTable("gwzichanrecord");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwziChanTable", b =>
                {
                    b.Property<string>("ZiChanId")
                        .HasColumnType("varchar(255)")
                        .HasColumnName("zichanid");

                    b.Property<DateTime>("AnZhuangDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("anzhuangdate");

                    b.Property<DateTime>("BaoXiuQiXian")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("baoxiuqixian");

                    b.Property<string>("ChangJia")
                        .HasColumnType("longtext")
                        .HasColumnName("changjia");

                    b.Property<DateTime>("GouMaiDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("goumaidate");

                    b.Property<DateTime>("LastEditDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("lasteditdate");

                    b.Property<string>("LastEditMan")
                        .HasColumnType("longtext")
                        .HasColumnName("lasteditman");

                    b.Property<string>("LianxiMail")
                        .HasColumnType("longtext")
                        .HasColumnName("lianximail");

                    b.Property<string>("LianxiRen")
                        .HasColumnType("longtext")
                        .HasColumnName("lianxiren");

                    b.Property<string>("LianxiTel")
                        .HasColumnType("longtext")
                        .HasColumnName("lianxitel");

                    b.Property<string>("RelatedPic")
                        .HasColumnType("longtext")
                        .HasColumnName("related_pic");

                    b.Property<string>("Reserve1")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve3");

                    b.Property<int>("WeiHuCycle")
                        .HasColumnType("int")
                        .HasColumnName("weihucycle");

                    b.Property<DateTime>("WeiHuDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("weihudate");

                    b.Property<string>("ZiChanImage")
                        .HasColumnType("longtext")
                        .HasColumnName("zichanimage");

                    b.Property<string>("ZiChanName")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("zichanname");

                    b.Property<string>("ZiChanSite")
                        .HasColumnType("longtext")
                        .HasColumnName("zichansite");

                    b.Property<string>("ZiChanType")
                        .HasColumnType("longtext")
                        .HasColumnName("zichantype");

                    b.HasKey("ZiChanId");

                    b.ToTable("gwzichantable");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.IoTAccountPasswordRule", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("JSON")
                        .HasColumnType("longtext")
                        .HasColumnName("json");

                    b.HasKey("Id");

                    b.ToTable("iotaccountpasswordrule");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.IoTdevice", b =>
                {
                    b.Property<string>("DeviceId")
                        .HasColumnType("varchar(255)")
                        .HasColumnName("deviceid");

                    b.Property<string>("AreaName")
                        .HasColumnType("longtext")
                        .HasColumnName("areaname");

                    b.Property<string>("BuildName")
                        .HasColumnType("longtext")
                        .HasColumnName("buildname");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("createtime");

                    b.Property<string>("Description")
                        .HasColumnType("longtext")
                        .HasColumnName("description");

                    b.Property<string>("DeviceModel")
                        .HasColumnType("longtext")
                        .HasColumnName("devicemodel");

                    b.Property<string>("DeviceType")
                        .HasColumnType("longtext")
                        .HasColumnName("devicetype");

                    b.Property<int>("EquipNo")
                        .HasColumnType("int")
                        .HasColumnName("equipno");

                    b.Property<string>("FwVersion")
                        .HasColumnType("longtext")
                        .HasColumnName("fwversion");

                    b.Property<string>("GatewayId")
                        .HasColumnType("longtext")
                        .HasColumnName("gatewayid");

                    b.Property<string>("Height")
                        .HasColumnType("longtext")
                        .HasColumnName("height");

                    b.Property<string>("HwVersion")
                        .HasColumnType("longtext")
                        .HasColumnName("hwversion");

                    b.Property<DateTime>("LastModifiedTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("lastmodifiedtime");

                    b.Property<string>("Latitude")
                        .HasColumnType("longtext")
                        .HasColumnName("latitude");

                    b.Property<string>("Location")
                        .HasColumnType("longtext")
                        .HasColumnName("location");

                    b.Property<string>("Longitude")
                        .HasColumnType("longtext")
                        .HasColumnName("longitude");

                    b.Property<string>("Mac")
                        .HasColumnType("longtext")
                        .HasColumnName("mac");

                    b.Property<string>("ManufacturerName")
                        .HasColumnType("longtext")
                        .HasColumnName("manufacturername");

                    b.Property<string>("OtherData")
                        .HasColumnType("longtext")
                        .HasColumnName("otherdata");

                    b.Property<string>("ProtocolType")
                        .HasColumnType("longtext")
                        .HasColumnName("protocoltype");

                    b.Property<string>("SceneParam")
                        .HasColumnType("longtext")
                        .HasColumnName("sceneparam");

                    b.Property<string>("SerialNumber")
                        .HasColumnType("longtext")
                        .HasColumnName("serialnumber");

                    b.Property<string>("SigVersion")
                        .HasColumnType("longtext")
                        .HasColumnName("sigversion");

                    b.Property<string>("SwVersion")
                        .HasColumnType("longtext")
                        .HasColumnName("swversion");

                    b.Property<string>("SystemName")
                        .HasColumnType("longtext")
                        .HasColumnName("systemname");

                    b.Property<string>("UnitName")
                        .HasColumnType("longtext")
                        .HasColumnName("unitname");

                    b.Property<string>("VideoParam")
                        .HasColumnType("longtext")
                        .HasColumnName("videoparam");

                    b.HasKey("DeviceId");

                    b.ToTable("iotdevice");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.IotEquip", b =>
                {
                    b.Property<int>("EquipNo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("equip_no");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("EquipNo"));

                    b.Property<int>("AccCyc")
                        .HasColumnType("int")
                        .HasColumnName("acc_cyc");

                    b.Property<int?>("AlarmRiseCycle")
                        .HasColumnType("int")
                        .HasColumnName("alarmrisecycle");

                    b.Property<int>("AlarmScheme")
                        .HasColumnType("int")
                        .HasColumnName("alarm_scheme");

                    b.Property<int>("Attrib")
                        .HasColumnType("int")
                        .HasColumnName("attrib");

                    b.Property<string>("Backup")
                        .HasColumnType("longtext")
                        .HasColumnName("backup");

                    b.Property<string>("CommunicationDrv")
                        .HasColumnType("longtext")
                        .HasColumnName("communication_drv");

                    b.Property<string>("CommunicationParam")
                        .HasColumnType("longtext")
                        .HasColumnName("communication_param");

                    b.Property<string>("CommunicationTimeParam")
                        .HasColumnType("longtext")
                        .HasColumnName("communication_time_param");

                    b.Property<string>("Contacted")
                        .HasColumnType("longtext")
                        .HasColumnName("contacted");

                    b.Property<string>("EquipAddr")
                        .HasColumnType("longtext")
                        .HasColumnName("equip_addr");

                    b.Property<int?>("EquipConnType")
                        .HasColumnType("int")
                        .HasColumnName("equipconntype");

                    b.Property<string>("EquipDetail")
                        .HasColumnType("longtext")
                        .HasColumnName("equip_detail");

                    b.Property<string>("EquipNm")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("equip_nm");

                    b.Property<string>("EventWav")
                        .HasColumnType("longtext")
                        .HasColumnName("event_wav");

                    b.Property<string>("LocalAddr")
                        .HasColumnType("longtext")
                        .HasColumnName("local_addr");

                    b.Property<string>("OutOfContact")
                        .HasColumnType("longtext")
                        .HasColumnName("out_of_contact");

                    b.Property<string>("PlanNo")
                        .HasColumnType("longtext")
                        .HasColumnName("planno");

                    b.Property<string>("ProcAdvice")
                        .HasColumnType("longtext")
                        .HasColumnName("proc_advice");

                    b.Property<int>("RawEquipNo")
                        .HasColumnType("int")
                        .HasColumnName("raw_equip_no");

                    b.Property<string>("RelatedPic")
                        .HasColumnType("longtext")
                        .HasColumnName("related_pic");

                    b.Property<string>("RelatedVideo")
                        .HasColumnType("longtext")
                        .HasColumnName("related_video");

                    b.Property<string>("Reserve1")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve3");

                    b.Property<string>("SafeTime")
                        .HasColumnType("longtext")
                        .HasColumnName("safetime");

                    b.Property<string>("StaIp")
                        .HasColumnType("longtext")
                        .HasColumnName("sta_ip");

                    b.Property<int>("StaN")
                        .HasColumnType("int")
                        .HasColumnName("sta_n");

                    b.Property<string>("Tabname")
                        .HasColumnType("longtext")
                        .HasColumnName("tabname");

                    b.Property<string>("ThingModelJson")
                        .HasColumnType("longtext")
                        .HasColumnName("thingmodeljson");

                    b.Property<string>("ZiChanId")
                        .HasColumnType("longtext")
                        .HasColumnName("zichanid");

                    b.HasKey("EquipNo");

                    b.ToTable("iotequip");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.IotSetParm", b =>
                {
                    b.Property<int>("EquipNo")
                        .HasColumnType("int")
                        .HasColumnName("equip_no");

                    b.Property<int>("SetNo")
                        .HasColumnType("int")
                        .HasColumnName("set_no");

                    b.Property<string>("Action")
                        .HasColumnType("longtext")
                        .HasColumnName("action");

                    b.Property<bool>("Canexecution")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("canexecution");

                    b.Property<bool>("EnableVoice")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("enablevoice");

                    b.Property<string>("MainInstruction")
                        .HasColumnType("longtext")
                        .HasColumnName("main_instruction");

                    b.Property<string>("MinorInstruction")
                        .HasColumnType("longtext")
                        .HasColumnName("minor_instruction");

                    b.Property<int>("QrEquipNo")
                        .HasColumnType("int")
                        .HasColumnName("qr_equip_no");

                    b.Property<bool>("Record")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("record");

                    b.Property<string>("Reserve1")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve3");

                    b.Property<string>("SetCode")
                        .HasColumnType("longtext")
                        .HasColumnName("set_code");

                    b.Property<string>("SetNm")
                        .HasColumnType("longtext")
                        .HasColumnName("set_nm");

                    b.Property<string>("SetType")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("set_type");

                    b.Property<int>("StaN")
                        .HasColumnType("int")
                        .HasColumnName("sta_n");

                    b.Property<string>("Value")
                        .HasColumnType("longtext")
                        .HasColumnName("value");

                    b.Property<string>("VoiceKeys")
                        .HasColumnType("longtext")
                        .HasColumnName("voicekeys");

                    b.HasKey("EquipNo", "SetNo");

                    b.ToTable("iotsetparm");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.IotYcp", b =>
                {
                    b.Property<int>("EquipNo")
                        .HasColumnType("int")
                        .HasColumnName("equip_no");

                    b.Property<int>("YcNo")
                        .HasColumnType("int")
                        .HasColumnName("yc_no");

                    b.Property<int>("AlarmAcceptableTime")
                        .HasColumnType("int")
                        .HasColumnName("alarm_acceptable_time");

                    b.Property<int>("AlarmRepeatTime")
                        .HasColumnType("int")
                        .HasColumnName("alarm_repeat_time");

                    b.Property<int?>("AlarmRiseCycle")
                        .HasColumnType("int")
                        .HasColumnName("alarmrisecycle");

                    b.Property<int>("AlarmScheme")
                        .HasColumnType("int")
                        .HasColumnName("alarm_scheme");

                    b.Property<string>("AlarmShield")
                        .HasColumnType("longtext")
                        .HasColumnName("alarm_shield");

                    b.Property<double?>("CurveLimit")
                        .HasColumnType("double")
                        .HasColumnName("curve_limit");

                    b.Property<bool>("CurveRcd")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("curve_rcd");

                    b.Property<string>("DataType")
                        .HasColumnType("longtext")
                        .HasColumnName("datatype");

                    b.Property<int>("LvlLevel")
                        .HasColumnType("int")
                        .HasColumnName("lvl_level");

                    b.Property<string>("MainInstruction")
                        .HasColumnType("longtext")
                        .HasColumnName("main_instruction");

                    b.Property<bool>("Mapping")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("mapping");

                    b.Property<string>("MinorInstruction")
                        .HasColumnType("longtext")
                        .HasColumnName("minor_instruction");

                    b.Property<string>("OutmaxEvt")
                        .HasColumnType("longtext")
                        .HasColumnName("outmax_evt");

                    b.Property<string>("OutminEvt")
                        .HasColumnType("longtext")
                        .HasColumnName("outmin_evt");

                    b.Property<double>("PhysicMax")
                        .HasColumnType("double")
                        .HasColumnName("physic_max");

                    b.Property<double>("PhysicMin")
                        .HasColumnType("double")
                        .HasColumnName("physic_min");

                    b.Property<string>("PlanNo")
                        .HasColumnType("longtext")
                        .HasColumnName("planno");

                    b.Property<string>("ProcAdvice")
                        .HasColumnType("longtext")
                        .HasColumnName("proc_advice");

                    b.Property<string>("RelatedPic")
                        .HasColumnType("longtext")
                        .HasColumnName("related_pic");

                    b.Property<string>("RelatedVideo")
                        .HasColumnType("longtext")
                        .HasColumnName("related_video");

                    b.Property<string>("Reserve1")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve3");

                    b.Property<int>("RestoreAcceptableTime")
                        .HasColumnType("int")
                        .HasColumnName("restore_acceptable_time");

                    b.Property<double>("RestoreMax")
                        .HasColumnType("double")
                        .HasColumnName("restore_max");

                    b.Property<double>("RestoreMin")
                        .HasColumnType("double")
                        .HasColumnName("restore_min");

                    b.Property<DateTime?>("SafeBgn")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("safe_bgn");

                    b.Property<DateTime?>("SafeEnd")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("safe_end");

                    b.Property<string>("SafeTime")
                        .HasColumnType("longtext")
                        .HasColumnName("safetime");

                    b.Property<int>("StaN")
                        .HasColumnType("int")
                        .HasColumnName("sta_n");

                    b.Property<string>("Unit")
                        .HasColumnType("longtext")
                        .HasColumnName("unit");

                    b.Property<double>("ValMax")
                        .HasColumnType("double")
                        .HasColumnName("val_max");

                    b.Property<double>("ValMin")
                        .HasColumnType("double")
                        .HasColumnName("val_min");

                    b.Property<int>("ValTrait")
                        .HasColumnType("int")
                        .HasColumnName("val_trait");

                    b.Property<string>("WaveFile")
                        .HasColumnType("longtext")
                        .HasColumnName("wave_file");

                    b.Property<string>("YcCode")
                        .HasColumnType("longtext")
                        .HasColumnName("yc_code");

                    b.Property<double>("YcMax")
                        .HasColumnType("double")
                        .HasColumnName("yc_max");

                    b.Property<double>("YcMin")
                        .HasColumnType("double")
                        .HasColumnName("yc_min");

                    b.Property<string>("YcNm")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("yc_nm");

                    b.Property<string>("ZiChanId")
                        .HasColumnType("longtext")
                        .HasColumnName("zichanid");

                    b.HasKey("EquipNo", "YcNo");

                    b.ToTable("iotycp");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.IotYxp", b =>
                {
                    b.Property<int>("EquipNo")
                        .HasColumnType("int")
                        .HasColumnName("equip_no");

                    b.Property<int>("YxNo")
                        .HasColumnType("int")
                        .HasColumnName("yx_no");

                    b.Property<int>("AlarmAcceptableTime")
                        .HasColumnType("int")
                        .HasColumnName("alarm_acceptable_time");

                    b.Property<int>("AlarmRepeatTime")
                        .HasColumnType("int")
                        .HasColumnName("alarm_repeat_time");

                    b.Property<int?>("AlarmRiseCycle")
                        .HasColumnType("int")
                        .HasColumnName("alarmrisecycle");

                    b.Property<int>("AlarmScheme")
                        .HasColumnType("int")
                        .HasColumnName("alarm_scheme");

                    b.Property<string>("AlarmShield")
                        .HasColumnType("longtext")
                        .HasColumnName("alarm_shield");

                    b.Property<bool>("CurveRcd")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("curve_rcd");

                    b.Property<string>("DataType")
                        .HasColumnType("longtext")
                        .HasColumnName("datatype");

                    b.Property<string>("Evt01")
                        .HasColumnType("longtext")
                        .HasColumnName("evt_01");

                    b.Property<string>("Evt10")
                        .HasColumnType("longtext")
                        .HasColumnName("evt_10");

                    b.Property<int>("Initval")
                        .HasColumnType("int")
                        .HasColumnName("initval");

                    b.Property<bool>("Inversion")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("inversion");

                    b.Property<int>("LevelD")
                        .HasColumnType("int")
                        .HasColumnName("level_d");

                    b.Property<int>("LevelR")
                        .HasColumnType("int")
                        .HasColumnName("level_r");

                    b.Property<string>("MainInstruction")
                        .HasColumnType("longtext")
                        .HasColumnName("main_instruction");

                    b.Property<string>("MinorInstruction")
                        .HasColumnType("longtext")
                        .HasColumnName("minor_instruction");

                    b.Property<string>("PlanNo")
                        .HasColumnType("longtext")
                        .HasColumnName("planno");

                    b.Property<string>("ProcAdviceD")
                        .HasColumnType("longtext")
                        .HasColumnName("proc_advice_d");

                    b.Property<string>("ProcAdviceR")
                        .HasColumnType("longtext")
                        .HasColumnName("proc_advice_r");

                    b.Property<string>("RelatedPic")
                        .HasColumnType("longtext")
                        .HasColumnName("related_pic");

                    b.Property<string>("RelatedVideo")
                        .HasColumnType("longtext")
                        .HasColumnName("related_video");

                    b.Property<string>("Reserve1")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve3");

                    b.Property<int>("RestoreAcceptableTime")
                        .HasColumnType("int")
                        .HasColumnName("restore_acceptable_time");

                    b.Property<DateTime?>("SafeBgn")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("safe_bgn");

                    b.Property<DateTime?>("SafeEnd")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("safe_end");

                    b.Property<string>("SafeTime")
                        .HasColumnType("longtext")
                        .HasColumnName("safetime");

                    b.Property<int>("StaN")
                        .HasColumnType("int")
                        .HasColumnName("sta_n");

                    b.Property<int>("ValTrait")
                        .HasColumnType("int")
                        .HasColumnName("val_trait");

                    b.Property<string>("WaveFile")
                        .HasColumnType("longtext")
                        .HasColumnName("wave_file");

                    b.Property<string>("YxCode")
                        .HasColumnType("longtext")
                        .HasColumnName("yx_code");

                    b.Property<string>("YxNm")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("yx_nm");

                    b.Property<string>("ZiChanId")
                        .HasColumnType("longtext")
                        .HasColumnName("zichanid");

                    b.HasKey("EquipNo", "YxNo");

                    b.ToTable("iotyxp");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.SetEvt", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ConfirmName")
                        .HasColumnType("longtext")
                        .HasColumnName("confirmname");

                    b.Property<DateTime>("ConfirmTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("confirmtime");

                    b.Property<string>("Confirmremark")
                        .HasColumnType("longtext")
                        .HasColumnName("confirmremark");

                    b.Property<int>("EquipNo")
                        .HasColumnType("int")
                        .HasColumnName("equip_no");

                    b.Property<string>("GUID")
                        .HasColumnType("longtext")
                        .HasColumnName("guid");

                    b.Property<string>("Gwevent")
                        .HasColumnType("longtext")
                        .HasColumnName("gwevent");

                    b.Property<string>("Gwoperator")
                        .HasColumnType("longtext")
                        .HasColumnName("gwoperator");

                    b.Property<string>("Gwsource")
                        .HasColumnType("longtext")
                        .HasColumnName("gwsource");

                    b.Property<DateTime>("Gwtime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("gwtime");

                    b.Property<int>("SetNo")
                        .HasColumnType("int")
                        .HasColumnName("set_no");

                    b.Property<int>("StaN")
                        .HasColumnType("int")
                        .HasColumnName("sta_n");

                    b.HasKey("Id");

                    b.ToTable("setevt");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.SetParm", b =>
                {
                    b.Property<int>("EquipNo")
                        .HasColumnType("int")
                        .HasColumnName("equip_no");

                    b.Property<int>("SetNo")
                        .HasColumnType("int")
                        .HasColumnName("set_no");

                    b.Property<string>("Action")
                        .HasColumnType("longtext")
                        .HasColumnName("action");

                    b.Property<bool>("Canexecution")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("canexecution");

                    b.Property<bool>("EnableVoice")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("enablevoice");

                    b.Property<string>("MainInstruction")
                        .HasColumnType("longtext")
                        .HasColumnName("main_instruction");

                    b.Property<string>("MinorInstruction")
                        .HasColumnType("longtext")
                        .HasColumnName("minor_instruction");

                    b.Property<int>("QrEquipNo")
                        .HasColumnType("int")
                        .HasColumnName("qr_equip_no");

                    b.Property<bool>("Record")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("record");

                    b.Property<string>("Reserve1")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve3");

                    b.Property<string>("SetCode")
                        .HasColumnType("longtext")
                        .HasColumnName("set_code");

                    b.Property<string>("SetNm")
                        .HasColumnType("longtext")
                        .HasColumnName("set_nm");

                    b.Property<string>("SetType")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("set_type");

                    b.Property<int>("StaN")
                        .HasColumnType("int")
                        .HasColumnName("sta_n");

                    b.Property<string>("Value")
                        .HasColumnType("longtext")
                        .HasColumnName("value");

                    b.Property<string>("VoiceKeys")
                        .HasColumnType("longtext")
                        .HasColumnName("voicekeys");

                    b.HasKey("EquipNo", "SetNo");

                    b.ToTable("setparm");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.SpeAlmReport", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Administrator")
                        .HasColumnType("longtext")
                        .HasColumnName("administrator");

                    b.Property<DateTime>("BeginTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("begin_time");

                    b.Property<string>("Color")
                        .HasColumnType("longtext")
                        .HasColumnName("color");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("end_time");

                    b.Property<int>("GroupNo")
                        .HasColumnType("int")
                        .HasColumnName("group_no");

                    b.Property<string>("Name")
                        .HasColumnType("longtext")
                        .HasColumnName("name");

                    b.Property<string>("Remark")
                        .HasColumnType("longtext")
                        .HasColumnName("remark");

                    b.Property<int>("StaN")
                        .HasColumnType("int")
                        .HasColumnName("sta_n");

                    b.HasKey("Id");

                    b.ToTable("spealmreport");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.SysEvt", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Confirmname")
                        .HasColumnType("longtext")
                        .HasColumnName("confirmname");

                    b.Property<string>("Confirmremark")
                        .HasColumnType("longtext")
                        .HasColumnName("confirmremark");

                    b.Property<DateTime>("Confirmtime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("confirmtime");

                    b.Property<string>("Event")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("event");

                    b.Property<string>("Guid")
                        .HasColumnType("longtext")
                        .HasColumnName("guid");

                    b.Property<int>("StaN")
                        .HasColumnType("int")
                        .HasColumnName("sta_n");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("time");

                    b.HasKey("Id");

                    b.ToTable("sysevt");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.YcYxEvt", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Alarmlevel")
                        .HasColumnType("int")
                        .HasColumnName("alarmlevel");

                    b.Property<int>("Alarmstate")
                        .HasColumnType("int")
                        .HasColumnName("alarmstate");

                    b.Property<string>("Confirmname")
                        .HasColumnType("longtext")
                        .HasColumnName("confirmname");

                    b.Property<string>("Confirmremark")
                        .HasColumnType("longtext")
                        .HasColumnName("confirmremark");

                    b.Property<DateTime>("Confirmtime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("confirmtime");

                    b.Property<int>("EquipNo")
                        .HasColumnType("int")
                        .HasColumnName("equip_no");

                    b.Property<string>("Event")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("event");

                    b.Property<string>("Guid")
                        .HasColumnType("longtext")
                        .HasColumnName("guid");

                    b.Property<string>("ProcRec")
                        .HasColumnType("longtext")
                        .HasColumnName("proc_rec");

                    b.Property<int>("Snapshotlevel")
                        .HasColumnType("int")
                        .HasColumnName("snapshotlevel");

                    b.Property<int>("StaN")
                        .HasColumnType("int")
                        .HasColumnName("sta_n");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("time");

                    b.Property<bool>("WuBao")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("wubao");

                    b.Property<int>("YcyxNo")
                        .HasColumnType("int")
                        .HasColumnName("ycyx_no");

                    b.Property<string>("YcyxType")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("ycyx_type");

                    b.HasKey("Id");

                    b.ToTable("ycyxevt");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.Ycp", b =>
                {
                    b.Property<int>("EquipNo")
                        .HasColumnType("int")
                        .HasColumnName("equip_no");

                    b.Property<int>("YcNo")
                        .HasColumnType("int")
                        .HasColumnName("yc_no");

                    b.Property<int>("AlarmAcceptableTime")
                        .HasColumnType("int")
                        .HasColumnName("alarm_acceptable_time");

                    b.Property<int>("AlarmRepeatTime")
                        .HasColumnType("int")
                        .HasColumnName("alarm_repeat_time");

                    b.Property<int?>("AlarmRiseCycle")
                        .HasColumnType("int")
                        .HasColumnName("alarmrisecycle");

                    b.Property<int>("AlarmScheme")
                        .HasColumnType("int")
                        .HasColumnName("alarm_scheme");

                    b.Property<string>("AlarmShield")
                        .HasColumnType("longtext")
                        .HasColumnName("alarm_shield");

                    b.Property<double?>("CurveLimit")
                        .HasColumnType("double")
                        .HasColumnName("curve_limit");

                    b.Property<bool>("CurveRcd")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("curve_rcd");

                    b.Property<string>("DataType")
                        .HasColumnType("longtext")
                        .HasColumnName("datatype");

                    b.Property<DateTime?>("GWTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("gwtime");

                    b.Property<string>("GWValue")
                        .HasColumnType("longtext")
                        .HasColumnName("gwvalue");

                    b.Property<int>("LvlLevel")
                        .HasColumnType("int")
                        .HasColumnName("lvl_level");

                    b.Property<string>("MainInstruction")
                        .HasColumnType("longtext")
                        .HasColumnName("main_instruction");

                    b.Property<bool?>("Mapping")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("mapping");

                    b.Property<string>("MinorInstruction")
                        .HasColumnType("longtext")
                        .HasColumnName("minor_instruction");

                    b.Property<string>("OutmaxEvt")
                        .HasColumnType("longtext")
                        .HasColumnName("outmax_evt");

                    b.Property<string>("OutminEvt")
                        .HasColumnType("longtext")
                        .HasColumnName("outmin_evt");

                    b.Property<double>("PhysicMax")
                        .HasColumnType("double")
                        .HasColumnName("physic_max");

                    b.Property<double>("PhysicMin")
                        .HasColumnType("double")
                        .HasColumnName("physic_min");

                    b.Property<string>("PlanNo")
                        .HasColumnType("longtext")
                        .HasColumnName("planno");

                    b.Property<string>("ProcAdvice")
                        .HasColumnType("longtext")
                        .HasColumnName("proc_advice");

                    b.Property<string>("RelatedPic")
                        .HasColumnType("longtext")
                        .HasColumnName("related_pic");

                    b.Property<string>("RelatedVideo")
                        .HasColumnType("longtext")
                        .HasColumnName("related_video");

                    b.Property<string>("Reserve1")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve3");

                    b.Property<int>("RestoreAcceptableTime")
                        .HasColumnType("int")
                        .HasColumnName("restore_acceptable_time");

                    b.Property<double>("RestoreMax")
                        .HasColumnType("double")
                        .HasColumnName("restore_max");

                    b.Property<double>("RestoreMin")
                        .HasColumnType("double")
                        .HasColumnName("restore_min");

                    b.Property<DateTime?>("SafeBgn")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("safe_bgn");

                    b.Property<DateTime?>("SafeEnd")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("safe_end");

                    b.Property<string>("SafeTime")
                        .HasColumnType("longtext")
                        .HasColumnName("safetime");

                    b.Property<int>("StaN")
                        .HasColumnType("int")
                        .HasColumnName("sta_n");

                    b.Property<string>("Unit")
                        .HasColumnType("longtext")
                        .HasColumnName("unit");

                    b.Property<double>("ValMax")
                        .HasColumnType("double")
                        .HasColumnName("val_max");

                    b.Property<double>("ValMin")
                        .HasColumnType("double")
                        .HasColumnName("val_min");

                    b.Property<int>("ValTrait")
                        .HasColumnType("int")
                        .HasColumnName("val_trait");

                    b.Property<string>("WaveFile")
                        .HasColumnType("longtext")
                        .HasColumnName("wave_file");

                    b.Property<string>("YcCode")
                        .HasColumnType("longtext")
                        .HasColumnName("yc_code");

                    b.Property<double>("YcMax")
                        .HasColumnType("double")
                        .HasColumnName("yc_max");

                    b.Property<double>("YcMin")
                        .HasColumnType("double")
                        .HasColumnName("yc_min");

                    b.Property<string>("YcNm")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("yc_nm");

                    b.Property<string>("ZiChanId")
                        .HasColumnType("longtext")
                        .HasColumnName("zichanid");

                    b.HasKey("EquipNo", "YcNo");

                    b.ToTable("ycp");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.Yxp", b =>
                {
                    b.Property<int>("EquipNo")
                        .HasColumnType("int")
                        .HasColumnName("equip_no");

                    b.Property<int>("YxNo")
                        .HasColumnType("int")
                        .HasColumnName("yx_no");

                    b.Property<int>("AlarmAcceptableTime")
                        .HasColumnType("int")
                        .HasColumnName("alarm_acceptable_time");

                    b.Property<int>("AlarmRepeatTime")
                        .HasColumnType("int")
                        .HasColumnName("alarm_repeat_time");

                    b.Property<int?>("AlarmRiseCycle")
                        .HasColumnType("int")
                        .HasColumnName("alarmrisecycle");

                    b.Property<int>("AlarmScheme")
                        .HasColumnType("int")
                        .HasColumnName("alarm_scheme");

                    b.Property<string>("AlarmShield")
                        .HasColumnType("longtext")
                        .HasColumnName("alarm_shield");

                    b.Property<bool>("CurveRcd")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("curve_rcd");

                    b.Property<string>("DataType")
                        .HasColumnType("longtext")
                        .HasColumnName("datatype");

                    b.Property<string>("Evt01")
                        .HasColumnType("longtext")
                        .HasColumnName("evt_01");

                    b.Property<string>("Evt10")
                        .HasColumnType("longtext")
                        .HasColumnName("evt_10");

                    b.Property<DateTime?>("GWTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("gwtime");

                    b.Property<string>("GWValue")
                        .HasColumnType("longtext")
                        .HasColumnName("gwvalue");

                    b.Property<int>("Initval")
                        .HasColumnType("int")
                        .HasColumnName("initval");

                    b.Property<bool>("Inversion")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("inversion");

                    b.Property<int>("LevelD")
                        .HasColumnType("int")
                        .HasColumnName("level_d");

                    b.Property<int>("LevelR")
                        .HasColumnType("int")
                        .HasColumnName("level_r");

                    b.Property<string>("MainInstruction")
                        .HasColumnType("longtext")
                        .HasColumnName("main_instruction");

                    b.Property<string>("MinorInstruction")
                        .HasColumnType("longtext")
                        .HasColumnName("minor_instruction");

                    b.Property<string>("PlanNo")
                        .HasColumnType("longtext")
                        .HasColumnName("planno");

                    b.Property<string>("ProcAdviceD")
                        .HasColumnType("longtext")
                        .HasColumnName("proc_advice_d");

                    b.Property<string>("ProcAdviceR")
                        .HasColumnType("longtext")
                        .HasColumnName("proc_advice_r");

                    b.Property<string>("RelatedPic")
                        .HasColumnType("longtext")
                        .HasColumnName("related_pic");

                    b.Property<string>("RelatedVideo")
                        .HasColumnType("longtext")
                        .HasColumnName("related_video");

                    b.Property<string>("Reserve1")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("longtext")
                        .HasColumnName("reserve3");

                    b.Property<int>("RestoreAcceptableTime")
                        .HasColumnType("int")
                        .HasColumnName("restore_acceptable_time");

                    b.Property<DateTime?>("SafeBgn")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("safe_bgn");

                    b.Property<DateTime?>("SafeEnd")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("safe_end");

                    b.Property<string>("SafeTime")
                        .HasColumnType("longtext")
                        .HasColumnName("safetime");

                    b.Property<int>("StaN")
                        .HasColumnType("int")
                        .HasColumnName("sta_n");

                    b.Property<int>("ValTrait")
                        .HasColumnType("int")
                        .HasColumnName("val_trait");

                    b.Property<string>("WaveFile")
                        .HasColumnType("longtext")
                        .HasColumnName("wave_file");

                    b.Property<string>("YxCode")
                        .HasColumnType("longtext")
                        .HasColumnName("yx_code");

                    b.Property<string>("YxNm")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("yx_nm");

                    b.Property<string>("ZiChanId")
                        .HasColumnType("longtext")
                        .HasColumnName("zichanid");

                    b.HasKey("EquipNo", "YxNo");

                    b.ToTable("yxp");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.IotSetParm", b =>
                {
                    b.HasOne("IoTCenter.Data.Model.IotEquip", "IotEquip")
                        .WithMany("IotSetParms")
                        .HasForeignKey("EquipNo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("IotEquip");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.IotYcp", b =>
                {
                    b.HasOne("IoTCenter.Data.Model.IotEquip", "IotEquip")
                        .WithMany("IotYcps")
                        .HasForeignKey("EquipNo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("IotEquip");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.IotYxp", b =>
                {
                    b.HasOne("IoTCenter.Data.Model.IotEquip", "IotEquip")
                        .WithMany("IotYxps")
                        .HasForeignKey("EquipNo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("IotEquip");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.SetParm", b =>
                {
                    b.HasOne("IoTCenter.Data.Model.Equip", "Equip")
                        .WithMany("SetParms")
                        .HasForeignKey("EquipNo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Equip");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.Ycp", b =>
                {
                    b.HasOne("IoTCenter.Data.Model.Equip", "Equip")
                        .WithMany("Ycps")
                        .HasForeignKey("EquipNo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Equip");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.Yxp", b =>
                {
                    b.HasOne("IoTCenter.Data.Model.Equip", "Equip")
                        .WithMany("Yxps")
                        .HasForeignKey("EquipNo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Equip");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.Equip", b =>
                {
                    b.Navigation("SetParms");

                    b.Navigation("Ycps");

                    b.Navigation("Yxps");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.IotEquip", b =>
                {
                    b.Navigation("IotSetParms");

                    b.Navigation("IotYcps");

                    b.Navigation("IotYxps");
                });
#pragma warning restore 612, 618
        }
    }
}
