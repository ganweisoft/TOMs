// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace IoTCenterWebApi.BaseCore;

public class CustomFilterAttribute : ActionFilterAttribute
{
    private static Regex SqlTokenPattern { get; set; }
    private static StringBuilder PatternString { get; set; } = new StringBuilder();

    private const string StrKeyWord = @"select |insert |delete |from |count|drop table |update |truncate|asc|mid|char|xp_cmdshell|exec|exec master|netlocalgroup administrators|net user|or |and |like |waitfor delay |sleep";

    private readonly ILoggingService _apiLog;

    public CustomFilterAttribute(ILoggingService apiLog)
    {
        _apiLog = apiLog;
    }

    static CustomFilterAttribute()
    {
        string[][] search_regex_replacement = new string[10][];
        search_regex_replacement[0] = new string[] { "\u0000", "\\x00", "\\\\0" };
        search_regex_replacement[1] = new string[] { "'", "'", "\\\\'" };
        search_regex_replacement[2] = new string[] { "\"", "\"", "\\\\\"" };
        search_regex_replacement[3] = new string[] { "\b", "\\x08", "\\\\b" };
        search_regex_replacement[4] = new string[] { "\n", "\\n", "\\\\n" };
        search_regex_replacement[5] = new string[] { "\r", "\\r", "\\\\r" };
        search_regex_replacement[6] = new string[] { "\t", "\\t", "\\\\t" };
        search_regex_replacement[7] = new string[] { "\u001A", "\\x1A", "\\\\Z" };
        search_regex_replacement[8] = new string[] { "\\", "\\\\", "\\\\\\\\" };
        search_regex_replacement[9] = new string[] { "\\%", "\\%", "\\\\'" };

        foreach (var srr in search_regex_replacement)
        {
            PatternString.Append($"{(string.IsNullOrEmpty(PatternString.ToString()) ? "" : "|")}" +
                $"{srr[1]}");
        }
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context == null)
        {
            return;
        }

        var action = context.ActionDescriptor as ControllerActionDescriptor;
        if (action != null && action.MethodInfo.GetCustomAttributes(typeof(SkipCustomFilterAttribute), false).Any())
        {
            return;
        }

        #region SQL注入校验
        var parameters = context.ActionDescriptor.Parameters
               .Where(p => context.ActionArguments.ContainsKey(p.Name) && context.ActionArguments[p.Name] != null);

        foreach (var p in parameters)
        {
            string parameterName = p.Name;

            if (p.ParameterType == typeof(string))//如果参数是str类型
            {
                string value = context.ActionArguments[parameterName].ToString();
                if (Validate(value) || CheckKeyWord(value))
                {
                    context.Result = new JsonResult(new { code = 403, message = "请求参数存在非法字符!", data = false });
                    base.OnActionExecuting(context);
                }
            }
            else if (p.ParameterType.IsClass && p.ParameterType.Name != "List`1")//当参数是一个实体
            {
                PostModelFieldFilter(context, p.ParameterType, context.ActionArguments[parameterName]);
            }
        }

        base.OnActionExecuting(context);
        #endregion
    }

        /// <summary>
        /// 对实体处理,获取属性的值
        /// </summary>
        /// <returns></returns>
        private void PostModelFieldFilter(ActionExecutingContext context, Type type, object obj)
    {
        if (obj == null)
        {
            return;
        }

        try
        {
            var props = type.GetProperties();

            PostModelFieldFilterOne(context, obj, props);
        }
        catch (Exception ex)
        {
            _apiLog.Error("ErrorRequestFilterAttribute【参数过滤异常】：" + ex.ToString());
        }
    }

    private void PostModelFieldFilterOne(ActionExecutingContext context, object obj, PropertyInfo[] props)
    {
        foreach (var propertyInfo in props)//遍历属性
        {
            if (propertyInfo.Name == "Capacity")
            {
                break;
            }
            if (propertyInfo.PropertyType.Name == "Dictionary`2")
                continue;

            if (propertyInfo.PropertyType == typeof(string))
            {
                var value = propertyInfo.GetValue(obj);
                if (value == null)
                {
                    continue;
                }
                if (Validate(value.ToString()) || CheckKeyWord(value.ToString()))
                {
                    context.Result = new JsonResult(new { code = 400, message = "请求参数存在非法字符!", data = false });
                    base.OnActionExecuting(context);
                }
                else
                {
                    base.OnActionExecuting(context);
                }
            }
            else if (propertyInfo.PropertyType.IsClass && propertyInfo.PropertyType.Name != "List`1")//当属性是一个实体
            {
                PostModelFieldFilter(context, propertyInfo.PropertyType, propertyInfo.GetValue(obj));
            }
        }
    }

#pragma warning disable CA1822 // 将成员标记为 static
    public bool CheckKeyWord(string sWord)
#pragma warning restore CA1822 // 将成员标记为 static
    {
        if (string.IsNullOrEmpty(sWord?.Trim()))
        {
            return false;
        }

        var word = sWord.Trim();

        var matches = StrKeyWord.Split('|');

        foreach (string matche in matches)
        {
            word = Regex.Replace(sWord.Trim(), matche.Trim(), "", RegexOptions.IgnoreCase);
        }

        return !word.Equals(sWord.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private static bool Validate(string text)
    {
        SqlTokenPattern = new Regex(PatternString.ToString(),
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        var matches = SqlTokenPattern.Matches(text);

        return matches.Count > 0;
    }

    public bool CheckStringForSql(string[] strs)
    {
        var strOld = "";
        var strNew = "";
        foreach (var str in strs)
        {
            if (str == null)
            {
                break;
            }

            strOld = str.Trim();
            strNew = str.Trim();

            strNew = strNew.Replace("'", "");
            strNew = strNew.Replace(";", "");
            strNew = strNew.Replace(",", "");
            strNew = strNew.Replace("?", "");
            strNew = strNew.Replace("<", "");
            strNew = strNew.Replace(">", "");
            strNew = strNew.Replace("(", "");
            strNew = strNew.Replace(")", "");
            strNew = strNew.Replace("@", "");
            strNew = strNew.Replace("=", "");
            strNew = strNew.Replace("+", "");
            strNew = strNew.Replace("*", "");
            strNew = strNew.Replace("&", "");
            strNew = strNew.Replace("#", "");
            strNew = strNew.Replace("%", "");
            strNew = strNew.Replace("$", "");
            strNew = strNew.Replace("||", "");
            strNew = strNew.Replace("!", "");

            if (strOld != strNew)
            {
                return false;
            }
        }

        return true;
    }
}