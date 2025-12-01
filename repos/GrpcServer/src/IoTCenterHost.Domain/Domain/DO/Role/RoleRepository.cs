//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using GWDataCenter.Database;
using IoTCenterHost.AppServices.Domain.PO;
using IoTCenterHost.Core;
using IoTCenterHost.Core.Abstraction.IotModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IoTCenterHost.AppServices.Domain.DO.Role
{
    public class RoleRepository : IRoleRepository
    {
        private readonly GanweiDbContext _dataContext;
        private IServiceScopeFactory serviceScopeFactory;

        public RoleRepository(GanweiDbContext ganweiDb)
        {
            _dataContext = ganweiDb;
        }


        public IQueryable<GWRoleTableRow> Roles
        {
            get
            {
                return _dataContext.GWRoleTable.AsNoTracking();
            }
        }

        public void AddRole(GWRoleTableRow roleEntity)
        {
            try
            {
                DetachAll(_dataContext);
                _dataContext.GWRoleTable.Add(roleEntity);
                _dataContext.SaveChanges();
            }
            catch (Exception)
            {
            }
        }

        public int Count(string name)
        {
            return _dataContext.GWRoleTable.Count(u => u.Name == name);
        }

        public bool DeleteRole(string name)
        {
            var role = _dataContext.GWRoleTable.FirstOrDefault(u => u.Name == name);
            if (role != null)
            {
                _dataContext.GWRoleTable.Remove(role);
                _dataContext.SaveChanges();
            }
            return true;
        }

        public IEnumerable<RoleItem> GetRoleEntities(Pagination pagination, out int total)
        {
            Func<RoleItem, bool> expression = null;
            if (string.IsNullOrWhiteSpace(pagination.WhereCause))
            {
                expression = u => u != null;
            }
            else
                expression = u => u.name.Contains(pagination.WhereCause);
            var gwRoleTableRows = _dataContext.GWRoleTable.Where(p => p != null).ToList();
            var listResult = gwRoleTableRows
                .Select(
                   p => new RoleItem
                   {
                       name = p.Name,
                       remark = p.remark,
                       Control_Equip_List = string.IsNullOrEmpty(p.ControlEquips) ? null : p.ControlEquips.GetControlEquipList(),
                       Control_SetItem_List = string.IsNullOrEmpty(p.ControlEquips_Unit) ? null : p.ControlEquips_Unit.GetSetItemList(),
                       Browse_Equip_List = string.IsNullOrEmpty(p.BrowseEquips) ? null : p.BrowseEquips.GetBrowserEquipList(),
                       AddinModule_List = string.IsNullOrEmpty(p.SystemModule) ? null : p.SystemModule.GetModuleList(),
                       Browse_SpecialEquip_List = string.IsNullOrEmpty(p.SpecialBrowseEquip) ? null : p.SpecialBrowseEquip.GetSpecialEquipList(),
                       Browse_Pages_List = p == null || string.IsNullOrEmpty(p.BrowsePages) ? null : p.BrowsePages.Split('#', StringSplitOptions.None).Where(p => p != null).Select(u => int.Parse(u)).ToList()
                   }
               ).ToList().Where(expression);
            total = listResult.Count();
            return listResult.Skip((pagination.PageIndex - 1) * pagination.PageSize).Take(pagination.PageSize).ToList();
        }


        public bool ModifyRole(GWRoleTableRow roleEntity)
        {
            try
            {
                DetachAll(_dataContext);
                _dataContext.Update(roleEntity);
                _dataContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public void DetachAll(DbContext dbContext)
        {
            while (true)
            {
                var currentEntry = dbContext.ChangeTracker.Entries()?.FirstOrDefault();

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
    }
}
