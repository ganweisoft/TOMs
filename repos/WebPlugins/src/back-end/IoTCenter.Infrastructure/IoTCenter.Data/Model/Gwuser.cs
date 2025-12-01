// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;

namespace IoTCenter.Data.Model
{
    public class Gwuser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Roles { get; set; }
        public string HomePages { get; set; }
        public string AutoInspectionPages { get; set; }
        public string Remark { get; set; }
        public string ControlLevel { get; set; }
        public bool? FirstLogin { get; set; }
        public string HistoryPasswords { get; set; }
        public int? AccessFailedCount { get; set; }
        public bool? LockoutEnabled { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public DateTime? PwdUpdateTime { get; set; }
        public string SecurityStamp { get; protected set; }
        public DateTime? UseExpiredTime { get; set; }


        public void SetSecurityStamp(string securityStamp)
        {
            SecurityStamp = securityStamp;
        }
    }
}
