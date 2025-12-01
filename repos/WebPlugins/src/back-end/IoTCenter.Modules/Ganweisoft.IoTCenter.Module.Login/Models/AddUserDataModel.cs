// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ganweisoft.IoTCenter.Module.Login;

public class AddUserDataModel
{
    [Required(ErrorMessage = "用户名不能为空")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "密码不能为空")]
    public string UserPwd { get; set; }

    public string Remark { get; set; }

    public int ControlLevel { get; set; }

    public bool IsAdministrator { get; set; }

    public List<string> RoleList { get; set; } = new List<string>();

    public List<string> HomePageList { get; } = new List<string>();

    public List<string> AutoInspectionPagesList { get; set; } = new List<string>();
}
