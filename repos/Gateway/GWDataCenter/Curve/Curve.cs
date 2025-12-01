﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using GWDataCenter.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
namespace GWDataCenter
{
    public static partial class DataCenter
    {
        public enum CurveType
        {
            Double = 1,
            State = 2
        }
        public enum CurveDataState
        {
            Normal = 1,
            NoRecord = 2,
        }
        static readonly string CurveProperty = "AlarmCenter.Gui.OptionPanels.CurveOptions";
        public static byte[] GetMyCurveData(DateTime d, int eqpno, int ycyxno, string type = "C")
        {
            if (!GWDataCenter.StationItem.bCurveStoreInDB)
            {
                FileStream filestream;
                string FullPathName, FileName = "";
                FullPathName = Path.Combine(StationItem.CurveRootPathName, d.Year.ToString());
                FullPathName = Path.Combine(FullPathName, d.Month.ToString());
                FullPathName = Path.Combine(FullPathName, d.Day.ToString());
                if (type.ToUpper() == "C")
                    FileName = string.Format("C_{0}_{1}.cur", eqpno, ycyxno);
                if (type.ToUpper() == "X")
                    FileName = string.Format("X_{0}_{1}.cur", eqpno, ycyxno);
                if (!Directory.Exists(FullPathName))
                {
                    return null;
                }
                if (!File.Exists(Path.Combine(FullPathName, FileName)))
                {
                    return null; ;
                }
                else
                {
                    try
                    {
                        filestream = File.Open(Path.Combine(FullPathName, FileName), FileMode.Open, FileAccess.Read);
                        byte[] buf = new byte[(int)filestream.Length];
                        filestream.Read(buf, 0, (int)filestream.Length);
                        filestream.Close();
                        filestream.Dispose();
                        return buf;
                    }
                    catch (Exception e)
                    {
                        DataCenter.WriteLogFile(e.ToString());
                        return null;
                    }
                }
            }
            else
            {
                Int64 key;
                key = Curve.GetCurveKey(eqpno, ycyxno, type);
                lock (GWCurveContext.lockstate)
                {
                    using (var db = new GWCurveContext() { strCurveDate = $"ZZCurveData{d.ToString("yyyyMMdd")}" })
                    {
                        try
                        {
                            var R = db.CurveDataTable.Single(m => (m.key == key));
                            return R.curvedata;
                        }
                        catch
                        {
                            return null;
                        }
                    }
                }
            }
        }
        public struct mySeriesPoint
        {
            public double datetime;
            public byte mySeriesPointType;
            public double value;
        }
        public struct myCurveData
        {
            public DateTime datetime;
            public double value;
            public CurveDataState state;
        }
        public static List<mySeriesPoint> LoadHistoryCurveData(DateTime BgnT, DateTime EndT, int stano, int eqpno, int ycyxno, string type)
        {
            List<byte> valuelist = new List<byte>(1024 * 128);
            List<mySeriesPoint> SeriesPointList = new List<mySeriesPoint>(1024 * 128);
            mySeriesPoint oldpt = new mySeriesPoint();
            int iCount = 0;
            while (BgnT.Date <= EndT.Date)
            {
                DateTime oldDT = new DateTime();
                double oldVal = 0;
                valuelist.Clear();
                DateTime date = new DateTime(BgnT.Year, BgnT.Month, BgnT.Day, 0, 0, 0);
                byte[] retdatas;
                retdatas = GetMyCurveData(date, eqpno, ycyxno, type);
                if (retdatas != null)
                    valuelist.AddRange(retdatas);
                byte[] Datas = valuelist.ToArray();
                BgnT = BgnT.AddDays(1);
                bool bStartDotOfToday = false;
                if (Datas.GetLength(0) == 0)
                    continue;
                if (type == "C")
                {
                    for (int k = 1; k < Datas.GetLength(0);)
                    {
                        if (Datas[k] == 254)
                        {
                            bStartDotOfToday = true;
                            if (iCount > 0)
                            {
                                mySeriesPoint pt0 = new mySeriesPoint();
                                pt0.datetime = oldpt.datetime;
                                pt0.mySeriesPointType = 1;
                                pt0.value = float.NaN;
                                SeriesPointList.Add(pt0);
                            }
                            byte[] m = { Datas[k + 1], Datas[k + 2], Datas[k + 3], Datas[k + 4] };
                            int millisecond = BitConverter.ToInt32(m, 0);
                            date = date.AddHours(millisecond / (3600 * 1000));
                            date = date.AddMinutes((millisecond % (3600 * 1000)) / (60 * 1000));
                            date = date.AddSeconds((millisecond % (60 * 1000)) / 1000);
                            date = date.AddMilliseconds(millisecond % 1000);
                            oldDT = date;
                            byte[] b = { Datas[k + 5], Datas[k + 6], Datas[k + 7], Datas[k + 8], Datas[k + 9], Datas[k + 10], Datas[k + 11], Datas[k + 12] };
                            mySeriesPoint pt = new mySeriesPoint();
                            pt.mySeriesPointType = 0;
                            pt.datetime = date.ToOADate();
                            pt.value = BitConverter.ToDouble(b, 0);
                            oldVal = pt.value;
                            mySeriesPoint pt1 = new mySeriesPoint();
                            pt1.mySeriesPointType = 0;
                            pt1.datetime = pt.datetime;
                            pt1.value = float.NaN;
                            SeriesPointList.Add(pt1);
                            SeriesPointList.Add(pt);
                            oldpt = pt;
                            k += 13;
                            date = date.Date;
                            iCount++;
                            continue;
                        }
                        if (Datas[k] == 255)
                        {
                            bStartDotOfToday = true;
                            byte[] m = { Datas[k + 1], Datas[k + 2], Datas[k + 3], Datas[k + 4] };
                            int millisecond = BitConverter.ToInt32(m, 0);
                            date = date.AddHours(millisecond / (3600 * 1000));
                            date = date.AddMinutes((millisecond % (3600 * 1000)) / (60 * 1000));
                            date = date.AddSeconds((millisecond % (60 * 1000)) / 1000);
                            date = date.AddMilliseconds(millisecond % 1000);
                            oldDT = date;
                            byte[] b = { Datas[k + 5], Datas[k + 6], Datas[k + 7], Datas[k + 8], Datas[k + 9], Datas[k + 10], Datas[k + 11], Datas[k + 12] };
                            mySeriesPoint pt = new mySeriesPoint();
                            pt.mySeriesPointType = 1;
                            pt.datetime = date.ToOADate();
                            pt.value = BitConverter.ToDouble(b, 0);
                            oldVal = pt.value;
                            SeriesPointList.Add(pt);
                            k += 13;
                            date = date.Date;
                            oldpt = pt;
                            iCount++;
                            continue;
                        }
                        /*
                        * 这段代码防止曲线文件损坏,每一个曲线文件，第一个点都是开始点或者连续点。
                        */
                        if (!bStartDotOfToday)
                        {
                            k++;
                            continue;
                        }
                    }
                }
                if (type == "X")
                {
                    for (int k = 1; k < Datas.GetLength(0);)
                    {
                        if (Datas[k] == 254)
                        {
                            bStartDotOfToday = true;
                            if (iCount > 0)
                            {
                                mySeriesPoint pt0 = new mySeriesPoint();
                                pt0.datetime = oldpt.datetime;
                                pt0.mySeriesPointType = 1;
                                pt0.value = float.NaN;
                                SeriesPointList.Add(pt0);
                            }
                            byte[] m = { Datas[k + 1], Datas[k + 2], Datas[k + 3], Datas[k + 4] };
                            int millisecond = BitConverter.ToInt32(m, 0);
                            date = date.AddHours(millisecond / (3600 * 1000));
                            date = date.AddMinutes((millisecond % (3600 * 1000)) / (60 * 1000));
                            date = date.AddSeconds((millisecond % (60 * 1000)) / 1000);
                            date = date.AddMilliseconds(millisecond % 1000);
                            oldDT = date;
                            mySeriesPoint pt = new mySeriesPoint();
                            pt.mySeriesPointType = 0;
                            pt.datetime = date.ToOADate();
                            pt.value = Datas[k + 5];
                            oldVal = pt.value;
                            mySeriesPoint pt1 = new mySeriesPoint();
                            pt1.mySeriesPointType = 0;
                            pt1.datetime = pt.datetime;
                            pt1.value = float.NaN;
                            SeriesPointList.Add(pt1);
                            SeriesPointList.Add(pt);
                            oldpt = pt;
                            k += 6;
                            date = date.Date;
                            iCount++;
                            continue;
                        }
                        if (Datas[k] == 255)
                        {
                            bStartDotOfToday = true;
                            byte[] m = { Datas[k + 1], Datas[k + 2], Datas[k + 3], Datas[k + 4] };
                            int millisecond = BitConverter.ToInt32(m, 0);
                            date = date.AddHours(millisecond / (3600 * 1000));
                            date = date.AddMinutes((millisecond % (3600 * 1000)) / (60 * 1000));
                            date = date.AddSeconds((millisecond % (60 * 1000)) / 1000);
                            date = date.AddMilliseconds(millisecond % 1000);
                            oldDT = date;
                            mySeriesPoint pt = new mySeriesPoint();
                            pt.mySeriesPointType = 1;
                            pt.datetime = date.ToOADate();
                            pt.value = Datas[k + 5];
                            oldVal = pt.value;
                            SeriesPointList.Add(pt);
                            k += 6;
                            date = date.Date;
                            oldpt = pt;
                            iCount++;
                            continue;
                        }
                        /*
                        * 这段代码防止曲线文件损坏,每一个曲线文件，第一个点都是开始点或者连续点。
                        */
                        if (!bStartDotOfToday)
                        {
                            k++;
                            continue;
                        }
                    }
                }
            }
            valuelist.Clear();
            List<mySeriesPoint> MyResult = (from e in SeriesPointList orderby e.datetime select e).ToList();
            return MyResult;
        }
        public static async Task<List<myCurveData>> GetDataFromCurveAsync(List<DateTime> DTList, int stano, int eqpno, int ycyxno, string type)
        {
            var task = await Task.Factory.StartNew(delegate
            {
                DTList.Sort();
                Dictionary<DateTime, List<DateTime>> DTDict = GetDayDataTime(DTList);
                List<myCurveData> DataList = new List<myCurveData>(1024);
                foreach (var item in DTDict)
                {
                    Dictionary<DateTime, myCurveData> CurveDataDict = new Dictionary<DateTime, myCurveData>();
                    foreach (DateTime dt in item.Value)
                    {
                        CurveDataDict.Add(dt, new myCurveData { value = float.NaN, state = CurveDataState.NoRecord, datetime = dt });
                    }
                    List<mySeriesPoint> SeriesPointList = LoadHistoryCurveData(item.Key, item.Key, stano, eqpno, ycyxno, type);
                    int DT_Index = 0;
                    List<DateTime> sub_DTList = item.Value;
                    for (int SP_Index = 0; SP_Index < SeriesPointList.Count; SP_Index++)
                    {
                        if ((SP_Index + 1) >= SeriesPointList.Count)
                            break;
                        for (; DT_Index < sub_DTList.Count;)
                        {
                            if (SeriesPointList[SP_Index].datetime <= sub_DTList[DT_Index].ToOADate() && SeriesPointList[SP_Index + 1].datetime >= sub_DTList[DT_Index].ToOADate())
                            {
                                if (SeriesPointList[SP_Index].value is float.NaN && SeriesPointList[SP_Index].datetime == SeriesPointList[SP_Index + 1].datetime)
                                {
                                    CurveDataDict[sub_DTList[DT_Index]] = new myCurveData { value = SeriesPointList[SP_Index + 1].value, datetime = sub_DTList[DT_Index], state = CurveDataState.Normal };
                                    DT_Index++;
                                }
                                else if (SeriesPointList[SP_Index + 1].value is float.NaN && SeriesPointList[SP_Index].datetime == SeriesPointList[SP_Index + 1].datetime)
                                {
                                    CurveDataDict[sub_DTList[DT_Index]] = new myCurveData { value = SeriesPointList[SP_Index].value, datetime = sub_DTList[DT_Index], state = CurveDataState.Normal };
                                    DT_Index++;
                                }
                                else if (SeriesPointList[SP_Index + 1].value is float.NaN && SeriesPointList[SP_Index].value is float.NaN)
                                {
                                    if (SeriesPointList[SP_Index + 1].datetime == sub_DTList[DT_Index].ToOADate())
                                    {
                                        if ((SP_Index + 2) < SeriesPointList.Count)
                                        {
                                            CurveDataDict[sub_DTList[DT_Index]] = new myCurveData { value = SeriesPointList[SP_Index + 2].value, datetime = sub_DTList[DT_Index], state = CurveDataState.Normal };
                                            DT_Index++;
                                        }
                                    }
                                    else
                                    {
                                        CurveDataDict[sub_DTList[DT_Index]] = new myCurveData { value = float.NaN, datetime = sub_DTList[DT_Index], state = CurveDataState.NoRecord };
                                        DT_Index++;
                                    }
                                }
                                else if (SeriesPointList[SP_Index].datetime == sub_DTList[DT_Index].ToOADate())
                                {
                                    CurveDataDict[sub_DTList[DT_Index]] = new myCurveData { value = SeriesPointList[SP_Index].value, datetime = sub_DTList[DT_Index], state = CurveDataState.Normal };
                                    DT_Index++;
                                }
                                else if (SeriesPointList[SP_Index + 1].datetime == sub_DTList[DT_Index].ToOADate())
                                {
                                    CurveDataDict[sub_DTList[DT_Index]] = new myCurveData { value = SeriesPointList[SP_Index + 1].value, datetime = sub_DTList[DT_Index], state = CurveDataState.Normal };
                                    DT_Index++;
                                }
                                else
                                {
                                    CurveDataDict[sub_DTList[DT_Index]] = new myCurveData { value = SeriesPointList[SP_Index].value, datetime = sub_DTList[DT_Index], state = CurveDataState.Normal };
                                    DT_Index++;
                                }
                            }
                            else
                            {
                                if (SeriesPointList[0].value is float.NaN && SeriesPointList[0].datetime > sub_DTList[DT_Index].ToOADate())
                                {
                                    CurveDataDict[sub_DTList[DT_Index]] = new myCurveData { value = float.NaN, datetime = sub_DTList[DT_Index], state = CurveDataState.NoRecord };
                                    DT_Index++;
                                }
                                else if (!(SeriesPointList[0].value is float.NaN) && SeriesPointList[0].datetime > sub_DTList[DT_Index].ToOADate())
                                {
                                    if (type == "C")
                                    {
                                        List<mySeriesPoint> SeriesPointList1 = LoadHistoryCurveData(item.Key.AddDays(-1), item.Key.AddDays(-1), stano, eqpno, ycyxno, type);
                                        if (SeriesPointList1.Count > 0)
                                            CurveDataDict[sub_DTList[DT_Index]] = new myCurveData { value = SeriesPointList1[SeriesPointList.Count - 1].value, datetime = sub_DTList[DT_Index], state = CurveDataState.Normal };
                                        DT_Index++;
                                    }
                                    else if (type == "X")
                                    {
                                        CurveDataDict[sub_DTList[DT_Index]] = new myCurveData { value = SeriesPointList[SP_Index].value, datetime = sub_DTList[DT_Index], state = CurveDataState.Normal };
                                        DT_Index++;
                                    }
                                }
                                else if (sub_DTList[DT_Index].ToOADate() > SeriesPointList[SeriesPointList.Count - 1].datetime)
                                {
                                    CurveDataDict[sub_DTList[DT_Index]] = new myCurveData { value = float.NaN, datetime = sub_DTList[DT_Index], state = CurveDataState.NoRecord };
                                    DT_Index++;
                                }
                                else
                                    break;
                            }
                        }
                        if (DT_Index == sub_DTList.Count)
                        {
                            break;
                        }
                    }
                    foreach (var item1 in CurveDataDict)
                    {
                        DataList.Add(item1.Value);
                    }
                }
                return DataList;
            }, TaskCreationOptions.LongRunning);
            return task;
        }
        public static async Task<List<myCurveData>> GetChangedDataFromCurveAsync(DateTime bgn, DateTime end, int stano, int eqpno, int ycyxno, string type)
        {
            var task = await Task.Factory.StartNew(delegate
            {
                List<myCurveData> DataList = new List<myCurveData>(1024);
                List<mySeriesPoint> SeriesPointList = LoadHistoryCurveData(bgn, end, stano, eqpno, ycyxno, type);
                double oldvalue = double.MaxValue;
                for (int SP_Index = 0; SP_Index < SeriesPointList.Count; SP_Index++)
                {
                    if (bgn.ToOADate() <= SeriesPointList[SP_Index].datetime && end.ToOADate() >= SeriesPointList[SP_Index].datetime)
                    {
                        if (!(SeriesPointList[SP_Index].value is float.NaN))
                        {
                            if (SeriesPointList[SP_Index].value != oldvalue)
                            {
                                DataList.Add(new myCurveData { value = SeriesPointList[SP_Index].value, state = CurveDataState.Normal, datetime = DateTime.FromOADate(SeriesPointList[SP_Index].datetime) });
                                oldvalue = SeriesPointList[SP_Index].value;
                            }
                        }
                    }
                    if (end.ToOADate() < SeriesPointList[SP_Index].datetime)
                        break;
                }
                return DataList;
            }, TaskCreationOptions.LongRunning);
            return task;
        }
        static Dictionary<DateTime, List<DateTime>> GetDayDataTime(List<DateTime> DTList)
        {
            Dictionary<DateTime, List<DateTime>> DTDict = new Dictionary<DateTime, List<DateTime>>();
            foreach (DateTime dt in DTList)
            {
                DateTime date = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
                if (DTDict.ContainsKey(date))
                {
                    DTDict[date].Add(dt);
                }
                else
                {
                    DTDict.Add(date, new List<DateTime> { dt });
                }
            }
            return DTDict;
        }
    }
    public static class Curve
    {
        #region 统一记录历史曲线的入口线程
        public struct myRecordData
        {
            public bool bSpanDay;
            public byte[] DotValues;
            public int iEqpNo;
            public int iYcYxNo;
            public string type;
        }
        public static List<myRecordData> RecordDataList = new List<myRecordData>();
        public static List<myRecordData> tempRecordDataList = new List<myRecordData>();
        public static object Lock4RecordDataList = false;
        static FileStream filestream;
        static Dictionary<int, List<CurveDataTableRow>> Rows4DayTable = new Dictionary<int, List<CurveDataTableRow>>();
        static Dictionary<int, GWCurveContext> DB4Day = new Dictionary<int, GWCurveContext>();
        static List<CurveDataTableRow> UpdateRows = new List<CurveDataTableRow>();
        static Int64 key = 0;
        static Queue iCreatedDays = new Queue();
        static Queue iNeedUpdateDays = new Queue();
        public static Int64 GetCurveKey(int equipno, int ycyxno, string type)
        {
            Int64 k1 = Convert.ToInt64(equipno) << 25;
            Int64 k2 = (Int64)ycyxno << 1;
            Int64 k3 = 0;
            if (type == "C")
                k3 = 0;
            if (type == "X")
                k3 = 1;
            return k1 + k2 + k3;
        }
        static string strUpdateTableName;
        public static void CreatCurveTableFromDate(DateTime dt)
        {
            if (iCreatedDays.Contains(dt.Day))
                return;
            string mysql = "";
            if (DataCenter.GetPropertyFromPropertyService("AlarmCenter.Gui.OptionPanels.DatabaseOptions", "SQLite.Select", "").ToUpper().Trim() == "TRUE")
            {
                mysql = $"CREATE TABLE ZZCurveData{dt.ToString("yyyyMMdd")} " +
                @"(
                    curvedata BLOB,
                    Reserve1  TEXT,
                    Reserve2  TEXT,
                    Reserve3  TEXT,
                    [key]     INTEGER NOT NULL,
                    CONSTRAINT PK_CurveData PRIMARY KEY (
                        [key]
                    )
                );";
            }
            if (DataCenter.GetPropertyFromPropertyService("AlarmCenter.Gui.OptionPanels.DatabaseOptions", "MySql.Select", "").ToUpper().Trim() == "TRUE")
            {
                mysql = $"CREATE TABLE ZZCurveData{dt.ToString("yyyyMMdd")} " +
                @"(
                  `Reserve1` text CHARACTER SET utf8 COLLATE utf8_general_ci,
                  `Reserve2` text CHARACTER SET utf8 COLLATE utf8_general_ci,
                  `Reserve3` text CHARACTER SET utf8 COLLATE utf8_general_ci,
                  `curvedata` longblob,
                  `key` bigint(20) NOT NULL,
                  PRIMARY KEY (`key`)
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8;";
            }
            try
            {
                lock (GWCurveContext.lockstate)
                {
                    using (var db = new GWCurveContext())
                    {
                        db.Database.ExecuteSqlRaw(mysql);
                    }
                }
            }
            catch (Exception e)
            {
            }
            iCreatedDays.Enqueue(dt.Day);
            DB4Day.Add(dt.Day, new GWCurveContext() { strCurveDate = strUpdateTableName });
            if (iCreatedDays.Count > 2)
            {
                DB4Day.Remove(Convert.ToInt32(iCreatedDays.Dequeue()));
            }
        }
        public static void PushRecordData2List(bool bSpan, byte[] Ls, string t, int EqpNo, int YcYxNo)
        {
            lock (Lock4RecordDataList)
            {
                RecordDataList.Add(new myRecordData { bSpanDay = bSpan, DotValues = Ls, type = t, iEqpNo = EqpNo, iYcYxNo = YcYxNo });
            }
        }
        static int iUpdateDay;
        static object CurLock = true;
        static void OpenFromDB(DateTime d, myRecordData item, CurveDataTableRow Row)
        {
            List<byte> streambytes = new List<byte>();
            if (!GWDataCenter.StationItem.bCurveStoreInDB)
            {
                string FullPathName;
                string FileName = "";
                bool bNewFile = false;
                FullPathName = Path.Combine(StationItem.CurveRootPathName, d.Year.ToString());
                FullPathName = Path.Combine(FullPathName, d.Month.ToString());
                FullPathName = Path.Combine(FullPathName, d.Day.ToString());
                if (item.type == "C")
                    FileName = string.Format("C_{0}_{1}.cur", item.iEqpNo, item.iYcYxNo);
                if (item.type == "X")
                    FileName = string.Format("X_{0}_{1}.cur", item.iEqpNo, item.iYcYxNo);
                if (!Directory.Exists(FullPathName))
                {
                    Directory.CreateDirectory(FullPathName);
                }
                if (!File.Exists(Path.Combine(FullPathName, FileName)))
                {
                    filestream = File.Create(Path.Combine(FullPathName, FileName), 64, FileOptions.Asynchronous);
                    filestream.Close();
                    bNewFile = true;
                }
                else
                {
                    bNewFile = false;
                }
                filestream = File.Open(Path.Combine(FullPathName, FileName), FileMode.Append, FileAccess.Write);
                if (bNewFile)
                {
                    byte[] f = new byte[] { (byte)DataCenter.CurveType.Double };
                    filestream.Write(f, 0, f.Count());
                }
            }
            else
            {
                strUpdateTableName = $"ZZCurveData{d.ToString("yyyyMMdd")}";
                if (!iNeedUpdateDays.Contains(d.Day))
                    iNeedUpdateDays.Enqueue(d.Day);
                iUpdateDay = d.Day;
                if (item.type == "C")
                    key = GetCurveKey(item.iEqpNo, item.iYcYxNo, "C");
                if (item.type == "X")
                    key = GetCurveKey(item.iEqpNo, item.iYcYxNo, "X");
                CreatCurveTableFromDate(d);
                streambytes.Clear();
                lock (CurLock)
                {
                    try
                    {
                        Row = DB4Day[d.Day].CurveDataTable.Single(m => (m.key == key));
                        streambytes = Row.curvedata.ToList();
                        streambytes.AddRange(item.DotValues);
                        Row.curvedata = streambytes.ToArray();
                    }
                    catch
                    {
                        streambytes.Add((byte)DataCenter.CurveType.Double);
                        streambytes.AddRange(item.DotValues);
                        Row = new CurveDataTableRow
                        {
                            key = key,
                            curvedata = streambytes.ToArray()
                        };
                        DB4Day[d.Day].CurveDataTable.Add(Row);
                    }
                }
            }
        }
        static void SaveChanges()
        {
            foreach (KeyValuePair<int, GWCurveContext> pair in DB4Day)
            {
                lock (GWCurveContext.lockstate)
                {
                    pair.Value.SaveChanges();
                }
            }
        }
        static void RecordCurve(myRecordData item)
        {
            CurveDataTableRow Row = null;
            DateTime d = DateTime.Now;
            if (!item.bSpanDay)
            {
                TimeSpan span = new TimeSpan(12, 0, 0);
                OpenFromDB((DateTime.Now - span).Date, item, Row);
            }
            else
            {
                OpenFromDB(DateTime.Now, item, Row);
            }
            if (!GWDataCenter.StationItem.bCurveStoreInDB)
            {
                if (filestream != null)
                {
                    filestream.Write(item.DotValues, 0, item.DotValues.Count());
                    filestream.Close();
                    filestream.Dispose();
                }
            }
        }
        static void DeleteCurve()
        {
            try
            {
                Properties properties = PropertyService.Get(StationItem.CurveProperty, new Properties());
                int iHistory_CurveStoreTime = Convert.ToInt32(properties.Get("History_CurveStoreTime", @"365"));
                string TableNm = $"ZZCurveData{DateTime.Now.AddDays(0 - iHistory_CurveStoreTime):yyyyMMdd}";
                if (GWDataCenter.StationItem.bCurveStoreInDB)
                {
                    lock (GWCurveContext.lockstate)
                    {
                        using (var db = new GWCurveContext())
                        {
                            int k = db.Database.ExecuteSqlRaw($"DROP TABLE {TableNm}");
                        }
                    }
                }
                else
                {
                    string CurveRootPathName, FullPathName;
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        CurveRootPathName = Path.Combine(General.GetApplicationRootPath(), "CurveData");
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        CurveRootPathName = properties.Get("Hostory_CurveStorePath", @"d:\AlarmCenter\CurveData");
                    DateTime d = DateTime.Now.AddDays(0 - iHistory_CurveStoreTime);
                    FullPathName = Path.Combine(StationItem.CurveRootPathName, d.Year.ToString());
                    FullPathName = Path.Combine(FullPathName, d.Month.ToString());
                    FullPathName = Path.Combine(FullPathName, d.Day.ToString());
                    if (Directory.Exists(FullPathName))
                    {
                        DirectoryInfo di = new DirectoryInfo(FullPathName);
                        di.Delete(true);
                    }
                }
            }
            catch (Exception e)
            {
            }
        }
        static int iOldHour = 0;
        public static void Run()
        {
            Task.Factory.StartNew(delegate
            {
                DeleteCurve();
            }, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(delegate
            {
                while (true)
                {
                    lock (Lock4RecordDataList)
                    {
                        tempRecordDataList.AddRange(RecordDataList);
                        RecordDataList.Clear();
                    }
                    if (tempRecordDataList.Count > 0)
                    {
                        try
                        {
                            iNeedUpdateDays.Clear();
#if DEBUG
                            Int64 startTime = Stopwatch.GetTimestamp();
#endif
                            foreach (myRecordData item in tempRecordDataList)
                            {
                                RecordCurve(item);
                            }
                            SaveChanges();
#if DEBUG
                            Console.WriteLine($"{DateTime.Now.ToString()}存储曲线{tempRecordDataList.Count}条总共耗时：");
                            Console.WriteLine((Stopwatch.GetTimestamp() - startTime) / (double)Stopwatch.Frequency);
#endif
                        }
                        catch (Exception e)
                        {
                            DataCenter.WriteLogFile("RecordCurve error>>" + General.GetExceptionInfo(e));
                        }
                        tempRecordDataList.Clear();
                    }
                    if (DateTime.Now.Hour < iOldHour)
                    {
                        Task.Factory.StartNew(delegate
                        {
                            DeleteCurve();
                        }, TaskCreationOptions.LongRunning);
                    }
                    Thread.Sleep(2000);
                    iOldHour = DateTime.Now.Hour;
                }
            }, TaskCreationOptions.LongRunning);
        }
        #endregion
    }
}
