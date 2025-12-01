//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.AppServices.Domain;
using IoTCenterHost.AppServices.Interfaces;
using Microsoft.Extensions.Localization;

namespace IoTCenterHost.AppServices.Application
{
    public class HostConfigurationAppServiceImpl : IHostConfigurationAppService
    {
        private readonly IStringLocalizer<LocalizationResource> _localizer;

        public HostConfigurationAppServiceImpl(IStringLocalizer<LocalizationResource> localizer)
        {
            _localizer = localizer;
        }

        const int DEFAULTSAFETYLEVEL = 1;
        public PropertiesXml GetPropertiesXmlModel()
        {
            return PropertiesXml.Default;
        }
        public void SetPropertiesXmlModel(PropertiesXml propertiesXmlModel)
        {
            PropertiesXml.Save(propertiesXmlModel);
        }

    }
}
