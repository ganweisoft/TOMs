/*
Navicat MySQL Data Transfer

Source Server         : localhost_3306
Source Server Version : 50730
Source Host           : localhost:3306
Source Database       : 6.0.2

Target Server Type    : MYSQL
Target Server Version : 50730
File Encoding         : 65001

Date: 2023-05-17 09:09:36
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for administrator
-- ----------------------------
DROP TABLE IF EXISTS `administrator`;
CREATE TABLE `administrator` (
  `Administrator` varchar(32) NOT NULL,
  `Telphone` text,
  `MobileTel` text,
  `EMail` text,
  `AckLevel` int(11) NOT NULL,
  `Reserve1` text,
  `Reserve2` text,
  `Reserve3` text,
  `photoImage` varchar(255) DEFAULT NULL,
  `UserName` varchar(32) DEFAULT NULL,
  PRIMARY KEY (`Administrator`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of administrator
-- ----------------------------

-- ----------------------------
-- Table structure for alarmproc
-- ----------------------------
DROP TABLE IF EXISTS `alarmproc`;
CREATE TABLE `alarmproc` (
  `Proc_Code` int(11) NOT NULL AUTO_INCREMENT,
  `Proc_Module` varchar(32) DEFAULT NULL,
  `Proc_name` varchar(48) DEFAULT NULL,
  `Proc_parm` varchar(200) DEFAULT NULL,
  `Comment` varchar(240) DEFAULT NULL,
  PRIMARY KEY (`Proc_Code`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of alarmproc
-- ----------------------------

-- ----------------------------
-- Table structure for alarmrec
-- ----------------------------
DROP TABLE IF EXISTS `alarmrec`;
CREATE TABLE `alarmrec` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `proc_name` varchar(48) DEFAULT NULL,
  `Administrator` varchar(32) NOT NULL,
  `event` varchar(128) NOT NULL,
  `time` datetime NOT NULL,
  `comment` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of alarmrec
-- ----------------------------

-- ----------------------------
-- Table structure for almreport
-- ----------------------------
DROP TABLE IF EXISTS `almreport`;
CREATE TABLE `almreport` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `sta_n` int(11) DEFAULT NULL,
  `group_no` int(11) DEFAULT NULL,
  `Administrator` varchar(32) DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of almreport
-- ----------------------------

-- ----------------------------
-- Table structure for autoproc
-- ----------------------------
DROP TABLE IF EXISTS `autoproc`;
CREATE TABLE `autoproc` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `iequip_no` int(11) NOT NULL,
  `iycyx_no` int(11) NOT NULL,
  `iycyx_type` varchar(16) DEFAULT NULL,
  `delay` int(11) NOT NULL,
  `oequip_no` int(11) NOT NULL,
  `oset_no` int(11) NOT NULL,
  `value` varchar(255) DEFAULT NULL,
  `ProcDesc` varchar(255) DEFAULT NULL,
  `Enable` int(11) NOT NULL,
  `Reserve` text,
  PRIMARY KEY (`ID`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of autoproc
-- ----------------------------


-- ----------------------------
-- Table structure for gwdatarecorditems
-- ----------------------------
DROP TABLE IF EXISTS `gwdatarecorditems`;
CREATE TABLE `gwdatarecorditems` (
  `equip_no` int(11) NOT NULL,
  `data_type` varchar(1) NOT NULL,
  `ycyx_no` int(11) NOT NULL,
  `data_name` varchar(255) DEFAULT NULL,
  `userName` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`equip_no`,`data_type`,`ycyx_no`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of gwdatarecorditems
-- ----------------------------

-- ----------------------------
-- Table structure for gwdelayaction
-- ----------------------------
DROP TABLE IF EXISTS `gwdelayaction`;
CREATE TABLE `gwdelayaction` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `GW_Sta_n` int(11) DEFAULT NULL,
  `GW_Equip_no` int(11) DEFAULT NULL,
  `GW_Set_no` int(11) DEFAULT NULL,
  `GW_Value` varchar(255) DEFAULT NULL,
  `GW_AddDateTime` datetime DEFAULT NULL,
  `GW_UserNm` varchar(255) DEFAULT NULL,
  `GW_DelayNum` int(11) DEFAULT NULL,
  `GW_State` int(11) DEFAULT NULL,
  `GW_Source` int(11) DEFAULT NULL,
  `Reserve1` text,
  `Reserve2` text,
  `Reserve3` text,
  PRIMARY KEY (`ID`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of gwdelayaction
-- ----------------------------

-- ----------------------------
-- Table structure for gwexproc
-- ----------------------------
DROP TABLE IF EXISTS `gwexproc`;
CREATE TABLE `gwexproc` (
  `Proc_Code` int(11) NOT NULL AUTO_INCREMENT,
  `Proc_Module` varchar(255) DEFAULT NULL,
  `Proc_name` varchar(255) DEFAULT NULL,
  `Proc_parm` varchar(255) DEFAULT NULL,
  `Comment` text,
  PRIMARY KEY (`Proc_Code`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of gwexproc
-- ----------------------------

-- ----------------------------
-- Table structure for gwexproccmd
-- ----------------------------
DROP TABLE IF EXISTS `gwexproccmd`;
CREATE TABLE `gwexproccmd` (
  `proc_code` int(11) NOT NULL,
  `cmd_nm` varchar(255) NOT NULL,
  `main_instruction` varchar(255) DEFAULT NULL,
  `minor_instruction` varchar(255) DEFAULT NULL,
  `value` varchar(255) DEFAULT NULL,
  `record` int(11) NOT NULL DEFAULT '1',
  PRIMARY KEY (`proc_code`,`cmd_nm`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC; 


-- ----------------------------
-- Table structure for gwproccycleprocesstime
-- ----------------------------
DROP TABLE IF EXISTS `gwproccycleprocesstime`;
CREATE TABLE `gwproccycleprocesstime` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `TableId` int(11) DEFAULT NULL,
  `Time` int(11) DEFAULT NULL,
  `ProcessOrder` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of gwproccycleprocesstime
-- ----------------------------

-- ----------------------------
-- Table structure for gwproccycletable
-- ----------------------------
DROP TABLE IF EXISTS `gwproccycletable`;
CREATE TABLE `gwproccycletable` (
  `TableID` int(11) NOT NULL,
  `DoOrder` int(11) NOT NULL,
  `Type` varchar(255) DEFAULT NULL,
  `equip_no` int(11) NOT NULL,
  `set_no` int(11) NOT NULL,
  `value` varchar(255) DEFAULT NULL,
  `proc_code` int(11) NOT NULL,
  `cmd_nm` varchar(255) DEFAULT NULL,
  `SleepTime` int(11) NOT NULL,
  `SleepUnit` varchar(255) DEFAULT NULL,
  `Reserve1` text,
  `Reserve2` text,
  `Reserve3` text,
  `ProcessOrder` varchar(255) DEFAULT NULL,
  `equipSetNm` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`TableID`,`DoOrder`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of gwproccycletable
-- ----------------------------

-- ----------------------------
-- Table structure for gwproccycletlist
-- ----------------------------
DROP TABLE IF EXISTS `gwproccycletlist`;
CREATE TABLE `gwproccycletlist` (
  `TableID` int(11) NOT NULL AUTO_INCREMENT,
  `TableName` varchar(255) NOT NULL,
  `BeginTime` datetime NOT NULL,
  `EndTime` datetime NOT NULL,
  `ZhenDianDo` int(11) DEFAULT NULL,
  `ZhidingDo` int(11) DEFAULT NULL,
  `CycleMustFinish` int(11) DEFAULT NULL,
  `ZhidingTime` datetime DEFAULT NULL,
  `MaxCycleNum` int(11) DEFAULT NULL,
  `Reserve1` text,
  `Reserve2` text,
  `Reserve3` text,
  `Reference` text,
  PRIMARY KEY (`TableID`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of gwproccycletlist
-- ----------------------------

-- ----------------------------
-- Table structure for gwprocprocess
-- ----------------------------
DROP TABLE IF EXISTS `gwprocprocess`;
CREATE TABLE `gwprocprocess` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `TableId` int(11) DEFAULT NULL,
  `TaskType` varchar(255) DEFAULT NULL,
  `From` varchar(255) DEFAULT NULL,
  `To` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of gwprocprocess
-- ----------------------------

-- ----------------------------
-- Table structure for gwprocspectable
-- ----------------------------
DROP TABLE IF EXISTS `gwprocspectable`;
CREATE TABLE `gwprocspectable` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `DateName` varchar(255) DEFAULT NULL,
  `BeginDate` datetime NOT NULL,
  `EndDate` datetime NOT NULL,
  `TableID` text,
  `Color` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`ID`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of gwprocspectable
-- ----------------------------

-- ----------------------------
-- Table structure for gwproctaskprocesstime
-- ----------------------------
DROP TABLE IF EXISTS `gwproctaskprocesstime`;
CREATE TABLE `gwproctaskprocesstime` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `TableId` int(11) DEFAULT NULL,
  `ControlType` varchar(255) DEFAULT NULL,
  `TimeType` varchar(255) DEFAULT NULL,
  `Time` datetime DEFAULT NULL,
  `ProcessOrder` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of gwproctaskprocesstime
-- ----------------------------

-- ----------------------------
-- Table structure for gwproctimeeqptable
-- ----------------------------
DROP TABLE IF EXISTS `gwproctimeeqptable`;
CREATE TABLE `gwproctimeeqptable` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `TableID` int(11) DEFAULT NULL,
  `Time` datetime NOT NULL,
  `TimeDur` datetime NOT NULL,
  `equip_no` int(11) NOT NULL,
  `set_no` int(11) NOT NULL,
  `value` varchar(64) DEFAULT NULL,
  `processOrder` varchar(255) DEFAULT NULL,
  `equipSetNm` varchar(255) DEFAULT NULL,
  `reserve1` text,
  `reserve2` text,
  `reserve3` text,
  PRIMARY KEY (`ID`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of gwproctimeeqptable
-- ----------------------------

-- ----------------------------
-- Table structure for gwproctimesystable
-- ----------------------------
DROP TABLE IF EXISTS `gwproctimesystable`;
CREATE TABLE `gwproctimesystable` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `TableID` int(11) NOT NULL,
  `Time` datetime NOT NULL,
  `TimeDur` datetime NOT NULL,
  `proc_code` int(11) NOT NULL,
  `cmd_nm` varchar(255) DEFAULT NULL,
  `processOrder` varchar(255) DEFAULT NULL,
  `reserve1` text,
  `reserve2` text,
  `reserve3` text,
  PRIMARY KEY (`ID`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of gwproctimesystable
-- ----------------------------

-- ----------------------------
-- Table structure for gwproctimetlist
-- ----------------------------
DROP TABLE IF EXISTS `gwproctimetlist`;
CREATE TABLE `gwproctimetlist` (
  `TableID` int(11) NOT NULL AUTO_INCREMENT,
  `TableName` varchar(255) NOT NULL,
  `Comment` varchar(255) DEFAULT NULL,
  `Reference` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`TableID`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of gwproctimetlist
-- ----------------------------

-- ----------------------------
-- Table structure for gwprocweektable
-- ----------------------------
DROP TABLE IF EXISTS `gwprocweektable`;
CREATE TABLE `gwprocweektable` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `Mon` text,
  `Tues` text,
  `Wed` text,
  `Thurs` text,
  `Fri` text,
  `Sat` text,
  `Sun` text,
  PRIMARY KEY (`ID`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of gwprocweektable
-- ----------------------------


-- ----------------------------
-- Table structure for spealmreport
-- ----------------------------
DROP TABLE IF EXISTS `spealmreport`;
CREATE TABLE `spealmreport` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `sta_n` int(11) DEFAULT NULL,
  `group_no` int(11) DEFAULT NULL,
  `Administrator` varchar(32) DEFAULT NULL,
  `begin_time` datetime DEFAULT NULL,
  `end_time` datetime DEFAULT NULL,
  `remark` text,
  `Color` varchar(255) DEFAULT NULL,
  `name` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of spealmreport
-- ----------------------------

-- ----------------------------
-- Table structure for weekalmreport
-- ----------------------------
DROP TABLE IF EXISTS `weekalmreport`;
CREATE TABLE `weekalmreport` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `sta_n` int(11) DEFAULT NULL,
  `group_no` int(11) DEFAULT NULL,
  `Administrator` varchar(32) DEFAULT NULL,
  `week_day` int(11) DEFAULT NULL,
  `begin_time` datetime DEFAULT NULL,
  `end_time` datetime DEFAULT NULL,
  `remark` text,
  `Color` varchar(255) DEFAULT NULL,
  `name` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of weekalmreport
-- ----------------------------

-- ----------------------------
-- Table structure for egroup
-- ----------------------------
DROP TABLE IF EXISTS `egroup`;
CREATE TABLE `egroup` (
  `GroupId` int(11) NOT NULL AUTO_INCREMENT,
  `GroupName` varchar(255) DEFAULT NULL,
  `ParentGroupId` int(11) DEFAULT NULL,
  PRIMARY KEY (`GroupId`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf16 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of egroup
-- ----------------------------
INSERT INTO `egroup` VALUES ('1', '综合管理平台', '0');

-- ----------------------------
-- Table structure for egrouplist
-- ----------------------------
DROP TABLE IF EXISTS `egrouplist`;
CREATE TABLE `egrouplist` (
  `EGroupListId` int(11) NOT NULL AUTO_INCREMENT,
  `GroupId` int(11) DEFAULT NULL,
  `EquipNo` int(11) DEFAULT NULL,
  `StaNo` int(11) DEFAULT NULL,
  PRIMARY KEY (`EGroupListId`) USING BTREE,
  KEY `egroup_pk` (`GroupId`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=31 DEFAULT CHARSET=utf16 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of egrouplist
-- ----------------------------
INSERT INTO `egrouplist` VALUES ('1', '1', '100', '1');

-- ----------------------------
-- Table structure for equip
-- ----------------------------
DROP TABLE IF EXISTS `equip`;
CREATE TABLE `equip` (
  `sta_n` int(11) NOT NULL,
  `equip_no` int(11) NOT NULL AUTO_INCREMENT,
  `equip_nm` varchar(64) NOT NULL,
  `equip_detail` varchar(255) DEFAULT NULL,
  `acc_cyc` int(11) DEFAULT NULL,
  `related_pic` varchar(255) DEFAULT NULL,
  `proc_advice` varchar(254) DEFAULT NULL,
  `out_of_contact` varchar(64) NOT NULL,
  `contacted` varchar(64) DEFAULT NULL,
  `event_wav` varchar(64) DEFAULT NULL,
  `communication_drv` varchar(128) DEFAULT NULL,
  `local_addr` varchar(64) DEFAULT NULL,
  `equip_addr` varchar(128) DEFAULT NULL,
  `communication_param` longtext,
  `communication_time_param` varchar(32) DEFAULT NULL,
  `raw_equip_no` int(11) NOT NULL,
  `tabname` varchar(15) DEFAULT NULL,
  `alarm_scheme` int(11) NOT NULL,
  `attrib` int(11) NOT NULL,
  `sta_IP` varchar(255) DEFAULT NULL,
  `AlarmRiseCycle` int(11) DEFAULT NULL,
  `Reserve1` longtext,
  `Reserve2` longtext,
  `Reserve3` longtext,
  `related_video` varchar(255) DEFAULT NULL,
  `ZiChanID` varchar(255) DEFAULT NULL,
  `PlanNo` varchar(255) DEFAULT NULL,
  `SafeTime` longtext,
  `backup` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`equip_no`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=130 DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of equip
-- ----------------------------

-- ----------------------------
-- Table structure for equipgroup
-- ----------------------------
DROP TABLE IF EXISTS `equipgroup`;
CREATE TABLE `equipgroup` (
  `sta_n` int(11) DEFAULT NULL,
  `group_no` int(11) unsigned NOT NULL,
  `group_name` varchar(50) NOT NULL,
  `equipcomb` text,
  PRIMARY KEY (`group_no`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of equipgroup
-- ----------------------------

-- ----------------------------
-- Table structure for gwrole
-- ----------------------------
DROP TABLE IF EXISTS `gwrole`;
CREATE TABLE `gwrole` (
  `Name` varchar(128) NOT NULL,
  `ControlEquips` text,
  `ControlEquips_Unit` text,
  `BrowseEquips` text,
  `BrowsePages` text,
  `remark` text,
  `SpecialBrowseEquip` text,
  `SystemModule` text,
  PRIMARY KEY (`Name`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of gwrole
-- ----------------------------

-- ----------------------------
-- Table structure for gwsnapshotconfig
-- ----------------------------
DROP TABLE IF EXISTS `gwsnapshotconfig`;
CREATE TABLE `gwsnapshotconfig` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `SnapshotName` varchar(128) DEFAULT NULL,
  `SnapshotLevelMin` int(11) DEFAULT NULL,
  `SnapshotLevelMax` int(11) DEFAULT NULL,
  `MaxCount` int(11) DEFAULT NULL,
  `IsShow` int(11) DEFAULT NULL,
  `IconRes` varchar(255) DEFAULT NULL,
  `Reserve1` text,
  `Reserve2` text,
  `Reserve3` text,
  PRIMARY KEY (`ID`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of gwsnapshotconfig
-- ----------------------------
INSERT INTO `gwsnapshotconfig` VALUES ('1', '故障', '10003', '10005', '-1', '1', 'Errors.png', null, null, null);
INSERT INTO `gwsnapshotconfig` VALUES ('2', '警告', '2', '9', '-1', '1', 'Warnings.png', null, null, null);
INSERT INTO `gwsnapshotconfig` VALUES ('3', '信息', '0', '1', '-1', '1', 'Informations.png', null, null, null);
INSERT INTO `gwsnapshotconfig` VALUES ('4', '设置', '10001', '10001', '-1', '1', 'Settings.png', null, null, null);
INSERT INTO `gwsnapshotconfig` VALUES ('5', '资产', '10002', '10002', '-1', '1', 'Assets.png', null, null, null);

-- ----------------------------
-- Table structure for gwuser
-- ----------------------------
DROP TABLE IF EXISTS `gwuser`;
CREATE TABLE `gwuser` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(128) DEFAULT NULL,
  `Password` text,
  `Roles` text,
  `HomePages` text,
  `AutoInspectionPages` text,
  `Remark` text,
  `ControlLevel` text,
  `Reserve1` varchar(255) DEFAULT NULL,
  `Reserve2` varchar(255) DEFAULT NULL,
  `Reserve3` varchar(255) DEFAULT NULL,
  `PwdUpdateTime` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `FirstLogin` bit(1) DEFAULT NULL,
  `HistoryPasswords` longtext,
  `AccessFailedCount` int(11) DEFAULT NULL,
  `LockoutEnabled` bit(1) DEFAULT NULL,
  `LockoutEnd` datetime DEFAULT NULL,
  `SecurityStamp` longtext,
  `UseExpiredTime` datetime DEFAULT NULL,
  PRIMARY KEY (`ID`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

INSERT INTO `GWUser` (
  `ID`, `Name`, `Password`, `Roles`, `HomePages`, `AutoInspectionPages`, `Remark`, 
  `ControlLevel`, `Reserve1`, `Reserve2`, `Reserve3`, `PwdUpdateTime`, 
  `FirstLogin`, `HistoryPasswords`, `AccessFailedCount`, `LockoutEnabled`, 
  `LockoutEnd`, `SecurityStamp`, `UseExpiredTime`
) VALUES (
  1, '/sLVCSQAae4hS2T6yxohPA==', 'czfMnBnCRjTkBTa8btKgKQ==', '6p4c1NnjPj8BJAHf5sWMnA==',
  '', '', '', 'iIMniI0QKZ6XZW3dwC+eeA==', '', '', '',
  '2025-03-21 08:31:25', 1, '', 0, 0, NULL, '', NULL
);

-- ----------------------------
-- Table structure for gwzichanrecord
-- ----------------------------
DROP TABLE IF EXISTS `gwzichanrecord`;
CREATE TABLE `gwzichanrecord` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `ZiChanID` varchar(255) DEFAULT NULL,
  `WeiHuDate` datetime DEFAULT NULL,
  `WeiHuName` varchar(255) DEFAULT NULL,
  `WeiHuRecord` text,
  `ItemAddMan` varchar(255) DEFAULT NULL,
  `ItemAddDate` datetime DEFAULT NULL,
  `Pictures` text,
  `Reserve1` text,
  `Reserve2` text,
  `Reserve3` text,
  PRIMARY KEY (`ID`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of gwzichanrecord
-- ----------------------------

-- ----------------------------
-- Table structure for gwzichantable
-- ----------------------------
DROP TABLE IF EXISTS `gwzichantable`;
CREATE TABLE `gwzichantable` (
  `ZiChanID` varchar(255) NOT NULL,
  `ZiChanName` varchar(255) DEFAULT NULL,
  `ZiChanType` varchar(255) DEFAULT NULL,
  `ZiChanImage` varchar(255) DEFAULT NULL,
  `ChangJia` varchar(255) DEFAULT NULL,
  `LianxiRen` varchar(255) DEFAULT NULL,
  `LianxiTel` varchar(255) DEFAULT NULL,
  `LianxiMail` varchar(255) DEFAULT NULL,
  `GouMaiDate` datetime DEFAULT NULL,
  `ZiChanSite` varchar(255) DEFAULT NULL,
  `WeiHuDate` datetime DEFAULT NULL,
  `WeiHuCycle` int(11) DEFAULT NULL,
  `BaoXiuQiXian` datetime DEFAULT NULL,
  `LastEditMan` varchar(255) DEFAULT NULL,
  `LastEditDate` datetime DEFAULT NULL,
  `related_pic` varchar(255) DEFAULT NULL,
  `Reserve1` text,
  `Reserve2` text,
  `Reserve3` text,
  PRIMARY KEY (`ZiChanID`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of gwzichantable
-- ----------------------------

-- ----------------------------
-- Table structure for iotaccountpasswordrule
-- ----------------------------
DROP TABLE IF EXISTS `iotaccountpasswordrule`;
CREATE TABLE `iotaccountpasswordrule` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `json` longtext NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for iotdevice
-- ----------------------------
DROP TABLE IF EXISTS `iotdevice`;
CREATE TABLE `iotdevice` (
  `deviceId` varchar(50) NOT NULL,
  `gatewayId` varchar(50) DEFAULT NULL,
  `createTime` datetime DEFAULT NULL,
  `lastModifiedTime` datetime DEFAULT NULL,
  `equipNo` int(11) NOT NULL,
  `deviceType` varchar(255) DEFAULT NULL,
  `deviceModel` varchar(255) DEFAULT NULL,
  `swVersion` varchar(255) DEFAULT NULL,
  `fwVersion` varchar(255) DEFAULT NULL,
  `hwVersion` varchar(255) DEFAULT NULL,
  `protocolType` varchar(255) DEFAULT NULL,
  `sigVersion` varchar(255) DEFAULT NULL,
  `serialNumber` varchar(255) DEFAULT NULL,
  `longitude` varchar(50) DEFAULT NULL,
  `latitude` varchar(50) DEFAULT NULL,
  `height` varchar(50) DEFAULT NULL,
  `mac` varchar(255) DEFAULT NULL,
  `manufacturerName` varchar(255) DEFAULT NULL,
  `areaName` varchar(255) DEFAULT NULL,
  `buildName` varchar(255) DEFAULT NULL,
  `unitName` varchar(255) DEFAULT NULL,
  `systemName` varchar(255) DEFAULT NULL,
  `location` varchar(2048) DEFAULT NULL,
  `description` varchar(2048) DEFAULT NULL,
  `VideoParam` varchar(255) DEFAULT NULL,
  `SceneParam` varchar(255) DEFAULT NULL,
  `OtherData` text,
  PRIMARY KEY (`deviceId`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of iotdevice
-- ----------------------------

-- ----------------------------
-- Table structure for iotequip
-- ----------------------------
DROP TABLE IF EXISTS `iotequip`;
CREATE TABLE `iotequip` (
  `sta_n` int(11) DEFAULT NULL,
  `equip_no` int(11) NOT NULL,
  `equip_nm` varchar(64) NOT NULL,
  `equip_detail` varchar(255) DEFAULT NULL,
  `acc_cyc` int(11) DEFAULT NULL,
  `related_pic` varchar(255) DEFAULT NULL,
  `proc_advice` varchar(255) DEFAULT NULL,
  `out_of_contact` varchar(64) NOT NULL,
  `contacted` varchar(64) DEFAULT NULL,
  `event_wav` varchar(64) DEFAULT NULL,
  `communication_drv` varchar(128) DEFAULT NULL,
  `local_addr` varchar(64) DEFAULT NULL,
  `equip_addr` varchar(128) DEFAULT NULL,
  `communication_param` text,
  `communication_time_param` varchar(32) DEFAULT NULL,
  `raw_equip_no` int(11) DEFAULT NULL,
  `tabname` varchar(15) DEFAULT NULL,
  `alarm_scheme` int(11) DEFAULT NULL,
  `attrib` int(11) DEFAULT NULL,
  `sta_IP` varchar(255) DEFAULT NULL,
  `AlarmRiseCycle` int(11) DEFAULT NULL,
  `Reserve1` text,
  `Reserve2` text,
  `Reserve3` text,
  `related_video` varchar(254) DEFAULT NULL,
  `ZiChanID` varchar(255) DEFAULT NULL,
  `PlanNo` varchar(255) DEFAULT NULL,
  `SafeTime` text,
  `backup` varchar(255) DEFAULT NULL,
  `equipconntype` int(40) DEFAULT NULL,
  `ThingModelJson` text,
  `ResourceSpaceId` int(11) DEFAULT NULL,
  `AppId` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`equip_no`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of iotequip
-- ----------------------------
INSERT INTO `iotequip` VALUES ('1', '2', '温湿度', 'TCP', '1', '', '', '设备故障', '', '', 'BCDataSimu.STD.dll', '4ec839503b7f4ec79ef6383f808e3c24', '1fd57489523f4e6193017261675d5451', '', '1000/6/16/400', '4', '', '0', '0', '', '0', '', '', '', '', '', '', '', '', '0', null, null, null);


-- ----------------------------
-- Table structure for iotsetparm
-- ----------------------------
DROP TABLE IF EXISTS `iotsetparm`;
CREATE TABLE `iotsetparm` (
  `sta_n` int(11) DEFAULT NULL,
  `equip_no` int(11) NOT NULL,
  `set_no` int(11) NOT NULL,
  `set_nm` text,
  `set_type` varchar(1) DEFAULT NULL,
  `main_instruction` varchar(64) DEFAULT NULL,
  `minor_instruction` text,
  `record` int(11) NOT NULL DEFAULT '1',
  `action` varchar(16) DEFAULT NULL,
  `value` text,
  `canexecution` int(11) NOT NULL DEFAULT '1',
  `VoiceKeys` text,
  `EnableVoice` int(11) NOT NULL DEFAULT '0',
  `qr_equip_no` int(11) DEFAULT NULL,
  `Reserve1` text,
  `Reserve2` text,
  `Reserve3` text,
  `set_code` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`equip_no`,`set_no`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of iotsetparm
-- ----------------------------

INSERT INTO `iotsetparm` VALUES ('1', '2', '1', '温度设置', 'V', 'SetYCYXValue', 'C_1', '1', '设置', '1', '0', '', '0', '0', '', '', '', 'SetTemperatureValue');
INSERT INTO `iotsetparm` VALUES ('1', '2', '2', '湿度设置', 'V', 'SetYCYXValue', 'C_2', '1', '设置', '0', '0', '', '0', '0', '', '', '', 'SetHumidnessValue');
INSERT INTO `iotsetparm` VALUES ('1', '2', '3', '遥信报警设置', 'X', 'SetYCYXValue', 'X_1', '1', '设置', '0', '0', '1', '0', '0', '', '', '', 'SetSignalingAlarm');
INSERT INTO `iotsetparm` VALUES ('1', '2', '4', '遥信正常设置', 'X', 'SetYCYXValue', 'X_1', '1', '设置', '1', '0', '1', '0', '0', '', '', '', 'SetSignalingNormal');
INSERT INTO `iotsetparm` VALUES ('1', '2', '5', '模拟产生事件', 'X', 'AddEvent', '我是事件内容', '1', '设置', '1', '0', '1', '0', '0', NULL, NULL, NULL, 'SetAddEvent');
INSERT INTO `iotsetparm` VALUES ('1', '2', '6', '模拟通讯故障', 'X', 'SetCommState', '0', '1', '设置', '1', '0', '1', '0', '0', NULL, NULL, NULL, 'SetCommState');

-- ----------------------------

-- ----------------------------
-- Table structure for iotycp
-- ----------------------------
DROP TABLE IF EXISTS `iotycp`;
CREATE TABLE `iotycp` (
  `sta_n` int(11) DEFAULT NULL,
  `equip_no` int(11) NOT NULL,
  `yc_no` int(11) NOT NULL,
  `yc_nm` varchar(80) DEFAULT NULL,
  `mapping` int(11) NOT NULL DEFAULT '0',
  `yc_min` double DEFAULT NULL,
  `yc_max` double DEFAULT NULL,
  `physic_min` double DEFAULT NULL,
  `physic_max` double DEFAULT NULL,
  `val_min` double DEFAULT NULL,
  `restore_min` double DEFAULT NULL,
  `restore_max` double DEFAULT NULL,
  `val_max` double DEFAULT NULL,
  `val_trait` int(11) DEFAULT NULL,
  `main_instruction` varchar(255) DEFAULT NULL,
  `minor_instruction` varchar(255) DEFAULT NULL,
  `safe_bgn` datetime DEFAULT NULL,
  `safe_end` datetime DEFAULT NULL,
  `alarm_acceptable_time` int(11) DEFAULT NULL,
  `restore_acceptable_time` int(11) DEFAULT NULL,
  `alarm_repeat_time` int(11) DEFAULT NULL,
  `proc_advice` varchar(255) DEFAULT NULL,
  `lvl_level` int(11) NOT NULL,
  `outmin_evt` varchar(64) DEFAULT NULL,
  `outmax_evt` varchar(64) DEFAULT NULL,
  `wave_file` varchar(64) DEFAULT NULL,
  `related_pic` varchar(255) DEFAULT NULL,
  `alarm_scheme` int(11) DEFAULT NULL,
  `curve_rcd` int(11) NOT NULL DEFAULT '0',
  `curve_limit` double DEFAULT NULL,
  `alarm_shield` text,
  `unit` varchar(50) DEFAULT NULL,
  `AlarmRiseCycle` int(11) DEFAULT NULL,
  `Reserve1` text,
  `Reserve2` text,
  `Reserve3` text,
  `related_video` varchar(255) DEFAULT NULL,
  `ZiChanID` varchar(255) DEFAULT NULL,
  `PlanNo` varchar(255) DEFAULT NULL,
  `SafeTime` text,
  `yc_code` varchar(200) DEFAULT NULL,
  `datatype` varchar(40) DEFAULT NULL,
  PRIMARY KEY (`equip_no`,`yc_no`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of iotycp
-- ----------------------------
INSERT INTO `iotycp` VALUES ('1', '2', '1', '温度', '0', '31', '32', '15', '20', '15', '16', '31', '32', '0', '0', '1', null, null, '0', '0', '0', null, '3', '', '', '', '', '0', '0', '0', '', '', '0', '', '', '', '', '', '', '', 'Temperature', null);
INSERT INTO `iotycp` VALUES ('1', '2', '2', '湿度', '0', '80', '90', '20', '30', '15', '16', '80', '90', '0', '0', '1', null, null, '0', '0', '0', null, '3', '', '', '', '', '0', '0', '0', '', '', '0', '', '', '', '', '', '', '', 'Humidness', null);

-- ----------------------------
-- Table structure for iotyxp
-- ----------------------------
DROP TABLE IF EXISTS `iotyxp`;
CREATE TABLE `iotyxp` (
  `sta_n` int(11) DEFAULT NULL,
  `equip_no` int(11) NOT NULL,
  `yx_no` int(11) NOT NULL,
  `yx_nm` varchar(80) DEFAULT NULL,
  `proc_advice_r` varchar(255) DEFAULT NULL,
  `proc_advice_d` varchar(255) DEFAULT NULL,
  `level_r` int(11) NOT NULL,
  `level_d` int(11) NOT NULL,
  `evt_01` varchar(64) DEFAULT NULL,
  `evt_10` varchar(64) DEFAULT NULL,
  `main_instruction` varchar(255) DEFAULT NULL,
  `minor_instruction` varchar(255) DEFAULT NULL,
  `safe_bgn` datetime DEFAULT NULL,
  `safe_end` datetime DEFAULT NULL,
  `alarm_acceptable_time` int(11) DEFAULT NULL,
  `restore_acceptable_time` int(11) DEFAULT NULL,
  `alarm_repeat_time` int(11) DEFAULT NULL,
  `wave_file` varchar(64) DEFAULT NULL,
  `related_pic` varchar(255) DEFAULT NULL,
  `alarm_scheme` int(11) DEFAULT NULL,
  `inversion` int(11) NOT NULL DEFAULT '0',
  `initval` int(11) DEFAULT NULL,
  `val_trait` int(11) DEFAULT NULL,
  `alarm_shield` text,
  `AlarmRiseCycle` int(11) DEFAULT NULL,
  `related_video` varchar(255) DEFAULT NULL,
  `ZiChanID` varchar(255) DEFAULT NULL,
  `PlanNo` varchar(255) DEFAULT NULL,
  `SafeTime` text,
  `curve_rcd` int(11) NOT NULL DEFAULT '0',
  `Reserve1` text,
  `Reserve2` text,
  `Reserve3` text,
  `yx_code` varchar(200) DEFAULT NULL,
  `datatype` varchar(40) DEFAULT NULL,
  PRIMARY KEY (`equip_no`,`yx_no`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

INSERT INTO `iotyxp` (
  `sta_n`, `equip_no`, `yx_no`, `yx_nm`, `proc_advice_r`, `proc_advice_d`, 
  `level_r`, `level_d`, `evt_01`, `evt_10`, `main_instruction`, `minor_instruction`, 
  `safe_bgn`, `safe_end`, `alarm_acceptable_time`, `restore_acceptable_time`, 
  `alarm_repeat_time`, `wave_file`, `related_pic`, `alarm_scheme`, 
  `inversion`, `curve_rcd`, `initval`, `val_trait`, `alarm_shield`, 
  `AlarmRiseCycle`, `Reserve1`, `Reserve2`, `Reserve3`, 
  `related_video`, `ZiChanID`, `PlanNo`, `SafeTime`, `yx_code`, `datatype`
) VALUES (
  1, 2, 1, '遥信', '请处理', '请处理', 
  2, 3, '正常', '报警', '02', '0000.0', 
  '2020-05-13 00:00:00', '2020-05-13 00:00:00', 0, 0, 
  0, 'YX62_1_0.wav/YX62_1_1.wav', '', 3, 
  0, 0, 0, 0, '', 
  0, '', '', '', 
  '', '0', '0', '', 'SignalingStatus', ''
);

-- ----------------------------
-- Records of iotyxp
-- ----------------------------

-- ----------------------------
-- Table structure for setevt
-- ----------------------------
DROP TABLE IF EXISTS `setevt`;
CREATE TABLE `setevt` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `sta_n` int(11) DEFAULT NULL,
  `equip_no` int(11) NOT NULL,
  `set_no` int(11) DEFAULT NULL,
  `GWEvent` varchar(128) NOT NULL,
  `GWTime` datetime NOT NULL,
  `GWOperator` text NOT NULL,
  `GWSource` text,
  `confirmname` varchar(128) DEFAULT NULL,
  `confirmtime` datetime DEFAULT NULL,
  `confirmremark` text,
  `GUID` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`ID`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of setevt
-- ----------------------------

-- ----------------------------
-- Table structure for setparm
-- ----------------------------
DROP TABLE IF EXISTS `setparm`;
CREATE TABLE `setparm` (
  `sta_n` int(11) DEFAULT NULL,
  `equip_no` int(11) NOT NULL,
  `set_no` int(11) NOT NULL,
  `set_nm` text,
  `set_type` varchar(1) DEFAULT NULL,
  `main_instruction` varchar(64) DEFAULT NULL,
  `minor_instruction` text,
  `record` int(11) NOT NULL DEFAULT '1',
  `action` varchar(16) DEFAULT NULL,
  `value` text,
  `canexecution` int(11) NOT NULL DEFAULT '1',
  `VoiceKeys` text,
  `EnableVoice` int(11) NOT NULL DEFAULT '0',
  `qr_equip_no` int(11) DEFAULT NULL,
  `Reserve1` text,
  `Reserve2` text,
  `Reserve3` text,
  `set_code` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`equip_no`,`set_no`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of setparm
-- ----------------------------

-- ----------------------------
-- Table structure for sysevt
-- ----------------------------
DROP TABLE IF EXISTS `sysevt`;
CREATE TABLE `sysevt` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `sta_n` int(11) NOT NULL,
  `event` text NOT NULL,
  `time` datetime NOT NULL ON UPDATE CURRENT_TIMESTAMP,
  `confirmname` varchar(128) DEFAULT NULL,
  `confirmtime` datetime NOT NULL ON UPDATE CURRENT_TIMESTAMP,
  `confirmremark` varchar(255) DEFAULT NULL,
  `GUID` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`ID`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=383 DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of sysevt
-- ----------------------------

-- ----------------------------
-- Table structure for ycp
-- ----------------------------
DROP TABLE IF EXISTS `ycp`;
CREATE TABLE `ycp` (
  `sta_n` int(11) DEFAULT NULL,
  `equip_no` int(11) NOT NULL,
  `yc_no` int(11) NOT NULL,
  `yc_nm` varchar(80) DEFAULT NULL,
  `mapping` int(11) NOT NULL DEFAULT '0',
  `yc_min` double DEFAULT NULL,
  `yc_max` double DEFAULT NULL,
  `physic_min` double DEFAULT NULL,
  `physic_max` double DEFAULT NULL,
  `val_min` double DEFAULT NULL,
  `restore_min` double DEFAULT NULL,
  `restore_max` double DEFAULT NULL,
  `val_max` double DEFAULT NULL,
  `val_trait` int(11) DEFAULT NULL,
  `main_instruction` varchar(255) DEFAULT NULL,
  `minor_instruction` varchar(255) DEFAULT NULL,
  `safe_bgn` datetime DEFAULT NULL,
  `safe_end` datetime DEFAULT NULL,
  `alarm_acceptable_time` int(11) DEFAULT NULL,
  `restore_acceptable_time` int(11) DEFAULT NULL,
  `alarm_repeat_time` int(11) DEFAULT NULL,
  `proc_advice` varchar(255) DEFAULT NULL,
  `lvl_level` int(11) NOT NULL,
  `outmin_evt` varchar(64) DEFAULT NULL,
  `outmax_evt` varchar(64) DEFAULT NULL,
  `wave_file` varchar(64) DEFAULT NULL,
  `related_pic` varchar(255) DEFAULT NULL,
  `alarm_scheme` int(11) DEFAULT NULL,
  `curve_rcd` int(11) NOT NULL DEFAULT '0',
  `curve_limit` double DEFAULT NULL,
  `alarm_shield` text,
  `unit` varchar(50) DEFAULT NULL,
  `AlarmRiseCycle` int(11) DEFAULT NULL,
  `Reserve1` text,
  `Reserve2` text,
  `Reserve3` text,
  `related_video` varchar(255) DEFAULT NULL,
  `ZiChanID` varchar(255) DEFAULT NULL,
  `PlanNo` varchar(255) DEFAULT NULL,
  `SafeTime` text,
  `GWValue` varchar(4000) DEFAULT NULL,
  `GWTime` datetime DEFAULT NULL,
  `datatype` varchar(4000) DEFAULT NULL,
  `yc_code` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`equip_no`,`yc_no`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of ycp
-- ----------------------------

-- ----------------------------
-- Table structure for ycyxevt
-- ----------------------------
DROP TABLE IF EXISTS `ycyxevt`;
CREATE TABLE `ycyxevt` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `sta_n` int(11) DEFAULT NULL,
  `equip_no` int(11) NOT NULL,
  `ycyx_no` int(11) NOT NULL,
  `ycyx_type` varchar(255) NOT NULL,
  `event` text NOT NULL,
  `snapshotlevel` int(11) DEFAULT NULL,
  `time` datetime NOT NULL,
  `proc_rec` varchar(255) DEFAULT NULL,
  `confirmname` varchar(128) DEFAULT NULL,
  `confirmtime` datetime DEFAULT NULL,
  `WuBao` int(11) NOT NULL DEFAULT '0',
  `alarmlevel` int(11) DEFAULT NULL,
  `alarmstate` int(11) DEFAULT NULL,
  `confirmremark` text,
  `GUID` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`ID`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=30 DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of ycyxevt
-- ----------------------------

-- ----------------------------
-- Table structure for yxp
-- ----------------------------
DROP TABLE IF EXISTS `yxp`;
CREATE TABLE `yxp` (
  `sta_n` int(11) DEFAULT NULL,
  `equip_no` int(11) NOT NULL,
  `yx_no` int(11) NOT NULL,
  `yx_nm` varchar(80) DEFAULT NULL,
  `proc_advice_r` varchar(255) DEFAULT NULL,
  `proc_advice_d` varchar(255) DEFAULT NULL,
  `level_r` int(11) NOT NULL,
  `level_d` int(11) NOT NULL,
  `evt_01` varchar(64) DEFAULT NULL,
  `evt_10` varchar(64) DEFAULT NULL,
  `main_instruction` varchar(255) DEFAULT NULL,
  `minor_instruction` varchar(255) DEFAULT NULL,
  `safe_bgn` datetime DEFAULT NULL,
  `safe_end` datetime DEFAULT NULL,
  `alarm_acceptable_time` int(11) DEFAULT NULL,
  `restore_acceptable_time` int(11) DEFAULT NULL,
  `alarm_repeat_time` int(11) DEFAULT NULL,
  `wave_file` varchar(64) DEFAULT NULL,
  `related_pic` varchar(255) DEFAULT NULL,
  `alarm_scheme` int(11) DEFAULT NULL,
  `inversion` int(11) NOT NULL DEFAULT '0',
  `initval` int(11) DEFAULT NULL,
  `val_trait` int(11) DEFAULT NULL,
  `alarm_shield` text,
  `AlarmRiseCycle` int(11) DEFAULT NULL,
  `related_video` varchar(255) DEFAULT NULL,
  `ZiChanID` varchar(255) DEFAULT NULL,
  `PlanNo` varchar(255) DEFAULT NULL,
  `SafeTime` text,
  `curve_rcd` int(11) NOT NULL DEFAULT '0',
  `Reserve1` text,
  `Reserve2` text,
  `Reserve3` text,
  `GWValue` varchar(4000) DEFAULT NULL,
  `GWTime` datetime DEFAULT NULL,
  `datatype` varchar(4000) DEFAULT NULL,
  `yx_code` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`equip_no`,`yx_no`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for conditionautoproc
-- ----------------------------
DROP TABLE IF EXISTS `conditionautoproc`;
CREATE TABLE `conditionautoproc` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `proc_name` text,
  `relate_autoproc` INT,
  `relate_yxNo` INT,
  `delay` int(11) NOT NULL DEFAULT '0',
  `oequip_no` int(11) NOT NULL DEFAULT '0',
  `oset_no` int(11) NOT NULL DEFAULT '0',
  `value` text,
  `proc_desc` text,
  `enable` bit(1) DEFAULT '0',
  `modifier` text,
  `modify_date` datetime DEFAULT NULL,
  `deleted` bit(1) DEFAULT '0',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for conditionequipexpr
-- ----------------------------
DROP TABLE IF EXISTS `conditionequipexpr`;
CREATE TABLE `conditionequipexpr` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `condition_no` int(11) NOT NULL,
  `iequip_no` int(11) NOT NULL DEFAULT '0',
  `iequip_nm` text,
  `iycyx_no` int(11) NOT NULL DEFAULT '0',
  `iycyx_type` text,
  `iycyx_value` text,
  `condition_expr` int(11) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;