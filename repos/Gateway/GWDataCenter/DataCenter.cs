﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using GWDataCenter.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenGWDataCenter.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
namespace GWDataCenter
{
    public enum AppEnvironment
    {
        Server = 0,
        Client = 1
    }
    public static partial class DataCenter
    {
        static object o_BackUpRunMode = true;
        private static string default_UserNm = "ganweisoft";
        private static string default_PWD = "ganweisoft";
        public static string Default_UserNm { get => default_UserNm; }
        public static string Default_PWD { get => default_PWD; }
        const string strVersion = "OpenIoTCenter-9.0.0.0";
        static public List<RoleItem> RoleList = new List<RoleItem>();
        static public List<UserItem> UserList = new List<UserItem>();
        static Dictionary<int, EquipItem> equipItemDict = new Dictionary<int, EquipItem>();
        static public object brunning = false;
        static public AppEnvironment? RunEnvironment = null;
        static public int InterScreenID = -1;
        static public double ExecSQLTipsSec = 2;
        public static UserItem LoginUser = null;
        public static object GetLoginUserState = true;
        static public void GetLoginUser(string nm, bool isClinet = true)
        {
            LoginUser = GetUserItem(nm, isClinet);
        }
        static object s_StationOnlyMark = "";
        static bool bEncrypt = false;
        static public string S_StationOnlyMark
        {
            get
            {
                lock (s_StationOnlyMark)
                {
                    return (string)s_StationOnlyMark;
                }
            }
            set
            {
                lock (s_StationOnlyMark)
                {
                    if (!bEncrypt)
                    {
                        bEncrypt = true;
                        string temp = Convert.ToString(value);
                        s_StationOnlyMark = temp.Replace('-', 'A').ToLower();
                    }
                }
            }
        }
        public static object myo = true;
        static public Dictionary<int, EquipItem> EquipItemDict
        {
            get
            {
                lock (myo)
                {
                    return equipItemDict;
                }
            }
        }
        static public string GetVersionInfo()
        {
            return strVersion;
        }
        static public EquipItem GetEquipItem(int iEquipNo)
        {
            if (EquipItemDict.ContainsKey(iEquipNo))
                return EquipItemDict[iEquipNo];
            return null;
        }
        static public object GetEquipState(int iEquipNo)
        {
            EquipItem equipitem = GetEquipItem(iEquipNo);
            if (equipitem != null)
            {
                return (int)equipitem.State;
            }
            return null;
        }
        static public object GetYCItemValue(int iEquipNo, int iYcpNo)
        {
            EquipItem equipitem = GetEquipItem(iEquipNo);
            if (equipitem != null)
            {
                YCItem ycitem = equipitem.GetYCItem(iYcpNo);
                if (ycitem != null)
                    return ycitem.YCValue;
            }
            return null;
        }
        static public object GetYXItemValue(int iEquipNo, int iYxpNo)
        {
            EquipItem equipitem = GetEquipItem(iEquipNo);
            if (equipitem != null)
            {
                YXItem yxitem = equipitem.GetYXItem(iYxpNo);
                if (yxitem != null)
                    return yxitem.YXValue;
            }
            return null;
        }
        static public bool IsExiteEqpNo(int iEquipNo)
        {
            if (EquipItemDict.ContainsKey(iEquipNo))
                return true;
            return false;
        }
        static public bool IsExiteYcpNo(int iEquipNo, int iYcpNo)
        {
            if (EquipItemDict.ContainsKey(iEquipNo))
            {
                if (EquipItemDict[iEquipNo].YCItemDict.ContainsKey(iYcpNo))
                    return true;
            }
            return false;
        }
        static public bool IsExiteYxpNo(int iEquipNo, int iYxpNo)
        {
            if (EquipItemDict.ContainsKey(iEquipNo))
            {
                if (EquipItemDict[iEquipNo].YXItemDict.ContainsKey(iYxpNo))
                    return true;
            }
            return false;
        }
        static public string GetVistor()
        {
            if (LoginUser != null)
                return LoginUser.UserName;
            return null;
        }
        static public SetParmTableRow GetDataRowFromSetParm(int iEqpNo, int iSetNo)
        {
            if (StationItem.db_Setparm != null)
            {
                string filterExp = string.Format("equip_no={0} AND set_no={1}", iEqpNo, iSetNo);
                List<SetParmTableRow> rows = StationItem.db_Setparm.Where(m => (m.equip_no == iEqpNo && m.set_no == iSetNo)).ToList();
                if (rows != null && rows.Count() == 1)
                {
                    return rows.First();
                }
            }
            return null;
        }
        public static void WriteLogFile(string input)
        {
            Console.WriteLine(input);
            try
            {
                byte[] bs;
                FileStream filestream;
                string RootPathName = Path.Combine(General.GetApplicationRootPath(), "log");
                System.IO.Directory.CreateDirectory(RootPathName);
                string FileName = null;
                FileName = "XLog.txt";
                if (!File.Exists(Path.Combine(RootPathName, FileName)))
                {
                    filestream = File.Create(Path.Combine(RootPathName, FileName));
                    filestream.Close();
                }
                else
                {
                    FileInfo fileInfo = new FileInfo(Path.Combine(RootPathName, FileName));
                    if (fileInfo.Length > 1024 * 1024 * 5)
                    {
                        File.Delete(Path.Combine(RootPathName, FileName));
                        filestream = File.Create(Path.Combine(RootPathName, FileName));
                        filestream.Close();
                    }
                }
                filestream = File.Open(Path.Combine(RootPathName, FileName), FileMode.Append, FileAccess.Write);
                string sss;
                if (DataCenter.RunEnvironment == AppEnvironment.Client)
                    sss = "客户端";
                else
                    sss = "服务端";
                string strNow = string.Format("*******************************************{0}-{1}({2})*******************************************{3}", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString(), sss, Environment.NewLine);
                bs = System.Text.Encoding.Default.GetBytes(strNow);
                filestream.Write(bs, 0, bs.Length);
                bs = System.Text.Encoding.Default.GetBytes(input);
                filestream.Write(bs, 0, bs.Length);
                bs = System.Text.Encoding.Default.GetBytes(Environment.NewLine);
                filestream.Write(bs, 0, bs.Length);
                string fg = "**********************************************************************************************";
                bs = System.Text.Encoding.Default.GetBytes(fg);
                filestream.Write(bs, 0, bs.Length);
                bs = System.Text.Encoding.Default.GetBytes(Environment.NewLine);
                filestream.Write(bs, 0, bs.Length);
                filestream.Close();
                filestream.Dispose();
            }
            catch (Exception e)
            {
            }
        }
        public static void WriteLogFile(string input, LogType type)
        {
            try
            {
                byte[] bs;
                FileStream filestream;
                string RootPathName = Path.Combine(General.GetApplicationRootPath(), "log");
                System.IO.Directory.CreateDirectory(RootPathName);
                string FileName = null;
                switch (type)
                {
                    case LogType.Error:
                        FileName = "XLog.txt";
                        break;
                    case LogType.Debug:
                        FileName = "Debug.txt";
                        break;
                    case LogType.Config:
                        FileName = "Config.txt";
                        break;
                    default:
                        break;
                }
                if (FileName == null)
                    return;
                if (!File.Exists(Path.Combine(RootPathName, FileName)))
                {
                    filestream = File.Create(Path.Combine(RootPathName, FileName));
                    filestream.Close();
                }
                else
                {
                    FileInfo fileInfo = new FileInfo(Path.Combine(RootPathName, FileName));
                    if (fileInfo.Length > 1024 * 1024 * 500)
                    {
                        if (type != LogType.Config)
                        {
                            File.Delete(Path.Combine(RootPathName, FileName));
                            filestream = File.Create(Path.Combine(RootPathName, FileName));
                            filestream.Close();
                        }
                    }
                }
                filestream = File.Open(Path.Combine(RootPathName, FileName), FileMode.Append, FileAccess.Write);
                string sss;
                if (DataCenter.RunEnvironment == AppEnvironment.Client)
                    sss = "客户端";
                else
                    sss = "服务端";
                string strNow = string.Format("*******************************************{0}-{1}({2})*******************************************{3}", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString(), sss, Environment.NewLine);
                bs = System.Text.Encoding.Default.GetBytes(strNow);
                filestream.Write(bs, 0, bs.Length);
                bs = System.Text.Encoding.Default.GetBytes(input);
                filestream.Write(bs, 0, bs.Length);
                bs = System.Text.Encoding.Default.GetBytes(Environment.NewLine);
                filestream.Write(bs, 0, bs.Length);
                string fg = "**********************************************************************************************";
                bs = System.Text.Encoding.Default.GetBytes(fg);
                filestream.Write(bs, 0, bs.Length);
                bs = System.Text.Encoding.Default.GetBytes(Environment.NewLine);
                filestream.Write(bs, 0, bs.Length);
                filestream.Close();
                filestream.Dispose();
            }
            catch (Exception e)
            {
            }
        }
        public static void ClearAllEvents(object o)
        {
            try
            {
                EventInfo[] events = o.GetType().GetEvents(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (events == null || events.Length < 1)
                {
                    return;
                }
                for (int i = 0; i < events.Length; i++)
                {
                    EventInfo ei = events[i];
                    FieldInfo fi = ei.DeclaringType.GetField(ei.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (fi != null)
                    {
                        fi.SetValue(o, null);
                    }
                }
            }
            catch
            {
            }
        }
        static readonly string EquipDebugProperty = "AlarmCenter.Debug";
        static public List<int> EquipNoDebugs = new List<int>();
        static public void GetEquipDebugState()
        {
            string s = PropertyService.Get(EquipDebugProperty);
            string[] ss = s.Split(',', '，');
            foreach (string s1 in ss)
            {
                try
                {
                    EquipNoDebugs.Add(Convert.ToInt32(s1));
                }
                catch (Exception e)
                {
                }
            }
        }
        static public bool IsDebugState(int iEqpNo)
        {
            foreach (int k in EquipNoDebugs)
            {
                if (k == iEqpNo)
                    return true;
            }
            return false;
        }
        static public string GetStackInfo()
        {
            StackTrace st = new StackTrace(1, true);
            StackFrame[] stFrames = st.GetFrames();
            string result = string.Empty;
            foreach (StackFrame sf in stFrames)
            {
                result += string.Format("{0}--> {1}--> {2}-->{3}" + System.Environment.NewLine, sf.GetFileName(), sf.GetFileLineNumber(), sf.GetFileColumnNumber(), sf.GetMethod().ToString());
            }
            return result;
        }
        static public void Start()
        {
            Console.WriteLine($"当前程序版本: {strVersion}");
            RunEnvironment = AppEnvironment.Server;
            lock (brunning)
            {
                if (!(bool)brunning)
                {
                    try
                    {
                        if (!StationItem.init())
                        {
                            return;
                        }
                        foreach (KeyValuePair<string, object> entry in StationItem.EquipCategoryDict)
                        {
                            SubEquipList EquipList = (SubEquipList)entry.Value;
                            EquipList.OldEquip = null;
                            EquipList.StartRefreshThread();
                            EquipList.StartSetParmThread();
                        }
                        brunning = true;
                        string s;
                        s = ResourceService.GetString("AlarmCenter.DataCenter.Msg6");
                        MessageService.AddMessage(MessageLevel.Info, s, 0, false);
                    }
                    catch (Exception e)
                    {
                        DataCenter.WriteLogFile(e.ToString());
                    }
                    Console.WriteLine("程序运行中...");
                }
            }
        }
        static public void Stop()
        {
            lock (brunning)
            {
                if ((bool)brunning)
                {
                    foreach (KeyValuePair<string, object> entry in StationItem.EquipCategoryDict)
                    {
                        SubEquipList EquipList = (SubEquipList)entry.Value;
                        EquipList.SuspendRefreshThread();
                    }
                    brunning = false;
                    string s;
                    s = ResourceService.GetString("AlarmCenter.DataCenter.Msg7");
                    MessageService.AddMessage(MessageLevel.Info, s, 0, false);
                }
            }
        }
        #region 读写系统的配置文件
        public static string GetPropertyFromPropertyService(string PropertyName, string NodeName, string DefaultValue)
        {
            string strValue = "";
            try
            {
                if (string.IsNullOrEmpty(NodeName))
                {
                    strValue = PropertyService.Get(PropertyName, DefaultValue);
                    return strValue;
                }
                Properties properties = PropertyService.Get(PropertyName, new Properties());
                if (properties.Contains(NodeName))
                {
                    strValue = properties.Get(NodeName, DefaultValue);
                }
                else
                {
                    strValue = DefaultValue;
                    properties.Set(NodeName, DefaultValue);
                    PropertyService.Save();
                }
                return strValue;
            }
            catch (Exception e)
            {
                DataCenter.WriteLogFile(e.ToString());
            }
            return strValue;
        }
        public static void SetPropertyToPropertyService(string PropertyName, string NodeName, string Value)
        {
            try
            {
                if (string.IsNullOrEmpty(NodeName))
                {
                    PropertyService.Set(PropertyName, Value);
                    return;
                }
                Properties properties = PropertyService.Get(PropertyName, new Properties());
                if (properties.Contains(NodeName))
                {
                    properties.Set(NodeName, Value);
                    PropertyService.Save();
                }
            }
            catch (Exception e)
            {
                DataCenter.WriteLogFile(e.ToString());
            }
        }
        #endregion
        static public UserItem GetUserItem(string nm, bool isClinet = true)
        {
            List<RoleItem> RoleList = new List<RoleItem>();
            RoleList.Clear();
            List<GWRoleTableRow> Rows;
            lock (GWDbProvider.lockstate)
            {
                using (var db = StationItem.MyGWDbProvider.serviceProvider.GetService<GWDataContext>())
                {
                    Rows = db.GWRoleTable.FromSqlRaw("SELECT * FROM GWRole").AsNoTracking().ToList();
                }
            }
            if (!Rows.Any())
                WriteLogFile("用户权限里面还没有设置角色!");
            else
            {
                foreach (GWRoleTableRow r in Rows)
                {
                    RoleItem roleitem = new RoleItem();
                    roleitem.name = AESDecrypt(r.Name, GeneratAESKey());
                    roleitem.remark = AESDecrypt(r.remark, GeneratAESKey());
                    string[] ControlEqps = AESDecrypt(r.ControlEquips, GeneratAESKey()).Split('#');
                    string[] ControlUnits = AESDecrypt(r.ControlEquips_Unit, GeneratAESKey()).Split('#');
                    string[] BrowseEqps = AESDecrypt(r.BrowseEquips, GeneratAESKey()).Split('#');
                    string[] BrowsePages = AESDecrypt(r.BrowsePages, GeneratAESKey()).Split('#');
                    string[] AddinModules = AESDecrypt(r.SystemModule, GeneratAESKey()).Split('#');
                    string[] SpecialBrowseEquips = AESDecrypt(r.SpecialBrowseEquip, GeneratAESKey()).Split('#');
                    foreach (string s in ControlEqps)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(s))
                                roleitem.Control_Equip_List.Add(int.Parse(s));
                        }
                        catch (Exception e)
                        {
                            WriteLogFile(e.ToString());
                        }
                    }
                    foreach (string s in ControlUnits)
                    {
                        roleitem.Control_SetItem_List.Add(s);
                    }
                    foreach (string s in BrowseEqps)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(s))
                                roleitem.Browse_Equip_List.Add(int.Parse(s));
                        }
                        catch (Exception e1)
                        {
                            WriteLogFile(e1.ToString());
                        }
                    }
                    foreach (string s in BrowsePages)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(s))
                                roleitem.Browse_Pages_List.Add(int.Parse(s));
                        }
                        catch (Exception e2)
                        {
                            WriteLogFile(e2.ToString());
                        }
                    }
                    foreach (string s in AddinModules)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(s))
                                roleitem.AddinModule_List.Add(int.Parse(s));
                        }
                        catch (Exception e3)
                        {
                            WriteLogFile(e3.ToString());
                        }
                    }
                    foreach (string s in SpecialBrowseEquips)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(s))
                                roleitem.Browse_SpecialEquip_List.Add(s);
                        }
                        catch (Exception e4)
                        {
                            WriteLogFile(e4.ToString());
                        }
                    }
                    RoleList.Add(roleitem);
                }
            }
            List<GWUserTableRow> UserRows;
            string nm1 = AESEncrypt(nm, GeneratAESKey());
            lock (GWDbProvider.lockstate)
            {
                using (var db = StationItem.MyGWDbProvider.serviceProvider.GetService<GWDataContext>())
                {
                    UserRows = db.GWUserTable.FromSqlRaw($"SELECT * FROM GWUser where name='{nm1}'").AsNoTracking().ToList();
                }
            }
            if (!UserRows.Any())
            {
                return null;
            }
            if (UserRows.Count() == 1)
            {
                GWUserTableRow r = UserRows.First();
                UserItem useritem = new UserItem();
                useritem.Remark = AESDecrypt(r.Remark, GeneratAESKey());
                useritem.UserName = AESDecrypt(r.Name, GeneratAESKey());
                useritem.UserPWD = AESDecrypt(r.Password, GeneratAESKey());
                if (string.IsNullOrEmpty(r.ControlLevel))
                {
                    useritem.ControlLevel = 1;
                }
                else
                {
                    useritem.ControlLevel = Int32.Parse(AESDecrypt(r.ControlLevel, GeneratAESKey()));
                }
                string[] RoleNmList = AESDecrypt(r.Roles, GeneratAESKey()).Split('#');
                if (RoleNmList.Length == 1 && RoleNmList[0] == "ADMIN")
                {
                    useritem.IsAdministrator = true;
                }
                else
                {
                    if (RoleList.Count == 0)
                        return null;
                    foreach (string s in RoleNmList)
                    {
                        foreach (RoleItem item in RoleList)
                        {
                            if (item.name == s)
                            {
                                useritem.Role_List.Add(item);
                            }
                        }
                    }
                }
                string homePages = AESDecrypt(r.HomePages, GeneratAESKey());
                if (!string.IsNullOrWhiteSpace(homePages))
                {
                    useritem.HomePage_List = homePages.Split('+').ToList();
                }
                string AutoInspectionPages = AESDecrypt(r.AutoInspectionPages, GeneratAESKey());
                if (!string.IsNullOrWhiteSpace(AutoInspectionPages))
                {
                    useritem.AutoInspectionPages_List = AutoInspectionPages.Split('+').ToList();
                }
                return useritem;
            }
            return null;
        }
    }
}
