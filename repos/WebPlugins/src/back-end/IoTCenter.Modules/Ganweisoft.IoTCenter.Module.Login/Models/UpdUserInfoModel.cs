// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace Ganweisoft.IoTCenter.Module.Login;

public class UpdUserInfoModel
{
    public string userName { get; set; }

    public string oldPassWord { get; set; }

    public string newPassWord { get; set; }
}

public class ResetUserPwdModel
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }

    public string OldPassWord { get; set; }
}
