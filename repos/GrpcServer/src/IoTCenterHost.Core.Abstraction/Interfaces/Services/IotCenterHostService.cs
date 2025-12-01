//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.Core.Abstraction.BaseModels;
using IoTCenterHost.Core.Abstraction.EnumDefine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTCenterHost.Core.Abstraction.Services
{
    public interface IotCenterHostService
    {

        bool Login(string User, string Pwd, GwClientType CT, bool bRetry = false);
        string LoginEx(string User, string Pwd, GwClientType CT);

        object GetYCValue(int iEquipNo, int iYcpNo);

        Dictionary<int, object> GetYCValueDictFromEquip(int iEquipNo);

        bool GetYCAlarmState(int iEquipNo, int iYcpNo);

        Dictionary<int, bool> GetYCAlarmStateDictFromEquip(int iEquipNo);

        object GetYXValue(int iEquipNo, int iYxpNo);

        Dictionary<int, string> GetYXValueDictFromEquip(int iEquipNo);

        bool GetYXAlarmState(int iEquipNo, int iYxpNo);


        string GetYXEvt01(int iEquipNo, int iYxpNo);


        string GetYXEvt10(int iEquipNo, int iYxpNo);

        Dictionary<int, bool> GetYXAlarmStateDictFromEquip(int iEquipNo);

        byte[] GetCurveData(DateTime d, int eqpno, int ycno);
        byte[] GetCurveData1(DateTime d, int eqpno, int ycyxno, string type);

        void SetHistoryStorePeriod(int period);

        int GetHistoryStorePeriod();

        void SetEquipDebug(int iEquipNo, bool bFlag);

        bool GetEquipDebugState(int iEquipNo);

        void SetParm(int EquipNo, string strCMD1, string strCMD2, string strCMD3, string strUser);

        void SetParm_1(int EquipNo, string strCMD1, string strCMD2, string strCMD3, string strUser, bool bShowDlg);

        void SetParm1(int EquipNo, int SetNo, string strUser);

        void SetParm1_1(int EquipNo, int SetNo, string strValue, string strUser, bool bShowDlg, string requestId = "");

        void SetParm2(int EquipNo, string strCMD1, string strCMD2, string strCMD3, string strType, string strUser);

        void SetParm2_1(int EquipNo, string strCMD1, string strCMD2, string strCMD3, string strType, string strUser, bool bShowDlg);

        void DoSetParmFromString(string csParmStr);
        bool HaveYCP(int EquipNo);
        bool HaveYXP(int EquipNo);
        bool HaveSet(int EquipNo);
        bool HaveHistoryCurve(int EquipNo, int YCPNo);

        string GetEquipListStr();

        string GetYCPListStr(int iEquipNo);

        string GetYXPListStr(int iEquipNo);

        string GetSetListStr(int iEquipNo);
        string GetYCAlarmComments(int iEqpNo, int iYCPNo);

        string GetYXAlarmComments(int iEqpNo, int iYXPNo);

        string GetVersionInfo();

        void GetTotalRTYCItemData1();
        void GetTotalRTYXItemData1();

        void GetChangedRTYCItemData1();

        void GetChangedRTYXItemData1();
        void GetTotalRTEquipItemData1();
        void GetChangedRTEquipItemData1();
        int[] GetAddRTEquipItemData();

        int[] GetDelRTEquipItemData();

        int[] GetEditRTEquipItemData();

        void GetAddRealEventItem1();

        void GetDelRealEventItem1();

        void DeleteDebugInfo(int iEquipNo);

        void ConfirmedRealTimeEventItem(WcfRealTimeEventItem item);


        Task SetParmEx(int EquipNo, string strCMD1, string strCMD2, string strCMD3, string strUser, string requestId, Action<string> action);

        string DoEquipSetItem(int EquipNo, int SetNo, string strValue, string strUser, bool bShowDlg, string Instance_GUID, string requestId = "");



        void ResetEquips();

        void ResetEquipmentLinkage();
        void ResetProcTimeManage();

        void ResetGWDataRecordItems();

        void ResetDelayActionPlan();


        void SetEquipNm(int EquipNo, string Nm);
        void SetYcpNm(int EquipNo, int YcpNo, string Nm);

        void SetYxpNm(int EquipNo, int YxpNo, string Nm);

        void MResetYcYxNo(int EquipNo, string sType, int YcYxNo);



        string GetPropertyFromPropertyService(string PropertyName, string NodeName, string DefaultValue);

        void SetPropertyToPropertyService(string PropertyName, string NodeName, string Value);

        bool SetWuBao(int eqpno, string type, int ycyxno);

        bool SetNoAlarm(int eqpno, string type, int ycyxno);

        bool Confirm2NormalState(int iEqpNo, string sYcYxType, int iYcYxNo);



        void AddChangedEquip(GrpcChangedEquip EqpList);

        void AddChangedEquipList(IEnumerable<Core.Abstraction.BaseModels.GrpcChangedEquip> changedEquips);

        Task<List<GrpcMyCurveData>> GetChangedDataFromCurveAsync(DateTime bgn, DateTime end, int stano, int eqpno, int ycyxno, string type);

        Task<List<GrpcMyCurveData>> GetDoubleCurveData(DateTime bgn, DateTime end, int stano, int eqpno, int ycyxno, string type);

        Dictionary<int, GrpcEquipState> GetEquipStateDict(IEnumerable<int> equipList);

        Dictionary<int, GrpcEquipState> GetEquipStateDict();

        GrpcEquipState GetEquipStateFromEquipNo(int iEquipNo);

    }

}
