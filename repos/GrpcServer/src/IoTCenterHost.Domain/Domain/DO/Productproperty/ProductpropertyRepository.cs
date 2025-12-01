//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.AppServices.Domain.PO;
using Microsoft.Extensions.DependencyInjection;

namespace IoTCenterHost.Domain.Domain.DO
{
    public class ProductpropertyRepository : IProductpropertyRepository
    {
        private GanweiDbContext _ganweiDbContext;
        private IServiceScopeFactory serviceScopeFactory;
        public ProductpropertyRepository(GanweiDbContext ganweiDbContext)
        {
            _ganweiDbContext = ganweiDbContext;
        }
        public void SaveChanges()
        {
            _ganweiDbContext.SaveChanges();
        }
        public void AddRange(List<Productproperty> productproperties)
        {
            _ganweiDbContext.AddRange(productproperties);
            _ganweiDbContext.SaveChanges();
        }

        public bool Any()
        {
            return _ganweiDbContext.ProductProperty.Any();
        }
    }
}
