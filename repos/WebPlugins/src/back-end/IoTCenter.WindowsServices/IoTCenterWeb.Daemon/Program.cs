// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.ServiceProcess;

CreateHostBuilder(args).Build().Run();

static IHostBuilder CreateHostBuilder(string[] args)
{
    return Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddHostedService<Worker>();
        })
        .UseWindowsService(opt => opt.ServiceName = "IoTCenterDaemon")
        .UseSystemd();

}

public class Worker : BackgroundService
{
    private readonly IHostEnvironment _environment;
    private readonly ILogger<Worker> _logger;
    public Worker()
    {

    }
    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        int waitTime = 10;
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("守护进程 正在运行 {time}", DateTimeOffset.Now);
            TestServiceIsRunning("IoTCenter");
            TestServiceIsRunning("IoTCenterWeb");
            await Task.Delay(waitTime * 1000, stoppingToken);
        }
    }

    private void TestServiceIsRunning(string serviceName)
    {
        try
        {
            var service = GetService(serviceName);
            if (service != null && service.Status == ServiceControllerStatus.Stopped)
            {
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running);
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation("守护进程获取当前服务{serviceName}失败，未找到指定的服务. {time}，错误内容{ex}", serviceName, DateTimeOffset.Now, ex);
        }

    }
    ServiceController GetService(string serviceName)
    {
        ServiceController[] services = ServiceController.GetServices();
        return services.FirstOrDefault(_ => _.ServiceName.ToLower() == serviceName.ToLower())!;
    }
    public override void Dispose()
    {
    }
}