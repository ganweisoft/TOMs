//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.AppServices.Domain;

namespace IoTCenterHost.AppServices.Interfaces
{
    public interface IHostConfigurationAppService
    {
        PropertiesXml GetPropertiesXmlModel();

        void SetPropertiesXmlModel(PropertiesXml propertiesXmlModel);
    }
}
