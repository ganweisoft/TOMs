// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Diagnostics;
string url = "https://localhost:44380";
int pingMax = 10;
int pingTimeout = 1;
Console.Out.WriteLine("准备启动IoTCenter，请稍后..");
bool isWebStarted = IsWebStarted(url);
while (!isWebStarted && pingTimeout < pingMax)
{
    isWebStarted = IsWebStarted(url);
    await Task.Delay(1000);
    pingTimeout += 1;
}
if (pingTimeout >= pingMax && !isWebStarted)
{
    Console.Out.WriteLine("IoTCenter启动失败，请检查【IoTCenterWeb】和【IoTCenterService】服务是否正常启动..");
    Console.Out.WriteLine("本窗口将在10秒后关闭...");
    await Task.Delay(10000);
}
else
{
    Console.Out.WriteLine("启动IoTCenter，请稍后..");
    Process.Start("explorer.exe", url);
}
Environment.Exit(0);


static bool IsWebStarted(string url)
{
    var handler = new HttpClientHandler();
    handler.ClientCertificateOptions = ClientCertificateOption.Manual;
    handler.ServerCertificateCustomValidationCallback =
        (httpRequestMessage, cert, cetChain, policyErrors) =>
    {
        return true;
    };
    var httpClient = new HttpClient(handler);
    httpClient.BaseAddress = new Uri(url);
    try
    {
        var result = httpClient.GetAsync(url).Result;
        return result.IsSuccessStatusCode;
    }
    catch (Exception ex)
    {
        Console.Out.WriteLine("连接失败，正在重试，请稍后...");
        return false;
    }
}