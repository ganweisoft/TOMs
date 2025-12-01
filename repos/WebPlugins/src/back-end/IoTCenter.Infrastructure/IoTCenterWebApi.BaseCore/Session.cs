// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Security.Claims;

namespace System
{
    public sealed class Session
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly ClaimsPrincipal _user;

        public Session(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
            _user = accessor.HttpContext?.User;
        }


        private string GetClaim(string type) => _user?.FindFirst(d => d.Type == type)?.Value;

        public HttpContext HttpContext => _accessor.HttpContext;

        public int UserId
        {
            get
            {
                var userId = GetClaim("userid");
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return 0;
                }
                if (int.TryParse(userId, out var userid))
                {
                    return userid;
                }
                return 0;
            }
        }

        public string Sid => GetClaim(ClaimTypes.Sid);
        public string UserName => GetClaim(ClaimTypes.Name);
        public string RoleName => GetClaim(ClaimTypes.Role);
        public string OrganizationCode => GetClaim(OrgCode);

        public int PersonNo
        {
            get
            {
                var pid = GetClaim(Pid);
                if (string.IsNullOrWhiteSpace(pid))
                {
                    return 0;
                }
                if (int.TryParse(pid, out var userid))
                {
                    return userid;
                }
                return 0;
            }
        }

        public string PersonName => GetClaim(PName);
        public string UserGroupId => GetClaim(ClaimTypes.GroupSid);
        public bool IsAdmin => GetClaim(ClaimTypes.Role).ToLower(CultureInfo.CurrentCulture)
                     .Equals("admin", StringComparison.OrdinalIgnoreCase);
        public string IpAddress => HttpContext?.Connection?.RemoteIpAddress?.MapToIPv4()?.ToString();
        public string Port => HttpContext?.Connection?.RemotePort.ToString();

        public const string OrgCode = "OrganizationCode";
        public const string Pid = "PersonName";
        public const string PName = "PersonId";
    }
}
