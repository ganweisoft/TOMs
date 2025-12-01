namespace System;

/// <summary>
/// 配置参数
/// </summary>
public class WebApiConfigOptions
{
    public string IpAddress { get; set; }
    public string HttpPort { get; set; }
    public string HttpsPort { get; set; }
    public string SSLName { get; set; }
    public string SSLPassword { get; set; }
    public bool SSLAutoGenerate { get; set; }
    public bool CipherAdapterEnable { get; set; }
    public string IsManyLoginEnabled { get; set; }
    public bool RSAAutoGenerate { get; set; }
    public string RSAPadding { get; set; }
    public int TryConnectTime { get; set; }
}
