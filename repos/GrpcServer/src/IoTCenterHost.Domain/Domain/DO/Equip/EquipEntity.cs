//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.AppServices.Domain.DO.Equip;
using IoTCenterHost.AppServices.Domain.DomainBase;

namespace IoTCenterHost.AppServices.Domain.Entity
{
    public class EquipEntity : EntityBase, IAggregateRoot
    {

        #region 私有字段
        #endregion

        #region  构造方法
        public EquipEntity()
        {

        }
        #endregion

        #region 公共属性
        public int AutoId { get; set; }
        public DriverInfo DriverInfo { get; set; }
        public CommStatus CommStatus { get; set; }
        public int equip_no { get; set; }
        public string equip_nm { get; set; }
        public string equip_detail { get; set; }
        public string related_pic { get; set; }
        public int acc_cyc { get; set; }
        public string tabname { get; set; }
        public int alarm_scheme { get; set; }
        public int attrib { get; set; }
        public string sta_IP { get; set; }
        public int AlarmRiseCycle { get; set; }
        public string Reserve1 { get; set; }
        public string Reserve2 { get; set; }
        public string Reserve3 { get; set; }
        public string related_video { get; set; }
        public string ZiChanID { get; set; }
        public string PlanNo { get; set; }
        public string SafeTime { get; set; }
        public string 字段1 { get; set; }
        public string 字段2 { get; set; }
        #endregion

        #region 方法
        #endregion

    }
}
