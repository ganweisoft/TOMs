//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using GWDataCenter.Database;
using IoTCenterHost.AppServices.Domain.DO.Equip.Service;
using IoTCenterHost.AppServices.Interfaces;
using IoTCenterHost.Core.Abstraction;
using IoTCenterHost.Core.Abstraction.AppServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IoTCenterHost.AppServices.AppServices
{
    public class CommandAppServiceImpl : BaseAppServiceImpl, ICommandAppService
    {
        private readonly IotRealTimeDataService iotRealTimeDataService;
        private readonly ICommandRepository commandRepository;
        private readonly GWDataContext _gWDataContext;
        private readonly object lockHelper = new object();
        private readonly ILogger<CommandAppServiceImpl> _logger;

        public CommandAppServiceImpl(IotRealTimeDataService iotRealTime,
            ICommandRepository commandRepo,
            IHttpContextAccessor contextAccessor,
            GWDataContext gWDataContext,
            ILogger<CommandAppServiceImpl> logger) : base(contextAccessor, logger)
        {
            iotRealTimeDataService = iotRealTime;
            commandRepository = commandRepo;
            _gWDataContext = gWDataContext;
            if (_gWDataContext != null && StationItem.db_Setparm == null)
            {
                try
                {
                    var result = _gWDataContext.SetParmTable;
                    if (result.Any())
                        StationItem.db_Setparm = result.ToList();
                }
                catch (Exception ex)
                {
                    DataCenter.WriteLogFile($"获取设置表内容失败：{ex}");
                }

            }
            _logger = logger;
        }

        public void DoSetParmFromString(string csParmStr)
        {
            iotRealTimeDataService.GetRealTimeDataItem().DoSetParmFromString(csParmStr);
        }

        public bool HaveSet(int EquipNo)
        {
            return commandRepository.HaveSet(EquipNo);
        }

        public void SetParm(int EquipNo, string strCMD1, string strCMD2, string strCMD3, string strUser)
        {
            iotRealTimeDataService.GetRealTimeDataItem().SetParm(EquipNo, strCMD1, strCMD2, strCMD3, strUser, GetLoginUser().LoginMark);
        }

        public void SetParm(int iEquipNo, string strCMD1, string strCMD2, string strCMD3, string strUser, string Instance_GUID)
        {
            iotRealTimeDataService.GetRealTimeDataItem().SetParm(iEquipNo, strCMD1, strCMD2, strCMD3, strUser, GetLoginUser().LoginMark);
        }
        public Task SetParmExAsync(int EquipNo, string strCMD1, string strCMD2, string strCMD3, string strUser, string requestId, Action<string> action)
        {
            iotRealTimeDataService.GetRealTimeDataItem().SetParmEx(EquipNo, strCMD1, strCMD2, strCMD3, strUser, GetLoginUser().LoginMark, requestId, action);
            return Task.CompletedTask;
        }

        public void SetParm1(int EquipNo, int SetNo, string strUser)
        {
            iotRealTimeDataService.GetRealTimeDataItem().SetParm1(EquipNo, SetNo, strUser, GetLoginUser().LoginMark);
        }

        public void SetParm1_1(int EquipNo, int SetNo, string strValue, string strUser, bool bShowDlg, string requestId = "")
        {
            var logMark = GetLoginUser()?.LoginMark;
            if (string.IsNullOrEmpty(logMark))
            {
                throw new Exception("获取用户信息出现异常");
            }

            var realTimeDataItem = iotRealTimeDataService.GetRealTimeDataItem();
            if (realTimeDataItem != null)
            {
                iotRealTimeDataService.CreateRealDataItem();
                realTimeDataItem = iotRealTimeDataService.GetRealTimeDataItem();
            }
            realTimeDataItem.SetParm1_1(EquipNo, SetNo, strValue, strUser, bShowDlg, logMark, requestId);
        }

        public void SetParm2(int EquipNo, string strCMD1, string strCMD2, string strCMD3, string strType, string strUser)
        {
            iotRealTimeDataService.GetRealTimeDataItem().SetParm2(EquipNo, strCMD1, strCMD2, strCMD3, strType, strUser, GetLoginUser().LoginMark);
        }

        public void SetParm2_1(int EquipNo, string strCMD1, string strCMD2, string strCMD3, string strType, string strUser, bool bShowDlg)
        {
            iotRealTimeDataService.GetRealTimeDataItem().SetParm2_1(EquipNo, strCMD1, strCMD2, strCMD3, strType, strUser, bShowDlg, GetLoginUser().LoginMark);
        }

        public void SetParm_1(int EquipNo, string strCMD1, string strCMD2, string strCMD3, string strUser, bool bShowDlg)
        {
            iotRealTimeDataService.GetRealTimeDataItem().SetParm_1(EquipNo, strCMD1, strCMD2, strCMD3, strUser, bShowDlg, GetLoginUser().LoginMark);
        }
        public string GetSetListStr(int iEquipNo)
        {
            return commandRepository.GetSetListStr(iEquipNo);
        }
        public GWDataCenter.SetParmItem[] SetParmArrayWaitReturn(GWDataCenter.SetParmItem[] Items, string strUser)
        {
            int EquipNo, SetNo;
            string strValue;
            bool bShowDlg;
            List<SetItem> SetItemList = new List<SetItem>();
            foreach (GWDataCenter.SetParmItem i in Items)
            {
                EquipNo = i.iEquipNo;
                SetNo = i.iSetNo;
                strValue = i.strValue;
                bShowDlg = i.bShowDlg;
                var r = DataCenter.GetDataRowFromSetParm(EquipNo, SetNo);
                string strCMD1, strCMD2, strCMD3;
                if (r != null)
                {
                    try
                    {
                        strCMD1 = Convert.ToString(r.main_instruction);
                        strCMD2 = Convert.ToString(r.minor_instruction);
                        if (string.IsNullOrEmpty(strValue))
                            strCMD3 = Convert.ToString(r.value);
                        else
                            strCMD3 = strValue;
                        SetItem item = new SetItem(EquipNo, SetNo, strCMD1, strCMD2, strCMD3, strUser);
                        item.IsWaitSetParm = true;
                        SetItemList.Add(item);
                        if (EquipNo == -1)//场景
                        {
                        }
                        else
                        {
                            item.Client_Instance_GUID = base.GetLoginUser().LoginMark;
                            item.bShowDlg = bShowDlg;
                            StationItem.GetEquipItemFromEquipNo(EquipNo).AddSetItem(item);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            while (true)//等待所有设置执行完成
            {
                bool bFinishSetParm = true;//是否完成所有设置 
                foreach (SetItem item in SetItemList)
                {
                    if (item.WaitSetParmIsFinish == null)
                        bFinishSetParm = false;
                    Task.Delay(10);
                }
                if (bFinishSetParm)
                    break;
            }
            List<GWDataCenter.SetParmItem> ListSetParmItem = new List<GWDataCenter.SetParmItem>();
            foreach (GWDataCenter.SetItem item in SetItemList)
            {
                if (item.WaitSetParmIsFinish == false)
                    ListSetParmItem.Add(new GWDataCenter.SetParmItem() { iEquipNo = item.EquipNo, iSetNo = item.m_SetNo, strValue = item.Value, bShowDlg = false });
            }
            return ListSetParmItem.ToArray();//返回的是设置失败的命令
        }

        public Task FireSetParmResponseEvent(int iEquipNo, int iSetNo, string Value, string Response, bool bFinish, string clientInstanceId, Action<SetParmRequestModel> action)
        {
            iotRealTimeDataService.GetRealTimeDataItem().FireSetParmResponseEvent(iEquipNo, iSetNo, Value, Response, bFinish, clientInstanceId, action);
            return Task.CompletedTask;
        }

        public string DoEquipSetItem(int EquipNo, int SetNo, string strValue, string strUser, bool bShowDlg, string Instance_GUID, string requestId = "")
        {
            if (string.IsNullOrEmpty(Instance_GUID))
                Instance_GUID = GetLoginUser().LoginMark;
            return iotRealTimeDataService.GetRealTimeDataItem().DoEquipSetItem(EquipNo, SetNo, strValue, strUser, bShowDlg, Instance_GUID, requestId);
        }
    }
}
