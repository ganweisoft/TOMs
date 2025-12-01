//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.AppServices.Domain.DomainBase;

namespace IoTCenterHost.AppServices.Domain.DO.User
{
    public class UserEntity : EntityBase, IAggregateRoot
    {
        #region 私有字段 
        #endregion

        #region  构造方法
        public UserEntity()
        {

        }
        #endregion

        #region 公共属性 
        public int ID { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Roles { get; set; }
        public string? HomePages { get; set; }
        public string? AutoInspectionPages { get; set; }
        public string? Remark { get; set; }
        public string ControlLevel { get; set; }
        public bool? FirstLogin { get; set; }
        public string? HistoryPasswords { get; set; }
        public int? AccessFailedCount { get; set; }
        public bool? LockoutEnabled { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public DateTime? PwdUpdateTime { get; set; }
        public string SecurityStamp { get; set; }

        public DateTime? UseExpiredTime { get; set; }
        #endregion

        #region 方法
        #endregion
    }
}
