// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Ganweisoft.IoTCenter.Module.Login;

public class GetUsersDataModel
{
    [JsonProperty("role_List")]
    public List<UserRoleModel> role_List { get; set; } = new List<UserRoleModel>();
    [JsonProperty("id")]
    public int Id { get; set; }
    [JsonProperty("userName")]
    public string UserName { get; set; }
    [JsonProperty("controlLevel")]
    public int ControlLevel { get; set; }
    [JsonProperty("lockoutEnabled")]
    public bool? LockoutEnabled { get; set; }
    [JsonProperty("isAdministrator")]
    public bool IsAdministrator { get; set; }
    public string Remark { get; set; }
}

public class UserRoleModel
{
    [JsonProperty("ischeck")]
    public bool IsCheck { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
}
