//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Threading.Tasks;

namespace IoTCenterHost.Core.Abstraction.AppServices
{
    public interface ICommandAppService
    {
        #region 新增、修改、删除 
        void SetParm(int EquipNo, string strCMD1, string strCMD2, string strCMD3, string strUser);

        Task SetParmExAsync(int EquipNo, string strCMD1, string strCMD2, string strCMD3, string strUser, string requestId, Action<string> action);



        void SetParm(int iEquipNo, string strCMD1, string strCMD2, string strCMD3, string strUser, string Instance_GUID);
        void SetParm_1(int EquipNo, string strCMD1, string strCMD2, string strCMD3, string strUser, bool bShowDlg);
        void SetParm1(int EquipNo, int SetNo, string strUser);

        void SetParm1_1(int EquipNo, int SetNo, string strValue, string strUser, bool bShowDlg, string requestId);
        void SetParm2(int EquipNo, string strCMD1, string strCMD2, string strCMD3, string strType, string strUser);
        void SetParm2_1(int EquipNo, string strCMD1, string strCMD2, string strCMD3, string strType, string strUser, bool bShowDlg);
        void DoSetParmFromString(string csParmStr);


        string DoEquipSetItem(int EquipNo, int SetNo, string strValue, string strUser, bool bShowDlg, string Instance_GUID, string requestId = "");


        #endregion

        #region 查询  
        bool HaveSet(int EquipNo);
        string GetSetListStr(int iEquipNo);
        #endregion 
    }
}
