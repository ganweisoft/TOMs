// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace Ganweisoft.IoTCenter.Module.Login;

public class LoginInputModel
{
    public string UserName { get; set; }

    public string Password { get; set; }

    public string VerificationKey { get; set; }

    public string VerificationCode { get; set; }
    public bool SIVC { get; set; }

    public VerificationType VerificationType { get; set; }
}

public enum VerificationType
{
    Code,

    Slide
}