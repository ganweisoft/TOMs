//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using IoTCenterHost.AppServices.Domain.DO.Equip;
using IoTCenterHost.AppServices.Domain.VO.Message;
using IoTCenterHost.AppServices.Interfaces;
using IoTCenterHost.Core.Abstraction.IotModels;
using IoTCenterHost.Core.Extension;
using IoTCenterHost.Core.ServerInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenGWDataCenter.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IoTCenterHost.AppServices.AppServices
{
    public class EquipBaseAppServiceImpl : BaseAppServiceImpl, IEquipBaseServerService
    {
        private readonly IEquipRepository _equipRepository;
        private readonly IMessageService _messageService;
        private readonly IotRealTimeDataService _iotRealTimeDataService;
        private readonly Timer _registEquipTimer;
        private readonly ILogger<EquipBaseAppServiceImpl> _logger;

        public EquipBaseAppServiceImpl(IEquipRepository equipRepository,
                                       IotRealTimeDataService iotRealTimeDataService,
                                       IMessageService messageService,
                                       IHttpContextAccessor contextAccessor,
                                       ILogger<EquipBaseAppServiceImpl> logger) : base(contextAccessor, logger)
        {
            _equipRepository = equipRepository;
            _messageService = messageService;
            _iotRealTimeDataService = iotRealTimeDataService;
            _logger = logger;
        }

        public void AddChangedEquip(GWDataCenter.ChangedEquip EqpList, bool addSingle = true)
        {
            if (addSingle)
                BindEquipEvent(EqpList);
        }

        private void BindEquipEvent(GWDataCenter.ChangedEquip EqpList)
        {
            if (EqpList.State == ChangedEquipState.Add)
                _iotRealTimeDataService.GetRealTimeDataItem().BindEventToNewEquip(EqpList.iEqpNo);
        }

        public void DeleteDebugInfo(int iEquipNo)
        {
            _messageService.DeleteDebugInfo(iEquipNo);
        }

        public void GetChangedRTEquipItemData()
        {
            _iotRealTimeDataService.GetIotRealTimeData(base.ConnectId, GetLoginUser()).GetChangedRTEquipItemData();

        }

        public void GetChangedRTEquipItemData1()
        {
            _iotRealTimeDataService.GetIotRealTimeData(base.ConnectId, GetLoginUser()).GetChangedRTEquipItemData();
        }

        public int[] GetEditRTEquipItemData()
        {
            var realTimeData = _iotRealTimeDataService.GetIotRealTimeData(base.ConnectId, GetLoginUser());
            int[] bs = realTimeData.EditEquipList.ToArray();
            realTimeData.EditEquipList.Clear();
            return bs;

        }

        public bool GetEquipDebugState(int iEquipNo)
        {
            return (bool)StationItem.GetEquipItemFromEquipNo(iEquipNo).IsDebug;

        }

        public string GetEquipListStr()
        {
            return _equipRepository.GetEquipListStr();
        }

        public Dictionary<int, GWDataCenter.EquipState> GetEquipStateDict()
        {
            return _equipRepository.GetEquipStateDict();

        }

        public GWDataCenter.EquipState GetEquipStateFromEquipNo(int iEquipNo)
        {
            return _equipRepository.GetEquipStateFromEquipNo(iEquipNo);
        }

        public byte[] GetEquipTreeXMLFile()
        {
            string sPath = System.IO.Path.Combine(General.GetApplicationRootPath(), @"\data\AlarmCenter\GWEquipTree.xml");
            byte[] fileBytes = new byte[new FileInfo(sPath).Length];
            using (var fileStream = new FileStream(sPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                fileStream.Read(fileBytes, 0, (int)(new FileInfo(sPath).Length));
            }
            return fileBytes;
        }

        public int[] GetAddRTEquipItemData()
        {
            return _iotRealTimeDataService.GetIotRealTimeData(base.ConnectId, GetLoginUser()).GetAddRTEquipItemData();
        }

        public void GetTotalRTEquipItemData()
        {
            var realTimeData = _iotRealTimeDataService.GetIotRealTimeData(base.ConnectId, GetLoginUser());
            realTimeData.GetTotalRTEquipItemData(null);
        }

        public void GetTotalRTEquipItemData1()
        {
            var realTimeData = _iotRealTimeDataService.GetIotRealTimeData(base.ConnectId, GetLoginUser());
            realTimeData.GetTotalRTEquipItemData(null);
        }

        public void ResetEquipmentLinkage()
        {
            EquipmentLinkage.bReset = true;
        }

        public void ResetEquips()
        {
            StationItem.UpdateMainDataTable();
            Task.Factory.StartNew(delegate
            {
                try
                {
                }
                catch (Exception)
                {
                }
            });
        }

        public void ResetEquips(List<int> list)
        {
            Task.Factory.StartNew(delegate
            {
                StationItem.DoHaveEquipReset(list);
            });
        }
        public void SetEquipDebug(int iEquipNo, bool bFlag)
        {
            StationItem.GetEquipItemFromEquipNo(iEquipNo).IsDebug = bFlag;
        }

        public void SetEquipNm(int EquipNo, string Nm)
        {
            _equipRepository.SetEquipNmAsync(EquipNo, Nm);
        }

        public int[] GetDelRTEquipItemData()
        {
            var realTimeData = _iotRealTimeDataService.GetIotRealTimeData(base.ConnectId, GetLoginUser());
            return new List<int>().ToArray();
        }

        public void GetTotalRTYCItemData1()
        {
            throw new NotImplementedException();
        }

        public void GetTotalRTYXItemData1()
        {
            throw new NotImplementedException();
        }

        public void GetChangedRTYCItemData1()
        {
            throw new NotImplementedException();
        }

        public void GetChangedRTYXItemData1()
        {
            throw new NotImplementedException();
        }
        [Obsolete]
        public void GetTotalEquipData(bool isDynamic)
        {
            var realTimeData = _iotRealTimeDataService.GetIotRealTimeData(base.ConnectId, GetLoginUser());
            realTimeData.GetTotalRTEquipItemData(null);
        }
        public IEnumerable<EquipItem> GetTotalEquipDataEx(bool isDynamic)
        {
            var realTimeData = _iotRealTimeDataService.GetIotRealTimeData(base.ConnectId);
            return realTimeData.GetTotalRTEquipItemDataEx();
        }

        public Dictionary<int, GWDataCenter.EquipState> GetEquipStateDict(IEnumerable<int> equipList)
        {
            return _equipRepository.GetEquipStateDict(equipList);
        }

        public void AddChangedEquipList(IEnumerable<GWDataCenter.ChangedEquip> changedEquips)
        {
            foreach (var changedEquip in changedEquips)
            {
                AddChangedEquip(changedEquip, false);
            }
            ResetEquips(changedEquips.Select(u => u.iEqpNo).ToList());
            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                changedEquips.ToList().ForEach(item =>
                {
                    BindEquipEvent(item);
                });
            });
        }

        public PaginationData GetEquipDict(Pagination pagination)
        {
            var realTimeData = _iotRealTimeDataService.GetIotRealTimeData(base.ConnectId);
            return new PaginationData
            {
                Data = realTimeData.CreateEquipDict().ToJson()
            };
        }
    }
}
