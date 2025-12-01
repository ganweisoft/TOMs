//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.AppServices.Domain.PO;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;

namespace IoTCenterHost.AppServices.Domain.DO.GWServiceToken
{
    public class GWServiceTokenRepository : IGWServiceTokenRepository
    {
        private GanweiDbContext _ganweiDbContext;
        private IServiceScopeFactory serviceScopeFactory;
        public GWServiceTokenRepository(GanweiDbContext ganweiDb)
        {
            _ganweiDbContext = ganweiDb;
        }
        public IEnumerable<GWServiceToken> GetList(Expression<Func<GWServiceToken, bool>> expression)
        {
            return _ganweiDbContext.GWServiceToken.Where(expression);
        }
        public void AddEntity(GWServiceToken serviceToken)
        {
            _ganweiDbContext.GWServiceToken.Add(serviceToken);
            _ganweiDbContext.SaveChanges();
        }
    }
}
