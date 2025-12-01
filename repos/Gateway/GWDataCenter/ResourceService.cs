﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Collections.Generic;
using System.Xml;

namespace GWDataCenter
{
    public static class ResourceService
    {
        class ResourceItem
        {
            public string StringID { get; set; }
            public string zh { get; set; }
            public string ft { get; set; }
            public string en { get; set; }
            public string GetString(string Language)
            {
                if (Language == "zh-CN" || Language == "zh")
                    return zh;
                if (Language == "zh-HK" || Language == "ft")
                    return ft;
                if (Language == "en-US" || Language == "en")
                    return en;

                return null;
            }
        }
        const string uiLanguageProperty = "CoreProperties.UILanguage";
        static string cnnstr;
        static IDbConnection conn = null;
        static IDbCommand cmd = null;
        static Assembly ASM = null;
        static DataTable dt;
        static Dictionary<string, ResourceItem> ResourceDict = new Dictionary<string, ResourceItem>();
        public static string GetApplicationRootPath()
        {
            string ApplicationRootPath = null;
            try
            {
                Assembly exe = typeof(ResourceService).Assembly;
                ApplicationRootPath = Path.Combine(Path.GetDirectoryName(exe.Location), "..");
            }
            catch (Exception e)
            {
            }
            return ApplicationRootPath;
        }

        public static string Language
        {
            get
            {
                return PropertyService.Get(uiLanguageProperty, Thread.CurrentThread.CurrentUICulture.Name);
            }
            set
            {
                if (Language != value)
                {
                    PropertyService.Set(uiLanguageProperty, value);
                }
            }
        }

        public static void InitializeService()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Path.Combine(GetApplicationRootPath(), "bin/GWRES1.dll"));
            XmlNodeList NodeList = xmlDoc.SelectSingleNode("NewDataSet").ChildNodes;
            foreach (XmlElement oon in NodeList)
            {
                ResourceItem Item = new ResourceItem();
                Item.StringID = oon.GetElementsByTagName("StringID")[0].InnerText;
                try
                {
                    Item.zh = oon.GetElementsByTagName("zh-CN")[0].InnerText;
                }
                catch (Exception e)
                {
                    Item.zh = oon.GetElementsByTagName("zh")[0].InnerText;
                }
                try
                {
                    Item.ft = oon.GetElementsByTagName("zh-HK")[0].InnerText;
                }
                catch (Exception e)
                {
                    Item.ft = oon.GetElementsByTagName("ft")[0].InnerText;
                }
                try
                {
                    Item.en = oon.GetElementsByTagName("en-US")[0].InnerText;
                }
                catch (Exception e)
                {
                    Item.en = oon.GetElementsByTagName("en")[0].InnerText;
                }
                ResourceDict.Add(DataCenter.DecodeBase64(Item.StringID).Trim(), Item);
            }
        }
        public static string GetString(string name)
        {
            string s = "";
            try
            {
                if (ResourceDict.ContainsKey(name.Trim()))
                {
                    s = DataCenter.DecodeBase64(ResourceDict[name].GetString(ResourceService.Language));
                }

            }
            catch (Exception e)
            {
                s = name;
            }
            if (string.IsNullOrEmpty(s))
                s = name;
            return s;
        }

        public static string GetString(string name, string defaultStr)
        {
            string s = "";
            try
            {
                if (ResourceDict.ContainsKey(name.Trim()))
                {
                    s = DataCenter.DecodeBase64(ResourceDict[name].GetString(ResourceService.Language));
                }
                else
                {
                    s = defaultStr;
                }
            }
            catch (Exception e)
            {
                s = defaultStr;
            }
            if (string.IsNullOrEmpty(s))
                s = defaultStr;
            return s;
        }
    }
}
