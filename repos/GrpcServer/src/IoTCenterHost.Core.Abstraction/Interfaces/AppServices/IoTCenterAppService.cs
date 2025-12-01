//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace IoTCenterHost.Core.Abstraction.AppServices
{
    public interface IoTCenterAppService
    {
        string GetVersionInfo();


        void ResetProcTimeManage();
        void ResetGWDataRecordItems();
        void ResetDelayActionPlan();
        void MResetYcYxNo(int EquipNo, string sType, int YcYxNo);

        string GetPropertyFromPropertyService(string PropertyName, string NodeName, string DefaultValue);

        void SetPropertyToPropertyService(string PropertyName, string NodeName, string Value);

    }
}
