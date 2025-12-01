//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace IoTCenterHost.Core.Abstraction
{
    public class WcfZCItem
    {
        string m_ZCID;
        string m_ZCName;
        int m_ZCDay;


        public string ZCID
        {
            get
            {
                return m_ZCID;
            }
            set
            {
                m_ZCID = value;
            }
        }
        public string ZCName
        {
            get
            {
                return m_ZCName;
            }
            set
            {
                m_ZCName = value;
            }
        }
        public int ZCDay
        {
            get
            {
                return m_ZCDay;
            }
            set
            {
                m_ZCDay = value;
            }
        }
    }
}
