//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace IoTCenterHost.Core.Abstraction
{
    public class GWUserVO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Roles { get; set; }
        public string HomePages { get; set; }
        public string AutoInspectionPages { get; set; }
        public string Remark { get; set; }
        public string ControlLevel { get; set; }
    }
}
