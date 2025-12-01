//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace IoTCenterHost.Domain.Domain.DO
{
    public interface IProductpropertyRepository
    {
        void SaveChanges();

        void AddRange(List<Productproperty> gWPluginInformation);

        bool Any();
    }
}
