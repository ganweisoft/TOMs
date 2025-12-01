//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.Core.Abstraction;
using Microsoft.Extensions.Configuration;

namespace IoTCenterHost.AppServices.Interfaces.Token
{
    public abstract class BaseTokenServiceImpl
    {

        #region 私有字段 
        protected const int EmpiredSeconds = 10 * 60 * 1000;//设置为10分钟
        protected const string TokenMark = "UserToken";
        protected readonly IMemoryCacheService _memoryCacheService;
        protected readonly IConfiguration _configuration;
        #endregion

        #region  构造方法
        public BaseTokenServiceImpl(IMemoryCacheService memoryCache, IConfiguration configuration)
        {
            _memoryCacheService = memoryCache;
            _configuration = configuration;
        }
        #endregion

        #region 公共属性  

        #endregion

        #region 接口方法
        public void SetTokenEmpired(string token)
        {
            RemoveTokenInMemory(token);
        }
        public bool IsTokenAlive(string token)
        {
            return _memoryCacheService.IsSet($"{TokenMark}:{token}");
        }
        public LoginUser GetLoginUser(string token)
        {
            return _memoryCacheService.Get<LoginUser>($"{TokenMark}:{token}");
        }
        public void SetTokenToMemory<T>(string token, T obj)
        {
            _memoryCacheService.Set($"{TokenMark}:{token}", obj, System.DateTime.Now.AddDays(EmpiredSeconds));
        }
        #endregion
        #region 方法 

        protected void RemoveTokenInMemory(string token)
        {
            _memoryCacheService.Remove($"{TokenMark}:{token}");
        }
        #endregion

    }
}
