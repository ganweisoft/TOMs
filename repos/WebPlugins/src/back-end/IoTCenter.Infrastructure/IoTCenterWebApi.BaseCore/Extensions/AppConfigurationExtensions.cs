// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace IoTCenterWebApi.BaseCore;

public static class AppConfigurationExtensions
{
    public static void SetAppSettingValue(string section, string key, string value, string appSettingsJsonFilePath = null)
    {
        if (string.IsNullOrEmpty(appSettingsJsonFilePath))
        {
            appSettingsJsonFilePath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        }

        var json = File.ReadAllText(appSettingsJsonFilePath);
        dynamic jsonObj = JsonConvert.DeserializeObject<JObject>(json);

        if (!string.IsNullOrEmpty(key))
        {
            jsonObj[section][key] = value;
        }
        else
        {
            jsonObj[section] = value;
        }
        string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
        using var streamWriter = new StreamWriter(appSettingsJsonFilePath, false);
        streamWriter.Write(output);
    }
}
