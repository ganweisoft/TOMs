﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections.Generic;
namespace GWDataCenter
{
    public class UserItem
    {
        bool isadministrator;
        string id, name, password, remark;
        int controllevel;
        public List<RoleItem> Role_List = new List<RoleItem>();
        public List<string> HomePage_List = new List<string>();
        public List<string> AutoInspectionPages_List = new List<string>();
        public event PropertyChangedEventHandler PropertyChanged;
        public string ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }
        public string UserName
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public bool IsAdministrator
        {
            get
            {
                return isadministrator;
            }
            set
            {
                isadministrator = value;
            }
        }
        public string UserPWD
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
            }
        }
        public int ControlLevel
        {
            get
            {
                return controllevel;
            }
            set
            {
                controllevel = value;
            }
        }
        public string Remark
        {
            get
            {
                return remark;
            }
            set
            {
                remark = value;
            }
        }
        public bool CanBrowse2Equip(int EquipNo)
        {
            if (IsAdministrator)
                return true;
            foreach (RoleItem item in Role_List)
            {
                if (item.Browse_Equip_List.Contains(EquipNo))
                    return true;
                foreach (string s in item.Browse_SpecialEquip_List)
                {
                    int idot = s.IndexOf('.');
                    if (idot > 0)
                    {
                        int eqpno;
                        try
                        {
                            eqpno = int.Parse(s.Substring(0, idot));
                        }
                        catch (Exception e)
                        {
                            return false;
                        }
                        if (eqpno == EquipNo)
                            return true;
                    }
                }
            }
            return false;
        }
        public bool CanBrowse2SpecialEquip(string tag)
        {
            if (IsAdministrator)
                return true;
            foreach (RoleItem item in Role_List)
            {
                if (tag.IndexOf('.') > 0)
                {
                    int iEquipNo = Convert.ToInt32(tag.Substring(0, tag.IndexOf('.')));
                    if (item.Browse_Equip_List.Contains(iEquipNo))
                        return true;
                    if (item.Browse_SpecialEquip_List.Contains(tag))
                        return true;
                }
            }
            return false;
        }
        public bool CanControl2SetItem(int EquipNo, int SetNo)
        {
            if (IsAdministrator)
                return true;
            foreach (RoleItem item in Role_List)
            {
                if (item.Control_Equip_List.Contains(EquipNo))
                    return true;
                if (item.Control_SetItem_List.Contains(string.Format("{0}.{1}", EquipNo, SetNo)))
                    return true;
            }
            return false;
        }
        public bool CanOpen2Addin(string classNm)
        {
            /*
            int ID;
            if (IsAdministrator)
                return true;
            string sql = string.Format("select ID from GWAddinModule where ClassName = '{0}'", classNm);
            if (DataCenter.RunEnvironment == AppEnvironment.Client)
            {
                if (DataCenter.db_proxy.GetObjectFromSQL(sql) == null)
                    ID = -1;
                else
                    ID = Convert.ToInt32(DataCenter.db_proxy.GetObjectFromSQL(sql));
            }
            else
            {
                if (StationItem.db.GetObjectFromSQL(sql) == null)
                    ID = -1;
                else
                    ID = Convert.ToInt32(StationItem.db.GetObjectFromSQL(sql));
            }
            foreach (RoleItem item in Role_List)
            {
                if (item.AddinModule_List.Contains(ID))
                    return true;
            }
            */
            return false;
        }
        public bool CanOpen2Addin(int ID)
        {
            if (IsAdministrator)
                return true;
            foreach (RoleItem item in Role_List)
            {
                if (item.AddinModule_List.Contains(ID))
                    return true;
            }
            return false;
        }
        public bool CanOpen2EquipPage(string classNm)
        {
            /*
            int ID = -1;
            try
            {
                DataTable dt = new DataTable();
                if (IsAdministrator)
                    return true;
                if (DataCenter.RunEnvironment == AppEnvironment.Client)
                    dt = DataCenter.db_proxy.GetDataTableFromSQL("select * from GWEquipPages");
                else
                    dt = StationItem.db.GetDataTableFromSQL("select * from GWEquipPages");
                if (dt == null)
                    return false;
                foreach (DataRow r in dt.Rows)
                {
                    string s = Convert.ToString(r["Pages"]);
                    if (s.IndexOf(classNm) >= 0)
                    {
                        ID = Convert.ToInt32(r["ID"]);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
            }
            foreach (RoleItem item in Role_List)
            {
                if (item.Browse_Pages_List.Contains(ID))
                    return true;
            }
            */
            return false;
        }
        public bool CanOpen2EquipPage(int ID)
        {
            if (IsAdministrator)
                return true;
            foreach (RoleItem item in Role_List)
            {
                if (item.Browse_Pages_List.Contains(ID))
                    return true;
            }
            return false;
        }
        public bool CanOpen2Page(string page)
        {
            bool canOpen2 = false;
            if (page.Contains('.'))
            {
                string classNmOfView = page;
                canOpen2 = (CanOpen2Addin(classNmOfView) || CanOpen2EquipPage(classNmOfView));
            }
            else
            {
                string EPID_AMID = page;
                int id;
                if (EPID_AMID.Length > 2
                    && int.TryParse(EPID_AMID.Substring(2), out id))
                {
                    string pagetype = EPID_AMID.Substring(0, 2).ToUpper();
                    if (pagetype == "EP" || pagetype == "AM")
                    {
                        bool isEP = (pagetype == "EP");
                        canOpen2 = isEP ? CanOpen2EquipPage(id) : CanOpen2Addin(id);
                    }
                }
            }
            return canOpen2;
        }
    }
}
