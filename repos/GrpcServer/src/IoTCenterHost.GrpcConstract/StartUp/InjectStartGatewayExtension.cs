//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using IoTCenterHost.AppServices.Domain.PO;
using Microsoft.Extensions.DependencyInjection;

namespace IoTCenterHost.AppServices
{
    public static class InjectStartGatewayExtension
    {
        public static EventHandler GWDataCenterStartedEvent;
        public static void InjectStartGateway(IServiceCollection services, string language)
        {
            string logStr = "IoTCenter网关数据采集平台已启动.", logStrErr = "网关启动失败，异常信息：";
            if (string.Equals(language, "en-US"))
            {
                logStr = "IoTCenter Data Acquisition Platform has been launched.";
                logStrErr = "IoTCenter Launch Exception，Message：";
            }
            try
            {
                if (IsDbConnect(services))
                {
                    if (string.IsNullOrEmpty(logStr)) logStr = "IotCenter网关数据采集平台已启动.";

                    DataCenter.brunning = (object)false; //为避免网关无法启动，需设置为false
                    var task = Task.Factory.StartNew(() =>
                    {
                        GWDataCenter.DataCenter.Start();
                        Console.Out.WriteLine(logStr);
                        GWDataCenterStartedEvent?.Invoke(null, EventArgs.Empty);
                    }, creationOptions: TaskCreationOptions.AttachedToParent);
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine($"{logStrErr}{ex}");
                Environment.Exit(1);
            }
        }
        private static bool IsDbConnect(IServiceCollection services)
        {
            var dbContext = services.BuildServiceProvider().GetService<GanweiDbContext>();
            return dbContext.Database.CanConnect();
        }
    }
}
