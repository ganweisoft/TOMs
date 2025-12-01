﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using GWDataCenter.Database;
using System;
namespace GWDataCenter
{
    public class SleepSetItem : ISetItem
    {
        public int ID;
        public int SleepSeconds = 0;
        public bool IsExcute = false;
        public DateTime StartValidTime, EndValidTime;
        public bool bCycleMustFinish = false;
        object btemporarilybreak = false;
        public bool m_bTemporarilyBreak
        {
            get
            {
                lock (btemporarilybreak)
                {
                    return (bool)btemporarilybreak;
                }
            }
            set
            {
                lock (btemporarilybreak)
                {
                    btemporarilybreak = value;
                }
            }
        }
        public SleepSetItem(object r)
        {
            if (r is GWProcCycleTableRow)
            {
                GWProcCycleTableRow Row = r as GWProcCycleTableRow;
                ID = 0;
                string T = Row.SleepUnit.ToUpper().Trim();
                int num = Row.SleepTime;
                if (T == "S")
                    SleepSeconds = num;
                if (T == "M")
                    SleepSeconds = num * 60;
                if (T == "H")
                    SleepSeconds = num * 60 * 60;
            }
        }
        public bool DoSetItem()
        {
            DateTime StartDoTime;
            StartDoTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            while (((DateTime.Now - StartDoTime).TotalMilliseconds / 100) < SleepSeconds * 10)
            {
                if ((bool)btemporarilybreak == true)
                    return false;
                if (CyclePlanItem.InValidTime(StartValidTime, EndValidTime) || bCycleMustFinish)
                {
                    System.Threading.Thread.Sleep(100);
                }
                else
                    return false;
            }
            return true;
        }
    }
}
