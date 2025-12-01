﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections.Generic;
namespace GWDataCenter
{
    public class RoleItem
    {
        string m_name, m_remark;
        public event PropertyChangedEventHandler PropertyChanged;
        public List<Int32> Control_Equip_List = new List<int>();
        public List<string> Control_SetItem_List = new List<string>();
        public List<Int32> Browse_Equip_List = new List<Int32>();
        public List<string> Browse_SpecialEquip_List = new List<string>();
        public List<Int32> Browse_Pages_List = new List<Int32>();
        public List<Int32> AddinModule_List = new List<Int32>();
        bool IsCheck;
        public static string strVideoType = "GWVIDEOCLASS";
        public static string strDoorType = "GWDOORCLASS";
        public static string strAlarmType = "GWALARMCLASS";
        public static string[] SpecialEqps = new string[] { strVideoType, strDoorType };
        public static string[] SpecialDrvs = new string[] { "GWChangJing.NET.dll" };
        public void FillAllControlEquips()
        {
            /*
            Control_Equip_List.Clear();
            DataTable dt = new DataTable();
            string SQL = "select DISTINCT equip_no from SetParm Order by equip_no";
            dt = AlarmCenter.DataCenter.DataCenter.db_proxy.GetDataTableFromSQL(SQL);
            if (dt == null)
                return;
            foreach (DataRow r in dt.Rows)
            {
                Control_Equip_List.Add(Convert.ToInt32(r["equip_no"]));
            }
            DataTable dt1 = AlarmCenter.DataCenter.DataCenter.db_proxy.GetDataTableOfEquip();
            foreach (DataRow r in dt1.Rows)
            {
                if (SpecialEqps.Contains(Convert.ToString(r["equip_detail"]).ToUpper().Trim()))
                {
                    if (!Control_Equip_List.Contains(Convert.ToInt32(r["equip_no"])))
                        Control_Equip_List.Add(Convert.ToInt32(r["equip_no"]));
                }
            }
            */
        }
        public void FillAllAddinModule()
        {
            /*
            AddinModule_List.Clear();
            DataTable dt = new DataTable();
            dt = AlarmCenter.DataCenter.DataCenter.db_proxy.GetDataTableFromSQL("select * from GWAddinModule");
            if (dt == null)
                return;
            foreach (DataRow r in dt.Rows)
            {
                AddinModule_List.Add(Convert.ToInt32(r["ID"]));
            }
            */
        }
        public void FillAllBrowsePages()
        {
            /*
            try
            {
                Browse_Pages_List.Clear();
                DataTable dt = new DataTable();
                dt = AlarmCenter.DataCenter.DataCenter.db_proxy.GetDataTableFromSQL("select * from GWEquipPages");
                if (dt == null)
                    return;
                foreach (DataRow r in dt.Rows)
                {
                    Browse_Pages_List.Add(Convert.ToInt32(r["ID"]));
                }
            }
            catch (Exception e)
            {
            }
            */
        }
        public void FillAllBrowseEquips()
        {
            /*
            Browse_Equip_List.Clear();
            DataTable dt = new DataTable();
            dt = AlarmCenter.DataCenter.DataCenter.db_proxy.GetDataTableOfEquip();
            foreach (DataRow r in dt.Rows)
            {
                Browse_Equip_List.Add(Convert.ToInt32(r["equip_no"])); 
            }
            */
        }
        public bool ischeck
        {
            get
            {
                return IsCheck;
            }
            set
            {
                IsCheck = value;
            }
        }
        public string name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }
        public string remark
        {
            get
            {
                return m_remark;
            }
            set
            {
                m_remark = value;
            }
        }
    }
}
