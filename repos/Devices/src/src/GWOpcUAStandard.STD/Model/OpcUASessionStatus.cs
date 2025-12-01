// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace GWOpcUAStandard.STD.Model
{
    public class OpcUASessionStatus
    {
        private int _timeOut;
        public OpcUASessionStatus(int timeOutSec = 10)
        {
            _timeOut = timeOutSec;
            SessionCreateTime = DateTime.Now;
        }
        public DateTime? LastKeepAliveTime { get; set; }
        public DateTime SessionCreateTime { get; set; }
        public bool GetTimeOutStatus()
        {
            var checkTime = LastKeepAliveTime ?? SessionCreateTime;
            return (DateTime.Now - checkTime) < TimeSpan.FromSeconds(_timeOut);
        }
        public void SetOnceKeepAliveTime()
        {
            LastKeepAliveTime = DateTime.Now;
        }
    }
}
