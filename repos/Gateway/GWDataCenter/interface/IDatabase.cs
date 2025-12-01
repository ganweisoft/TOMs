﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using System;
using System.Data;
using System.Data.Common;
namespace AlarmCenter.DataCenter
{
    interface IDatabase : IDisposable
    {
        void GetDataAdapter(string strSQL);
        void CompactDB();
        void SaveDataTableWithDataAdapter(DataTable dt);
        void ExecuteSQLsWithTransaction(string[] cmdtext);
        bool JudgeColumnInTable(string ColumnNm, string TableNm);
        DataRow GetDataRowOfEquip(int sta_no, int equip_no);
        DataRow GetDataRowOfSetParm(int eqp_no, int set_no);
        DataRow GetDataRowOfZiChan(string ZCID);
        DataTable GetDataTableOfEquip();
        DataTable GetAdminsOfAlarm(int iEquip, DateTime time);
        DataTable GetDataTableOfYCP(int sta_no, int equip_no);
        DataTable GetDataTableOfYXP(int sta_no, int equip_no);
        DataTable GetDataTableOfSetParm(int sta_no, int equip_no);
        DataTable GetDataTableFromSQL(string strSQL);
        object GetObjectFromSQL(string strSQL);
        int ExecuteSQL(string strSQL);
        DbDataAdapter DataAdapter
        {
            get;
        }
    }
}
