//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace IoTCenterHost.Core.Abstraction
{
    public class SetParmRequestModel
    {
        public int m_iEquipNo { get; set; }
        public int m_iSetNo { get; set; }
        public string m_Value { get; set; }
        public string m_Response { get; set; }
        public bool m_bFinish { get; set; }
        public string m_GUID { get; set; }
    }
}
