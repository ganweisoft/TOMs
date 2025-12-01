//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using IoTCenterHost.AppServices.Domain.DO.Role;
using IoTCenterHost.AppServices.Domain.PO;
using IoTCenterHost.Core;
using IoTCenterHost.Core.Abstraction;
using IoTCenterHost.Core.Abstraction.EnumDefine;
using IoTCenterHost.Core.Abstraction.IotModels;
using IoTCenterHost.Core.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;

namespace IoTCenterHost.AppServices.Domain.DO.User
{
    public class UserRepository : IUserRepository
    {
        private GanweiDbContext _dataContext;
        private IRoleRepository _roleRepository;
        private readonly object lockHelper = new object();
        private IServiceScopeFactory serviceScopeFactory;
        protected readonly IMemoryCacheService _memoryCacheService;

        public UserRepository(IRoleRepository roleRepository, IServiceScopeFactory serviceScope, IMemoryCacheService memoryCacheService, GanweiDbContext ganweiDb)
        {
            _roleRepository = roleRepository;
            serviceScopeFactory = serviceScope;
            _dataContext = ganweiDb;
            _memoryCacheService = memoryCacheService;
        }


        public IQueryable<UserEntity> Users
        {
            get
            {
                return _dataContext.GWUserTable.AsNoTracking();
            }
        }
        public IEnumerable<UserEntity> GetUserEntities(Expression<Func<UserEntity, bool>> expression)
        {
            lock (lockHelper)
                return Users.Where<UserEntity>(expression).Map<UserEntity>();
        }
        public Task<UserEntity> FirstOrDefault(Expression<Func<UserEntity, bool>> expression)
        {
            lock (lockHelper)
                return Users.FirstOrDefault(expression).Map<Task<UserEntity>>();
        }

        public Task<UserEntity> GetUserEntity(string uid)
        {
            return _dataContext.GWUserTable.Find(uid).Map<Task<UserEntity>>();
        }


        public bool CheckUserInfo(string username, string password)
        {
            lock (lockHelper)
            {
                return Users.AsNoTracking().ToList().Any(u => username == u.Name && password == u.Password);
            }
        }

        public async Task<UserEntity> GetUserEntity(string username, string password = null)
        {
            var userEntity = await GetOrSetCacheAsync(
                $"userinfo_{username}",
                async () =>
                {
                    var userEntity = await Users
                        .AsNoTracking()
                        .FirstOrDefaultAsync(u => u.Name == username);
                    return userEntity;
                },
                TimeSpan.FromSeconds(10)
            );

            if (userEntity == null) return null;

            if (password == null) return userEntity;

            return userEntity.Password == password ? userEntity : null;
        }
        public async Task<GWUserItem> GetUserItem(UserEntity userEntity, bool isClient = true, GwClientType gwClientType = GwClientType.Web)
        {
            var roleItems = await GetOrSetCacheAsync(
                "gw_role_items",
                async () =>
                {
                    var roles = await _roleRepository.Roles.Where(u => u != null).ToListAsync();
                    var items = roles.Select(p => new GWRoleItem
                    {
                        name = p.Name,
                        remark = p.remark,
                        Control_Equip_List = p.ControlEquips.GetControlEquipList(),
                        Control_SetItem_List = p.ControlEquips_Unit.GetSetItemList(),
                        Browse_Equip_List = p.BrowseEquips.GetBrowserEquipList(),
                        AddinModule_List = p.SystemModule.GetModuleList(),
                        Browse_SpecialEquip_List = p.SpecialBrowseEquip.GetSpecialEquipList(),
                    }).ToList();

                    items.Add(new GWRoleItem { name = "ADMIN" });
                    return items;
                },
                TimeSpan.FromSeconds(30)
            );

            var aesKey = DataCenter.GeneratAESKey();

            string Decrypt(string input) => string.IsNullOrWhiteSpace(input) ? "" : DataCenter.AESDecrypt(input, aesKey);

            var decryptedUserName = Decrypt(userEntity.Name);
            var decryptedPassword = Decrypt(userEntity.Password);
            var decryptedControlLevel = Decrypt(userEntity.ControlLevel);
            var decryptedRoles = Decrypt(userEntity.Roles);
            var decryptedHomePages = Decrypt(userEntity.HomePages);
            var decryptedAutoInspectionPages = Decrypt(userEntity.AutoInspectionPages);

            return new GWUserItem
            {
                ID = userEntity.ID,
                Remark = userEntity.Remark ?? "",
                UserName = decryptedUserName,
                UserPWD = decryptedPassword,
                ControlLevel = int.TryParse(decryptedControlLevel, out int level) ? level : 1,
                IsAdministrator = !string.IsNullOrWhiteSpace(decryptedRoles) && GetAdministrator(decryptedRoles),
                Role_List = string.IsNullOrWhiteSpace(decryptedRoles) ? null : GetRoleItems(decryptedRoles, roleItems),
                HomePage_List = string.IsNullOrWhiteSpace(decryptedHomePages) ? null : decryptedHomePages.GetHomePageList(),
                AutoInspectionPages_List = string.IsNullOrWhiteSpace(decryptedAutoInspectionPages) ? null : decryptedAutoInspectionPages.GetAutoInspectionPages()
            };
        }


        private T GetOrSetCache<T>(string key, Func<T> factory, TimeSpan expiration)
        {
            if (_memoryCacheService.IsSet(key))
                return _memoryCacheService.Get<T>(key);

            var value = factory();
            _memoryCacheService.Set(key, value, DateTimeOffset.Now.Add(expiration));
            return value;
        }

        private async Task<T> GetOrSetCacheAsync<T>(string key, Func<Task<T>> factory, TimeSpan expiration)
        {
            if (_memoryCacheService.IsSet(key))
                return _memoryCacheService.Get<T>(key);

            var value = await factory();
            _memoryCacheService.Set(key, value, DateTimeOffset.Now.Add(expiration));
            return value;
        }


        private static List<GWRoleItem> GetRoleItems(string roles, List<GWRoleItem> roleItems)
        {
            var result = new List<GWRoleItem>();
            try
            {
                var rolesArr = roles.Split('#');
                var list = rolesArr.Length > 0 ?
                                    rolesArr.
                                    AsEnumerable().Join(roleItems, s => s, a => a.name, (x, y) => new GWRoleItem
                                    {
                                        remark = y.remark,
                                        name = y.name,
                                        ischeck = y.ischeck,
                                        AddinModule_List = y.AddinModule_List,
                                        Browse_Equip_List = y.Browse_Equip_List,
                                        Browse_Pages_List = y.Browse_Pages_List,
                                        Browse_SpecialEquip_List = y.Browse_SpecialEquip_List,
                                        Control_Equip_List = y.Control_Equip_List,
                                        Control_SetItem_List = y.Control_SetItem_List,
                                    }).ToList()
                                    : new List<GWRoleItem>();
                if (rolesArr.Length == 1 && rolesArr[0].ToUpper() == "ADMIN")
                {
                    list.Add(new GWRoleItem() { name = "ADMIN" });
                }
                return list;
            }
            catch
            {
                throw;
            }
        }

        private static bool GetAdministrator(string roles)
        {
            try
            {
                var decryptedRoles = roles;
                var parts = decryptedRoles.Split('#');
                return parts.Length == 1 && parts[0].ToUpper() == "ADMIN";
            }
            catch
            {
                throw;
            }
        }

        public IEnumerable<GWUserItem> GetUserEntities(Pagination pagination, out int total)
        {
            lock (lockHelper)
            {
                var roleItems = _roleRepository.Roles.Where(u => u != null)
               .Select(
                   p => new GWRoleItem
                   {
                       name = p.Name,
                       remark = p.remark,
                       Control_Equip_List = p.ControlEquips.GetControlEquipList(),
                       Control_SetItem_List = p.ControlEquips_Unit.GetSetItemList(),
                       Browse_Equip_List = p.BrowseEquips.GetBrowserEquipList(),
                       AddinModule_List = p.SystemModule.GetModuleList(),
                       Browse_SpecialEquip_List = p.SpecialBrowseEquip.GetSpecialEquipList(),
                   }
               )
               .ToList();
                roleItems.Add(new GWRoleItem { name = "ADMIN" });
                Func<GWUserItem, bool> expression = null;
                string queryStr = pagination.WhereCause;
                if (string.IsNullOrWhiteSpace(pagination.WhereCause))
                {
                    expression = u => u != null;
                }
                else
                    expression = u => u.UserName.Contains(queryStr);

                var expressResult = Users.Select(p => new GWUserItem
                {
                    ID = p.ID,
                    Remark = p.Remark,
                    UserName = p.Name,
                    ControlLevel = string.IsNullOrEmpty(p.ControlLevel) ? 1 : int.Parse(p.ControlLevel),
                    IsAdministrator = GetAdministrator(p.Roles),
                    Role_List = GetRoleItems(p.Roles, roleItems),
                    HomePage_List = p.HomePages.GetHomePageList(),
                    AutoInspectionPages_List = p.AutoInspectionPages.GetAutoInspectionPages(),
                }).ToList().Where(expression);//;.Where(expression);
                total = expressResult.Count();
                return expressResult
                    .Skip((pagination.PageIndex - 1) * pagination.PageSize).Take(pagination.PageSize).ToList();
            }
        }
        public void AddUserEntity(UserEntity gWUserTableRow)
        {
            _dataContext.GWUserTable.Add(gWUserTableRow);
            _dataContext.SaveChanges();
        }


        public void DeleteUserEntity(int Id)
        {
            var entity = _dataContext.GWUserTable.Find(Id);
            if (entity != null)
            {
                _dataContext.GWUserTable.Remove(entity);
                _dataContext.SaveChanges();
            }
        }
        public void ModifyUserEntity(UserEntity userEntity)
        {
            try
            {
                DetachAll(_dataContext);
                var entity = _dataContext.GWUserTable.AsNoTracking().FirstOrDefault(u => u.ID == userEntity.ID);
                if (!string.IsNullOrEmpty(userEntity.Name))
                    entity.Name = userEntity.Name;
                if (!string.IsNullOrEmpty(userEntity.Password))
                    entity.Password = userEntity.Password;
                if (!string.IsNullOrEmpty(userEntity.Roles))
                    entity.Roles = userEntity.Roles;
                entity.AutoInspectionPages = userEntity.AutoInspectionPages;
                entity.Remark = userEntity.Remark;
                entity.ControlLevel = userEntity.ControlLevel;
                _dataContext.GWUserTable.Update(entity);
                _dataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public void DetachAll(DbContext dbContext)
        {
            while (true)
            {
                var currentEntry = dbContext.ChangeTracker.Entries().FirstOrDefault();

                if (currentEntry != null)
                {
                    currentEntry.State = EntityState.Detached;
                }
                else
                {
                    break;
                }
            }
        }

        public int Count(string userName)
        {
            return Users.Count(u => u.Name == userName);
        }

        public bool Any(string userName)
        {
            return Users.ToList().Any(u => u.Name == userName);
        }
        public bool RevisePassword(int userId, string password)
        {
            var userEntity = _dataContext.GWUserTable.AsNoTracking().FirstOrDefault(u => u.ID == userId);
            userEntity.Password = password;
            ModifyUserEntity(userEntity);
            return true;
        }
    }
}
