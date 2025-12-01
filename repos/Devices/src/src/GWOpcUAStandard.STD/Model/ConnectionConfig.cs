// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWOpcUAStandard.STD.Model;

public class ConnectionConfig
{
    public string ServerUrl { get; set; }
    public bool Polling { get; set; } = false;
    public int PollingSleepTime { get; set; } = 1000;
    public ConnectionType ConnectionType { get; set; } = ConnectionType.Anonymous;
    public string UserName { get; set; }
    public string Password { get; set; }
    public string CertificatePath { get; set; }

    public string CertificatePwd { get; set; }
    public string CertificateName { get; set; }
    public List<NodeQualityType> NodeQualityType { get; set; }
}
