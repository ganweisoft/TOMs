﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
namespace GWDataCenter
{
    public static class General
    {
        public static DateTime Convert2DT(DateTime DT)
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        }
        public static bool VerifyStringFormat(string s, string s1)
        {
            Regex regex1 = new Regex(s);
            if (regex1.IsMatch(s1))
                return true;
            else
                return false;
        }
        public static string GetExceptionInfo(Exception e)
        {
            string msg;
            msg = e.Message + e.Source + e.StackTrace;
            return msg;
        }
        public static string GetApplicationRootPath()
        {
            string ApplicationRootPath = null;
            try
            {
                Assembly exe = typeof(General).Assembly;
                ApplicationRootPath = Path.Combine(Path.GetDirectoryName(exe.Location), "..");
            }
            catch (Exception e)
            {
            }
            return ApplicationRootPath;
        }
        public static int GetDayOfWeek(DateTime t)
        {
            switch (t.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return 1;
                case DayOfWeek.Monday:
                    return 2;
                case DayOfWeek.Tuesday:
                    return 3;
                case DayOfWeek.Wednesday:
                    return 4;
                case DayOfWeek.Thursday:
                    return 5;
                case DayOfWeek.Friday:
                    return 6;
                case DayOfWeek.Saturday:
                    return 7;
                default:
                    return 1;
            }
        }
        static public string GetString1(string MacAddr)
        {
            string strID = "AlarmCenter1";
            strID += MacAddr;
            strID = strID.Replace(':', '8');
            strID = strID.Replace(' ', 'F');
            char[] charArray = strID.ToCharArray();
            Array.Reverse(charArray);
            strID = new string(charArray).Substring(0, 8);
            return strID;
        }
    }
}
