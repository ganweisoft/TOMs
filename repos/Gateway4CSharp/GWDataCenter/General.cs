using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Reflection;
using System.IO;

namespace GWDataCenter
{
    public static class General
    {

        static public string CfgValue1 = "MicrosoftWPF4.0";//密钥
        static public string CfgValue2 = "MicrosoftWCF4.0";//初始向量

        public static DateTime Convert2DT(DateTime DT)
        {
            var dt =  new DateTime(DT.Year, DT.Month, DT.Day, DT.Hour, DT.Minute, DT.Second);
            return DateTime.SpecifyKind(dt, DateTimeKind.Local);
        }

        /// <summary>
        /// 正则表达式验证 字符串是否符合预期格式
        /// </summary>
        /// <param name="s">正则表达式</param>
        /// <param name="s1">目标字符串</param>
        /// <returns></returns>

        public static bool VerifyStringFormat(string s, string s1)
        {
            Regex regex1 = new Regex(s);
            if (regex1.IsMatch(s1))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 获取抛出异常的相关信息
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetExceptionInfo(Exception e)
        {
            string msg;
            msg = e.Message + e.Source + e.StackTrace;
            /*
            msg = msg.Replace('\'', '*');
            msg = msg.Replace('\"', '*');
            if (msg.Length > 250)
            {
                msg = msg.Substring(0, 250);
                msg +=  "...";
            }
             */
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

        public static string GetExecutingAssemblyFileName()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            // 获取程序集的位置
            string location = assembly.Location;
            // 从位置中提取文件名
            return Path.GetFileName(location);
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
            //网卡序列号
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
