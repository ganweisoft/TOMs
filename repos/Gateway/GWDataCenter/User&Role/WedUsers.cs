﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using System.Collections.Generic;
namespace GWDataCenter
{
    public static class WebUserDictionary
    {
        static Dictionary<string, UserItem> WebUserDict = new Dictionary<string, UserItem>();
        static public UserItem GetWebUser(string Name)
        {
            if (WebUserDict.ContainsKey(Name))
                return WebUserDict[Name];
            return null;
        }
        static public void AddUser(string Name)
        {
            if (!WebUserDict.ContainsKey(Name))
                WebUserDict.Add(Name, DataCenter.GetUserItem(Name));
        }
        static public void RemoveUser(string Name)
        {
            if (WebUserDict.ContainsKey(Name))
                WebUserDict.Remove(Name);
        }
    }
}
