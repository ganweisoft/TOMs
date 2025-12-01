// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.Core.Abstraction;
using IoTCenterHost.Core.Abstraction.AppServices;
using IoTCenterHost.Core.Abstraction.BaseModels;
using IoTCenterHost.Core.Abstraction.EnumDefine;
using IoTCenterHost.Core.Abstraction.Interfaces.AppServices;
using IoTCenterHost.Core.Abstraction.IotModels;
using IoTCenterHost.Core.Abstraction.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTCenterHost.Proxy
{
    public class AlarmCenterServiceClientImpl: IotCenterHostService
    {
        private readonly IConnectService _connectService;
        private readonly IEquipBaseClientAppService _equipBaseAppService;
        private readonly IEquipAlarmAppService _equipAlarmAppService;
        private readonly IAlarmEventClientAppService _alarmEventAppService;
        private readonly ICommandAppService _commandAppService;

        private readonly ICurveClientAppService _curveAppService;
        private readonly IoTCenterAppService _alarmCenterAppService;
        private readonly IYXClientAppService _yXAppService;
        private readonly IYCClientAppService _yCAppService;
        private readonly IUserAppService _userAppService;
        public AlarmCenterServiceClientImpl(IConnectService connectService,
            IEquipBaseClientAppService equipBaseAppService,
            IEquipAlarmAppService equipAlarmAppService,
            IAlarmEventClientAppService alarmEventAppService,
            ICommandAppService commandAppService,
            ICurveClientAppService curveAppService,
           IoTCenterAppService alarmCenterAppService,
           IYXClientAppService yXAppService,
           IYCClientAppService yCAppService,
           IUserAppService userAppService
            )
        {
            _connectService = connectService;
            _equipBaseAppService = equipBaseAppService;
            _equipAlarmAppService = equipAlarmAppService;
            _alarmEventAppService = alarmEventAppService;
            _commandAppService = commandAppService;
            _curveAppService = curveAppService;
            _alarmCenterAppService = alarmCenterAppService;
            _yXAppService = yXAppService;
            _yCAppService = yCAppService;
            _userAppService = userAppService;
        }

        public void AddChangedEquip(GrpcChangedEquip EqpList)
        {
            _equipBaseAppService.AddChangedEquip(EqpList);
        }

        public bool Confirm2NormalState(int iEqpNo, string sYcYxType, int iYcYxNo)
        {
            return _equipAlarmAppService.Confirm2NormalState(iEqpNo, sYcYxType, iYcYxNo);
        }

        public void ConfirmedRealTimeEventItem(WcfRealTimeEventItem item)
        {
            _alarmEventAppService.ConfirmedRealTimeEventItem(item);

        }

        public void DeleteDebugInfo(int iEquipNo)
        {
            throw new NotImplementedException();
        }


        public void DoSetParmFromString(string csParmStr)
        {
            _commandAppService.DoSetParmFromString(csParmStr);

        }

        public void FirstGetRealEventItem1(Action<WcfRealTimeEventItem> action)
        {
        }


        public void GetAddRealEventItem1()
        {
            _alarmEventAppService.GetAddRealEventItem1();
        }

        public int[] GetAddRTEquipItemData()
        {
            return _equipBaseAppService.GetAddRTEquipItemData();
        }

        public Task<List<GrpcMyCurveData>> GetChangedDataFromCurveAsync(DateTime bgn, DateTime end, int stano, int eqpno, int ycyxno, string type)
        {
            return _curveAppService.GetChangedDataFromCurveAsync(bgn, end, stano, eqpno, ycyxno, type);
        }

        public void GetChangedRTEquipItemData1()
        {
            _equipBaseAppService.GetChangedRTEquipItemData1();
        }

        public void GetChangedRTYCItemData1()
        {
            _equipBaseAppService.GetChangedRTYCItemData1();
        }

        public void GetChangedRTYXItemData1()
        {
            _equipBaseAppService.GetChangedRTYXItemData1();
        }

        public byte[] GetCurveData(DateTime d, int eqpno, int ycno)
        {
            return _curveAppService.GetCurveData(d, eqpno, ycno);
        }

        public byte[] GetCurveData1(DateTime d, int eqpno, int ycyxno, string type)
        {
            return _curveAppService.GetCurveData1(d, eqpno, ycyxno, type);
        }

        public Task<List<GrpcMyCurveData>> GetDataFromCurve(List<DateTime> DTList, int stano, int eqpno, int ycyxno, string type)
        {
            return _curveAppService.GetDataFromCurve(DTList, stano, eqpno, ycyxno, type);
        }

        public void GetDelRealEventItem1()
        {
            _alarmEventAppService.GetDelRealEventItem1();
        }

        public int[] GetDelRTEquipItemData()
        {
            return _equipBaseAppService.GetDelRTEquipItemData();
        }

        public int[] GetEditRTEquipItemData()
        {
            return _equipBaseAppService.GetEditRTEquipItemData();
        }

        public bool GetEquipDebugState(int iEquipNo)
        {
            return _equipBaseAppService.GetEquipDebugState(iEquipNo);
        }

        public string GetEquipListStr()
        {
            return _equipBaseAppService.GetEquipListStr();

        }

        public Dictionary<int, GrpcEquipState> GetEquipStateDict()
        {
            return _equipBaseAppService.GetEquipStateDict();
        }

        public GrpcEquipState GetEquipStateFromEquipNo(int iEquipNo)
        {
            return _equipBaseAppService.GetEquipStateFromEquipNo(iEquipNo);
        }



        public string GetPropertyFromPropertyService(string PropertyName, string NodeName, string DefaultValue)
        {
            return _alarmCenterAppService.GetPropertyFromPropertyService(PropertyName, NodeName, DefaultValue);
        }


        public string GetSetListStr(int iEquipNo)
        {
            return _commandAppService.GetSetListStr(iEquipNo);
        }



        public void GetTotalRTEquipItemData1()
        {
            _equipBaseAppService.GetTotalRTEquipItemData1();
        }

        public void GetTotalRTYCItemData1()
        {
            _equipBaseAppService.GetTotalRTYCItemData1();
        }

        public void GetTotalRTYXItemData1()
        {
            _equipBaseAppService.GetTotalRTYXItemData1();
        }

       

        public string GetVersionInfo()
        {
            return _alarmCenterAppService.GetVersionInfo();
        }

        public string GetYCAlarmComments(int iEqpNo, int iYCPNo)
        {
            return _yCAppService.GetYCAlarmComments(iEqpNo, iYCPNo);
        }

        public bool GetYCAlarmState(int iEquipNo, int iYcpNo)
        {
            return _yCAppService.GetYCAlarmState(iEquipNo, iYcpNo);
        }

        public Dictionary<int, bool> GetYCAlarmStateDictFromEquip(int iEquipNo)
        {
            return _yCAppService.GetYCAlarmStateDictFromEquip(iEquipNo);
        }

        public PaginationData GetYCPListByEquipNo(Pagination pagination)
        {
            return _yCAppService.GetYCPListByEquipNo(pagination);
        }

        public string GetYCPListStr(int iEquipNo)
        {
            return _yCAppService.GetYCPListStr(iEquipNo);
        }

        public object GetYCValue(int iEquipNo, int iYcpNo)
        {
            return _yCAppService.GetYCValue(iEquipNo, iYcpNo);
        }

        public Dictionary<int, object> GetYCValueDictFromEquip(int iEquipNo)
        {
            return _yCAppService.GetYCValueDictFromEquip(iEquipNo);
        }

        public string GetYXAlarmComments(int iEqpNo, int iYXPNo)
        {
            return _yXAppService.GetYXAlarmComments(iEqpNo, iYXPNo);
        }

        public bool GetYXAlarmState(int iEquipNo, int iYxpNo)
        {
            return _yXAppService.GetYXAlarmState(iEquipNo, iYxpNo);
        }

        public Dictionary<int, bool> GetYXAlarmStateDictFromEquip(int iEquipNo)
        {
            return _yXAppService.GetYXAlarmStateDictFromEquip(iEquipNo);
        }

        public string GetYXEvt01(int iEquipNo, int iYxpNo)
        {
            return _yXAppService.GetYXEvt01(iEquipNo, iYxpNo);
        }

        public string GetYXEvt10(int iEquipNo, int iYxpNo)
        {
            return _yXAppService.GetYXEvt10(iEquipNo, iYxpNo);
        }

        public PaginationData GetYXPListByEquipNo(Pagination pagination)
        {
            return _yXAppService.GetYXPListByEquipNo(pagination);
        }

        public string GetYXPListStr(int iEquipNo)
        {
            return _yXAppService.GetYXPListStr(iEquipNo);
        }

        public object GetYXValue(int iEquipNo, int iYxpNo)
        {
            return _yXAppService.GetYXValue(iEquipNo, iYxpNo);
        }

        public Dictionary<int, string> GetYXValueDictFromEquip(int iEquipNo)
        {
            return _yXAppService.GetYXValueDictFromEquip(iEquipNo);
        }

        public bool HaveHistoryCurve(int EquipNo, int YCPNo)
        {
            return _curveAppService.HaveHistoryCurve(EquipNo, YCPNo);
        }

        public bool HaveSet(int EquipNo)
        {
            return _commandAppService.HaveSet(EquipNo);
        }

        public bool HaveYCP(int EquipNo)
        {
            return _yCAppService.HaveYCP(EquipNo);
        }

        public bool HaveYXP(int EquipNo)
        {
            return _yXAppService.HaveYXP(EquipNo);
        }

        public bool Login(string User, string Pwd, GwClientType CT, bool bRetry = false)
        {
            _connectService.Login(User, Pwd, CT);
            return true;
        }

        public string LoginEx(string User, string Pwd, GwClientType CT)
        {
            return _connectService.Login(User, Pwd, CT);
        }
        public void MResetYcYxNo(int EquipNo, string sType, int YcYxNo)
        {
            _alarmCenterAppService.MResetYcYxNo(EquipNo, sType, YcYxNo);
        }


        public void ResetDelayActionPlan()
        {
            _alarmCenterAppService.ResetDelayActionPlan();
        }

        public void ResetEquipmentLinkage()
        {
            _equipBaseAppService.ResetEquipmentLinkage();
        }

        public void ResetEquips()
        {
            _equipBaseAppService.ResetEquips();
        }

        public void ResetEquips(List<int> list)
        {
            _equipBaseAppService.ResetEquips(list);

        }

        public void ResetGWDataRecordItems()
        {
            _alarmCenterAppService.ResetGWDataRecordItems();
        }

        public void ResetProcTimeManage()
        {
            _alarmCenterAppService.ResetProcTimeManage();
        }

        public void SetEquipDebug(int iEquipNo, bool bFlag)
        {
            _equipBaseAppService.SetEquipDebug(iEquipNo, bFlag);
        }

        public void SetEquipNm(int EquipNo, string Nm)
        {
            _equipBaseAppService.SetEquipNm(EquipNo, Nm);
        }


        public bool SetNoAlarm(int eqpno, string type, int ycyxno)
        {
            return _equipAlarmAppService.SetNoAlarm(eqpno, type, ycyxno);
        }

        public void SetParm(int EquipNo, string strCMD1, string strCMD2, string strCMD3, string strUser)
        {
            _commandAppService.SetParm(EquipNo, strCMD1, strCMD2, strCMD3, strUser);
        }
     
        public void SetParm1(int EquipNo, int SetNo, string strUser)
        {
            _commandAppService.SetParm1(EquipNo, SetNo, strUser);
        }

        public void SetParm1_1(int EquipNo, int SetNo, string strValue, string strUser, bool bShowDlg, string requestId = "")
        {
            _commandAppService.SetParm1_1(EquipNo, SetNo, strValue, strUser, bShowDlg, requestId);
        }

        public void SetParm2(int EquipNo, string strCMD1, string strCMD2, string strCMD3, string strType, string strUser)
        {
            _commandAppService.SetParm2(EquipNo, strCMD1, strCMD2, strCMD3, strType, strUser);
        }

        public void SetParm2_1(int EquipNo, string strCMD1, string strCMD2, string strCMD3, string strType, string strUser, bool bShowDlg)
        {
            _commandAppService.SetParm2_1(EquipNo, strCMD1, strCMD2, strCMD3, strType, strUser, bShowDlg);
        }
        public void SetPropertyToPropertyService(string PropertyName, string NodeName, string Value)
        {
            _alarmCenterAppService.SetPropertyToPropertyService(PropertyName, NodeName, Value);
        }

        public bool SetWuBao(int eqpno, string type, int ycyxno)
        {
            return _equipAlarmAppService.SetWuBao(eqpno, type, ycyxno);
        }

        public void SetYcpNm(int EquipNo, int YcpNo, string Nm)
        {
            _yCAppService.SetYcpNm(EquipNo, YcpNo, Nm);
        }

        public void SetYxpNm(int EquipNo, int YxpNo, string Nm)
        {
            _yXAppService.SetYxpNm(EquipNo, YxpNo, Nm);
        }

        public void SetHistoryStorePeriod(int period)
        {
            _curveAppService.SetHistoryStorePeriod(period);
        }

        public int GetHistoryStorePeriod()
        {
            return _curveAppService.GetHistoryStorePeriod();
        }

        public Task<List<GrpcMyCurveData>> GetDoubleCurveData(DateTime bgn, DateTime end, int stano, int eqpno, int ycyxno, string type)
        {
            return _curveAppService.GetCurveDataAsync(bgn, end, stano, eqpno, ycyxno, type);
        }

        public Dictionary<int, GrpcEquipState> GetEquipStateDict(IEnumerable<int> equipList)
        {
            return _equipBaseAppService.GetEquipStateDict(equipList);
        }

        public void AddChangedEquipList(IEnumerable<GrpcChangedEquip> changedEquips)
        {
            _equipBaseAppService.AddChangedEquipList(changedEquips);
        }


        public async Task SetParmEx(int EquipNo, string strCMD1, string strCMD2, string strCMD3, string strUser, string requestId, Action<string> action)
        {
            await _commandAppService.SetParmExAsync(EquipNo, strCMD1, strCMD2, strCMD3, strUser, requestId, action);
        }

        public string DoEquipSetItem(int EquipNo, int SetNo, string strValue, string strUser, bool bShowDlg, string Instance_GUID, string requestId = "")
        {
            return _commandAppService.DoEquipSetItem(EquipNo, SetNo, strValue, strUser, bShowDlg, "", requestId);
        }

        public void SetParm_1(int EquipNo, string strCMD1, string strCMD2, string strCMD3, string strUser, bool bShowDlg)
        {
            throw new NotImplementedException();
        }
    }
}
