//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using GWDataCenter.Database;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
namespace IoTCenterHost.AppServices.Domain.DO.Equip.Service
{
    public class CommandRepositoryImpl : ICommandRepository
    {
        private readonly object lockHelper = new object();
        public CommandRepositoryImpl()
        {

        }

        public GWDataContext CurrentDbContext
        {
            get
            {
                lock (lockHelper)
                {
                    return StationItem.MyGWDbProvider.serviceProvider.GetService<GWDataContext>(); ;
                }
            }
        }

        public void ResetEquipmentLinkage()
        {
            EquipmentLinkage.bReset = true;
        }
        public bool HaveSet(int EquipNo)
        {
            var setParmTableRows = CurrentDbContext.SetParmTable;
            return setParmTableRows.Count(u => u.equip_no == EquipNo) > 0;
        }
        public string GetSetListStr(int iEquipNo)
        {
            StringBuilder str = new StringBuilder();
            var setParmTableRows = CurrentDbContext.SetParmTable.Where(u => u.equip_no == iEquipNo);
            var list = setParmTableRows.Select(u => new string("[{" + u.set_nm + "}][{" + u.main_instruction + "}][{" + u.minor_instruction + "}][{" + u.value + "}]")).ToList();
            return string.Join(";", list).TrimEnd(';');
        }
    }
}
