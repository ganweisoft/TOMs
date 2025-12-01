//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using IoTCenterHost.AppServices.Domain.DO.RemoteStatus;
using IoTCenterHost.AppServices.Domain.DO.RemoteValue;
using IoTCenterHost.Core.Abstraction.AppServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenGWDataCenter.Model;

namespace IoTCenterHost.AppServices.AppServices
{
    public class EquipAlarmAppServiceImpl : BaseAppServiceImpl, IEquipAlarmAppService
    {

        private readonly IRemoteStatusRepository _remoteStatusRepository;
        private readonly IRemoteValueRepository _remoteValueRepository;
        private readonly ILogger<EquipAlarmAppServiceImpl> _logger;


        public EquipAlarmAppServiceImpl(IRemoteStatusRepository remoteStatusRepository, IRemoteValueRepository remoteValueRepository, IHttpContextAccessor contextAccessor, ILogger<EquipAlarmAppServiceImpl> logger) : base(contextAccessor, logger)
        {
            _remoteStatusRepository = remoteStatusRepository;
            _remoteValueRepository = remoteValueRepository;
            _logger = logger;
        }

        public bool Confirm2NormalState(int iEqpNo, string sYcYxType, int iYcYxNo)
        {
            EquipItem Item = StationItem.GetEquipItemFromEquipNo(iEqpNo);
            if (Item == null || Item.ICommunication == null)
                return false;
            return Item.ICommunication.Confirm2NormalState(sYcYxType, iYcYxNo);
        }

        public bool SetNoAlarm(int eqpno, string type, int ycyxno)
        {
            if (type == "C")
            {
                DataCenter.GetEquipItem(eqpno).YCItemDict[ycyxno].SetNoAlarm();
            }
            if (type == "X")
            {
                DataCenter.GetEquipItem(eqpno).YXItemDict[ycyxno].SetNoAlarm();
            }
            return true;
        }

        public bool SetWuBao(int eqpno, string type, int ycyxno)
        {
            if (type == "C")
            {
                _remoteValueRepository.SetWuBao(eqpno, ycyxno);
            }
            if (type == "X")
            {
                _remoteStatusRepository.SetWuBao(eqpno, ycyxno);

            }
            return true;
        }
    }
}
