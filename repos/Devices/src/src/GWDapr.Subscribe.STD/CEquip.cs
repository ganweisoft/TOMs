// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using GWDataCenter.Database;

namespace GWDapr.Subscribe.STD
{
    public class CEquip : CEquipBase
    {
        /// <summary>
        /// 是否初始化过
        /// </summary>
        private bool _isInit = false;

        public override bool init(EquipItem item)
        {
            try
            {
                if (!base.init(item)) return false;
                if (!_isInit || ResetFlag)
                {
                    _isInit = true;
                }
            }
            catch (Exception e)
            {
                DataCenter.WriteLogFile($"初始化异常：{e}");
                return false;
            }

            return _isInit;
        }

        public override bool GetYC(YcpTableRow r)
        {
            return true;
        }

        public override bool GetYX(YxpTableRow r)
        {
            return true;
        }
    }
}
