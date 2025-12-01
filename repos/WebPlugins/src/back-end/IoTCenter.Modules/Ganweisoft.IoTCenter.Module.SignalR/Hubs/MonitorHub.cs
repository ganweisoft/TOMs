// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using IoTCenterHost.Core.Abstraction.Services;
using IoTCenterWebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.SignalR
{
    [Authorize(AuthenticationSchemes = "Bearer,Cookies")]
    public class MonitorHub : Hub
    {
        private readonly IotCenterHostService _alarmCenterService;
        private readonly ILoggingService _apiLog;

        public MonitorHub(IotCenterHostService alarmCenterService, ILoggingService apiLog)
        {
            _alarmCenterService = alarmCenterService;
            _apiLog = apiLog;
        }

        public async Task OnConnectMultipleEquipNo(string equipNo)
        {
            var accessToken = Context.GetHttpContext().Request.Cookies["x-access-token"];

            if (string.IsNullOrEmpty(accessToken))
            {
                await Clients.Clients(Context.ConnectionId).SendAsync("SignalR connect fail,token is invalid").ConfigureAwait(false);
                return;
            }

            if (string.IsNullOrEmpty(equipNo))
            {
                await Clients.Clients(Context.ConnectionId).SendAsync("SignalR connect fail,equipNo is invalid").ConfigureAwait(false);
                return;
            }

            var equipNos = equipNo.Split(',', StringSplitOptions.RemoveEmptyEntries);

            if (equipNos.Length <= 0)
            {
                await Clients.Clients(Context.ConnectionId).SendAsync("SignalR connect fail,equipNo is invalid").ConfigureAwait(false);
                return;
            }

            try
            {
                var userName = Context.GetHttpContext().User?.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(userName))
                {
                    await Clients.Clients(Context.ConnectionId).SendAsync("SignalR连接失败，请核查Token是否过期或非法").ConfigureAwait(false);
                    return;
                }

                var sessions = new List<UserSession>();

                foreach (var n in equipNos)
                {
                    if (!int.TryParse(n, out var no))
                    {
                        continue;
                    }

                    await Groups.AddToGroupAsync(Context.ConnectionId, no.ToString()).ConfigureAwait(false);

                    await Clients.Clients(Context.ConnectionId).SendAsync("Signalr连接成功").ConfigureAwait(false);

                    var session = new UserSession()
                    {
                        Token = accessToken,
                        UserName = userName,
                        SignalRConnectId = Context.ConnectionId,
                        DeviceSubscribed = no
                    };

                    sessions.Add(session);
                }

                if (sessions.Count <= 0)
                {
                    return;
                }

                if (GlobalConst.UserSessions.ContainsKey(accessToken))
                    GlobalConst.UserSessions[accessToken] = sessions;
                else
                    GlobalConst.UserSessions.Add(accessToken, sessions);

            }
            catch (Exception ex)
            {
                _apiLog.Error("On Connect SignalR Throw Exception:" + ex.ToString());
                await Clients.Clients(Context.ConnectionId).SendAsync($"SignalR连接失败,错误{ex.Message}").ConfigureAwait(false);
            }
        }

        public async Task OnConnectWithToken(string accessToken, int equipNo)
        {

            if (string.IsNullOrEmpty(accessToken) || accessToken.Length <= "Bearer ".Length)
            {
                await Clients.Clients(Context.ConnectionId).SendAsync("SignalR connect fail,token is invalid").ConfigureAwait(false);
                return;
            }

            try
            {
                var userName = Context.GetHttpContext().User?.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrWhiteSpace(userName))
                {
                    await Clients.Clients(Context.ConnectionId).SendAsync("SignalR连接失败，请核查Token是否过期或非法").ConfigureAwait(false);
                    return;
                }

                await Groups.AddToGroupAsync(Context.ConnectionId, Convert.ToString(equipNo)).ConfigureAwait(false);
                await Clients.Clients(Context.ConnectionId).SendAsync("Sigalr连接成功").ConfigureAwait(false);

                var sessions = new List<UserSession>()
                {
                    new UserSession()
                    {
                        Token = accessToken,
                        UserName = userName,
                        SignalRConnectId = Context.ConnectionId,
                        DeviceSubscribed = equipNo
                    }
                };

                if (GlobalConst.UserSessions.ContainsKey(accessToken))
                    GlobalConst.UserSessions[accessToken] = sessions;
                else
                    GlobalConst.UserSessions.Add(accessToken, sessions);
            }
            catch (Exception ex)
            {
                _apiLog.Error("On Connect SignalR Throw Exception:" + ex.ToString());
                await Clients.Clients(Context.ConnectionId).SendAsync($"SignalR连接失败,错误{ex.Message}").ConfigureAwait(false);
            }
        }

        public async Task OnConnect(int equipNo)
        {
            var accessToken = Context.GetHttpContext().Request.Cookies["x-access-token"];

            if (string.IsNullOrEmpty(accessToken))
            {
                await Clients.Clients(Context.ConnectionId).SendAsync("SignalR connect fail,token is invalid").ConfigureAwait(false);

                _apiLog.Warn("SignalR connect fail, token is invalid");
                return;
            }

            try
            {
                var userName = Context.GetHttpContext().User?.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrWhiteSpace(userName))
                {
                    await Clients.Clients(Context.ConnectionId).SendAsync("SignalR连接失败，请核查Token是否过期或非法").ConfigureAwait(false);
                    return;
                }

                await Groups.AddToGroupAsync(Context.ConnectionId, Convert.ToString(equipNo)).ConfigureAwait(false);
                await Clients.Clients(Context.ConnectionId).SendAsync("Sigalr连接成功").ConfigureAwait(false);

                var sessions = new List<UserSession>()
                {
                    new UserSession()
                    {
                        Token = accessToken,
                        UserName = userName,
                        SignalRConnectId = Context.ConnectionId,
                        DeviceSubscribed = equipNo
                    }
                };

                if (GlobalConst.UserSessions.ContainsKey(accessToken))
                    GlobalConst.UserSessions[accessToken] = sessions;
                else
                    GlobalConst.UserSessions.Add(accessToken, sessions);
            }
            catch (Exception ex)
            {
                _apiLog.Error("On Connect SignalR Throw Exception:" + ex.ToString());
                await Clients.Clients(Context.ConnectionId).SendAsync($"SignalR连接失败,错误{ex.Message}").ConfigureAwait(false);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _apiLog.Debug($"开始关闭连接,当前在线连接数:{GlobalConst.UserSessions.Count}");

            try
            {
                foreach (var v in GlobalConst.UserSessions.Values)
                {
                    var session = v.FirstOrDefault(s => s.SignalRConnectId == Context.ConnectionId);

                    if (session == null)
                    {
                        continue;
                    }

                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, session.DeviceSubscribed.ToString()).ConfigureAwait(false);

                    GlobalConst.UserSessions.Remove(session.Token);
                }

                await base.OnDisconnectedAsync(exception).ConfigureAwait(false);

                _apiLog.Debug($"连接已经关闭,当前在线连接数:{GlobalConst.UserSessions.Count}");
            }
            catch (Exception ex)
            {
                _apiLog.Error($"OnDisconnectedAsync SignalR断开连接失败,错误:{ex.ToString()}");
            }
        }

        public async Task SendCommand(int equipNo, int setNo, string value)
        {
            try
            {
                Parallel.ForEach(GlobalConst.UserSessions.Values, async (v) =>
                {
                    var session = v.FirstOrDefault(s => s.SignalRConnectId == Context.ConnectionId);

                    if (session == null)
                    {
                        await Clients.Clients(Context.ConnectionId).SendAsync($"SignalR 登录信息丢失,请重新连接").ConfigureAwait(false);
                        return;
                    }

                    _alarmCenterService.SetParm1_1(equipNo, setNo, value, session.UserName, true);

                    await Clients.Clients(Context.ConnectionId).SendAsync("已执行命令!").ConfigureAwait(false);
                });

            }
            catch (Exception ex)
            {
                _apiLog.Error("SendCommand【客户端执行指令通知服务端】:" + ex.ToString());
            }
            await Task.CompletedTask;
        }

        public async Task AddUserToGroup()
        {
            var accessToken = Context.GetHttpContext().Request.Cookies["x-access-token"];

            if (string.IsNullOrEmpty(accessToken))
            {
                await Clients.Clients(Context.ConnectionId).SendAsync("SignalR connect fail,token is invalid").ConfigureAwait(false);

                _apiLog.Warn("SignalR connect fail, token is invalid");
                return;
            }

            try
            {
                var userName = Context.GetHttpContext().User?.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrWhiteSpace(userName))
                {
                    await Clients.Clients(Context.ConnectionId).SendAsync("SignalR连接失败，请核查Token是否过期或非法").ConfigureAwait(false);
                    return;
                }
                await Groups.AddToGroupAsync(Context.ConnectionId, userName).ConfigureAwait(false);
                await Clients.Clients(Context.ConnectionId).SendAsync("Sigalr连接成功").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _apiLog.Error("On Connect SignalR Throw Exception:" + ex.ToString());
                await Clients.Clients(Context.ConnectionId).SendAsync($"SignalR连接失败,错误{ex.Message}").ConfigureAwait(false);
            }
        }
    }
}
