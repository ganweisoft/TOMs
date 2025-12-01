// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using IoTCenter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace IoTCenter.Data.Migrations.SQlite
{
    [DbContext(typeof(GWDbContext))]
    [Migration("20250604075504_InitialSQlite")]
    partial class InitialSQlite
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.4");

            modelBuilder.Entity("IoTCenter.Data.Model.Administrator", b =>
                {
                    b.Property<string>("AdministratorName")
                        .HasColumnType("TEXT")
                        .HasColumnName("administrator");

                    b.Property<int>("AckLevel")
                        .HasColumnType("INTEGER")
                        .HasColumnName("acklevel");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT")
                        .HasColumnName("email");

                    b.Property<string>("MobileTel")
                        .HasColumnType("TEXT")
                        .HasColumnName("mobiletel");

                    b.Property<string>("PhotoImage")
                        .HasColumnType("TEXT")
                        .HasColumnName("photoimage");

                    b.Property<string>("Reserve1")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve3");

                    b.Property<string>("Telphone")
                        .HasColumnType("TEXT")
                        .HasColumnName("telphone");

                    b.Property<string>("UserName")
                        .HasColumnType("TEXT")
                        .HasColumnName("username");

                    b.HasKey("AdministratorName");

                    b.ToTable("administrator");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.AlarmProc", b =>
                {
                    b.Property<int>("ProcCode")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("proc_code");

                    b.Property<string>("Comment")
                        .HasColumnType("TEXT")
                        .HasColumnName("comment");

                    b.Property<string>("ProcModule")
                        .HasColumnType("TEXT")
                        .HasColumnName("proc_module");

                    b.Property<string>("ProcName")
                        .HasColumnType("TEXT")
                        .HasColumnName("proc_name");

                    b.Property<string>("ProcParm")
                        .HasColumnType("TEXT")
                        .HasColumnName("proc_parm");

                    b.Property<string>("Reserve1")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve3");

                    b.HasKey("ProcCode");

                    b.ToTable("alarmproc");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.AlarmRec", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<string>("Administrator")
                        .HasColumnType("TEXT")
                        .HasColumnName("administrator");

                    b.Property<string>("Comment")
                        .HasColumnType("TEXT")
                        .HasColumnName("comment");

                    b.Property<string>("Event")
                        .HasColumnType("TEXT")
                        .HasColumnName("event");

                    b.Property<string>("ProcName")
                        .HasColumnType("TEXT")
                        .HasColumnName("proc_name");

                    b.Property<DateTime>("Time")
                        .HasColumnType("TEXT")
                        .HasColumnName("time");

                    b.HasKey("Id");

                    b.ToTable("alarmrec");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.AlmReport", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<string>("Administrator")
                        .HasColumnType("TEXT")
                        .HasColumnName("administrator");

                    b.Property<int>("GroupNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("group_no");

                    b.Property<int?>("StaN")
                        .HasColumnType("INTEGER")
                        .HasColumnName("sta_n");

                    b.HasKey("Id");

                    b.ToTable("almreport");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.AutoProc", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<int>("Delay")
                        .HasColumnType("INTEGER")
                        .HasColumnName("delay");

                    b.Property<bool>("Enable")
                        .HasColumnType("INTEGER")
                        .HasColumnName("enable");

                    b.Property<int>("IequipNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("iequip_no");

                    b.Property<int>("IycyxNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("iycyx_no");

                    b.Property<string>("IycyxType")
                        .HasColumnType("TEXT")
                        .HasColumnName("iycyx_type");

                    b.Property<int>("OequipNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("oequip_no");

                    b.Property<int>("OsetNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("oset_no");

                    b.Property<string>("ProcDesc")
                        .HasColumnType("TEXT")
                        .HasColumnName("procdesc");

                    b.Property<string>("Reserve")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT")
                        .HasColumnName("value");

                    b.HasKey("Id");

                    b.ToTable("autoproc");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.EGroup", b =>
                {
                    b.Property<int>("GroupId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("groupid");

                    b.Property<string>("GroupName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("groupname");

                    b.Property<int>("ParentGroupId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("parentgroupid");

                    b.HasKey("GroupId");

                    b.ToTable("egroup");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.EGroupList", b =>
                {
                    b.Property<int>("EGroupListId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("egrouplistid");

                    b.Property<int>("EquipNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("equipno");

                    b.Property<int>("GroupId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("groupid");

                    b.Property<int>("StaNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("stano");

                    b.HasKey("EGroupListId");

                    b.ToTable("egrouplist");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.Equip", b =>
                {
                    b.Property<int>("EquipNo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("equip_no");

                    b.Property<int>("AccCyc")
                        .HasColumnType("INTEGER")
                        .HasColumnName("acc_cyc");

                    b.Property<int?>("AlarmRiseCycle")
                        .HasColumnType("INTEGER")
                        .HasColumnName("alarmrisecycle");

                    b.Property<int>("AlarmScheme")
                        .HasColumnType("INTEGER")
                        .HasColumnName("alarm_scheme");

                    b.Property<int>("Attrib")
                        .HasColumnType("INTEGER")
                        .HasColumnName("attrib");

                    b.Property<string>("Backup")
                        .HasColumnType("TEXT")
                        .HasColumnName("backup");

                    b.Property<string>("CommunicationDrv")
                        .HasColumnType("TEXT")
                        .HasColumnName("communication_drv");

                    b.Property<string>("CommunicationParam")
                        .HasColumnType("TEXT")
                        .HasColumnName("communication_param");

                    b.Property<string>("CommunicationTimeParam")
                        .HasColumnType("TEXT")
                        .HasColumnName("communication_time_param");

                    b.Property<string>("Contacted")
                        .HasColumnType("TEXT")
                        .HasColumnName("contacted");

                    b.Property<string>("EquipAddr")
                        .HasColumnType("TEXT")
                        .HasColumnName("equip_addr");

                    b.Property<string>("EquipDetail")
                        .HasColumnType("TEXT")
                        .HasColumnName("equip_detail");

                    b.Property<string>("EquipNm")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("equip_nm");

                    b.Property<string>("EventWav")
                        .HasColumnType("TEXT")
                        .HasColumnName("event_wav");

                    b.Property<bool?>("IsDisable")
                        .HasColumnType("INTEGER")
                        .HasColumnName("isdisable");

                    b.Property<string>("LocalAddr")
                        .HasColumnType("TEXT")
                        .HasColumnName("local_addr");

                    b.Property<string>("OutOfContact")
                        .HasColumnType("TEXT")
                        .HasColumnName("out_of_contact");

                    b.Property<string>("PlanNo")
                        .HasColumnType("TEXT")
                        .HasColumnName("planno");

                    b.Property<string>("ProcAdvice")
                        .HasColumnType("TEXT")
                        .HasColumnName("proc_advice");

                    b.Property<int>("RawEquipNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("raw_equip_no");

                    b.Property<string>("RelatedPic")
                        .HasColumnType("TEXT")
                        .HasColumnName("related_pic");

                    b.Property<string>("RelatedVideo")
                        .HasColumnType("TEXT")
                        .HasColumnName("related_video");

                    b.Property<string>("Reserve1")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve3");

                    b.Property<string>("SafeTime")
                        .HasColumnType("TEXT")
                        .HasColumnName("safetime");

                    b.Property<string>("StaIp")
                        .HasColumnType("TEXT")
                        .HasColumnName("sta_ip");

                    b.Property<int>("StaN")
                        .HasColumnType("INTEGER")
                        .HasColumnName("sta_n");

                    b.Property<string>("Tabname")
                        .HasColumnType("TEXT")
                        .HasColumnName("tabname");

                    b.Property<string>("ZiChanId")
                        .HasColumnType("TEXT")
                        .HasColumnName("zichanid");

                    b.HasKey("EquipNo");

                    b.ToTable("equip");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.EquipGroup", b =>
                {
                    b.Property<int>("GroupNo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("group_no");

                    b.Property<string>("Equipcomb")
                        .HasColumnType("TEXT")
                        .HasColumnName("equipcomb");

                    b.Property<string>("GroupName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("group_name");

                    b.Property<int>("StaN")
                        .HasColumnType("INTEGER")
                        .HasColumnName("sta_n");

                    b.HasKey("GroupNo");

                    b.ToTable("equipgroup");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwdataRecordItems", b =>
                {
                    b.Property<int>("EquipNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("equip_no");

                    b.Property<string>("DataType")
                        .HasColumnType("TEXT")
                        .HasColumnName("data_type");

                    b.Property<int>("YcyxNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("ycyx_no");

                    b.Property<string>("DataName")
                        .HasColumnType("TEXT")
                        .HasColumnName("data_name");

                    b.Property<string>("RuleName")
                        .HasColumnType("TEXT")
                        .HasColumnName("rulename");

                    b.HasKey("EquipNo", "DataType", "YcyxNo");

                    b.ToTable("gwdatarecorditems");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwdelayAction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<string>("GwAddDateTime")
                        .HasColumnType("TEXT")
                        .HasColumnName("gw_adddatetime");

                    b.Property<int>("GwDelayNum")
                        .HasColumnType("INTEGER")
                        .HasColumnName("gw_delaynum");

                    b.Property<int>("GwEquipNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("gw_equip_no");

                    b.Property<int>("GwSetNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("gw_set_no");

                    b.Property<int>("GwSource")
                        .HasColumnType("INTEGER")
                        .HasColumnName("gw_source");

                    b.Property<int>("GwStaN")
                        .HasColumnType("INTEGER")
                        .HasColumnName("gw_sta_n");

                    b.Property<int>("GwState")
                        .HasColumnType("INTEGER")
                        .HasColumnName("gw_state");

                    b.Property<string>("GwUserNm")
                        .HasColumnType("TEXT")
                        .HasColumnName("gw_usernm");

                    b.Property<string>("GwValue")
                        .HasColumnType("TEXT")
                        .HasColumnName("gw_value");

                    b.Property<string>("Reserve1")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve3");

                    b.HasKey("Id");

                    b.ToTable("gwdelayaction");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwexProc", b =>
                {
                    b.Property<int>("ProcCode")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("proc_code");

                    b.Property<string>("Comment")
                        .HasColumnType("TEXT")
                        .HasColumnName("comment");

                    b.Property<string>("ProcModule")
                        .HasColumnType("TEXT")
                        .HasColumnName("proc_module");

                    b.Property<string>("ProcName")
                        .HasColumnType("TEXT")
                        .HasColumnName("proc_name");

                    b.Property<string>("ProcParm")
                        .HasColumnType("TEXT")
                        .HasColumnName("proc_parm");

                    b.Property<string>("Reserve1")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve3");

                    b.HasKey("ProcCode");

                    b.ToTable("gwexproc");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwexProcCmd", b =>
                {
                    b.Property<int>("ProcCode")
                        .HasColumnType("INTEGER")
                        .HasColumnName("proc_code");

                    b.Property<string>("CmdNm")
                        .HasColumnType("TEXT")
                        .HasColumnName("cmd_nm");

                    b.Property<string>("MainInstruction")
                        .HasColumnType("TEXT")
                        .HasColumnName("main_instruction");

                    b.Property<string>("MinorInstruction")
                        .HasColumnType("TEXT")
                        .HasColumnName("minor_instruction");

                    b.Property<bool>("Record")
                        .HasColumnType("INTEGER")
                        .HasColumnName("record");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT")
                        .HasColumnName("value");

                    b.HasKey("ProcCode", "CmdNm");

                    b.ToTable("gwexproccmd");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwprocCycleProcessTime", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<string>("ProcessOrder")
                        .HasColumnType("TEXT")
                        .HasColumnName("processorder");

                    b.Property<int?>("TableId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("tableid");

                    b.Property<int?>("Time")
                        .HasColumnType("INTEGER")
                        .HasColumnName("time");

                    b.HasKey("Id");

                    b.ToTable("gwproccycleprocesstime");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwprocCycleTable", b =>
                {
                    b.Property<int>("TableId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("tableid");

                    b.Property<int>("DoOrder")
                        .HasColumnType("INTEGER")
                        .HasColumnName("doorder");

                    b.Property<string>("CmdNm")
                        .HasColumnType("TEXT")
                        .HasColumnName("cmd_nm");

                    b.Property<int>("EquipNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("equip_no");

                    b.Property<string>("EquipSetNm")
                        .HasColumnType("TEXT")
                        .HasColumnName("equipsetnm");

                    b.Property<int>("ProcCode")
                        .HasColumnType("INTEGER")
                        .HasColumnName("proc_code");

                    b.Property<string>("ProcessOrder")
                        .HasColumnType("TEXT")
                        .HasColumnName("processorder");

                    b.Property<string>("Reserve1")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve3");

                    b.Property<int>("SetNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("set_no");

                    b.Property<int>("SleepTime")
                        .HasColumnType("INTEGER")
                        .HasColumnName("sleeptime");

                    b.Property<string>("SleepUnit")
                        .HasColumnType("TEXT")
                        .HasColumnName("sleepunit");

                    b.Property<string>("Type")
                        .HasColumnType("TEXT")
                        .HasColumnName("type");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT")
                        .HasColumnName("value");

                    b.HasKey("TableId", "DoOrder");

                    b.ToTable("gwproccycletable");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwprocCycleTlist", b =>
                {
                    b.Property<int>("TableId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("tableid");

                    b.Property<DateTime>("BeginTime")
                        .HasColumnType("TEXT")
                        .HasColumnName("begintime");

                    b.Property<bool>("CycleMustFinish")
                        .HasColumnType("INTEGER")
                        .HasColumnName("cyclemustfinish");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("TEXT")
                        .HasColumnName("endtime");

                    b.Property<int>("MaxCycleNum")
                        .HasColumnType("INTEGER")
                        .HasColumnName("maxcyclenum");

                    b.Property<string>("Reserve1")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve3");

                    b.Property<string>("TableName")
                        .HasColumnType("TEXT")
                        .HasColumnName("tablename");

                    b.Property<bool>("ZhenDianDo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("zhendiando");

                    b.Property<bool>("ZhidingDo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("zhidingdo");

                    b.Property<DateTime>("ZhidingTime")
                        .HasColumnType("TEXT")
                        .HasColumnName("zhidingtime");

                    b.HasKey("TableId");

                    b.ToTable("gwproccycletlist");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwprocProcess", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<string>("From")
                        .HasColumnType("TEXT")
                        .HasColumnName("from");

                    b.Property<int?>("TableId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("tableid");

                    b.Property<string>("TaskType")
                        .HasColumnType("TEXT")
                        .HasColumnName("tasktype");

                    b.Property<string>("To")
                        .HasColumnType("TEXT")
                        .HasColumnName("to");

                    b.HasKey("Id");

                    b.ToTable("gwprocprocess");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwprocSpecTable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<DateTime>("BeginDate")
                        .HasColumnType("TEXT")
                        .HasColumnName("begindate");

                    b.Property<string>("Color")
                        .HasColumnType("TEXT")
                        .HasColumnName("color");

                    b.Property<string>("DateName")
                        .HasColumnType("TEXT")
                        .HasColumnName("datename");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("TEXT")
                        .HasColumnName("enddate");

                    b.Property<string>("TableId")
                        .HasColumnType("TEXT")
                        .HasColumnName("tableid");

                    b.HasKey("Id");

                    b.ToTable("gwprocspectable");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwprocTaskProcessTime", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<string>("ControlType")
                        .HasColumnType("TEXT")
                        .HasColumnName("controltype");

                    b.Property<string>("ProcessOrder")
                        .HasColumnType("TEXT")
                        .HasColumnName("processorder");

                    b.Property<int?>("TableId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("tableid");

                    b.Property<DateTime>("Time")
                        .HasColumnType("TEXT")
                        .HasColumnName("time");

                    b.Property<string>("TimeType")
                        .HasColumnType("TEXT")
                        .HasColumnName("timetype");

                    b.HasKey("Id");

                    b.ToTable("gwproctaskprocesstime");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwprocTimeEqpTable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<int>("EquipNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("equip_no");

                    b.Property<string>("EquipSetNm")
                        .HasColumnType("TEXT")
                        .HasColumnName("equipsetnm");

                    b.Property<string>("ProcessOrder")
                        .HasColumnType("TEXT")
                        .HasColumnName("processorder");

                    b.Property<string>("Reserve1")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve3");

                    b.Property<int>("SetNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("set_no");

                    b.Property<int>("TableId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("tableid");

                    b.Property<DateTime>("Time")
                        .HasColumnType("TEXT")
                        .HasColumnName("time");

                    b.Property<DateTime>("TimeDur")
                        .HasColumnType("TEXT")
                        .HasColumnName("timedur");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT")
                        .HasColumnName("value");

                    b.HasKey("Id");

                    b.ToTable("gwproctimeeqptable");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwprocTimeSysTable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<string>("CmdNm")
                        .HasColumnType("TEXT")
                        .HasColumnName("cmd_nm");

                    b.Property<int>("ProcCode")
                        .HasColumnType("INTEGER")
                        .HasColumnName("proc_code");

                    b.Property<string>("ProcessOrder")
                        .HasColumnType("TEXT")
                        .HasColumnName("processorder");

                    b.Property<string>("Reserve1")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve3");

                    b.Property<int>("TableId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("tableid");

                    b.Property<DateTime>("Time")
                        .HasColumnType("TEXT")
                        .HasColumnName("time");

                    b.Property<DateTime>("TimeDur")
                        .HasColumnType("TEXT")
                        .HasColumnName("timedur");

                    b.HasKey("Id");

                    b.ToTable("gwproctimesystable");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwprocTimeTlist", b =>
                {
                    b.Property<int>("TableId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("tableid");

                    b.Property<string>("Comment")
                        .HasColumnType("TEXT")
                        .HasColumnName("comment");

                    b.Property<string>("TableName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("tablename");

                    b.HasKey("TableId");

                    b.ToTable("gwproctimetlist");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwprocWeekTable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<string>("Fri")
                        .HasColumnType("TEXT")
                        .HasColumnName("fri");

                    b.Property<string>("Mon")
                        .HasColumnType("TEXT")
                        .HasColumnName("mon");

                    b.Property<string>("Sat")
                        .HasColumnType("TEXT")
                        .HasColumnName("sat");

                    b.Property<string>("Sun")
                        .HasColumnType("TEXT")
                        .HasColumnName("sun");

                    b.Property<string>("Thurs")
                        .HasColumnType("TEXT")
                        .HasColumnName("thurs");

                    b.Property<string>("Tues")
                        .HasColumnType("TEXT")
                        .HasColumnName("tues");

                    b.Property<string>("Wed")
                        .HasColumnType("TEXT")
                        .HasColumnName("wed");

                    b.HasKey("Id");

                    b.ToTable("gwprocweektable");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.Gwrole", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.Property<string>("BrowseEquips")
                        .HasColumnType("TEXT")
                        .HasColumnName("browseequips");

                    b.Property<string>("BrowsePages")
                        .HasColumnType("TEXT")
                        .HasColumnName("browsepages");

                    b.Property<string>("ControlEquips")
                        .HasColumnType("TEXT")
                        .HasColumnName("controlequips");

                    b.Property<string>("ControlEquipsUnit")
                        .HasColumnType("TEXT")
                        .HasColumnName("controlequips_unit");

                    b.Property<string>("Remark")
                        .HasColumnType("TEXT")
                        .HasColumnName("remark");

                    b.Property<string>("SpecialBrowseEquip")
                        .HasColumnType("TEXT")
                        .HasColumnName("specialbrowseequip");

                    b.Property<string>("SystemModule")
                        .HasColumnType("TEXT")
                        .HasColumnName("systemmodule");

                    b.HasKey("Name");

                    b.ToTable("gwrole");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwsnapshotConfig", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<string>("IconRes")
                        .HasColumnType("TEXT")
                        .HasColumnName("iconres");

                    b.Property<int>("IsShow")
                        .HasColumnType("INTEGER")
                        .HasColumnName("isshow");

                    b.Property<int>("MaxCount")
                        .HasColumnType("INTEGER")
                        .HasColumnName("maxcount");

                    b.Property<string>("Reserve1")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve3");

                    b.Property<int>("SnapshotLevelMax")
                        .HasColumnType("INTEGER")
                        .HasColumnName("snapshotlevelmax");

                    b.Property<int>("SnapshotLevelMin")
                        .HasColumnType("INTEGER")
                        .HasColumnName("snapshotlevelmin");

                    b.Property<string>("SnapshotName")
                        .HasColumnType("TEXT")
                        .HasColumnName("snapshotname");

                    b.HasKey("Id");

                    b.ToTable("gwsnapshotconfig");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.Gwuser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<int?>("AccessFailedCount")
                        .HasColumnType("INTEGER")
                        .HasColumnName("accessfailedcount");

                    b.Property<string>("AutoInspectionPages")
                        .HasColumnType("TEXT")
                        .HasColumnName("autoinspectionpages");

                    b.Property<string>("ControlLevel")
                        .HasColumnType("TEXT")
                        .HasColumnName("controllevel");

                    b.Property<bool?>("FirstLogin")
                        .HasColumnType("INTEGER")
                        .HasColumnName("firstlogin");

                    b.Property<string>("HistoryPasswords")
                        .HasColumnType("TEXT")
                        .HasColumnName("historypasswords");

                    b.Property<string>("HomePages")
                        .HasColumnType("TEXT")
                        .HasColumnName("homepages");

                    b.Property<bool?>("LockoutEnabled")
                        .HasColumnType("INTEGER")
                        .HasColumnName("lockoutenabled");

                    b.Property<DateTime?>("LockoutEnd")
                        .HasColumnType("TEXT")
                        .HasColumnName("lockoutend");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.Property<string>("Password")
                        .HasColumnType("TEXT")
                        .HasColumnName("password");

                    b.Property<DateTime?>("PwdUpdateTime")
                        .HasColumnType("TEXT")
                        .HasColumnName("pwdupdatetime");

                    b.Property<string>("Remark")
                        .HasColumnType("TEXT")
                        .HasColumnName("remark");

                    b.Property<string>("Roles")
                        .HasColumnType("TEXT")
                        .HasColumnName("roles");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("TEXT")
                        .HasColumnName("securitystamp");

                    b.Property<DateTime?>("UseExpiredTime")
                        .HasColumnType("TEXT")
                        .HasColumnName("useexpiredtime");

                    b.HasKey("Id");

                    b.ToTable("gwuser");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwziChanRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<DateTime>("ItemAddDate")
                        .HasColumnType("TEXT")
                        .HasColumnName("itemadddate");

                    b.Property<string>("ItemAddMan")
                        .HasColumnType("TEXT")
                        .HasColumnName("itemaddman");

                    b.Property<string>("Pictures")
                        .HasColumnType("TEXT")
                        .HasColumnName("pictures");

                    b.Property<string>("Reserve1")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve3");

                    b.Property<DateTime>("WeiHuDate")
                        .HasColumnType("TEXT")
                        .HasColumnName("weihudate");

                    b.Property<string>("WeiHuName")
                        .HasColumnType("TEXT")
                        .HasColumnName("weihuname");

                    b.Property<string>("WeiHuRecord")
                        .HasColumnType("TEXT")
                        .HasColumnName("weihurecord");

                    b.Property<string>("ZiChanId")
                        .HasColumnType("TEXT")
                        .HasColumnName("zichanid");

                    b.HasKey("Id");

                    b.ToTable("gwzichanrecord");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.GwziChanTable", b =>
                {
                    b.Property<string>("ZiChanId")
                        .HasColumnType("TEXT")
                        .HasColumnName("zichanid");

                    b.Property<DateTime>("AnZhuangDate")
                        .HasColumnType("TEXT")
                        .HasColumnName("anzhuangdate");

                    b.Property<DateTime>("BaoXiuQiXian")
                        .HasColumnType("TEXT")
                        .HasColumnName("baoxiuqixian");

                    b.Property<string>("ChangJia")
                        .HasColumnType("TEXT")
                        .HasColumnName("changjia");

                    b.Property<DateTime>("GouMaiDate")
                        .HasColumnType("TEXT")
                        .HasColumnName("goumaidate");

                    b.Property<DateTime>("LastEditDate")
                        .HasColumnType("TEXT")
                        .HasColumnName("lasteditdate");

                    b.Property<string>("LastEditMan")
                        .HasColumnType("TEXT")
                        .HasColumnName("lasteditman");

                    b.Property<string>("LianxiMail")
                        .HasColumnType("TEXT")
                        .HasColumnName("lianximail");

                    b.Property<string>("LianxiRen")
                        .HasColumnType("TEXT")
                        .HasColumnName("lianxiren");

                    b.Property<string>("LianxiTel")
                        .HasColumnType("TEXT")
                        .HasColumnName("lianxitel");

                    b.Property<string>("RelatedPic")
                        .HasColumnType("TEXT")
                        .HasColumnName("related_pic");

                    b.Property<string>("Reserve1")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve3");

                    b.Property<int>("WeiHuCycle")
                        .HasColumnType("INTEGER")
                        .HasColumnName("weihucycle");

                    b.Property<DateTime>("WeiHuDate")
                        .HasColumnType("TEXT")
                        .HasColumnName("weihudate");

                    b.Property<string>("ZiChanImage")
                        .HasColumnType("TEXT")
                        .HasColumnName("zichanimage");

                    b.Property<string>("ZiChanName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("zichanname");

                    b.Property<string>("ZiChanSite")
                        .HasColumnType("TEXT")
                        .HasColumnName("zichansite");

                    b.Property<string>("ZiChanType")
                        .HasColumnType("TEXT")
                        .HasColumnName("zichantype");

                    b.HasKey("ZiChanId");

                    b.ToTable("gwzichantable");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.IoTAccountPasswordRule", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<string>("JSON")
                        .HasColumnType("TEXT")
                        .HasColumnName("json");

                    b.HasKey("Id");

                    b.ToTable("iotaccountpasswordrule");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.IoTdevice", b =>
                {
                    b.Property<string>("DeviceId")
                        .HasColumnType("TEXT")
                        .HasColumnName("deviceid");

                    b.Property<string>("AreaName")
                        .HasColumnType("TEXT")
                        .HasColumnName("areaname");

                    b.Property<string>("BuildName")
                        .HasColumnType("TEXT")
                        .HasColumnName("buildname");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("TEXT")
                        .HasColumnName("createtime");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT")
                        .HasColumnName("description");

                    b.Property<string>("DeviceModel")
                        .HasColumnType("TEXT")
                        .HasColumnName("devicemodel");

                    b.Property<string>("DeviceType")
                        .HasColumnType("TEXT")
                        .HasColumnName("devicetype");

                    b.Property<int>("EquipNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("equipno");

                    b.Property<string>("FwVersion")
                        .HasColumnType("TEXT")
                        .HasColumnName("fwversion");

                    b.Property<string>("GatewayId")
                        .HasColumnType("TEXT")
                        .HasColumnName("gatewayid");

                    b.Property<string>("Height")
                        .HasColumnType("TEXT")
                        .HasColumnName("height");

                    b.Property<string>("HwVersion")
                        .HasColumnType("TEXT")
                        .HasColumnName("hwversion");

                    b.Property<DateTime>("LastModifiedTime")
                        .HasColumnType("TEXT")
                        .HasColumnName("lastmodifiedtime");

                    b.Property<string>("Latitude")
                        .HasColumnType("TEXT")
                        .HasColumnName("latitude");

                    b.Property<string>("Location")
                        .HasColumnType("TEXT")
                        .HasColumnName("location");

                    b.Property<string>("Longitude")
                        .HasColumnType("TEXT")
                        .HasColumnName("longitude");

                    b.Property<string>("Mac")
                        .HasColumnType("TEXT")
                        .HasColumnName("mac");

                    b.Property<string>("ManufacturerName")
                        .HasColumnType("TEXT")
                        .HasColumnName("manufacturername");

                    b.Property<string>("OtherData")
                        .HasColumnType("TEXT")
                        .HasColumnName("otherdata");

                    b.Property<string>("ProtocolType")
                        .HasColumnType("TEXT")
                        .HasColumnName("protocoltype");

                    b.Property<string>("SceneParam")
                        .HasColumnType("TEXT")
                        .HasColumnName("sceneparam");

                    b.Property<string>("SerialNumber")
                        .HasColumnType("TEXT")
                        .HasColumnName("serialnumber");

                    b.Property<string>("SigVersion")
                        .HasColumnType("TEXT")
                        .HasColumnName("sigversion");

                    b.Property<string>("SwVersion")
                        .HasColumnType("TEXT")
                        .HasColumnName("swversion");

                    b.Property<string>("SystemName")
                        .HasColumnType("TEXT")
                        .HasColumnName("systemname");

                    b.Property<string>("UnitName")
                        .HasColumnType("TEXT")
                        .HasColumnName("unitname");

                    b.Property<string>("VideoParam")
                        .HasColumnType("TEXT")
                        .HasColumnName("videoparam");

                    b.HasKey("DeviceId");

                    b.ToTable("iotdevice");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.IotEquip", b =>
                {
                    b.Property<int>("EquipNo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("equip_no");

                    b.Property<int>("AccCyc")
                        .HasColumnType("INTEGER")
                        .HasColumnName("acc_cyc");

                    b.Property<int?>("AlarmRiseCycle")
                        .HasColumnType("INTEGER")
                        .HasColumnName("alarmrisecycle");

                    b.Property<int>("AlarmScheme")
                        .HasColumnType("INTEGER")
                        .HasColumnName("alarm_scheme");

                    b.Property<int>("Attrib")
                        .HasColumnType("INTEGER")
                        .HasColumnName("attrib");

                    b.Property<string>("Backup")
                        .HasColumnType("TEXT")
                        .HasColumnName("backup");

                    b.Property<string>("CommunicationDrv")
                        .HasColumnType("TEXT")
                        .HasColumnName("communication_drv");

                    b.Property<string>("CommunicationParam")
                        .HasColumnType("TEXT")
                        .HasColumnName("communication_param");

                    b.Property<string>("CommunicationTimeParam")
                        .HasColumnType("TEXT")
                        .HasColumnName("communication_time_param");

                    b.Property<string>("Contacted")
                        .HasColumnType("TEXT")
                        .HasColumnName("contacted");

                    b.Property<string>("EquipAddr")
                        .HasColumnType("TEXT")
                        .HasColumnName("equip_addr");

                    b.Property<int?>("EquipConnType")
                        .HasColumnType("INTEGER")
                        .HasColumnName("equipconntype");

                    b.Property<string>("EquipDetail")
                        .HasColumnType("TEXT")
                        .HasColumnName("equip_detail");

                    b.Property<string>("EquipNm")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("equip_nm");

                    b.Property<string>("EventWav")
                        .HasColumnType("TEXT")
                        .HasColumnName("event_wav");

                    b.Property<string>("LocalAddr")
                        .HasColumnType("TEXT")
                        .HasColumnName("local_addr");

                    b.Property<string>("OutOfContact")
                        .HasColumnType("TEXT")
                        .HasColumnName("out_of_contact");

                    b.Property<string>("PlanNo")
                        .HasColumnType("TEXT")
                        .HasColumnName("planno");

                    b.Property<string>("ProcAdvice")
                        .HasColumnType("TEXT")
                        .HasColumnName("proc_advice");

                    b.Property<int>("RawEquipNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("raw_equip_no");

                    b.Property<string>("RelatedPic")
                        .HasColumnType("TEXT")
                        .HasColumnName("related_pic");

                    b.Property<string>("RelatedVideo")
                        .HasColumnType("TEXT")
                        .HasColumnName("related_video");

                    b.Property<string>("Reserve1")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve3");

                    b.Property<string>("SafeTime")
                        .HasColumnType("TEXT")
                        .HasColumnName("safetime");

                    b.Property<string>("StaIp")
                        .HasColumnType("TEXT")
                        .HasColumnName("sta_ip");

                    b.Property<int>("StaN")
                        .HasColumnType("INTEGER")
                        .HasColumnName("sta_n");

                    b.Property<string>("Tabname")
                        .HasColumnType("TEXT")
                        .HasColumnName("tabname");

                    b.Property<string>("ThingModelJson")
                        .HasColumnType("TEXT")
                        .HasColumnName("thingmodeljson");

                    b.Property<string>("ZiChanId")
                        .HasColumnType("TEXT")
                        .HasColumnName("zichanid");

                    b.HasKey("EquipNo");

                    b.ToTable("iotequip");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.IotSetParm", b =>
                {
                    b.Property<int>("EquipNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("equip_no");

                    b.Property<int>("SetNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("set_no");

                    b.Property<string>("Action")
                        .HasColumnType("TEXT")
                        .HasColumnName("action");

                    b.Property<bool>("Canexecution")
                        .HasColumnType("INTEGER")
                        .HasColumnName("canexecution");

                    b.Property<bool>("EnableVoice")
                        .HasColumnType("INTEGER")
                        .HasColumnName("enablevoice");

                    b.Property<string>("MainInstruction")
                        .HasColumnType("TEXT")
                        .HasColumnName("main_instruction");

                    b.Property<string>("MinorInstruction")
                        .HasColumnType("TEXT")
                        .HasColumnName("minor_instruction");

                    b.Property<int>("QrEquipNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("qr_equip_no");

                    b.Property<bool>("Record")
                        .HasColumnType("INTEGER")
                        .HasColumnName("record");

                    b.Property<string>("Reserve1")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve3");

                    b.Property<string>("SetCode")
                        .HasColumnType("TEXT")
                        .HasColumnName("set_code");

                    b.Property<string>("SetNm")
                        .HasColumnType("TEXT")
                        .HasColumnName("set_nm");

                    b.Property<string>("SetType")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("set_type");

                    b.Property<int>("StaN")
                        .HasColumnType("INTEGER")
                        .HasColumnName("sta_n");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT")
                        .HasColumnName("value");

                    b.Property<string>("VoiceKeys")
                        .HasColumnType("TEXT")
                        .HasColumnName("voicekeys");

                    b.HasKey("EquipNo", "SetNo");

                    b.ToTable("iotsetparm");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.IotYcp", b =>
                {
                    b.Property<int>("EquipNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("equip_no");

                    b.Property<int>("YcNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("yc_no");

                    b.Property<int>("AlarmAcceptableTime")
                        .HasColumnType("INTEGER")
                        .HasColumnName("alarm_acceptable_time");

                    b.Property<int>("AlarmRepeatTime")
                        .HasColumnType("INTEGER")
                        .HasColumnName("alarm_repeat_time");

                    b.Property<int?>("AlarmRiseCycle")
                        .HasColumnType("INTEGER")
                        .HasColumnName("alarmrisecycle");

                    b.Property<int>("AlarmScheme")
                        .HasColumnType("INTEGER")
                        .HasColumnName("alarm_scheme");

                    b.Property<string>("AlarmShield")
                        .HasColumnType("TEXT")
                        .HasColumnName("alarm_shield");

                    b.Property<double?>("CurveLimit")
                        .HasColumnType("REAL")
                        .HasColumnName("curve_limit");

                    b.Property<bool>("CurveRcd")
                        .HasColumnType("INTEGER")
                        .HasColumnName("curve_rcd");

                    b.Property<string>("DataType")
                        .HasColumnType("TEXT")
                        .HasColumnName("datatype");

                    b.Property<int>("LvlLevel")
                        .HasColumnType("INTEGER")
                        .HasColumnName("lvl_level");

                    b.Property<string>("MainInstruction")
                        .HasColumnType("TEXT")
                        .HasColumnName("main_instruction");

                    b.Property<bool>("Mapping")
                        .HasColumnType("INTEGER")
                        .HasColumnName("mapping");

                    b.Property<string>("MinorInstruction")
                        .HasColumnType("TEXT")
                        .HasColumnName("minor_instruction");

                    b.Property<string>("OutmaxEvt")
                        .HasColumnType("TEXT")
                        .HasColumnName("outmax_evt");

                    b.Property<string>("OutminEvt")
                        .HasColumnType("TEXT")
                        .HasColumnName("outmin_evt");

                    b.Property<double>("PhysicMax")
                        .HasColumnType("REAL")
                        .HasColumnName("physic_max");

                    b.Property<double>("PhysicMin")
                        .HasColumnType("REAL")
                        .HasColumnName("physic_min");

                    b.Property<string>("PlanNo")
                        .HasColumnType("TEXT")
                        .HasColumnName("planno");

                    b.Property<string>("ProcAdvice")
                        .HasColumnType("TEXT")
                        .HasColumnName("proc_advice");

                    b.Property<string>("RelatedPic")
                        .HasColumnType("TEXT")
                        .HasColumnName("related_pic");

                    b.Property<string>("RelatedVideo")
                        .HasColumnType("TEXT")
                        .HasColumnName("related_video");

                    b.Property<string>("Reserve1")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve3");

                    b.Property<int>("RestoreAcceptableTime")
                        .HasColumnType("INTEGER")
                        .HasColumnName("restore_acceptable_time");

                    b.Property<double>("RestoreMax")
                        .HasColumnType("REAL")
                        .HasColumnName("restore_max");

                    b.Property<double>("RestoreMin")
                        .HasColumnType("REAL")
                        .HasColumnName("restore_min");

                    b.Property<DateTime?>("SafeBgn")
                        .HasColumnType("TEXT")
                        .HasColumnName("safe_bgn");

                    b.Property<DateTime?>("SafeEnd")
                        .HasColumnType("TEXT")
                        .HasColumnName("safe_end");

                    b.Property<string>("SafeTime")
                        .HasColumnType("TEXT")
                        .HasColumnName("safetime");

                    b.Property<int>("StaN")
                        .HasColumnType("INTEGER")
                        .HasColumnName("sta_n");

                    b.Property<string>("Unit")
                        .HasColumnType("TEXT")
                        .HasColumnName("unit");

                    b.Property<double>("ValMax")
                        .HasColumnType("REAL")
                        .HasColumnName("val_max");

                    b.Property<double>("ValMin")
                        .HasColumnType("REAL")
                        .HasColumnName("val_min");

                    b.Property<int>("ValTrait")
                        .HasColumnType("INTEGER")
                        .HasColumnName("val_trait");

                    b.Property<string>("WaveFile")
                        .HasColumnType("TEXT")
                        .HasColumnName("wave_file");

                    b.Property<string>("YcCode")
                        .HasColumnType("TEXT")
                        .HasColumnName("yc_code");

                    b.Property<double>("YcMax")
                        .HasColumnType("REAL")
                        .HasColumnName("yc_max");

                    b.Property<double>("YcMin")
                        .HasColumnType("REAL")
                        .HasColumnName("yc_min");

                    b.Property<string>("YcNm")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("yc_nm");

                    b.Property<string>("ZiChanId")
                        .HasColumnType("TEXT")
                        .HasColumnName("zichanid");

                    b.HasKey("EquipNo", "YcNo");

                    b.ToTable("iotycp");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.IotYxp", b =>
                {
                    b.Property<int>("EquipNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("equip_no");

                    b.Property<int>("YxNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("yx_no");

                    b.Property<int>("AlarmAcceptableTime")
                        .HasColumnType("INTEGER")
                        .HasColumnName("alarm_acceptable_time");

                    b.Property<int>("AlarmRepeatTime")
                        .HasColumnType("INTEGER")
                        .HasColumnName("alarm_repeat_time");

                    b.Property<int?>("AlarmRiseCycle")
                        .HasColumnType("INTEGER")
                        .HasColumnName("alarmrisecycle");

                    b.Property<int>("AlarmScheme")
                        .HasColumnType("INTEGER")
                        .HasColumnName("alarm_scheme");

                    b.Property<string>("AlarmShield")
                        .HasColumnType("TEXT")
                        .HasColumnName("alarm_shield");

                    b.Property<bool>("CurveRcd")
                        .HasColumnType("INTEGER")
                        .HasColumnName("curve_rcd");

                    b.Property<string>("DataType")
                        .HasColumnType("TEXT")
                        .HasColumnName("datatype");

                    b.Property<string>("Evt01")
                        .HasColumnType("TEXT")
                        .HasColumnName("evt_01");

                    b.Property<string>("Evt10")
                        .HasColumnType("TEXT")
                        .HasColumnName("evt_10");

                    b.Property<int>("Initval")
                        .HasColumnType("INTEGER")
                        .HasColumnName("initval");

                    b.Property<bool>("Inversion")
                        .HasColumnType("INTEGER")
                        .HasColumnName("inversion");

                    b.Property<int>("LevelD")
                        .HasColumnType("INTEGER")
                        .HasColumnName("level_d");

                    b.Property<int>("LevelR")
                        .HasColumnType("INTEGER")
                        .HasColumnName("level_r");

                    b.Property<string>("MainInstruction")
                        .HasColumnType("TEXT")
                        .HasColumnName("main_instruction");

                    b.Property<string>("MinorInstruction")
                        .HasColumnType("TEXT")
                        .HasColumnName("minor_instruction");

                    b.Property<string>("PlanNo")
                        .HasColumnType("TEXT")
                        .HasColumnName("planno");

                    b.Property<string>("ProcAdviceD")
                        .HasColumnType("TEXT")
                        .HasColumnName("proc_advice_d");

                    b.Property<string>("ProcAdviceR")
                        .HasColumnType("TEXT")
                        .HasColumnName("proc_advice_r");

                    b.Property<string>("RelatedPic")
                        .HasColumnType("TEXT")
                        .HasColumnName("related_pic");

                    b.Property<string>("RelatedVideo")
                        .HasColumnType("TEXT")
                        .HasColumnName("related_video");

                    b.Property<string>("Reserve1")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve3");

                    b.Property<int>("RestoreAcceptableTime")
                        .HasColumnType("INTEGER")
                        .HasColumnName("restore_acceptable_time");

                    b.Property<DateTime?>("SafeBgn")
                        .HasColumnType("TEXT")
                        .HasColumnName("safe_bgn");

                    b.Property<DateTime?>("SafeEnd")
                        .HasColumnType("TEXT")
                        .HasColumnName("safe_end");

                    b.Property<string>("SafeTime")
                        .HasColumnType("TEXT")
                        .HasColumnName("safetime");

                    b.Property<int>("StaN")
                        .HasColumnType("INTEGER")
                        .HasColumnName("sta_n");

                    b.Property<int>("ValTrait")
                        .HasColumnType("INTEGER")
                        .HasColumnName("val_trait");

                    b.Property<string>("WaveFile")
                        .HasColumnType("TEXT")
                        .HasColumnName("wave_file");

                    b.Property<string>("YxCode")
                        .HasColumnType("TEXT")
                        .HasColumnName("yx_code");

                    b.Property<string>("YxNm")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("yx_nm");

                    b.Property<string>("ZiChanId")
                        .HasColumnType("TEXT")
                        .HasColumnName("zichanid");

                    b.HasKey("EquipNo", "YxNo");

                    b.ToTable("iotyxp");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.SetEvt", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<string>("ConfirmName")
                        .HasColumnType("TEXT")
                        .HasColumnName("confirmname");

                    b.Property<DateTime>("ConfirmTime")
                        .HasColumnType("TEXT")
                        .HasColumnName("confirmtime");

                    b.Property<string>("Confirmremark")
                        .HasColumnType("TEXT")
                        .HasColumnName("confirmremark");

                    b.Property<int>("EquipNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("equip_no");

                    b.Property<string>("GUID")
                        .HasColumnType("TEXT")
                        .HasColumnName("guid");

                    b.Property<string>("Gwevent")
                        .HasColumnType("TEXT")
                        .HasColumnName("gwevent");

                    b.Property<string>("Gwoperator")
                        .HasColumnType("TEXT")
                        .HasColumnName("gwoperator");

                    b.Property<string>("Gwsource")
                        .HasColumnType("TEXT")
                        .HasColumnName("gwsource");

                    b.Property<DateTime>("Gwtime")
                        .HasColumnType("TEXT")
                        .HasColumnName("gwtime");

                    b.Property<int>("SetNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("set_no");

                    b.Property<int>("StaN")
                        .HasColumnType("INTEGER")
                        .HasColumnName("sta_n");

                    b.HasKey("Id");

                    b.ToTable("setevt");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.SetParm", b =>
                {
                    b.Property<int>("EquipNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("equip_no");

                    b.Property<int>("SetNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("set_no");

                    b.Property<string>("Action")
                        .HasColumnType("TEXT")
                        .HasColumnName("action");

                    b.Property<bool>("Canexecution")
                        .HasColumnType("INTEGER")
                        .HasColumnName("canexecution");

                    b.Property<bool>("EnableVoice")
                        .HasColumnType("INTEGER")
                        .HasColumnName("enablevoice");

                    b.Property<string>("MainInstruction")
                        .HasColumnType("TEXT")
                        .HasColumnName("main_instruction");

                    b.Property<string>("MinorInstruction")
                        .HasColumnType("TEXT")
                        .HasColumnName("minor_instruction");

                    b.Property<int>("QrEquipNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("qr_equip_no");

                    b.Property<bool>("Record")
                        .HasColumnType("INTEGER")
                        .HasColumnName("record");

                    b.Property<string>("Reserve1")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve3");

                    b.Property<string>("SetCode")
                        .HasColumnType("TEXT")
                        .HasColumnName("set_code");

                    b.Property<string>("SetNm")
                        .HasColumnType("TEXT")
                        .HasColumnName("set_nm");

                    b.Property<string>("SetType")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("set_type");

                    b.Property<int>("StaN")
                        .HasColumnType("INTEGER")
                        .HasColumnName("sta_n");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT")
                        .HasColumnName("value");

                    b.Property<string>("VoiceKeys")
                        .HasColumnType("TEXT")
                        .HasColumnName("voicekeys");

                    b.HasKey("EquipNo", "SetNo");

                    b.ToTable("setparm");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.SpeAlmReport", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<string>("Administrator")
                        .HasColumnType("TEXT")
                        .HasColumnName("administrator");

                    b.Property<DateTime>("BeginTime")
                        .HasColumnType("TEXT")
                        .HasColumnName("begin_time");

                    b.Property<string>("Color")
                        .HasColumnType("TEXT")
                        .HasColumnName("color");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("TEXT")
                        .HasColumnName("end_time");

                    b.Property<int>("GroupNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("group_no");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.Property<string>("Remark")
                        .HasColumnType("TEXT")
                        .HasColumnName("remark");

                    b.Property<int>("StaN")
                        .HasColumnType("INTEGER")
                        .HasColumnName("sta_n");

                    b.HasKey("Id");

                    b.ToTable("spealmreport");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.SysEvt", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<string>("Confirmname")
                        .HasColumnType("TEXT")
                        .HasColumnName("confirmname");

                    b.Property<string>("Confirmremark")
                        .HasColumnType("TEXT")
                        .HasColumnName("confirmremark");

                    b.Property<DateTime>("Confirmtime")
                        .HasColumnType("TEXT")
                        .HasColumnName("confirmtime");

                    b.Property<string>("Event")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("event");

                    b.Property<string>("Guid")
                        .HasColumnType("TEXT")
                        .HasColumnName("guid");

                    b.Property<int>("StaN")
                        .HasColumnType("INTEGER")
                        .HasColumnName("sta_n");

                    b.Property<DateTime>("Time")
                        .HasColumnType("TEXT")
                        .HasColumnName("time");

                    b.HasKey("Id");

                    b.ToTable("sysevt");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.YcYxEvt", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<int>("Alarmlevel")
                        .HasColumnType("INTEGER")
                        .HasColumnName("alarmlevel");

                    b.Property<int>("Alarmstate")
                        .HasColumnType("INTEGER")
                        .HasColumnName("alarmstate");

                    b.Property<string>("Confirmname")
                        .HasColumnType("TEXT")
                        .HasColumnName("confirmname");

                    b.Property<string>("Confirmremark")
                        .HasColumnType("TEXT")
                        .HasColumnName("confirmremark");

                    b.Property<DateTime>("Confirmtime")
                        .HasColumnType("TEXT")
                        .HasColumnName("confirmtime");

                    b.Property<int>("EquipNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("equip_no");

                    b.Property<string>("Event")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("event");

                    b.Property<string>("Guid")
                        .HasColumnType("TEXT")
                        .HasColumnName("guid");

                    b.Property<string>("ProcRec")
                        .HasColumnType("TEXT")
                        .HasColumnName("proc_rec");

                    b.Property<int>("Snapshotlevel")
                        .HasColumnType("INTEGER")
                        .HasColumnName("snapshotlevel");

                    b.Property<int>("StaN")
                        .HasColumnType("INTEGER")
                        .HasColumnName("sta_n");

                    b.Property<DateTime>("Time")
                        .HasColumnType("TEXT")
                        .HasColumnName("time");

                    b.Property<bool>("WuBao")
                        .HasColumnType("INTEGER")
                        .HasColumnName("wubao");

                    b.Property<int>("YcyxNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("ycyx_no");

                    b.Property<string>("YcyxType")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("ycyx_type");

                    b.HasKey("Id");

                    b.ToTable("ycyxevt");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.Ycp", b =>
                {
                    b.Property<int>("EquipNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("equip_no");

                    b.Property<int>("YcNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("yc_no");

                    b.Property<int>("AlarmAcceptableTime")
                        .HasColumnType("INTEGER")
                        .HasColumnName("alarm_acceptable_time");

                    b.Property<int>("AlarmRepeatTime")
                        .HasColumnType("INTEGER")
                        .HasColumnName("alarm_repeat_time");

                    b.Property<int?>("AlarmRiseCycle")
                        .HasColumnType("INTEGER")
                        .HasColumnName("alarmrisecycle");

                    b.Property<int>("AlarmScheme")
                        .HasColumnType("INTEGER")
                        .HasColumnName("alarm_scheme");

                    b.Property<string>("AlarmShield")
                        .HasColumnType("TEXT")
                        .HasColumnName("alarm_shield");

                    b.Property<double?>("CurveLimit")
                        .HasColumnType("REAL")
                        .HasColumnName("curve_limit");

                    b.Property<bool>("CurveRcd")
                        .HasColumnType("INTEGER")
                        .HasColumnName("curve_rcd");

                    b.Property<string>("DataType")
                        .HasColumnType("TEXT")
                        .HasColumnName("datatype");

                    b.Property<DateTime?>("GWTime")
                        .HasColumnType("TEXT")
                        .HasColumnName("gwtime");

                    b.Property<string>("GWValue")
                        .HasColumnType("TEXT")
                        .HasColumnName("gwvalue");

                    b.Property<int>("LvlLevel")
                        .HasColumnType("INTEGER")
                        .HasColumnName("lvl_level");

                    b.Property<string>("MainInstruction")
                        .HasColumnType("TEXT")
                        .HasColumnName("main_instruction");

                    b.Property<bool?>("Mapping")
                        .HasColumnType("INTEGER")
                        .HasColumnName("mapping");

                    b.Property<string>("MinorInstruction")
                        .HasColumnType("TEXT")
                        .HasColumnName("minor_instruction");

                    b.Property<string>("OutmaxEvt")
                        .HasColumnType("TEXT")
                        .HasColumnName("outmax_evt");

                    b.Property<string>("OutminEvt")
                        .HasColumnType("TEXT")
                        .HasColumnName("outmin_evt");

                    b.Property<double>("PhysicMax")
                        .HasColumnType("REAL")
                        .HasColumnName("physic_max");

                    b.Property<double>("PhysicMin")
                        .HasColumnType("REAL")
                        .HasColumnName("physic_min");

                    b.Property<string>("PlanNo")
                        .HasColumnType("TEXT")
                        .HasColumnName("planno");

                    b.Property<string>("ProcAdvice")
                        .HasColumnType("TEXT")
                        .HasColumnName("proc_advice");

                    b.Property<string>("RelatedPic")
                        .HasColumnType("TEXT")
                        .HasColumnName("related_pic");

                    b.Property<string>("RelatedVideo")
                        .HasColumnType("TEXT")
                        .HasColumnName("related_video");

                    b.Property<string>("Reserve1")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve3");

                    b.Property<int>("RestoreAcceptableTime")
                        .HasColumnType("INTEGER")
                        .HasColumnName("restore_acceptable_time");

                    b.Property<double>("RestoreMax")
                        .HasColumnType("REAL")
                        .HasColumnName("restore_max");

                    b.Property<double>("RestoreMin")
                        .HasColumnType("REAL")
                        .HasColumnName("restore_min");

                    b.Property<DateTime?>("SafeBgn")
                        .HasColumnType("TEXT")
                        .HasColumnName("safe_bgn");

                    b.Property<DateTime?>("SafeEnd")
                        .HasColumnType("TEXT")
                        .HasColumnName("safe_end");

                    b.Property<string>("SafeTime")
                        .HasColumnType("TEXT")
                        .HasColumnName("safetime");

                    b.Property<int>("StaN")
                        .HasColumnType("INTEGER")
                        .HasColumnName("sta_n");

                    b.Property<string>("Unit")
                        .HasColumnType("TEXT")
                        .HasColumnName("unit");

                    b.Property<double>("ValMax")
                        .HasColumnType("REAL")
                        .HasColumnName("val_max");

                    b.Property<double>("ValMin")
                        .HasColumnType("REAL")
                        .HasColumnName("val_min");

                    b.Property<int>("ValTrait")
                        .HasColumnType("INTEGER")
                        .HasColumnName("val_trait");

                    b.Property<string>("WaveFile")
                        .HasColumnType("TEXT")
                        .HasColumnName("wave_file");

                    b.Property<string>("YcCode")
                        .HasColumnType("TEXT")
                        .HasColumnName("yc_code");

                    b.Property<double>("YcMax")
                        .HasColumnType("REAL")
                        .HasColumnName("yc_max");

                    b.Property<double>("YcMin")
                        .HasColumnType("REAL")
                        .HasColumnName("yc_min");

                    b.Property<string>("YcNm")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("yc_nm");

                    b.Property<string>("ZiChanId")
                        .HasColumnType("TEXT")
                        .HasColumnName("zichanid");

                    b.HasKey("EquipNo", "YcNo");

                    b.ToTable("ycp");
                });

            modelBuilder.Entity("IoTCenter.Data.Model.Yxp", b =>
                {
                    b.Property<int>("EquipNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("equip_no");

                    b.Property<int>("YxNo")
                        .HasColumnType("INTEGER")
                        .HasColumnName("yx_no");

                    b.Property<int>("AlarmAcceptableTime")
                        .HasColumnType("INTEGER")
                        .HasColumnName("alarm_acceptable_time");

                    b.Property<int>("AlarmRepeatTime")
                        .HasColumnType("INTEGER")
                        .HasColumnName("alarm_repeat_time");

                    b.Property<int?>("AlarmRiseCycle")
                        .HasColumnType("INTEGER")
                        .HasColumnName("alarmrisecycle");

                    b.Property<int>("AlarmScheme")
                        .HasColumnType("INTEGER")
                        .HasColumnName("alarm_scheme");

                    b.Property<string>("AlarmShield")
                        .HasColumnType("TEXT")
                        .HasColumnName("alarm_shield");

                    b.Property<bool>("CurveRcd")
                        .HasColumnType("INTEGER")
                        .HasColumnName("curve_rcd");

                    b.Property<string>("DataType")
                        .HasColumnType("TEXT")
                        .HasColumnName("datatype");

                    b.Property<string>("Evt01")
                        .HasColumnType("TEXT")
                        .HasColumnName("evt_01");

                    b.Property<string>("Evt10")
                        .HasColumnType("TEXT")
                        .HasColumnName("evt_10");

                    b.Property<DateTime?>("GWTime")
                        .HasColumnType("TEXT")
                        .HasColumnName("gwtime");

                    b.Property<string>("GWValue")
                        .HasColumnType("TEXT")
                        .HasColumnName("gwvalue");

                    b.Property<int>("Initval")
                        .HasColumnType("INTEGER")
                        .HasColumnName("initval");

                    b.Property<bool>("Inversion")
                        .HasColumnType("INTEGER")
                        .HasColumnName("inversion");

                    b.Property<int>("LevelD")
                        .HasColumnType("INTEGER")
                        .HasColumnName("level_d");

                    b.Property<int>("LevelR")
                        .HasColumnType("INTEGER")
                        .HasColumnName("level_r");

                    b.Property<string>("MainInstruction")
                        .HasColumnType("TEXT")
                        .HasColumnName("main_instruction");

                    b.Property<string>("MinorInstruction")
                        .HasColumnType("TEXT")
                        .HasColumnName("minor_instruction");

                    b.Property<string>("PlanNo")
                        .HasColumnType("TEXT")
                        .HasColumnName("planno");

                    b.Property<string>("ProcAdviceD")
                        .HasColumnType("TEXT")
                        .HasColumnName("proc_advice_d");

                    b.Property<string>("ProcAdviceR")
                        .HasColumnType("TEXT")
                        .HasColumnName("proc_advice_r");

                    b.Property<string>("RelatedPic")
                        .HasColumnType("TEXT")
                        .HasColumnName("related_pic");

                    b.Property<string>("RelatedVideo")
                        .HasColumnType("TEXT")
                        .HasColumnName("related_video");

                    b.Property<string>("Reserve1")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve1");

                    b.Property<string>("Reserve2")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve2");

                    b.Property<string>("Reserve3")
                        .HasColumnType("TEXT")
                        .HasColumnName("reserve3");

                    b.Property<int>("RestoreAcceptableTime")
                        .HasColumnType("INTEGER")
                        .HasColumnName("restore_acceptable_time");

                    b.Property<DateTime?>("SafeBgn")
                        .HasColumnType("TEXT")
                        .HasColumnName("safe_bgn");

                    b.Property<DateTime?>("SafeEnd")
                        .HasColumnType("TEXT")
                        .HasColumnName("safe_end");

                    b.Property<string>("SafeTime")
                        .HasColumnType("TEXT")
                        .HasColumnName("safetime");

                    b.Property<int>("StaN")
                        .HasColumnType("INTEGER")
                        .HasColumnName("sta_n");

                    b.Property<int>("ValTrait")
                        .HasColumnType("INTEGER")
                        .HasColumnName("val_trait");

                    b.Property<string>("WaveFile")
                        .HasColumnType("TEXT")
                        .HasColumnName("wave_file");

                    b.Property<string>("YxCode")
                        .HasColumnType("TEXT")
                        .HasColumnName("yx_code");

                    b.Property<string>("YxNm")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("yx_nm");

                    b.Property<string>("ZiChanId")
                        .HasColumnType("TEXT")
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
