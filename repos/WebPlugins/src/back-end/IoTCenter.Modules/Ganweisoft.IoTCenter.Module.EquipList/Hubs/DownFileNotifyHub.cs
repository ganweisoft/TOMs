// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IoTCenterWebApi.Hubs
{
    [Authorize(AuthenticationSchemes = "Cookies")]
    public class DownFileNotifyHub : Hub
    {
        private readonly ILoggingService _apiLog;
        public DownFileNotifyHub(
            ILoggingService apiLog)
        {
            _apiLog = apiLog;
        }

        public override async Task OnConnectedAsync()
        {
            var userName = Context.GetHttpContext().User.FindFirst(ClaimTypes.Name)?.Value;
            var roleName = Context.GetHttpContext().User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(roleName) || string.IsNullOrEmpty(userName))
            {
                await Clients.Clients(Context.ConnectionId).SendAsync("error", "用户信息异常");
                await Clients.Clients(Context.ConnectionId).SendAsync("connectionSucceeded", false);
                return;
            }

            var cache = new UserBaseSession
            {
                UserName = userName,
                SignalRConnectId = Context.ConnectionId,
            };

            if (GlobalConst.DownloadFileSession.TryGetValue(userName, out var value))
            {
                value = cache;
            }
            else
            {
                GlobalConst.DownloadFileSession.TryAdd(userName, cache);
            }

            await Clients.Clients(Context.ConnectionId).SendAsync("connectionSucceeded", true);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _apiLog.Debug($"开始关闭连接,当前在线连接数:{GlobalConst.DownloadFileSession.Count}");

            try
            {
                if (GlobalConst.DownloadFileSession
                    .Any(d => d.Value.SignalRConnectId == Context.ConnectionId))
                {
                    var dfSession = GlobalConst.DownloadFileSession
                        .FirstOrDefault(d => d.Value.SignalRConnectId == Context.ConnectionId);

                    GlobalConst.DownloadFileSession.Remove(dfSession.Key, out _);
                }

                await base.OnDisconnectedAsync(exception).ConfigureAwait(false);

                _apiLog.Debug($"连接已经关闭,当前在线连接数:{GlobalConst.DownloadFileSession.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OnDisconnectedAsync SignalR断开连接失败,错误:{ex}");
            }
        }
    }
}
